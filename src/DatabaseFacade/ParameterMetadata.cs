using System;
using System.Data;

namespace CloudyWing.DatabaseFacade {
    /// <summary>The parameter metadata.</summary>
    /// <seealso cref="ICloneable" />
    public sealed class ParameterMetadata : ICloneable {
        private string parameterName;

        /// <summary>Initializes a new instance of the <see cref="ParameterMetadata" /> class.</summary>
        public ParameterMetadata() { }

        /// <summary>Initializes a new instance of the <see cref="ParameterMetadata" /> class.</summary>
        /// <param name="from">From.</param>
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

        /// <summary>Initializes a new instance of the <see cref="ParameterMetadata" /> class.</summary>
        /// <param name="from">From.</param>
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

        /// <summary>Gets or sets the name of the parameter.</summary>
        /// <value>The name of the parameter.</value>
        public string ParameterName {
            get => parameterName;
            set {
                string name = value?.Trim();
                if (!string.IsNullOrEmpty(name)) {
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

        /// <summary>Gets or sets the value.</summary>
        /// <value>The value.</value>
        public object Value { get; set; }

        /// <summary>Gets or sets the direction.</summary>
        /// <value>The direction.</value>
        public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

        /// <summary>Gets or sets the type of the database.</summary>
        /// <value>The type of the database.</value>
        public DbType? DbType { get; set; }

        /// <summary>Gets or sets the size.</summary>
        /// <value>The size.</value>
        public int? Size { get; set; }

        /// <summary>Gets or sets the precision.</summary>
        /// <value>The precision.</value>
        public byte? Precision { get; set; }

        /// <summary>Gets or sets the scale.</summary>
        /// <value>The scale.</value>
        public byte? Scale { get; set; }

        /// <summary>Gets or sets the source column.</summary>
        /// <value>The source column.</value>
        public string SourceColumn { get; set; }

        /// <summary>Gets or sets the source version.</summary>
        /// <value>The source version.</value>
        public DataRowVersion? SourceVersion { get; set; }

        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone() {
            return new ParameterMetadata(this);
        }

        /// <summary>Applies the parameter.</summary>
        /// <param name="to">To.</param>
        public void ApplyParameter(IDbDataParameter to) {
            to.ParameterName = ParameterName;
            to.Direction = Direction;

            // 雖然 DbDataParameter 的 Property 都有預設值
            // 但有可能會因為設定某些 Property，而影響其他值
            // 例如在完全沒設定過 DbType 的情況下，DbType 會依 Value Type 自動調整
            // 所以 Metadata 只將有值的項目設定到 DbDataParameter，至於預設值為何給 DbDataParameter 自己決定
            if (DbType.HasValue) {
                to.DbType = DbType.Value;
            }

            if (Size.HasValue) {
                to.Size = Size.Value;
            }

            if (Precision.HasValue) {
                to.Precision = Precision.Value;
            }

            if (Precision.HasValue) {
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
