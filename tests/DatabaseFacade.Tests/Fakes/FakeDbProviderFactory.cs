using System.Data;
using System.Data.Common;

namespace CloudyWing.DatabaseFacade.Tests.Fakes {
    /// <summary>
    /// 不連線真實資料庫的 provider，用來驗證 CommandExecutor 建構的 DbCommand 與參數。
    /// </summary>
    internal sealed class FakeDbProviderFactory : DbProviderFactory {
        public List<FakeDbConnection> CreatedConnections { get; } = [];

        public List<FakeDbCommand> ExecutedCommands { get; } = [];

        /// <summary>
        /// 執行 DbCommand 時呼叫，可模擬 provider 寫回 Output 參數或拋出例外。
        /// </summary>
        public Action<FakeDbCommand>? OnExecuting { get; set; }

        /// <summary>
        /// ExecuteScalar 回傳的值。
        /// </summary>
        public object? ScalarResult { get; set; }

        /// <summary>
        /// ExecuteReader 回傳的資料，每個 DataTable 對應一個 result set。
        /// </summary>
        public IReadOnlyList<DataTable> ReaderResults { get; set; } = [];

        /// <summary>
        /// 在 DataReader 關閉時呼叫，模擬 SqlClient 在讀取器關閉後才寫回 Output 參數的行為。
        /// </summary>
        public Action<FakeDbCommand>? OnReaderClosed { get; set; }

        public bool ShouldFailOnOpen { get; set; }

        public override DbConnection CreateConnection() {
            FakeDbConnection connection = new(this);
            CreatedConnections.Add(connection);
            return connection;
        }

        public override DbParameter CreateParameter() {
            return new FakeDbParameter();
        }
    }
}
