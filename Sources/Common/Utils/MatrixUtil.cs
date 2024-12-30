using CSparse.Double;
using CSparse.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Common.Utils
{
    /// <summary>
    /// Some utilities for matrix
    /// </summary>
    public static class MatrixUtil
    {

        // TODO: EXTENSION - ToCCs could be removed (CSparse should handle this case well)
        public static SparseMatrix ToCCs(CoordinateStorage<double> crd)
        {
            //https://brieffiniteelementnet.codeplex.com/workitem/6
            if (crd.RowCount == 0 || crd.ColumnCount == 0)
                return new SparseMatrix(0, 0) { ColumnPointers = new int[0], RowIndices = new int[0], Values = new double[0] };

            return (SparseMatrix)SparseMatrix.OfIndexed(crd, true);
        }

    }
}
