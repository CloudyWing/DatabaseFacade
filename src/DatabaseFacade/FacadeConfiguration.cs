using System;
using System.Data;
using System.Data.Common;

namespace CloudyWing.DatabaseFacade {
    /// <summary>The facade configuration.</summary>
    public static class FacadeConfiguration {
        /// <summary>Gets or sets the default database provider factory.</summary>
        /// <value>The default database provider factory.</value>
        public static DbProviderFactory DefaultDbProviderFactory { get; set; }

        /// <summary>Gets or sets the default connection string.</summary>
        /// <value>The default connection string.</value>
        public static string DefaultConnectionString { get; set; }

        /// <summary>Gets or sets a value indicating whether [default keep connection].</summary>
        /// <value>
        ///   <c>true</c> if [default keep connection]; otherwise, <c>false</c>.</value>
        public static bool DefaultKeepConnection { get; set; }

        /// <summary>Gets or sets the default command timeout.</summary>
        /// <value>The default command timeout.</value>
        public static int DefaultCommandTimeout { get; set; } = 30;

        /// <summary>Gets or sets the parameter name prefix.
        /// if parameter value is <c>IEnumerable</c>，parameter name will be converted to <c>{ParameterNamePrefix}_{parameter name}_{serial number}</c></summary>
        /// <value>The parameter name prefix.</value>
        public static string ParameterNamePrefix { get; set; } = "CloudyWing";

        /// <summary>Gets or sets the on command creating.</summary>
        /// <value>The on command creating.</value>
        public static Action<ParameterCollection, string> OnCommandCreating { get; set; }

        /// <summary>Gets or sets the on command created.</summary>
        /// <value>The on command created.</value>
        public static Action<IDbCommand> OnCommandCreated { get; set; }

        /// <summary>Gets or sets the default isolation level.</summary>
        /// <value>The default isolation level.</value>
        public static IsolationLevel DefaultIsolationLevel { get; set; } = IsolationLevel.ReadCommitted;

        /// <summary>Sets the configuration.</summary>
        /// <param name="dbProviderFactory">The database provider factory.</param>
        /// <param name="connectionString">The connection string.</param>
        public static void SetConfiguration(DbProviderFactory dbProviderFactory, string connectionString) {
            DefaultDbProviderFactory = dbProviderFactory;
            DefaultConnectionString = connectionString;
        }
    }
}
