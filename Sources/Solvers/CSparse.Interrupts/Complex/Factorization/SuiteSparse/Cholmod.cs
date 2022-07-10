
namespace CSparse.Complex.Factorization.SuiteSparse
{
    using CSparse.Interop.SuiteSparse.Cholmod;
    using CSparse.Storage;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// CHOLMOD wrapper.
    /// </summary>
    public class Cholmod : CholmodContext<Complex>
    {
        private double[] buffer;

        /// <summary>
        /// Initializes a new instance of the Cholmod class.
        /// </summary>
        public Cholmod(SparseMatrix matrix)
            : base(matrix)
        {
        }

        /// <inheritdoc />
        public override void Solve(Complex[] input, Complex[] result)
        {
            Solve(new DenseMatrix(matrix.RowCount, 1, input), new DenseMatrix(matrix.ColumnCount, 1, result));
        }

        protected override CholmodDense CreateDense(DenseColumnMajorStorage<Complex> matrix, List<GCHandle> handles)
        {
            return CholmodHelper.CreateDense(matrix, handles);
        }

        protected override CholmodSparse CreateSparse(CompressedColumnStorage<Complex> matrix, List<GCHandle> handles)
        {
            // TODO: Stype this should be configurable!
            return CholmodHelper.CreateSparse(matrix, Stype.Upper, handles);
        }

        protected override void CopyDense(CholmodDense dense, DenseColumnMajorStorage<Complex> matrix)
        {
            CholmodHelper.CopyArray(2 * (int)dense.nzmax, dense.x, matrix.Values, ref buffer);
        }
    }
}
