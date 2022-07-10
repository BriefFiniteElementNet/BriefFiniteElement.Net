
namespace CSparse.Double.Factorization.SuiteSparse
{
    using CSparse.Interop.SuiteSparse.Umfpack;
    using System;

    /// <summary>
    /// UMFPACK wrapper.
    /// </summary>
    public class Umfpack : UmfpackContext<double>
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
            NativeMethods.umfpack_di_defaults(control.Raw);
        }

        protected override int DoSymbolic()
        {
            return NativeMethods.umfpack_di_symbolic(matrix.RowCount, matrix.ColumnCount,
                matrix.ColumnPointers, matrix.RowIndices, matrix.Values,
                out symbolic, control.Raw, info.Raw);
        }

        protected override int DoNumeric()
        {
            return NativeMethods.umfpack_di_numeric(matrix.ColumnPointers, matrix.RowIndices, matrix.Values,
                symbolic, out numeric, control.Raw, info.Raw);
        }

        protected override int DoFactorize()
        {
            int status = NativeMethods.umfpack_di_symbolic(matrix.RowCount, matrix.ColumnCount,
                matrix.ColumnPointers, matrix.RowIndices, matrix.Values,
                out symbolic, control.Raw, info.Raw);

            if (status != Constants.UMFPACK_OK)
            {
                return status;
            }

            return NativeMethods.umfpack_di_numeric(matrix.ColumnPointers, matrix.RowIndices, matrix.Values,
                symbolic, out numeric, control.Raw, info.Raw);
        }

        protected override int DoSolve(UmfpackSolve sys, double[] input, double[] result)
        {
            return NativeMethods.umfpack_di_solve((int)sys, matrix.ColumnPointers, matrix.RowIndices, matrix.Values,
                result, input, numeric, control.Raw, info.Raw);
        }

        protected override int DoSolve(UmfpackSolve sys, double[] input, double[] result, int[] wi, double[] wx)
        {
            return NativeMethods.umfpack_di_wsolve((int)sys, matrix.ColumnPointers, matrix.RowIndices, matrix.Values,
                result, input, numeric, control.Raw, info.Raw, wi, wx);
        }

        protected override double[] CreateWorkspace(int n, bool refine)
        {
            return new double[refine ? 5 * n : n];
        }

        protected override void Dispose(bool disposing)
        {
            if (symbolic != IntPtr.Zero)
            {
                NativeMethods.umfpack_di_free_symbolic(ref symbolic);
            }

            if (numeric != IntPtr.Zero)
            {
                NativeMethods.umfpack_di_free_numeric(ref numeric);
            }
        }
    }
}
