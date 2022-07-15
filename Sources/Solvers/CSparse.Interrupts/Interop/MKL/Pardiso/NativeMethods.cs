
namespace CSparse.Interop.MKL.Pardiso
{
    using System;
    using System.Runtime.InteropServices;

    // See https://software.intel.com/en-us/mkl-developer-reference-c-intel-mkl-pardiso-parallel-direct-sparse-solver-interface
    // to download pardiso dlls, download and install intel oneapi, use web installer and only install math kernel library
    internal static class NativeMethods
    {
        

        [DllImport(Interrupts.Interop.NativeBinaryPathes.MKL_DLL, EntryPoint = "pardisoinit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void pardisoinit(IntPtr[] pt, /*const*/ ref int mtype, int[] iparm);

        [DllImport(Interrupts.Interop.NativeBinaryPathes.MKL_DLL, EntryPoint = "pardiso", CallingConvention = CallingConvention.Cdecl)]
        public static extern void pardiso(IntPtr[] pt, /*const*/ ref int maxfct, /*const*/ ref int mnum, /*const*/ ref int mtype,
            /*const*/ ref int phase, /*const*/ ref int n, /*const*/ IntPtr a, /*const*/ IntPtr ia, /*const*/ IntPtr ja, int[] perm,
            /*const*/ ref int nrhs, int[] iparm, /*const*/ ref int msglvl, IntPtr b, IntPtr x, out int error);



    }
}
