
namespace CSparse.Double.Solver
{
    using CSparse.Interop.Common;
    using CSparse.Interop.MKL;
    using CSparse.Interop.MKL.ExtendedEigensolver;
    using CSparse.Storage;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;

    public class ExtendedEigensolver : ExtendedEigensolverContext<double>
    {
        /// <summary>
        /// Initializes a new instance of the ExtendedEigensolver class.
        /// </summary>
        /// <param name="A">Real symmetric matrix.</param>
        public ExtendedEigensolver(SparseMatrix A)
            : base(A, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExtendedEigensolver class.
        /// </summary>
        /// <param name="A">Real symmetric matrix.</param>
        /// <param name="symmetric">Set to true, if the matrix A is symmetric.</param>
        public ExtendedEigensolver(SparseMatrix A, bool symmetric)
            : base(A, symmetric)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExtendedEigensolver class.
        /// </summary>
        /// <param name="A">Real symmetric matrix.</param>
        /// <param name="B">Real symmetric positive definite matrix for generalized problem.</param>
        public ExtendedEigensolver(SparseMatrix A, SparseMatrix B)
            : base(A, B, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExtendedEigensolver class.
        /// </summary>
        /// <param name="A">Real symmetric matrix.</param>
        /// <param name="B">Real symmetric positive definite matrix for generalized problem.</param>
        /// <param name="symmetric">Set to true, if the matrix A is symmetric and B is symmetric positive definite.</param>
        public ExtendedEigensolver(SparseMatrix A, SparseMatrix B, bool symmetric)
            : base(A, B, symmetric)
        {
        }

        /// <inheritdoc />
        public override ExtendedEigensolverResult<double> SolveStandard(int k0, Job job)
        {
            return SolveStandard(k0, job, new DenseMatrix(A.RowCount, k0));
        }

        /// <inheritdoc />
        public override ExtendedEigensolverResult<double> SolveStandard(int k0, Job job, DenseColumnMajorStorage<double> eigenvectors)
        {
            // TODO: check eigenvectors matrix dimensions.

            string which = job == Job.Smallest ? "S" : "L"; 

            int n = A.RowCount;
            
            int k = 0; // Total number of eigenvalues found in the interval.

            var E = new double[k0]; // Eigenvalues
            var R = new double[k0]; // Residual
            var X = eigenvectors.Values; // Eigenvectors
            
            var h = new List<GCHandle>();

            try
            {
                var pe = InteropHelper.Pin(E, h);
                var px = InteropHelper.Pin(X, h);
                var pr = InteropHelper.Pin(R, h);

                var h_A = CreateMatrixHandle(A, h);
                var desc = CreateMatrixDecriptor();

                var info = NativeMethods.mkl_sparse_d_ev(new StringBuilder(which), pm, h_A, desc, k0, ref k, pe, px, pr);
                
                //if (info != sparse_status.SUCCESS)
                //{
                //    throw new Exception(info.ToString());
                //}

                return new ExtendedEigensolverResult(info, n, k, E, eigenvectors, R);
            }
            finally
            {
                InteropHelper.Free(h);
            }
        }

        /// <inheritdoc />
        public override ExtendedEigensolverResult<double> SolveGeneralized(int k0, Job job)
        {
            return SolveGeneralized(k0, job, new DenseMatrix(A.RowCount, k0));
        }

        /// <inheritdoc />
        public override ExtendedEigensolverResult<double> SolveGeneralized(int k0, Job job, DenseColumnMajorStorage<double> eigenvectors)
        {
            // TODO: check eigenvectors matrix dimensions.

            string which = job == Job.Smallest ? "S" : "L";

            int n = A.RowCount;
            
            int k = 0; // Total number of eigenvalues found in the interval.

            var E = new double[k0]; // Eigenvalues
            var R = new double[k0]; // Residual
            var X = eigenvectors.Values; // Eigenvectors
            
            var h = new List<GCHandle>();

            try
            {
                var pe = InteropHelper.Pin(E, h);
                var px = InteropHelper.Pin(X, h);
                var pr = InteropHelper.Pin(R, h);
                
                var h_A = CreateMatrixHandle(A, h);
                var h_B = CreateMatrixHandle(B, h);

                var descA = CreateMatrixDecriptor();
                var descB = CreateMatrixDecriptor();

                var info = NativeMethods.mkl_sparse_d_gv(new StringBuilder(which), pm, h_A, descA, h_B, descB, k0, ref k, pe, px, pr);
                
                return new ExtendedEigensolverResult(info, n, k, E, eigenvectors, R);
            }
            finally
            {
                InteropHelper.Free(h);
            }
        }

        private IntPtr CreateMatrixHandle(CompressedColumnStorage<double> matrix, List<GCHandle> h)
        {
            var a = InteropHelper.Pin(matrix.Values, h);
            var ia = InteropHelper.Pin(matrix.ColumnPointers, h);
            var ja = InteropHelper.Pin(matrix.RowIndices, h);

            IntPtr h_matrix = IntPtr.Zero;
            
            // HACK: the extended eigensolver expects CSR. Since the matrix is symmetric, CSR is same as CSC.

            var result = NativeMethods.mkl_sparse_d_create_csr(ref h_matrix, SparseIndexBase.Zero, matrix.RowCount, matrix.ColumnCount, ia, IntPtr.Add(ia, sizeof(int)), ja, a);

            if (result != SparseStatus.Success)
            {
                throw new Exception(result.ToString());
            }

            return h_matrix;
        }


        private MatrixDescriptor CreateMatrixDecriptor()
        {
            MatrixDescriptor desc = default(MatrixDescriptor);

            desc.type = SparseMatrixType.General;
            desc.mode = SparseFillMode.Full;
            desc.diag = SparseDiagType.NonUnit;

            return desc;
        }
    }
}
