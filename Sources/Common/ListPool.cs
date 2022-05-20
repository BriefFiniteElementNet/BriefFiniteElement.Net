///Copyright http://geekswithblogs.net/
///

using System;
using System.Collections.Generic;
using System.Linq;

namespace BriefFiniteElementNet
{
    public class ListPool<T>
    {
        private readonly Stack<List<T>> _pool = new Stack<List<T>>();

        //public readonly T[] Empty = new T[0];

        public virtual void Clear()
        {
            _pool.Clear();
        }

        public virtual List<T> Allocate()
        {
            var buf = _pool.Any() ? _pool.Pop() : new List<T>();// .TryGetValue(size, out candidates) && candidates.Count > 0 ? candidates.Pop() : new T[size];

            buf.Clear();

            return buf;
        }

        public virtual void Free(List<T> array)
        {
            if (array == null) throw new ArgumentNullException("array");

            array.Clear();

            _pool.Push(array);
        }
    }
}
