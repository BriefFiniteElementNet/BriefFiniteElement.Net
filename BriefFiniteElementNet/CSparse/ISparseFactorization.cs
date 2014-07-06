
namespace CSparse
{
    using System;

    /// <summary>
    /// Interface for factorization methods.
    /// </summary>
    /// <typeparam name="T">Supported data types are <c>double</c> and <see cref="Complex"/>.</typeparam>
    public interface ISparseFactorization<T>
        where T : struct, IEquatable<T>, IFormattable
    {
        /// <summary>
        /// Gets the total number of non-zero entries (all factors).
        /// </summary>
        int NonZerosCount { get; }

        /// <summary>
        /// Solves a system of linear equations, Ax = b.
        /// </summary>
        /// <param name="input">Right hand side b</param>
        /// <param name="result">Solution vector x.</param>
        void Solve(T[] input, T[] result);

        // TODO: what about SolveTranspose()
    }
}
