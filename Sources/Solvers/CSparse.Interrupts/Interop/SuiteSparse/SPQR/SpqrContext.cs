
namespace CSparse.Interop.SuiteSparse.SPQR
{
    using CSparse.Factorization;
    using CSparse.Interop.SuiteSparse.Cholmod;
    using CSparse.Interop.Common;
    using CSparse.Storage;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// SPQR context wrapping native factorization.
    /// </summary>
    public abstract class SpqrContext<T> : IDisposableSolver<T>
        where T : struct, IEquatable<T>, IFormattable
    {
        protected readonly CompressedColumnStorage<T> matrix;

        protected IntPtr qr;

        protected CholmodCommon common;

        protected bool factorized;

        /// <summary>
        /// Initializes a new instance of the SpqrContext class.
        /// </summary>
        /// <param name="matrix">The sparse matrix to factorize.</param>
        public SpqrContext(CompressedColumnStorage<T> matrix)
        {
            this.matrix = matrix;

            common = new CholmodCommon();
            common.Initialize();
            
            Cholmod.NativeMethods.cholmod_start(ref common);
        }

        ~SpqrContext()
        {
            Dispose(false);
        }

        public abstract void Solve(T[] input, T[] result);

        public void Solve(DenseColumnMajorStorage<T> input, DenseColumnMajorStorage<T> result)
        {
            DoSolve(Constants.SPQR_RETX_EQUALS_B, Constants.SPQR_QTX, input, result);
        }

        protected virtual void DoSymbolic()
        {
            var h = new List<GCHandle>();

            try
            {
                var A = CreateSparse(matrix, h);

                int ordering = (int)SpqrOrdering.Default;
                int allow_tol = 1;

                qr = NativeMethods.SuiteSparseQR_C_symbolic(ordering, allow_tol, ref A, ref common);

                if (common.status != Constants.CHOLMOD_OK)
                {
                    throw new CholmodException(common.status);
                }
            }
            finally
            {
                InteropHelper.Free(h);
            }
        }

        protected virtual int DoNumeric()
        {
            if (qr == IntPtr.Zero)
            {
                throw new Exception("A call to DoNumeric() must be preceeded by DoSymbolic().");
            }

            var h = new List<GCHandle>();

            try
            {
                var A = CreateSparse(matrix, h);

                double tol = Constants.SPQR_DEFAULT_TOL;

                int status = NativeMethods.SuiteSparseQR_C_numeric(tol, ref A, qr, ref common);

                if (common.status != Constants.CHOLMOD_OK)
                {
                    throw new CholmodException(common.status);
                }

                return status;
            }
            finally
            {
                InteropHelper.Free(h);
            }
        }

        protected virtual void DoFactorize()
        {
            var h = new List<GCHandle>();

            try
            {
                var A = CreateSparse(matrix, h);
                
                int ordering = (int)SpqrOrdering.Default;
                double tol = Constants.SPQR_DEFAULT_TOL;

                qr = NativeMethods.SuiteSparseQR_C_factorize(ordering, tol, ref A, ref common);

                if (common.status != Constants.CHOLMOD_OK)
                {
                    throw new CholmodException(common.status);
                }
            }
            finally
            {
                InteropHelper.Free(h);
            }
        }
        
        protected virtual int DoSolve(int sys, int method, DenseColumnMajorStorage<T> input, DenseColumnMajorStorage<T> result)
        {
            if (!factorized)
            {
                DoFactorize();
            }

            var h = new List<GCHandle>();

            try
            {
                var B = CreateDense(input, h);
                
                // Y = Q'*B
                var Yp = NativeMethods.SuiteSparseQR_C_qmult(method, qr, ref B, ref common);

                if (Yp == IntPtr.Zero)
                {
                    return -1000;
                }

                // X = R\(E*Y)
                var Xp = NativeMethods.SuiteSparseQR_C_solve(sys, qr, Yp, ref common);

                if (Xp == IntPtr.Zero)
                {
                    return -1000;
                }

                var X = (CholmodDense)Marshal.PtrToStructure(Xp, typeof(CholmodDense));

                CopyDense(X, result);

                Cholmod.NativeMethods.cholmod_free_dense(ref Yp, ref common);
                Cholmod.NativeMethods.cholmod_free_dense(ref Xp, ref common);
                
                return 1;
            }
            finally
            {
                InteropHelper.Free(h);
            }
        }

        /// <summary>
        /// Copy native memory to dense matrix.
        /// </summary>
        /// <param name="dense">CHOLMOD dense matrix.</param>
        /// <param name="matrix">Target storage.</param>
        protected abstract void CopyDense(CholmodDense dense, DenseColumnMajorStorage<T> matrix);

        /// <summary>
        /// Create CHOLMOD dense matrix from managed type.
        /// </summary>
        /// <param name="matrix">The source matrix.</param>
        /// <param name="handles">List of handles.</param>
        /// <returns></returns>
        protected abstract CholmodDense CreateDense(DenseColumnMajorStorage<T> matrix, List<GCHandle> handles);

        /// <summary>
        /// Create CHOLMOD sparse matrix from managed type.
        /// </summary>
        /// <param name="matrix">The source matrix.</param>
        /// <param name="handles">List of handles.</param>
        /// <returns></returns>
        protected abstract CholmodSparse CreateSparse(CompressedColumnStorage<T> matrix, List<GCHandle> handles);
        
        #region IDisposable

        // See https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //InteropHelper.Free(handles);
            }

            if (qr != IntPtr.Zero)
            {
                NativeMethods.SuiteSparseQR_C_free(ref qr, ref common);

                Cholmod.NativeMethods.cholmod_finish(ref common);

                qr = IntPtr.Zero;
            }
        }

        #endregion
    }
}
