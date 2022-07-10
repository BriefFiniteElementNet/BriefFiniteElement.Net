
namespace CSparse.Interop.MKL.Feast
{
    using CSparse.Storage;
    using System;

    // TODO: no need for IDisposable at the moment.

    /// <summary>
    /// FEAST eigenvalue solver (MKL supports real symmetric or complex Hermetian matrices only).
    /// </summary>
    public abstract class FeastContext<T> : IDisposable
        where T : struct, IEquatable<T>, IFormattable
    {
        // Sparse matrix (column compressed storage).
        protected readonly CompressedColumnStorage<T> A;

        // Sparse matrix for generalized problems.
        protected readonly CompressedColumnStorage<T> B;
        
        public FeastOptions Options { get; private set; }
        
        /// <summary>
        /// Initialize the standard eigenvalue problem.
        /// </summary>
        protected FeastContext(CompressedColumnStorage<T> A)
        {
            if (A.RowCount != A.ColumnCount)
            {
                throw new ArgumentException("Matrix must be square.", "A");
            }
            
            this.A = A;
            
            Options = new FeastOptions();

            DoInitialize();
        }

        /// <summary>
        /// Initialize the generalized eigenvalue problem.
        /// </summary>
        protected FeastContext(CompressedColumnStorage<T> A, CompressedColumnStorage<T> B)
            : this(A)
        {
            if (B.RowCount != B.ColumnCount)
            {
                throw new ArgumentException("Matrix must be square.", "B");
            }

            if (A.RowCount != B.RowCount)
            {
                throw new ArgumentException("Matrix must be of same dimension as A.", "B");
            }

            this.B = B;
        }
        
        ~FeastContext()
        {
            Dispose(false);
        }

        protected virtual void DoInitialize()
        {
            NativeMethods.feastinit(this.Options.fpm);
        }

        /// <summary>
        /// Solve the standard eigenvalue problem.
        /// </summary>
        /// <param name="m0">The initial guess for subspace dimension.</param>
        /// <param name="emin">The lower bound of the interval to be searched for eigenvalues.</param>
        /// <param name="emax">The upper bound of the interval to be searched for eigenvalues.</param>
        /// <returns></returns>
        public abstract FeastResult<T> SolveStandard(int m0, double emin, double emax);

        /// <summary>
        /// Solve the standard eigenvalue problem.
        /// </summary>
        /// <param name="m0">The initial guess for subspace dimension.</param>
        /// <param name="emin">The lower bound of the interval to be searched for eigenvalues.</param>
        /// <param name="emax">The upper bound of the interval to be searched for eigenvalues.</param>
        /// <param name="subspace">The subspace containing the eigenvectors (can be used for an initial guess).</param>
        /// <returns></returns>
        public abstract FeastResult<T> SolveStandard(int m0, double emin, double emax, DenseColumnMajorStorage<T> subspace);

        /// <summary>
        /// Solve the generalized eigenvalue problem.
        /// </summary>
        /// <param name="m0">The initial guess for subspace dimension.</param>
        /// <param name="emin">The lower bound of the interval to be searched for eigenvalues.</param>
        /// <param name="emax">The upper bound of the interval to be searched for eigenvalues.</param>
        /// <returns></returns>
        public abstract FeastResult<T> SolveGeneralized(int m0, double emin, double emax);

        /// <summary>
        /// Solve the generalized eigenvalue problem.
        /// </summary>
        /// <param name="m0">The initial guess for subspace dimension.</param>
        /// <param name="emin">The lower bound of the interval to be searched for eigenvalues.</param>
        /// <param name="emax">The upper bound of the interval to be searched for eigenvalues.</param>
        /// <param name="subspace">The subspace containing the eigenvectors (can be used for an initial guess).</param>
        /// <returns></returns>
        public abstract FeastResult<T> SolveGeneralized(int m0, double emin, double emax, DenseColumnMajorStorage<T> subspace);

        // See https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
        }

        // NOTE: there seems to be no way to put FEAST into C-mode (0-based indexing), so
        //       the storage has to modified accordingly.

        /// <summary>
        /// FEAST uses 1-based indexing (FORTRAN style).
        /// </summary>
        protected void Increment(CompressedColumnStorage<T> A)
        {
            var ia = A.ColumnPointers;
            var ja = A.RowIndices;
            
            for (int i = 0; i < ia.Length; i++)
            {
                ia[i]++;
            }
            
            for (int i = 0; i < ja.Length; i++)
            {
                ja[i]++;
            }
        }

        /// <summary>
        /// FEAST uses 1-based indexing (FORTRAN style).
        /// </summary>
        protected void Decrement(CompressedColumnStorage<T> A)
        {
            var ia = A.ColumnPointers;
            var ja = A.RowIndices;

            for (int i = 0; i < ia.Length; i++)
            {
                ia[i]--;
            }

            for (int i = 0; i < ja.Length; i++)
            {
                ja[i]--;
            }
        }
    }
}
