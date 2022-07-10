
namespace CSparse.Interop.Spectra
{
    using CSparse.Interop.Common;
    using CSparse.Solvers;
    using CSparse.Storage;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    // TODO: no need for IDisposable at the moment.

    /// <summary>
    /// Spectra eigenvalue solver.
    /// </summary>
    public abstract class SpectraContext<T> : IEigenSolver<T>
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
        /// Gets or sets the residual tolerance.
        /// </summary>
        public double Tolerance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to compute eigenvectors.
        /// </summary>
        public bool ComputeEigenVectors { get; set; }

        /// <summary>
        /// Initialize the standard eigenvalue problem.
        /// </summary>
        public SpectraContext(CompressedColumnStorage<T> A, bool symmetric)
        {
            if (A.RowCount != A.ColumnCount)
            {
                throw new ArgumentException("Matrix must be square.", "A");
            }

            this.size = A.RowCount;
            this.A = A;

            this.symmetric = symmetric;

            Tolerance = 1e-5;
            Iterations = 1000;
        }

        /// <summary>
        /// Initialize the generalized eigenvalue problem.
        /// </summary>
        public SpectraContext(CompressedColumnStorage<T> A, CompressedColumnStorage<T> B, bool symmetric)
            : this(A, symmetric)
        {
            if (B.RowCount != B.ColumnCount)
            {
                throw new ArgumentException("Matrix must be square.", "B");
            }

            if (this.size != B.RowCount)
            {
                throw new ArgumentException("Matrix must be of same dimension as A.", "B");
            }

            if (!symmetric)
            {
                //throw new ArgumentException(Properties.Resources.ArgumentMatrixSymmetric);
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

        internal spectra_spmat GetMatrix(CompressedColumnStorage<T> matrix, List<GCHandle> handles)
        {
            spectra_spmat a = default(spectra_spmat);

            a.n = size;
            a.p = InteropHelper.Pin(matrix.ColumnPointers, handles);
            a.i = InteropHelper.Pin(matrix.RowIndices, handles);
            a.x = InteropHelper.Pin(matrix.Values, handles);
            a.nnz = matrix.NonZerosCount;

            return a;
        }

        // See SELECT_EIGENVALUE enum in Spectra header SelectionRule.h
        internal int GetJob(Spectrum job)
        {
            switch (job)
            {
                case Spectrum.LargestMagnitude:
                    return 0;
                case Spectrum.LargestRealPart:
                    return 1;
                case Spectrum.LargestImaginaryPart:
                    return 2;
                case Spectrum.LargestAlgebraic:
                    return 3;
                case Spectrum.SmallestMagnitude:
                    return 4;
                case Spectrum.SmallestRealPart:
                    return 5;
                case Spectrum.SmallestImaginaryPart:
                    return 6;
                case Spectrum.SmallestAlgebraic:
                    return 7;
                case Spectrum.BothEnds:
                    return 8;
                default:
                    break;
            }

            throw new ArgumentException("", nameof(job));
        }
    }
}
