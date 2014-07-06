
namespace CSparse
{
    /// <summary>
    /// Types of matrix norms.
    /// </summary>
    public enum MatrixNorm : byte
    {
        /// <summary>
        /// The 1-norm (maximum absolute column sum).
        /// </summary>
        OneNorm,

        /// <summary>
        /// The Frobenius norm.
        /// </summary>
        FrobeniusNorm,

        /// <summary>
        /// The infinity norm (maximum absolute row sum).
        /// </summary>
        InfinityNorm,

        /// <summary>
        /// The largest absolute value norm.
        /// </summary>
        LargestAbsoluteValue
    }
}
