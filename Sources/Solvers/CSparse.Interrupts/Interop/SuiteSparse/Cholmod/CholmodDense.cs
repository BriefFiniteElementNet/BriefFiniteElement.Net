namespace CSparse.Interop.SuiteSparse.Cholmod
{
    using System;

#if X64
    using size_t = System.UInt64;
#else
    using size_t = System.UInt32;
#endif

    /// <summary>
    /// A dense matrix in column-oriented form.
    /// </summary>
    /// <remarks>
    /// Entry in row i and column j is located in x [i+j*d]
    /// </remarks>
    public struct CholmodDense
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
        /// leading dimension (d >= nrow must hold)
        /// </summary>
        public size_t d;
        /// <summary>
        /// size nzmax or 2*nzmax, if present
        /// </summary>
        public IntPtr x;
        /// <summary>
        /// size nzmax, if present
        /// </summary>
        public IntPtr z;
        /// <summary>
        /// pattern, real, complex, or zomplex
        /// </summary>
        public Xtype xtype;
        /// <summary>
        /// x and z double or float
        /// </summary>
        public Dtype dtype;
    }
}
