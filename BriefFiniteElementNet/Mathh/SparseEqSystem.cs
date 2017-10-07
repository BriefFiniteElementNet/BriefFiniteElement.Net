using CSparse.Double;
using CSparse.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Mathh
{
    public class SparseEqSystem
    {
        public SortedSet<SparseRow> RowsPool;

        public SparseRow Rent(int minLength)
        {
            throw new NotImplementedException();
        }

        public void Return()
        {
            throw new NotImplementedException();
        }

        public CompressedColumnStorage ToCcs()
        {
            var crd = new CoordinateStorage<double>(this.RowCount, this.ColumnCount, this.Equations.Sum(i => i.Size) + 1);

            for(var i=0;i<this.Equations.Length;i++)
            {
                foreach(var tuple in this.Equations[i].EnumerateIndexed())
                {
                    crd.At(i, tuple.Item1, tuple.Item2);
                }
            }

            return crd.ToCCs();
        }


        public SparseRow[] Equations;

        /// <summary>
        /// The column nonzeros
        /// </summary>
        /// <remarks>
        /// nonzero is nonzero count
        /// </remarks>
        public int[] ColumnNonzeros;

        /// <summary>
        /// The row nonzeros
        /// </summary>
        /// <remarks>
        /// nonzero is nonzero count except last column (right side)
        /// </remarks>
        public int[] RowNonzeros;


        public static SparseEqSystem Generate(CSparse.Double.CompressedColumnStorage eqSystem)
        {
            var buf = new SparseEqSystem();
            var lastCol = eqSystem.ColumnCount;

            var eqs = buf.Equations = new SparseRow[eqSystem.RowCount];
            var colNnzs = buf.ColumnNonzeros = new int[eqSystem.ColumnCount];
            buf.RowNonzeros = new int[eqSystem.RowCount];


            for (var i = 0; i < eqs.Length; i++)
            {
                buf.Equations[i] = new SparseRow(1);
            }


            foreach(var tuple in eqSystem.EnumerateIndexed2())
            {
                var rw = tuple.Item1;//tuple.Item1;
                var col = tuple.Item2;//tuple.Item2;
                var val = tuple.Item3;

                eqs[rw].Add(col, val);

                if (col != lastCol)
                    colNnzs[col]++;
            }


            for (var i = 0; i < eqs.Length; i++)
            {
                buf.RowNonzeros[i] = buf.Equations[i].CalcNnz(lastCol);
            }

            buf.RowCount = eqSystem.RowCount;
            buf.ColumnCount= eqSystem.ColumnCount;

            return buf;
        }

        public int RowCount, ColumnCount;

    }
}
