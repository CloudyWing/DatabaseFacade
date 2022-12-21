using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using static CloudyWing.DatabaseFacade.FacadeConfiguration;

namespace CloudyWing.DatabaseFacade {
    public sealed class CommandExecutor : IDisposable {
        private IDbConnection connection;
        private IDbTransaction transaction;
        private bool disposedValue;

        public CommandExecutor(bool? keepConnection = null)
            : this(null, null, keepConnection) { }

        public CommandExecutor(string connStr, bool? keepConnection = null)
            : this(null, connStr, keepConnection) { }

        public CommandExecutor(DbProviderFactory providerFactory, string connStr, bool? keepConnection) {
            DbProviderFactory = providerFactory ?? DefaultDbProviderFactory;
            ConnectionString = connStr ?? DefaultConnectionString;
            KeepConnection = keepConnection ?? DefaultKeepConnection;
            Initialize();
        }

        ~CommandExecutor() {
            Dispose(disposing: false);
        }

        public DbProviderFactory DbProviderFactory { get; private set; }

        public string ConnectionString { get; private set; }

        public bool KeepConnection { get; private set; }

        public string CommandText { get; set; }

        public int CommandTimeout { get; set; }

        public CommandType CommandType { get; set; }

        public ParameterCollection Parameters { get; } = new ParameterCollection();

        public IDataReader CreateDataReader(ResetItems thenReset = ResetItems.All, CommandBehavior behavior = CommandBehavior.SequentialAccess) {
            BuildConnection();

            IDataReader dr;

            if (KeepConnection) {
                IDbCommand cmd = CreateCommand(connection);
                try {
                    dr = cmd.ExecuteReader(behavior);
                } catch {
                    cmd?.Dispose();
                    throw;
                }
            } else {
                behavior |= CommandBehavior.CloseConnection;
                IDbCommand cmd = CreateCommand(connection);
                dr = cmd.ExecuteReader(behavior);
            }

            Initialize(thenReset);

            return dr;
        }

        public DataTable CreateDataTable(ResetItems thenReset = ResetItems.All) {
            return ExecuteInternal(
                cmd => {
                    CommandBehavior behavior = CommandBehavior.SequentialAccess;

                    if (!KeepConnection) {
                        behavior |= CommandBehavior.CloseConnection;
                    }

                    using (IDataReader dr = cmd.ExecuteReader(behavior)) {
                        DataTable dt = new DataTable();
                        dt.Load(dr);
                        return dt;
                    }
                },
                thenReset
            );

        }

        public object QueryScalar(ResetItems thenReset = ResetItems.All) {
            return ExecuteInternal(cmd => cmd.ExecuteScalar(), thenReset);
        }

        public int Execute(ResetItems thenReset = ResetItems.All) {
            return ExecuteInternal(cmd => cmd.ExecuteNonQuery(), thenReset);
        }

        private T ExecuteInternal<T>(Func<IDbCommand, T> executor, ResetItems thenReset) {
            T result = default;

            BuildConnection();

            if (KeepConnection) {
                using (IDbCommand cmd = CreateCommand(connection)) {
                    result = executor(cmd);
                }
            } else {
                using (connection)
                using (IDbCommand cmd = CreateCommand(connection)) {
                    result = executor(cmd);

                    connection = null;
                }
            }

            Initialize(thenReset);

            return result;
        }

        public IDbTransaction BeginTransaction(IsolationLevel? level = null) {
            if (!KeepConnection) {
                throw new KeepConnectionRequiredException($"{nameof(KeepConnection)} must be set to true to use {nameof(BeginTransaction)}.");
            }

            level = level ?? DefaultIsolationLevel;
            BuildConnection();
            transaction = connection.BeginTransaction(level.Value);
            return transaction;
        }

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

        private void BuildConnection() {
            connection = connection ?? DbProviderFactory.CreateConnection();

            // 某一版 Microsoft.Data.SqlClient 在沒有使用 BeginTransaction() 的情況下，
            // 疑似 Close Command 會順便重置 Connection
            // 為預防萬一，沒值就重新設定
            if (string.IsNullOrWhiteSpace(connection.ConnectionString)) {
                connection.ConnectionString = ConnectionString;
            }

            if (connection.State == ConnectionState.Broken) {
                connection.Close();
            }

            if (connection != null && connection.State == ConnectionState.Closed && transaction is null) {
                connection.Open();
            }
        }

        private IDbCommand CreateCommand(IDbConnection connection) {
            OnCommandCreating?.Invoke(Parameters, CommandText);

            IDbCommand cmd = connection.CreateCommand();
            string sql = CommandText;
            cmd.CommandTimeout = CommandTimeout;
            cmd.CommandType = CommandType;
            cmd.Transaction = transaction;

            foreach (ParameterMetadata metadata in Parameters) {
                Regex regex = new Regex(GetParameterNamePattern(metadata.ParameterName), RegexOptions.IgnoreCase);
                Match match = regex.Match(sql);
                // 統一不會因為多宣告的parameter而出錯(SqlCommand允許，OracleCommand不允許)
                if (!match.Success) {
                    continue;
                }

                if (IsEnumerable(metadata.Value)) {
                    List<string> postfixedNames = new List<string>();
                    int count = 0;

                    foreach (object value in metadata.Value as IEnumerable) {
                        IDbDataParameter parameter = cmd.CreateParameter();
                        metadata.ApplyParameter(parameter);
                        parameter.ParameterName = $"{ParameterNamePrefix}_{metadata.ParameterName}_{count++}";
                        parameter.Value = value;

                        cmd.Parameters.Add(parameter);
                        postfixedNames.Add(match.Value[0] + parameter.ParameterName);
                    }

                    if (count > 0) {
                        sql = regex.Replace(sql, $"({string.Join(", ", postfixedNames)})");
                    } else {
                        sql = regex.Replace(sql, $"(NULL)");
                    }
                } else {
                    IDbDataParameter parameter = cmd.CreateParameter();
                    metadata.ApplyParameter(parameter);
                    cmd.Parameters.Add(parameter);
                }
            }
            cmd.CommandText = sql;

            OnCommandCreated?.Invoke(cmd);

            return cmd;
        }

        private bool IsEnumerable(object value) {
            return !(value is string) && value is IEnumerable;
        }

        private string GetParameterNamePattern(string name) {
            // HACK 應該是可以用了，但最好視實際情況寫更精準點
            return $@"(?<!@|\?|:)[@?:]({name}(?=[\W])|{name}$)";
        }

        private void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    if (connection != null) {
                        connection.Dispose();
                        connection = null;
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
