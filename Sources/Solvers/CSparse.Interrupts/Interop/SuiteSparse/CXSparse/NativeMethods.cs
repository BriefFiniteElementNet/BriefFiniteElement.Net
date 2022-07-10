namespace CSparse.Interop.SuiteSparse.CXSparse
{
    using System;
    using System.Runtime.InteropServices;
    using System.Numerics;

#if X64
    using size_t = System.UInt64;
#else
    using size_t = System.UInt32;
#endif

    #region Structures

    /// <summary>
    /// Matrix in compressed-column or triplet form.
    /// </summary>
    internal struct cs /* cs_di, cs_ci */
    {
        /// <summary>
        /// maximum number of entries
        /// </summary>
        public int nzmax;
        /// <summary>
        /// number of rows
        /// </summary>
        public int m;
        /// <summary>
        /// number of columns
        /// </summary>
        public int n;
        /// <summary>
        /// column pointers (size n+1) or col indices (size nzmax)
        /// </summary>
        public IntPtr p;
        /// <summary>
        /// row indices, size nzmax
        /// </summary>
        public IntPtr i;
        /// <summary>
        /// // numerical values, size nzmax
        /// </summary>
        public IntPtr x;
        /// <summary>
        /// // # of entries in triplet matrix, -1 for compressed-col
        /// </summary>
        public int nz;
    }

    /// <summary>
    /// Numeric Cholesky, LU, or QR factorization.
    /// </summary>
    internal struct csn /* cs_din */
    {
        /// <summary>
        /// cs_di: L for LU and Cholesky, V for QR
        /// </summary>
        public IntPtr L;
        /// <summary>
        /// cs_di: U for LU, r for QR, not used for Cholesky
        /// </summary>
        public IntPtr U;
        /// <summary>
        /// partial pivoting for LU
        /// </summary>
        public IntPtr pinv;
        /// <summary>
        /// beta [0..n-1] for QR
        /// </summary>
        public IntPtr B;
    }

    /// <summary>
    /// Symbolic Cholesky, LU, or QR analysis.
    /// </summary>
    internal struct css /* cs_dis */
    {
        /// <summary>
        /// inverse row perm. for QR, fill red. perm for Chol
        /// </summary>
        public IntPtr pinv;
        /// <summary>
        /// fill-reducing column permutation for LU and QR
        /// </summary>
        public IntPtr q;
        /// <summary>
        /// elimination tree for Cholesky and QR
        /// </summary>
        public IntPtr parent;
        /// <summary>
        /// column pointers for Cholesky, row counts for QR
        /// </summary>
        public IntPtr cp;
        /// <summary>
        /// leftmost[i] = min(find(A(i,:))), for QR
        /// </summary>
        public IntPtr leftmost;
        /// <summary>
        /// # of rows for QR, after adding fictitious rows
        /// </summary>
        public int m2;
        /// <summary>
        /// # entries in L for LU or Cholesky; in V for QR
        /// </summary>
        public double lnz;
        /// <summary>
        /// # entries in U for LU; in R for QR
        /// </summary>
        public double unz;
    }

    /// <summary>
    /// cs_dmperm or cs_scc output
    /// </summary>
    internal struct csd /* cs_did */
    {
        /// <summary>
        /// size m, row permutation
        /// </summary>
        public IntPtr p;
        /// <summary>
        /// size n, column permutation
        /// </summary>
        public IntPtr q;
        /// <summary>
        /// size nb+1, block k is rows r[k] to r[k+1]-1 in A(p,q)
        /// </summary>
        public IntPtr r;
        /// <summary>
        /// size nb+1, block k is cols s[k] to s[k+1]-1 in A(p,q)
        /// </summary>
        public IntPtr s;
        /// <summary>
        /// # of blocks in fine dmperm decomposition
        /// </summary>
        public int nb;
        /// <summary>
        /// coarse row decomposition
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public int[] rr;
        /// <summary>
        /// coarse column decomposition
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public int[] cc;
    }

    #endregion

    internal static class NativeMethods
    {
#if SUITESPARSE_AIO
        const string CXSPARSE_DLL = "libsuitesparse";
#else
        const string CXSPARSE_DLL = "libcxsparse";
#endif

        #region Double

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_add", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* cs* */ cs_di_add(ref cs A, ref cs B, double alpha, double beta);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_cholsol", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_cholsol(int order, ref cs A, IntPtr /* double* */ b);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_dupl", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_dupl(ref cs A);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_entry", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_entry(ref cs T, int i, int j, double x);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_lusol", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_lusol(int order, ref cs A, IntPtr /* double* */ b, double tol);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_gaxpy", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_gaxpy(ref cs A, IntPtr /* double* */ x, IntPtr /* double* */ y);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_multiply", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* cs* */ cs_di_multiply(ref cs A, ref cs B);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_qrsol", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_qrsol(int order, ref cs A, IntPtr /* double* */ b);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_transpose", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* cs* */ cs_di_transpose(ref cs A, int values);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_compress", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* cs* */ cs_di_compress(ref cs T);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_norm", CallingConvention = CallingConvention.Cdecl)]
        public static extern double cs_di_norm(ref cs A);

        /* utilities */

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_calloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* void* */ cs_di_calloc(int n, size_t size);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* void* */ cs_di_free(IntPtr /* void* */ p);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_realloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* void* */ cs_di_realloc(IntPtr /* void* */ p, int n, size_t size, ref int ok);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_spalloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* cs* */ cs_di_spalloc(int m, int n, int nzmax, int values, int t);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_spfree", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* cs* */ cs_di_spfree(ref cs A);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_spfree", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* cs* */ cs_di_spfree(IntPtr A);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_sprealloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_sprealloc(ref cs A, int nzmax);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_malloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* void* */ cs_di_malloc(int n, size_t size);


        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_amd", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* int* */ cs_di_amd(int order, ref cs A);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_chol", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* csn* */ cs_di_chol(ref cs A, ref css S);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_dmperm", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* csd* */ cs_di_dmperm(ref cs A, int seed);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_droptol", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_droptol(ref cs A, double tol);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_dropzeros", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_dropzeros(ref cs A);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_happly", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_happly(IntPtr /* cs* */ V, int i, double beta, IntPtr /* double* */ x);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_ipvec", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_ipvec(IntPtr /* int* */ p, IntPtr /* double* */ b, IntPtr /* double* */ x, int n);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_lsolve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_lsolve(IntPtr /* cs* */ L, IntPtr /* double* */ x);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_ltsolve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_ltsolve(IntPtr /* cs* */ L, IntPtr /* double* */ x);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_lu", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* csn* */ cs_di_lu(ref cs A, ref css S, double tol);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_permute", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* cs* */ cs_di_permute(ref cs A, IntPtr /* int* */ pinv, IntPtr /* int* */ q, int values);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_pinv", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* int* */ cs_di_pinv(IntPtr /* int* */ p, int n);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_pvec", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_pvec(IntPtr /* int* */ p, IntPtr /* double* */ b, IntPtr /* double* */ x, int n);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_qr", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* csn* */ cs_di_qr(ref cs A, ref css S);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_schol", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* css* */ cs_di_schol(int order, ref cs A);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_sqr", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* css* */ cs_di_sqr(int order, ref cs A, int qr);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_symperm", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* cs* */ cs_di_symperm(ref cs A, IntPtr /* int* */ pinv, int values);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_usolve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_usolve(IntPtr /* cs* */ U, IntPtr /* double* */ x);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_utsolve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_utsolve(IntPtr /* cs* */ U, IntPtr /* double* */ x);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_updown", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_updown(ref cs L, int sigma, ref cs C, IntPtr /* int* */ parent);

        /* utilities */

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_sfree", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* css* */ cs_di_sfree(ref css S);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_nfree", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* csn* */ cs_di_nfree(ref csn N);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_dfree", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* csd* */ cs_di_dfree(ref csd D);

        /* --- tertiary CSparse routines -------------------------------------------- */

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_counts", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* int* */ cs_di_counts(ref cs A, IntPtr /* int* */ parent, IntPtr /* int* */ post, int ata);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_cumsum", CallingConvention = CallingConvention.Cdecl)]
        public static extern double cs_di_cumsum(IntPtr /* int* */ p, IntPtr /* int* */ c, int n);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_dfs", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_dfs(int j, ref cs G, int top, IntPtr /* int* */ xi, IntPtr /* int* */ pstack, IntPtr /* int* */ pinv);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_etree", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* int* */ cs_di_etree(ref cs A, int ata);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int dfkeep(int i, int j, double aij, IntPtr cs);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_fkeep", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_fkeep(ref cs A, [MarshalAs(UnmanagedType.FunctionPtr)] dfkeep fkeep, IntPtr /* void* */ other);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_house", CallingConvention = CallingConvention.Cdecl)]
        public static extern double cs_di_house(IntPtr /* double* */ x, ref double beta, int n);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_maxtrans", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* int* */ cs_di_maxtrans(ref cs A, int seed);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_post", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* int* */ cs_di_post(IntPtr /* int* */ parent, int n);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_scc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* csd* */ cs_di_scc(ref cs A);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_scatter", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_scatter(ref cs A, int j, double beta, IntPtr /* int* */ w, IntPtr /* double* */ x, int mark, ref cs C, int nz);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_tdfs", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_tdfs(int j, int k, IntPtr /* int* */ head, IntPtr /* int* */ next, IntPtr /* int* */ post, IntPtr /* int* */ stack);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_leaf", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_leaf(int i, int j, IntPtr /* int* */ first, IntPtr /* int* */ maxfirst, IntPtr /* int* */ prevleaf, IntPtr /* int* */ ancestor, IntPtr /* int* */ jleaf);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_reach", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_reach(ref cs G, ref cs B, int k, IntPtr /* int* */ xi, IntPtr /* int* */ pinv);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_spsolve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_spsolve(ref cs L, ref cs B, int k, IntPtr /* int* */ xi, IntPtr /* double* */ x, IntPtr /* int* */ pinv, int lo);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_ereach", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_di_ereach(ref cs A, int k, IntPtr /* int* */ parent, IntPtr /* int* */ s, IntPtr /* int* */ w);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_randperm", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* int* */ cs_di_randperm(int n, int seed);

        /* utilities */

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_dalloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* csd* */ cs_di_dalloc(int m, int n);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_done", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* cs* */ cs_di_done(ref cs C, IntPtr /* void* */ w, IntPtr /* void* */ x, int ok);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_idone", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* int* */ cs_di_idone(IntPtr /* int* */ p, ref cs C, IntPtr /* void* */ w, int ok);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_ndone", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* csn* */ cs_di_ndone(ref csn N, ref cs C, IntPtr /* void* */ w, IntPtr /* void* */ x, int ok);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_di_ddone", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* csd* */ cs_di_ddone(ref csd D, ref cs C, IntPtr /* void* */ w, int ok);

        #endregion

        #region Complex

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_ci_transpose", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* cs* */ cs_ci_transpose(ref cs A, int values);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_ci_cholsol", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_ci_cholsol(int order, ref cs A, IntPtr /* double* */ b);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_ci_lusol", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_ci_lusol(int order, ref cs A, IntPtr /* double* */ b, double tol);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_ci_qrsol", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_ci_qrsol(int order, ref cs A, IntPtr /* double* */ b);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_ci_chol", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* csn* */ cs_ci_chol(ref cs A, ref css S);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_ci_happly", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_ci_happly(IntPtr /* cs* */ V, int i, double beta, IntPtr /* double* */ x);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_ci_ipvec", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_ci_ipvec(IntPtr /* int* */ p, IntPtr /* double* */ b, IntPtr /* double* */ x, int n);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_ci_lsolve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_ci_lsolve(IntPtr /* cs* */ L, IntPtr /* double* */ x);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_ci_ltsolve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_ci_ltsolve(IntPtr /* cs* */ L, IntPtr /* double* */ x);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_ci_lu", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* csn* */ cs_ci_lu(ref cs A, ref css S, double tol);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_ci_pinv", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* int* */ cs_ci_pinv(IntPtr /* int* */ p, int n);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_ci_pvec", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_ci_pvec(IntPtr /* int* */ p, IntPtr /* double* */ b, IntPtr /* double* */ x, int n);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_ci_qr", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* csn* */ cs_ci_qr(ref cs A, ref css S);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_ci_schol", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* css* */ cs_ci_schol(int order, ref cs A);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_ci_sqr", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* css* */ cs_ci_sqr(int order, ref cs A, int qr);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_ci_usolve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_ci_usolve(IntPtr /* cs* */ U, IntPtr /* double* */ x);

        [DllImport(CXSPARSE_DLL, EntryPoint = "cs_ci_utsolve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cs_ci_utsolve(IntPtr /* cs* */ U, IntPtr /* double* */ x);

        #endregion
    }
}
