namespace CSparse.Interop.SuperLU
{
    /// <summary>
    /// Column ordering method.
    /// </summary>
    public enum OrderingMethod
    {
        /// <summary>
        /// Natural ordering.
        /// </summary>
        Natural,
        /// <summary>
        /// Minimum degree ordering on the structure of A^TA.
        /// </summary>
        MinimumDegreeAtA,
        /// <summary>
        /// Minimum degree ordering on the structure of A^T+A.
        /// </summary>
        MinimumDegreeAtPlusA,
        /// <summary>
        /// Column approximate minimum degree ordering.
        /// </summary>
        ColumnApproximateMinimumDegree
        /*
        /// <summary>
        /// ???
        /// </summary>
        MetisAtPlusA,
        /// <summary>
        /// ???
        /// </summary>
        ParMetis,
        /// <summary>
        /// ???
        /// </summary>
        ZOLTAN,
        /// <summary>
        /// Custom permutation (stored in perm_c array).
        /// </summary>
        Custom
        //*/
    }
}
