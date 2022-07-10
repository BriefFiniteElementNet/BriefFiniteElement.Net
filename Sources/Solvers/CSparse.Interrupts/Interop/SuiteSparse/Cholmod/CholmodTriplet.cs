namespace CSparse.Interop.SuiteSparse.Cholmod
{
    using System;

#if X64
    using size_t = System.UInt64;
#else
    using size_t = System.UInt32;
#endif

    /// <summary>
    /// A sparse matrix stored in triplet form.
    /// </summary>
    /// <remarks>
    ///  stype: Describes what parts of the matrix are considered:
    /// 
    /// 0:  matrix is "unsymmetric": use both upper and lower triangular parts
    ///     (the matrix may actually be symmetric in pattern and value, but
    ///     both parts are explicitly stored and used).  May be square or
    ///     rectangular.
    /// >0: matrix is square and symmetric.  Entries in the lower triangular
    ///     part are transposed and added to the upper triangular part when
    ///     the matrix is converted to cholmod_sparse form.
    /// &lt;0: matrix is square and symmetric.  Entries in the upper triangular
    ///     part are transposed and added to the lower triangular part when
    ///     the matrix is converted to cholmod_sparse form.
    /// 
    /// Note that stype>0 and stype&lt;0 are different for cholmod_sparse and
    /// cholmod_triplet.  The reason is simple.  You can permute a symmetric
    /// triplet matrix by simply replacing a row and column index with their
    /// new row and column indices, via an inverse permutation.  Suppose
    /// P = L->Perm is your permutation, and Pinv is an array of size n.
    /// Suppose a symmetric matrix A is represent by a triplet matrix T, with
    /// entries only in the upper triangular part.  Then the following code:
    /// 
    /// 	Ti = T->i ;
    /// 	Tj = T->j ;
    /// 	for (k = 0 ; k &lt; n  ; k++) Pinv [P [k]] = k ;
    /// 	for (k = 0 ; k &lt; nz ; k++) Ti [k] = Pinv [Ti [k]] ;
    /// 	for (k = 0 ; k &lt; nz ; k++) Tj [k] = Pinv [Tj [k]] ;
    /// 
    /// creates the triplet form of C=P*A*P'.  However, if T initially
    /// contains just the upper triangular entries (T->stype = 1), after
    /// permutation it has entries in both the upper and lower triangular
    /// parts.  These entries should be transposed when constructing the
    /// cholmod_sparse form of A, which is what cholmod_triplet_to_sparse
    /// does.  Thus:
    /// 
    /// 	C = cholmod_triplet_to_sparse (T, 0, &Common);
    /// 
    /// will return the matrix C = P*A*P'.
    /// 
    /// Since the triplet matrix T is so simple to generate, it's quite easy
    /// to remove entries that you do not want, prior to converting T to the
    /// cholmod_sparse form.  So if you include these entries in T, CHOLMOD
    /// assumes that there must be a reason (such as the one above).  Thus,
    /// no entry in a triplet matrix is ever ignored.
    /// </remarks>
    public struct CholmodTriplet
    {
        /// <summary>
        /// the matrix is nrow-by-ncol
        /// </summary>
        public size_t nrow;
        /// <summary>
        /// the matrix is nrow-by-ncol
        /// </summary>
        public size_t ncol;
        /// <summary>
        /// maximum number of entries in the matrix
        /// </summary>
        public size_t nzmax;
        /// <summary>
        /// number of nonzeros in the matrix
        /// </summary>
        public size_t nnz;

        /* pointers to int or SuiteSparse_long: */
        /// <summary>
        /// i [0..nzmax-1], the row indices
        /// </summary>
        public IntPtr i;
        /// <summary>
        /// j [0..nzmax-1], the column indices
        /// </summary>
        public IntPtr j;

        /* pointers to double or float: */
        /// <summary>
        /// size nzmax or 2*nzmax, if present
        /// </summary>
        public IntPtr x;
        /// <summary>
        /// size nzmax, if present
        /// </summary>
        public IntPtr z;

        /// <summary>
        /// Describes what parts of the matrix are considered
        /// </summary>
        public Stype stype;

        /// <summary>
        /// CHOLMOD_LONG: i and j are SuiteSparse_long.  Otherwise int
        /// </summary>
        public int itype;

        /// <summary>
        /// pattern, real, complex, or zomplex
        /// </summary>
        public Xtype xtype;
        /// <summary>
        /// x and z are double or float
        /// </summary>
        public Dtype dtype;
    }
}
