
namespace CSparse.Interop.ARPACK
{
    using CSparse.Interop.Common;
    using CSparse.Solvers;
    using CSparse.Storage;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;

    // TODO: no need for IDisposable at the moment.

    /// <summary>
    /// ARPACK eigenvalue solver.
    /// </summary>
    public abstract class ArpackContext<T> : IEigenSolver<T>
        where T : struct, IEquatable<T>, IFormattable
    {
        // Sparse matrix (column compressed storage).
        protected readonly CompressedColumnStorage<T> A;

        // Sparse matrix for generalized problems.
        protected readonly CompressedColumnStorage<T> B;

        protected bool symmetric;

        protected int size;

        /// <summary>
        /// Gets or sets the number of Arnoldi vectors used in each iteration.
        /// </summary>
        public int ArnoldiCount { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of iterations.
        /// </summary>
        public int Iterations { get; set; }

        /// <summary>
        /// Gets or sets the residual tolerance (if &lt;= 0, ARPACK will use machine epsilon).
        /// </summary>
        public double Tolerance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to compute eigenvectors.
        /// </summary>
        public bool ComputeEigenVectors { get; set; }

        /// <summary>
        /// Initialize the standard eigenvalue problem.
        /// </summary>
        public ArpackContext(CompressedColumnStorage<T> A, bool symmetric)
        {
            if (symmetric && !CheckSquare(A))
            {
                throw new ArgumentException("Matrix must be square.", "A");
            }

            this.size = A.RowCount;
            this.A = A;

            this.symmetric = symmetric;

            Iterations = 1000;
        }

        /// <summary>
        /// Initialize the generalized eigenvalue problem.
        /// </summary>
        public ArpackContext(CompressedColumnStorage<T> A, CompressedColumnStorage<T> B, bool symmetric)
            : this(A, symmetric)
        {
            if (!CheckSquare(B))
            {
                throw new ArgumentException("Matrix must be square.", "B");
            }

            if (this.size != B.RowCount)
            {
                throw new ArgumentException("Matrix must be of same dimension as A.", "B");
            }

            if (!symmetric)
            {
                if (!CheckSquare(A))
                {
                    throw new ArgumentException("Matrix must be square.", "A");
                }
            }

            this.B = B;
        }

        /// <summary>
        /// Solve the standard eigenvalue problem.
        /// </summary>
        /// <param name="k">The number of eigenvalues to compute.</param>
        /// <param name="job">The part of the spectrum to compute.</param>
        /// <returns>The number of converged eigenvalues.</returns>
        public abstract IEigenSolverResult SolveStandard(int k, Spectrum job);

        /// <summary>
        /// Solve the standard eigenvalue problem in shift-invert mode.
        /// </summary>
        /// <param name="k">The number of eigenvalues to compute.</param>
        /// <param name="sigma">The shift value.</param>
        /// <param name="job">The part of the spectrum to compute.</param>
        /// <returns>The number of converged eigenvalues.</returns>
        public abstract IEigenSolverResult SolveStandard(int k, T sigma, Spectrum job = Spectrum.LargestMagnitude);

        /// <summary>
        /// Solve the generalized eigenvalue problem.
        /// </summary>
        /// <param name="k">The number of eigenvalues to compute.</param>
        /// <param name="job">The part of the spectrum to compute.</param>
        /// <returns>The number of converged eigenvalues.</returns>
        public abstract IEigenSolverResult SolveGeneralized(int k, Spectrum job);

        /// <summary>
        /// Solve the generalized eigenvalue problem in shift-invert mode.
        /// </summary>
        /// <param name="k">The number of eigenvalues to compute.</param>
        /// <param name="sigma">The shift value.</param>
        /// <param name="job">The part of the spectrum to compute.</param>
        /// <returns>The number of converged eigenvalues.</returns>
        public abstract IEigenSolverResult SolveGeneralized(int k, T sigma, Spectrum job = Spectrum.LargestMagnitude);

        internal ar_spmat GetMatrix(CompressedColumnStorage<T> matrix, List<GCHandle> handles)
        {
            ar_spmat a = default(ar_spmat);

            a.m = matrix.RowCount;
            a.n = matrix.ColumnCount;
            a.p = InteropHelper.Pin(matrix.ColumnPointers, handles);
            a.i = InteropHelper.Pin(matrix.RowIndices, handles);
            a.x = InteropHelper.Pin(matrix.Values, handles);
            a.nnz = matrix.NonZerosCount;

            return a;
        }

        internal bool CheckSquare(CompressedColumnStorage<T> matrix)
        {
            return matrix.RowCount == matrix.ColumnCount;
        }

        internal StringBuilder GetJob(Spectrum job)
        {
            switch (job)
            {
                case Spectrum.LargestAlgebraic:
                    return new StringBuilder("LA");
                case Spectrum.LargestMagnitude:
                    return new StringBuilder("LM");
                case Spectrum.LargestRealPart:
                    return new StringBuilder("LR");
                case Spectrum.LargestImaginaryPart:
                    return new StringBuilder("LI");
                case Spectrum.SmallestAlgebraic:
                    return new StringBuilder("SA");
                case Spectrum.SmallestMagnitude:
                    return new StringBuilder("SM");
                case Spectrum.SmallestRealPart:
                    return new StringBuilder("SR");
                case Spectrum.SmallestImaginaryPart:
                    return new StringBuilder("SI");
                case Spectrum.BothEnds:
                    return new StringBuilder("BE");
                default:
                    break;
            }

            throw new ArgumentException("", nameof(job));
        }
    }
}
