namespace CSparse.Interop.Spectra
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    /// <summary>
    /// Sparse matrix in column compressed format.
    /// </summary>
    internal struct spectra_spmat
    {
        /// <summary>
        /// number of rows/columns
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

    internal struct spectra_result
    {
        public IntPtr eigval;
        public IntPtr eigvec;
        public int iterations;
        public int info;
    }

    internal static class NativeMethods
    {
        const string SPECTRA_DLL = "libspectra";

        // Method naming: spectra_[1]i_[2][3]_(shift)
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

        #endregion

        #region Double

        [DllImport(SPECTRA_DLL, EntryPoint = "spectra_di_ss", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int spectra_di_ss(int which, int k, int ncv, int maxit, double tol,
            ref spectra_spmat A, ref spectra_result eigs);

        [DllImport(SPECTRA_DLL, EntryPoint = "spectra_di_ss_shift", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int spectra_di_ss_shift(int which, int k, int ncv, int maxit, double tol, double sigma,
            ref spectra_spmat A, ref spectra_result eigs);

        [DllImport(SPECTRA_DLL, EntryPoint = "spectra_di_ns", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int spectra_di_ns(int which, int k, int ncv, int maxit, double tol,
            ref spectra_spmat A, ref spectra_result eigs);

        [DllImport(SPECTRA_DLL, EntryPoint = "spectra_di_ns_shift", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int spectra_di_ns_shift(int which, int k, int ncv, int maxit, double tol, double sigma,
            ref spectra_spmat A, ref spectra_result eigs);

        [DllImport(SPECTRA_DLL, EntryPoint = "spectra_di_sg", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int spectra_di_sg(int which, int k, int ncv, int maxit, double tol,
            ref spectra_spmat A, ref spectra_spmat B, ref spectra_result eigs);

        [DllImport(SPECTRA_DLL, EntryPoint = "spectra_di_sg_shift", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        public static extern int spectra_di_sg_shift(int which, char mode, int k, int ncv, int maxit, double tol, double sigma,
            ref spectra_spmat A, ref spectra_spmat B, ref spectra_result eigs);

        #endregion

        #region Complex32

        #endregion

        #region Complex

        #endregion
    }
}
