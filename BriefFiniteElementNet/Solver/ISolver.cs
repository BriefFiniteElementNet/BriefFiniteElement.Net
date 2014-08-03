
using BriefFiniteElementNet.CSparse.Storage;

namespace BriefFiniteElementNet.Solver
{
    using System;

    /// <summary>
    /// Iterative solver interface.
    /// </summary>
    public interface ISolver<T>
        where T : struct, IEquatable<T>, IFormattable
    {
        /// <summary>
        /// Gets a value indicating whether the solver is initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Gets a value indicating whether the solver is direct or iterative.
        /// </summary>
        bool IsDirect { get; }

        /// <summary>
        /// Initializes the solver.
        /// </summary>
        /// <param name="matrix">The system matrix.</param>
        void Initialize(CompressedColumnStorage<T> matrix);

        /// <summary>
        /// Solves a system of linear equations iteratively.
        /// </summary>
        /// <param name="A">Linear operator providing a matrix-vector product implementation.</param>
        /// <param name="input">Right hand side</param>
        /// <param name="result">Solution</param>
        /// <returns>Iterative solver result.</returns>
        SolverResult Solve(CompressedColumnStorage<T> A, T[] input, T[] result);
    }
}
