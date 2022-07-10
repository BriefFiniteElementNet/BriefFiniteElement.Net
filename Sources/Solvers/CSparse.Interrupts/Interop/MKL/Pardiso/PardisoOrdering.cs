
namespace CSparse.Interop.MKL.Pardiso
{
    /// <summary>
    /// Column ordering method.
    /// </summary>
    public enum PardisoOrdering : int
    {
        /// <summary>
        /// Minimum degree ordering.
        /// </summary>
        MinimumDegree = 0,
        /// <summary>
        /// Nested dissection using METIS.
        /// </summary>
        NestedDissection = 2,
        /// <summary>
        /// Parallel (OpenMP) version of the nested dissection algorithm.
        /// </summary>
        ParallelNestedDissection = 3
    }
}
