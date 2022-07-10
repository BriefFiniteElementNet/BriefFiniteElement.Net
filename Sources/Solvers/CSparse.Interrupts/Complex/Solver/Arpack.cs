
namespace CSparse.Complex.Solver
{
    using CSparse.Interop.ARPACK;
    using CSparse.Interop.Common;
    using CSparse.Solvers;
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Runtime.InteropServices;

    public sealed class Arpack : ArpackContext<Complex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Arpack"/> class for the standard eigenvalue problem.
        /// </summary>
        /// <param name="A">Complex matrix.</param>
        public Arpack(SparseMatrix A)
            : this(A, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Arpack"/> class for the standard eigenvalue problem.
        /// </summary>
        /// <param name="A">Complex matrix.</param>
        /// <param name="symmetric">Set to true, if the matrix A is Hermitian.</param>
        public Arpack(SparseMatrix A, bool symmetric)
            : base(A, symmetric)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Arpack"/> class for the generalized eigenvalue problem.
        /// </summary>
        /// <param name="A">Complex matrix.</param>
        /// <param name="B">Complex matrix for generalized problem.</param>
        public Arpack(SparseMatrix A, SparseMatrix B)
            : this(A, B, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Arpack"/> class for the generalized eigenvalue problem.
        /// </summary>
        /// <param name="A">Complex matrix.</param>
        /// <param name="B">Complex matrix for generalized problem.</param>
        /// <param name="symmetric">Set to true, if the matrix A is Hermitian and B is Hermitian positive definite.</param>
        public Arpack(SparseMatrix A, SparseMatrix B, bool symmetric)
            : base(A, B, symmetric)
        {
        }

        #region Overriden methods

        /// <summary>
        /// Solve the standard eigenvalue problem.
        /// </summary>
        /// <param name="k">The number of eigenvalues to compute.</param>
        /// <param name="job">The part of the spectrum to compute.</param>
        /// <returns>The number of converged eigenvalues.</returns>
        public override IEigenSolverResult SolveStandard(int k, Spectrum job)
        {
            if (!CheckSquare(A))
            {
                throw new InvalidOperationException("Cannot solve eigenvalue problem with non-square matrix.");
            }

            if (!Job.Validate(symmetric, job))
            {
                throw new ArgumentException("Invalid job for given eigenvalue problem.", "job");
            }

            var result = new ArpackResult(k, size, ComputeEigenVectors);

            var handles = new List<GCHandle>();

            var a = GetMatrix(A, handles);
            var e = result.GetEigenvalueStorage(handles);

            int conv = NativeMethods.ar_zi_ns(GetJob(job), k, ArnoldiCount,
                    Iterations, Tolerance, ref a, ref e);

            result.IterationsTaken = e.iterations;
            result.ArnoldiCount = e.ncv;
            result.ConvergedEigenValues = conv;
            result.ErrorCode = e.info;

            InteropHelper.Free(handles);

            return result;
        }

        /// <summary>
        /// Solve the standard eigenvalue problem in shift-invert mode.
        /// </summary>
        public override IEigenSolverResult SolveStandard(int k, Complex sigma, Spectrum job = Spectrum.LargestMagnitude)
        {
            if (!CheckSquare(A))
            {
                throw new InvalidOperationException("Cannot solve eigenvalue problem with non-square matrix.");
            }

            if (!Job.Validate(symmetric, job))
            {
                throw new ArgumentException("Invalid job for given eigenvalue problem.", "job");
            }

            var result = new ArpackResult(k, size, ComputeEigenVectors);

            var handles = new List<GCHandle>();

            var a = GetMatrix(A, handles);
            var e = result.GetEigenvalueStorage(handles);

            int conv = NativeMethods.ar_zi_ns_shift(GetJob(job), k, ArnoldiCount, Iterations,
                    Tolerance, sigma, ref a, ref e);

            result.IterationsTaken = e.iterations;
            result.ArnoldiCount = e.ncv;
            result.ConvergedEigenValues = conv;
            result.ErrorCode = e.info;

            InteropHelper.Free(handles);

            return result;
        }

        /// <summary>
        /// Solve the generalized eigenvalue problem.
        /// </summary>
        public override IEigenSolverResult SolveGeneralized(int k, Spectrum job)
        {
            if (!CheckSquare(A))
            {
                throw new InvalidOperationException("Cannot solve eigenvalue problem with non-square matrix.");
            }

            if (!Job.Validate(symmetric, job))
            {
                throw new ArgumentException("Invalid job for given eigenvalue problem.", "job");
            }

            var result = new ArpackResult(k, size, ComputeEigenVectors);

            var handles = new List<GCHandle>();

            var a = GetMatrix(A, handles);
            var b = GetMatrix(B, handles);
            var e = result.GetEigenvalueStorage(handles);

            int conv = NativeMethods.ar_zi_ng(GetJob(job), k, ArnoldiCount,
                    Iterations, Tolerance, ref a, ref b, ref e);

            result.IterationsTaken = e.iterations;
            result.ArnoldiCount = e.ncv;
            result.ConvergedEigenValues = conv;
            result.ErrorCode = e.info;

            InteropHelper.Free(handles);

            return result;
        }

        /// <summary>
        /// Solve the generalized eigenvalue problem in user-defined shift-invert mode.
        /// </summary>
        public override IEigenSolverResult SolveGeneralized(int k, Complex sigma, Spectrum job = Spectrum.LargestMagnitude)
        {
            if (!CheckSquare(A))
            {
                throw new InvalidOperationException("Cannot solve eigenvalue problem with non-square matrix.");
            }

            if (!Job.Validate(symmetric, job))
            {
                throw new ArgumentException("Invalid job for given eigenvalue problem.", "job");
            }

            var result = new ArpackResult(k, size, ComputeEigenVectors);

            var handles = new List<GCHandle>();

            var a = GetMatrix(A, handles);
            var b = GetMatrix(B, handles);
            var e = result.GetEigenvalueStorage(handles);

            int conv = NativeMethods.ar_zi_ng_shift(GetJob(job), k, ArnoldiCount, Iterations,
                    Tolerance, sigma, ref a, ref b, ref e);

            result.IterationsTaken = e.iterations;
            result.ArnoldiCount = e.ncv;
            result.ConvergedEigenValues = conv;
            result.ErrorCode = e.info;

            InteropHelper.Free(handles);

            return result;
        }

        #endregion
    }
}
