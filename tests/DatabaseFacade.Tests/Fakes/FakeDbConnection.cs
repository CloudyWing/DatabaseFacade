using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace CloudyWing.DatabaseFacade.Tests.Fakes {
    internal sealed class FakeDbConnection : DbConnection {
        private ConnectionState state = ConnectionState.Closed;

        public FakeDbConnection(FakeDbProviderFactory factory) {
            Factory = factory;
        }

        public FakeDbProviderFactory Factory { get; }

        public bool IsDisposed { get; private set; }

        public List<FakeDbTransaction> Transactions { get; } = [];

        [AllowNull]
        public override string ConnectionString { get; set; } = "";

        public override string Database => "Fake";

        public override string DataSource => "Fake";

        public override string ServerVersion => "1.0";

        public override ConnectionState State => state;

        public override void ChangeDatabase(string databaseName) {
        }

        public override void Close() {
            state = ConnectionState.Closed;
        }

        public override void Open() {
            if (Factory.ShouldFailOnOpen) {
                throw new InvalidOperationException("Fake open failure.");
            }

            state = ConnectionState.Open;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) {
            FakeDbTransaction transaction = new(this, isolationLevel);
            Transactions.Add(transaction);
            return transaction;
        }

        protected override DbCommand CreateDbCommand() {
            return new FakeDbCommand(this);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                IsDisposed = true;
                state = ConnectionState.Closed;
            }

            base.Dispose(disposing);
        }
    }
}
