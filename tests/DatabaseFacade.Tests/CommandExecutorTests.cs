using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace CloudyWing.DatabaseFacade.Tests {
    public class CommandExecutorTests {
        private const string DbName = "Test.db";
        private const string ConnectionString = $"Data Source={DbName};";
        private const string InsertSql = "INSERT INTO Test (Id, Name) VALUES (@Id, @Name)";

        private readonly Record[] BasicRecords = new Record[] {
            new Record { Id=  0, Name = "Wing" },
            new Record { Id = 1, Name = "Terry" },
            new Record { Id = 2, Name = "Marry" }
        };

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

            foreach (Record record in BasicRecords) {
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
        public void DbProviderFactory_構造參數是否覆蓋Configuration設定_應該設置為指定的值() {
            // Init 已設定過 DbProviderFactory
            CommandExecutor executor = new();
            executor.DbProviderFactory.Should().BeSameAs(SqliteFactory.Instance);

            executor = new(null, "Test");
            executor.DbProviderFactory.Should().BeSameAs(SqliteFactory.Instance);

            executor = new(FakeDbProviderFactory.Instance, "Test");
            executor.DbProviderFactory.Should().BeSameAs(FakeDbProviderFactory.Instance);
        }

        [Test]
        public void ConnectionString_構造參數是否覆蓋Configuration設定_應該設置為指定的值() {
            // Init 已設定過 ConnectionString
            CommandExecutor executor = new();
            executor.ConnectionString.Should().BeSameAs(ConnectionString);

            executor = new(null, null);
            executor.ConnectionString.Should().BeSameAs(ConnectionString);

            executor = new("Test");
            executor.ConnectionString.Should().BeSameAs("Test");
        }

        [Test]
        public void KeepConnection_設為True並執行Execute_Connection應該維持Open狀態() {
            using CommandExecutor executor = new(true) {
                CommandText = "SELECT COUNT(1) FROM Test"
            };
            executor.Execute();
            bool result = executor.Connection.State == ConnectionState.Open;

            executor.Connection.State.Should().Be(ConnectionState.Open);
        }

        [Test]
        public void KeepConnection_設為False並執行Execute_Connection應該設為Null() {
            using CommandExecutor executor = new(false) {
                CommandText = "SELECT COUNT(1) FROM Test"
            };
            executor.Execute();

            executor.Connection.Should().BeNull();
        }

        [Test]
        public void KeepConnection_構造參數傳入bool時覆蓋預設值_應該設置為指定值() {
            FacadeConfiguration.DefaultKeepConnection = true;

            CommandExecutor executor = new();
            executor.KeepConnection.Should().Be(true);

            executor = new(null);
            executor.KeepConnection.Should().Be(true);

            executor = new(false);
            executor.KeepConnection.Should().Be(false);
        }

        [Test]
        public void ParameterNamePrefix_應以指定前綴設置參數名稱() {
            FacadeConfiguration.ParameterNamePrefix = "Test";
            FacadeConfiguration.OnCommandCreated += (cmd) => {
                int i = 0;
                foreach (IDbDataParameter parameter in cmd.Parameters) {
                    parameter.ParameterName.Should().Be("Test_Ids_" + i++);
                }
            };

            using CommandExecutor executor = new() {
                CommandText = "SELECT * FROM Test WHERE Id IN @Ids ORDER BY Id"
            };
            executor.Parameters.Add("Ids", new long[] { 1, 2 });
            DataTable dt = executor.CreateDataTable();
        }

        [Test]
        [TestCase("Text1", null)]
        [TestCase("Text2", CommandType.Text)]
        [TestCase("Text3", CommandType.StoredProcedure)]
        [TestCase("Text3", CommandType.TableDirect)]
        public void SetCommandText_傳入CommandText和CommandType_應設置為指定值(string commandText, CommandType? commandType) {
            CommandExecutor executor = new CommandExecutor()
                .SetCommandText(commandText, commandType);

            executor.CommandText.Should().Be(commandText);
            if (commandType.HasValue) {
                executor.CommandType.Should().Be(commandType.Value);
            } else {
                executor.CommandType = CommandType.Text;
            }
        }

        [Test]
        public void SetCommandTimeout_傳入秒數_應設置為指定秒數() {
            CommandExecutor executor = new CommandExecutor();
            executor.CommandTimeout.Should().Be(FacadeConfiguration.DefaultCommandTimeout);

            int newSecond = FacadeConfiguration.DefaultCommandTimeout + 100;
            executor.SetCommandTimeout(newSecond);
            executor.CommandTimeout.Should().Be(newSecond);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateDataReader_應回傳符合查詢條件的記錄集合(bool keepConnection) {
            IEnumerable<Record> list = QueryRecordsByDataReader(keepConnection);

            list.Should().BeEquivalentTo(BasicRecords);
        }

        private IEnumerable<Record> QueryRecordsByDataReader(bool keepConnection) {
            using CommandExecutor executor = CreateQueryExecutor(keepConnection);
            using IDataReader dr = executor.CreateDataReader();

            while (dr.Read()) {
                yield return new Record { Id = (long)dr["Id"], Name = (string)dr["Name"] };
            }
        }

        private static CommandExecutor CreateQueryExecutor(bool keepConnection) {
            return new CommandExecutor(keepConnection) {
                CommandText = "SELECT * FROM Test ORDER BY Id"
            };
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateDataTable_應回傳符合查詢條件的記錄集合(bool keepConnection) {
            using CommandExecutor executor = CreateQueryExecutor(keepConnection);
            DataTable dt = executor.CreateDataTable();

            IEnumerable<Record> list = ConvertFrom(dt);

            list.Should().BeEquivalentTo(BasicRecords);
        }

        private IEnumerable<Record> ConvertFrom(DataTable dataTable) {
            foreach (DataRow dr in dataTable.Rows) {
                yield return new Record { Id = (long)dr["Id"], Name = (string)dr["Name"] };
            }
        }

        [Test]
        public void Query_Where條件有使用且ParameterValue為IEnumerable_會換成複數Parameter() {
            using CommandExecutor executor = new() {
                CommandText = "SELECT * FROM Test WHERE Id IN @Ids ORDER BY Id"
            };
            executor.Parameters.Add("Ids", new long[] { 1, 2, 10 }); // BasicRecords 沒有 Id = 10 的資料
            DataTable dt = executor.CreateDataTable();
            IEnumerable<Record> records = ConvertFrom(dt);

            records.Count().Should().Be(2);
            records.ElementAt(0).Should().BeEquivalentTo(BasicRecords.Single(x => x.Id == 1));
            records.ElementAt(1).Should().BeEquivalentTo(BasicRecords.Single(x => x.Id == 2));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void QueryScalar_應回傳符合查詢條件的第一行第一次的值(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection) {
                CommandText = "SELECT COUNT(1) FROM Test"
            };
            long count = (long)executor.QueryScalar();

            count.Should().Be(BasicRecords.Length);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Execute_執行Insert_應成功新增資料(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection) {
                CommandText = "INSERT INTO Test (Id, Name) VALUES (10, '新增測試')"
            };
            int result = executor.Execute();

            IEnumerable<Record> records = QueryRecordsByDataReader(keepConnection);
            Record? insertedRecord = records.SingleOrDefault(x => x.Id == 10);

            result.Should().Be(1);
            records.Count().Should().Be(BasicRecords.Length + 1);
            insertedRecord.Should().NotBeNull();
            insertedRecord?.Id.Should().Be(10);
            insertedRecord?.Name.Should().Be("新增測試");
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Execute_參數使用ParameterMetadata來執行Insert_應成功新增資料(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection);
            int result = executor.SetCommandText(InsertSql)
                .Parameters.Add(new ParameterMetadata { ParameterName = "Id", Value = 10 })
                    .Add(new ParameterMetadata { ParameterName = "Name", Value = "新增測試" })
                    .GetCommandExecutor()
                .Execute();

            IEnumerable<Record> records = QueryRecordsByDataReader(keepConnection);
            Record? insertedRecord = records.SingleOrDefault(x => x.Id == 10);

            result.Should().Be(1);
            records.Count().Should().Be(BasicRecords.Length + 1);
            insertedRecord.Should().NotBeNull();
            insertedRecord?.Id.Should().Be(10);
            insertedRecord?.Name.Should().Be("新增測試");
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Execute_參數使用SqlParameter來執行Insert_應成功新增資料(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection);
            int result = executor.SetCommandText(InsertSql)
                .Parameters.Add(new SqliteParameter { ParameterName = "Id", Value = 10 })
                    .Add(new SqliteParameter { ParameterName = "Name", Value = "新增測試" })
                    .GetCommandExecutor()
                .Execute();

            IEnumerable<Record> records = QueryRecordsByDataReader(keepConnection);
            Record? insertedRecord = records.SingleOrDefault(x => x.Id == 10);

            result.Should().Be(1);
            records.Count().Should().Be(BasicRecords.Length + 1);
            insertedRecord.Should().NotBeNull();
            insertedRecord?.Id.Should().Be(10);
            insertedRecord?.Name.Should().Be("新增測試");
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Execute_參數使用Object來執行Insert_應成功新增資料(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection);
            int result = executor.SetCommandText(InsertSql)
                .Parameters.AddRange(new { Id = 10, Name = "新增測試" })
                    .GetCommandExecutor()
                .Execute();

            IEnumerable<Record> records = QueryRecordsByDataReader(keepConnection);
            Record? insertedRecord = records.SingleOrDefault(x => x.Id == 10);

            result.Should().Be(1);
            records.Count().Should().Be(BasicRecords.Length + 1);
            insertedRecord.Should().NotBeNull();
            insertedRecord?.Id.Should().Be(10);
            insertedRecord?.Name.Should().Be("新增測試");
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Execute_參數使用Dictionary來執行Insert_應成功新增資料(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection);
            int result = executor.SetCommandText(InsertSql)
                .Parameters.AddRange(new Dictionary<string, object> { ["Id"] = 10, ["Name"] = "新增測試" })
                .GetCommandExecutor()
                .Execute();

            IEnumerable<Record> records = QueryRecordsByDataReader(keepConnection);
            Record? insertedRecord = records.SingleOrDefault(x => x.Id == 10);

            result.Should().Be(1);
            records.Count().Should().Be(BasicRecords.Length + 1);
            insertedRecord.Should().NotBeNull();
            insertedRecord?.Id.Should().Be(10);
            insertedRecord?.Name.Should().Be("新增測試");
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Execute_執行Update_應成功修改資料(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection) {
                CommandText = "UPDATE Test SET Name = '修改測試' WHERE Id = 1"
            };
            int result = executor.Execute();

            Record? record = QueryRecordsByDataReader(keepConnection).SingleOrDefault(x => x.Id == 1);

            result.Should().Be(1);
            record.Should().NotBeNull();
            record?.Name.Should().Be("修改測試");
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Execute_執行Delete_應成功刪除資料(bool keepConnection) {
            using CommandExecutor executor = new(keepConnection) {
                CommandText = "DELETE FROM Test WHERE Id = 1"
            };
            int result = executor.Execute();

            IEnumerable<Record> records = QueryRecordsByDataReader(keepConnection);
            Record? deletedRecord = records.SingleOrDefault(x => x.Id == 1);

            result.Should().Be(1);
            deletedRecord.Should().BeNull();
            records.Count().Should().Be(BasicRecords.Length - 1);
        }

        [Test]
        public void BeginTransaction_KeepConnection為False_ShouldThrowKeepConnectionRequiredException() {
            using CommandExecutor executor = new(false);
            Func<IDbTransaction> act = () => executor.BeginTransaction();

            act.Should().Throw<KeepConnectionRequiredException>()
                .WithMessage("KeepConnection must be set to true to use BeginTransaction.");
        }

        [Test]
        public void BeginTransaction_執行Rollback_應成功還原資料() {
            using CommandExecutor executor = new(true);
            using IDbTransaction transaction = executor.BeginTransaction();

            int result = executor.SetCommandText(InsertSql)
                .Parameters.AddRange(
                    new SqliteParameter { ParameterName = "Id", Value = 10 },
                    new SqliteParameter { ParameterName = "Name", Value = "新增測試" }
                ).GetCommandExecutor()
                .Execute();

            result.Should().Be(1);

            transaction.Rollback();

            DataTable dt = executor.SetCommandText("SELECT * FROM Test ORDER BY Id")
                .CreateDataTable();
            IEnumerable<Record> records = ConvertFrom(dt);
            Record? insteredRecord = records.SingleOrDefault(x => x.Id == 10);

            records.Count().Should().Be(BasicRecords.Length);
            insteredRecord.Should().BeNull();
        }

        [Test]
        public void BeginTransaction_執行Commit_應成功寫入資料() {
            using CommandExecutor executor = new(true);
            using IDbTransaction transaction = executor.BeginTransaction();

            int result = executor.SetCommandText(InsertSql)
                .Parameters.AddRange(
                    new SqliteParameter { ParameterName = "Id", Value = 10 },
                    new SqliteParameter { ParameterName = "Name", Value = "新增測試" }
                ).GetCommandExecutor()
                .Execute();

            result.Should().Be(1);

            transaction.Commit();

            DataTable dt = executor.SetCommandText("SELECT * FROM Test ORDER BY Id")
                .CreateDataTable();
            IEnumerable<Record> records = ConvertFrom(dt);
            Record? insteredRecord = records.SingleOrDefault(x => x.Id == 10);

            records.Count().Should().Be(BasicRecords.Length + 1);
            insteredRecord?.Id.Should().Be(10);
            insteredRecord?.Name.Should().Be("新增測試");
        }

        [Test]
        [TestCase(ResetItems.None)]
        [TestCase(ResetItems.CommandText)]
        [TestCase(ResetItems.CommandTimeout)]
        [TestCase(ResetItems.CommandType)]
        [TestCase(ResetItems.Parameters)]
        [TestCase(ResetItems.All)]
        public void Initialize(ResetItems items) {
            using CommandExecutor executor = CreateQueryExecutor(false);
            executor.CreateDataTable(items);

            if ((items & ResetItems.CommandText) == ResetItems.CommandText) {
                executor.CommandText.Should().BeNull();
            }

            if ((items & ResetItems.CommandTimeout) == ResetItems.CommandTimeout) {
                executor.CommandTimeout.Should().Be(FacadeConfiguration.DefaultCommandTimeout);
            }

            if ((items & ResetItems.CommandType) == ResetItems.CommandType) {
                executor.CommandType.Should().Be(CommandType.Text);
            }

            if ((items & ResetItems.Parameters) == ResetItems.Parameters) {
                executor.Parameters.Count.Should().Be(0);
            }
        }

        [TearDown]
        public void TearDown() {
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
