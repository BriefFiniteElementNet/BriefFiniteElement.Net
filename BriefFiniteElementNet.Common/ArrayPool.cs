///Copyright http://geekswithblogs.net/
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    public static class MathUtil
    {
        /// <summary>
        /// Fills the array with specified value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="val"></param>
        public static void FillWith<T>(this T[] arr,T val)
        {
            if (arr != null)
                for (var i = 0; i < arr.Length; i++)
                    arr[i] = val;
        }

        public static int FirstIndexOf<T>(this T[] arr, T val) 
        {

            if (arr != null)
                for (var i = 0; i < arr.Length; i++)
                    if (SafeEquals(arr[i], val))
                        return i;

            return -1;
        }


        public static bool SafeEquals(object o1,object o2)
        {
            if (ReferenceEquals(o1, o2))
                return true;

            if (ReferenceEquals(o1, null) || ReferenceEquals(o2, null))
                return false;

            return o1.Equals(o2);
        }



        /// <summary>
        /// Multiplies the <see cref="matrix"/> by a constant value.
        /// </summary>
        /// <param name="matrix">The Matrix</param>
        /// <param name="constant">The constant value</param>
        public static void MultiplyByConstant(this Matrix matrix, double constant)
        {
            for (var i = 0; i < matrix.CoreArray.Length; i++)
            {
                matrix.CoreArray[i] *= constant;
            }
        }

        public static void MultiplyRowByConstant(this Matrix matrix, int row, double constant)
        {
            for (int j = 0; j < matrix.ColumnCount; j++)
                matrix[row, j] *= constant;
        }

        public static void MultiplyColumnByConstant(this Matrix matrix, int column, double constant)
        {
            for (int i = 0; i < matrix.RowCount; i++)
                matrix[i, column] *= constant;
        }
    }

    public class ArrayPool<T>
    {
        private readonly Dictionary<int, Stack<T[]>> _pool = new Dictionary<int, Stack<T[]>>();

        public readonly T[] Empty = new T[0];

        public virtual void Clear()
        {
            _pool.Clear();
        }

        internal virtual T[] Allocate(int size)
        {
            if (size < 0) throw new ArgumentOutOfRangeException("size", "Must be positive.");

            if (size == 0) return Empty;

            Stack<T[]> candidates;

            if (_pool.TryGetValue(size, out candidates))
                if (candidates.Count > 0)
                    return candidates.Pop();

            return new T[size];

            return _pool.TryGetValue(size, out candidates) && candidates.Count > 0 ? candidates.Pop() : new T[size];
        }

        internal virtual void Free(T[] array)
        {
            if (array == null) throw new ArgumentNullException("array");

            if (array.Length == 0) return;

            Stack<T[]> candidates;

            if (!_pool.TryGetValue(array.Length, out candidates))
                _pool.Add(array.Length, candidates = new Stack<T[]>());

            Array.Clear(array, 0, array.Length);

            if (candidates.Count < MaxQLength)
                //if (!candidates.Contains(array))
                {
                    candidates.Push(array);
                    //Console.WriteLine("Freeing");
                }
        }

        private readonly int MaxQLength = 20;
    }
}
