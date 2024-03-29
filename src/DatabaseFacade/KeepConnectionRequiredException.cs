﻿using System;
using System.Runtime.Serialization;

namespace CloudyWing.DatabaseFacade {
    /// <summary>The keep connection required exception.</summary>
    /// <seealso cref="Exception" />
    [Serializable]
    public class KeepConnectionRequiredException : Exception {

        /// <summary>Initializes a new instance of the <see cref="KeepConnectionRequiredException" /> class.</summary>
        public KeepConnectionRequiredException() { }

        /// <summary>Initializes a new instance of the <see cref="KeepConnectionRequiredException" /> class.</summary>
        /// <param name="message">The message that describes the error.</param>
        public KeepConnectionRequiredException(string message) : base(message) { }

        /// <summary>Initializes a new instance of the <see cref="KeepConnectionRequiredException" /> class.</summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public KeepConnectionRequiredException(string message, KeepConnectionRequiredException inner) : base(message, inner) { }

        /// <summary>Initializes a new instance of the <see cref="KeepConnectionRequiredException" /> class.</summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo">SerializationInfo</see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext">StreamingContext</see> that contains contextual information about the source or destination.</param>
        protected KeepConnectionRequiredException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
