
namespace CSparse.Complex.Factorization.MKL
{
    using CSparse.Interop.MKL.Pardiso;
    using System;
    using System.Numerics;

    /// <summary>
    /// PARDISO wrapper.
    /// </summary>
    public class Pardiso : PardisoContext<Complex>
    {
        /// <summary>
        /// Initializes a new instance of the Pardiso class.
        /// </summary>
        public Pardiso(SparseMatrix matrix)
            : base(matrix, PardisoMatrixType.ComplexNonsymmetric)
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
                case PardisoMatrixType.ComplexHermitianIndefinite:
                case PardisoMatrixType.ComplexHermitianPositiveDefinite:
                case PardisoMatrixType.ComplexNonsymmetric:
                case PardisoMatrixType.ComplexStructurallySymmetric:
                case PardisoMatrixType.ComplexSymmetric:
                    break;
                default:
                    throw new ArgumentException("Invalid matrix type: expected complex.", "mtype");
            }
        }

        /// <inheritdoc />
        protected override void Solve(int sys, Complex[] input, Complex[] result)
        {
            Solve(sys, new DenseMatrix(matrix.RowCount, 1, input), new DenseMatrix(matrix.ColumnCount, 1, result));
        }
    }
}
