
namespace CSparse.Solvers
{
    using System;

    /// <summary>
    /// Interface for eigensolvers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEigenSolver<T>
        where T : struct, IEquatable<T>, IFormattable
    {
        /// <summary>
        /// Solve the standard eigenvalue problem.
        /// </summary>
        /// <param name="k">The number of eigenvalues to compute.</param>
        /// <param name="job">The part of the spectrum to compute.</param>
        /// <returns>Retruns an <see cref="IEigenSolverResult"/>.</returns>
        IEigenSolverResult SolveStandard(int k, Spectrum job);

        /// <summary>
        /// Solve the standard eigenvalue problem in shift-invert mode.
        /// </summary>
        /// <param name="k">The number of eigenvalues to compute.</param>
        /// <param name="sigma">The shift value.</param>
        /// <param name="job">The part of the spectrum to compute.</param>
        /// <returns>Retruns an <see cref="IEigenSolverResult"/>.</returns>
        IEigenSolverResult SolveStandard(int k, T sigma, Spectrum job);

        /// <summary>
        /// Solve the generalized eigenvalue problem.
        /// </summary>
        /// <param name="k">The number of eigenvalues to compute.</param>
        /// <param name="job">The part of the spectrum to compute.</param>
        /// <returns>Retruns an <see cref="IEigenSolverResult"/>.</returns>
        IEigenSolverResult SolveGeneralized(int k, Spectrum job);

        /// <summary>
        /// Solve the generalized eigenvalue problem in shift-invert mode.
        /// </summary>
        /// <param name="k">The number of eigenvalues to compute.</param>
        /// <param name="sigma">The shift value.</param>
        /// <param name="job">The part of the spectrum to compute.</param>
        /// <returns>Retruns an <see cref="IEigenSolverResult"/>.</returns>
        IEigenSolverResult SolveGeneralized(int k, T sigma, Spectrum job);
    }
}
