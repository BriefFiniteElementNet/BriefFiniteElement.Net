using CSparse.Double;
using CSparse.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Common.Math
{
    /// <summary>
    /// Represents a permutation matrix (stored in a integer array)
    /// </summary>
    [DebuggerDisplay("{RowCount} x {ColumnCount}")]
    public class HollowPermutationMatrix
    {


        public HollowPermutationMatrix Transpose()
        {
            return HollowPermutationMatrix.Transpose(this);
        }

        public HollowPermutationMatrix()
        {
        }

        public HollowPermutationMatrix(int[] p, int columnCount)
        {
            P = p;
            ColumnCount = columnCount;
        }

        public int[] P;

        public int RowCount { get { return P.Length; } }

        public int ColumnCount { get; set; }

        public static void Pa(HollowPermutationMatrix p, double[] a, double[] b)
        {
            //b+=p.A

            if (p.ColumnCount != a.Length)
                throw new ArgumentException();

            if (p.RowCount != b.Length)
                throw new ArgumentException();

            
            var n = p.RowCount;

            for (int i = 0; i < n; i++)
            {
                var oldRow = i;
                var newRow = p.P[i];

                if (newRow == -1)
                    continue;

                b[oldRow] += a[newRow];
            }
        }

        public static SparseMatrix Pta(HollowPermutationMatrix p, SparseMatrix a)
        {
            //b=p^t.A

            if (p.RowCount != a.RowCount)
                throw new ArgumentException();

            var buf = new CoordinateStorage<double>(p.ColumnCount, a.ColumnCount, a.NonZerosCount);


            foreach (var i in a.EnumerateIndexed())
            {
                var rw = i.Item1;
                var col = i.Item2;
                var val = i.Item3;

                var newRow = p.P[rw];

                if (newRow == -1)
                    continue;

                buf.At(newRow, col, val);
            }

            return BriefFiniteElementNet.Common.Utils.MatrixUtil.ToCCs(buf);
        }


        public static SparseMatrix AP(HollowPermutationMatrix p, SparseMatrix a)
        {
            //b=A.P

            if (p.RowCount != a.ColumnCount)
                throw new ArgumentException();

            var buf = new CoordinateStorage<double>(a.RowCount, p.ColumnCount, a.NonZerosCount);


            foreach (var i in a.EnumerateIndexed())
            {
                var rw = i.Item1;
                var col = i.Item2;
                var val = i.Item3;

                var newCol = p.P[col];

                if (newCol == -1)
                    continue;

                buf.At(rw, newCol, val);
            }

            return BriefFiniteElementNet.Common.Utils.MatrixUtil.ToCCs(buf);
        }


        public static SparseMatrix PtaQ(HollowPermutationMatrix p, HollowPermutationMatrix q, SparseMatrix a)
        {
            //b=p.A

            if (p.RowCount != a.RowCount)
                throw new ArgumentException();

            if (a.ColumnCount != q.RowCount)
                throw new ArgumentException();

            var buf = new CoordinateStorage<double>(p.ColumnCount, q.ColumnCount, a.NonZerosCount);


            foreach (var i in a.EnumerateIndexed())
            {
                var rw = i.Item1;
                var col = i.Item2;
                var val = i.Item3;

                var newRow = p.P[rw];
                var newCol = q.P[col];

                if (newRow == -1 || newCol == -1)
                    continue;


                buf.At(newRow, newCol, val);
            }

            return BriefFiniteElementNet.Common.Utils.MatrixUtil.ToCCs(buf);
        }


        public static void Pta(HollowPermutationMatrix p, double[] a, double[] b)
        {
            //b+=p^T.A

            if (p.RowCount != a.Length)
                throw new ArgumentException();

            if (p.ColumnCount != b.Length)
                throw new ArgumentException();

            var n = p.RowCount;


            for (var i = 0; i < n; i++)
            {
                var oldRow = i;
                var newRow = p.P[i];

                if (newRow == -1)
                    continue;

                b[newRow] += a[oldRow];
            }
        }

        public static HollowPermutationMatrix Transpose(HollowPermutationMatrix mtx)
        {
            var p2 = new int[mtx.ColumnCount];

            for (int i = 0; i < p2.Length; i++)
            {
                p2[i] = -1;
            }

            for (var i = 0; i < mtx.P.Length; i++)
            {
                var j = mtx.P[i];

                if (j != -1)
                    p2[j] = i;
            }

            var buf = new HollowPermutationMatrix();

            buf.P = p2;
            buf.ColumnCount = mtx.RowCount;

            return buf;
        }

        public static HollowPermutationMatrix FromSparseMatrix(SparseMatrix mtx)
        {
            var rowFlags = new int[mtx.RowCount];
            var colFlags = new int[mtx.ColumnCount];

            foreach (var item in mtx.EnumerateIndexed())
            {
                rowFlags[item.Item1]++;
                colFlags[item.Item2]++;
            }




            if (rowFlags.Max() >= 2)
                throw new Exception("invalid hollow permutation matrix");

            if (colFlags.Max() >= 2)
                throw new Exception("invalid hollow permutation matrix");

            if (mtx.Values.Any(i => (i != 0.0) && (i != 1.0)))
                throw new Exception("invalid hollow permutation matrix");

            var buf = new HollowPermutationMatrix();

            var p = buf.P = new int[mtx.RowCount];

            for (int i = 0; i < p.Length; i++)
                p[i] = -1;

            buf.ColumnCount = mtx.ColumnCount;

            foreach (var item in mtx.EnumerateIndexed())
            {
                var row = item.Item1;
                var col = item.Item2;

                p[row] = col;
            }


            return buf;
        }

        public static SparseMatrix ToSparseMatrix(HollowPermutationMatrix mtx)
        {
            var nnz = mtx.P.Count(i => i != -1);

            var buf = new CoordinateStorage<double>(mtx.RowCount, mtx.ColumnCount, nnz);

            for (var i = 0; i < mtx.RowCount; i++)
            {
                var j = mtx.P[i];

                if (j != -1)
                    buf.At(i, j, 1);
            }

            return BriefFiniteElementNet.Common.Utils.MatrixUtil.ToCCs(buf);
        }

        public static HollowPermutationMatrix GenerateRandom(int rowCount, int colCount, int hollows, int seed = 0)
        {
            var rnd = new Random(seed);

            var min = System.Math.Min(rowCount, colCount);
            var max = System.Math.Max(rowCount, colCount);


            if (hollows < 0 || hollows > min)
                throw new Exception();

            var pa = new int[rowCount];// Enumerable.Range(0, rowCount).ToArray();

            for (int i = 0; i < rowCount; i++)
            {
                if (i < min)
                    pa[i] = i;
                else
                    pa[i] = -1;
            }

            Shuffle(rnd, pa);

            var hc = 0;

            while (hc < hollows)
            {
                var idx = rnd.Next(0, rowCount);

                if (pa[idx] == -1)
                    continue;

                pa[idx] = -1;
                hc++;
            }

            var buf = new HollowPermutationMatrix();

            buf.P = pa;
            buf.ColumnCount = colCount;

            return buf;
        }

        public static HollowPermutationMatrix GenerateRandom_old(int rowCount, int colCount, int hollows,  int seed = 0)
        {
            var rnd = new Random(seed);

            var min = System.Math.Min(rowCount, colCount);
            var max = System.Math.Max(rowCount, colCount);


            if (hollows < 0 || hollows > min)
                throw new Exception();

            var pa = new int[max];// Enumerable.Range(0, rowCount).ToArray();

            for (int i = 0; i < rowCount; i++)
            {
                if (i < min)
                    pa[i] = i;
                else
                    pa[i] = -1;
            }

            Shuffle(rnd, pa);

            var hc = 0;

            while (hc <= hollows)
            {
                var idx = rnd.Next(0, rowCount);

                if (pa[idx] == -1)
                    continue;

                pa[idx] = -1;
                hc++;
            }

            var buf = new HollowPermutationMatrix();

            buf.P = pa;
            buf.ColumnCount = colCount;

            return buf;
        }

        //https://stackoverflow.com/a/1262619
        public static void Shuffle<T>(Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
