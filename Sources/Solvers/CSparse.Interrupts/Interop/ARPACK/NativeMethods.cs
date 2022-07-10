namespace CSparse.Interop.ARPACK
{
    using System;
    using System.Numerics;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;

    /// <summary>
    /// Sparse matrix in column compressed format.
    /// </summary>
    internal struct ar_spmat
    {
        /// <summary>
        /// number of rows
        /// </summary>
        public int m;
        /// <summary>
        /// number of columns
        /// </summary>
        public int n;
        /// <summary>
        /// array of nonzero values
        /// </summary>
        public IntPtr x;
        /// <summary>
        /// array of column indices
        /// </summary>
        public IntPtr i;
        /// <summary>
        /// array of row pointers
        /// </summary>
        public IntPtr p;
        /// <summary>
        /// number of nonzeros in the matrix
        /// </summary>
        public int nnz;
    }

    internal struct ar_result
    {
        public IntPtr eigvalr;
        public IntPtr eigvali;
        public IntPtr eigvec;
        public int iterations;
        public int ncv;
        public int info;
    }

    internal static class NativeMethods
    {
        const string ARPACK_DLL = "libarpack";

        // Method naming: ar_[1]i_[2][3]_(shift)
        //
        // [1] s|d|c|z = single / double / complex32 / complex
        // [2]     s|n = symmetric / non-symmetric
        // [3]     s|g = standard  / generalized

        // Method parameters:
        //
        // [in]       which = requested spectrum
        // [in]           k = number of eigenvalues
        // [in]         ncv = number of basis vectors
        // [in]       maxit = maximum number of iterations
        // [in]         tol = tolerance
        // [in]       sigma = sigma for shifted mode
        
        #region Single

        [DllImport(ARPACK_DLL, EntryPoint = "ar_si_ss", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_si_ss(StringBuilder which, int k, int ncv, int maxit, double tol,
            ref ar_spmat A, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_si_ss_shift", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_si_ss_shift(StringBuilder which, int k, int ncv, int maxit, double tol, float sigma,
            ref ar_spmat A, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_si_ns", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_si_ns(StringBuilder which, int k, int ncv, int maxit, double tol,
            ref ar_spmat A, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_si_ns_shift", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_si_ns_shift(StringBuilder which, int k, int ncv, int maxit, double tol, float sigma,
            ref ar_spmat A, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_si_sg", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_si_sg(StringBuilder which, int k, int ncv, int maxit, double tol,
            ref ar_spmat A, ref ar_spmat B, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_si_sg_shift", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_si_sg_shift(StringBuilder which, char mode, int k, int ncv, int maxit, double tol, float sigma,
            ref ar_spmat A, ref ar_spmat B, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_si_ng", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_si_ng(StringBuilder which, int k, int ncv, int maxit, double tol,
            ref ar_spmat A, ref ar_spmat B, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_si_ng_shift", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_si_ng_shift(StringBuilder which, int k, int ncv, int maxit, double tol, float sigma,
            ref ar_spmat A, ref ar_spmat B, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_si_ng_shift_cx", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_si_ng_shift_cx(StringBuilder which, int k, int ncv, int maxit, double tol, char part, float sigma_r, float sigma_i,
            ref ar_spmat A, ref ar_spmat B, ref ar_result eigs);
        
        #endregion

        #region Double

        [DllImport(ARPACK_DLL, EntryPoint = "ar_di_ss", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_di_ss(StringBuilder which, int k, int ncv, int maxit, double tol,
            ref ar_spmat A, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_di_ss_shift", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_di_ss_shift(StringBuilder which, int k, int ncv, int maxit, double tol, double sigma,
            ref ar_spmat A, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_di_ns", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_di_ns(StringBuilder which, int k, int ncv, int maxit, double tol,
            ref ar_spmat A, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_di_ns_shift", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_di_ns_shift(StringBuilder which, int k, int ncv, int maxit, double tol, double sigma,
            ref ar_spmat A, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_di_sg", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_di_sg(StringBuilder which, int k, int ncv, int maxit, double tol,
            ref ar_spmat A, ref ar_spmat B, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_di_sg_shift", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_di_sg_shift(StringBuilder which, char mode, int k, int ncv, int maxit, double tol, double sigma,
            ref ar_spmat A, ref ar_spmat B, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_di_ng", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_di_ng(StringBuilder which, int k, int ncv, int maxit, double tol,
            ref ar_spmat A, ref ar_spmat B, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_di_ng_shift", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_di_ng_shift(StringBuilder which, int k, int ncv, int maxit, double tol, double sigma,
            ref ar_spmat A, ref ar_spmat B, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_di_ng_shift_cx", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_di_ng_shift_cx(StringBuilder which, int k, int ncv, int maxit, double tol, char part, double sigma_r, double sigma_i,
            ref ar_spmat A, ref ar_spmat B, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_di_svd_nrm", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_di_svd_nrm(StringBuilder which, int k, int ncv, int maxit, double tol,
            ref ar_spmat A, ref ar_result result);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_di_svd", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_di_svd(StringBuilder which, int k, int ncv, int maxit, double tol,
            ref ar_spmat A, ref ar_result result);

        #endregion

        #region Complex32

        /*
        [DllImport(DllName, EntryPoint = "ar_ci_ns", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_ci_ns(StringBuilder which, int k, int ncv, int maxit, double tol,
            ref spmat A, ref ar_result eigs);

        [DllImport(DllName, EntryPoint = "ar_ci_ns_shift", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_ci_ns_shift(StringBuilder which, int k, int ncv, int maxit, double tol, Complex32 sigma,
            ref spmat A, ref ar_result eigs);

        [DllImport(DllName, EntryPoint = "ar_ci_ng", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_ci_ng(StringBuilder which, int k, int ncv, int maxit, double tol,
            ref spmat A, ref spmat B, ref ar_result eigs);

        [DllImport(DllName, EntryPoint = "ar_ci_ng_shift", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_ci_ng_shift(StringBuilder which, int k, int ncv, int maxit, double tol, Complex32 sigma,
            ref spmat A, ref spmat B, ref ar_result eigs);
        //*/

        #endregion

        #region Complex

        [DllImport(ARPACK_DLL, EntryPoint = "ar_zi_ns", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_zi_ns(StringBuilder which, int k, int ncv, int maxit, double tol,
            ref ar_spmat A, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_zi_ns_shift", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_zi_ns_shift(StringBuilder which, int k, int ncv, int maxit, double tol, Complex sigma,
            ref ar_spmat A, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_zi_ng", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_zi_ng(StringBuilder which, int k, int ncv, int maxit, double tol,
            ref ar_spmat A, ref ar_spmat B, ref ar_result eigs);

        [DllImport(ARPACK_DLL, EntryPoint = "ar_zi_ng_shift", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int ar_zi_ng_shift(StringBuilder which, int k, int ncv, int maxit, double tol, Complex sigma,
            ref ar_spmat A, ref ar_spmat B, ref ar_result eigs);

        #endregion
    }
}
