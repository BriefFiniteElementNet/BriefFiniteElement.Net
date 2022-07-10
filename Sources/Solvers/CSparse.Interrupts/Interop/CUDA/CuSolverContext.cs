
namespace CSparse.Interop.CUDA
{
    using CSparse.Factorization;
    using CSparse.Interop.Common;
    using CSparse.Storage;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The CuSolver context represents a solver and the associated workspace in device memory.
    /// </summary>
    /// <remarks>
    /// The solver is based on the CUSOLVER low level interface.
    /// </remarks>
    public abstract class CuSolverContext<T> : IDisposableSolver<T>
        where T : struct, IEquatable<T>, IFormattable
    {
        protected readonly CompressedColumnStorage<T> matrix;
        
        protected bool factorized;

        protected IntPtr _p; // cusolverSp
        protected IntPtr _buffer;

        // Vectors stored in device memory.
        protected IntPtr d_x;
        protected IntPtr d_b;

        protected bool transpose;
        protected int sizeT;

        // The lifetime of the stream is controlled by the calling code!
        private CudaStream stream;

        /// <summary>
        /// Gets the factorization time (seconds).
        /// </summary>
        public double FactorizationTime { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CuSolverContext{T}"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="CudaStream"/>.</param>
        /// <param name="A">The sparse matrix.</param>
        /// <param name="transpose">A value indicating, whether the storage should be transposed.</param>
        /// <remarks>
        /// Matrix transposition is done on a storage level, meaning, for complex matrices, values will not be
        /// conjugated. This is necessary, because CUDA expects CSR storage, while CSparse uses CSC storage.
        /// 
        /// The value of <paramref name="transpose"/> should be true for all matrix types, except real
        /// valued, symmetric matrices.
        /// </remarks>
        public CuSolverContext(CudaStream stream, CompressedColumnStorage<T> A, bool transpose)
        {
            Check(NativeMethods.cusolverSpCreate(ref _p));
            Check(NativeMethods.cusolverSpSetStream(_p, stream.Pointer));

            sizeT = Marshal.SizeOf(typeof(T));

            this.stream = stream;
            this.matrix = A;
            this.transpose = transpose;
        }

        ~CuSolverContext()
        {
            Dispose(false);
        }

        /// <summary>
        /// Solves a system of linear equations, Ax = b.
        /// </summary>
        /// <param name="input">Right hand side vector b.</param>
        /// <param name="result">Solution vector x.</param>
        public void Solve(T[] input, T[] result)
        {
            if (!factorized)
            {
                Factorize();
            }

            var handles = new List<GCHandle>();

            try
            {
                var h_b = InteropHelper.Pin(input, handles);
                var h_x = InteropHelper.Pin(result, handles);

                int rows = input.Length;
                int columns = result.Length;

                Cuda.CopyToDevice(d_b, h_b, sizeT * rows);

                // Solve A*x = b.
                Check(Solve(rows, columns));

                Cuda.CopyToHost(h_x, d_x, sizeT * columns);
            }
            finally
            {
                InteropHelper.Free(handles);
            }
        }

        public abstract bool Singular(double tol);

        /// <summary>
        /// Factorize the sparse matrix associated to the solver instance.
        /// </summary>
        public void Factorize()
        {
            if (_buffer != IntPtr.Zero)
            {
                throw new Exception("Context already created.");
            }

            int rows = matrix.RowCount;
            int columns = matrix.ColumnCount;

            Cuda.Malloc(ref d_x, sizeT * columns);
            Cuda.Malloc(ref d_b, sizeT * rows);

            // TODO: can the original matrix really be disposed after factorization?

            using (var cusparse = new CuSparseContext<T>(stream, matrix, MatrixType.General, transpose))
            {
                PrepareFactorize();

                // Start the timer after the first call to cusolver.
                var timer = Stopwatch.StartNew();

                Factorize(rows, columns, matrix.NonZerosCount, cusparse);

                factorized = true;

                timer.Stop();

                FactorizationTime = TimeSpan.FromTicks(timer.ElapsedTicks).TotalSeconds;
            }
        }

        protected abstract SolverStatus Solve(int rows, int columns);

        protected abstract void Factorize(int rows, int columns, int nnz, CuSparseContext<T> A);

        protected abstract void PrepareFactorize();

        protected void Check(SolverStatus status)
        {
            if (status != SolverStatus.Success)
            {
                throw new CuSolverException(status);
            }
        }

        #region IDisposable

        // See https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose

        bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            // Free unmanaged objects.

            if (_p != IntPtr.Zero)
            {
                Check(NativeMethods.cusolverSpDestroy(_p));
                _p = IntPtr.Zero;
            }

            if (_buffer != IntPtr.Zero) Cuda.Free(_buffer);

            if (d_x != IntPtr.Zero) Cuda.Free(d_x);
            if (d_b != IntPtr.Zero) Cuda.Free(d_b);

            disposed = true;
        }

        #endregion
    }
}
