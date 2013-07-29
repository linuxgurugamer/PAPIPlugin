#region Usings

using System;
using System.ComponentModel;

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
    }
}
