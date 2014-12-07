using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation
{
    public static class Extensions
    {
        public static object[] Union<T>(this object num, params T[] others)
        {
            var buf = new object[others.Length + 1];
            buf[0] = num;

            for (int i = 0; i < others.Length; i++)
            {
                buf[i + 1] = others[i];
            }

            return buf;
        }

        public static string Join<T>(T[] array)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < array.Length; i++)
            {
                sb.Append(array[i]);
                if (i != array.Length - 1)
                    sb.Append("\t");
            }

            return sb.ToString();
        }

        public static void AppendLineArray(this StringBuilder sb, params object[] array)
        {
            sb.AppendLine(Join(array));
        }

        public static void AppendLine(this StringBuilder sb, string format, params object[] args)
        {
            sb.AppendLine(string.Format(format, args));
        }
    }
}
