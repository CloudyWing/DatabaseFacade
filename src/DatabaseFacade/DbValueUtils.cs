using System;
using System.Globalization;

namespace CloudyWing.DatabaseFacade {
    /// <summary>
    /// Converts values returned by ADO.NET providers to CLR types.
    /// </summary>
    internal static class DbValueUtils {
        /// <summary>
        /// Converts a provider value to the specified type.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The provider value.</param>
        /// <returns>
        /// The converted value, or the default value of <typeparamref name="T" /> when <paramref name="value" />
        /// is <see langword="null" /> or <see cref="DBNull.Value" />.
        /// </returns>
        /// <exception cref="InvalidCastException">The value cannot be converted to <typeparamref name="T" />.</exception>
        public static T? ConvertTo<T>(object? value) {
            if (value is null || value is DBNull) {
                return default;
            }

            if (value is T typedValue) {
                return typedValue;
            }

            Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            try {
                return (T)ConvertTo(value, targetType);
            } catch (Exception ex) when (ex is FormatException or OverflowException or ArgumentException) {
                throw new InvalidCastException($"Cannot convert a value of type '{value.GetType()}' to '{typeof(T)}'.", ex);
            }
        }

        private static object ConvertTo(object value, Type targetType) {
            if (targetType.IsInstanceOfType(value)) {
                return value;
            }

            if (targetType.IsEnum) {
                return value is string text
                    ? Enum.Parse(targetType, text, true)
                    : Enum.ToObject(targetType, value);
            }

            if (targetType == typeof(Guid)) {
                return value switch {
                    string text => Guid.Parse(text),
                    byte[] bytes => new Guid(bytes),
                    _ => throw new InvalidCastException($"Cannot convert a value of type '{value.GetType()}' to '{targetType}'.")
                };
            }

            return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }
    }
}
