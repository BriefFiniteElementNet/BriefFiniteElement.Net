
namespace CSparse.Complex.Factorization.SuiteSparse
{
    using CSparse.Interop.SuiteSparse.Umfpack;
    using System;
    using System.Numerics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// UMFPACK wrapper.
    /// </summary>
    public class Umfpack : UmfpackContext<Complex>
    {
        /// <summary>
        /// Initializes a new instance of the Umfpack class.
        /// </summary>
        public Umfpack(SparseMatrix matrix)
            : base(matrix)
        {
        }

        protected override void DoInitialize()
        {
            NativeMethods.umfpack_zi_defaults(control.Raw);
        }

        protected override int DoSymbolic()
        {
            var h = GCHandle.Alloc(matrix.Values, GCHandleType.Pinned);

            try
            {
                return NativeMethods.umfpack_zi_symbolic(matrix.RowCount, matrix.ColumnCount,
                    matrix.ColumnPointers, matrix.RowIndices, h.AddrOfPinnedObject(), IntPtr.Zero,
                    out symbolic, control.Raw, info.Raw);
            }
            finally
            {
                h.Free();
            }
        }

        protected override int DoNumeric()
        {
            var h = GCHandle.Alloc(matrix.Values, GCHandleType.Pinned);

            try
            {
                return NativeMethods.umfpack_zi_numeric(matrix.ColumnPointers, matrix.RowIndices,
                    h.AddrOfPinnedObject(), IntPtr.Zero, symbolic, out numeric, control.Raw, info.Raw);
            }
            finally
            {
                h.Free();
            }
        }

        protected override int DoFactorize()
        {
            var h = GCHandle.Alloc(matrix.Values, GCHandleType.Pinned);

            try
            {
                int status = NativeMethods.umfpack_zi_symbolic(matrix.RowCount, matrix.ColumnCount,
                    matrix.ColumnPointers, matrix.RowIndices, h.AddrOfPinnedObject(), IntPtr.Zero,
                    out symbolic, control.Raw, info.Raw);

                if (status != Constants.UMFPACK_OK)
                {
                    return status;
                }

                return NativeMethods.umfpack_zi_numeric(matrix.ColumnPointers, matrix.RowIndices,
                    h.AddrOfPinnedObject(), IntPtr.Zero, symbolic, out numeric, control.Raw, info.Raw);
            }
            finally
            {
                h.Free();
            }
        }

        protected override int DoSolve(UmfpackSolve sys, Complex[] input, Complex[] result)
        {
            var ha = GCHandle.Alloc(matrix.Values, GCHandleType.Pinned);
            var hx = GCHandle.Alloc(result, GCHandleType.Pinned);
            var hb = GCHandle.Alloc(input, GCHandleType.Pinned);

            try
            {
                return NativeMethods.umfpack_zi_solve((int)sys, matrix.ColumnPointers, matrix.RowIndices,
                    ha.AddrOfPinnedObject(), IntPtr.Zero,
                    hx.AddrOfPinnedObject(), IntPtr.Zero,
                    hb.AddrOfPinnedObject(), IntPtr.Zero, numeric, control.Raw, info.Raw);
            }
            finally
            {
                ha.Free();
                hx.Free();
                hb.Free();
            }
        }

        protected override int DoSolve(UmfpackSolve sys, Complex[] input, Complex[] result, int[] wi, double[] wx)
        {
            var ha = GCHandle.Alloc(matrix.Values, GCHandleType.Pinned);
            var hx = GCHandle.Alloc(result, GCHandleType.Pinned);
            var hb = GCHandle.Alloc(input, GCHandleType.Pinned);

            try
            {
                return NativeMethods.umfpack_zi_wsolve((int)sys, matrix.ColumnPointers, matrix.RowIndices,
                    ha.AddrOfPinnedObject(), IntPtr.Zero,
                    hx.AddrOfPinnedObject(), IntPtr.Zero,
                    hb.AddrOfPinnedObject(), IntPtr.Zero, numeric, control.Raw, info.Raw, wi, wx);
            }
            finally
            {
                ha.Free();
                hx.Free();
                hb.Free();
            }
        }

        protected override double[] CreateWorkspace(int n, bool refine)
        {
            return new double[refine ? 10 * n : 4 * n];
        }

        protected override void Dispose(bool disposing)
        {
            if (symbolic != IntPtr.Zero)
            {
                NativeMethods.umfpack_zi_free_symbolic(ref symbolic);
            }

            if (numeric != IntPtr.Zero)
            {
                NativeMethods.umfpack_zi_free_numeric(ref numeric);
            }
        }
    }
}
