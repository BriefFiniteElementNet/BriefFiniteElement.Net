///Copyright http://geekswithblogs.net/
///
using System;

namespace BriefFiniteElementNet.Common
{
    public static class MathUtil
    {
        /// <summary>
        /// The epsilon, threshold using for determining whether two mumbers are equal or not
        /// </summary>
        //public static double Epsilon = 1e-8;
        /*
        /// <summary>
        /// Determines val1 equals val2 or not.
        /// </summary>
        /// <param name="val1">The val1.</param>
        /// <param name="val2">The val2.</param>
        /// <param name="epsilon">The epsilon.</param>
        /// <returns>
        /// true if val1 equals val2 else false
        /// </returns>
        public static bool Equals(double val1, double val2, double epsilon)
        {
            return FEquals(val1, val2, epsilon);
        }*/

        /// <summary>
        /// Determines val1 equals val2 or not with fuzzy approach
        /// </summary>
        /// <param name="val1">The val1.</param>
        /// <param name="val2">The val2.</param>
        /// <param name="epsilon">The epsilon.</param>
        /// <returns>
        /// true if val1 equals val2 else false
        /// </returns>
        public static bool FEquals(double val1, double val2, double epsilon=0)
        {
            var diff = System.Math.Abs(val1 - val2);

            return diff <= epsilon;
        }

        /// <summary>
        /// Fills the array with specified value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="val"></param>
        public static void FillWith<T>(this T[] arr, T val)
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

        private static bool SafeEquals(object o1,object o2)
        {
            if (ReferenceEquals(o1, o2))
                return true;

            if (ReferenceEquals(o1, null) || ReferenceEquals(o2, null))
                return false;

            return o1.Equals(o2);
        }

        // TODO: LEGACY CODE - FillLowerTriangleFromUpperTriangle, move out of main assembly?

        /// <summary>
        /// Fills the lower triangle from upper triangle.
        /// </summary>
        /// <param name="mtx">The Matrix (square matrix).</param>
        /// <remarks>
        /// In element stiffness matrix the lower triangle of matrix is symmetry of upper triangle
        /// of matrix (matrix is symmetric). For improving performance, it is suggested that only
        /// upper part (and diagonal) of member stiffness matrix be calculated and lower triangular
        /// be mirrored from upper part using this method
        /// </remarks>
        public static void FillLowerTriangleFromUpperTriangle(Matrix mtx)
        {
            var c = mtx.RowCount;

            for (var i = 0; i < c; i++)
                for (var j = 0; j < i; j++)
                    mtx[i, j] = mtx[j, i];

        }

        /* UNUSED

        /// <summary>
        /// Determines val1 equals val2 or not.
        /// </summary>
        /// <param name="val1">The val1.</param>
        /// <param name="val2">The val2.</param>
        /// <returns>true if val1 == val2 else false</returns>
        public static bool Equals_(double val1, double val2)
        {
            return Equals(val1, val2, Epsilon);
        }

        /// <summary>
        /// Calculates the minus of two array (a-b)
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">B.</param>
        /// <returns>A-B</returns>
        public static double[] ArrayMinus(double[] a, double[] b)
        {
            var buf = (double[])a.Clone();

            for (int i = 0; i < buf.Length; i++)
                buf[i] -= b[i];

            return buf;
        }

        //*/
    }
}
