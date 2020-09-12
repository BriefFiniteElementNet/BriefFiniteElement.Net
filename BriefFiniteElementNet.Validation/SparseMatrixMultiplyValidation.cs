using System;
using System.Collections.Generic;
using System.Linq;

namespace BriefFiniteElementNet.Validation
{
    public static class SparseMatrixMultiplyValidation
    {
        class NonzeroPattern
        {
            #region nonzero Pattern

            /// <summary>
            /// The nonzero pattern for each column
            /// </summary>
            [NonSerialized]
            internal List<int>[] ColumnNonzeros;

            /// <summary>
            /// The nonzero pattern for each row
            /// </summary>
            [NonSerialized]
            internal List<int>[] RowNonzeros;


            public void UpdateNonzeroPattern(Matrix m)
            {
                #region row nonzeros

                if (RowNonzeros == null)
                    RowNonzeros = new List<int>[m.RowCount];

                for (int i = 0; i < m.RowCount; i++)
                {
                    if (RowNonzeros[i] == null)
                        RowNonzeros[i] = new List<int>();
                    else
                        RowNonzeros[i].Clear();


                    for (int j = 0; j < m.ColumnCount; j++)
                    {
                        if (!m[i, j].Equals(0.0))
                            RowNonzeros[i].Add(j);
                    }
                }

                #endregion

                #region col nonzeros

                if (ColumnNonzeros == null)
                    ColumnNonzeros = new List<int>[m.ColumnCount];

                for (int j = 0; j < m.ColumnCount; j++)
                {
                    if (ColumnNonzeros[j] == null)
                        ColumnNonzeros[j] = new List<int>();
                    else
                        ColumnNonzeros[j].Clear();


                    for (int i = 0; i < m.RowCount; i++)
                    {
                        if (!m[i, j].Equals(0.0))
                            ColumnNonzeros[j].Add(i);
                    }
                }

                #endregion
            }

            #endregion
        }

        public static void Test1()
        {
            var pattern = new NonzeroPattern();

            var d1 = 100;
            var d2 = 70;
            var d3 = 50;

            var m1 = new Matrix(d1, d2);
            var m2 = new Matrix(d2, d3);

            var rnd = new Random(10);

            var nnz1Percent = 25;
            var nnz2Percent = 25;

            FillRandomely(m1, m1.Values.Length * nnz1Percent / 100, rnd);
            FillRandomely(m2, m2.Values.Length * nnz2Percent / 100, rnd);

            pattern.UpdateNonzeroPattern(m1);
            pattern.UpdateNonzeroPattern(m2);

            var result1 = new Matrix(d1, d3);

            var sp = System.Diagnostics.Stopwatch.StartNew();
            //new SparseMatrixMultiplication() {M1 = m1, M2 = m2}.MultiplyM1M2(result1);
            Console.WriteLine("Sparse multiply takes {0} ms", sp.ElapsedMilliseconds);

            sp.Restart();
            var result2 = m1 * m2;
            Console.WriteLine("normal multiply takes {0} ms", sp.ElapsedMilliseconds);

            var d = result1 - result2;

            var maxErr = d.Values.Max(i => Math.Abs(i));

            Console.WriteLine("Max Absolute Err: {0:G}", maxErr);

        }

        private static void FillRandomely(Matrix matrix, int count, Random rnd)
        {
            while (matrix.Values.Count(i => i != 0.0) < count)
            {
                matrix[rnd.Next(0, matrix.RowCount), rnd.Next(0, matrix.ColumnCount)] = (1 - rnd.NextDouble()) * 1000;
            }
        }
    }
}
