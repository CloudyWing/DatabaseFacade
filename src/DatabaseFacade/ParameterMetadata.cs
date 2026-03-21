using System;
using System.Data;

namespace CloudyWing.DatabaseFacade {
    /// <summary>
    /// Describes the values used to create and configure a provider-specific database parameter.
    /// </summary>
    /// <seealso cref="ICloneable" />
    public sealed class ParameterMetadata : ICloneable {
        private string? parameterName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMetadata" /> class.
        /// </summary>
        public ParameterMetadata() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMetadata" /> class by copying another metadata instance.
        /// </summary>
        /// <param name="from">The metadata instance to copy.</param>
        internal ParameterMetadata(ParameterMetadata from) {
            ParameterName = from.ParameterName;
            Direction = from.Direction;
            DbType = from.DbType;
            Size = from.Size;
            Precision = from.Precision;
            Scale = from.Scale;
            SourceColumn = from.SourceColumn;
            SourceVersion = from.SourceVersion;
            Value = from.Value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMetadata" /> class by copying a provider parameter.
        /// </summary>
        /// <param name="from">The parameter to copy.</param>
        internal ParameterMetadata(IDbDataParameter from) {
            ParameterName = from.ParameterName;
            Direction = from.Direction;
            DbType = from.DbType;
            Size = from.Size;
            Precision = from.Precision;
            Scale = from.Scale;
            SourceColumn = from.SourceColumn;
            SourceVersion = from.SourceVersion;
            Value = from.Value;
        }

        /// <summary>
        /// Gets or sets the logical parameter name without the provider prefix.
        /// Leading <c>@</c>, <c>:</c>, or <c>?</c> characters are trimmed automatically.
        /// </summary>
        public string? ParameterName {
            get => parameterName;
            set {
                string? name = value?.Trim();
                if (name is { Length: > 0 }) {
                    switch (name[0]) {
                        case '@':
                        case ':':
                        case '?':
                            parameterName = name.Substring(1);
                            return;
                    }
                }

                parameterName = name;
            }
        }

        /// <summary>
        /// Gets or sets the value assigned to the provider parameter.
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// Gets or sets the parameter direction.
        /// </summary>
        public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

        /// <summary>
        /// Gets or sets the database type to apply when one is explicitly required.
        /// </summary>
        public DbType? DbType { get; set; }

        /// <summary>
        /// Gets or sets the parameter size.
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// Gets or sets the numeric precision.
        /// </summary>
        public byte? Precision { get; set; }

        /// <summary>
        /// Gets or sets the numeric scale.
        /// </summary>
        public byte? Scale { get; set; }

        /// <summary>
        /// Gets or sets the source column name used by data adapters.
        /// </summary>
        public string? SourceColumn { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataRowVersion" /> used by data adapters.
        /// </summary>
        public DataRowVersion? SourceVersion { get; set; }

        /// <summary>
        /// Creates a copy of the current parameter metadata.
        /// </summary>
        /// <returns>A new <see cref="ParameterMetadata" /> instance that contains the same values.</returns>
        public object Clone() {
            return new ParameterMetadata(this);
        }

        /// <summary>
        /// Copies the configured metadata to the specified provider parameter.
        /// Only explicitly assigned values are applied.
        /// </summary>
        /// <param name="to">The provider parameter to configure.</param>
        public void ApplyParameter(IDbDataParameter to) {
            to.ParameterName = ParameterName
                ?? throw new InvalidOperationException("ParameterName cannot be null.");
            to.Direction = Direction;

            if (DbType.HasValue) {
                to.DbType = DbType.Value;
            }

            if (Size.HasValue) {
                to.Size = Size.Value;
            }

            if (Precision.HasValue) {
                to.Precision = Precision.Value;
            }

            if (Scale.HasValue) {
                to.Scale = Scale.Value;
            }

            if (SourceColumn != null) {
                to.SourceColumn = SourceColumn;
            }

            if (SourceVersion.HasValue) {
                to.SourceVersion = SourceVersion.Value;
            }

            to.Value = Value;
        }
    }
}
