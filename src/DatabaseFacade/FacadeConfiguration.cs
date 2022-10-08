using System;
using System.Data;
using System.Data.Common;

namespace CloudyWing.DatabaseFacade {
    public static class FacadeConfiguration {
        public static DbProviderFactory DefaultDbProviderFactory { get; set; }

        public static string DefaultConnectionString { get; set; }

        public static bool DefaultKeepConnection { get; set; }

        public static int DefaultCommandTimeout { get; set; } = 30;

        /// <summary>
        /// 如果ParameterValue是IEnumerable型別，則會將ParameterName轉換成{ParameterNamePrefix}_{ParameterName}_{流水號}
        /// </summary>
        public static string ParameterNamePrefix { get; set; } = "CloudyWing";

        public static Action<ParameterCollection, string> OnCommandCreating { get; set; }

        public static Action<IDbCommand> OnCommandCreated { get; set; }

        public static IsolationLevel DefaultIsolationLevel { get; set; } = IsolationLevel.ReadCommitted;

        public static void SetConfiguration(DbProviderFactory dbProviderFactory, string connectionString) {
            DefaultDbProviderFactory = dbProviderFactory;
            DefaultConnectionString = connectionString;
        }
    }
}
