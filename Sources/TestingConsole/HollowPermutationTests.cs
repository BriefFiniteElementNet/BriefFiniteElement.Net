using BriefFiniteElementNet;
using BriefFiniteElementNet.Common.Math;
using BriefFiniteElementNet.Utils;
using CSparse;
using CSparse.Double;
using CSparse.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingConsole
{
    internal static class HollowPermutationTests
    {
        public static void TestArrMtx()
        {
            //random perm to matrix to perm should result in same value as begining

            var n = 100;
            var m = 10;

            var p = HollowPermutationMatrix.GenerateRandom(n, m, m / 5);

            var mtx = HollowPermutationMatrix.ToSparseMatrix(p);
            var p2 = HollowPermutationMatrix.FromSparseMatrix(mtx);

            var a = p.P;
            var b = p2.P;

            if (p.RowCount != p2.RowCount)
                throw new Exception();

            if (p.ColumnCount != p2.ColumnCount)
                throw new Exception();

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    throw new Exception();
            }
        }

        public static void TestTranspose()
        {
            //random perm to matrix to perm should result in same value as begining

            var n = 100;
            var m = 10;

            var p = HollowPermutationMatrix.GenerateRandom(n, m, m / 5);
            var pt = HollowPermutationMatrix.Transpose(p);

            var mtx1 = HollowPermutationMatrix.ToSparseMatrix(pt);

            var mtx = HollowPermutationMatrix.ToSparseMatrix(p);
            var mtx2 = mtx.Transpose();

            mtx2.InplaceNegate();

            var zer = mtx1.Add(mtx2);

            var zerp = zer.InfinityNorm();

            if (zerp > 1e-5)
                throw new Exception();
        }

        public static void TestMultArr()
        {
            //perm*vec should be same as matrix*vec

            var n = 50;
            var m = 100;

            var P = HollowPermutationMatrix.GenerateRandom(n, m,  m / 5);

            var pMatrix = HollowPermutationMatrix.ToSparseMatrix(P);



            var arr = new double[m];
            var rnd = new Random(1);

            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = rnd.NextDouble();
            }

            var a1 = new double[n];
            var a2 = new double[n];

            HollowPermutationMatrix.Pa(P, arr, a1);

            pMatrix.Multiply(arr, a2);


            for (int i = 0; i < n; i++)
            {
                var eps = Math.Abs(a1[i] - a2[i]);

                if (eps > 1e-5)
                    throw new Exception();
            }
        }

        public static void TestMultArr_Transpose()
        {
            var n = 50;
            var m = 100;

            var P = HollowPermutationMatrix.GenerateRandom(n, m, m / 5);

            var pMatrix = HollowPermutationMatrix.ToSparseMatrix(P);

            var arr = new double[n];
            var rnd = new Random(1);

            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = rnd.NextDouble();
            }

            var a1 = new double[m];
            var a2 = new double[m];

            HollowPermutationMatrix.Pta(P, arr, a1);

            pMatrix.TransposeMultiply(arr, a2);

            for (var i = 0; i < m; i++)
            {
                var eps = Math.Abs(a1[i] - a2[i]);

                if (eps > 1e-5)
                    throw new Exception();
            }

        }

        public static void TestMultMtx()
        {
            var n = 10;
            var m1 = 50;
            var n2 = m1;
            var m2 = 100;

            var p = HollowPermutationMatrix.GenerateRandom(n, m1, m1 / 5);
            var pt = HollowPermutationMatrix.Transpose(p);

            var pm = HollowPermutationMatrix.ToSparseMatrix(p);

            var a = RandomMatrix(n2, m2);

            var res1 = HollowPermutationMatrix.Pta(pt, a);
            var res2 = pm.Multiply(a);


            res2.InplaceNegate();

            var zero = res1.Add(res2).InfinityNorm();


            if (zero > 1e-5)
                throw new Exception();
        }

        public static void TestMultMtx2()
        {
            var n1 = 10;
            var m1 = 50;
            var n2 = m1;
            var m2 = 100;

            var p = HollowPermutationMatrix.GenerateRandom(n2, m2, m2 / 5);
            var pt = HollowPermutationMatrix.Transpose(p);

            var pm = HollowPermutationMatrix.ToSparseMatrix(p);

            var a = RandomMatrix(n1, m1);

            var res1 = HollowPermutationMatrix.AP(p, a);
            var res2 = a.Multiply(pm);

            res2.InplaceNegate();

            var zero = res1.Add(res2).InfinityNorm();

            if (zero > 1e-5)
                throw new Exception();
        }

        public static void TestMult2Mtx()
        {
            var n1 = 40;
            var m1 = 50;
            var n2 = m1;
            var m2 = 60;
            var n3 = m2;
            var m3 = 45;

            var p = HollowPermutationMatrix.GenerateRandom(n1, m1, m1 / 5);
            var pt = HollowPermutationMatrix.Transpose(p);

            var q = HollowPermutationMatrix.GenerateRandom(n3, m3, m3 / 5);
            var qt = HollowPermutationMatrix.Transpose(q);

            var pm = HollowPermutationMatrix.ToSparseMatrix(p);
            var qm = HollowPermutationMatrix.ToSparseMatrix(q);

            var a = RandomMatrix(n2, m2);

            var res1 = HollowPermutationMatrix.PtaQ(pt, q, a);
            var res2 = pm.Multiply(a).Multiply(qm);

            res2.InplaceNegate();

            var zero = res1.Add(res2).InfinityNorm();

            if (zero > 1e-5)
                throw new Exception();
        }


        private static SparseMatrix RandomMatrix(int rows,int cols,int seed=0)
        {
            SparseMatrix A;

            var rnd = new Random(seed);

            {
                var mtx = new CoordinateStorage<double>(rows, cols, rows * cols);

                for (var i = 0; i < rows; i++)
                    for (var j = 0; j < cols; j++)
                        mtx.At(i, j, rnd.NextDouble());

                A = (SparseMatrix)mtx.ToCCs();
            }

            return A;

        }


        /*

        public static void TestPaQ()
        {
            var n = 10;
            var n2 = 50;

            var perm1 = Enumerable.Range(0, n).ToArray();
            var perm2 = Enumerable.Range(0, n2).ToArray();

            var rnd = new Random(1);
            rnd.Shuffle(perm1);
            rnd.Shuffle(perm2);

            var p2t = BfePermutation.InvertPermutation(perm2, n);

            var P1 = BfePermutation.PermutationArrayToMatrix(perm1, n);
            var P2 = BfePermutation.PermutationArrayToMatrix(perm2, n2);

            SparseMatrix A;

            {
                var mtx = new CoordinateStorage<double>(n, n2, n * n2);

                for (var i = 0; i < n; i++)
                    for (var j = 0; j < n2; j++)
                        mtx.At(i, j, rnd.NextDouble());

                A = (SparseMatrix)mtx.ToCCs();
            }

            var p1t = BfePermutation.InvertPermutation(perm2, n2);

            var a1 = BfePermutation.PaQt(perm1, perm2, A);

            var tmp = A.Multiply(P2.Transpose());

            var a2 = P1.Multiply(tmp);

            a2.InplaceNegate();

            var diff = a1.Add(a2);



            foreach (var i in diff.EnumerateIndexed())
            {
                if (Math.Abs(i.Item3) > 1e-5)
                    throw new Exception();
            }
        }
        */
        public static void Shuffle<T>(this Random rng, T[] array)
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
