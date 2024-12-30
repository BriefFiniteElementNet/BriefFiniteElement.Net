///Copyright http://geekswithblogs.net/
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Common
{

    public class ArrayPool<T>
    {
        private readonly Dictionary<int, Stack<T[]>> _pool = new Dictionary<int, Stack<T[]>>();

        public readonly T[] Empty = new T[0];

        private readonly object _lock = new object();

        public virtual void Clear()
        {
            _pool.Clear();
        }

        internal virtual T[] Allocate(int size)
        {
            if (size < 0) throw new ArgumentOutOfRangeException("size", "Must be positive.");

            if (size == 0) return Empty;

            Stack<T[]> candidates;

            lock (_lock)
            {
                if (_pool.TryGetValue(size, out candidates))
                    if (candidates.Count > 0)
                        return candidates.Pop();
            }

            return new T[size];
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
