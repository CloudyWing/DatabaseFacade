using System;

namespace CloudyWing.DatabaseFacade {
    [Flags]
    public enum ResetItems {
        None = 0,
        CommandText = 1,
        CommandTimeout = 2,
        CommandType = 4,
        Parameters = 8,
        TextAndParameters = CommandText | Parameters,
        All = CommandText | CommandTimeout | CommandType | Parameters
    }
}
