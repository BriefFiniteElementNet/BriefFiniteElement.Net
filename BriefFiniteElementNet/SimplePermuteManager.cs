using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    public class SimplePermuteManager
    {
        public static SimplePermuteManager Make(Func<int, int> permute, int originalLength, int newLength)
        {
            var buf = new SimplePermuteManager();

            buf.p = new int[originalLength];
            buf.ip = new int[newLength];
            buf.OriginalLength = originalLength;
            buf.NewLength = newLength;

            for (var i = 0; i < originalLength; i++)
            {
                var pi = buf.p[i] = permute(i);

                if (pi != -1)
                    buf.ip[buf.p[i]] = i;
            }

            return buf;
        }


        /// <summary>
        /// Applies the permutation into defined <see cref="matrix"/>.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>permuted matrix</returns>
        public Matrix ApplyTo(Matrix matrix)
        {
            if (matrix.RowCount != OriginalLength && matrix.ColumnCount != OriginalLength)
                throw new Exception();

            var buf = new Matrix(NewLength, NewLength);

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    if (p[i] != -1 && p[j] != -1)
                        buf[p[i], p[j]] = matrix[i, j];
                }
            }

            return buf;
        }

        private int[] p, ip;

        private int OriginalLength;

        private int NewLength;
    }
}
