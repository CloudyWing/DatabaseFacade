using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace CloudyWing.DatabaseFacade.Tests.Fakes {
    internal sealed class FakeDbCommand : DbCommand {
        private readonly FakeDbConnection connection;
        private readonly FakeDbParameterCollection parameters = new();

        public FakeDbCommand(FakeDbConnection connection) {
            this.connection = connection;
        }

        public bool IsDisposed { get; private set; }

        public CommandBehavior? ExecutedBehavior { get; private set; }

        public IEnumerable<FakeDbParameter> FakeParameters => parameters.Cast<FakeDbParameter>();

        [AllowNull]
        public override string CommandText { get; set; } = "";

        public override int CommandTimeout { get; set; }

        public override CommandType CommandType { get; set; }

        public override bool DesignTimeVisible { get; set; }

        public override UpdateRowSource UpdatedRowSource { get; set; }

        protected override DbConnection? DbConnection {
            get => connection;
            set { }
        }

        protected override DbParameterCollection DbParameterCollection => parameters;

        protected override DbTransaction? DbTransaction { get; set; }

        public override void Cancel() {
        }

        public override int ExecuteNonQuery() {
            Execute();
            return 1;
        }

        public override object? ExecuteScalar() {
            Execute();
            return connection.Factory.ScalarResult;
        }

        public override void Prepare() {
        }

        protected override DbParameter CreateDbParameter() {
            return new FakeDbParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) {
            Execute();
            ExecutedBehavior = behavior;
            return new FakeDbDataReader(this, connection.Factory.ReaderResults, behavior);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                IsDisposed = true;
            }

            base.Dispose(disposing);
        }

        internal void CloseReader(CommandBehavior behavior) {
            connection.Factory.OnReaderClosed?.Invoke(this);
            if ((behavior & CommandBehavior.CloseConnection) == CommandBehavior.CloseConnection) {
                connection.Close();
            }
        }

        private void Execute() {
            if (connection.State != ConnectionState.Open) {
                throw new InvalidOperationException("Connection is not open.");
            }

            connection.Factory.ExecutedCommands.Add(this);
            connection.Factory.OnExecuting?.Invoke(this);
        }
    }
}
