using System;
using System.Runtime.Serialization;

namespace CloudyWing.DatabaseFacade {
    /// <summary>
    /// The exception that is thrown when an operation requires <see cref="CommandExecutor.KeepConnection" /> to be enabled.
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
    public class KeepConnectionRequiredException : Exception {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeepConnectionRequiredException" /> class.
        /// </summary>
        public KeepConnectionRequiredException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeepConnectionRequiredException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public KeepConnectionRequiredException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeepConnectionRequiredException" /> class with a specified error message and inner exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that caused the current exception.</param>
        public KeepConnectionRequiredException(string message, KeepConnectionRequiredException inner)
            : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeepConnectionRequiredException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized exception data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
#pragma warning disable SYSLIB0051
        protected KeepConnectionRequiredException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#pragma warning restore SYSLIB0051
    }
}
