/*******************************************************************************
!   Copyright (C) 2009 Intel Corporation. All Rights Reserved.
!
!   The information and  material ("Material") provided below is  owned by Intel
!   Corporation  or its  suppliers  or  licensors, and  title  to such  Material
!   remains with Intel Corporation or  its suppliers or licensors.  The Material
!   contains proprietary  information of Intel  or its suppliers  and licensors.
!   The Material is protected by worldwide copyright laws and treaty provisions.
!   No  part  of  the  Material  may  be  used,  copied,  reproduced,  modified,
!   published, uploaded,  posted, transmitted,  distributed or disclosed  in any
!   way without Intel's prior express  written permission.  No license under any
!   patent, copyright or  other intellectual property rights in  the Material is
!   granted  to  or  conferred  upon  you,  either  expressly,  by  implication,
!   inducement,  estoppel or  otherwise.   Any license  under such  intellectual
!   property rights must be express and approved by Intel in writing.
!*******************************************************************************
!   Content : MKL PARDISO C# example
!        http://www.pardiso-project.org/manual/pardiso_sym.c
!*******************************************************************************
*/
/* -------------------------------------------------------------------- */
/* Example program to show the use of the "PARDISO" routine */
/* on symmetric linear systems */
/* -------------------------------------------------------------------- */
/* This program can be downloaded from the following site: */
/* www.pardiso-project.org */
/* */
/* (C) Olaf Schenk, Department of Computer Science, */
/* University of Basel, Switzerland. */
/* Email: olaf.schenk@unibas.ch */
/* -------------------------------------------------------------------- */
using System;
using System.Security;
using System.Runtime.InteropServices;
using mkl;

/**
 * Example showing how to call Intel MKL PARDISO
 * to solve a symmetric sparse linear system of equations .
 */

namespace BriefFiniteElementNet.PardisoThing
{


    public class test_pardiso
    {
        private test_pardiso()
        {
        }

        public static int Main(string[] args)
        {
            /* Matrix data. */
            int n = 8;
            int[] ia /*[9]*/ = new int[] {1, 5, 8, 10, 12, 15, 17, 18, 19};
            int[] ja /*[18]*/ = new int[]
            {
                1, 3, 6, 7,
                2, 3, 5,
                3, 8,
                4, 7,
                5, 6, 7,
                6, 8,
                7,
                8
            };
            double[] a /*[18]*/ = new double[]
            {
                7.0, 1.0, 2.0, 7.0,
                -4.0, 8.0, 2.0,
                1.0, 5.0,
                7.0, 9.0,
                5.0, 1.0, 5.0,
                -1.0, 5.0,
                11.0,
                5.0
            };
            int mtype = -2; /* Real symmetric matrix */
            /* RHS and solution vectors. */
            double[] b = new double[8];
            double[] x = new double[8];
            int nrhs = 1; /* Number of right hand sides. */
            /* Internal solver memory pointer pt, */
            /* 32-bit: int pt[64]; 64-bit: long int pt[64] */
            /* or void *pt[64] should be OK on both architectures */
            /* void *pt[64]; */
            IntPtr[] pt = new IntPtr[64];
            /* Pardiso control parameters. */
            int[] iparm = new int[64];
            int maxfct, mnum, phase, error, msglvl;
            /* Auxiliary variables. */
            int i;
            double[] ddum = new double[1]; /* Double dummy */
            int[] idum = new int[1]; /* Integer dummy. */
            /* ----------------------------------------------------------------- */
            /* .. Setup Pardiso control parameters. */
            /* ----------------------------------------------------------------- */
            for (i = 0; i < 64; i++)
            {
                iparm[i] = 0;
            }
            iparm[0] = 1; /* No solver default */
            iparm[1] = 2; /* Fill-in reordering from METIS */
            /* Numbers of processors, value of OMP_NUM_THREADS */
            iparm[2] = 1;
            iparm[3] = 0; /* No iterative-direct algorithm */
            iparm[4] = 0; /* No user fill-in reducing permutation */
            iparm[5] = 0; /* Write solution into x */
            iparm[6] = 0; /* Not in use */
            iparm[7] = 2; /* Max numbers of iterative refinement steps */
            iparm[8] = 0; /* Not in use */
            iparm[9] = 13; /* Perturb the pivot elements with 1E-13 */
            iparm[10] = 1; /* Use nonsymmetric permutation and scaling MPS */
            iparm[11] = 0; /* Not in use */
            iparm[12] = 0; /* Maximum weighted matching algorithm is switched-off
                        * (default for symmetric). Try iparm[12] = 1 in case of
                        *  inappropriate accuracy */
            iparm[13] = 0; /* Output: Number of perturbed pivots */
            iparm[14] = 0; /* Not in use */
            iparm[15] = 0; /* Not in use */
            iparm[16] = 0; /* Not in use */
            iparm[17] = -1; /* Output: Number of nonzeros in the factor LU */
            iparm[18] = -1; /* Output: Mflops for LU factorization */
            iparm[19] = 0; /* Output: Numbers of CG Iterations */
            maxfct = 1; /* Maximum number of numerical factorizations. */
            mnum = 1; /* Which factorization to use. */
            msglvl = 1; /* Print statistical information in file */
            error = 0; /* Initialize error flag */
            /* ----------------------------------------------------------------- */
            /* .. Initialize the internal solver memory pointer. This is only */
            /* necessary for the FIRST call of the PARDISO solver. */
            /* ----------------------------------------------------------------- */
            for (i = 0; i < 64; i++)
            {
                pt[i] = IntPtr.Zero;
            }
            /* ----------------------------------------------------------------- */
            /* .. Reordering and Symbolic Factorization. This step also allocates */
            /* all memory that is necessary for the factorization. */
            /* ----------------------------------------------------------------- */
            phase = 11;
            Pardiso.pardiso(pt, ref maxfct, ref mnum, ref mtype, ref phase,
                ref n, a, ia, ja, idum, ref nrhs,
                iparm, ref msglvl, ddum, ddum, ref error);
            if (error != 0)
            {
                Console.WriteLine("\nERROR during symbolic factorization: " + error);
                return(1);
            }
            Console.Write("\nReordering completed ... ");
            Console.Write("\nNumber of nonzeros in factors = " + iparm[17]);
            Console.WriteLine("\nNumber of factorization MFLOPS = " + iparm[18]);
            /* ----------------------------------------------------------------- */
            /* .. Numerical factorization. */
            /* ----------------------------------------------------------------- */
            phase = 22;
            Pardiso.pardiso(pt, ref maxfct, ref mnum, ref mtype, ref phase,
                ref n, a, ia, ja, idum, ref nrhs,
                iparm, ref msglvl, ddum, ddum, ref error);
            if (error != 0)
            {
                Console.WriteLine("\nERROR during numerical factorization: " + error);
                return(2);
            }
            Console.WriteLine("\nFactorization completed ... ");
            /* ----------------------------------------------------------------- */
            /* .. Back substitution and iterative refinement. */
            /* ----------------------------------------------------------------- */
            phase = 33;
            iparm[7] = 2; /* Max numbers of iterative refinement steps. */
            /* Set right hand side to one. */
            for (i = 0; i < n; i++)
            {
                b[i] = 1;
            }
            Pardiso.pardiso(pt, ref maxfct, ref mnum, ref mtype, ref phase,
                ref n, a, ia, ja, idum, ref nrhs,
                iparm, ref msglvl, b, x, ref error);
            if (error != 0)
            {
                Console.WriteLine("\nERROR during solution: " + error);
                return(3);
            }
            Console.WriteLine("\nSolve completed ... ");
            Console.WriteLine("\nThe solution of the system is: ");
            for (i = 0; i < n; i++)
            {
                Console.Write("\n x [" + i + "] = " + x[i]);
            }
            Console.WriteLine();
            /* ----------------------------------------------------------------- */
            /* .. Termination and release of memory. */
            /* ----------------------------------------------------------------- */
            phase = -1; /* Release internal memory. */
            Pardiso.pardiso(pt, ref maxfct, ref mnum, ref mtype, ref phase,
                ref n, ddum, ia, ja, idum, ref nrhs,
                iparm, ref msglvl, ddum, ddum, ref error);
            Console.WriteLine("TEST PASSED");
            Console.WriteLine();
            return 0;
        }

    }

   


}

namespace mkl
{
    /** Pardiso wrappers */
    public sealed class Pardiso
    {
        private Pardiso()
        {
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
    }

    /** Pardiso native declarations */
    [SuppressUnmanagedCodeSecurity]
    internal sealed class PardisoNative
    {
        private PardisoNative()
        {
        }

        [DllImport("mkl_rt.dll", CallingConvention = CallingConvention.Cdecl,
            ExactSpelling = true, SetLastError = false)]
        internal static extern int pardiso([In, Out] IntPtr[] handle,
            ref int maxfct, ref int mnum,
            ref int mtype, ref int phase, ref int n,
            [In] double[] a, [In] int[] ia, [In] int[] ja, [In] int[] perm,
            ref int nrhs, [In, Out] int[] iparm, ref int msglvl,
            [In, Out] double[] b, [Out] double[] x, ref int error);
    }
}