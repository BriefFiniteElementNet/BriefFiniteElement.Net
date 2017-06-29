using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a cache for caching values from complicated calculation and reusing them
    /// </summary>
    [Obsolete("No use")]
    public static class CalculationCache
    {
        private static ConcurrentDictionary<DictionaryKey, object> Dict;

        /// <summary>
        /// Adds a value to cache
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public static void SetValue(object key, int tag, object value)
        {
            var fullKey = new DictionaryKey() { Key = key, Tag = tag };

            Dict[fullKey] = value;
        }

        /// <summary>
        /// Tries to get the value based on defined key and tag.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static bool TryGetValue<T>(object key, int tag, out T value)
        {
            var fullKey = new DictionaryKey() { Key = key, Tag = tag };

            object buf;

            if(!Dict.TryGetValue(fullKey,out buf))
            {
                value = default(T);
                return false;
            }

            if (ReferenceEquals(buf, null))
            {
                value = default(T);
                return true;
            }

            if (typeof(T) == buf.GetType())
            {
                value = (T)buf;
                return true;
            }

            value = default(T);
            return false;
        }

        private class DictionaryKey : IEquatable<DictionaryKey>
        {
            public object Key;
            public int Tag;

            public override int GetHashCode()
            {
                return (Key.GetHashCode() * 397) ^ Tag.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public bool Equals(DictionaryKey other)
            {
                if (this.Tag != other.Tag)
                    return false;

                if (this.Key.Equals(other.Key))
                    return false;

                return true;
            }

            private static bool EqualKeys(DictionaryKey k1, DictionaryKey k2)
            {
                if (k1.Key.Equals(k2.Key))
                    return true;

                if (k1.GetType() != k2.GetType())
                    return false;

                #region Array

                if (k1 is Array && k2 is Array)
                {
                    var a1 = k1.Key as Array;
                    var a2 = k2.Key as Array;

                    if (a1.Length != a2.Length)
                        return false;

                    var l = a1.Length;

                    for (var j = 0; j < l; j++)
                    {
                        if (a1.GetValue(j) != a2.GetValue(j))
                            return false;
                    }

                    return true;
                }

                #endregion

                #region IList

                if (k1 is IList && k2 is IList)
                {
                    var a1 = k1.Key as IList;
                    var a2 = k2.Key as IList;

                    if (a1.Count != a2.Count)
                        return false;

                    var l = a1.Count;

                    for (var j = 0; j < l; j++)
                    {
                        if (a1[j] != a2[j])
                            return false;
                    }

                    return true;
                }

                #endregion

                throw new NotImplementedException();
            }
        }
    }
}
