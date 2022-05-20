
using CSparse.Storage;

namespace BriefFiniteElementNet.Solver
{
    using System;

    /// <summary>
    /// Preconditioner interface.
    /// </summary>
    public interface IPreconditioner<T>
        where T : struct, IEquatable<T>, IFormattable
    {
        /// <summary>
        /// Gets a value indicating whether the preconditioner is initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Initializes the preconditioner and loads the internal data structures.
        /// </summary>
        /// <param name="matrix">The matrix on which the preconditioner is based.</param>
        void Initialize(CompressedColumnStorage<T> matrix);

        /// <summary>
        /// Applies the preconditioner to the matrix equation <b>Mx = b</b>.
        /// </summary>
        /// <param name="input">The right hand side vector b.</param>
        /// <param name="result">The left hand side vector x.</param>
        void Solve(T[] input, T[] result);
    }
}
