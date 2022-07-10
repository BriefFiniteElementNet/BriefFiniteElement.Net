
namespace CSparse.Interop.SuiteSparse.Cholmod
{
    using System;
    using System.Runtime.InteropServices;

    using cholmod_common = System.IntPtr;
    using cholmod_dense = System.IntPtr;
    using cholmod_factor = System.IntPtr;
    using cholmod_sparse = System.IntPtr;
    using cholmod_triplet = System.IntPtr;

#if X64
    using size_t = System.UInt64;
    using sp_long = System.Int64;
#else
    using size_t = System.UInt32;
    using sp_long = System.Int32;
#endif

    internal static class NativeMethods
    {
#if SUITESPARSE_AIO
        const string CHOLMOD_DLL = "libsuitesparse";
#else
        const string CHOLMOD_DLL = "libcholmod";
#endif

        /// <summary>
        /// Returns a pointer to a cholmod_common struct.
        /// </summary>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_init", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_common cholmod_init();

        #region Core

        /// <summary>
        /// first call to CHOLMOD
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_start", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_start(ref CholmodCommon c);

        /// <summary>
        /// last call to CHOLMOD
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_finish", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_finish(ref CholmodCommon c);

        /// <summary>
        /// restore default parameters
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_defaults", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_defaults(ref CholmodCommon c);
        
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr cholmod_free(size_t n, size_t size, IntPtr p, ref CholmodCommon c);

        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_free_sparse", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_free_sparse(ref IntPtr A, ref CholmodCommon c);
        
        /*

        /// <summary>
        /// return valid maximum rank for update/downdate
        /// </summary>
        /// <param name="n">A and L will have n rows</param>
        /// <param name="c"></param>
        /// <returns>returns validated value of Common->maxrank</returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_maxrank", CallingConvention = CallingConvention.Cdecl)]
        public static extern size_t cholmod_maxrank(size_t n, ref CholmodCommon c);

        /// <summary>
        /// allocate workspace in Common
        /// </summary>
        /// <param name="nrow">size: Common->Flag (nrow), Common->Head (nrow+1)</param>
        /// <param name="iworksize">size of Common->Iwork</param>
        /// <param name="xworksize">size of Common->Xwork</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_allocate_work", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_allocate_work(size_t nrow, size_t iworksize, size_t xworksize, ref CholmodCommon c);

        /// <summary>
        /// free workspace in Common
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_work", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_work(ref CholmodCommon c);

        /// <summary>
        /// clear Flag workspace in Common
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_clear_flag", CallingConvention = CallingConvention.Cdecl)]
        public static extern SuiteSparse_long cholmod_clear_flag(ref CholmodCommon c);

        /// <summary>
        /// compute sqrt (x*x + y*y) accurately
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_hypot", CallingConvention = CallingConvention.Cdecl)]
        public static extern double cholmod_hypot (
            double x, double y
        );

        /// <summary>
        /// complex division, c = a/b
        /// </summary>
        /// <param name="ar">real and imaginary parts of a</param>
        /// <param name="ai"></param>
        /// <param name="br">real and imaginary parts of b</param>
        /// <param name="bi"></param>
        /// <param name="cr">real and imaginary parts of c </param>
        /// <param name="ci"></param>
        /// <returns>return 1 if divide-by-zero, 0 otherise</returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_divcomplex", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_divcomplex (
            double ar, double ai,
            double br, double bi,
            out double cr, out double ci
        );

        /// <summary>
        /// allocate a sparse matrix
        /// </summary>
        /// <param name="nrow"># of rows of A</param>
        /// <param name="ncol"># of columns of A</param>
        /// <param name="nzmax">max # of nonzeros of A</param>
        /// <param name="sorted">TRUE if columns of A sorted, FALSE otherwise</param>
        /// <param name="packed">TRUE if A will be packed, FALSE otherwise</param>
        /// <param name="stype">stype of A</param>
        /// <param name="xtype">CHOLMOD_PATTERN, _REAL, _COMPLEX, or _ZOMPLEX</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_allocate_sparse", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_sparse cholmod_allocate_sparse (
            size_t nrow,
            size_t ncol,
            size_t nzmax,
            int sorted,
            int packed,
            int stype,
            int xtype,
            ref CholmodCommon c);

        /// <summary>
        /// free a sparse matrix
        /// </summary>
        /// <param name="A">matrix to deallocate, NULL on output</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_sparse", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_sparse(
            ref cholmod_sparse A,
            ref CholmodCommon c);

        /// <summary>
        /// change the size (# entries) of sparse matrix
        /// </summary>
        /// <param name="nznew">new # of entries in A</param>
        /// <param name="A">matrix to reallocate</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_reallocate_sparse", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_reallocate_sparse (
            size_t nznew,
            ref CholmodSparse A,
            ref CholmodCommon c);

        /// <summary>
        /// return number of nonzeros in a sparse matrix
        /// </summary>
        /// <param name="A"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_nnz", CallingConvention = CallingConvention.Cdecl)]
        public static extern SuiteSparse_long cholmod_nnz (
            ref CholmodSparse A,
            ref CholmodCommon c);

        /// <summary>
        /// sparse identity matrix
        /// </summary>
        /// <param name="nrow"># of rows of A</param>
        /// <param name="ncol"># of columns of A</param>
        /// <param name="xtype">CHOLMOD_PATTERN, _REAL, _COMPLEX, or _ZOMPLEX</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_speye", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_sparse cholmod_speye (
            size_t nrow,
            size_t ncol,
            int xtype,
            ref CholmodCommon c);

        /// <summary>
        /// sparse zero matrix
        /// </summary>
        /// <param name="nrow"># of rows of A</param>
        /// <param name="ncol"># of columns of A</param>
        /// <param name="nzmax">max # of nonzeros of A</param>
        /// <param name="xtype">CHOLMOD_PATTERN, _REAL, _COMPLEX, or _ZOMPLEX</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_spzeros", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_sparse cholmod_spzeros (
            size_t nrow,
            size_t ncol,
            size_t nzmax,
            int xtype,
            ref CholmodCommon c);

        //*/

        /// <summary>
        /// transpose a sparse matrix
        /// </summary>
        /// <param name="A">matrix to transpose</param>
        /// <param name="values">0: pattern, 1: array transpose, 2: conj. transpose</param>
        /// <param name="c"></param>
        /// <returns>Return A' or A.'  The "values" parameter is 0, 1, or 2 to denote the pattern
        /// transpose, the array transpose (A.'), and the complex conjugate transpose (A').</returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_transpose", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_sparse cholmod_transpose (
            ref CholmodSparse A,
            int values,
            ref CholmodCommon c);

        /// <summary>
        /// transpose an unsymmetric sparse matrix
        /// </summary>
        /// <param name="A">matrix to transpose</param>
        /// <param name="values">0: pattern, 1: array transpose, 2: conj. transpose</param>
        /// <param name="Perm">size nrow, if present (can be NULL)</param>
        /// <param name="fset">subset of 0:(A->ncol)-1</param>
        /// <param name="fsize">size of fset</param>
        /// <param name="F">F = A', A(:,f)', or A(p,f)'</param>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <remarks>
        /// Compute F = A', A (:,f)', or A (p,f)', where A is unsymmetric and F is
        /// already allocated.  See cholmod_transpose for a simpler routine.
        /// </remarks>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_transpose_unsym", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_transpose_unsym (
            ref CholmodSparse A,
            int values,
            /* (int*) */ IntPtr Perm,
            /* (int*) */ IntPtr fset,
            size_t fsize,
            ref CholmodSparse F,
            ref CholmodCommon c);

        /// <summary>
        /// transpose a symmetric sparse matrix
        /// </summary>
        /// <param name="A">matrix to transpose</param>
        /// <param name="values">0: pattern, 1: array transpose, 2: conj. transpose</param>
        /// <param name="Perm">size nrow, if present (can be NULL)</param>
        /// <param name="F">F = A' or A(p,p)'</param>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <remarks>
        /// Compute F = A' or A (p,p)', where A is symmetric and F is already allocated.
        /// See cholmod_transpose for a simpler routine.
        /// </remarks>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_transpose_sym", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_transpose_sym (
            ref CholmodSparse A,
            int values,
            /* (int*) */ IntPtr Perm,
            ref CholmodSparse F,
            ref CholmodCommon c);

        /*

        /// <summary>
        /// transpose a sparse matrix
        /// </summary>
        /// <param name="A">matrix to transpose</param>
        /// <param name="values">0: pattern, 1: array transpose, 2: conj. transpose</param>
        /// <param name="Perm">if non-NULL, F = A(p,f) or A(p,p)</param>
        /// <param name="fset">subset of 0:(A->ncol)-1</param>
        /// <param name="fsize">size of fset</param>
        /// <param name="c"></param>
        /// <returns>Return A' or A(p,p)' if A is symmetric.  Return A', A(:,f)', or A(p,f)' if A is unsymmetric.</returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_ptranspose", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_sparse cholmod_ptranspose (
            ref CholmodSparse A,
            int values,
            IntPtr Perm, // (int*)
            IntPtr fset, // (int*)
            size_t fsize,
            ref CholmodCommon c);

        /// <summary>
        /// sort row indices in each column of sparse matrix
        /// </summary>
        /// <param name="A">matrix to sort</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_sort", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_sort(
            ref CholmodSparse A,
            ref CholmodCommon c);

        /// <summary>
        /// C = tril (triu (A,k1), k2)
        /// </summary>
        /// <param name="A">matrix to extract band matrix from</param>
        /// <param name="k1">ignore entries below the k1-st diagonal</param>
        /// <param name="k2">ignore entries above the k2-nd diagonal</param>
        /// <param name="mode">>0: numerical, 0: pattern, &lt;0: pattern (no diag)</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_band", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_sparse cholmod_band (
            ref CholmodSparse A,
            SuiteSparse_long k1,
            SuiteSparse_long k2,
            int mode,
            ref CholmodCommon c);

        /// <summary>
        /// A = tril (triu (A,k1), k2)
        /// </summary>
        /// <param name="k1">ignore entries below the k1-st diagonal</param>
        /// <param name="k2">ignore entries above the k2-nd diagonal</param>
        /// <param name="mode">>0: numerical, 0: pattern, &lt;0: pattern (no diag)</param>
        /// <param name="A">matrix from which entries not in band are removed</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_band_inplace", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_band_inplace (
            SuiteSparse_long k1,
            SuiteSparse_long k2,
            int mode,
            ref CholmodSparse A,
            ref CholmodCommon c);

        /// <summary>
        /// C = A*A' or A(:,f)*A(:,f)'
        /// </summary>
        /// <param name="A">input matrix; C=A*A' is constructed</param>
        /// <param name="fset">subset of 0:(A->ncol)-1</param>
        /// <param name="fsize">size of fset</param>
        /// <param name="mode">>0: numerical, 0: pattern, &lt;0: pattern (no diag),
        ///  -2: pattern only, no diagonal, add 50%+n extra space to C</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_aat", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_sparse cholmod_aat (
            ref CholmodSparse A,
            IntPtr fset, // (int*)
            size_t fsize,
            int mode,
            ref CholmodCommon c);

        /// <summary>
        /// C = A, create an exact copy of a sparse matrix
        /// </summary>
        /// <param name="A">matrix to copy</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_copy_sparse", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_sparse cholmod_copy_sparse (
            ref CholmodSparse A,
            ref CholmodCommon c);

        /// <summary>
        /// C = A, with possible change of stype
        /// </summary>
        /// <param name="A">matrix to copy</param>
        /// <param name="stype">requested stype of C</param>
        /// <param name="mode">>0: numerical, 0: pattern, &lt;0: pattern (no diag)</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_copy", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_sparse cholmod_copy (
            ref CholmodSparse A,
            int stype,
            int mode,
            ref CholmodCommon c);

        //*/

        /// <summary>
        /// C = alpha*A + beta*B
        /// </summary>
        /// <param name="A">matrix to add</param>
        /// <param name="B">matrix to add</param>
        /// <param name="alpha">scale factor for A</param>
        /// <param name="beta">scale factor for B</param>
        /// <param name="values">if TRUE compute the numerical values of C</param>
        /// <param name="sorted">if TRUE, sort columns of C</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_add", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_sparse cholmod_add (
            ref CholmodSparse A,
            ref CholmodSparse B,
            double[] alpha,	    /*   [2] */
            double[] beta,	    /*   [2] */
            int values,
            int sorted,
            ref CholmodCommon c);

        /*

        /// <summary>
        /// change the xtype of a sparse matrix
        /// </summary>
        /// <param name="to_xtype">requested xtype (pattern, real, complex, zomplex)</param>
        /// <param name="A">sparse matrix to change</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_sparse_xtype", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_sparse_xtype (
            int to_xtype,
            ref CholmodSparse A,
            ref CholmodCommon c);

        //*/

        // === Core/cholmod_factor =================================================

        /*

        /// <summary>
        /// allocate a factor (symbolic LL' or LDL')
        /// </summary>
        /// <param name="n">L is n-by-n</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_allocate_factor", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_factor cholmod_allocate_factor (
            size_t n,
            ref CholmodCommon c);

        //*/

        
        /// <summary>
        /// free a factor
        /// </summary>
        /// <param name="L">factor to free, NULL on output</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_free_factor", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_free_factor(
            ref cholmod_factor L,
            ref CholmodCommon c);

        /*

        /// <summary>
        /// change the # entries in a factor
        /// </summary>
        /// <param name="nznew">new # of entries in L</param>
        /// <param name="L">factor to modify</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_reallocate_factor", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_reallocate_factor (
            size_t nznew,
            ref CholmodFactor L,
            ref CholmodCommon c);

        /// <summary>
        /// change the type of factor (e.g., LDL' to LL')
        /// </summary>
        /// <param name="to_xtype">to CHOLMOD_PATTERN, _REAL, _COMPLEX, _ZOMPLEX</param>
        /// <param name="to_ll">TRUE: convert to LL', FALSE: LDL'</param>
        /// <param name="to_super">TRUE: convert to supernodal, FALSE: simplicial</param>
        /// <param name="to_packed">TRUE: pack simplicial columns, FALSE: do not pack</param>
        /// <param name="to_monotonic">TRUE: put simplicial columns in order, FALSE: not</param>
        /// <param name="L">factor to modify</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_change_factor", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_change_factor (
            int to_xtype,
            int to_ll,
            int to_super,
            int to_packed,
            int to_monotonic,
            ref CholmodFactor L,
            ref CholmodCommon c);

        /// <summary>
        /// pack the columns of a factor
        /// </summary>
        /// <param name="L">factor to modify</param>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <remarks>
        /// Pack the columns of a simplicial factor.  Unlike cholmod_change_factor,
        /// it can pack the columns of a factor even if they are not stored in their
        /// natural order (non-monotonic).
        /// </remarks>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_pack_factor", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_pack_factor(
            ref CholmodFactor L,
            ref CholmodCommon c);

        /// <summary>
        /// resize a single column of a factor
        /// </summary>
        /// <param name="j">the column to reallocate</param>
        /// <param name="need">required size of column j</param>
        /// <param name="L">factor to modify</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_reallocate_column", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_reallocate_column (
            size_t j,
            size_t need,
            ref CholmodFactor L,
            ref CholmodCommon c);

        /// <summary>
        /// create a sparse matrix copy of a factor
        /// </summary>
        /// <param name="L">factor to copy, converted to symbolic on output</param>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <remarks>
        /// Only operates on numeric factors, not symbolic ones
        /// </remarks>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_factor_to_sparse", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_sparse cholmod_factor_to_sparse(
            ref CholmodFactor L,
            ref CholmodCommon c);

        /// <summary>
        /// create a copy of a factor
        /// </summary>
        /// <param name="L">factor to copy</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_copy_factor", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_factor cholmod_copy_factor (
            ref CholmodFactor L,
            ref CholmodCommon c);

        /// <summary>
        /// change the xtype of a factor
        /// </summary>
        /// <param name="to_xtype">requested xtype (real, complex, or zomplex)</param>
        /// <param name="L">factor to change</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_factor_xtype", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_factor_xtype (
            int to_xtype,
            ref CholmodFactor L,
            ref CholmodCommon c);

        //*/

        // === Core/cholmod_dense ===================================================

        /*

        /// <summary>
        /// allocate a dense matrix (contents uninitialized)
        /// </summary>
        /// <param name="nrow"># of rows of matrix</param>
        /// <param name="ncol"># of columns of matrix</param>
        /// <param name="d">leading dimension</param>
        /// <param name="xtype">CHOLMOD_REAL, _COMPLEX, or _ZOMPLEX</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_allocate_dense", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_dense cholmod_allocate_dense (
            size_t nrow,
            size_t ncol,
            size_t d,
            int xtype,
            ref CholmodCommon c);

        /// <summary>
        /// allocate a dense matrix and set it to zero
        /// </summary>
        /// <param name="nrow"># of rows of matrix</param>
        /// <param name="ncol"># of columns of matrix</param>
        /// <param name="xtype">CHOLMOD_REAL, _COMPLEX, or _ZOMPLEX</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_zeros", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_dense cholmod_zeros (
            size_t nrow,
            size_t ncol,
            int xtype,
            ref CholmodCommon c);

        /// <summary>
        /// allocate a dense matrix and set it to all ones
        /// </summary>
        /// <param name="nrow"># of rows of matrix</param>
        /// <param name="ncol"># of columns of matrix</param>
        /// <param name="xtype">CHOLMOD_REAL, _COMPLEX, or _ZOMPLEX</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_ones", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_dense cholmod_ones (
            size_t nrow,
            size_t ncol,
            int xtype,
            ref CholmodCommon c);

        /// <summary>
        /// allocate a dense matrix and set it to the identity matrix
        /// </summary>
        /// <param name="nrow"># of rows of matrix</param>
        /// <param name="ncol"># of columns of matrix</param>
        /// <param name="xtype">CHOLMOD_REAL, _COMPLEX, or _ZOMPLEX</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_eye", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_dense cholmod_eye (
            size_t nrow,
            size_t ncol,
            int xtype,
            ref CholmodCommon c);

        //*/

        /// <summary>
        /// free a dense matrix
        /// </summary>
        /// <param name="X">dense matrix to deallocate, NULL on output</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_free_dense", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_free_dense(
            ref cholmod_dense X,
            ref CholmodCommon c);

        /*

        /// <summary>
        /// ensure a dense matrix has a given size and type
        /// </summary>
        /// <param name="XHandle">matrix handle to check</param>
        /// <param name="nrow"># of rows of matrix</param>
        /// <param name="ncol"># of columns of matrix</param>
        /// <param name="d">leading dimension</param>
        /// <param name="xtype">CHOLMOD_REAL, _COMPLEX, or _ZOMPLEX</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_ensure_dense", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_dense cholmod_ensure_dense(
            ref cholmod_dense XHandle,
            size_t nrow,
            size_t ncol,
            size_t d,
            int xtype,
            ref CholmodCommon c);

        /// <summary>
        /// create a dense matrix copy of a sparse matrix
        /// </summary>
        /// <param name="A">matrix to copy</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_sparse_to_dense", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_dense cholmod_sparse_to_dense (
            ref CholmodSparse A,
            ref CholmodCommon c);

        /// <summary>
        /// create a sparse matrix copy of a dense matrix
        /// </summary>
        /// <param name="X">matrix to copy</param>
        /// <param name="values">TRUE if values to be copied, FALSE otherwise</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_dense_to_sparse", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_sparse cholmod_dense_to_sparse (
            ref CholmodDense X,
            int values,
            ref CholmodCommon c);

        /// <summary>
        /// create a copy of a dense matrix
        /// </summary>
        /// <param name="X">matrix to copy</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_copy_dense", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_dense cholmod_copy_dense (
            ref CholmodDense X,
            ref CholmodCommon c);

        /// <summary>
        /// copy a dense matrix (pre-allocated)
        /// </summary>
        /// <param name="X">matrix to copy</param>
        /// <param name="Y">copy of matrix X</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_copy_dense2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_copy_dense2 (
            ref CholmodDense X,
            ref CholmodDense Y,
            ref CholmodCommon c);

        /// <summary>
        /// change the xtype of a dense matrix
        /// </summary>
        /// <param name="to_xtype">requested xtype (real, complex,or zomplex)</param>
        /// <param name="X">dense matrix to change</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_dense_xtype", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_dense_xtype (
            int to_xtype,
            ref CholmodDense X,
            ref CholmodCommon c);

        //*/

        // === Core/cholmod_triplet =================================================

        /*

        /// <summary>
        /// allocate a triplet matrix
        /// </summary>
        /// <param name="nrow"># of rows of T</param>
        /// <param name="ncol"># of columns of T</param>
        /// <param name="nzmax">max # of nonzeros of T</param>
        /// <param name="stype">stype of T</param>
        /// <param name="xtype">CHOLMOD_PATTERN, _REAL, _COMPLEX, or _ZOMPLEX</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_allocate_triplet", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_triplet cholmod_allocate_triplet (
            size_t nrow,
            size_t ncol,
            size_t nzmax,
            int stype,
            int xtype,
            ref CholmodCommon c);

        /// <summary>
        /// free a triplet matrix
        /// </summary>
        /// <param name="T">triplet matrix to deallocate, NULL on output</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_free_triplet", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_free_triplet(
            ref cholmod_triplet T,
            ref CholmodCommon c);

        /// <summary>
        /// change the # of entries in a triplet matrix
        /// </summary>
        /// <param name="nznew">new # of entries in T</param>
        /// <param name="T">triplet matrix to modify</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_reallocate_triplet", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_reallocate_triplet (
            size_t nznew,
            ref CholmodTriplet T,
            ref CholmodCommon c);

        /// <summary>
        /// create a triplet matrix copy of a sparse matrix
        /// </summary>
        /// <param name="A">matrix to copy</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_sparse_to_triplet", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_triplet cholmod_sparse_to_triplet (
            ref CholmodSparse A,
            ref CholmodCommon c);

        /// <summary>
        /// create a sparse matrix copy of a triplet matrix
        /// </summary>
        /// <param name="T">matrix to copy</param>
        /// <param name="nzmax">allocate at least this much space in output matrix</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_triplet_to_sparse", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_sparse cholmod_triplet_to_sparse (
            ref CholmodTriplet T,
            size_t nzmax,
            ref CholmodCommon c);

        /// <summary>
        /// create a copy of a triplet matrix
        /// </summary>
        /// <param name="T">matrix to copy</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_copy_triplet", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_triplet cholmod_copy_triplet (
            ref CholmodTriplet T,
            ref CholmodCommon c);

        /// <summary>
        /// change the xtype of a triplet matrix 
        /// </summary>
        /// <param name="to_xtype">requested xtype (pattern, real, complex,or zomplex)</param>
        /// <param name="T">triplet matrix to change</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_triplet_xtype", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_triplet_xtype (
            int to_xtype,
            ref CholmodTriplet T,
            ref CholmodCommon c);

        //*/

        // === Core/cholmod_version =================================================

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version">output, contents not defined on input.  Not used if NULL.</param>
        /// <returns>returns CHOLMOD_VERSION</returns>
        /// <remarks>
        ///    version [0] = CHOLMOD_MAIN_VERSION
        ///    version [1] = CHOLMOD_SUB_VERSION
        ///    version [2] = CHOLMOD_SUBSUB_VERSION
        /// </remarks>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_version", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_version([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] int[] version);

        #endregion

        #region CAMD

        /*

        /// <summary>
        /// Order AA' or A(:,f)*A(:,f)' using CCOLAMD.
        /// </summary>
        /// <param name="A">matrix to order</param>
        /// <param name="fset">subset of 0:(A->ncol)-1</param>
        /// <param name="fsize">size of fset</param>
        /// <param name="Cmember">size A->nrow.  Cmember [i] = c if row i is in the
        /// constraint set c.  c must be >= 0.  The # of constraint sets is max (Cmember) + 1.  If Cmember is
        /// NULL, then it is interpretted as Cmember [i] = 0 for all i</param>
        /// <param name="Perm">size A->nrow, output permutation</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_ccolamd", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_ccolamd (
            ref CholmodSparse A,
            IntPtr fset, // (int*)
            size_t fsize,
            IntPtr Cmember, // (int*)
            IntPtr Perm, // (int*)
            ref CholmodCommon c);

        /// <summary>
        /// Order A using CSYMAMD.
        /// </summary>
        /// <param name="A">matrix to order</param>
        /// <param name="Cmember">size nrow.  see cholmod_ccolamd above</param>
        /// <param name="Perm">size A->nrow, output permutation</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_csymamd", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_csymamd (
            ref CholmodSparse A,
            IntPtr Cmember, // (int*)
            IntPtr Perm, // (int*)
            ref CholmodCommon c);

        /// <summary>
        /// Order A using CAMD.
        /// </summary>
        /// <param name="A">matrix to order</param>
        /// <param name="fset">subset of 0:(A->ncol)-1</param>
        /// <param name="fsize">size of fset</param>
        /// <param name="Cmember">size nrow.  see cholmod_ccolamd above</param>
        /// <param name="Perm">size A->nrow, output permutation</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_camd", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_camd (
            ref CholmodSparse A,
            IntPtr fset, // (int*)
            size_t fsize,
            IntPtr Cmember, // (int*)
            IntPtr Perm, // (int*)
            ref CholmodCommon c);

        //*/

        #endregion

        #region Check

        /// <summary>
        /// check the Common object
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_check_common", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_check_common(ref CholmodCommon c);

        /// <summary>
        /// check a sparse matrix
        /// </summary>
        /// <param name="A">sparse matrix to check</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_check_sparse", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_check_sparse (
            ref CholmodSparse A,
            ref CholmodCommon c);

        /// <summary>
        /// check a dense matrix
        /// </summary>
        /// <param name="X">dense matrix to check</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_check_dense", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_check_dense (
            ref CholmodDense X,
            ref CholmodCommon c);

        /// <summary>
        /// check a factor
        /// </summary>
        /// <param name="L">factor to check</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_check_factor", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_check_factor (
            ref CholmodFactor L,
            ref CholmodCommon c);

        /// <summary>
        /// check a sparse matrix in triplet form
        /// </summary>
        /// <param name="T">triplet matrix to check</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_check_triplet", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_check_triplet (
            ref CholmodTriplet T,
            ref CholmodCommon c);

        /// <summary>
        /// check a subset
        /// </summary>
        /// <param name="Set">Set [0:len-1] is a subset of 0:n-1.  Duplicates OK</param>
        /// <param name="len">size of Set (an integer array)</param>
        /// <param name="n">0:n-1 is valid range</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_check_subset", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_check_subset (
            /* (int*) */ IntPtr Set,
            sp_long len,
            size_t n,
            ref CholmodCommon c);

        /// <summary>
        /// check a permutation
        /// </summary>
        /// <param name="Perm">Perm [0:len-1] is a permutation of subset of 0:n-1</param>
        /// <param name="len">size of Perm (an integer array)</param>
        /// <param name="n">0:n-1 is valid range</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_check_perm", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_check_perm (
            /* (int*) */ IntPtr Perm,
            size_t len,
            size_t n,
            ref CholmodCommon c);

        #endregion

        #region Cholesky
        
        /// <summary>
        /// order and analyze (simplicial or supernodal)
        /// </summary>
        /// <param name="A">matrix to order and analyze</param>
        /// <param name="c"></param>
        /// <returns>cholmod_factor</returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_analyze", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_factor cholmod_analyze (
            ref CholmodSparse A,
            ref CholmodCommon c);
        
        /// <summary>
        /// analyze, with user-provided permutation or f set
        /// </summary>
        /// <param name="A">matrix to order and analyze</param>
        /// <param name="UserPerm">user-provided permutation, size A->nrow</param>
        /// <param name="fset">subset of 0:(A->ncol)-1</param>
        /// <param name="fsize">size of fset</param>
        /// <param name="c"></param>
        /// <returns>cholmod_factor</returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_analyze_p", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_factor cholmod_analyze_p (
            ref CholmodSparse A,
            IntPtr UserPerm, // (int*)
            IntPtr fset, // (int*)
            size_t fsize,
            ref CholmodCommon c);

        /// <summary>
        /// analyze for sparse Cholesky or sparse QR
        /// </summary>
        /// <param name="for_whom">
        /// FOR_SPQR     (0): for SPQR but not GPU-accelerated
        /// FOR_CHOLESKY (1): for Cholesky (GPU or not)
        /// FOR_SPQRGPU  (2): for SPQR with GPU acceleration
        /// </param>
        /// <param name="A">matrix to order and analyze</param>
        /// <param name="UserPerm">user-provided permutation, size A->nrow</param>
        /// <param name="fset">subset of 0:(A->ncol)-1</param>
        /// <param name="fsize">size of fset</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_analyze_p2", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_factor cholmod_analyze_p2 (
            int for_whom,
            ref CholmodSparse A,
            IntPtr UserPerm, // (int*)
            IntPtr fset, // (int*)
            size_t fsize,
            ref CholmodCommon c);
        
        /// <summary>
        /// simplicial or supernodal Cholesky factorization
        /// </summary>
        /// <param name="A">matrix to factorize</param>
        /// <param name="L">resulting factorization</param>
        /// <param name="c"></param>
        /// <returns>returns TRUE on success, FALSE on failure</returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_factorize", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_factorize (
            ref CholmodSparse A,
            ref CholmodFactor L,
            ref CholmodCommon c);
        
        /// <summary>
        /// factorize, with user-provided permutation or fset
        /// </summary>
        /// <param name="A">matrix to factorize</param>
        /// <param name="beta">factorize beta*I+A or beta*I+A'*A  [size 2]</param>
        /// <param name="fset">subset of 0:(A->ncol)-1</param>
        /// <param name="fsize">size of fset</param>
        /// <param name="L">resulting factorization</param>
        /// <param name="c"></param>
        /// <returns>returns TRUE on success, FALSE on failure</returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_factorize_p", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_factorize_p (
            ref CholmodSparse A,
            double[] beta,
            IntPtr fset, // (int*)
            size_t fsize,
            ref CholmodFactor L,
            ref CholmodCommon c);
        
        /// <summary>
        /// solve a linear system (simplicial or supernodal)
        /// </summary>
        /// <param name="sys">system to solve</param>
        /// <param name="L">factorization to use</param>
        /// <param name="B">right-hand-side</param>
        /// <param name="c"></param>
        /// <returns>returns the solution X</returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_solve", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_dense cholmod_solve(
            int sys,
            ref CholmodFactor L,
            ref CholmodDense B,
            ref CholmodCommon c);

        /// <summary>
        /// like cholmod_solve, but with reusable workspace
        /// </summary>
        /// <param name="sys">system to solve</param>
        /// <param name="L">factorization to use</param>
        /// <param name="B">right-hand-side</param>
        /// <param name="Bset">solution, allocated if need be</param>
        /// <param name="X_Handle">cholmod_sparse</param>
        /// <param name="Xset_Handle">cholmod_sparse</param>
        /// <param name="Y_Handle">workspace, or NULL (cholmod_dense)</param>
        /// <param name="E_Handle">workspace, or NULL (cholmod_dense)</param>
        /// <param name="c"></param>
        /// <returns>returns TRUE on success, FALSE on failure</returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_solve2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_solve2 (
            int sys,
            ref CholmodFactor L,
            ref CholmodDense B,
            ref CholmodSparse Bset,
            ref IntPtr X_Handle,
            ref IntPtr Xset_Handle,
            ref IntPtr Y_Handle,
            ref IntPtr E_Handle,
            ref CholmodCommon c);

        /// <summary>
        /// solve a linear system with a sparse right-hand-side
        /// </summary>
        /// <param name="sys">system to solve</param>
        /// <param name="L">factorization to use</param>
        /// <param name="B">right-hand-side</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_spsolve", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_sparse cholmod_spsolve (
            int sys,
            ref CholmodFactor L,
            ref CholmodSparse B,
            ref CholmodCommon c);

        /*

        /// <summary>
        /// find the elimination tree of A or A'*A
        /// </summary>
        /// <param name="A"></param>
        /// <param name="Parent">size ncol.  Parent [j] = p if p is the parent of j</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_etree", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_etree (
            ref CholmodSparse A,
            IntPtr Parent, // (int*)
            ref CholmodCommon c);

        /// <summary>
        /// compute the row/column counts of L
        /// </summary>
        /// <param name="A">matrix to analyze</param>
        /// <param name="fset">subset of 0:(A->ncol)-1</param>
        /// <param name="fsize">size of fset</param>
        /// <param name="Parent">size nrow.  Parent [i] = p if p is the parent of i</param>
        /// <param name="Post">size nrow.  Post [k] = i if i is the kth node in the postordered etree.</param>
        /// <param name="RowCount">size nrow. RowCount [i] = # entries in the ith row of L, including the diagonal.</param>
        /// <param name="ColCount">size nrow. ColCount [i] = # entries in the ith column of L, including the diagonal.</param>
        /// <param name="First">size nrow.  First [i] = k is the least postordering of any descendant of i.</param>
        /// <param name="Level">size nrow.  Level [i] is the length of the path from i to the root, with Level [root] = 0.</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_rowcolcounts", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_rowcolcounts (
            ref CholmodSparse A,
            IntPtr fset, // (int*)
            size_t fsize,
            IntPtr Parent, // (int*)
            IntPtr Post, // (int*)
            IntPtr RowCount, // (int*)
            IntPtr ColCount, // (int*)
            IntPtr First, // (int*)
            IntPtr Level, // (int*)
            ref CholmodCommon c);

        /// <summary>
        /// analyze a fill-reducing ordering
        /// </summary>
        /// <param name="A">matrix to analyze</param>
        /// <param name="ordering">ordering method used</param>
        /// <param name="Perm">size n, fill-reducing permutation to analyze</param>
        /// <param name="fset">subset of 0:(A->ncol)-1</param>
        /// <param name="fsize">size of fset</param>
        /// <param name="Parent">size n, elimination tree</param>
        /// <param name="Post">size n, postordering of elimination tree</param>
        /// <param name="ColCount">size n, nnz in each column of L</param>
        /// <param name="First">size n workspace for cholmod_postorder</param>
        /// <param name="Level">size n workspace for cholmod_postorder</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_analyze_ordering", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_analyze_ordering (
            ref CholmodSparse A,
            int ordering,
            IntPtr Perm, // (int*)
            IntPtr fset, // (int*)
            size_t fsize,
            IntPtr Parent, // (int*)
            IntPtr Post, // (int*)
            IntPtr ColCount, // (int*)
            IntPtr First, // (int*)
            IntPtr Level, // (int*)
            ref CholmodCommon c);
        
        /// <summary>
        /// order using AMD
        /// </summary>
        /// <param name="A">matrix to order</param>
        /// <param name="fset">subset of 0:(A->ncol)-1</param>
        /// <param name="fsize">size of fset</param>
        /// <param name="Perm"size A->nrow, output permutation></param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_amd", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_amd (
            ref CholmodSparse A,
            IntPtr fset, // (int*)
            size_t fsize,
            IntPtr Perm, // (int*)
            ref CholmodCommon c);
        
        /// <summary>
        /// order using COLAMD
        /// </summary>
        /// <param name="A">matrix to order</param>
        /// <param name="fset">subset of 0:(A->ncol)-1</param>
        /// <param name="fsize">size of fset</param>
        /// <param name="postorder">if TRUE, follow with a coletree postorder</param>
        /// <param name="Perm">size A->nrow, output permutation</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_colamd", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_colamd (
            ref CholmodSparse A,
            IntPtr fset, // (int*)
            size_t fsize,
            int postorder,
            IntPtr Perm, // (int*)
            ref CholmodCommon c);

        /// <summary>
        /// incremental simplicial factorization
        /// </summary>
        /// <param name="A">matrix to factorize</param>
        /// <param name="F">used for A*A' case only. F=A' or A(:,fset)'</param>
        /// <param name="beta">factorize beta*I+A or beta*I+A'*A</param>
        /// <param name="kstart">first row to factorize</param>
        /// <param name="kend">last row to factorize is kend-1</param>
        /// <param name="L"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_rowfac", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_rowfac (
            ref CholmodSparse A,
            ref CholmodSparse F,
            double[] beta,	// const size [2]
            size_t kstart,
            size_t kend,
            ref CholmodFactor L,
            ref CholmodCommon c);
        
        /// <summary>
        /// incremental simplicial factorization
        /// </summary>
        /// <param name="A">matrix to factorize</param>
        /// <param name="F">used for A*A' case only. F=A' or A(:,fset)'</param>
        /// <param name="beta">factorize beta*I+A or beta*I+A'*A</param>
        /// <param name="kstart">first row to factorize</param>
        /// <param name="kend">last row to factorize is kend-1</param>
        /// <param name="mask">if mask[i] >= 0, then set row i to zero</param>
        /// <param name="RLinkUp">link list of rows to compute</param>
        /// <param name="L"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_rowfac_mask", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_rowfac_mask (
            ref CholmodSparse A,
            ref CholmodSparse F,
            double[] beta, // const size [2]
            size_t kstart,
            size_t kend,
            IntPtr mask, // (int*)
            IntPtr RLinkUp, // (int*)
            ref CholmodFactor L,
            ref CholmodCommon c);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A">matrix to factorize</param>
        /// <param name="F">used for A*A' case only. F=A' or A(:,fset)'</param>
        /// <param name="beta">factorize beta*I+A or beta*I+A'*A</param>
        /// <param name="kstart">first row to factorize</param>
        /// <param name="kend">last row to factorize is kend-1</param>
        /// <param name="mask">if mask[i] >= maskmark, then set row i to zero</param>
        /// <param name="maskmark"></param>
        /// <param name="RLinkUp">link list of rows to compute</param>
        /// <param name="L"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_rowfac_mask2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_rowfac_mask2 (
            ref CholmodSparse A,
            ref CholmodSparse F,
            double[] beta, // const size [2]
            size_t kstart,
            size_t kend,
            IntPtr mask, // (int*)
            int maskmark,
            IntPtr RLinkUp, // (int*)
            ref CholmodFactor L,
            ref CholmodCommon c);
        
        /// <summary>
        /// Find the nonzero pattern of x for the system Lx=b where L = (0:k-1,0:k-1)
        /// and b = kth column of A or A*A' (rows 0 to k-1 only)
        /// </summary>
        /// <param name="A">matrix to analyze</param>
        /// <param name="F">used for A*A' case only. F=A' or A(:,fset)'</param>
        /// <param name="k">row k of L</param>
        /// <param name="Parent">elimination tree</param>
        /// <param name="R">pattern of L(k,:), n-by-1 with R->nzmax >= n</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_row_subtree", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_row_subtree (
            ref CholmodSparse A,
            ref CholmodSparse F,
            size_t k,
            IntPtr Parent, // (int*)
            ref CholmodSparse R,
            ref CholmodCommon c);

        /// <summary>
        /// find the nonzero pattern of x=L\b
        /// </summary>
        /// <param name="B">sparse right-hand-side (a single sparse column)</param>
        /// <param name="L">the factor L from which parent(i) is derived</param>
        /// <param name="X">pattern of X=L\B, n-by-1 with X->nzmax >= n</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_lsolve_pattern", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_lsolve_pattern (
            ref CholmodSparse B,
            ref CholmodFactor L,
            ref CholmodSparse X,
            ref CholmodCommon c);

        /// <summary>
        /// Identical to cholmod_row_subtree, except that it finds the elimination tree from L itself.
        /// </summary>
        /// <param name="A">matrix to analyze</param>
        /// <param name="Fi">nonzero pattern of kth row of A', not required for the symmetric case.  Need not be sorted.</param>
        /// <param name="fnz"></param>
        /// <param name="k">row k of L</param>
        /// <param name="L">the factor L from which parent(i) is derived</param>
        /// <param name="R">pattern of L(k,:), n-by-1 with R->nzmax >= n</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_row_lsubtree", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_row_lsubtree (
            ref CholmodSparse A,
            IntPtr Fi, // (int*)
            size_t fnz,
            size_t k,
            ref CholmodFactor L,
            ref CholmodSparse R,
            ref CholmodCommon c);

        /// <summary>
        /// Remove entries from L that are not in the factorization of P*A*P', P*A*A'*P',
        /// or P*F*F'*P' (depending on A->stype and whether fset is NULL or not).
        /// </summary>
        /// <param name="A">matrix to analyze</param>
        /// <param name="fset">subset of 0:(A->ncol)-1</param>
        /// <param name="fsize">size of fset</param>
        /// <param name="pack">if TRUE, pack the columns of L</param>
        /// <param name="L">factorization, entries pruned on output</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_resymbol", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_resymbol (
            ref CholmodSparse A,
            IntPtr fset, // (int*)
            size_t fsize,
            int pack,
            ref CholmodFactor L,
            ref CholmodCommon c);

        /// <summary>
        /// Remove entries from L that are not in the factorization of A, A*A',
        /// or F*F' (depending on A->stype and whether fset is NULL or not).
        /// </summary>
        /// <param name="A">matrix to analyze</param>
        /// <param name="fset">subset of 0:(A->ncol)-1</param>
        /// <param name="fsize">size of fset</param>
        /// <param name="pack">if TRUE, pack the columns of L</param>
        /// <param name="L">factorization, entries pruned on output</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_resymbol_noperm", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_resymbol_noperm (
            ref CholmodSparse A,
            IntPtr fset, // (int*)
            size_t fsize,
            int pack,
            ref CholmodFactor L,
            ref CholmodCommon c);

        /// <summary>
        /// compute rough estimate of reciprocal of condition number
        /// </summary>
        /// <param name="L"></param>
        /// <param name="c"></param>
        /// <returns>return min(diag(L)) / max(diag(L))</returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_rcond", CallingConvention = CallingConvention.Cdecl)]
        public static extern double cholmod_rcond (
            ref CholmodFactor L,
            ref CholmodCommon c);

        /// <summary>
        /// Compute the postorder of a tree
        /// </summary>
        /// <param name="Parent">size n. Parent [j] = p if p is the parent of j</param>
        /// <param name="n"></param>
        /// <param name="Weight_p">size n, optional. Weight [j] is weight of node j</param>
        /// <param name="Post">size n. Post [k] = j is kth in postordered tree</param>
        /// <param name="c"></param>
        /// <returns>return # of nodes postordered</returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_postorder", CallingConvention = CallingConvention.Cdecl)]
        public static extern SuiteSparse_long cholmod_postorder (
            IntPtr Parent, // (int*)
            size_t n,
            IntPtr Weight_p, // (int*)
            IntPtr Post, // (int*)
            ref CholmodCommon c);

        //*/

        #endregion

        #region MatrixOp

        /*

        /// <summary>
        /// drop entries with small absolute value
        /// </summary>
        /// <param name="tol">keep entries with absolute value > tol</param>
        /// <param name="A">matrix to drop entries from</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_drop", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_drop (
            double tol,
            ref CholmodSparse A,
            ref CholmodCommon c);

        /// <summary>
        /// s = norm (X), 1-norm, inf-norm, or 2-norm
        /// </summary>
        /// <param name="X">matrix to compute the norm of</param>
        /// <param name="norm">type of norm: 0: inf. norm, 1: 1-norm, 2: 2-norm</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_norm_dense", CallingConvention = CallingConvention.Cdecl)]
        public static extern double cholmod_norm_dense (
            ref CholmodDense X,
            int norm,
            ref CholmodCommon c);

        /// <summary>
        /// s = norm (A), 1-norm or inf-norm
        /// </summary>
        /// <param name="A">matrix to compute the norm of</param>
        /// <param name="norm">type of norm: 0: inf. norm, 1: 1-norm</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_norm_sparse", CallingConvention = CallingConvention.Cdecl)]
        public static extern double cholmod_norm_sparse (
            ref CholmodSparse A,
            int norm,
            ref CholmodCommon c);

        /// <summary>
        /// C = [A,B]
        /// </summary>
        /// <param name="A">left matrix to concatenate</param>
        /// <param name="B">right matrix to concatenate</param>
        /// <param name="values">if TRUE compute the numerical values of C</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_horzcat", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_sparse cholmod_horzcat (
            ref CholmodSparse A,
            ref CholmodSparse B,
            int values,
            ref CholmodCommon c);

        /// <summary>
        /// A = diag(s)*A, A*diag(s), s*A or diag(s)*A*diag(s)
        /// </summary>
        /// <param name="S">scale factors (scalar or vector)</param>
        /// <param name="scale">type of scaling to compute</param>
        /// <param name="A">matrix to scale</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_scale", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_scale (
            ref CholmodDense S,
            int scale,
            ref CholmodSparse A,
            ref CholmodCommon c);

        /// <summary>
        /// Sparse matrix times dense matrix:  Y = alpha*(A*X) + beta*Y or alpha*(A'*X) + beta*Y
        /// </summary>
        /// <param name="A">sparse matrix to multiply</param>
        /// <param name="transpose">use A if 0, or A' otherwise</param>
        /// <param name="alpha">scale factor for A</param>
        /// <param name="beta">scale factor for Y</param>
        /// <param name="X">dense matrix to multiply</param>
        /// <param name="Y">resulting dense matrix</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_sdmult", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_sdmult (
            ref CholmodSparse A,
            int transpose,
            double[] alpha, // [2]
            double[] beta, // [2]
            ref CholmodDense X,
            ref CholmodDense Y,
            ref CholmodCommon c);

        /// <summary>
        /// Sparse matrix times sparse matrix:  C = A*B
        /// </summary>
        /// <param name="A">left matrix to multiply</param>
        /// <param name="B">right matrix to multiply</param>
        /// <param name="stype">requested stype of C</param>
        /// <param name="values">TRUE: do numerical values, FALSE: pattern only</param>
        /// <param name="sorted">if TRUE then return C with sorted columns</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_ssmult", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_sparse cholmod_ssmult (
            ref CholmodSparse A,
            ref CholmodSparse B,
            int stype,
            int values,
            int sorted,
            ref CholmodCommon c);

        /// <summary>
        /// C = A (r,c), where i and j are arbitrary vectors
        /// </summary>
        /// <param name="A">matrix to subreference</param>
        /// <param name="rset">set of row indices, duplicates OK</param>
        /// <param name="rsize">size of r; rsize &lt; 0 denotes ":"</param>
        /// <param name="cset">set of column indices, duplicates OK</param>
        /// <param name="csize">size of c; csize &lt; 0 denotes ":"</param>
        /// <param name="values">if TRUE compute the numerical values of C </param>
        /// <param name="sorted">if TRUE then return C with sorted columns</param>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <remarks>
        /// rsize &lt; 0 denotes ":" in MATLAB notation, or more precisely 0:(A->nrow)-1.
        /// In this case, r can be NULL.  An rsize of zero, or r = NULL and rsize >= 0,
        /// denotes "[ ]" in MATLAB notation (the empty set). Similar rules hold for csize.
        /// </remarks>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_submatrix", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_sparse cholmod_submatrix (
            ref CholmodSparse A,
            IntPtr rset, // (int*)
            SuiteSparse_long rsize,
            IntPtr cset, // (int*)
            SuiteSparse_long csize,
            int values,
            int sorted,
            ref CholmodCommon c);

        /// <summary>
        /// C = [A ; B]
        /// </summary>
        /// <param name="A">left matrix to concatenate</param>
        /// <param name="B">right matrix to concatenate</param>
        /// <param name="values">if TRUE compute the numerical values of C</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_vertcat", CallingConvention = CallingConvention.Cdecl)]
        public static extern cholmod_sparse cholmod_vertcat (
            ref CholmodSparse A,
            ref CholmodSparse B,
            int values,
            ref CholmodCommon c);

        /// <summary>
        /// determine if a sparse matrix is symmetric
        /// </summary>
        /// <param name="A"></param>
        /// <param name="option"></param>
        /// <param name="xmatched">output</param>
        /// <param name="pmatched">output</param>
        /// <param name="nzoffdiag">output</param>
        /// <param name="nzdiag">output</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_symmetry", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_symmetry (
            ref CholmodSparse A,
            int option,
            out int xmatched, // (int*)
            out int pmatched, // (int*)
            out int nzoffdiag, // (int*)
            out int nzdiag, // (int*)
            ref CholmodCommon c);

        //*/

        #endregion

        #region Modify
            
        /// <summary>
        /// multiple rank update/downdate
        /// </summary>
        /// <param name="update">TRUE for update, FALSE for downdate</param>
        /// <param name="C">the incoming sparse update</param>
        /// <param name="L">factor to modify</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_updown", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_updown (
            int update,
            ref CholmodSparse C,
            ref CholmodFactor L,
            ref CholmodCommon c);
        
        /// <summary>
        /// update/downdate, and modify solution to Lx=b
        /// </summary>
        /// <param name="update">TRUE for update, FALSE for downdate</param>
        /// <param name="C">the incoming sparse update</param>
        /// <param name="L">factor to modify</param>
        /// <param name="X">solution to Lx=b (size n-by-1)</param>
        /// <param name="DeltaB">change in b, zero on output</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_updown_solve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_updown_solve (
            int update,
            ref CholmodSparse C,
            ref CholmodFactor L,
            ref CholmodDense X,
            ref CholmodDense DeltaB,
            ref CholmodCommon c);

        /*
        
        /// <summary>
        /// update/downdate, and modify solution to partial Lx=b
        /// </summary>
        /// <param name="update">TRUE for update, FALSE for downdate</param>
        /// <param name="C">the incoming sparse update</param>
        /// <param name="colmark">int array of size n.  See cholmod_updown.c</param>
        /// <param name="L">factor to modify</param>
        /// <param name="X">solution to Lx=b (size n-by-1)</param>
        /// <param name="DeltaB">change in b, zero on output</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_updown_mark", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_updown_mark (
            int update,
            ref CholmodSparse C,
            IntPtr colmark, // (int*)
            ref CholmodFactor L,
            ref CholmodDense X,
            ref CholmodDense DeltaB,
            ref CholmodCommon c);
        
        /// <summary>
        /// update/downdate, for LPDASA
        /// </summary>
        /// <param name="update">TRUE for update, FALSE for downdate</param>
        /// <param name="C">the incoming sparse update</param>
        /// <param name="colmark">int array of size n.  See cholmod_updown.c</param>
        /// <param name="mask">size n</param>
        /// <param name="L">factor to modify</param>
        /// <param name="X">solution to Lx=b (size n-by-1)</param>
        /// <param name="DeltaB">change in b, zero on output</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_updown_mask", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_updown_mask (
            int update,
            ref CholmodSparse C,
            IntPtr colmark, // (int*)
            IntPtr mask, // (int*)
            ref CholmodFactor L,
            ref CholmodDense X,
            ref CholmodDense DeltaB,
            ref CholmodCommon c);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="update">TRUE for update, FALSE for downdate</param>
        /// <param name="C">the incoming sparse update</param>
        /// <param name="colmark">int array of size n.  See cholmod_updown.c</param>
        /// <param name="mask">size n</param>
        /// <param name="maskmark"></param>
        /// <param name="L">factor to modify</param>
        /// <param name="X">solution to Lx=b (size n-by-1)</param>
        /// <param name="DeltaB">change in b, zero on output</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_updown_mask2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_updown_mask2 (
            int update,
            ref CholmodSparse C,
            IntPtr colmark, // (int*)
            IntPtr mask, // (int*)
            int maskmark,
            ref CholmodFactor L,
            ref CholmodDense X,
            ref CholmodDense DeltaB,
            ref CholmodCommon c);
        
        /// <summary>
        /// add a row to an LDL' factorization (a rank-2 update)
        /// </summary>
        /// <param name="k">row/column index to add</param>
        /// <param name="R">row/column of matrix to factorize (n-by-1)</param>
        /// <param name="L">factor to modify</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_rowadd", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_rowadd (
            size_t k,
            ref CholmodSparse R,
            ref CholmodFactor L,
            ref CholmodCommon c);
        
        /// <summary>
        /// add a row, and update solution to Lx=b
        /// </summary>
        /// <param name="k">row/column index to add</param>
        /// <param name="R">row/column of matrix to factorize (n-by-1)</param>
        /// <param name="bk">kth entry of the right-hand-side b</param>
        /// <param name="L">factor to modify</param>
        /// <param name="X">solution to Lx=b (size n-by-1)</param>
        /// <param name="DeltaB">change in b, zero on output</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_rowadd_solve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_rowadd_solve (
            size_t k,
            ref CholmodSparse R,
            double[] bk, // const size [2]
            ref CholmodFactor L,
            ref CholmodDense X,
            ref CholmodDense DeltaB,
            ref CholmodCommon c);
        
        /// <summary>
        /// add a row, and update solution to partial Lx=b
        /// </summary>
        /// <param name="k">row/column index to add</param>
        /// <param name="R">row/column of matrix to factorize (n-by-1)</param>
        /// <param name="bk">kth entry of the right hand side, b</param>
        /// <param name="colmark">int array of size n.  See cholmod_updown.c</param>
        /// <param name="L">factor to modify</param>
        /// <param name="X">solution to Lx=b (size n-by-1)</param>
        /// <param name="DeltaB">change in b, zero on output</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_rowadd_mark", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_rowadd_mark (
            size_t k,
            ref CholmodSparse R,
            double[] bk, // const size [2]
            IntPtr colmark, // (int*)
            ref CholmodFactor L,
            ref CholmodDense X,
            ref CholmodDense DeltaB,
            ref CholmodCommon c);
        
        /// <summary>
        /// delete a row from an LDL' factorization (a rank-2 update)
        /// </summary>
        /// <param name="k">row/column index to delete</param>
        /// <param name="R">NULL, or the nonzero pattern of kth row of L</param>
        /// <param name="L">factor to modify</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_rowdel", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_rowdel (
            size_t k,
            ref CholmodSparse R,
            ref CholmodFactor L,
            ref CholmodCommon c);
        
        /// <summary>
        /// delete a row, and downdate Lx=b
        /// </summary>
        /// <param name="k">row/column index to delete</param>
        /// <param name="R">NULL, or the nonzero pattern of kth row of L</param>
        /// <param name="yk">kth entry in the solution to A*y=b</param>
        /// <param name="L">factor to modify</param>
        /// <param name="X">solution to Lx=b (size n-by-1)</param>
        /// <param name="DeltaB">change in b, zero on output</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_rowdel_solve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_rowdel_solve (
            size_t k,
            ref CholmodSparse R,
            double[] yk, // const size [2]
            ref CholmodFactor L,
            ref CholmodDense X,
            ref CholmodDense DeltaB,
            ref CholmodCommon c);
        
        /// <summary>
        /// delete a row, and downdate solution to partial Lx=b
        /// </summary>
        /// <param name="k">row/column index to delete</param>
        /// <param name="R">NULL, or the nonzero pattern of kth row of L</param>
        /// <param name="yk">kth entry in the solution to A*y=b</param>
        /// <param name="colmark">int array of size n.  See cholmod_updown.c</param>
        /// <param name="L">factor to modify</param>
        /// <param name="X">solution to Lx=b (size n-by-1)</param>
        /// <param name="DeltaB">change in b, zero on output</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_rowdel_mark", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_rowdel_mark (
            size_t k,
            ref CholmodSparse R,
            double[] yk, // const size [2]
            IntPtr colmark, // (int*)
            ref CholmodFactor L,
            ref CholmodDense X,
            ref CholmodDense DeltaB,
            ref CholmodCommon c);

        //*/

        #endregion

        #region Partition

        /*

        /// <summary>
        /// Order A, AA', or A(:,f)*A(:,f)' using CHOLMOD's nested dissection method
        /// (METIS's node bisector applied recursively to compute the separator tree
        /// and constraint sets, followed by CCOLAMD using the constraints).  Usually
        /// finds better orderings than METIS_NodeND, but takes longer.
        /// </summary>
        /// <param name="A">matrix to order</param>
        /// <param name="fset">subset of 0:(A->ncol)-1</param>
        /// <param name="fsize">size of fset</param>
        /// <param name="Perm">size A->nrow, output permutation</param>
        /// <param name="CParent">size A->nrow.  On output, CParent [c] is the parent
        /// of component c, or EMPTY if c is a root, and where
        /// c is in the range 0 to # of components minus 1</param>
        /// <param name="Cmember">size A->nrow.  Cmember [j] = c if node j of A is in component c</param>
        /// <param name="c"></param>
        /// <returns>returns # of components</returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_nested_dissection", CallingConvention = CallingConvention.Cdecl)]
        public static extern SuiteSparse_long cholmod_nested_dissection	(
            ref CholmodSparse A,
            IntPtr fset, // (int*)
            size_t fsize,
            IntPtr Perm, // (int*)
            IntPtr CParent, // (int*)
            IntPtr Cmember, // (int*)
            ref CholmodCommon c);

        /// <summary>
        /// Order A, AA', or A(:,f)*A(:,f)' using METIS_NodeND.
        /// </summary>
        /// <param name="A">matrix to order</param>
        /// <param name="fset">subset of 0:(A->ncol)-1</param>
        /// <param name="fsize">size of fset</param>
        /// <param name="postorder">if TRUE, follow with etree or coletree postorder</param>
        /// <param name="Perm">size A->nrow, output permutation</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_metis", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_metis (
            ref CholmodSparse A,
            IntPtr fset, // (int*)
            size_t fsize,
            int postorder,
            IntPtr Perm, // (int*)
            ref CholmodCommon c);

        /// <summary>
        /// Finds a node bisector of A, A*A', A(:,f)*A(:,f)'.
        /// </summary>
        /// <param name="A">matrix to bisect</param>
        /// <param name="fset">subset of 0:(A->ncol)-1</param>
        /// <param name="fsize">size of fset</param>
        /// <param name="compress">if TRUE, compress the graph first</param>
        /// <param name="Partition">size A->nrow.  Node i is in the left graph if
        /// Partition [i] = 0, the right graph if 1, and in the separator if 2.</param>
        /// <param name="c"></param>
        /// <returns>returns # of nodes in separator</returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_bisect", CallingConvention = CallingConvention.Cdecl)]
        public static extern SuiteSparse_long cholmod_bisect (
            ref CholmodSparse A,
            IntPtr fset, // (int*)
            size_t fsize,
            int compress,
            IntPtr Partition, // (int*)
            ref CholmodCommon c);

        /// <summary>
        /// Find a set of nodes that bisects the graph of A or AA' (direct interface
        /// to METIS_ComputeVertexSeperator).
        /// </summary>
        /// <param name="A">matrix to bisect</param>
        /// <param name="Anw">size A->nrow, node weights, can be NULL, which means the graph is unweighted.</param>
        /// <param name="Aew">size nz, edge weights (silently ignored).
        /// This option was available with METIS 4, but not in METIS 5.  This argument is now unused, but
        /// it remains for backward compatibilty, so as not to change the API for cholmod_metis_bisector.</param>
        /// <param name="Partition"></param>
        /// <param name="c">size A->nrow</param>
        /// <returns>returns separator size</returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_metis_bisector", CallingConvention = CallingConvention.Cdecl)]
        public static extern SuiteSparse_long cholmod_metis_bisector (
            ref CholmodSparse A,
            IntPtr Anw, // (int*)
            IntPtr Aew, // (int*)
            IntPtr Partition, // (int*)
            ref CholmodCommon c);

        /// <summary>
        /// Collapse nodes in a separator tree.
        /// </summary>
        /// <param name="n"># of nodes in the graph</param>
        /// <param name="ncomponents"># of nodes in the separator tree (must be <= n)</param>
        /// <param name="nd_oksep">collapse if #sep >= nd_oksep * #nodes in subtree</param>
        /// <param name="nd_small">collapse if #nodes in subtree < nd_small</param>
        /// <param name="CParent">size ncomponents; from cholmod_nested_dissection</param>
        /// <param name="Cmember">size n; from cholmod_nested_dissection</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_collapse_septree", CallingConvention = CallingConvention.Cdecl)]
        public static extern SuiteSparse_long cholmod_collapse_septree (
            size_t n,
            size_t ncomponents,
            double nd_oksep,
            size_t nd_small,
            IntPtr CParent, // (int*)
            IntPtr Cmember, // (int*)
            ref CholmodCommon c);

        //*/

        #endregion

        #region Supernodal

        /*

        /// <summary>
        /// Analyzes A, AA', or A(:,f)*A(:,f)' in preparation for a supernodal numeric
        /// factorization.  The user need not call this directly; cholmod_analyze is
        /// a "simple" wrapper for this routine.
        /// </summary>
        /// <param name="A">matrix to analyze</param>
        /// <param name="F">F = A' or A(:,f)'</param>
        /// <param name="Parent">elimination tree</param>
        /// <param name="L">simplicial symbolic on input, supernodal symbolic on output</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_super_symbolic", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_super_symbolic (
            ref CholmodSparse A,
            ref CholmodSparse F,
            IntPtr Parent, // (int*)
            ref CholmodFactor L,
            ref CholmodCommon c);

        /// <summary>
        /// Analyze for supernodal Cholesky or multifrontal QR
        /// </summary>
        /// <param name="for_whom">FOR_SPQR     (0): for SPQR but not GPU-accelerated
        /// FOR_CHOLESKY (1): for Cholesky (GPU or not)
        /// FOR_SPQRGPU  (2): for SPQR with GPU acceleration</param>
        /// <param name="A">matrix to analyze</param>
        /// <param name="F">F = A' or A(:,f)'</param>
        /// <param name="Parent">elimination tree</param>
        /// <param name="L">simplicial symbolic on input, supernodal symbolic on output</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_super_symbolic2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_super_symbolic2 (
            int for_whom,
            ref CholmodSparse A,
            ref CholmodSparse F,
            IntPtr Parent, // (int*)
            ref CholmodFactor L,
            ref CholmodCommon c);

        /// <summary>
        /// Computes the numeric LL' factorization of A, AA', or A(:,f)*A(:,f)' using
        /// a BLAS-based supernodal method.  The user need not call this directly;
        /// cholmod_factorize is a "simple" wrapper for this routine.
        /// </summary>
        /// <param name="A">matrix to factorize</param>
        /// <param name="F">F = A' or A(:,f)'</param>
        /// <param name="beta">beta*I is added to diagonal of matrix to factorize</param>
        /// <param name="L">factorization</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_super_numeric", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_super_numeric (
            ref CholmodSparse A,
            ref CholmodSparse F,
            double[] beta, // const size [2]
            ref CholmodFactor L,
            ref CholmodCommon c);

        /// <summary>
        /// Solve Lx=b where L is from a supernodal numeric factorization.  The user
        /// need not call this routine directly.  cholmod_solve is a "simple" wrapper
        /// for this routine
        /// </summary>
        /// <param name="L">factor to use for the forward solve</param>
        /// <param name="X">b on input, solution to Lx=b on output</param>
        /// <param name="E">workspace of size nrhs*(L->maxesize)</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_super_lsolve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_super_lsolve (
            ref CholmodFactor L,
            ref CholmodDense X,
            ref CholmodDense E,
            ref CholmodCommon c);

        /// <summary>
        /// Solve L'x=b where L is from a supernodal numeric factorization.  The user
        /// need not call this routine directly.  cholmod_solve is a "simple" wrapper
        /// for this routine.
        /// </summary>
        /// <param name="L">factor to use for the backsolve</param>
        /// <param name="X">b on input, solution to L'x=b on output</param>
        /// <param name="E">workspace of size nrhs*(L->maxesize)</param>
        /// <param name="c"></param>
        /// <returns></returns>
        [DllImport(CHOLMOD_DLL, EntryPoint = "cholmod_super_ltsolve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cholmod_super_ltsolve (
            ref CholmodFactor L,
            ref CholmodDense X,
            ref CholmodDense E,
            ref CholmodCommon c);

        //*/

        #endregion
    }
}
