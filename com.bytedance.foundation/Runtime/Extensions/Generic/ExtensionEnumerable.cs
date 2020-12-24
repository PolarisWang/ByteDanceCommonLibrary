using System;
using System.Collections.Generic;
using System.Text;

namespace ByteDance.Foundation
{
    public static class ExtensionEnumerable
    {
        public static string GetPrettyString<T>(this IEnumerable<T> l)
        {
            return l.GetPrettyString(i => i.ToString());
        }

        public static string GetPrettyString<T>(this IEnumerable<T> l, Func<T, string> func)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            foreach (var i in l)
            {
                sb.Append(func(i));
                sb.Append(',');
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(']');
            return sb.ToString();
        }
    }
}