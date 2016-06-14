using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation
{
    public class SparseMatrixMultiplyValidation
    {
        public static void Test1()
        {
            var d1 = 100;
            var d2 = 70;
            var d3 = 50;

            var m1 = new Matrix(d1, d2);
            var m2 = new Matrix(d2, d3);

            var rnd = new Random(10);

            var nnz1Percent = 25;
            var nnz2Percent = 25;

            FillRandomely(m1, m1.CoreArray.Length* nnz1Percent/ 100, rnd);
            FillRandomely(m2, m2.CoreArray.Length * nnz2Percent/ 100, rnd);

            m1.UpdateNonzeroPattern();
            m2.UpdateNonzeroPattern();

            var result1 = new Matrix(d1, d3);

            var sp = System.Diagnostics.Stopwatch.StartNew();
            new SparseMatrixMultiplication() {M1 = m1, M2 = m2}.MultiplyM1M2(result1);
            Console.WriteLine("Sparse multiply takes {0} ms", sp.ElapsedMilliseconds);

            sp.Restart();
            var result2 = m1*m2;
            Console.WriteLine("normal multiply takes {0} ms", sp.ElapsedMilliseconds);

            var d = result1 - result2;

            var maxErr = d.CoreArray.Max(i => Math.Abs(i));

            Console.WriteLine("Max Absolute Err: {0:G}",maxErr);

        }

        private static void FillRandomely(Matrix matrix, int count,Random rnd)
        {
            while (matrix.CoreArray.Count(i => i != 0.0) < count)
            {
                matrix[rnd.Next(0, matrix.RowCount), rnd.Next(0, matrix.ColumnCount)] = (1 - rnd.NextDouble()) * 1000;
            }
        }
    }
}
