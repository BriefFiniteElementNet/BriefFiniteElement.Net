using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    public class ArrayEqualityComparer : IEqualityComparer<Array>
    {
        public bool Equals(Array x, Array y)
        {
            if (x.Length != y.Length)
            {
                return false;
            }

            for (int i = 0; i < x.Length; i++)
            {
                if (!AreEqualObjects(x.GetValue(i), y.GetValue(i)))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool AreEqualObjects(object obj1, object obj2)
        {
            var obj1Null = ReferenceEquals(obj1, null);
            var obj2Null = ReferenceEquals(obj2, null);

            if (obj1Null && obj2Null)
                return true;

            if (obj1Null || obj2Null)
                return false;

            if (obj1.GetType() != obj2.GetType())
                return false;

            if (ReferenceEquals(obj1, obj2))
                return true;

            return obj1.Equals(obj2);
        }

        public int GetHashCode(Array obj)
        {
            int result = 17;
            for (int i = 0; i < obj.Length; i++)
            {
                unchecked
                {
                    result = result * 23 + obj.GetValue(i).GetHashCode();
                }
            }
            return result;
        }
    }
}
