
namespace BriefFiniteElementNet.CSparse
{
    /// <summary>
    /// Column ordering for AMD.
    /// </summary>
    public enum ColumnOrdering
    {
        /// <summary>
        /// Natural ordering.
        /// </summary>
        Natural = 0,
        /// <summary>
        /// Minimum degree ordering of (A'+A).
        /// </summary>
        MinimumDegreeAtPlusA = 1,
        /// <summary>
        /// Minimum degree ordering of (A'*A) with removal of dense rows (not suited for Cholesky).
        /// </summary>
        MinimumDegreeStS = 2,
        /// <summary>
        /// Minimum degree ordering of (A'*A) (not suited for Cholesky).
        /// </summary>
        MinimumDegreeAtA = 3
    }
}
