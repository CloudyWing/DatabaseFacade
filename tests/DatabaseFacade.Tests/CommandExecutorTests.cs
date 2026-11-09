using System.Data;
using System.Data.Common;
using CloudyWing.DatabaseFacade.Tests.Fakes;
using Microsoft.Data.Sqlite;

namespace CloudyWing.DatabaseFacade.Tests {
    [TestFixture]
    public class CommandExecutorTests {
        private const string DbName = "Test.db";
        private const string ConnectionString = $"Data Source={DbName};";
        private const string InsertSql = "INSERT INTO Test (Id, Name) VALUES (@Id, @Name)";

        private readonly Record[] basicRecords = [
            new Record { Id = 0, Name = "Wing" },
            new Record { Id = 1, Name = "Terry" },
            new Record { Id = 2, Name = "Marry" }
        ];

        [OneTimeSetUp]
        public void Init() {
            // 預防單元測試意外中斷
            if (File.Exists(DbName)) {
                File.Delete(DbName);
            }

            using SqliteConnection conn = new(ConnectionString);
            conn.Open();

            using SqliteCommand cmd = new() {
                Connection = conn,
                CommandText = "CREATE TABLE Test (Id INTEGER PRIMARY KEY, Name TEXT NOT NULL)"
            };
            cmd.ExecuteNonQuery();

            FacadeConfiguration.SetConfiguration(SqliteFactory.Instance, ConnectionString);
        }

        [SetUp]
        public void Setup() {
            using SqliteConnection conn = new(ConnectionString);
            conn.Open();

            foreach (Record record in basicRecords) {
                using SqliteCommand cmd = new() {
                    Connection = conn,
                    CommandText = InsertSql
                };

                cmd.Parameters.AddWithValue("Id", record.Id);
                cmd.Parameters.AddWithValue("Name", record.Name);
                cmd.ExecuteNonQuery();
            }

            FacadeConfiguration.DefaultKeepConnection = true;
        }

        [Test]
        public void Constructor_WhenDefaultFactoryConfigured_ShouldPreferConfiguredFactory() {
            // Arrange - Init 已設定過 DbProviderFactory

            // Act & Assert
            CommandExecutor executor = new();
            Assert.That(executor.DbProviderFactory, Is.SameAs(SqliteFactory.Instance));

            executor = new(null, "Test");
            Assert.That(executor.DbProviderFactory, Is.SameAs(SqliteFactory.Instance));

            FakeDbProviderFactory fakeFactory = new();
            executor = new(fakeFactory, "Test");
            Assert.That(executor.DbProviderFactory, Is.SameAs(fakeFactory));
        }

        [Test]
        public void Constructor_WhenDefaultConnectionStringConfigured_ShouldPreferConfiguredConnectionString() {
            // Arrange - Init 已設定過 ConnectionString

            // Act & Assert
            CommandExecutor executor = new();
            Assert.That(executor.ConnectionString, Is.EqualTo(ConnectionString));

            executor = new(null, null);
            Assert.That(executor.ConnectionString, Is.EqualTo(ConnectionString));

            executor = new("Test");
            Assert.That(executor.ConnectionString, Is.EqualTo("Test"));
        }

        [Test]
        public void Execute_WhenKeepConnectionIsTrue_ShouldKeepConnectionOpen() {
            using CommandExecutor executor = new(true) {
                CommandText = "SELECT COUNT(1) FROM Test"
            };

            executor.Execute();

            Assert.That(executor.Connection?.State, Is.EqualTo(ConnectionState.Open));
        }

        [Test]
        public void Execute_WhenKeepConnectionIsFalse_ShouldClearConnection() {
            using CommandExecutor executor = new(false) {
                CommandText = "SELECT COUNT(1) FROM Test"
            };

            executor.Execute();

            Assert.That(executor.Connection, Is.Null);
        }

        [Test]
        public void Constructor_WhenKeepConnectionArgumentProvided_ShouldOverrideConfiguredValue() {
            FacadeConfiguration.DefaultKeepConnection = true;

            // Act & Assert
            CommandExecutor executor = new();
            Assert.That(executor.KeepConnection, Is.True);

            executor = new(null);
            Assert.That(executor.KeepConnection, Is.True);

            executor = new(false);
            Assert.That(executor.KeepConnection, Is.False);
        }

        [Test]
        public void CreateDataTable_WhenParameterNamePrefixConfigured_ShouldPrefixExpandedParameterNames() {
            FacadeConfiguration.ParameterNamePrefix = "Test";
            FacadeConfiguration.OnCommandCreated += (cmd) => {
                int i = 0;
                foreach (IDbDataParameter parameter in cmd.Parameters) {
                    Assert.That(parameter.ParameterName, Is.EqualTo("Test_Ids_" + i++));
                }
            };

            using CommandExecutor executor = new() {
                CommandText = "SELECT * FROM Test WHERE Id IN @Ids ORDER BY Id"
            };
            executor.Parameters.Add("Ids", new long[] { 1, 2 });
            executor.CreateDataTable();
        }

        [Test]
        [TestCase("Text1", null)]
        [TestCase("Text2", CommandType.Text)]
        [TestCase("Text3", CommandType.StoredProcedure)]
        [TestCase("Text3", CommandType.TableDirect)]
        public void SetCommandText_WhenCommandTextAndTypeProvided_ShouldApplyValues(string commandText, CommandType? commandType) {
            CommandExecutor executor = new CommandExecutor()
                .SetCommandText(commandText, commandType);

            Assert.That(executor.CommandText, Is.EqualTo(commandText));
            if (commandType.HasValue) {
                Assert.That(executor.CommandType, Is.EqualTo(commandType.Value));
            } else {
                Assert.That(executor.CommandType, Is.EqualTo(CommandType.Text));
            }
        }

        [Test]
        public void SetCommandTimeout_WhenSecondsProvided_ShouldUpdateTimeout() {
            CommandExecutor executor = new();

            int newSecond = FacadeConfiguration.DefaultCommandTimeout + 100;
            executor.SetCommandTimeout(newSecond);

            Assert.That(executor.CommandTimeout, Is.EqualTo(newSecond));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateDataReader_WhenQueryExecuted_ShouldReturnMatchingRecords(bool keepConnection) {
            IEnumerable<Record> list = QueryRecordsByDataReader(keepConnection);

            Assert.That(
                list.Select(r => (r.Id, r.Name)),
                Is.EquivalentTo(basicRecords.Select(r => (r.Id, r.Name))));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task CreateDataReaderAsync_WhenQueryExecuted_ShouldReturnMatchingRecords(bool keepConnection) {
            IReadOnlyList<Record> list = await QueryRecordsByDataReaderAsync(keepConnection);

            Assert.That(
                list.Select(r => (r.Id, r.Name)),
                Is.EquivalentTo(basicRecords.Select(r => (r.Id, r.Name))));
        }

        private static IEnumerable<Record> QueryRecordsByDataReader(bool keepConnection) {
            using CommandExecutor executor = CreateQueryExecutor(keepConnection);
            using IDataReader dr = executor.CreateDataReader();

            while (dr.Read()) {
                yield return new Record { Id = (long)dr["Id"], Name = (string)dr["Name"] };
            }
        }

        private static async Task<IReadOnlyList<Record>> QueryRecordsByDataReaderAsync(bool keepConnection) {
            List<Record> records = [];

            using CommandExecutor executor = CreateQueryExecutor(keepConnection);
            using DbDataReader dr = await executor.CreateDataReaderAsync();

            while (await dr.ReadAsync()) {
                records.Add(new Record { Id = (long)dr["Id"], Name = (string)dr["Name"] });
            }

            return records;
        }

        private static CommandExecutor CreateQueryExecutor(bool keepConnection) {
            return new CommandExecutor(keepConnection) {
                CommandText = "SELECT * FROM Test ORDER BY Id"
            };
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateDataTable_WhenQueryExecuted_ShouldReturnMatchingRecords(bool keepConnection) {
            using CommandExecutor executor = CreateQueryExecutor(keepConnection);
            DataTable dt = executor.CreateDataTable();
            IEnumerable<Record> list = ConvertFrom(dt);

            Assert.That(
                list.Select(r => (r.Id, r.Name)),
                Is.EquivalentTo(basicRecords.Select(r => (r.Id, r.Name))));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task CreateDataTableAsync_WhenQueryExecuted_ShouldReturnMatchingRecords(bool keepConnection) {
            using CommandExecutor executor = CreateQueryExecutor(keepConnection);
            DataTable dt = await executor.CreateDataTableAsync();
            IEnumerable<Record> list = ConvertFrom(dt);

            Assert.That(
                list.Select(r => (r.Id, r.Name)),
                Is.EquivalentTo(basicRecords.Select(r => (r.Id, r.Name))));
        }

        private static IEnumerable<Record> ConvertFrom(DataTable dataTable) {
            foreach (DataRow dr in dataTable.Rows) {
                yield return new Record { Id = (long)dr["Id"], Name = (string)dr["Name"] };
            }
        }

        [Test]
        public void CreateDataTable_WhenParameterValueIsEnumerable_ShouldExpandParameters() {
            using CommandExecutor executor = new() {
                CommandText = "SELECT * FROM Test WHERE Id IN @Ids ORDER BY Id"
            };
            executor.Parameters.Add("Ids", new long[] { 1, 2, 10 }); // BasicRecords 沒有 Id = 10 的資料

            DataTable dt = executor.CreateDataTable();
            IEnumerable<Record> records = ConvertFrom(dt);

            using (Assert.EnterMultipleScope()) {
                Assert.That(records.Count(), Is.EqualTo(2));
                Assert.That(records.ElementAt(0).Id, Is.EqualTo(basicRecords.Single(x => x.Id == 1).Id));
                Assert.That(records.ElementAt(0).Name, Is.EqualTo(basicRecords.Single(x => x.Id == 1).Name));
                Assert.That(records.ElementAt(1).Id, Is.EqualTo(basicRecords.Single(x => x.Id == 2).Id));
                Assert.That(records.ElementAt(1).Name, Is.EqualTo(basicRecords.Single(x => x.Id == 2).Name));
            }
        }

        [Test]
        public async Task CreateDataTableAsync_WhenParameterValueIsEnumerable_ShouldExpandParameters() {
            using CommandExecutor executor = new() {
                CommandText = "SELECT * FROM Test WHERE Id IN @Ids ORDER BY Id"
            };
            executor.Parameters.Add("Ids", new long[] { 1, 2, 10 }); // BasicRecords 沒有 Id = 10 的資料

            DataTable dt = await executor.CreateDataTableAsync();
            IEnumerable<Record> records = ConvertFrom(dt);

            using (Assert.EnterMultipleScope()) {
                Assert.That(records.Count(), Is.EqualTo(2));
                Assert.That(records.ElementAt(0).Id, Is.EqualTo(basicRecords.Single(x => x.Id == 1).Id));
                Assert.That(records.ElementAt(0).Name, Is.EqualTo(basicRecords.Single(x => x.Id == 1).Name));
                Assert.That(records.ElementAt(1).Id, Is.EqualTo(basicRecords.Single(x => x.Id == 2).Id));
                Assert.That(records.ElementAt(1).Name, Is.EqualTo(basicRecords.Single(x => x.Id == 2).Name));
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void QueryScalar_WhenQueryExecuted_ShouldReturnFirstCellValue(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection) {
                CommandText = "SELECT COUNT(1) FROM Test"
            };

            long count = Convert.ToInt64(executor.QueryScalar());

            Assert.That(count, Is.EqualTo(basicRecords.Length));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task QueryScalarAsync_WhenQueryExecuted_ShouldReturnFirstCellValue(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection) {
                CommandText = "SELECT COUNT(1) FROM Test"
            };

            long count = Convert.ToInt64(await executor.QueryScalarAsync());

            Assert.That(count, Is.EqualTo(basicRecords.Length));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Execute_WhenInsertSqlProvided_ShouldInsertRecord(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection) {
                CommandText = "INSERT INTO Test (Id, Name) VALUES (10, '新增測試')"
            };

            int result = executor.Execute();

            IEnumerable<Record> records = QueryRecordsByDataReader(keepConnection);
            Record? insertedRecord = records.SingleOrDefault(x => x.Id == 10);

            using (Assert.EnterMultipleScope()) {
                Assert.That(result, Is.EqualTo(1));
                Assert.That(records.Count(), Is.EqualTo(basicRecords.Length + 1));
                Assert.That(insertedRecord, Is.Not.Null);
            }
            using (Assert.EnterMultipleScope()) {
                Assert.That(insertedRecord?.Id, Is.EqualTo(10));
                Assert.That(insertedRecord?.Name, Is.EqualTo("新增測試"));
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task ExecuteAsync_WhenInsertUsesAnonymousObject_ShouldInsertRecord(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection);

            int result = await executor.SetCommandText(InsertSql)
                .Parameters.AddRange(new { Id = 10, Name = "新增測試" })
                .GetCommandExecutor()
                .ExecuteAsync();

            IReadOnlyList<Record> records = await QueryRecordsByDataReaderAsync(keepConnection);
            Record? insertedRecord = records.SingleOrDefault(x => x.Id == 10);

            using (Assert.EnterMultipleScope()) {
                Assert.That(result, Is.EqualTo(1));
                Assert.That(records, Has.Count.EqualTo(basicRecords.Length + 1));
                Assert.That(insertedRecord, Is.Not.Null);
            }

            using (Assert.EnterMultipleScope()) {
                Assert.That(insertedRecord?.Id, Is.EqualTo(10));
                Assert.That(insertedRecord?.Name, Is.EqualTo("新增測試"));
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Execute_WhenInsertUsesParameterMetadata_ShouldInsertRecord(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection);

            int result = executor.SetCommandText(InsertSql)
                .Parameters.Add(new ParameterMetadata { ParameterName = "Id", Value = 10 })
                    .Add(new ParameterMetadata { ParameterName = "Name", Value = "新增測試" })
                    .GetCommandExecutor()
                .Execute();

            IEnumerable<Record> records = QueryRecordsByDataReader(keepConnection);
            Record? insertedRecord = records.SingleOrDefault(x => x.Id == 10);

            using (Assert.EnterMultipleScope()) {
                Assert.That(result, Is.EqualTo(1));
                Assert.That(records.Count(), Is.EqualTo(basicRecords.Length + 1));
                Assert.That(insertedRecord, Is.Not.Null);
            }

            using (Assert.EnterMultipleScope()) {
                Assert.That(insertedRecord?.Id, Is.EqualTo(10));
                Assert.That(insertedRecord?.Name, Is.EqualTo("新增測試"));
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Execute_WhenInsertUsesDbParameter_ShouldInsertRecord(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection);

            int result = executor.SetCommandText(InsertSql)
                .Parameters.Add(new SqliteParameter { ParameterName = "Id", Value = 10 })
                    .Add(new SqliteParameter { ParameterName = "Name", Value = "新增測試" })
                    .GetCommandExecutor()
                .Execute();

            IEnumerable<Record> records = QueryRecordsByDataReader(keepConnection);
            Record? insertedRecord = records.SingleOrDefault(x => x.Id == 10);

            using (Assert.EnterMultipleScope()) {
                Assert.That(result, Is.EqualTo(1));
                Assert.That(records.Count(), Is.EqualTo(basicRecords.Length + 1));
                Assert.That(insertedRecord, Is.Not.Null);
            }

            using (Assert.EnterMultipleScope()) {
                Assert.That(insertedRecord?.Id, Is.EqualTo(10));
                Assert.That(insertedRecord?.Name, Is.EqualTo("新增測試"));
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Execute_WhenInsertUsesAnonymousObject_ShouldInsertRecord(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection);

            int result = executor.SetCommandText(InsertSql)
                .Parameters.AddRange(new { Id = 10, Name = "新增測試" })
                    .GetCommandExecutor()
                .Execute();

            IEnumerable<Record> records = QueryRecordsByDataReader(keepConnection);
            Record? insertedRecord = records.SingleOrDefault(x => x.Id == 10);

            using (Assert.EnterMultipleScope()) {
                Assert.That(result, Is.EqualTo(1));
                Assert.That(records.Count(), Is.EqualTo(basicRecords.Length + 1));
                Assert.That(insertedRecord, Is.Not.Null);
            }

            using (Assert.EnterMultipleScope()) {
                Assert.That(insertedRecord?.Id, Is.EqualTo(10));
                Assert.That(insertedRecord?.Name, Is.EqualTo("新增測試"));
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Execute_WhenInsertUsesDictionary_ShouldInsertRecord(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection);

            int result = executor.SetCommandText(InsertSql)
                .Parameters.AddRange(new Dictionary<string, object> { ["Id"] = 10, ["Name"] = "新增測試" })
                .GetCommandExecutor()
                .Execute();

            IEnumerable<Record> records = QueryRecordsByDataReader(keepConnection);
            Record? insertedRecord = records.SingleOrDefault(x => x.Id == 10);

            using (Assert.EnterMultipleScope()) {
                Assert.That(result, Is.EqualTo(1));
                Assert.That(records.Count(), Is.EqualTo(basicRecords.Length + 1));
                Assert.That(insertedRecord, Is.Not.Null);
            }

            using (Assert.EnterMultipleScope()) {
                Assert.That(insertedRecord?.Id, Is.EqualTo(10));
                Assert.That(insertedRecord?.Name, Is.EqualTo("新增測試"));
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Execute_WhenUpdateSqlProvided_ShouldUpdateRecord(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection) {
                CommandText = "UPDATE Test SET Name = '修改測試' WHERE Id = 1"
            };

            int result = executor.Execute();

            Record? record = QueryRecordsByDataReader(keepConnection).SingleOrDefault(x => x.Id == 1);

            using (Assert.EnterMultipleScope()) {
                Assert.That(result, Is.EqualTo(1));
                Assert.That(record, Is.Not.Null);
            }
            Assert.That(record?.Name, Is.EqualTo("修改測試"));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Execute_WhenDeleteSqlProvided_ShouldDeleteRecord(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection) {
                CommandText = "DELETE FROM Test WHERE Id = 1"
            };

            int result = executor.Execute();

            IEnumerable<Record> records = QueryRecordsByDataReader(keepConnection);
            Record? deletedRecord = records.SingleOrDefault(x => x.Id == 1);

            using (Assert.EnterMultipleScope()) {
                Assert.That(result, Is.EqualTo(1));
                Assert.That(deletedRecord, Is.Null);
                Assert.That(records.Count(), Is.EqualTo(basicRecords.Length - 1));
            }
        }

        [Test]
        public void BeginTransaction_WhenKeepConnectionIsFalse_ShouldThrowKeepConnectionRequiredException() {
            using CommandExecutor executor = new(false);

            Assert.That(
                () => executor.BeginTransaction(),
                Throws.TypeOf<KeepConnectionRequiredException>()
                    .With.Message.EqualTo("KeepConnection must be set to true to use BeginTransaction."));
        }

        [Test]
        public void BeginTransaction_WhenRollbackInvoked_ShouldRevertChanges() {
            using CommandExecutor executor = new(true);
            using IDbTransaction transaction = executor.BeginTransaction();

            int result = executor.SetCommandText(InsertSql)
                .Parameters.AddRange(
                    new SqliteParameter { ParameterName = "Id", Value = 10 },
                    new SqliteParameter { ParameterName = "Name", Value = "新增測試" }
                ).GetCommandExecutor()
                .Execute();

            Assert.That(result, Is.EqualTo(1));

            transaction.Rollback();

            DataTable dt = executor.SetCommandText("SELECT * FROM Test ORDER BY Id")
                .CreateDataTable();
            IEnumerable<Record> records = ConvertFrom(dt);
            Record? insteredRecord = records.SingleOrDefault(x => x.Id == 10);

            using (Assert.EnterMultipleScope()) {
                Assert.That(records.Count(), Is.EqualTo(basicRecords.Length));
                Assert.That(insteredRecord, Is.Null);
            }
        }

        [Test]
        public void BeginTransaction_WhenCommitInvoked_ShouldPersistChanges() {
            using CommandExecutor executor = new(true);
            using IDbTransaction transaction = executor.BeginTransaction();

            int result = executor.SetCommandText(InsertSql)
                .Parameters.AddRange(
                    new SqliteParameter { ParameterName = "Id", Value = 10 },
                    new SqliteParameter { ParameterName = "Name", Value = "新增測試" }
                ).GetCommandExecutor()
                .Execute();

            Assert.That(result, Is.EqualTo(1));

            transaction.Commit();

            DataTable dt = executor.SetCommandText("SELECT * FROM Test ORDER BY Id")
                .CreateDataTable();
            IEnumerable<Record> records = ConvertFrom(dt);
            Record? insteredRecord = records.SingleOrDefault(x => x.Id == 10);

            using (Assert.EnterMultipleScope()) {
                Assert.That(records.Count(), Is.EqualTo(basicRecords.Length + 1));
                Assert.That(insteredRecord?.Id, Is.EqualTo(10));
                Assert.That(insteredRecord?.Name, Is.EqualTo("新增測試"));
            }
        }

        [Test]
        public async Task BeginTransaction_WhenExecuteAsyncAndRollbackInvoked_ShouldRevertChanges() {
            using CommandExecutor executor = new(true);
            using IDbTransaction transaction = executor.BeginTransaction();

            int result = await executor.SetCommandText(InsertSql)
                .Parameters.AddRange(
                    new SqliteParameter { ParameterName = "Id", Value = 10 },
                    new SqliteParameter { ParameterName = "Name", Value = "新增測試" }
                ).GetCommandExecutor()
                .ExecuteAsync();

            Assert.That(result, Is.EqualTo(1));

            transaction.Rollback();

            DataTable dt = await executor.SetCommandText("SELECT * FROM Test ORDER BY Id")
                .CreateDataTableAsync();
            IEnumerable<Record> records = ConvertFrom(dt);
            Record? insteredRecord = records.SingleOrDefault(x => x.Id == 10);

            using (Assert.EnterMultipleScope()) {
                Assert.That(records.Count(), Is.EqualTo(basicRecords.Length));
                Assert.That(insteredRecord, Is.Null);
            }
        }

        [Test]
        public async Task BeginTransaction_WhenExecuteAsyncAndCommitInvoked_ShouldPersistChanges() {
            using CommandExecutor executor = new(true);
            using IDbTransaction transaction = executor.BeginTransaction();

            int result = await executor.SetCommandText(InsertSql)
                .Parameters.AddRange(
                    new SqliteParameter { ParameterName = "Id", Value = 10 },
                    new SqliteParameter { ParameterName = "Name", Value = "新增測試" }
                ).GetCommandExecutor()
                .ExecuteAsync();

            Assert.That(result, Is.EqualTo(1));

            transaction.Commit();

            DataTable dt = await executor.SetCommandText("SELECT * FROM Test ORDER BY Id")
                .CreateDataTableAsync();
            IEnumerable<Record> records = ConvertFrom(dt);
            Record? insteredRecord = records.SingleOrDefault(x => x.Id == 10);

            using (Assert.EnterMultipleScope()) {
                Assert.That(records.Count(), Is.EqualTo(basicRecords.Length + 1));
                Assert.That(insteredRecord?.Id, Is.EqualTo(10));
                Assert.That(insteredRecord?.Name, Is.EqualTo("新增測試"));
            }
        }

        [Test]
        [TestCase(ResetItems.None)]
        [TestCase(ResetItems.CommandText)]
        [TestCase(ResetItems.CommandTimeout)]
        [TestCase(ResetItems.CommandType)]
        [TestCase(ResetItems.Parameters)]
        [TestCase(ResetItems.All)]
        public void CreateDataTable_WhenResetItemsSpecified_ShouldResetMatchingCommandState(ResetItems items) {
            using CommandExecutor executor = CreateQueryExecutor(false);
            executor.CreateDataTable(items);

            if ((items & ResetItems.CommandText) == ResetItems.CommandText) {
                Assert.That(executor.CommandText, Is.Null);
            }

            if ((items & ResetItems.CommandTimeout) == ResetItems.CommandTimeout) {
                Assert.That(executor.CommandTimeout, Is.EqualTo(FacadeConfiguration.DefaultCommandTimeout));
            }

            if ((items & ResetItems.CommandType) == ResetItems.CommandType) {
                Assert.That(executor.CommandType, Is.EqualTo(CommandType.Text));
            }

            if ((items & ResetItems.Parameters) == ResetItems.Parameters) {
                Assert.That(executor.Parameters, Is.Empty);
            }
        }

        [Test]
        [TestCase(ResetItems.None)]
        [TestCase(ResetItems.CommandText)]
        [TestCase(ResetItems.CommandTimeout)]
        [TestCase(ResetItems.CommandType)]
        [TestCase(ResetItems.Parameters)]
        [TestCase(ResetItems.All)]
        public async Task CreateDataTableAsync_WhenResetItemsSpecified_ShouldResetMatchingCommandState(ResetItems items) {
            using CommandExecutor executor = CreateQueryExecutor(false);
            await executor.CreateDataTableAsync(items);

            if ((items & ResetItems.CommandText) == ResetItems.CommandText) {
                Assert.That(executor.CommandText, Is.Null);
            }

            if ((items & ResetItems.CommandTimeout) == ResetItems.CommandTimeout) {
                Assert.That(executor.CommandTimeout, Is.EqualTo(FacadeConfiguration.DefaultCommandTimeout));
            }

            if ((items & ResetItems.CommandType) == ResetItems.CommandType) {
                Assert.That(executor.CommandType, Is.EqualTo(CommandType.Text));
            }

            if ((items & ResetItems.Parameters) == ResetItems.Parameters) {
                Assert.That(executor.Parameters, Is.Empty);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void QueryScalar_WhenParameterValueIsNull_ShouldBindDbNull(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection) {
                CommandText = "SELECT CASE WHEN @Value IS NULL THEN 1 ELSE 0 END"
            };
            executor.Parameters.Add("Value", null);

            long result = Convert.ToInt64(executor.QueryScalar());

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void QueryScalar_WhenParameterValueIsByteArray_ShouldBindSingleBinaryParameter() {
            byte[] value = [1, 2, 3];
            using CommandExecutor executor = new() {
                CommandText = "SELECT length(@Value)"
            };
            executor.Parameters.Add("Value", value);

            long result = Convert.ToInt64(executor.QueryScalar());

            Assert.That(result, Is.EqualTo(value.Length));
        }

        [Test]
        public void Execute_WhenParameterValueIsCharArray_ShouldBindSingleParameter() {
            FakeDbProviderFactory factory = new();
            char[] value = "Wing".ToCharArray();
            using CommandExecutor executor = new(factory, "Fake", true) {
                CommandText = "UPDATE Test SET Name = @Name"
            };
            executor.Parameters.Add("Name", value);

            executor.Execute();

            FakeDbCommand command = factory.ExecutedCommands.Single();
            using (Assert.EnterMultipleScope()) {
                Assert.That(command.CommandText, Is.EqualTo("UPDATE Test SET Name = @Name"));
                Assert.That(command.FakeParameters.Select(x => x.ParameterName), Is.EqualTo(new[] { "Name" }));
                Assert.That(command.FakeParameters.Single().Value, Is.SameAs(value));
            }
        }

        [Test]
        [TestCase(CommandType.StoredProcedure)]
        [TestCase(CommandType.TableDirect)]
        public void Execute_WhenCommandTypeIsNotText_ShouldPassParametersWithoutMatchingCommandText(CommandType commandType) {
            FakeDbProviderFactory factory = new();
            int[] ids = [1, 2];
            using CommandExecutor executor = new(factory, "Fake", true);
            executor.SetCommandText("dbo.UpdateTest", commandType)
                .Parameters.Add("Name", "Wing")
                .Add("Ids", ids);

            executor.Execute();

            FakeDbCommand command = factory.ExecutedCommands.Single();
            using (Assert.EnterMultipleScope()) {
                Assert.That(command.CommandText, Is.EqualTo("dbo.UpdateTest"));
                Assert.That(command.CommandType, Is.EqualTo(commandType));
                Assert.That(command.FakeParameters.Select(x => x.ParameterName), Is.EqualTo(new[] { "Name", "Ids" }));
                Assert.That(command.FakeParameters.Single(x => x.ParameterName == "Ids").Value, Is.SameAs(ids));
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateDataReader_WhenReaderClosedBeforeNextExecution_ShouldDisposeReaderCommand(bool keepConnection) {
            FakeDbProviderFactory factory = new();
            using CommandExecutor executor = new(factory, "Fake", keepConnection);
            executor.SetCommandText("SELECT 1").CreateDataReader().Dispose();
            FakeDbCommand readerCommand = factory.ExecutedCommands.Single();
            bool isDisposedBeforeNextExecution = readerCommand.IsDisposed;

            executor.SetCommandText("SELECT 2").Execute();

            using (Assert.EnterMultipleScope()) {
                Assert.That(isDisposedBeforeNextExecution, Is.False);
                Assert.That(readerCommand.IsDisposed, Is.True);
            }
        }

        [Test]
        public void Dispose_WhenKeepConnectionIsTrueAndReaderNotClosed_ShouldDisposeReaderCommand() {
            FakeDbProviderFactory factory = new();
            CommandExecutor executor = new(factory, "Fake", true);
            using IDataReader dataReader = executor.SetCommandText("SELECT 1").CreateDataReader();

            executor.Dispose();

            Assert.That(factory.ExecutedCommands.Single().IsDisposed, Is.True);
        }

        [Test]
        public void CreateDataReader_WhenKeepConnectionIsFalse_ShouldHandConnectionOverToReader() {
            FakeDbProviderFactory factory = new();
            using CommandExecutor executor = new(factory, "Fake", false);

            using IDataReader dataReader = executor.SetCommandText("SELECT 1").CreateDataReader();

            using (Assert.EnterMultipleScope()) {
                Assert.That(executor.Connection, Is.Null);
                Assert.That(factory.CreatedConnections.Single().State, Is.EqualTo(ConnectionState.Open));
                Assert.That(factory.ExecutedCommands.Single().ExecutedBehavior & CommandBehavior.CloseConnection, Is.EqualTo(CommandBehavior.CloseConnection));
            }
        }

        [Test]
        public void Execute_WhenKeepConnectionIsFalseAndPreviousReaderOpen_ShouldUseNewConnection() {
            FakeDbProviderFactory factory = new();
            using CommandExecutor executor = new(factory, "Fake", false);
            using IDataReader dataReader = executor.SetCommandText("SELECT 1").CreateDataReader();

            executor.SetCommandText("UPDATE Test SET Name = 'Wing'").Execute();

            using (Assert.EnterMultipleScope()) {
                Assert.That(factory.CreatedConnections, Has.Count.EqualTo(2));
                Assert.That(factory.CreatedConnections[0].State, Is.EqualTo(ConnectionState.Open));
                Assert.That(factory.CreatedConnections[0].IsDisposed, Is.False);
                Assert.That(factory.CreatedConnections[1].IsDisposed, Is.True);
            }
        }

        [Test]
        public void CreateDataReader_WhenKeepConnectionIsFalseAndExecutorDisposed_ShouldKeepReaderReadable() {
            CommandExecutor executor = CreateQueryExecutor(false);
            using IDataReader dataReader = executor.CreateDataReader();

            executor.Dispose();
            List<long> ids = [];
            while (dataReader.Read()) {
                ids.Add(dataReader.GetInt64(0));
            }

            Assert.That(ids, Is.EqualTo(basicRecords.Select(x => x.Id)));
        }

        [Test]
        public async Task CreateDataReaderAsync_WhenKeepConnectionIsFalseAndExecutorDisposed_ShouldKeepReaderReadable() {
            CommandExecutor executor = CreateQueryExecutor(false);
            using DbDataReader dataReader = await executor.CreateDataReaderAsync();

            executor.Dispose();
            List<long> ids = [];
            while (await dataReader.ReadAsync()) {
                ids.Add(dataReader.GetInt64(0));
            }

            Assert.That(ids, Is.EqualTo(basicRecords.Select(x => x.Id)));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Execute_WhenConnectionOpenFails_ShouldReleaseConnection(bool keepConnection) {
            FakeDbProviderFactory factory = new() { ShouldFailOnOpen = true };
            using CommandExecutor executor = new(factory, "Fake", keepConnection) {
                CommandText = "SELECT 1"
            };

            Assert.That(() => executor.Execute(), Throws.InvalidOperationException);

            using (Assert.EnterMultipleScope()) {
                Assert.That(executor.Connection, Is.Null);
                Assert.That(factory.CreatedConnections.Single().IsDisposed, Is.True);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ExecuteAsync_WhenCancellationRequestedBeforeOpen_ShouldThrowAndReleaseConnection(bool keepConnection) {
            using CancellationTokenSource cancellationTokenSource = new();
            cancellationTokenSource.Cancel();
            using CommandExecutor executor = new(keepConnection) {
                CommandText = "SELECT COUNT(1) FROM Test"
            };

            Assert.That(
                async () => await executor.ExecuteAsync(cancellationToken: cancellationTokenSource.Token),
                Throws.InstanceOf<OperationCanceledException>());

            using (Assert.EnterMultipleScope()) {
                Assert.That(executor.Connection, Is.Null);
                Assert.That(executor.CommandText, Is.EqualTo("SELECT COUNT(1) FROM Test"));
            }
        }

        [Test]
        public void Execute_WhenCommandCreatedHookThrows_ShouldDisposeCommandAndReleaseConnection() {
            FakeDbProviderFactory factory = new();
            DbCommand? createdCommand = null;
            FacadeConfiguration.OnCommandCreated = command => {
                createdCommand = (DbCommand)command;
                throw new InvalidOperationException("Hook failure.");
            };
            using CommandExecutor executor = new(factory, "Fake", false) {
                CommandText = "SELECT 1"
            };

            Assert.That(() => executor.Execute(), Throws.InvalidOperationException.With.Message.EqualTo("Hook failure."));

            using (Assert.EnterMultipleScope()) {
                Assert.That((createdCommand as FakeDbCommand)?.IsDisposed, Is.True);
                Assert.That(executor.Connection, Is.Null);
                Assert.That(factory.CreatedConnections.Single().IsDisposed, Is.True);
            }
        }

        [Test]
        public void Execute_WhenCommandTextIsNullAndKeepConnectionIsFalse_ShouldReleaseConnection() {
            FakeDbProviderFactory factory = new();
            using CommandExecutor executor = new(factory, "Fake", false);

            Assert.That(() => executor.Execute(), Throws.InvalidOperationException);

            using (Assert.EnterMultipleScope()) {
                Assert.That(executor.Connection, Is.Null);
                Assert.That(factory.CreatedConnections.Single().IsDisposed, Is.True);
            }
        }

        [Test]
        public void Execute_WhenCommandFails_ShouldKeepCommandState() {
            FakeDbProviderFactory factory = new() {
                OnExecuting = _ => throw new InvalidOperationException("Execution failure.")
            };
            using CommandExecutor executor = new(factory, "Fake", true);
            executor.SetCommandText("UPDATE Test SET Name = @Name")
                .Parameters.Add("Name", "Wing");

            Assert.That(() => executor.Execute(), Throws.InvalidOperationException);

            using (Assert.EnterMultipleScope()) {
                Assert.That(executor.CommandText, Is.EqualTo("UPDATE Test SET Name = @Name"));
                Assert.That(executor.Parameters, Has.Count.EqualTo(1));
                Assert.That(factory.ExecutedCommands.Single().IsDisposed, Is.True);
            }
        }

        [Test]
        [TestCase(ParameterDirection.Output)]
        [TestCase(ParameterDirection.InputOutput)]
        [TestCase(ParameterDirection.ReturnValue)]
        public void Execute_WhenParameterIsNotInputDirection_ShouldWriteBackValue(ParameterDirection direction) {
            FakeDbProviderFactory factory = new() {
                OnExecuting = command => command.FakeParameters.Single(x => x.ParameterName == "Total").Value = 42
            };
            using CommandExecutor executor = new(factory, "Fake", true);
            executor.SetCommandText("dbo.CountTest", CommandType.StoredProcedure)
                .Parameters.Add("Total", 0, DbType.Int32, direction);

            executor.Execute(ResetItems.None);

            Assert.That(executor.Parameters.GetValue<int>("Total"), Is.EqualTo(42));
        }

        [Test]
        public void Execute_WhenParametersReset_ShouldWriteBackValueToMetadata() {
            FakeDbProviderFactory factory = new() {
                OnExecuting = command => command.FakeParameters.Single().Value = "Wing"
            };
            ParameterMetadata output = new() {
                ParameterName = "@Name",
                Direction = ParameterDirection.Output,
                DbType = DbType.String,
                Size = 50
            };
            using CommandExecutor executor = new(factory, "Fake", true);
            executor.SetCommandText("SELECT @Name = Name FROM Test WHERE Id = 1")
                .Parameters.Add(output);

            executor.Execute();

            using (Assert.EnterMultipleScope()) {
                Assert.That(executor.Parameters, Is.Empty);
                Assert.That(output.Value, Is.EqualTo("Wing"));
            }
        }

        [Test]
        public async Task ExecuteAsync_WhenParameterIsOutput_ShouldWriteBackValue() {
            FakeDbProviderFactory factory = new() {
                OnExecuting = command => command.FakeParameters.Single().Value = 42L
            };
            using CommandExecutor executor = new(factory, "Fake", false);
            executor.SetCommandText("dbo.CountTest", CommandType.StoredProcedure)
                .Parameters.Add("Total", null, DbType.Int64, ParameterDirection.Output);

            await executor.ExecuteAsync(ResetItems.None);

            Assert.That(executor.Parameters.GetValue<long?>("Total"), Is.EqualTo(42L));
        }

        [Test]
        public void QueryScalar_WhenParameterIsOutput_ShouldWriteBackValue() {
            FakeDbProviderFactory factory = new() {
                ScalarResult = 1,
                OnExecuting = command => command.FakeParameters.Single().Value = 42
            };
            using CommandExecutor executor = new(factory, "Fake", true);
            executor.SetCommandText("dbo.CountTest", CommandType.StoredProcedure)
                .Parameters.Add("Total", null, DbType.Int32, ParameterDirection.Output);

            executor.QueryScalar(ResetItems.None);

            Assert.That(executor.Parameters.GetValue<int>("Total"), Is.EqualTo(42));
        }

        [Test]
        public void CreateDataReader_WhenParameterIsOutput_ShouldWriteBackValueAfterReaderClosed() {
            FakeDbProviderFactory factory = new() {
                OnReaderClosed = command => command.FakeParameters.Single().Value = 42
            };
            using CommandExecutor executor = new(factory, "Fake", true);
            executor.SetCommandText("dbo.QueryTest", CommandType.StoredProcedure)
                .Parameters.Add("Total", null, DbType.Int32, ParameterDirection.Output);
            IDataReader dataReader = executor.CreateDataReader(ResetItems.None);
            int? valueBeforeClose = executor.Parameters.GetValue<int?>("Total");

            dataReader.Dispose();

            using (Assert.EnterMultipleScope()) {
                Assert.That(valueBeforeClose, Is.Null);
                Assert.That(executor.Parameters.GetValue<int?>("Total"), Is.EqualTo(42));
            }
        }

        [Test]
        public async Task CreateDataReaderAsync_WhenParameterIsOutput_ShouldWriteBackValueOnNextExecution() {
            FakeDbProviderFactory factory = new() {
                OnReaderClosed = command => command.FakeParameters.Single().Value = 42
            };
            ParameterMetadata output = new() {
                ParameterName = "Total",
                Direction = ParameterDirection.Output
            };
            using CommandExecutor executor = new(factory, "Fake", false);
            executor.SetCommandText("dbo.QueryTest", CommandType.StoredProcedure)
                .Parameters.Add(output);
            DbDataReader dataReader = await executor.CreateDataReaderAsync();
            dataReader.Dispose();

            await executor.SetCommandText("SELECT 1").ExecuteAsync();

            Assert.That(output.Value, Is.EqualTo(42));
        }

        [Test]
        public void GetValue_WhenValueIsDbNull_ShouldReturnDefault() {
            using CommandExecutor executor = new();
            executor.Parameters.Add("Total", DBNull.Value);

            using (Assert.EnterMultipleScope()) {
                Assert.That(executor.Parameters.GetValue<int>("Total"), Is.Zero);
                Assert.That(executor.Parameters.GetValue<int?>("Total"), Is.Null);
                Assert.That(executor.Parameters.GetValue<string>("@Total"), Is.Null);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task CreateDataTableAsync_WhenComparedWithCreateDataTable_ShouldProduceSameSchemaAndRows(bool keepConnection) {
            const string sql = "SELECT Id, Name, Name, NULL AS Note FROM Test ORDER BY Id";
            using CommandExecutor executor = new(keepConnection);

            DataTable expected = executor.SetCommandText(sql).CreateDataTable();
            DataTable actual = await executor.SetCommandText(sql).CreateDataTableAsync();

            using (Assert.EnterMultipleScope()) {
                Assert.That(DescribeColumns(actual), Is.EqualTo(DescribeColumns(expected)));
                Assert.That(actual.PrimaryKey.Select(x => x.ColumnName), Is.EqualTo(expected.PrimaryKey.Select(x => x.ColumnName)));
                Assert.That(actual.Rows.Cast<DataRow>().Select(x => x.ItemArray), Is.EqualTo(expected.Rows.Cast<DataRow>().Select(x => x.ItemArray)));
                Assert.That(actual.Rows.Cast<DataRow>().Select(x => x.RowState), Is.EqualTo(expected.Rows.Cast<DataRow>().Select(x => x.RowState)));
            }
        }

        private static IEnumerable<string> DescribeColumns(DataTable dataTable) {
            return dataTable.Columns
                .Cast<DataColumn>()
                .Select(x => $"{x.ColumnName}|{x.DataType}|{x.AllowDBNull}|{x.Unique}|{x.AutoIncrement}|{x.ReadOnly}|{x.MaxLength}")
                .ToList();
        }

        [Test]
        public void Execute_WhenParameterNamePrefixContainsDollarSign_ShouldKeepPrefixLiteral() {
            FakeDbProviderFactory factory = new();
            FacadeConfiguration.ParameterNamePrefix = "P$0";
            using CommandExecutor executor = new(factory, "Fake", true) {
                CommandText = "SELECT * FROM Test WHERE Id IN @Ids"
            };
            executor.Parameters.Add("Ids", new[] { 1, 2 });

            executor.Execute();

            Assert.That(factory.ExecutedCommands.Single().CommandText, Is.EqualTo("SELECT * FROM Test WHERE Id IN (@P$0_Ids_0, @P$0_Ids_1)"));
        }

        [Test]
        public void Execute_WhenEnumerableParameterIsEmpty_ShouldReplaceWithNull() {
            FakeDbProviderFactory factory = new();
            using CommandExecutor executor = new(factory, "Fake", true) {
                CommandText = "SELECT * FROM Test WHERE Id NOT IN @Ids"
            };
            executor.Parameters.Add("Ids", Array.Empty<int>());

            executor.Execute();

            FakeDbCommand command = factory.ExecutedCommands.Single();
            using (Assert.EnterMultipleScope()) {
                Assert.That(command.CommandText, Is.EqualTo("SELECT * FROM Test WHERE Id NOT IN (NULL)"));
                Assert.That(command.FakeParameters, Is.Empty);
            }
        }

        [Test]
        public void Execute_WhenParameterNameCaseDiffersFromCommandText_ShouldAddParameter() {
            FakeDbProviderFactory factory = new();
            using CommandExecutor executor = new(factory, "Fake", true) {
                CommandText = "SELECT Name FROM Test WHERE Id = @ID"
            };
            executor.Parameters.Add("id", 1);

            executor.Execute();

            Assert.That(factory.ExecutedCommands.Single().FakeParameters.Select(x => x.ParameterName), Is.EqualTo(new[] { "id" }));
        }

        [Test]
        public void Execute_WhenGlobalAndInstanceHooksConfigured_ShouldInvokeGlobalHooksBeforeInstanceHooks() {
            FakeDbProviderFactory factory = new();
            List<string> calls = [];
            FacadeConfiguration.OnCommandCreating = (parameters, commandText) => calls.Add($"global creating:{commandText}:{parameters.Count}");
            FacadeConfiguration.OnCommandCreated = command => calls.Add($"global created:{command.CommandText}");
            using CommandExecutor executor = new(factory, "Fake", true) {
                CommandCreating = (parameters, commandText) => calls.Add($"instance creating:{commandText}:{parameters.Count}"),
                CommandCreated = command => calls.Add($"instance created:{command.CommandText}")
            };
            executor.SetCommandText("SELECT * FROM Test WHERE Id IN @Ids")
                .Parameters.Add("Ids", new[] { 1 });

            executor.Execute();

            Assert.That(calls, Is.EqualTo(new[] {
                "global creating:SELECT * FROM Test WHERE Id IN @Ids:1",
                "instance creating:SELECT * FROM Test WHERE Id IN @Ids:1",
                "global created:SELECT * FROM Test WHERE Id IN (@CloudyWing_Ids_0)",
                "instance created:SELECT * FROM Test WHERE Id IN (@CloudyWing_Ids_0)"
            }));
        }

        [Test]
        public void Execute_WhenInstanceHookConfigured_ShouldNotAffectOtherExecutors() {
            FakeDbProviderFactory factory = new();
            int count = 0;
            using CommandExecutor executor = new(factory, "Fake", true) {
                CommandText = "SELECT 1",
                CommandCreated = _ => count++
            };
            using CommandExecutor otherExecutor = new(factory, "Fake", true) {
                CommandText = "SELECT 1"
            };

            executor.Execute();
            otherExecutor.Execute();

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task BeginTransactionAsync_WhenRollbackAsyncInvoked_ShouldRevertChanges() {
            using CommandExecutor executor = new(true);
            await executor.BeginTransactionAsync();
            await executor.SetCommandText(InsertSql)
                .Parameters.AddRange(new { Id = 10, Name = "新增測試" })
                .GetCommandExecutor()
                .ExecuteAsync();

            await executor.RollbackAsync();

            long count = Convert.ToInt64(await executor.SetCommandText("SELECT COUNT(1) FROM Test").QueryScalarAsync());
            using (Assert.EnterMultipleScope()) {
                Assert.That(count, Is.EqualTo(basicRecords.Length));
                Assert.That(executor.Transaction, Is.Null);
            }
        }

        [Test]
        public async Task BeginTransactionAsync_WhenCommitAsyncInvoked_ShouldPersistChanges() {
            using CommandExecutor executor = new(true);
            await executor.BeginTransactionAsync(IsolationLevel.Serializable);
            await executor.SetCommandText(InsertSql)
                .Parameters.AddRange(new { Id = 10, Name = "新增測試" })
                .GetCommandExecutor()
                .ExecuteAsync();

            await executor.CommitAsync();

            long count = Convert.ToInt64(await executor.SetCommandText("SELECT COUNT(1) FROM Test").QueryScalarAsync());
            using (Assert.EnterMultipleScope()) {
                Assert.That(count, Is.EqualTo(basicRecords.Length + 1));
                Assert.That(executor.Transaction, Is.Null);
            }
        }

        [Test]
        public void BeginTransactionAsync_WhenKeepConnectionIsFalse_ShouldThrowKeepConnectionRequiredException() {
            using CommandExecutor executor = new(false);

            Assert.That(
                async () => await executor.BeginTransactionAsync(),
                Throws.TypeOf<KeepConnectionRequiredException>());
        }

        [Test]
        public void BeginTransactionAsync_WhenLevelProvided_ShouldUseLevel() {
            FakeDbProviderFactory factory = new();
            using CommandExecutor executor = new(factory, "Fake", true);

            DbTransaction transaction = executor.BeginTransactionAsync(IsolationLevel.Serializable).GetAwaiter().GetResult();

            using (Assert.EnterMultipleScope()) {
                Assert.That(transaction.IsolationLevel, Is.EqualTo(IsolationLevel.Serializable));
                Assert.That(executor.Transaction, Is.SameAs(transaction));
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Commit_WhenTransactionActive_ShouldCompleteAndReleaseTransaction(bool isCommit) {
            FakeDbProviderFactory factory = new();
            using CommandExecutor executor = new(factory, "Fake", true);
            FakeDbTransaction transaction = (FakeDbTransaction)executor.BeginTransaction();

            if (isCommit) {
                executor.Commit();
            } else {
                executor.Rollback();
            }

            using (Assert.EnterMultipleScope()) {
                Assert.That(transaction.IsCommitted, Is.EqualTo(isCommit));
                Assert.That(transaction.IsRolledBack, Is.EqualTo(!isCommit));
                Assert.That(transaction.IsDisposed, Is.True);
                Assert.That(executor.Transaction, Is.Null);
            }
        }

        [Test]
        public void Commit_WhenNoTransaction_ShouldThrowInvalidOperationException() {
            using CommandExecutor executor = new(true);

            using (Assert.EnterMultipleScope()) {
                Assert.That(() => executor.Commit(), Throws.InvalidOperationException);
                Assert.That(() => executor.Rollback(), Throws.InvalidOperationException);
                Assert.That(async () => await executor.CommitAsync(), Throws.InvalidOperationException);
                Assert.That(async () => await executor.RollbackAsync(), Throws.InvalidOperationException);
            }
        }

#if NET10_0_OR_GREATER
        [Test]
        public async Task DisposeAsync_WhenTransactionAndConnectionExist_ShouldReleaseResources() {
            FakeDbProviderFactory factory = new();
            CommandExecutor executor = new(factory, "Fake", true);
            FakeDbTransaction transaction = (FakeDbTransaction)await executor.BeginTransactionAsync();

            await executor.DisposeAsync();

            using (Assert.EnterMultipleScope()) {
                Assert.That(transaction.IsDisposed, Is.True);
                Assert.That(factory.CreatedConnections.Single().IsDisposed, Is.True);
                Assert.That(executor.Transaction, Is.Null);
                Assert.That(executor.Connection, Is.Null);
            }
        }

        [Test]
        public async Task DisposeAsync_WhenUsedWithAwaitUsing_ShouldDisposeReaderCommand() {
            FakeDbProviderFactory factory = new();
            DbDataReader dataReader;
            await using (CommandExecutor executor = new(factory, "Fake", true)) {
                dataReader = await executor.SetCommandText("SELECT 1").CreateDataReaderAsync();
            }

            using (Assert.EnterMultipleScope()) {
                Assert.That(factory.ExecutedCommands.Single().IsDisposed, Is.True);
                Assert.That(factory.CreatedConnections.Single().IsDisposed, Is.True);
            }

            await dataReader.DisposeAsync();
        }
#endif

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Query_WhenRowsExist_ShouldMapAllRows(bool keepConnection) {
            using CommandExecutor executor = CreateQueryExecutor(keepConnection);

            IReadOnlyList<Record> records = executor.Query(record => new Record {
                Name = record.GetString(1),
                Id = record.GetInt64(0)
            });

            using (Assert.EnterMultipleScope()) {
                Assert.That(records.Select(x => (x.Id, x.Name)), Is.EqualTo(basicRecords.Select(x => (x.Id, x.Name))));
                Assert.That(executor.CommandText, Is.Null);
                Assert.That(executor.Connection is null, Is.EqualTo(!keepConnection));
            }
        }

        [Test]
        public void Query_WhenMapIsNull_ShouldThrowArgumentNullException() {
            using CommandExecutor executor = CreateQueryExecutor(true);

            Assert.That(() => executor.Query<Record>(null!), Throws.ArgumentNullException);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void QueryScalarOfT_WhenValueTypeDiffers_ShouldConvertValue(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection) {
                CommandText = "SELECT COUNT(1) FROM Test"
            };

            int count = executor.QueryScalar<int>();

            Assert.That(count, Is.EqualTo(basicRecords.Length));
        }

        [Test]
        public void QueryScalarOfT_WhenResultIsEmptyOrNull_ShouldReturnDefault() {
            using CommandExecutor executor = new();

            int? empty = executor.SetCommandText("SELECT Id FROM Test WHERE Id = 100").QueryScalar<int?>();
            long nullValue = executor.SetCommandText("SELECT NULL").QueryScalar<long>();

            using (Assert.EnterMultipleScope()) {
                Assert.That(empty, Is.Null);
                Assert.That(nullValue, Is.Zero);
            }
        }

        [Test]
        public async Task QueryScalarAsyncOfT_WhenValueTypeDiffers_ShouldConvertValue() {
            using CommandExecutor executor = new() {
                CommandText = "SELECT COUNT(1) FROM Test"
            };

            decimal count = await executor.QueryScalarAsync<decimal>();

            Assert.That(count, Is.EqualTo(basicRecords.Length));
        }

#if NET10_0_OR_GREATER
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task QueryAsync_WhenEnumerated_ShouldStreamMappedRows(bool keepConnection) {
            using CommandExecutor executor = CreateQueryExecutor(keepConnection);
            List<Record> records = [];

            await foreach (Record record in executor.QueryAsync(x => new Record { Id = x.GetInt64(0), Name = x.GetString(1) })) {
                records.Add(record);
            }

            using (Assert.EnterMultipleScope()) {
                Assert.That(records.Select(x => (x.Id, x.Name)), Is.EqualTo(basicRecords.Select(x => (x.Id, x.Name))));
                Assert.That(executor.CommandText, Is.Null);
                Assert.That(executor.Connection is null, Is.EqualTo(!keepConnection));
            }
        }

        [Test]
        public async Task QueryAsync_WhenNotEnumerated_ShouldNotExecuteCommand() {
            FakeDbProviderFactory factory = new();
            using CommandExecutor executor = new(factory, "Fake", true) {
                CommandText = "SELECT 1"
            };

            IAsyncEnumerable<int> items = executor.QueryAsync(x => x.GetInt32(0));
            await Task.Yield();

            using (Assert.EnterMultipleScope()) {
                Assert.That(items, Is.Not.Null);
                Assert.That(factory.ExecutedCommands, Is.Empty);
                Assert.That(executor.CommandText, Is.EqualTo("SELECT 1"));
            }
        }

        [Test]
        public async Task QueryAsync_WhenEnumerationStoppedEarly_ShouldReleaseReaderAndConnection() {
            DataTable result = new();
            result.Columns.Add("Id", typeof(int));
            result.Rows.Add(1);
            result.Rows.Add(2);
            FakeDbProviderFactory factory = new() { ReaderResults = [result] };
            using CommandExecutor executor = new(factory, "Fake", false) {
                CommandText = "SELECT Id FROM Test"
            };

            await foreach (int id in executor.QueryAsync(x => x.GetInt32(0))) {
                break;
            }

            using (Assert.EnterMultipleScope()) {
                Assert.That(factory.ExecutedCommands.Single().IsDisposed, Is.True);
                Assert.That(factory.CreatedConnections.Single().IsDisposed, Is.True);
                Assert.That(executor.Connection, Is.Null);
            }
        }

        [Test]
        public void QueryAsync_WhenCancellationRequested_ShouldThrowOperationCanceledException() {
            using CancellationTokenSource cancellationTokenSource = new();
            cancellationTokenSource.Cancel();
            using CommandExecutor executor = CreateQueryExecutor(true);

            Assert.That(
                async () => {
                    await foreach (long id in executor.QueryAsync(x => x.GetInt64(0), cancellationToken: cancellationTokenSource.Token)) {
                    }
                },
                Throws.InstanceOf<OperationCanceledException>());
        }
#endif

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateDataSet_WhenMultipleResultSets_ShouldLoadEachResultSet(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection) {
                CommandText = "SELECT Id FROM Test ORDER BY Id; SELECT Name FROM Test WHERE Id = @Id"
            };
            executor.Parameters.Add("Id", 1);

            DataSet dataSet = executor.CreateDataSet();

            using (Assert.EnterMultipleScope()) {
                Assert.That(dataSet.Tables.Cast<DataTable>().Select(x => x.TableName), Is.EqualTo(new[] { "Table", "Table1" }));
                Assert.That(dataSet.Tables[0].Rows.Cast<DataRow>().Select(x => x["Id"]), Is.EqualTo(basicRecords.Select(x => (object)x.Id)));
                Assert.That(dataSet.Tables[1].Rows.Cast<DataRow>().Select(x => x["Name"]), Is.EqualTo(new[] { "Terry" }));
                Assert.That(executor.Parameters, Is.Empty);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task CreateDataSetAsync_WhenComparedWithCreateDataSet_ShouldProduceSameTables(bool keepConnection) {
            const string sql = "SELECT Id, Name FROM Test ORDER BY Id; SELECT Name, Name FROM Test WHERE Id > 0 ORDER BY Id";
            using CommandExecutor executor = new(keepConnection);

            DataSet expected = executor.SetCommandText(sql).CreateDataSet();
            DataSet actual = await executor.SetCommandText(sql).CreateDataSetAsync();

            using (Assert.EnterMultipleScope()) {
                Assert.That(actual.Tables, Has.Count.EqualTo(2));
                Assert.That(actual.Tables.Cast<DataTable>().Select(x => x.TableName), Is.EqualTo(expected.Tables.Cast<DataTable>().Select(x => x.TableName)));
                for (int i = 0; i < expected.Tables.Count; i++) {
                    Assert.That(DescribeColumns(actual.Tables[i]), Is.EqualTo(DescribeColumns(expected.Tables[i])));
                    Assert.That(actual.Tables[i].Rows.Cast<DataRow>().Select(x => x.ItemArray), Is.EqualTo(expected.Tables[i].Rows.Cast<DataRow>().Select(x => x.ItemArray)));
                }
            }
        }

        [Test]
        public void CreateDataSet_WhenFakeProviderReturnsResultSets_ShouldUseDataAdapterTableNames() {
            DataTable first = new();
            first.Columns.Add("Id", typeof(int));
            first.Rows.Add(1);
            DataTable second = new();
            second.Columns.Add("Name", typeof(string));
            second.Rows.Add("Wing");
            DataTable third = new();
            third.Columns.Add("Total", typeof(int));
            FakeDbProviderFactory factory = new() { ReaderResults = [first, second, third] };
            using CommandExecutor executor = new(factory, "Fake", false) {
                CommandText = "dbo.QueryTest"
            };

            DataSet dataSet = executor.CreateDataSet();

            using (Assert.EnterMultipleScope()) {
                Assert.That(dataSet.Tables.Cast<DataTable>().Select(x => x.TableName), Is.EqualTo(new[] { "Table", "Table1", "Table2" }));
                Assert.That(dataSet.Tables[2].Rows, Is.Empty);
                Assert.That(factory.CreatedConnections.Single().IsDisposed, Is.True);
            }
        }

        [Test]
        public void QueryScalar_WhenParameterNamesDifferOnlyInCase_ShouldBindEachParameterOnCaseSensitiveProvider() {
            using CommandExecutor executor = new() {
                CommandText = "SELECT @Id * 10 + @id"
            };
            executor.Parameters.Add("Id", 1).Add("id", 2);

            long result = executor.QueryScalar<long>();

            Assert.That(result, Is.EqualTo(12));
        }

        [TearDown]
        public void TearDown() {
            FacadeConfiguration.OnCommandCreating = null;
            FacadeConfiguration.OnCommandCreated = null;
            FacadeConfiguration.ParameterNamePrefix = "CloudyWing";

            using SqliteConnection conn = new(ConnectionString);
            conn.Open();

            using SqliteCommand cmd = new() {
                Connection = conn,
                CommandText = "DELETE FROM Test"
            };

            cmd.ExecuteNonQuery();
        }

        [OneTimeTearDown]
        public void Cleanup() {
            if (File.Exists(DbName)) {
                SqliteConnection.ClearAllPools();
                File.Delete(DbName);
            }
        }

        private class Record {
            public long Id { get; set; }

            public string? Name { get; set; }
        }
    }
}
