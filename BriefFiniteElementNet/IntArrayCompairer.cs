using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCS = CSparse.Double.CompressedColumnStorage;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a class for comparing two arrays of int
    /// </summary>
    public class IntArrayCompairer : IEqualityComparer<int[]>
    {
        /// <inheritdoc/>
        public int GetHashCode(int[] values)
        {
            var result = 0;
            var shift = 0;

            for (var i = 0; i < values.Length; i++)
            {
                shift = (shift + 11)%21;
                result ^= (values[i] + 1024) << shift;
            }

            return result;
        }

        /// <inheritdoc/>
        public bool Equals(int[] x, int[] y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            if (x.Length != y.Length)
                return false;

            for (int i = x.Length - 1; i >= 0; i--)
            {
                if (x[i] != y[i])
                    return false;
            }

            return true;
        }
    }

}