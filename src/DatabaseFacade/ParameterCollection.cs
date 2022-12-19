using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace CloudyWing.DatabaseFacade {
    /// <summary>The parameter collection.</summary>
    public sealed class ParameterCollection : KeyedCollection<string, ParameterMetadata> {
        /// <inheritdoc/>
        protected override string GetKeyForItem(ParameterMetadata item) {
            return item.ParameterName;
        }

        /// <summary>Adds the specified parameter name.</summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        public void Add(string parameterName, object value) {
            Add(new ParameterMetadata {
                ParameterName = parameterName,
                Value = value,
            });
        }

        /// <summary>Adds the specified parameter name.</summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <param name="dbType">Type of the database.</param>
        public void Add(string parameterName, object value, DbType dbType) {
            Add(new ParameterMetadata {
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
        public void Add(string parameterName, object value, DbType dbType, int size) {
            Add(new ParameterMetadata {
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
        public void Add(string parameterName, object value, DbType dbType, byte precision, byte scale) {
            Add(new ParameterMetadata {
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
        public void Add(string parameterName, object value, DbType dbType, ParameterDirection direction) {
            Add(new ParameterMetadata {
                ParameterName = parameterName,
                Value = value,
                Direction = direction,
                DbType = dbType
            });
        }

        /// <summary>Adds the specified parameter.</summary>
        /// <param name="parameter">The parameter.</param>
        public void Add(IDbDataParameter parameter) {
            Add(new ParameterMetadata {
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
        public void AddRange(params ParameterMetadata[] parameters) {
            AddRange(parameters as IEnumerable<ParameterMetadata>);
        }

        /// <summary>Adds the range.</summary>
        /// <param name="parameters">The parameters.</param>
        /// <exception cref="ArgumentNullException">parameters</exception>
        public void AddRange(IEnumerable<ParameterMetadata> parameters) {
            if (parameters == null) {
                throw new ArgumentNullException(nameof(parameters));
            }

            foreach (ParameterMetadata parameter in parameters) {
                Add(parameter);
            }
        }

        /// <summary>Adds the range.</summary>
        /// <param name="parameters">The parameters.</param>
        public void AddRange(params IDbDataParameter[] parameters) {
            AddRange(parameters as IEnumerable<IDbDataParameter>);
        }

        /// <summary>Adds the range.</summary>
        /// <param name="parameters">The parameters.</param>
        /// <exception cref="ArgumentNullException">parameters</exception>
        public void AddRange(IEnumerable<IDbDataParameter> parameters) {
            if (parameters == null) {
                throw new ArgumentNullException(nameof(parameters));
            }

            foreach (IDbDataParameter parameter in parameters) {
                Add(new ParameterMetadata(parameter));
            }
        }

        /// <summary>Adds the range.</summary>
        /// <param name="pairs">The pairs.</param>
        /// <exception cref="ArgumentNullException">pairs</exception>
        public void AddRange(IDictionary<string, object> pairs) {
            if (pairs == null) {
                throw new ArgumentNullException(nameof(pairs));
            }

            foreach (KeyValuePair<string, object> pair in pairs) {
                Add(pair.Key, pair.Value);
            }
        }

        /// <summary>Adds the range.</summary>
        /// <param name="obj">The object.</param>
        public void AddRange(object obj) {
            Type type = obj.GetType();

            if (obj is IEnumerable<ParameterMetadata> metadatas) {
                AddRange(metadatas);
            } else if (obj is IEnumerable<IDbDataParameter> parameters) {
                AddRange(parameters);
            } else if (obj is IDictionary<string, object> dictionary) {
                AddRange(dictionary);
            } else {
                AddRangeFromObject(obj);
            }
        }

        private void AddRangeFromObject(object obj) {
            IEnumerable<PropertyInfo> props = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.CanRead);

            foreach (PropertyInfo prop in props) {
                object val = prop.GetValue(obj, null);
                Add(prop.Name, val);
            }
        }
    }
}
