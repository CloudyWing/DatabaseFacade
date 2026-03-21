using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace CloudyWing.DatabaseFacade {
    /// <summary>
    /// Represents the parameters queued for the next <see cref="CommandExecutor" /> operation.
    /// </summary>
    public sealed class ParameterCollection : KeyedCollection<string, ParameterMetadata> {
        private readonly CommandExecutor commandExecutor;

        internal ParameterCollection(CommandExecutor executor) {
            commandExecutor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        /// <inheritdoc />
        protected override string GetKeyForItem(ParameterMetadata item) {
            return item.ParameterName ?? throw new InvalidOperationException("ParameterName cannot be null.");
        }

        /// <summary>
        /// Adds the specified parameter metadata.
        /// </summary>
        /// <param name="metadata">The metadata to add.</param>
        /// <returns>The current <see cref="ParameterCollection" /> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="metadata" /> is <see langword="null" />.</exception>
        public new ParameterCollection Add(ParameterMetadata? metadata) {
            if (metadata is null) {
                throw new ArgumentNullException(nameof(metadata));
            }

            base.Add(metadata);
            return this;
        }

        /// <summary>
        /// Adds a parameter by name and value.
        /// </summary>
        /// <param name="parameterName">The logical parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>The current <see cref="ParameterCollection" /> instance.</returns>
        public ParameterCollection Add(string parameterName, object? value) {
            return Add(new ParameterMetadata {
                ParameterName = parameterName,
                Value = value,
            });
        }

        /// <summary>
        /// Adds a parameter by name, value, and explicit database type.
        /// </summary>
        /// <param name="parameterName">The logical parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <param name="dbType">The database type.</param>
        /// <returns>The current <see cref="ParameterCollection" /> instance.</returns>
        public ParameterCollection Add(string parameterName, object? value, DbType dbType) {
            return Add(new ParameterMetadata {
                ParameterName = parameterName,
                Value = value,
                DbType = dbType
            });
        }

        /// <summary>
        /// Adds a parameter by name, value, database type, and size.
        /// </summary>
        /// <param name="parameterName">The logical parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <param name="dbType">The database type.</param>
        /// <param name="size">The parameter size.</param>
        /// <returns>The current <see cref="ParameterCollection" /> instance.</returns>
        public ParameterCollection Add(string parameterName, object? value, DbType dbType, int size) {
            return Add(new ParameterMetadata {
                ParameterName = parameterName,
                Value = value,
                DbType = dbType,
                Size = size
            });
        }

        /// <summary>
        /// Adds a parameter by name, value, database type, precision, and scale.
        /// </summary>
        /// <param name="parameterName">The logical parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <param name="dbType">The database type.</param>
        /// <param name="precision">The numeric precision.</param>
        /// <param name="scale">The numeric scale.</param>
        /// <returns>The current <see cref="ParameterCollection" /> instance.</returns>
        public ParameterCollection Add(string parameterName, object? value, DbType dbType, byte precision, byte scale) {
            return Add(new ParameterMetadata {
                ParameterName = parameterName,
                Value = value,
                DbType = dbType,
                Precision = precision,
                Scale = scale,
            });
        }

        /// <summary>
        /// Adds a parameter by name, value, database type, and direction.
        /// </summary>
        /// <param name="parameterName">The logical parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <param name="dbType">The database type.</param>
        /// <param name="direction">The parameter direction.</param>
        /// <returns>The current <see cref="ParameterCollection" /> instance.</returns>
        public ParameterCollection Add(string parameterName, object? value, DbType dbType, ParameterDirection direction) {
            return Add(new ParameterMetadata {
                ParameterName = parameterName,
                Value = value,
                Direction = direction,
                DbType = dbType
            });
        }

        /// <summary>
        /// Adds a provider-specific parameter by copying its metadata.
        /// </summary>
        /// <param name="parameter">The provider parameter to copy.</param>
        /// <returns>The current <see cref="ParameterCollection" /> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameter" /> is <see langword="null" />.</exception>
        public ParameterCollection Add(IDbDataParameter? parameter) {
            if (parameter is null) {
                throw new ArgumentNullException(nameof(parameter));
            }

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

        /// <summary>
        /// Adds a range of <see cref="ParameterMetadata" /> objects.
        /// </summary>
        /// <param name="parameters">The parameters to add.</param>
        /// <returns>The current <see cref="ParameterCollection" /> instance.</returns>
        public ParameterCollection AddRange(params ParameterMetadata[] parameters) {
            return AddRange(parameters as IEnumerable<ParameterMetadata>);
        }

        /// <summary>
        /// Adds a range of <see cref="ParameterMetadata" /> objects.
        /// </summary>
        /// <param name="parameters">The parameters to add.</param>
        /// <returns>The current <see cref="ParameterCollection" /> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameters" /> is <see langword="null" />.</exception>
        public ParameterCollection AddRange(IEnumerable<ParameterMetadata> parameters) {
            if (parameters is null) {
                throw new ArgumentNullException(nameof(parameters));
            }

            foreach (ParameterMetadata parameter in parameters) {
                Add(parameter);
            }

            return this;
        }

        /// <summary>
        /// Adds a range of provider-specific parameters by copying their metadata.
        /// </summary>
        /// <param name="parameters">The parameters to add.</param>
        /// <returns>The current <see cref="ParameterCollection" /> instance.</returns>
        public ParameterCollection AddRange(params IDbDataParameter[] parameters) {
            return AddRange(parameters as IEnumerable<IDbDataParameter>);
        }

        /// <summary>
        /// Adds a range of provider-specific parameters by copying their metadata.
        /// </summary>
        /// <param name="parameters">The parameters to add.</param>
        /// <returns>The current <see cref="ParameterCollection" /> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameters" /> is <see langword="null" />.</exception>
        public ParameterCollection AddRange(IEnumerable<IDbDataParameter> parameters) {
            if (parameters is null) {
                throw new ArgumentNullException(nameof(parameters));
            }

            foreach (IDbDataParameter parameter in parameters) {
                Add(new ParameterMetadata(parameter));
            }

            return this;
        }

        /// <summary>
        /// Adds parameters from a dictionary of name/value pairs.
        /// </summary>
        /// <param name="pairs">The name/value pairs to add.</param>
        /// <returns>The current <see cref="ParameterCollection" /> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pairs" /> is <see langword="null" />.</exception>
        public ParameterCollection AddRange(IDictionary<string, object> pairs) {
            if (pairs is null) {
                throw new ArgumentNullException(nameof(pairs));
            }

            foreach (KeyValuePair<string, object> pair in pairs) {
                Add(pair.Key, pair.Value);
            }

            return this;
        }

        /// <summary>
        /// Adds parameters from supported container types such as dictionaries, parameter collections, or anonymous objects.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <returns>The current <see cref="ParameterCollection" /> instance.</returns>
        public ParameterCollection AddRange(object obj) {
            if (obj is null) {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj is IEnumerable<ParameterMetadata> metadatas) {
                return AddRange(metadatas);
            } else if (obj is IEnumerable<IDbDataParameter> parameters) {
                return AddRange(parameters);
            } else if (obj is IDictionary<string, object> dictionary) {
                return AddRange(dictionary);
            } else if (obj is IEnumerable<KeyValuePair<string, object?>> nullablePairs) {
                return AddNullablePairs(nullablePairs);
            } else {
                return AddRangeFromObject(obj);
            }
        }

        /// <summary>
        /// Returns the owning <see cref="CommandExecutor" /> instance so that fluent chaining can continue.
        /// </summary>
        /// <returns>The owning <see cref="CommandExecutor" />.</returns>
        public CommandExecutor GetCommandExecutor() {
            return commandExecutor;
        }

        private ParameterCollection AddNullablePairs(IEnumerable<KeyValuePair<string, object?>> pairs) {
            foreach (KeyValuePair<string, object?> pair in pairs) {
                Add(pair.Key, pair.Value);
            }

            return this;
        }

        private ParameterCollection AddRangeFromObject(object obj) {
            IEnumerable<PropertyInfo> props = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.CanRead);

            foreach (PropertyInfo prop in props) {
                object? val = prop.GetValue(obj, null);
                Add(prop.Name, val);
            }

            return this;
        }
    }
}
