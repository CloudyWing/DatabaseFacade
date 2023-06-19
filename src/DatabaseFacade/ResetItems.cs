using System;

namespace CloudyWing.DatabaseFacade {
    /// <summary>The reset items.</summary>
    [Flags]
    public enum ResetItems {
        /// <summary>The none.</summary>
        None = 0,

        /// <summary>The command text.</summary>
        CommandText = 1,

        /// <summary>The command timeout.</summary>
        CommandTimeout = 2,

        /// <summary>The command type.</summary>
        CommandType = 4,

        /// <summary>The parameters.</summary>
        Parameters = 8,

        /// <summary>All.</summary>
        All = CommandText | CommandTimeout | CommandType | Parameters
    }
}
