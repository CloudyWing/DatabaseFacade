using System;

namespace CloudyWing.DatabaseFacade {
    /// <summary>
    /// Specifies which parts of a <see cref="CommandExecutor" /> should be reset after an operation completes.
    /// </summary>
    [Flags]
    public enum ResetItems {
        /// <summary>
        /// Do not reset any command state.
        /// </summary>
        None = 0,

        /// <summary>
        /// Reset <see cref="CommandExecutor.CommandText" />.
        /// </summary>
        CommandText = 1,

        /// <summary>
        /// Reset <see cref="CommandExecutor.CommandTimeout" />.
        /// </summary>
        CommandTimeout = 2,

        /// <summary>
        /// Reset <see cref="CommandExecutor.CommandType" />.
        /// </summary>
        CommandType = 4,

        /// <summary>
        /// Clear <see cref="CommandExecutor.Parameters" />.
        /// </summary>
        Parameters = 8,

        /// <summary>
        /// Reset all supported command state values.
        /// </summary>
        All = CommandText | CommandTimeout | CommandType | Parameters
    }
}
