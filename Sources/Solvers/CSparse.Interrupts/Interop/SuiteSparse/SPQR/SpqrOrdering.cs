
namespace CSparse.Interop.SuiteSparse.SPQR
{
    /// <summary>
    /// Ordering method to use.
    /// </summary>
    public enum SpqrOrdering : int
    {
        /// <summary>
        /// Fixed ordering.
        /// </summary>
        Fixed = Constants.SPQR_ORDERING_FIXED,
        /// <summary>
        /// Use natural ordering.
        /// </summary>
        Natural = Constants.SPQR_ORDERING_NATURAL,
        /// <summary>
        /// Use COLAMD.
        /// </summary>
        COLAMD = Constants.SPQR_ORDERING_COLAMD,
        /// <summary>
        /// Use given permutation.
        /// </summary>
        Given = Constants.SPQR_ORDERING_GIVEN,
        /// <summary>
        /// Use CHOLMOD best-effort (COLAMD, METIS, ...).
        /// </summary>
        CHOLMOD = Constants.SPQR_ORDERING_CHOLMOD,
        /// <summary>
        /// Use minimum degree AMD(A'*A).
        /// </summary>
        AMD = Constants.SPQR_ORDERING_AMD,
        /// <summary>
        /// Use METIS(A'*A) nested dissection.
        /// </summary>
        METIS = Constants.SPQR_ORDERING_METIS,
        /// <summary>
        /// Use SuiteSparseQR default ordering.
        /// </summary>
        Default = Constants.SPQR_ORDERING_DEFAULT,
        /// <summary>
        /// Try COLAMD/AMD/METIS and use best.
        /// </summary>
        Best = Constants.SPQR_ORDERING_BEST,
        /// <summary>
        /// Try COLAMD/AMD and use best.
        /// </summary>
        BestAMD = Constants.SPQR_ORDERING_BESTAMD
    }
}
