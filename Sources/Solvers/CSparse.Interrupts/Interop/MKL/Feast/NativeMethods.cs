
namespace CSparse.Interop.MKL.Feast
{
    using System;
    using System.Runtime.InteropServices;

    // See https://software.intel.com/en-us/mkl-developer-reference-c-extended-eigensolver-routines

    internal static class NativeMethods
    {
        const string DLL = "mkl_rt.1";

        /// <summary>
        /// Initialize Extended Eigensolver input parameters with default values.
        /// </summary>
        /// <param name="fpm">Array, size 128. This array is used to pass various parameters to Extended Eigensolver routines.</param>
        [DllImport(DLL, EntryPoint = "feastinit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void feastinit(int[] fpm);

        #region Dense

        // Standard Eigenvalue Problem

        [DllImport(DLL, EntryPoint = "dfeast_syev", CallingConvention = CallingConvention.Cdecl)]
        public static extern void dfeast_syev(ref char uplo, ref int n, IntPtr a, ref int lda,
            int[] fpm, out double epsout, out int loop, ref double emin, ref double emax, ref int m0, IntPtr e,
            IntPtr x, out int m, IntPtr res, out int info);

        [DllImport(DLL, EntryPoint = "zfeast_heev", CallingConvention = CallingConvention.Cdecl)]
        public static extern void zfeast_heev(ref char uplo, ref int n, IntPtr a, ref int lda,
            int[] fpm, out double epsout, out int loop, ref double emin, ref double emax, ref int m0, IntPtr e,
            IntPtr x, out int m, IntPtr res, out int info);

        // Generalized Eigenvalue Problem

        [DllImport(DLL, EntryPoint = "dfeast_sygv", CallingConvention = CallingConvention.Cdecl)]
        public static extern void dfeast_sygv(ref char uplo, ref int n, IntPtr a, ref int lda, IntPtr b, ref int ldb,
            int[] fpm, out double epsout, out int loop, ref double emin, ref double emax, ref int m0, IntPtr e,
            IntPtr x, out int m, IntPtr res, out int info);

        [DllImport(DLL, EntryPoint = "zfeast_hegv", CallingConvention = CallingConvention.Cdecl)]
        public static extern void zfeast_hegv(ref char uplo, ref int n, IntPtr a, ref int lda, IntPtr b, ref int ldb,
            int[] fpm, out double epsout, out int loop, ref double emin, ref double emax, ref int m0, IntPtr e,
            IntPtr x, out int m, IntPtr res, out int info);

        #endregion

        #region Sparse

        // Standard Eigenvalue Problem

        [DllImport(DLL, EntryPoint = "dfeast_scsrev", CallingConvention = CallingConvention.Cdecl)]
        public static extern void dfeast_scsrev(ref char uplo, ref int n, IntPtr a, IntPtr ia, IntPtr ja,
            int[] fpm, out double epsout, out int loop, ref double emin, ref double emax, ref int m0, IntPtr e,
            IntPtr x, out int m, IntPtr res, out int info);

        [DllImport(DLL, EntryPoint = "zfeast_hcsrev", CallingConvention = CallingConvention.Cdecl)]
        public static extern void zfeast_hcsrev(ref char uplo, ref int n, IntPtr a, IntPtr ia, IntPtr ja,
            int[] fpm, out double epsout, out int loop, ref double emin, ref double emax, ref int m0, IntPtr e,
            IntPtr x, out int m, IntPtr res, out int info);

        // Generalized Eigenvalue Problem

        [DllImport(DLL, EntryPoint = "dfeast_scsrgv", CallingConvention = CallingConvention.Cdecl)]
        public static extern void dfeast_scsrgv(ref char uplo, ref int n, IntPtr a, IntPtr ia, IntPtr ja,
            IntPtr b, IntPtr ib, IntPtr jb, int[] fpm, out double epsout, out int loop, ref double emin, ref double emax, ref int m0, IntPtr e,
            IntPtr x, out int m, IntPtr res, out int info);

        [DllImport(DLL, EntryPoint = "zfeast_hcsrgv", CallingConvention = CallingConvention.Cdecl)]
        public static extern void zfeast_hcsrgv(ref char uplo, ref int n, IntPtr a, IntPtr ia, IntPtr ja,
            IntPtr b, IntPtr ib, IntPtr jb, int[] fpm, out double epsout, out int loop, ref double emin, ref double emax, ref int m0, IntPtr e,
            IntPtr x, out int m, IntPtr res, out int info);

        #endregion
    }
}
