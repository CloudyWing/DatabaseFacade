using System;
using System.Data;
using System.Data.Common;

namespace CloudyWing.DatabaseFacade {
    /// <summary>
    /// Provides global defaults used when creating <see cref="CommandExecutor" /> instances.
    /// </summary>
    public static class FacadeConfiguration {
        /// <summary>
        /// Gets or sets the default database provider factory used to create connections and commands.
        /// </summary>
        public static DbProviderFactory? DefaultDbProviderFactory { get; set; }

        /// <summary>
        /// Gets or sets the default connection string.
        /// </summary>
        public static string? DefaultConnectionString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether connections stay open by default after command execution.
        /// </summary>
        public static bool DefaultKeepConnection { get; set; }

        /// <summary>
        /// Gets or sets the default command timeout, in seconds.
        /// </summary>
        public static int DefaultCommandTimeout { get; set; } = 30;

        /// <summary>
        /// Gets or sets the prefix used when an <see cref="System.Collections.IEnumerable" /> parameter is expanded into multiple provider parameters.
        /// Expanded names follow the pattern <c>{ParameterNamePrefix}_{parameterName}_{index}</c>.
        /// </summary>
        public static string ParameterNamePrefix { get; set; } = "CloudyWing";

        /// <summary>
        /// Gets or sets the callback invoked before a command is created, so the queued parameters and SQL text can be inspected.
        /// </summary>
        public static Action<ParameterCollection, string?>? OnCommandCreating { get; set; }

        /// <summary>
        /// Gets or sets the callback invoked after a command has been created and populated.
        /// </summary>
        public static Action<IDbCommand>? OnCommandCreated { get; set; }

        /// <summary>
        /// Gets or sets the default isolation level used by <see cref="CommandExecutor.BeginTransaction" />.
        /// </summary>
        public static IsolationLevel DefaultIsolationLevel { get; set; } = IsolationLevel.ReadCommitted;

        /// <summary>
        /// Sets the minimum configuration required to start using <see cref="CommandExecutor" />.
        /// </summary>
        /// <param name="dbProviderFactory">The provider factory used to create ADO.NET objects.</param>
        /// <param name="connectionString">The default connection string.</param>
        public static void SetConfiguration(DbProviderFactory dbProviderFactory, string connectionString) {
            DefaultDbProviderFactory = dbProviderFactory;
            DefaultConnectionString = connectionString;
        }
    }
}
