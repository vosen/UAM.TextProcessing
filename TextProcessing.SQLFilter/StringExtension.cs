using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vosen.SQLFilter
{
    internal static class StringExtension
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            if (source == null)
                return false;
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static bool Equals(this string source, string toCheck, StringComparison comp)
        {
            if (source == null)
                return false;
            return source.Equals(toCheck, comp);
        }

        public static bool StartsWith(this string source, string toCheck, StringComparison comp)
        {
            if (source == null)
                return false;
            return source.StartsWith(toCheck, comp);
        }
        public static bool EndsWith(this string source, string toCheck, StringComparison comp)
        {
            if (source == null)
                return false;
            return source.EndsWith(toCheck, comp);
        }

    }
}
