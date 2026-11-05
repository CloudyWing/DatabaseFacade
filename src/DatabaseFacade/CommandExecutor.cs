using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static CloudyWing.DatabaseFacade.FacadeConfiguration;

namespace CloudyWing.DatabaseFacade {
    /// <summary>
    /// Provides a fluent facade for executing ADO.NET commands with shared configuration and parameter handling.
    /// </summary>
    public sealed class CommandExecutor : IDisposable {
        private const int MaxCachedParameterNameRegexCount = 1024;
        private static readonly ConcurrentDictionary<string, Regex> ParameterNameRegexCache = new(StringComparer.Ordinal);
        private readonly List<OpenReader> openReaders = [];
        private IDbTransaction? transaction;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutor" /> class by using the configured defaults.
        /// </summary>
        /// <param name="keepConnection">
        /// Overrides the configured keep-connection behavior when a value is supplied.
        /// </param>
        public CommandExecutor(bool? keepConnection = null)
            : this(null, null, keepConnection) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutor" /> class by using the configured provider factory.
        /// </summary>
        /// <param name="connStr">
        /// The connection string to use. When <see langword="null" />, the configured default connection string is used.
        /// </param>
        /// <param name="keepConnection">
        /// Overrides the configured keep-connection behavior when a value is supplied.
        /// </param>
        public CommandExecutor(string? connStr, bool? keepConnection = null)
            : this(null, connStr, keepConnection) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutor" /> class.
        /// </summary>
        /// <param name="providerFactory">
        /// The provider factory used to create connections and commands. When <see langword="null" />, the configured default factory is used.
        /// </param>
        /// <param name="connStr">
        /// The connection string to use. When <see langword="null" />, the configured default connection string is used.
        /// </param>
        /// <param name="keepConnection">
        /// Overrides the configured keep-connection behavior when a value is supplied.
        /// </param>
        public CommandExecutor(DbProviderFactory? providerFactory, string? connStr, bool? keepConnection = null) {
            DbProviderFactory = providerFactory ?? DefaultDbProviderFactory;
            ConnectionString = connStr ?? DefaultConnectionString;
            KeepConnection = keepConnection ?? DefaultKeepConnection;
            Parameters = new ParameterCollection(this);

            Initialize();
        }

        /// <summary>
        /// Gets the database provider factory used to create connections and commands.
        /// </summary>
        public DbProviderFactory? DbProviderFactory { get; private set; }

        /// <summary>
        /// Gets the connection string used when opening the underlying connection.
        /// </summary>
        public string? ConnectionString { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the underlying connection should remain open after command execution.
        /// </summary>
        public bool KeepConnection { get; private set; }

        /// <summary>
        /// Gets the current connection instance when one has been created.
        /// </summary>
        public IDbConnection? Connection { get; private set; }

        /// <summary>
        /// Gets the active transaction, or <see langword="null" /> when no usable transaction exists.
        /// </summary>
        public IDbTransaction? Transaction {
            get {
                if (transaction?.Connection is null) {
                    transaction = null;
                }

                return transaction;
            }
            private set => transaction = value;
        }

        /// <summary>
        /// Gets or sets the SQL statement, stored procedure name, or other provider-specific command text to execute.
        /// </summary>
        public string? CommandText { get; set; }

        /// <summary>
        /// Gets or sets the command timeout, in seconds.
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        /// Gets or sets the command type used when executing <see cref="CommandText" />.
        /// </summary>
        public CommandType CommandType { get; set; }

        /// <summary>
        /// Gets the parameters that will be applied to the next command execution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When <see cref="CommandType" /> is <see cref="System.Data.CommandType.Text" />, only parameters whose names appear in
        /// <see cref="CommandText" /> are added to the command. A value that implements <see cref="IEnumerable" />
        /// (except <see cref="string" />, <see cref="byte" /> arrays, and <see cref="char" /> arrays) is expanded into
        /// <c>(@{ParameterNamePrefix}_{name}_0, @{ParameterNamePrefix}_{name}_1, ...)</c> for <c>IN</c> clauses.
        /// An empty collection is replaced with <c>(NULL)</c>, so <c>IN @name</c> matches no rows and
        /// <c>NOT IN @name</c> also matches no rows, because comparisons with <c>NULL</c> are never true.
        /// </para>
        /// <para>
        /// For other command types, every parameter is added as is and collection values are not expanded.
        /// </para>
        /// </remarks>
        public ParameterCollection Parameters { get; }

        /// <summary>
        /// Gets or sets the callback invoked before this executor creates a command, so the queued parameters and SQL text can be inspected.
        /// </summary>
        /// <remarks>
        /// This callback runs after <see cref="FacadeConfiguration.OnCommandCreating" />.
        /// </remarks>
        public Action<ParameterCollection, string?>? CommandCreating { get; set; }

        /// <summary>
        /// Gets or sets the callback invoked after this executor has created and populated a command.
        /// </summary>
        /// <remarks>
        /// This callback runs after <see cref="FacadeConfiguration.OnCommandCreated" />.
        /// </remarks>
        public Action<IDbCommand>? CommandCreated { get; set; }

        /// <summary>
        /// Sets the command text and optionally updates the command type.
        /// </summary>
        /// <param name="commadText">The command text to execute.</param>
        /// <param name="commandType">
        /// The command type to apply. When <see langword="null" />, the existing <see cref="CommandType" /> value is preserved.
        /// </param>
        /// <returns>The current <see cref="CommandExecutor" /> instance.</returns>
        public CommandExecutor SetCommandText(string commadText, CommandType? commandType = null) {
            CommandText = commadText;
            if (commandType.HasValue) {
                CommandType = commandType.Value;
            }

            return this;
        }

        /// <summary>
        /// Sets the command timeout.
        /// </summary>
        /// <param name="second">The timeout, in seconds.</param>
        /// <returns>The current <see cref="CommandExecutor" /> instance.</returns>
        public CommandExecutor SetCommandTimeout(int second) {
            CommandTimeout = second;

            return this;
        }

        /// <summary>
        /// Executes the current command and returns a data reader.
        /// </summary>
        /// <param name="thenReset">Specifies which command state values are reset after the command succeeds.</param>
        /// <param name="behavior">The <see cref="CommandBehavior" /> flags applied to the reader.</param>
        /// <returns>
        /// An <see cref="IDataReader" /> for the executed command. The caller is responsible for disposing the returned reader.
        /// </returns>
        /// <remarks>
        /// <para>
        /// When <see cref="KeepConnection" /> is <see langword="false" />, <see cref="CommandBehavior.CloseConnection" /> is added
        /// and the connection is handed over to the returned reader: <see cref="Connection" /> becomes <see langword="null" />,
        /// later commands open a new connection, and disposing this executor does not close the reader.
        /// </para>
        /// <para>
        /// The underlying <see cref="DbCommand" /> is disposed after the reader has been closed, the next time this executor
        /// creates a command or when this executor is disposed.
        /// </para>
        /// </remarks>
        public IDataReader CreateDataReader(
            ResetItems thenReset = ResetItems.All,
            CommandBehavior behavior = CommandBehavior.SequentialAccess) {
            DbConnection connection = BuildConnection();

            if (!KeepConnection) {
                behavior |= CommandBehavior.CloseConnection;
            }

            DbCommand? command = null;
            IReadOnlyList<OutputParameter> outputParameters;
            DbDataReader dataReader;
            try {
                command = CreateCommand(connection, out outputParameters);
                dataReader = command.ExecuteReader(behavior);
            } catch {
                command?.Dispose();
                if (!KeepConnection) {
                    ReleaseConnection(connection);
                }

                throw;
            }

            TrackOpenReader(dataReader, command, outputParameters, connection);
            Initialize(thenReset);

            return dataReader;
        }

        /// <summary>
        /// Executes the current command asynchronously and returns a data reader.
        /// </summary>
        /// <param name="thenReset">Specifies which command state values are reset after the command succeeds.</param>
        /// <param name="behavior">The <see cref="CommandBehavior" /> flags applied to the reader.</param>
        /// <param name="cancellationToken">The token used to cancel the asynchronous operation.</param>
        /// <returns>
        /// A task that returns a <see cref="DbDataReader" /> for the executed command. The caller is responsible for disposing the returned reader.
        /// </returns>
        /// <remarks>
        /// The connection and command lifetime rules are the same as <see cref="CreateDataReader(ResetItems, CommandBehavior)" />.
        /// </remarks>
        public async Task<DbDataReader> CreateDataReaderAsync(
            ResetItems thenReset = ResetItems.All,
            CommandBehavior behavior = CommandBehavior.SequentialAccess,
            CancellationToken cancellationToken = default) {
            DbConnection connection = await BuildConnectionAsync(cancellationToken).ConfigureAwait(false);

            if (!KeepConnection) {
                behavior |= CommandBehavior.CloseConnection;
            }

            DbCommand? command = null;
            IReadOnlyList<OutputParameter> outputParameters;
            DbDataReader dataReader;
            try {
                command = CreateCommand(connection, out outputParameters);
                dataReader = await command.ExecuteReaderAsync(behavior, cancellationToken).ConfigureAwait(false);
            } catch {
                command?.Dispose();
                if (!KeepConnection) {
                    ReleaseConnection(connection);
                }

                throw;
            }

            TrackOpenReader(dataReader, command, outputParameters, connection);
            Initialize(thenReset);

            return dataReader;
        }

        private void TrackOpenReader(
            DbDataReader dataReader,
            DbCommand command,
            IReadOnlyList<OutputParameter> outputParameters,
            DbConnection connection
        ) {
            openReaders.Add(new OpenReader(dataReader, command, outputParameters));

            if (!KeepConnection && ReferenceEquals(Connection, connection)) {
                // 連線改由讀取器的 CommandBehavior.CloseConnection 負責關閉，executor 不再持有，
                // 避免後續指令在讀取器未關閉時共用同一條連線，或 Dispose executor 時關掉讀取器的連線
                Connection = null;
            }
        }

        /// <summary>
        /// Executes the current command and materializes the first result set into a <see cref="DataTable" />.
        /// </summary>
        /// <param name="thenReset">Specifies which command state values are reset after the command succeeds.</param>
        /// <returns>A populated <see cref="DataTable" />.</returns>
        public DataTable CreateDataTable(ResetItems thenReset = ResetItems.All) {
            return ExecuteInternal(
                command => {
                    CommandBehavior behavior = CommandBehavior.SequentialAccess;

                    if (!KeepConnection) {
                        behavior |= CommandBehavior.CloseConnection;
                    }

                    using DbDataReader dataReader = command.ExecuteReader(behavior);
                    DataTable dataTable = new();
                    dataTable.Load(dataReader);
                    return dataTable;
                },
                thenReset);
        }

        /// <summary>
        /// Executes the current command asynchronously and materializes the first result set into a <see cref="DataTable" />.
        /// </summary>
        /// <param name="thenReset">Specifies which command state values are reset after the command succeeds.</param>
        /// <param name="cancellationToken">The token used to cancel the asynchronous operation.</param>
        /// <returns>A task that returns a populated <see cref="DataTable" />.</returns>
        public Task<DataTable> CreateDataTableAsync(
            ResetItems thenReset = ResetItems.All,
            CancellationToken cancellationToken = default) {
            return ExecuteInternalAsync(
                async (command, token) => {
                    CommandBehavior behavior = CommandBehavior.SequentialAccess;

                    if (!KeepConnection) {
                        behavior |= CommandBehavior.CloseConnection;
                    }

                    using DbDataReader dataReader = await command.ExecuteReaderAsync(behavior, token).ConfigureAwait(false);
                    using BufferedDataReader bufferedReader = await BufferedDataReader.LoadAsync(dataReader, token).ConfigureAwait(false);
                    DataTable dataTable = new();
                    dataTable.Load(bufferedReader);
                    return dataTable;
                },
                thenReset,
                cancellationToken);
        }

        /// <summary>
        /// Executes the current command and returns the first column of the first row in the result set.
        /// </summary>
        /// <param name="thenReset">Specifies which command state values are reset after the command succeeds.</param>
        /// <returns>The first column of the first row in the result set.</returns>
        public object? QueryScalar(ResetItems thenReset = ResetItems.All) {
            return ExecuteInternal(command => command.ExecuteScalar(), thenReset);
        }

        /// <summary>
        /// Executes the current command asynchronously and returns the first column of the first row in the result set.
        /// </summary>
        /// <param name="thenReset">Specifies which command state values are reset after the command succeeds.</param>
        /// <param name="cancellationToken">The token used to cancel the asynchronous operation.</param>
        /// <returns>A task that returns the first column of the first row in the result set.</returns>
        public Task<object?> QueryScalarAsync(
            ResetItems thenReset = ResetItems.All,
            CancellationToken cancellationToken = default) {
            return ExecuteInternalAsync(
                (command, token) => command.ExecuteScalarAsync(token),
                thenReset,
                cancellationToken);
        }

        /// <summary>
        /// Executes the current command and returns the number of affected rows.
        /// </summary>
        /// <param name="thenReset">Specifies which command state values are reset after the command succeeds.</param>
        /// <returns>The number of rows affected.</returns>
        public int Execute(ResetItems thenReset = ResetItems.All) {
            return ExecuteInternal(command => command.ExecuteNonQuery(), thenReset);
        }

        /// <summary>
        /// Executes the current command asynchronously and returns the number of affected rows.
        /// </summary>
        /// <param name="thenReset">Specifies which command state values are reset after the command succeeds.</param>
        /// <param name="cancellationToken">The token used to cancel the asynchronous operation.</param>
        /// <returns>A task that returns the number of rows affected.</returns>
        public Task<int> ExecuteAsync(
            ResetItems thenReset = ResetItems.All,
            CancellationToken cancellationToken = default) {
            return ExecuteInternalAsync(
                (command, token) => command.ExecuteNonQueryAsync(token),
                thenReset,
                cancellationToken);
        }

        /// <summary>
        /// Begins a transaction on the current connection.
        /// </summary>
        /// <param name="level">
        /// The isolation level to use. When <see langword="null" />, <see cref="FacadeConfiguration.DefaultIsolationLevel" /> is used.
        /// </param>
        /// <returns>The newly created transaction.</returns>
        /// <exception cref="KeepConnectionRequiredException">
        /// Thrown when <see cref="KeepConnection" /> is <see langword="false" />.
        /// </exception>
        public IDbTransaction BeginTransaction(IsolationLevel? level = null) {
            if (!KeepConnection) {
                throw new KeepConnectionRequiredException($"{nameof(KeepConnection)} must be set to true to use {nameof(BeginTransaction)}.");
            }

            level ??= DefaultIsolationLevel;
            DbConnection connection = BuildConnection();

            Transaction = connection.BeginTransaction(level.Value);
            return Transaction ?? throw new InvalidOperationException("BeginTransaction returned null.");
        }

        /// <summary>
        /// Begins a transaction on the current connection asynchronously.
        /// </summary>
        /// <param name="level">
        /// The isolation level to use. When <see langword="null" />, <see cref="FacadeConfiguration.DefaultIsolationLevel" /> is used.
        /// </param>
        /// <param name="cancellationToken">The token used to cancel the asynchronous operation.</param>
        /// <returns>A task that returns the newly created transaction.</returns>
        /// <remarks>
        /// On .NET 10, <c>DbConnection.BeginTransactionAsync</c> is used.
        /// On .NET Framework and .NET Standard 2.0, which have no asynchronous transaction API, the connection is opened
        /// asynchronously and the transaction is started synchronously.
        /// </remarks>
        /// <exception cref="KeepConnectionRequiredException">
        /// Thrown when <see cref="KeepConnection" /> is <see langword="false" />.
        /// </exception>
        public async Task<DbTransaction> BeginTransactionAsync(
            IsolationLevel? level = null,
            CancellationToken cancellationToken = default
        ) {
            if (!KeepConnection) {
                throw new KeepConnectionRequiredException($"{nameof(KeepConnection)} must be set to true to use {nameof(BeginTransactionAsync)}.");
            }

            IsolationLevel isolationLevel = level ?? DefaultIsolationLevel;
            DbConnection connection = await BuildConnectionAsync(cancellationToken).ConfigureAwait(false);

#if NET
            DbTransaction dbTransaction = await connection.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
#else
            cancellationToken.ThrowIfCancellationRequested();
            DbTransaction dbTransaction = connection.BeginTransaction(isolationLevel);
#endif
            Transaction = dbTransaction;

            return dbTransaction;
        }

        /// <summary>
        /// Commits the active transaction and releases it.
        /// </summary>
        /// <remarks>
        /// After the transaction is committed, it is disposed and <see cref="Transaction" /> becomes <see langword="null" />.
        /// Disposing the returned transaction again from a <see langword="using" /> statement is safe.
        /// </remarks>
        /// <exception cref="InvalidOperationException">No active transaction exists.</exception>
        public void Commit() {
            IDbTransaction activeTransaction = GetActiveTransaction(nameof(Commit));
            activeTransaction.Commit();
            ReleaseTransaction(activeTransaction);
        }

        /// <summary>
        /// Rolls back the active transaction and releases it.
        /// </summary>
        /// <remarks>
        /// After the transaction is rolled back, it is disposed and <see cref="Transaction" /> becomes <see langword="null" />.
        /// </remarks>
        /// <exception cref="InvalidOperationException">No active transaction exists.</exception>
        public void Rollback() {
            IDbTransaction activeTransaction = GetActiveTransaction(nameof(Rollback));
            activeTransaction.Rollback();
            ReleaseTransaction(activeTransaction);
        }

        /// <summary>
        /// Commits the active transaction asynchronously and releases it.
        /// </summary>
        /// <param name="cancellationToken">The token used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <remarks>
        /// On .NET 10, <see cref="DbTransaction" /> asynchronous APIs are used.
        /// On .NET Framework and .NET Standard 2.0, the transaction is committed synchronously.
        /// </remarks>
        /// <exception cref="InvalidOperationException">No active transaction exists.</exception>
        public Task CommitAsync(CancellationToken cancellationToken = default) {
#if NET
            return CompleteTransactionAsync(true, nameof(CommitAsync), cancellationToken);
#else
            return RunSynchronously(Commit, cancellationToken);
#endif
        }

        /// <summary>
        /// Rolls back the active transaction asynchronously and releases it.
        /// </summary>
        /// <param name="cancellationToken">The token used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <remarks>
        /// On .NET 10, <see cref="DbTransaction" /> asynchronous APIs are used.
        /// On .NET Framework and .NET Standard 2.0, the transaction is rolled back synchronously.
        /// </remarks>
        /// <exception cref="InvalidOperationException">No active transaction exists.</exception>
        public Task RollbackAsync(CancellationToken cancellationToken = default) {
#if NET
            return CompleteTransactionAsync(false, nameof(RollbackAsync), cancellationToken);
#else
            return RunSynchronously(Rollback, cancellationToken);
#endif
        }

#if NET
        private async Task CompleteTransactionAsync(bool isCommit, string operation, CancellationToken cancellationToken) {
            IDbTransaction activeTransaction = GetActiveTransaction(operation);
            if (activeTransaction is DbTransaction dbTransaction) {
                if (isCommit) {
                    await dbTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                } else {
                    await dbTransaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                }

                await dbTransaction.DisposeAsync().ConfigureAwait(false);
                Transaction = null;
                return;
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (isCommit) {
                activeTransaction.Commit();
            } else {
                activeTransaction.Rollback();
            }

            ReleaseTransaction(activeTransaction);
        }
#else
        private static Task RunSynchronously(Action action, CancellationToken cancellationToken) {
            // .NET Framework 4.5 沒有 Task.FromException 與 Task.CompletedTask，以 TaskCompletionSource 回傳結果
            TaskCompletionSource<bool> completionSource = new();
            if (cancellationToken.IsCancellationRequested) {
                completionSource.SetCanceled();
                return completionSource.Task;
            }

            try {
                action();
                completionSource.SetResult(true);
            } catch (Exception ex) {
                completionSource.SetException(ex);
            }

            return completionSource.Task;
        }
#endif

        private IDbTransaction GetActiveTransaction(string operation) {
            return Transaction
                ?? throw new InvalidOperationException($"{operation} requires an active transaction started by {nameof(BeginTransaction)} or {nameof(BeginTransactionAsync)}.");
        }

        private void ReleaseTransaction(IDbTransaction completedTransaction) {
            completedTransaction.Dispose();
            Transaction = null;
        }

        /// <summary>
        /// Resets the selected command state values to their configured defaults.
        /// </summary>
        /// <param name="items">The command state values to reset.</param>
        public void Initialize(ResetItems items = ResetItems.All) {
            if ((items & ResetItems.CommandText) == ResetItems.CommandText) {
                CommandText = null;
            }

            if ((items & ResetItems.CommandTimeout) == ResetItems.CommandTimeout) {
                CommandTimeout = DefaultCommandTimeout;
            }

            if ((items & ResetItems.CommandType) == ResetItems.CommandType) {
                CommandType = CommandType.Text;
            }

            if ((items & ResetItems.Parameters) == ResetItems.Parameters) {
                Parameters.Clear();
            }
        }

        /// <summary>
        /// Releases the underlying connection and transaction resources.
        /// </summary>
        public void Dispose() {
            Dispose(disposing: true);
        }

        private T ExecuteInternal<T>(Func<DbCommand, T> executor, ResetItems thenReset) {
            DbConnection connection = BuildConnection();
            try {
                T result;
                using (DbCommand command = CreateCommand(connection, out IReadOnlyList<OutputParameter> outputParameters)) {
                    result = executor(command);
                    WriteBack(outputParameters);
                }

                Initialize(thenReset);

                return result;
            } finally {
                if (!KeepConnection) {
                    ReleaseConnection(connection);
                }
            }
        }

        private async Task<T> ExecuteInternalAsync<T>(
            Func<DbCommand, CancellationToken, Task<T>> executor,
            ResetItems thenReset,
            CancellationToken cancellationToken) {
            DbConnection connection = await BuildConnectionAsync(cancellationToken).ConfigureAwait(false);
            try {
                T result;
                using (DbCommand command = CreateCommand(connection, out IReadOnlyList<OutputParameter> outputParameters)) {
                    result = await executor(command, cancellationToken).ConfigureAwait(false);
                    WriteBack(outputParameters);
                }

                Initialize(thenReset);

                return result;
            } finally {
                if (!KeepConnection) {
                    ReleaseConnection(connection);
                }
            }
        }

        private DbConnection BuildConnection() {
            DbConnection connection = EnsureConnection();
            if (connection.State == ConnectionState.Closed) {
                try {
                    connection.Open();
                } catch {
                    ReleaseConnection(connection);
                    throw;
                }
            }

            return connection;
        }

        private async Task<DbConnection> BuildConnectionAsync(CancellationToken cancellationToken) {
            DbConnection connection = EnsureConnection();
            if (connection.State == ConnectionState.Closed) {
                try {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                } catch {
                    ReleaseConnection(connection);
                    throw;
                }
            }

            return connection;
        }

        private DbConnection EnsureConnection() {
            if (Connection is null) {
                DbProviderFactory factory = DbProviderFactory
                    ?? throw new InvalidOperationException($"{nameof(FacadeConfiguration)}.{nameof(FacadeConfiguration.DefaultDbProviderFactory)} is not configured.");

                Connection = factory.CreateConnection()
                    ?? throw new InvalidOperationException("DbProviderFactory.CreateConnection() returned null.");
            }

            if (Connection is not DbConnection connection) {
                throw new InvalidOperationException("The configured provider must create DbConnection instances.");
            }

            if (string.IsNullOrWhiteSpace(connection.ConnectionString)) {
                if (ConnectionString is null) {
                    ReleaseConnection(connection);
                    throw new InvalidOperationException($"{nameof(FacadeConfiguration)}.{nameof(FacadeConfiguration.DefaultConnectionString)} is not configured.");
                }

                connection.ConnectionString = ConnectionString;
            }

            if (connection.State == ConnectionState.Broken) {
                connection.Close();
            }

            return connection;
        }

        private void ReleaseConnection(DbConnection connection) {
            connection.Dispose();
            if (ReferenceEquals(Connection, connection)) {
                Connection = null;
            }
        }

        /// <summary>
        /// Writes back output parameter values of closed data readers and releases their commands.
        /// </summary>
        internal void WriteBackClosedReaderParameters() {
            for (int i = openReaders.Count - 1; i >= 0; i--) {
                OpenReader openReader = openReaders[i];
                if (openReader.DataReader.IsClosed) {
                    // Output 參數要等讀取器關閉後 provider 才會填入
                    WriteBack(openReader.OutputParameters);
                    openReader.Command.Dispose();
                    openReaders.RemoveAt(i);
                }
            }
        }

        private static void WriteBack(IReadOnlyList<OutputParameter> outputParameters) {
            foreach (OutputParameter outputParameter in outputParameters) {
                outputParameter.Metadata.Value = outputParameter.Parameter.Value;
            }
        }

        private DbCommand CreateCommand(DbConnection connection, out IReadOnlyList<OutputParameter> outputParameters) {
            WriteBackClosedReaderParameters();

            OnCommandCreating?.Invoke(Parameters, CommandText);
            CommandCreating?.Invoke(Parameters, CommandText);

            string sql = CommandText
                ?? throw new InvalidOperationException("CommandText must be set before executing a command.");
            DbCommand command = connection.CreateCommand();
            try {
                command.CommandTimeout = CommandTimeout;
                command.CommandType = CommandType;
                command.Transaction = Transaction as DbTransaction;
                List<OutputParameter> outputs = [];
                command.CommandText = BuildParameters(command, sql, outputs);
                outputParameters = outputs;

                OnCommandCreated?.Invoke(command);
                CommandCreated?.Invoke(command);
            } catch {
                command.Dispose();
                throw;
            }

            return command;
        }

        private string BuildParameters(DbCommand command, string sql, List<OutputParameter> outputParameters) {
            foreach (ParameterMetadata metadata in Parameters) {
                string parameterName = metadata.ParameterName
                    ?? throw new InvalidOperationException("ParameterName cannot be null.");

                if (CommandType != CommandType.Text) {
                    // 預存程序與 TableDirect 的 CommandText 不是 SQL 語法，無從比對參數名稱；
                    // 集合值（例如資料表值參數）也不展開，原樣交給 provider 處理
                    AddParameter(command, metadata, outputParameters);
                    continue;
                }

                Regex regex = GetParameterNameRegex(parameterName);
                Match match = regex.Match(sql);
                if (!match.Success) {
                    continue;
                }

                if (AsExpandableValues(metadata.Value) is IEnumerable enumerableValue) {
                    List<string> postfixedNames = [];
                    int count = 0;

                    foreach (object? value in enumerableValue) {
                        DbParameter parameter = command.CreateParameter();
                        metadata.ApplyParameter(parameter);
                        parameter.ParameterName = $"{ParameterNamePrefix}_{metadata.ParameterName}_{count++}";
                        parameter.Value = value ?? DBNull.Value;

                        command.Parameters.Add(parameter);
                        postfixedNames.Add(match.Value[0] + parameter.ParameterName);
                    }

                    // 以 MatchEvaluator 取代，避免參數名稱或前綴中的 $ 被當成 Regex 取代語法
                    string replacement = count > 0
                        ? $"({string.Join(", ", postfixedNames)})"
                        : "(NULL)";
                    sql = regex.Replace(sql, _ => replacement);
                } else {
                    AddParameter(command, metadata, outputParameters);
                }
            }

            return sql;
        }

        private static void AddParameter(DbCommand command, ParameterMetadata metadata, List<OutputParameter> outputParameters) {
            DbParameter parameter = command.CreateParameter();
            metadata.ApplyParameter(parameter);
            command.Parameters.Add(parameter);

            if (metadata.Direction != ParameterDirection.Input) {
                outputParameters.Add(new OutputParameter(metadata, parameter));
            }
        }

        private static IEnumerable? AsExpandableValues(object? value) {
            // string、byte[] 與 char[] 雖然實作 IEnumerable，但 provider 會視為單一純量值（文字或二進位）
            return value is IEnumerable enumerable and not string and not byte[] and not char[]
                ? enumerable
                : null;
        }

        private static Regex GetParameterNameRegex(string name) {
            if (ParameterNameRegexCache.TryGetValue(name, out Regex? regex)) {
                return regex;
            }

            // CultureInvariant 讓快取的 Regex 不受建立當下執行緒文化特性影響
            regex = new Regex(GetParameterNamePattern(name), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            if (ParameterNameRegexCache.Count < MaxCachedParameterNameRegexCount) {
                ParameterNameRegexCache.TryAdd(name, regex);
            }

            return regex;
        }

        private static string GetParameterNamePattern(string name) {
            string escapedName = Regex.Escape(name);
            return $@"(?<!@|\?|:)[@?:]({escapedName}(?=[\W])|{escapedName}$)";
        }

        private void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    Transaction?.Dispose();
                    Transaction = null;

                    WriteBackClosedReaderParameters();
                    foreach (OpenReader openReader in openReaders) {
                        // KeepConnection 為 false 時連線屬於讀取器，讀取器未關閉就保留 command，讓呼叫端仍可繼續讀取
                        if (KeepConnection) {
                            openReader.Command.Dispose();
                        }
                    }

                    openReaders.Clear();

                    Connection?.Dispose();
                    Connection = null;
                }

                disposedValue = true;
            }
        }

        private sealed class OpenReader {
            public OpenReader(DbDataReader dataReader, DbCommand command, IReadOnlyList<OutputParameter> outputParameters) {
                DataReader = dataReader;
                Command = command;
                OutputParameters = outputParameters;
            }

            public DbDataReader DataReader { get; }

            public DbCommand Command { get; }

            public IReadOnlyList<OutputParameter> OutputParameters { get; }
        }

        private sealed class OutputParameter {
            public OutputParameter(ParameterMetadata metadata, DbParameter parameter) {
                Metadata = metadata;
                Parameter = parameter;
            }

            public ParameterMetadata Metadata { get; }

            public DbParameter Parameter { get; }
        }
    }
}
