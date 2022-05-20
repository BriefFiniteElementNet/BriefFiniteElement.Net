using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    public class SparseMatrixMultiplication
    {

        public Matrix M1;
        public Matrix M2;
        public Matrix M3;

        /// <summary>
        /// Multiplies the specified m1 with m2.
        /// </summary>
        /// <remarks>
        /// Operation can be done with simple * operator, but this is for sparse situation</remarks>
        /// <param name="m1">The m1.</param>
        /// <param name="m2">The m2.</param>
        /// <param name="result">The result.</param>
        public void MultiplyM1M2(Matrix result)
        {
            var m1 = M1;
            var m2 = M2;

            
            if (result.RowCount != m1.RowCount || result.ColumnCount != m2.ColumnCount)
                throw new Exception();

            result.Clear();

            for (var row1 = 0; row1 < m1.RowCount; row1++)
            {
                for (var col2 = 0; col2 < m2.ColumnCount; col2++)
                {
                    //multiply row1'th of M1 with col1'th column of M2

                    result[row1, col2] = M1RowByM2Col(row1, col2);
                }
            }
        }

        public void MultiplyM1M2M3(Matrix result)
        {
            var m1 = M1;
            var m2 = M2;
            var m3 = M3;

            if (result.RowCount != m1.RowCount || result.ColumnCount != m3.ColumnCount)
                throw new Exception();

            result.Clear();

            for (var col3 = 0; col3 < m3.ColumnCount; col3++)
            {
                //multiply row1'th of M1 with col1'th column of M2

                //result[row3, col3] = M1RowByM2Col(row3, col3);
            }
        }


        public double M1RowByM2Col(int m1Row, int m2Col)
        {
            //code from here: http://www.geeksforgeeks.org/union-and-intersection-of-two-sorted-arrays-2/

            var row = M1.RowNonzeros[m1Row];
            var col = M2.ColumnNonzeros[m2Col];

            int i = 0, j = 0;

            var buf = 0.0;

            while (i < row.Count && j < col.Count)
            {
                if (row[i] < col[j])
                    i++;
                else if (col[j] < row[i])
                    j++;
                else /* if arr1[i] == arr2[j] */
                {
                    buf += M1[m1Row, row[i++]]*M2[col[j++], m2Col];
                }
            }

            return buf;
        }
    }
}