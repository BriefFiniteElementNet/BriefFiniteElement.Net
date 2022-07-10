
namespace CSparse.Double.Factorization
{
    using CSparse.Interop.Common;
    using CSparse.Interop.SuperLU;
    using CSparse.Storage;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class SuperLU : SuperLUContext<double>
    {
        /// <summary>
        /// Initializes a new instance of the SuperLU class.
        /// </summary>
        public SuperLU(SparseMatrix matrix)
            : base(matrix)
        {
        }

        public override void Solve(double[] input, double[] result)
        {
            Solve(new DenseMatrix(matrix.RowCount, 1, input), new DenseMatrix(matrix.ColumnCount, 1, result));
        }

        protected override int DoFactorize()
        {
            int info = 0;

            var mem_usage = new mem_usage();
            var stat = new SuperLUStat();

            int m = A.nrow;
            int n = A.ncol;

            var h = new List<GCHandle>();

            // Create matrix with ncol = 0 to indicate not to solve the system.
            var B = CreateEmptyDense(Dtype.SLU_D, m, h);
            var X = CreateEmptyDense(Dtype.SLU_D, n, h);

            try
            {
                var o = options.Raw;

                NativeMethods.StatInit(ref stat);

                NativeMethods.dgssvx(ref o, ref A, perm_c, perm_r, etree, equed,
                    (double[])R, (double[])C, ref L, ref U,
                    IntPtr.Zero, 0, ref B, ref X, out rpg, out rcond, null, null,
                    ref glu, ref mem_usage, ref stat, out info);

                NativeMethods.StatFree(ref stat);

                options.Raw.Fact = Constants.FACTORED;
            }
            finally
            {
                InteropHelper.Free(h);
            }
            
            return info;
        }

        protected override int DoSolve(DenseColumnMajorStorage<double> input, DenseColumnMajorStorage<double> result)
        {
            int info = 0;

            var mem_usage = new mem_usage();
            var stat = new SuperLUStat();
            
            int nrhs = input.ColumnCount;

            var ferr = new double[nrhs];
            var berr = new double[nrhs];
            
            var h = new List<GCHandle>();

            var B = CreateDense(Dtype.SLU_D, input, h);
            var X = CreateDense(Dtype.SLU_D, result, h);

            try
            {
                var o = options.Raw;

                NativeMethods.StatInit(ref stat);

                NativeMethods.dgssvx(ref o, ref A, perm_c, perm_r, etree, equed,
                    (double[])R, (double[])C, ref L, ref U,
                    IntPtr.Zero, 0, ref B, ref X, out rpg, out rcond, ferr, berr,
                    ref glu, ref mem_usage, ref stat, out info);

                NativeMethods.StatFree(ref stat);
            }
            finally
            {
                InteropHelper.Free(h);
            }
            
            return info;
        }

        protected override SuperMatrix CreateSparse(CompressedColumnStorage<double> matrix, List<GCHandle> handles)
        {
            var A = new SuperMatrix();

            A.nrow = matrix.RowCount;
            A.ncol = matrix.ColumnCount;

            A.Dtype = Dtype.SLU_D;
            A.Mtype = Mtype.SLU_GE;
            A.Stype = Stype.SLU_NC;

            var store = new NCformat();

            var ap = matrix.ColumnPointers;

            store.nnz = matrix.NonZerosCount;
            store.colptr = InteropHelper.Pin(matrix.ColumnPointers, handles);
            store.rowind = InteropHelper.Pin(matrix.RowIndices, handles);
            store.nzval = InteropHelper.Pin(matrix.Values, handles);

            A.Store = InteropHelper.Pin(store, handles);

            return A;
        }

        protected override object CreateArray(int size)
        {
            return new double[size];
        }
    }
}
