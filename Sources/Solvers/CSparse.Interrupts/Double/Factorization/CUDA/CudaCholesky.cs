
namespace CSparse.Double.Factorization.CUDA
{
    using CSparse.Interop.CUDA;
    using CSparse.Storage;
    using System;

    // Based on low level interface example cuSolverSp_LowlevelCholesky

    public class CudaCholesky : CuSolverContext<double>
    {
        private IntPtr _info;

        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CudaCholesky"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="CudaStream"/>.</param>
        /// <param name="A">The sparse matrix.</param>
        public CudaCholesky(CudaStream stream, CompressedColumnStorage<double> A)
            : this(stream, A, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CudaCholesky"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="CudaStream"/>.</param>
        /// <param name="A">The sparse matrix.</param>
        /// <param name="transpose">A value indicating, whether the storage should be transposed.</param>
        public CudaCholesky(CudaStream stream, CompressedColumnStorage<double> A, bool transpose)
            : base(stream, A, transpose)
        {
        }

        ~CudaCholesky()
        {
            Dispose(false);
        }

        protected override SolverStatus Solve(int rows, int columns)
        {
            return NativeMethods.cusolverSpDcsrcholSolve(_p, rows, d_b, d_x, _info, _buffer);
        }

        public override bool Singular(double tol)
        {
            int singularity = 0;

            // Check if the matrix is singular.
            Check(NativeMethods.cusolverSpDcsrcholZeroPivot(_p, _info, tol, ref singularity));

            return singularity >= 0;
        }

        protected override void Factorize(int rows, int columns, int nnz, CuSparseContext<double> A)
        {
            var ap = A.ColumnPointers;
            var ai = A.RowIndices;
            var ax = A.Values;

            var desc = A.MatrixDescriptor;
            
            // Analyze chol(A) to know structure of L.
            Check(NativeMethods.cusolverSpXcsrcholAnalysis(_p, rows, nnz, desc, ap, ai, _info));

            int size_internal = 0, size_chol = 0;

            // Workspace for chol(A).
            Check(NativeMethods.cusolverSpDcsrcholBufferInfo(_p, rows, nnz, desc, ax, ap, ai, _info,
                ref size_internal, ref size_chol));


            Cuda.Malloc(ref _buffer, sizeof(char) * size_chol);

            // Compute A = L*L^T.
            Check(NativeMethods.cusolverSpDcsrcholFactor(_p, rows, nnz, desc, ax, ap, ai, _info, _buffer));
        }

        protected override void PrepareFactorize()
        {
            // Create opaque info structure.
            Check(NativeMethods.cusolverSpCreateCsrcholInfo(ref _info));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed) return;

            if (_info != IntPtr.Zero)
            {
                Check(NativeMethods.cusolverSpDestroyCsrcholInfo(_info));
                _info = IntPtr.Zero;
            }

            disposed = true;

            base.Dispose(disposing);
        }
    }
}
