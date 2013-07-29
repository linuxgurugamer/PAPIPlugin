#region Usings

using System;
using System.ComponentModel;

#endregion

namespace PAPIPlugin.Internal
{
    public static class ConfigNodeExtensions
    {
        public static T ConvertValue<T>(this ConfigNode node, string key, T def = default(T))
        {
            if (!node.HasValue(key))
            {
                return def;
            }

            var stringValue = node.GetValue(key);

            var typeConverter = TypeDescriptor.GetConverter(typeof(T));

            try
            {
                return (T) typeConverter.ConvertFromInvariantString(stringValue);
            }
            catch (NotSupportedException)
            {
                Util.LogWarning(string.Format("Cannot convert value \"{0}\" to type {1}", stringValue, typeof(T).FullName));

                return def;
            }
        }

        public static T ConvertValueWithException<T>(this ConfigNode node, string key)
        {
            if (!node.HasValue(key))
            {
                throw new FormatException(string.Format("The key \"{0}\" could not be found.", key));
            }

            var stringValue = node.GetValue(key);

            var typeConverter = TypeDescriptor.GetConverter(typeof(T));

            try
            {
                return (T) typeConverter.ConvertFromInvariantString(stringValue);
            }
            catch (NotSupportedException)
            {
                Util.LogWarning(string.Format("Cannot convert value \"{0}\" to type {1}", stringValue, typeof(T).FullName));

                throw new FormatException(string.Format("Failed to convert the value \"{0}\" for key \"{1}\" to type \"{2}\"", stringValue, key,
                    typeof(T).FullName));
            }
        }
    }
}
