using System.Data;
using System.Data.Common;
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

            executor = new(FakeDbProviderFactory.Instance, "Test");
            Assert.That(executor.DbProviderFactory, Is.SameAs(FakeDbProviderFactory.Instance));
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

        private class FakeDbProviderFactory : DbProviderFactory {
            public static FakeDbProviderFactory Instance { get; } = new();
        }
    }
}
