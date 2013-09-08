#region Usings

using System;
using System.ComponentModel;
using UnityEngine;

#endregion

namespace PAPIPlugin.Internal
{
    public static class ConfigNodeExtensions
    {
        #region Delegates

        public delegate bool ConverterDelegate<T>(ConfigNode node, string key, out T value);

        #endregion

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
                    value = (T) typeConverter.ConvertFromInvariantString(stringValue);

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
            if (typeof(T) == typeof(Vector3))
            {
                return (ConfigNode node, string key, out T value) =>
                    {
                        value = default(T);

                        if (!node.HasValue(key))
                        {
                            return false;
                        }

                        object obj = ConfigNode.ParseVector3(node.GetValue(key));

                        // Little hack to make casting work but this is a safe cast
                        value = (T) obj;

                        return true;
                    };
            }

            if (typeof(T) == typeof(Vector3d))
            {
                return (ConfigNode node, string key, out T value) =>
                    {
                        value = default(T);

                        if (!node.HasValue(key))
                        {
                            return false;
                        }

                        object obj = ConfigNode.ParseVector3D(node.GetValue(key));

                        // Little hack to make casting work but this is a safe cast
                        value = (T) obj;

                        return true;
                    };
            }

            if (typeof(T) == typeof(Color))
            {
                return (ConfigNode node, string key, out T value) =>
                {
                    value = default(T);

                    if (!node.HasValue(key))
                    {
                        return false;
                    }

                    object obj = ConfigNode.ParseColor(node.GetValue(key));

                    // Little hack to make casting work but this is a safe cast
                    value = (T)obj;

                    return true;
                };
            }

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
