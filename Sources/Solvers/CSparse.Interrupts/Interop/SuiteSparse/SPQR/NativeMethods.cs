
namespace CSparse.Interop.SuiteSparse.SPQR
{
    using System;
    using System.Runtime.InteropServices;
    using CSparse.Interop.SuiteSparse.Cholmod;

#if X64
    using size_t = System.UInt64;
    using sp_long = System.Int32; // Modified version of SPQR uses Long = int
#else
    using size_t = System.UInt32;
    using sp_long = System.Int32;
#endif

    internal static class NativeMethods
    {
#if SUITESPARSE_AIO
        const string SPQR_DLL = "libsuitesparse";
#else
        const string SPQR_DLL = "libspqr";
#endif

        [DllImport(SPQR_DLL, EntryPoint = "SuiteSparse_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SuiteSparse_free(IntPtr p);

        /// <summary>
        /// SuiteSparseQR_C
        /// </summary>
        /// <param name="ordering">all, except 3:given treated as 0:fixed</param>
        /// <param name="tol">columns with 2-norm <= tol treated as 0</param>
        /// <param name="econ">e = max(min(m,econ),rank(A))</param>
        /// <param name="getCTX">0: Z=C (e-by-k), 1: Z=C', 2: Z=X (e-by-k)</param>
        /// <param name="A">m-by-n sparse matrix to factorize</param>
        /// <param name="Bsparse">sparse m-by-k B</param>
        /// <param name="Bdense">dense  m-by-k B</param>
        /// <param name="Zsparse">sparse Z</param>
        /// <param name="Zdense">dense Z</param>
        /// <param name="R">e-by-n sparse matrix</param>
        /// <param name="E">size n column perm, NULL if identity</param>
        /// <param name="H">m-by-nh Householder vectors</param>
        /// <param name="HPinv">size m row permutation</param>
        /// <param name="HTau">1-by-nh Householder coefficients</param>
        /// <param name="cc">workspace and parameters</param>
        /// <returns>returns rank(A) estimate, (-1) if failure</returns>
        [DllImport(SPQR_DLL, EntryPoint = "SuiteSparseQR_C", CallingConvention = CallingConvention.Cdecl)]
        public static extern sp_long SuiteSparseQR_C
        (
            /* inputs: */
            int ordering,
            double tol,
            sp_long econ,
            int getCTX,
            ref CholmodSparse A,
            IntPtr Bsparse, // ref CholmodSparse
            ref CholmodDense Bdense,
            /* outputs: */
            out IntPtr Zsparse, // CholmodSparse
            out IntPtr Zdense, // CholmodDense
            out IntPtr R, // CholmodSparse
            out IntPtr E, // SuiteSparse_long[]
            out IntPtr H, // CholmodSparse
            out IntPtr HPinv, // SuiteSparse_long[]
            out IntPtr HTau, // CholmodDense
            ref CholmodCommon cc // 
        );

        /// <summary>
        /// [Q,R,E] = qr(A), returning Q as a sparse matrix
        /// </summary>
        /// <param name="ordering">all, except 3:given treated as 0:fixed</param>
        /// <param name="tol">columns with 2-norm <= tol treated as 0</param>
        /// <param name="econ">e = max(min(m,econ),rank(A))</param>
        /// <param name="A">m-by-n sparse matrix to factorize</param>
        /// <param name="Q">m-by-e sparse matrix</param>
        /// <param name="R">e-by-n sparse matrix</param>
        /// <param name="E">size n column perm, NULL if identity</param>
        /// <param name="cc">workspace and parameters</param>
        /// <returns>returns rank(A) est., (-1) if failure</returns>
        [DllImport(SPQR_DLL, EntryPoint = "SuiteSparseQR_C_QR", CallingConvention = CallingConvention.Cdecl)]
        public static extern sp_long SuiteSparseQR_C_QR
        (
            /* inputs: */
            int ordering,
            double tol,
            sp_long econ,
            ref CholmodSparse A,
            /* outputs: */
            out IntPtr Q, // CholmodSparse
            out IntPtr R, // CholmodSparse
            out IntPtr E, // SuiteSparse_long[]
            ref CholmodCommon cc
        );

        /// <summary>
        /// X = A\B where B is dense
        /// </summary>
        /// <param name="ordering">all, except 3:given treated as 0:fixed</param>
        /// <param name="tol">columns with 2-norm <= tol treated as 0</param>
        /// <param name="A">m-by-n sparse matrix</param>
        /// <param name="B">m-by-k</param>
        /// <param name="cc">workspace and parameters</param>
        /// <returns>returns X, NULL if failure</returns>
        [DllImport(SPQR_DLL, EntryPoint = "SuiteSparseQR_C_backslash", CallingConvention = CallingConvention.Cdecl)]
        public static extern CholmodDense SuiteSparseQR_C_backslash
        (
            int ordering,
            double tol,
            ref CholmodSparse A,
            ref CholmodDense B,
            ref CholmodCommon cc
        );

        /// <summary>
        /// X = A\B where B is dense, using default ordering and tol
        /// </summary>
        /// <param name="A">m-by-n sparse matrix</param>
        /// <param name="B">m-by-k</param>
        /// <param name="cc">workspace and parameters</param>
        /// <returns>returns X, NULL if failure</returns>
        [DllImport(SPQR_DLL, EntryPoint = "SuiteSparseQR_C_backslash_default", CallingConvention = CallingConvention.Cdecl)]
        public static extern CholmodDense SuiteSparseQR_C_backslash_default
        (
            ref CholmodSparse A,
            ref CholmodDense B,
            ref CholmodCommon cc
        );

        /// <summary>
        /// X = A\B where B is sparse
        /// </summary>
        /// <param name="ordering">all, except 3:given treated as 0:fixed</param>
        /// <param name="tol">columns with 2-norm <= tol treated as 0</param>
        /// <param name="A">m-by-n sparse matrix</param>
        /// <param name="B">m-by-k</param>
        /// <param name="cc">workspace and parameters</param>
        /// <returns>returns X, or NULL</returns>
        [DllImport(SPQR_DLL, EntryPoint = "SuiteSparseQR_C_backslash_sparse", CallingConvention = CallingConvention.Cdecl)]
        public static extern CholmodSparse SuiteSparseQR_C_backslash_sparse
        (
            /* inputs: */
            int ordering,
            double tol,
            ref CholmodSparse A,
            ref CholmodSparse B,
            ref CholmodCommon cc
        );

        // EXPERT

        /// <summary>
        /// Performs both the symbolic and numeric factorizations and returns a QR factorization object
        /// such that A*P=Q*R. It always exploits singletons.
        /// </summary>
        /// <param name="ordering">all, except 3:given treated as 0:fixed</param>
        /// <param name="tol">columns with 2-norm <= tol treated as 0</param>
        /// <param name="A">m-by-n sparse matrix</param>
        /// <param name="cc">workspace and parameters</param>
        /// <returns>SuiteSparseQR_C_factorization</returns>
        [DllImport(SPQR_DLL, EntryPoint = "SuiteSparseQR_C_factorize", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SuiteSparseQR_C_factorize
        (
            /* inputs: */
            int ordering,
            double tol,
            ref CholmodSparse A,
            ref CholmodCommon cc
        );

        /// <summary>
        /// Performs the symbolic factorization and returns a QR factorization object to be passed to
        /// SuiteSparseQR_numeric. It does not exploit singletons.
        /// </summary>
        /// <param name="ordering">all, except 3:given treated as 0:fixed</param>
        /// <param name="allow_tol">if TRUE allow tol for rank detection</param>
        /// <param name="A">m-by-n sparse matrix, A->x ignored</param>
        /// <param name="cc">workspace and parameters</param>
        /// <returns>SuiteSparseQR_C_factorization</returns>
        [DllImport(SPQR_DLL, EntryPoint = "SuiteSparseQR_C_symbolic", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SuiteSparseQR_C_symbolic
        (
            /* inputs: */
            int ordering,
            int allow_tol,
            ref CholmodSparse A,
            ref CholmodCommon cc
        );

        /// <summary>
        /// Performs the numeric factorization on a QR factorization object, either one constructed
        /// by SuiteSparseQR_symbolic, or reusing one from a prior call to SuiteSparseQR_numeric for
        /// a matrix A with the same pattern as the first one, but with different numerical values.
        /// </summary>
        /// <param name="tol">treat columns with 2-norm &lt;= tol as zero</param>
        /// <param name="A">sparse matrix to factorize</param>
        /// <param name="QR">SuiteSparseQR_C_factorization</param>
        /// <param name="cc">workspace and parameters</param>
        /// <returns>returns TRUE (1) if successful, FALSE (0) otherwise</returns>
        [DllImport(SPQR_DLL, EntryPoint = "SuiteSparseQR_C_numeric", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SuiteSparseQR_C_numeric
        (
            /* inputs: */
            double tol,
            ref CholmodSparse A,
            /* input/output: */
            IntPtr QR,
            ref CholmodCommon cc
        );

        /// <summary>
        /// Free the QR factors computed by SuiteSparseQR_C_factorize
        /// </summary>
        /// <param name="QR">SuiteSparseQR_C_factorization</param>
        /// <param name="cc">workspace and parameters</param>
        /// <returns>returns TRUE (1) if OK, FALSE (0) otherwise</returns>
        [DllImport(SPQR_DLL, EntryPoint = "SuiteSparseQR_C_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SuiteSparseQR_C_free
        (
            ref IntPtr QR,
            ref CholmodCommon cc
        );

        /// <summary>
        /// Solves a linear system using the QR object.
        /// </summary>
        /// <param name="system">which system to solve</param>
        /// <param name="QR">SuiteSparseQR_C_factorization</param>
        /// <param name="B">right-hand-side, m-by-k or n-by-k</param>
        /// <param name="cc">workspace and parameters</param>
        /// <returns>return CholmodDense pointer X, or NULL if failure</returns>
        /// <remarks>
        /// system SPQR_RX_EQUALS_B    (0): X = R\B         B is m-by-k and X is n-by-k
        /// system SPQR_RETX_EQUALS_B  (1): X = E*(R\B)     as above, E is a permutation
        /// system SPQR_RTX_EQUALS_B   (2): X = R'\B        B is n-by-k and X is m-by-k
        /// system SPQR_RTX_EQUALS_ETB (3): X = R'\(E'*B)   as above, E is a permutation
        /// </remarks>
        [DllImport(SPQR_DLL, EntryPoint = "SuiteSparseQR_C_solve", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SuiteSparseQR_C_solve
        (
            int system,
            IntPtr QR,
            IntPtr B,
            ref CholmodCommon cc
        );

        /// <summary>
        /// Applies Q in Householder form (as stored in the QR factorization object
        /// returned by SuiteSparseQR_C_factorize) to a dense matrix X.
        /// </summary>
        /// <param name="method">0,1,2,3</param>
        /// <param name="QR">SuiteSparseQR_C_factorization</param>
        /// <param name="X">cholmod_dense size m-by-n with leading dimension ldx</param>
        /// <param name="cc">workspace and parameters</param>
        /// <returns>return CholmodDense pointer X, or NULL if failure</returns>
        /// <remarks>
        /// method SPQR_QTX (0): Y = Q'*X
        /// method SPQR_QX  (1): Y = Q*X
        /// method SPQR_XQT (2): Y = X*Q'
        /// method SPQR_XQ  (3): Y = X*Q
        /// </remarks>
        [DllImport(SPQR_DLL, EntryPoint = "SuiteSparseQR_C_qmult", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SuiteSparseQR_C_qmult /* returns Y, or NULL on failure */
        (
            /* inputs: */
            int method,
            IntPtr QR,
            ref CholmodDense X,
            ref CholmodCommon cc
        );
    }
}
