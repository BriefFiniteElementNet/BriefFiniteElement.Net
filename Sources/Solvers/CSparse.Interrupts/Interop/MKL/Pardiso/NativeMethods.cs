
namespace CSparse.Interop.MKL.Pardiso
{
    using System;
    using System.Runtime.InteropServices;

    // See https://software.intel.com/en-us/mkl-developer-reference-c-intel-mkl-pardiso-parallel-direct-sparse-solver-interface

    internal static class NativeMethods
    {
        const string DLL = @"C:\Program Files (x86)\Intel\oneAPI\mkl\2022.1.0\redist\intel64\mkl_rt.2.dll";

        [DllImport(DLL, EntryPoint = "pardisoinit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void pardisoinit(IntPtr[] pt, /*const*/ ref int mtype, int[] iparm);

        [DllImport(DLL, EntryPoint = "pardiso", CallingConvention = CallingConvention.Cdecl)]
        public static extern void pardiso(IntPtr[] pt, /*const*/ ref int maxfct, /*const*/ ref int mnum, /*const*/ ref int mtype,
            /*const*/ ref int phase, /*const*/ ref int n, /*const*/ IntPtr a, /*const*/ IntPtr ia, /*const*/ IntPtr ja, int[] perm,
            /*const*/ ref int nrhs, int[] iparm, /*const*/ ref int msglvl, IntPtr b, IntPtr x, out int error);



    }
}
