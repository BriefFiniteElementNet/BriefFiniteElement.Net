using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet;
using CSparse.Double;
using System.Runtime.InteropServices;
using System.Security;

namespace BriefFiniteElementNet.IntelMklSolver
{
    /// <summary>
    /// Represents a direct solver for SPD (Symetric Positive Definite) matrixes, based on intel's Math Kernel Library (MKL) pardiso
    /// </summary>
    [Obsolete()]
    public class MklPardisoDirectSPDSolver : ISolver,IDisposable
    {
        private readonly int mtype = 2;//real and symmetric positive definite : 2

        public CompressedColumnStorage A { get; set; }

        //upper triangle of A
        //public CompressedColumnStorage UPart { get; set; }


        public bool IsInitialized
        {
            get { return _isInitialized; }
        }

        private bool _isInitialized;

        int[] iparm;
        IntPtr[] pt;
        
        int maxfct = 1, mnum = 1, msglvl = 1, phase, error = 0;


        int[] idum = new int[1]; /* Integer dummy. */
        double[] ddum = new double[1];/* Double dummy */


        private void InitVariables()
        {
            var oldPath = Environment.GetEnvironmentVariable("PATH");

            var newPath = oldPath+ @"C:\Program Files (x86)\IntelSWTools\compilers_and_libraries_2018.0.124\windows\redist\intel64_win\mkl;";

            Environment.SetEnvironmentVariable("PATH",newPath);

            var tt = System.IO.File.Exists(@"mkl_intel_thread.dll");

            pt = new IntPtr[64];
            /* Pardiso control parameters. */
            iparm = new int[64];

            int i;

            /* ----------------------------------------------------------------- */
            /* .. Setup Pardiso control parameters. */
            /* ----------------------------------------------------------------- */
            for (i = 0; i < 64; i++)
                iparm[i] = 0;

            iparm[0] = 1; /* No solver default */
            iparm[1] = 2; /* Fill-in reordering from METIS */
            iparm[3] = 0; /* No iterative-direct algorithm */
            iparm[4] = 0; /* No user fill-in reducing permutation */
            iparm[5] = 0; /* Write solution into x */
            iparm[7] = 2; /* Max numbers of iterative refinement steps */
            iparm[8] = 0; /* Not in use */
            iparm[9] = 13; /* Perturb the pivot elements with 1E-13 */
            iparm[10] = 1; /* Use nonsymmetric permutation and scaling MPS */
            iparm[12] = 0; /* Maximum weighted matching algorithm is switched-off
                        * (default for symmetric). Try iparm[12] = 1 in case of
                        *  inappropriate accuracy */
            iparm[13] = 0; /* Output: Number of perturbed pivots */
            iparm[17] = -1; /* Output: Number of nonzeros in the factor LU */
            iparm[18] = -1; /* Output: Mflops for LU factorization */
            iparm[19] = 0; /* Output: Numbers of CG Iterations */
            iparm[34] = 1; /* Zero based indexing of ia and ja */


            for (i = 0; i < 64; i++)
                pt[i] = IntPtr.Zero;

            maxfct = 1;
            mnum = 1;
            error = 0;
            msglvl = 1;
        }

        public Trace Trace;

        public void Initialize()
        {
            if (_isInitialized)
                return;

            var trace = Trace;

            InitVariables();

            var B = A;

            var n = A.RowCount;
            var ia = (int[])B.ColumnPointers.Clone();
            var ja = (int[])B.RowIndices.Clone();
            var a = (double[])B.Values.Clone();

            int mtype = this.mtype;             
            int nrhs = 1;
            
            int i;

            var sp = System.Diagnostics.Stopwatch.StartNew();

            {
                phase = 11;
                var reslt = pardiso(pt, ref maxfct, ref mnum, ref mtype, ref phase,
                    ref n, a, ia, ja, idum, ref nrhs,
                    iparm, ref msglvl, ddum, ddum, ref error);
            }

            if (trace != null)
                trace.Write(TraceLevel.Info, "mkl analysis took {0} ms", sp.ElapsedMilliseconds);

            sp.Restart();

            if (error != 0)
                throw new Exception(string.Format("MKL error: '{0}' in phase {1}", ErrCodeToString(error), phase));
            {
                phase = 22;
                pardiso(pt, ref maxfct, ref mnum, ref mtype, ref phase,
                    ref n, a, ia, ja, idum, ref nrhs,
                    iparm, ref msglvl, ddum, ddum, ref error);
            }

            if (trace != null)
                trace.Write(TraceLevel.Info, "mkl Numerical factorization took {0} ms", sp.ElapsedMilliseconds);

            if (error != 0)
                throw new Exception(string.Format("MKL error: '{0}' in phase {1}", ErrCodeToString(error), phase));

            sp.Stop();
            
            _isInitialized = true;
        }

        public void Solve(double[] b, double[] x)
        {
            var B = A;

            var trace = Trace;

            var n = A.RowCount;
            var ia = (int[])B.ColumnPointers.Clone();
            var ja = (int[])B.RowIndices.Clone();
            var a = (double[])B.Values.Clone();

            int mtype = this.mtype;

            int nrhs = 1;

            var sp = System.Diagnostics.Stopwatch.StartNew();

            {
                int phase = 33;

                pardiso(pt, ref maxfct, ref mnum, ref mtype, ref phase,
                    ref n, a, ia, ja, idum, ref nrhs,
                    iparm, ref msglvl, b, x, ref error);
            }
            if (trace != null)
                trace.Write(TraceLevel.Info, "mkl solve took {0} ms", sp.ElapsedMilliseconds);

            if (error != 0)
            {
                throw new Exception(string.Format("MKL error: '{0}' in phase {1}", ErrCodeToString(error), phase));
            }   

        }

        ~MklPardisoDirectSPDSolver()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            phase = -1; /* Release internal memory. */

            int mtype = this.mtype;

            int n = 8;
            var ia = new int[0];
            var ja = new int[0];
            var a = new double[0];
            int nrhs = 1;

            pardiso(pt, ref maxfct, ref mnum, ref mtype, ref phase,
                ref n, ddum, ia, ja, idum, ref nrhs,
                iparm, ref msglvl, ddum, ddum, ref error);

            if (error != 0)
                throw new Exception(string.Format("MKL error: {0} in phase {1}", error, phase));
        }

        #region 
        [SuppressUnmanagedCodeSecurity]
        internal sealed class PardisoNative
        {
            private PardisoNative() { }

            [DllImport(@"mkl_rt.dll",
                CallingConvention = CallingConvention.Cdecl,
                 ExactSpelling = true, SetLastError = false)]
            internal static extern int pardiso([In, Out] IntPtr[] handle,
                ref int maxfct, ref int mnum,
                ref int mtype, ref int phase, ref int n,
                [In] double[] a, [In] int[] ia, [In] int[] ja, [In] int[] perm,
                ref int nrhs, [In, Out] int[] iparm, ref int msglvl,
                [In, Out] double[] b, [Out] double[] x, ref int error);
        }

        public static int pardiso(IntPtr[] handle,
            ref int maxfct, ref int mnum,
            ref int mtype, ref int phase, ref int n,
            double[] a, int[] ia, int[] ja, int[] perm,
            ref int nrhs, int[] iparm, ref int msglvl,
            double[] b, double[] x, ref int error)
        {
            return PardisoNative.pardiso(handle,
                ref maxfct, ref mnum, ref mtype, ref phase, ref n,
                a, ia, ja, perm, ref nrhs, iparm, ref msglvl,
                b, x, ref error);
        }

        


        #endregion

        public static string ErrCodeToString(int code)
        {
            //more info: https://software.intel.com/en-us/articles/description-of-pardiso-errors-and-messages

            switch (code)
            {
                case 0:
                    return "no error";
                case -1:
                    return "input inconsistency";
                case -2:
                    return "not enough memory";
                case -3:
                    return "reordering problem";
                case -4:
                    return "zero pivot, numerical factorization or  iterative refinement problem";
                case -5:
                    return "unclassified (internal) error";
                case -6:
                    return "preordering failed (matrix types 11, 13 only)";
                case -7:
                    return "diagonal matrix is singular";
                case -8:
                    return "32 - bit integer overflow problem";
                case -9:
                    return "not enough memory for OOC";
                case -10:
                    return "problems with opening OOC temporary files";
                case -11:
                    return "read/write problems with the OOC data file";
                default:
                    return "unknown error (error code:" + code + ")";
            }
        }
    }
}