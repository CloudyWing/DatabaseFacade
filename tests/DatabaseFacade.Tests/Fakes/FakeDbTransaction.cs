using System.Data;
using System.Data.Common;

namespace CloudyWing.DatabaseFacade.Tests.Fakes {
    internal sealed class FakeDbTransaction : DbTransaction {
        private FakeDbConnection? connection;

        public FakeDbTransaction(FakeDbConnection connection, IsolationLevel isolationLevel) {
            this.connection = connection;
            IsolationLevel = isolationLevel;
        }

        public bool IsCommitted { get; private set; }

        public bool IsRolledBack { get; private set; }

        public bool IsDisposed { get; private set; }

        public override IsolationLevel IsolationLevel { get; }

        protected override DbConnection? DbConnection => connection;

        public override void Commit() {
            IsCommitted = true;
            connection = null;
        }

        public override void Rollback() {
            IsRolledBack = true;
            connection = null;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                IsDisposed = true;
                connection = null;
            }

            base.Dispose(disposing);
        }
    }
}
