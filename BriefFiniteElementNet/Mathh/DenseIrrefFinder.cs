using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSparse.Double;
using CSparse.Ordering;
using CCS= CSparse.Storage.CompressedColumnStorage<double>;
namespace BriefFiniteElementNet.Mathh
{
    public class DenseIrrefFinder:IRrefFinder
    {
        public CompressedColumnStorage CalculateRref(CCS a)
        {
            var parts = EnumerateGraphParts(a);

            throw new NotImplementedException();
        }

        public CCS GetVariableConnection(CCS a)
        {
            var t2 = a.Clone(false);

            t2.Values = new double[t2.RowIndices.Length];

            for (int i = 0; i < t2.Values.Length; i++)
            {
                t2.Values[i] = 1;
            }

            var t2tr = t2.Transpose();


            var buf = t2tr.Multiply(t2);

            for (int i = 0; i < buf.Values.Length; i++)
            {
                buf.Values[i] = 1;
            }


            return buf;
        }

        public int[] EnumerateGraphParts(CCS a)
        {
            var gr = GetVariableConnection(a);

            throw new NotImplementedException();
        }
    }
}
