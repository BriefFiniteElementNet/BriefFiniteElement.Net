
namespace CSparse.Complex.Factorization.SuiteSparse
{
    using CSparse.Interop.Common;
    using CSparse.Interop.SuiteSparse.CXSparse;
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Runtime.InteropServices;

    public class CXSparseQR : CXSparseContext<Complex>
    {
        cs AT;

        double[] beta;

        /// <summary>
        /// Initializes a new instance of the SuperLU class.
        /// </summary>
        public CXSparseQR(SparseMatrix matrix, ColumnOrdering ordering)
            : base(matrix, ordering)
        {
        }

        public override void Solve(Complex[] input, Complex[] result)
        {
            if (!factorized && DoFactorize() != 0)
            {
                throw new Exception(); // TODO: exception
            }

            var m = matrix.RowCount;
            var n = matrix.ColumnCount;

            var h = new List<GCHandle>();

            var b = InteropHelper.Pin(input, h);
            var x = InteropHelper.Pin(result, h);
            var t = InteropHelper.Pin(w, h);

            try
            {
                // x = b(p)
                if (NativeMethods.cs_ci_ipvec(N.pinv, b, t, n) == 0) return;

                // x = L\x
                if (NativeMethods.cs_ci_lsolve(N.L, t) == 0) return;

                // x = U\x
                if (NativeMethods.cs_ci_ltsolve(N.U, t) == 0) return;

                // b(q) = x 
                if (NativeMethods.cs_ci_pvec(S.q, t, x, n) == 0) return;
            }
            finally
            {
                InteropHelper.Free(h);
            }


            if (m >= n)
            {
                // x(0:m-1) = b(p(0:m-1)
                NativeMethods.cs_ci_ipvec(S.pinv, b, x, m);

                // apply Householder refl. to x
                for (int k = 0; k < n; k++)
                {
                    NativeMethods.cs_ci_happly(N.L, k, beta[k], x);
                }
                // x = R\x
                NativeMethods.cs_ci_usolve(N.U, x);

                // b(q(0:n-1)) = x(0:n-1)
                NativeMethods.cs_ci_ipvec(S.q, x, b, n);
            }
            else
            {
                // x(q(0:m-1)) = b(0:m-1)
                NativeMethods.cs_ci_pvec(S.q, b, x, m);

                // x = R'\x
                NativeMethods.cs_ci_utsolve(N.U, x);

                // apply Householder refl. to x
                for (int k = m - 1; k >= 0; k--)
                {
                    NativeMethods.cs_ci_happly(N.L, k, beta[k], x);
                }

                // b(0:n-1) = x(p(0:n-1))
                NativeMethods.cs_ci_pvec(S.pinv, x, b, n);
            }
        }

        protected override int DoFactorize()
        {
            var h = new List<GCHandle>();
            var A = CreateSparse(matrix, h);

            int m = matrix.RowCount;
            int n = matrix.ColumnCount;

            try
            {
                int order = (int)ordering;

                if (m >= n)
                {
                    // ordering and symbolic analysis
                    var pS = NativeMethods.cs_ci_sqr(order, ref A, 1);

                    if (pS == IntPtr.Zero) return -1;

                    S = Marshal.PtrToStructure<css>(pS);

                    // numeric QR factorization
                    var pN = NativeMethods.cs_ci_qr(ref A, ref S);

                    if (pN == IntPtr.Zero) return -1;

                    N = Marshal.PtrToStructure<csn>(pN);

                    beta = new double[n];

                    // Copy beta values for later access.
                    Marshal.Copy(N.B, beta, 0, n);
                }
                else
                {
                    // Ax=b is underdetermined
                    var pAT = NativeMethods.cs_ci_transpose(ref A, 1);

                    AT = Marshal.PtrToStructure<cs>(pAT);

                    // ordering and symbolic analysis
                    var pS = NativeMethods.cs_ci_sqr(order, ref AT, 1);

                    if (pS == IntPtr.Zero) return -1;

                    S = Marshal.PtrToStructure<css>(pS);

                    // numeric QR factorization of A'
                    var pN = NativeMethods.cs_ci_qr(ref AT, ref S);

                    if (pN == IntPtr.Zero) return -1;

                    N = Marshal.PtrToStructure<csn>(pN);

                    beta = new double[m];

                    // Copy beta values for later access.
                    Marshal.Copy(N.B, beta, 0, m);
                }

                if (w.Length < S.m2)
                {
                    // Fix workspace length, if neccessary.
                    w = new Complex[S.m2];
                }
            }
            finally
            {
                InteropHelper.Free(h);
            }

            return 0;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (AT.x != IntPtr.Zero)
            {
                cs_spfree(ref AT);
                AT.x = IntPtr.Zero;
            }
        }
    }
}
