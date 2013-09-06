#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;

#endregion

namespace PAPIPlugin.Internal
{
    public static class ConfigNodeExtensions
    {
        public delegate bool ConverterDelegate<T>(ConfigNode node, string key, out T value);

        public static bool TryConvertValue<T>(this ConfigNode node, string key, out T value, ConverterDelegate<T> converter)
        {
            if (converter != null || (converter = GetDefaultConverter<T>()) != null)
            {
                return converter(node, key, out value);
            }
            else
            {
                value = default(T);

                if (!node.HasValue(key))
                {
                    return false;
                }

                var stringValue = node.GetValue(key);

                var typeConverter = TypeDescriptor.GetConverter(typeof(T));

                try
                {
                    value = (T)typeConverter.ConvertFromInvariantString(stringValue);

                    return true;
                }
                catch (NotSupportedException)
                {
                    Util.LogWarning(string.Format("Cannot convert value \"{0}\" to type {1}", stringValue, typeof(T).FullName));

                    return false;
                }
            }
        }

        private static ConverterDelegate<T> GetDefaultConverter<T>()
        {
            return null;
        }

        public static T ConvertValue<T>(this ConfigNode node, string key, T def = default(T), ConverterDelegate<T> converter = null)
        {
            T value;

            return TryConvertValue(node, key, out value, converter) ? value : def;
        }

        public static T ConvertValueWithException<T>(this ConfigNode node, string key, ConverterDelegate<T> converter = null)
        {
            T value;

            if (TryConvertValue(node, key, out value, converter))
            {
                return value;
            }
            else
            {
                throw new FormatException(string.Format("Failed to convert the value for key \"{0}\" to type \"{1}\"", key, typeof(T).FullName));
            }
        }
    }
}
