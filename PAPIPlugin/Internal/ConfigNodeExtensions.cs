#region Usings

using System;
using System.ComponentModel;

#endregion

namespace PAPIPlugin.Internal
{
    public static class ConfigNodeExtensions
    {
        public static bool TryConvertValue<T>(this ConfigNode node, string key, out T value)
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
                value = (T) typeConverter.ConvertFromInvariantString(stringValue);

                return true;
            }
            catch (NotSupportedException)
            {
                Util.LogWarning(string.Format("Cannot convert value \"{0}\" to type {1}", stringValue, typeof(T).FullName));

                return false;
            }
        }

        public static T ConvertValue<T>(this ConfigNode node, string key, T def = default(T))
        {
            T value;

            return TryConvertValue(node, key, out value) ? value : def;
        }

        public static T ConvertValueWithException<T>(this ConfigNode node, string key)
        {
            T value;

            if (TryConvertValue(node, key, out value))
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
