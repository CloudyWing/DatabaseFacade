using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace CloudyWing.DatabaseFacade {
    /// <summary>The parameter collection.</summary>
    public sealed class ParameterCollection : KeyedCollection<string, ParameterMetadata> {
        private readonly CommandExecutor commandExecutor;

        internal ParameterCollection(CommandExecutor executor) {
            commandExecutor = executor ?? throw new ArgumentNullException(nameof(commandExecutor));
        }

        /// <inheritdoc/>
        protected override string GetKeyForItem(ParameterMetadata item) {
            return item.ParameterName;
        }

        /// <summary>Adds the specified metadata.</summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The self.</returns>
        public new ParameterCollection Add(ParameterMetadata metadata) {
            base.Add(metadata);
            return this;
        }

        /// <summary>Adds the specified parameter name.</summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns>The self.</returns>
        public ParameterCollection Add(string parameterName, object value) {
            return Add(new ParameterMetadata {
                ParameterName = parameterName,
                Value = value,
            });
        }

        /// <summary>Adds the specified parameter name.</summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <param name="dbType">Type of the database.</param>
        public ParameterCollection Add(string parameterName, object value, DbType dbType) {
            return Add(new ParameterMetadata {
                ParameterName = parameterName,
                Value = value,
                DbType = dbType
            });
        }

        /// <summary>Adds the specified parameter name.</summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <param name="dbType">Type of the database.</param>
        /// <param name="size">The size.</param>
        /// <returns>The self.</returns>
        public ParameterCollection Add(string parameterName, object value, DbType dbType, int size) {
            return Add(new ParameterMetadata {
                ParameterName = parameterName,
                Value = value,
                DbType = dbType,
                Size = size
            });
        }

        /// <summary>Adds the specified parameter name.</summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <param name="dbType">Type of the database.</param>
        /// <param name="precision">The precision.</param>
        /// <param name="scale">The scale.</param>
        /// <returns>The self.</returns>
        public ParameterCollection Add(string parameterName, object value, DbType dbType, byte precision, byte scale) {
            return Add(new ParameterMetadata {
                ParameterName = parameterName,
                Value = value,
                DbType = dbType,
                Precision = precision,
                Scale = scale,
            });
        }

        /// <summary>Adds the specified parameter name.</summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <param name="dbType">Type of the database.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>The self.</returns>
        public ParameterCollection Add(string parameterName, object value, DbType dbType, ParameterDirection direction) {
            return Add(new ParameterMetadata {
                ParameterName = parameterName,
                Value = value,
                Direction = direction,
                DbType = dbType
            });
        }

        /// <summary>Adds the specified parameter.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The self.</returns>
        public ParameterCollection Add(IDbDataParameter parameter) {
            return Add(new ParameterMetadata {
                ParameterName = parameter.ParameterName,
                Value = parameter.Value,
                DbType = parameter.DbType,
                Size = parameter.Size,
                Precision = parameter.Precision,
                Scale = parameter.Scale,
                Direction = parameter.Direction,
                SourceColumn = parameter.SourceColumn,
                SourceVersion = parameter.SourceVersion
            });
        }

        /// <summary>Adds the range.</summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The self.</returns>
        public ParameterCollection AddRange(params ParameterMetadata[] parameters) {
            return AddRange(parameters as IEnumerable<ParameterMetadata>);
        }

        /// <summary>Adds the range.</summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The self.</returns>
        /// <exception cref="ArgumentNullException">parameters</exception>
        public ParameterCollection AddRange(IEnumerable<ParameterMetadata> parameters) {
            if (parameters == null) {
                throw new ArgumentNullException(nameof(parameters));
            }

            foreach (ParameterMetadata parameter in parameters) {
                Add(parameter);
            }

            return this;
        }

        /// <summary>Adds the range.</summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The self.</returns>
        public ParameterCollection AddRange(params IDbDataParameter[] parameters) {
            return AddRange(parameters as IEnumerable<IDbDataParameter>);
        }

        /// <summary>Adds the range.</summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The self.</returns>
        /// <exception cref="ArgumentNullException">parameters</exception>
        public ParameterCollection AddRange(IEnumerable<IDbDataParameter> parameters) {
            if (parameters is null) {
                throw new ArgumentNullException(nameof(parameters));
            }

            foreach (IDbDataParameter parameter in parameters) {
                Add(new ParameterMetadata(parameter));
            }

            return this;
        }

        /// <summary>Adds the range.</summary>
        /// <param name="pairs">The pairs.</param>
        /// <returns>The self.</returns>
        /// <exception cref="ArgumentNullException">pairs</exception>
        public ParameterCollection AddRange(IDictionary<string, object> pairs) {
            if (pairs is null) {
                throw new ArgumentNullException(nameof(pairs));
            }

            foreach (KeyValuePair<string, object> pair in pairs) {
                Add(pair.Key, pair.Value);
            }

            return this;
        }

        /// <summary>Adds the range.</summary>
        /// <param name="obj">The object.</param>
        /// <returns>The self.</returns>
        public ParameterCollection AddRange(object obj) {
            if (obj is IEnumerable<ParameterMetadata> metadatas) {
                return AddRange(metadatas);
            } else if (obj is IEnumerable<IDbDataParameter> parameters) {
                return AddRange(parameters);
            } else if (obj is IDictionary<string, object> dictionary) {
                return AddRange(dictionary);
            } else {
                return AddRangeFromObject(obj);
            }
        }

        private ParameterCollection AddRangeFromObject(object obj) {
            IEnumerable<PropertyInfo> props = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.CanRead);

            foreach (PropertyInfo prop in props) {
                object val = prop.GetValue(obj, null);
                Add(prop.Name, val);
            }

            return this;
        }

        /// <summary>Gets the command executor.</summary>
        /// <returns>The command executor.</returns>
        public CommandExecutor GetCommandExecutor() {
            return commandExecutor;
        }
    }
}
