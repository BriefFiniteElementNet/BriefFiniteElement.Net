
namespace CSparse.Double.Factorization.MKL
{
    using CSparse.Interop.MKL.Pardiso;
    using System;

    /// <summary>
    /// PARDISO wrapper.
    /// </summary>
    public class Pardiso : PardisoContext<double>
    {
        /// <summary>
        /// Initializes a new instance of the Pardiso class.
        /// </summary>
        public Pardiso(SparseMatrix matrix)
            : base(matrix, PardisoMatrixType.RealNonsymmetric)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Pardiso class.
        /// </summary>
        public Pardiso(SparseMatrix matrix, int mtype)
            : base(matrix, mtype)
        {
            switch (mtype)
            {
                case PardisoMatrixType.RealNonsymmetric:
                case PardisoMatrixType.RealStructurallySymmetric:
                case PardisoMatrixType.RealSymmetricIndefinite:
                case PardisoMatrixType.RealSymmetricPositiveDefinite:
                    break;
                default:
                    throw new ArgumentException("Invalid matrix type: expected real.", "mtype");
            }
        }

        /// <inheritdoc />
        protected override void Solve(int sys, double[] input, double[] result)
        {
            Solve(sys, new DenseMatrix(matrix.RowCount, 1, input), new DenseMatrix(matrix.ColumnCount, 1, result));
        }
    }
}
