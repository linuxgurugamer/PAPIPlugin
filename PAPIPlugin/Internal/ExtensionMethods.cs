#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace PAPIPlugin.Internal
{
    public static class ExtensionMethods
    {
        public static T ClampAndLog<T>(this T val, T lower, T upper) where T : IComparable<T>
        {
            if (lower.CompareTo(upper) > 0)
            {
                throw new ArgumentException("Lower bound is greater than upper bound!");
            }

            var outVal = val;
            if (val.CompareTo(lower) < 0)
            {
                outVal = lower;

                Util.LogWarning(string.Format("Clamped value \"{0}\" to \"{1}\" because it is out of bounds for the range of [{2}, {3}].", val, lower,
                                              lower, upper));
            }
            else if (val.CompareTo(upper) > 0)
            {
                outVal = upper;

                Util.LogWarning(string.Format("Clamped value \"{0}\" to \"{1}\" because it is out of bounds for the range of [{2}, {3}].", val, upper,
                                              lower, upper));
            }

            return outVal;
        }

        public static string SafeToString(this object obj)
        {
            return obj == null ? "null" : obj.ToString();
        }

        public static IList<T> Fill<T>(this IList<T> list, Func<T> creator)
        {
            if (list == null)
            {
                return null;
            }

            if (creator == null)
            {
                creator = () => default(T);
            }

            for (var i = 0; i < list.Count; i++)
            {
                list[i] = creator();
            }

            return list;
        }

        public static IList<T> Fill<T>(this IList<T> list, Func<int, T> creator)
        {
            if (list == null)
            {
                return null;
            }

            if (creator == null)
            {
                creator = i => default(T);
            }

            for (var i = 0; i < list.Count; i++)
            {
                list[i] = creator(i);
            }

            return list;
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null || action == null) return;

            foreach (var element in enumerable)
            {
                action(element);
            }
        }
    }
}
