using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Mathh
{
    public static  class Extensions
    {
        public static Matrix ToDense(CSparse.Storage.CompressedColumnStorage<double> a)
        {
            var buf = new Matrix(a.RowCount, a.ColumnCount);

            foreach (var t in a.EnumerateIndexed())
                buf[t.Item1, t.Item2] = t.Item3;

            return buf;
        }
    }
}
