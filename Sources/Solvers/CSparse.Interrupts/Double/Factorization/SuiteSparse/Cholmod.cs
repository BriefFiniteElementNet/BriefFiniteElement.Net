
namespace CSparse.Double.Factorization.SuiteSparse
{
    using CSparse.Interop.SuiteSparse.Cholmod;
    using CSparse.Storage;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// CHOLMOD wrapper.
    /// </summary>
    public class Cholmod : CholmodContext<double>
    {
        /// <summary>
        /// Initializes a new instance of the Cholmod class.
        /// </summary>
        public Cholmod(SparseMatrix matrix)
            : base(matrix)
        {
        }

        /// <inheritdoc />
        public override void Solve(double[] input, double[] result)
        {
            Solve(new DenseMatrix(matrix.RowCount, 1, input), new DenseMatrix(matrix.ColumnCount, 1, result));
        }

        protected override CholmodDense CreateDense(DenseColumnMajorStorage<double> matrix, List<GCHandle> handles)
        {
            return CholmodHelper.CreateDense(matrix, handles);
        }

        protected override CholmodSparse CreateSparse(CompressedColumnStorage<double> matrix, List<GCHandle> handles)
        {
            // TODO: Stype this should be configurable!
            return CholmodHelper.CreateSparse(matrix, Stype.Upper, handles);
        }

        protected override void CopyDense(CholmodDense dense, DenseColumnMajorStorage<double> matrix)
        {
            Marshal.Copy(dense.x, matrix.Values, 0, (int)dense.nzmax);
        }
    }
}
