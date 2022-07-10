
namespace CSparse.Double.Solver.MKL
{
    using CSparse.Interop.Common;
    using CSparse.Interop.MKL.Feast;
    using CSparse.Storage;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class Feast : FeastContext<double>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Feast"/> class.
        /// </summary>
        /// <param name="A">Real symmetric matrix.</param>
        public Feast(SparseMatrix A)
            : base(A)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Feast"/> class.
        /// </summary>
        /// <param name="A">Real symmetric matrix.</param>
        /// <param name="B">Real symmetric positive definite matrix for generalized problem.</param>
        public Feast(SparseMatrix A, SparseMatrix B)
            : base(A, B)
        {
        }

        /// <inheritdoc />
        public override FeastResult<double> SolveStandard(int m0, double emin, double emax)
        {
            return SolveStandard(m0, emin, emax, new DenseMatrix(A.RowCount, m0));
        }

        /// <inheritdoc />
        public override FeastResult<double> SolveStandard(int m0, double emin, double emax, DenseColumnMajorStorage<double> subspace)
        {
            // TODO: check subspace matrix dimensions.

            char uplo = 'F'; // Full matrix.
            int n = A.RowCount;

            double epsout; // Relative error on the trace.
            int loop; // Number of refinement loops.
            int m = m0; // Total number of eigenvalues found in the interval.
            int info;

            var E = new double[m0]; // Eigenvalues
            var R = new double[m0]; // Residual
            var X = subspace.Values; // Eigenvectors

            var fpm = this.Options.fpm;

            var h = new List<GCHandle>();

            try
            {
                Increment(A);

                var a = InteropHelper.Pin(A.Values, h);
                var ia = InteropHelper.Pin(A.ColumnPointers, h);
                var ja = InteropHelper.Pin(A.RowIndices, h);
                
                var pe = InteropHelper.Pin(E, h);
                var px = InteropHelper.Pin(X, h);
                var pr = InteropHelper.Pin(R, h);
                
                NativeMethods.dfeast_scsrev(ref uplo, ref n, a, ia, ja, fpm, out epsout, out loop, ref emin, ref emax, ref m0, pe, px, out m, pr, out info);

                Decrement(A);

                if (info < 0)
                {
                    throw new FeastException(info);
                }

                return new FeastResult(info, m0, n, loop, epsout, m, E, subspace, R);
            }
            finally
            {
                InteropHelper.Free(h);
            }
        }

        /// <inheritdoc />
        public override FeastResult<double> SolveGeneralized(int m0, double emin, double emax)
        {
            return SolveGeneralized(m0, emin, emax, new DenseMatrix(A.RowCount, m0));
        }

        /// <inheritdoc />
        public override FeastResult<double> SolveGeneralized(int m0, double emin, double emax, DenseColumnMajorStorage<double> subspace)
        {
            // TODO: check subspace matrix dimensions.

            char uplo = 'F'; // Full matrix.
            int n = A.RowCount;

            double epsout; // Relative error on the trace.
            int loop; // Number of refinement loops.
            int m = m0; // Total number of eigenvalues found in the interval.
            int info;

            var E = new double[m0]; // Eigenvalues
            var R = new double[m0]; // Residual
            var X = subspace.Values; // Eigenvectors

            var fpm = this.Options.fpm;

            var h = new List<GCHandle>();

            try
            {
                Increment(A);
                Increment(B);

                var a = InteropHelper.Pin(A.Values, h);
                var ia = InteropHelper.Pin(A.ColumnPointers, h);
                var ja = InteropHelper.Pin(A.RowIndices, h);
                
                var b = InteropHelper.Pin(B.Values, h);
                var ib = InteropHelper.Pin(B.ColumnPointers, h);
                var jb = InteropHelper.Pin(B.RowIndices, h);

                var pe = InteropHelper.Pin(E, h);
                var px = InteropHelper.Pin(X, h);
                var pr = InteropHelper.Pin(R, h);

                NativeMethods.dfeast_scsrgv(ref uplo, ref n, a, ia, ja, b, ib, jb, fpm, out epsout, out loop, ref emin, ref emax, ref m0, pe, px, out m, pr, out info);

                Decrement(A);
                Decrement(B);

                return new FeastResult(info, m0, n, loop, epsout, m, E, subspace, R);
            }
            finally
            {
                InteropHelper.Free(h);
            }
        }
    }
}
