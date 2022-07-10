
namespace CSparse.Interop.MKL.ExtendedEigensolver
{
    using CSparse.Storage;
    using System;

    public enum EigensolverAlgorithm
    {
        Auto = 0,
        KrylovSchur = 1,
        FEAST = 2
    }

    public enum Job
    {
        Smallest = 0,
        Largest = 1
    }

    // TODO: no need for IDisposable at the moment.

    /// <summary>
    /// Extended eigensolver (MKL 2019.0 supports real symmetric matrices only).
    /// </summary>
    public abstract class ExtendedEigensolverContext<T> : IDisposable
        where T : struct, IEquatable<T>, IFormattable
    {
        // Sparse matrix (column compressed storage).
        protected readonly CompressedColumnStorage<T> A;

        // Sparse matrix for generalized problems.
        protected readonly CompressedColumnStorage<T> B;

        protected readonly int[] pm;

        // TODO: implement MKL extended eigensolver symmetric mode.
        protected bool symmetric;

        #region Options

        // See https://software.intel.com/en-us/mkl-developer-reference-c-extended-eigensolver-input-parameters-for-extremal-eigenvalue-problem

        /// <summary>
        /// Gets or sets tolerance.
        /// </summary>
        public int ErrorTolerance
        {
            get { return pm[1]; }
            set { pm[1] = value; }
        }

        /// <summary>
        /// Gets or sets algorithm used to compute eigenvalues.
        /// </summary>
        public EigensolverAlgorithm Algorithm
        {
            get { return (EigensolverAlgorithm)pm[2]; }
            set { pm[2] = (int)value; }
        }

        /// <summary>
        /// Gets or sets the number of Lanczos vectors generated at each iteration (Krylov-Schur only).
        /// </summary>
        /// <remarks>
        /// This parameter must be less than or equal to size of matrix and greater than number of eigenvalues (k0) to be computed.
        /// </remarks>
        public int LanczosVectors
        {
            get { return pm[3]; }
            set { pm[3] = value; }
        }

        /// <summary>
        /// Gets or sets maximum iteration number.
        /// </summary>
        public int MaxIterations
        {
            get { return pm[4]; }
            set { pm[4] = value; }
        }

        /// <summary>
        /// Gets or sets power of Chebychev expansion for approximate spectral projector (Krylov-Schur only).
        /// </summary>
        public int ChebychevExpansion
        {
            get { return pm[5]; }
            set { pm[5] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether eigenvectors should be computed (Krylov-Schur only).
        /// </summary>
        public bool ComputeEigenVectors
        {
            get { return pm[6] == 1; }
            set { pm[6] = value ? 1 : 0; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the absolute stopping criteria applies.
        /// </summary>
        public bool UseAbsoluteError
        {
            get { return pm[7] == 1; }
            set { pm[7] = value ? 1 : 0; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the true residual should be used.
        /// </summary>
        public bool UseTrueResidual
        {
            get { return pm[8] == 1; }
            set { pm[8] = value ? 1 : 0; }
        }

        #endregion
        
        /// <summary>
        /// Initialize the standard eigenvalue problem.
        /// </summary>
        protected ExtendedEigensolverContext(CompressedColumnStorage<T> A, bool symmetric)
        {
            if (A.RowCount != A.ColumnCount)
            {
                throw new ArgumentException("Matrix must be square.", "A");
            }

            this.A = A;

            pm = new int[128];

            NativeMethods.mkl_sparse_ee_init(pm);
        }

        /// <summary>
        /// Initialize the generalized eigenvalue problem.
        /// </summary>
        protected ExtendedEigensolverContext(CompressedColumnStorage<T> A, CompressedColumnStorage<T> B, bool symmetric)
            : this(A, symmetric)
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

        ~ExtendedEigensolverContext()
        {
            Dispose(false);
        }

        /// <summary>
        /// Solve the standard eigenvalue problem.
        /// </summary>
        /// <param name="k0">The initial guess for subspace dimension.</param>
        /// <param name="job"></param>
        /// <returns></returns>
        public abstract ExtendedEigensolverResult<T> SolveStandard(int k0, Job job);

        /// <summary>
        /// Solve the standard eigenvalue problem.
        /// </summary>
        /// <param name="k0">The initial guess for subspace dimension.</param>
        /// <param name="job"></param>
        /// <param name="eigenvectors">The subspace containing the eigenvectors (can be used for an initial guess).</param>
        /// <returns></returns>
        public abstract ExtendedEigensolverResult<T> SolveStandard(int k0, Job job, DenseColumnMajorStorage<T> eigenvectors);

        /// <summary>
        /// Solve the generalized eigenvalue problem.
        /// </summary>
        /// <param name="k0">The initial guess for subspace dimension.</param>
        /// <param name="job"></param>
        /// <returns></returns>
        public abstract ExtendedEigensolverResult<T> SolveGeneralized(int k0, Job job);

        /// <summary>
        /// Solve the generalized eigenvalue problem.
        /// </summary>
        /// <param name="k0">The initial guess for subspace dimension.</param>
        /// <param name="job"></param>
        /// <param name="eigenvectors">The subspace containing the eigenvectors (can be used for an initial guess).</param>
        /// <returns></returns>
        public abstract ExtendedEigensolverResult<T> SolveGeneralized(int k0, Job job, DenseColumnMajorStorage<T> eigenvectors);

        // See https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
        }
    }
}
