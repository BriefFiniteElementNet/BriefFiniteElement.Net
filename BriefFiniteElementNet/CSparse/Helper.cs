// -----------------------------------------------------------------------
// <copyright file="Helper.cs">
// Original CSparse code by Tim Davis, http://www.cise.ufl.edu/research/sparse/CXSparse/
// CSparse.NET code by Christian Woltering, http://csparse.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

namespace BriefFiniteElementNet.CSparse
{
    internal static class Helper
    {
        /// <summary>
        /// Cumulative sum of given array.
        /// </summary>
        /// <param name="sum">Output: cumulative sum of counts</param>
        /// <param name="counts">input array, overwritten with sum</param>
        /// <param name="size">length of counts</param>
        /// <returns>sum[size] (non-zeros)</returns>
        public static int CumulativeSum(int[] sum, int[] counts, int size)
        {
            int i, nz = 0;

            for (i = 0; i < size; i++)
            {
                sum[i] = nz;
                nz += counts[i];
                counts[i] = sum[i]; // also copy p[0..n-1] back into c[0..n-1]
            }

            sum[size] = nz;

            return nz;
        }
    }
}
