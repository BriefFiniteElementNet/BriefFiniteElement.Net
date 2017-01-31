///Copyright http://geekswithblogs.net/
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
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

            return _pool.TryGetValue(size, out candidates) && candidates.Count > 0 ? candidates.Pop() : new T[size];
        }

        internal virtual void Free(T[] array)
        {
            if (array == null) throw new ArgumentNullException("array");

            if (array.Length == 0) return;

            Stack<T[]> candidates;

            if (!_pool.TryGetValue(array.Length, out candidates))
                _pool.Add(array.Length, candidates = new Stack<T[]>());

            candidates.Push(array);
        }
    }

    public class ConcurrentArrayPool<T> : ArrayPool<T>
    {
        internal override T[] Allocate(int size)
        {
            lock (this)
            {
                return base.Allocate(size);
            }
        }

        internal override void Free(T[] array)
        {
            lock (this)
            {
                base.Free(array);
            }
        }

        public override void Clear()
        {
            lock (this)
            {
                base.Clear();
            }
        }
    }

    public static class MatrixPool
    {
        private static ConcurrentArrayPool<double> Pool = new ConcurrentArrayPool<double>();

        public static Matrix Allocate(int rows, int columns)
        {
            var arr = Pool.Allocate(rows * columns);

            for (var i = 0; i < arr.Length; i++)
                arr[i] = 0.0;

            var buf = new Matrix(rows, columns, arr);

            return buf;
        }

        public static void Free(params Matrix[] matrices)
        {
            foreach(var mtx in matrices)
            {
                Pool.Free(mtx.CoreArray);
                mtx.CoreArray = null;
            }
        }
    }
}
