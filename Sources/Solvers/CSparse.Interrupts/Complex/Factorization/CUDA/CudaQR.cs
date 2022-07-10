
namespace CSparse.Complex.Factorization.CUDA
{
    using CSparse.Interop.CUDA;
    using CSparse.Storage;
    using System;
    using System.Numerics;

    // Based on low level interface example cuSolverSp_LowlevelQR

    public class CudaQR : CuSolverContext<Complex>
    {
        private IntPtr _info;

        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CudaQR"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="CudaStream"/>.</param>
        /// <param name="A">The sparse matrix.</param>
        public CudaQR(CudaStream stream, CompressedColumnStorage<Complex> A)
            : this(stream, A, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CudaQR"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="CudaStream"/>.</param>
        /// <param name="A">The sparse matrix.</param>
        /// <param name="transpose">A value indicating, whether the storage should be transposed.</param>
        public CudaQR(CudaStream stream, CompressedColumnStorage<Complex> A, bool transpose)
            : base(stream, A, transpose)
        {
        }

        ~CudaQR()
        {
            Dispose(false);
        }

        protected override SolverStatus Solve(int rows, int columns)
        {
            return NativeMethods.cusolverSpZcsrqrSolve(_p, rows, columns, d_b, d_x, _info, _buffer);
        }

        public override bool Singular(double tol)
        {
            int singularity = 0;

            // Check if the matrix is singular.
            Check(NativeMethods.cusolverSpZcsrqrZeroPivot(_p, _info, tol, ref singularity));

            return singularity >= 0;
        }

        protected override void Factorize(int rows, int columns, int nnz, CuSparseContext<Complex> A)
        {
            var ap = A.ColumnPointers;
            var ai = A.RowIndices;
            var ax = A.Values;

            var desc = A.MatrixDescriptor;

            // Analyze qr(A) to know structure of L.
            Check(NativeMethods.cusolverSpXcsrqrAnalysis(_p, rows, columns, nnz, desc, ap, ai, _info));

            int size_internal = 0, size_qr = 0;

            // Workspace for qr(A).
            Check(NativeMethods.cusolverSpZcsrqrBufferInfo(_p, rows, columns, nnz, desc, ax, ap, ai, _info,
                ref size_internal, ref size_qr));

            Cuda.Malloc(ref _buffer, sizeof(char) * size_qr);

            Check(NativeMethods.cusolverSpZcsrqrSetup(_p, rows, columns, nnz, desc, ax, ap, ai, 0.0, _info));

            // Compute A = Q*R.
            Check(NativeMethods.cusolverSpZcsrqrFactor(_p, rows, columns, nnz, IntPtr.Zero, IntPtr.Zero, _info, _buffer));
        }

        protected override void PrepareFactorize()
        {
            // Create opaque info structure.
            Check(NativeMethods.cusolverSpCreateCsrqrInfo(ref _info));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed) return;

            if (_info != IntPtr.Zero)
            {
                Check(NativeMethods.cusolverSpDestroyCsrqrInfo(_info));
                _info = IntPtr.Zero;
            }

            disposed = true;

            base.Dispose(disposing);
        }
    }
}
