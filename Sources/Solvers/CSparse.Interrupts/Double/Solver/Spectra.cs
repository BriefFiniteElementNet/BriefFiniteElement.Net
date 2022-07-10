
namespace CSparse.Double.Solver
{
    using CSparse.Interop.Common;
    using CSparse.Interop.Spectra;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System;
    using CSparse.Solvers;

    public sealed class Spectra : SpectraContext<double>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Spectra"/> class for the standard eigenvalue problem.
        /// </summary>
        /// <param name="A">Real matrix.</param>
        public Spectra(SparseMatrix A)
            : this(A, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Spectra"/> class for the standard eigenvalue problem.
        /// </summary>
        /// <param name="A">Real matrix.</param>
        /// <param name="symmetric">Set to true, if the matrix A is symmetric.</param>
        public Spectra(SparseMatrix A, bool symmetric)
            : base(A, symmetric)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Spectra"/> class for the generalized eigenvalue problem.
        /// </summary>
        /// <param name="A">Real matrix.</param>
        /// <param name="B">Real matrix for generalized problem.</param>
        public Spectra(SparseMatrix A, SparseMatrix B)
            : this(A, B, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Spectra"/> class for the generalized eigenvalue problem.
        /// </summary>
        /// <param name="A">Real matrix.</param>
        /// <param name="B">Real matrix for generalized problem.</param>
        /// <param name="symmetric">Set to true, if the matrix A is symmetric and B is symmetric positive definite.</param>
        public Spectra(SparseMatrix A, SparseMatrix B, bool symmetric)
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
        public override IEigenSolverResult SolveStandard(int k, Spectrum job = Spectrum.LargestMagnitude)
        {
            if (!Job.Validate(symmetric, job))
            {
                throw new ArgumentException("Invalid job for given eigenvalue problem.", "job");
            }

            if (ArnoldiCount < k)
            {
                ArnoldiCount = Math.Min(3 * k, A.RowCount);
            }

            var result = new SpectraResult(k, size, ComputeEigenVectors, symmetric);

            var handles = new List<GCHandle>();

            var a = GetMatrix(A, handles);
            var e = result.GetEigenvalueStorage(handles);

            int conv = 0;

            if (symmetric)
            {
                conv = NativeMethods.spectra_di_ss(GetJob(job), k, ArnoldiCount,
                    Iterations, Tolerance, ref a, ref e);
            }
            else
            {
                conv = NativeMethods.spectra_di_ns(GetJob(job), k, ArnoldiCount,
                    Iterations, Tolerance, ref a, ref e);
            }

            result.IterationsTaken = e.iterations;
            result.ConvergedEigenValues = conv;
            result.ErrorCode = e.info;

            InteropHelper.Free(handles);

            return result;
        }

        /// <summary>
        /// Solve the standard eigenvalue problem in shift-invert mode.
        /// </summary>
        public override IEigenSolverResult SolveStandard(int k, double sigma, Spectrum job = Spectrum.LargestMagnitude)
        {
            if (!Job.Validate(symmetric, job))
            {
                throw new ArgumentException("Invalid job for given eigenvalue problem.", "job");
            }

            if (ArnoldiCount < k)
            {
                ArnoldiCount = Math.Min(3 * k, A.RowCount);
            }

            var result = new SpectraResult(k, size, ComputeEigenVectors, symmetric);

            var handles = new List<GCHandle>();

            var a = GetMatrix(A, handles);
            var e = result.GetEigenvalueStorage(handles);

            int conv = 0;

            if (symmetric)
            {
                conv = NativeMethods.spectra_di_ss_shift(GetJob(job), k, ArnoldiCount, Iterations,
                    Tolerance, sigma, ref a, ref e);
            }
            else
            {
                conv = NativeMethods.spectra_di_ns_shift(GetJob(job), k, ArnoldiCount, Iterations,
                    Tolerance, sigma, ref a, ref e);
            }

            result.IterationsTaken = e.iterations;
            result.ConvergedEigenValues = conv;
            result.ErrorCode = e.info;

            InteropHelper.Free(handles);

            return result;
        }

        /// <summary>
        /// Solve the generalized eigenvalue problem.
        /// </summary>
        public override IEigenSolverResult SolveGeneralized(int k, Spectrum job = Spectrum.LargestMagnitude)
        {
            if (!Job.Validate(symmetric, job))
            {
                throw new ArgumentException("Invalid job for given eigenvalue problem.", "job");
            }

            if (ArnoldiCount < k)
            {
                ArnoldiCount = Math.Min(3 * k, A.RowCount);
            }

            var result = new SpectraResult(k, size, ComputeEigenVectors, symmetric);

            var handles = new List<GCHandle>();

            var a = GetMatrix(A, handles);
            var b = GetMatrix(B, handles);
            var e = result.GetEigenvalueStorage(handles);

            int conv = 0;

            if (symmetric)
            {
                conv = NativeMethods.spectra_di_sg(GetJob(job), k, ArnoldiCount,
                    Iterations, Tolerance, ref a, ref b, ref e);
            }
            else
            {
                throw new NotImplementedException();
                //conv = NativeMethods.spectra_di_ng(GetJob(job), k, ArnoldiCount,
                //    Iterations, Tolerance, ref a, ref b, ref e);
            }

            result.IterationsTaken = e.iterations;
            result.ConvergedEigenValues = conv;
            result.ErrorCode = e.info;

            InteropHelper.Free(handles);

            return result;
        }

        /// <summary>
        /// Solve the generalized eigenvalue problem in user-defined shift-invert mode.
        /// </summary>
        public override IEigenSolverResult SolveGeneralized(int k, double sigma, Spectrum job = Spectrum.LargestMagnitude)
        {
            return SolveGeneralized(k, sigma, ShiftMode.Regular, job);
        }

        /// <summary>
        /// Solve the generalized eigenvalue problem in user-defined shift-invert mode.
        /// </summary>
        public IEigenSolverResult SolveGeneralized(int k, double sigma, ShiftMode mode, Spectrum job = Spectrum.LargestMagnitude)
        {
            if (!symmetric && !(mode == ShiftMode.None || mode == ShiftMode.Regular))
            {
                throw new InvalidOperationException("This mode is only available for symmetric eigenvalue problems.");
            }

            if (!Job.Validate(symmetric, job))
            {
                throw new ArgumentException("Invalid job for symmetric eigenvalue problem.", "job");
            }

            if (ArnoldiCount < k)
            {
                ArnoldiCount = Math.Min(3 * k, A.RowCount);
            }

            var result = new SpectraResult(k, size, ComputeEigenVectors, symmetric);

            var handles = new List<GCHandle>();

            var a = GetMatrix(A, handles);
            var b = GetMatrix(B, handles);
            var e = result.GetEigenvalueStorage(handles);

            int conv = 0;

            if (symmetric)
            {
                char m = 'S';

                if (mode == ShiftMode.Buckling)
                {
                    m = 'B';
                }
                else if (mode == ShiftMode.Cayley)
                {
                    m = 'C';
                }

                conv = NativeMethods.spectra_di_sg_shift(GetJob(job), m, k, ArnoldiCount, Iterations,
                    Tolerance, sigma, ref a, ref b, ref e);
            }
            else
            {
                throw new NotImplementedException("Shift-invert mode only available for symmetric problems.");
            }

            result.IterationsTaken = e.iterations;
            result.ConvergedEigenValues = conv;
            result.ErrorCode = e.info;

            InteropHelper.Free(handles);

            return result;
        }

        #endregion
    }
}
