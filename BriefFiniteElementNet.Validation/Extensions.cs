using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation
{
    public static class Extensions
    {

        public static int GetIndex(this StructurePart node)
        {
            return (int)node.GetMemberValue("Index");
        }

        public static int ToInt(this object obj)
        {
            return Convert.ToInt32(obj);
        }

        public static double ToDouble(this object obj)
        {
            return Convert.ToDouble(obj);
        }

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

        public static string ToScientificNotationString(this double dbl, int floatCount=4)
        {
            var t = dbl.ToString("e" + floatCount);

            if (dbl >= 0)
                t = "+" + t;

            return t;
        }


        /// <summary>
        /// Gets the public or private member using reflection.
        /// </summary>
        /// <param name="obj">The source target.</param>
        /// <param name="memberName">Name of the field or property.</param>
        /// <returns>the value of member</returns>
        public static object GetMemberValue(this object obj, string memberName)
        {
            var memInf = GetMemberInfo(obj, memberName);

            if (memInf == null)
                throw new System.Exception("memberName");

            if (memInf is System.Reflection.PropertyInfo)
                return memInf.As<System.Reflection.PropertyInfo>().GetValue(obj, null);

            if (memInf is System.Reflection.FieldInfo)
                return memInf.As<System.Reflection.FieldInfo>().GetValue(obj);

            throw new System.Exception();
        }

        /// <summary>
        /// Gets the public or private member using reflection.
        /// </summary>
        /// <param name="obj">The target object.</param>
        /// <param name="memberName">Name of the field or property.</param>
        /// <returns>Old Value</returns>
        public static object SetMemberValue(this object obj, string memberName, object newValue)
        {
            var memInf = GetMemberInfo(obj, memberName);


            if (memInf == null)
                throw new System.Exception("memberName");

            var oldValue = obj.GetMemberValue(memberName);

            if (memInf is System.Reflection.PropertyInfo)
                memInf.As<System.Reflection.PropertyInfo>().SetValue(obj, newValue, null);
            else if (memInf is System.Reflection.FieldInfo)
                memInf.As<System.Reflection.FieldInfo>().SetValue(obj, newValue);
            else
                throw new System.Exception();

            return oldValue;
        }

        /// <summary>
        /// Gets the member info
        /// </summary>
        /// <param name="obj">source object</param>
        /// <param name="memberName">name of member</param>
        /// <returns>instanse of MemberInfo corresponsing to member</returns>
        private static System.Reflection.MemberInfo GetMemberInfo(object obj, string memberName)
        {
            var prps = new System.Collections.Generic.List<System.Reflection.PropertyInfo>();

            prps.Add(obj.GetType().GetProperty(memberName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.FlattenHierarchy));
            prps = System.Linq.Enumerable.ToList(System.Linq.Enumerable.Where(prps, i => !ReferenceEquals(i, null)));
            if (prps.Count != 0)
                return prps[0];

            var flds = new System.Collections.Generic.List<System.Reflection.FieldInfo>();

            flds.Add(obj.GetType().GetField(memberName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.FlattenHierarchy));

            //to add more types of properties

            flds = System.Linq.Enumerable.ToList(System.Linq.Enumerable.Where(flds, i => !ReferenceEquals(i, null)));

            if (flds.Count != 0)
                return flds[0];

            return null;
        }

        [System.Diagnostics.DebuggerHidden]
        private static T As<T>(this object obj)
        {
            return (T) obj;
        }

    }
}
