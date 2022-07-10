namespace CSparse.Interop.SuiteSparse.Cholmod
{
    using System;

#if X64
    using size_t = System.UInt64;
#else
    using size_t = System.UInt32;
#endif

    /// <summary>
    /// A sparse matrix stored in compressed-column form.
    /// </summary>
    public struct CholmodSparse
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

        /* pointers to int or SuiteSparse_long: */
        /// <summary>
        /// p [0..ncol], the column pointers
        /// </summary>
        public IntPtr p;
        /// <summary>
        /// i [0..nzmax-1], the row indices
        /// </summary>
        public IntPtr i;

        /* for unpacked matrices only: */
        /// <summary>
        /// nz [0..ncol-1], the # of nonzeros in each col.
        /// </summary>
        public IntPtr nz;

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
        /// interger type
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
        /// <summary>
        /// TRUE if columns are sorted, FALSE otherwise
        /// </summary>
        public int sorted;
        /// <summary>
        /// TRUE if packed (nz ignored), FALSE if unpacked (nz is required)
        /// </summary>
        public int packed;
    }
}
