using CSparse;
using CSparse.Double;
using CSparse.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet
{

    //copy from csparse Permutation class
    //just allow hollow permutations
    //p[i]=j means p[i,j]=1
    public static class BfePermutation
    {


        public static int[] CreateIdentity(int n)
        {
            var buf = new int[n];
            for (int i = 0; i < n; i++)
            {
                buf[i]= i;
            }

            return buf;
            throw new NotImplementedException();
        }


        //convert permutation array to permutation matrix
        public static SparseMatrix PermutationArrayToMatrix(int[] arr ,int length=-1)
        {
            if (length == -1)
                length = arr.Max() + 1;

            var buf = new CoordinateStorage<double>(length, arr.Length, arr.Length);

            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i] == -1)
                    continue;

                buf.At(i, arr[i], 1);
            }

            return BriefFiniteElementNet.Common.Utils.MatrixUtil.ToCCs(buf);
        }

        //convert permutation array to permutation matrix
        public static int[] PermutationMatrixToArray(SparseMatrix matrix)
        {
            var max = matrix.RowCount;

            var buf = new int[max];

            for (var i = 0; i < buf.Length; i++)
                buf[i] = -1;

            foreach (var item in matrix.EnumerateIndexed())
            {
                var row = item.Item1;
                var col = item.Item2;
                var val = item.Item3;

                if (val != 1)
                    continue;

                buf[row] = col;
            }

            return buf;
        }

        /// <summary>
        /// Permutes a vector, x+=P^T*a.
        /// </summary>
        /// <param name="p">Permutation vector.</param>
        /// <param name="a">Input vector.</param>
        /// <param name="x">output</param>
        /// <param name="n">Length of p, b and x.</param>
        public static void Pta(int[] p, double[] a, double[] x, int n) 
        {
            var max = p.Max();

            if (max > n)
                throw new Exception();
            
            for (int i = 0; i < n; i++)
            {
                if (p[i] == -1)
                    continue;

                x[p[i]] += a[i];
            }
        }


        /// <summary>
        /// Permutes a vector, x+=P*a.
        /// </summary>
        /// <param name="p">Permutation vector.</param>
        /// <param name="a">Input vector.</param>
        /// <param name="x">output</param>
        /// <param name="n">Length of p, b and x.</param>
        public static void Pa(int[] p, double[] a, double[] x, int n)
        {
            var max = p.Max();

            if (max > n)
                throw new Exception();

            for (var i = 0; i < n; i++)
            {
                if (p[i] == -1)
                    continue;

                x[i] += a[p[i]];
            }
        }

        /// <summary>
        /// Permutes a vector, x+=P*a.
        /// </summary>
        /// <param name="p">Permutation vector.</param>
        /// <param name="a">Input vector.</param>
        /// <param name="x">output</param>
        /// <param name="n">Length of p, b and x.</param>
        public static SparseMatrix Pa(int[] p, SparseMatrix a,int n=-1)
        {
            if (n == -1)
                n = p.Max() + 1;

            var buf = new CoordinateStorage<double>(n,a.ColumnCount, a.NonZerosCount);

            foreach (var item in a.EnumerateIndexed())
            {
                var oldRow = item.Item1;
                var oldCol = item.Item2;
                var val = item.Item3;

                var newRow = p[oldRow];

                if (newRow == -1)
                    continue;

                buf.At(newRow, oldCol, val);
            }

            return BriefFiniteElementNet.Common.Utils.MatrixUtil.ToCCs(buf);
        }


        /// <summary>
        /// Calculates P.A.Q^T
        /// </summary>
        /// <param name="p">the P</param>
        /// <param name="q">the Q</param>
        /// <param name="a"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static SparseMatrix PaQt(int[] p, int[] q, SparseMatrix a, int n = -1,int m=-1)
        {
            if (n == -1)
            {
                n = Math.Max(p.Max() + 1, p.Length);
            }

            if (m == -1)
            {
                m = Math.Max(q.Max() + 1, q.Length);
            }
                

            var buf = new CoordinateStorage<double>(n, m, a.NonZerosCount);

            foreach (var item in a.EnumerateIndexed())
            {
                var oldRow = item.Item1;
                var oldCol = item.Item2;
                var val = item.Item3;

                var newRow = p[oldRow];
                var newCol = q[oldCol];

                if (newRow == -1 || newCol == -1)
                    continue;

                buf.At(newRow, newCol, val);
            }

            return BriefFiniteElementNet.Common.Utils.MatrixUtil.ToCCs(buf);
        }

        /// <summary>
        /// Permutes a vector, x+=P^T*a.
        /// </summary>
        /// <param name="p">Permutation vector.</param>
        /// <param name="a">Input vector.</param>
        /// <param name="x">output</param>
        /// <param name="n">Length of p, b and x.</param>
        public static SparseMatrix Pta(int[] p, SparseMatrix a)
        {
            //not simply possible to implement
            throw new NotImplementedException("Use Pa instead, not");
        }


        /// <summary>
        /// Permutes a matrix a*P^T
        /// </summary>
        /// <param name="p">Permutation vector.</param>
        /// <param name="a">Input vector.</param>
        /// <param name="x">output</param>
        /// <param name="n">Length of p, b and x.</param>
        public static SparseMatrix Apt(int[] p, SparseMatrix a, int n)
        {
            if (n == -1)
                n = p.Max() + 1;

            var buf = new CoordinateStorage<double>(a.RowCount, n, a.NonZerosCount);


            foreach (var item in a.EnumerateIndexed())
            {
                var oldRow = item.Item1;
                var oldCol = item.Item2;
                var val = item.Item3;

                if (oldCol >= p.Length)
                    continue;

                var newCol = p[oldCol];

                if (newCol == -1)
                    continue;

                buf.At(oldRow, newCol, val);
            }

            return BriefFiniteElementNet.Common.Utils.MatrixUtil.ToCCs(buf);
        }


        public static int[] InvertPermutation(int[] p,int n)
        {
            var max = p.Max();

            if (max >= n)
                throw new ArgumentException();

            var res = new int[n];


            for (int i = 0; i < res.Length; i++)
            {
                res[i] = -1;
            }

            for (int i = 0; i < p.Length; i++)
            {
                
                if (p[i] == -1)
                    continue;

                res[p[i]] = i;
            }

            return res;
        }
    }
}
