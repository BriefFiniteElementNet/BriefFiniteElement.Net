
using CSparse.Storage;

namespace BriefFiniteElementNet.Solver
{
    using System;

    /// <summary>
    /// Generic preconditioner that acts like the identity matrix.
    /// </summary>
    public class IdentityPreconditioner<T> : IPreconditioner<T>
        where T : struct, IEquatable<T>, IFormattable
    {
        /// <inheritdoc />
        public bool IsInitialized
        {
            get { return true; }
        }

        /// <inheritdoc />
        public void Initialize(CompressedColumnStorage<T> matrix)
        {
        }

        /// <inheritdoc />
        public void Solve(T[] input, T[] result)
        {
            Array.Copy(input, result, input.Length);
        }
    }
}
