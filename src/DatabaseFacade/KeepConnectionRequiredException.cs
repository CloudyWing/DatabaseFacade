using System;
using System.Runtime.Serialization;

namespace CloudyWing.DatabaseFacade {
    [Serializable]
    public class KeepConnectionRequiredException : Exception {
        public KeepConnectionRequiredException() { }

        public KeepConnectionRequiredException(string message) : base(message) { }

        public KeepConnectionRequiredException(string message, KeepConnectionRequiredException inner) : base(message, inner) { }

        protected KeepConnectionRequiredException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
