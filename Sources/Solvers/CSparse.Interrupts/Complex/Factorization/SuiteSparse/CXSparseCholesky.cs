
namespace CSparse.Complex.Factorization.SuiteSparse
{
    using CSparse.Interop.Common;
    using CSparse.Interop.SuiteSparse.CXSparse;
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Runtime.InteropServices;

    public class CXSparseCholesky : CXSparseContext<Complex>
    {
        /// <summary>
        /// Initializes a new instance of the SuperLU class.
        /// </summary>
        public CXSparseCholesky(SparseMatrix matrix, ColumnOrdering ordering)
            : base(matrix, ordering)
        {
        }

        public override void Solve(Complex[] input, Complex[] result)
        {
            if (!factorized && DoFactorize() != 0)
            {
                throw new Exception(); // TODO: exception
            }

            var n = matrix.RowCount;

            var h = new List<GCHandle>();

            var b = InteropHelper.Pin(input, h);
            var x = InteropHelper.Pin(result, h);
            var t = InteropHelper.Pin(w, h);

            try
            {
                // x = P*b
                if (NativeMethods.cs_ci_ipvec(S.pinv, b, t, n) == 0) return;

                // x = L\x
                if (NativeMethods.cs_ci_lsolve(N.L, t) == 0) return;

                // x = L'\x
                if (NativeMethods.cs_ci_ltsolve(N.L, t) == 0) return;

                // b = P'*x
                if (NativeMethods.cs_ci_pvec(S.pinv, t, x, n) == 0) return;
            }
            finally
            {
                InteropHelper.Free(h);
            }
        }

        protected override int DoFactorize()
        {
            var h = new List<GCHandle>();
            var A = CreateSparse(matrix, h);

            try
            {
                int order = (int)ordering;

                // ordering and symbolic analysis
                var pS = NativeMethods.cs_ci_schol(order, ref A);

                if (pS == IntPtr.Zero)
                {
                    return -1;
                }

                S = Marshal.PtrToStructure<css>(pS);

                // numeric Cholesky factorization
                var pN = NativeMethods.cs_ci_chol(ref A, ref S);

                if (pN == IntPtr.Zero)
                {
                    return -1;
                }

                N = Marshal.PtrToStructure<csn>(pN);
            }
            finally
            {
                InteropHelper.Free(h);
            }

            return 0;
        }
    }
}
