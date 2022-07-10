
namespace CSparse.Double.Factorization.SuiteSparse
{
    using CSparse.Interop.SuiteSparse.Cholmod;
    using CSparse.Interop.Common;
    using CSparse.Storage;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    internal static class CholmodHelper
    {
        public static CompressedColumnStorage<double> GetSparseMatrix(CholmodSparse sparse)
        {
            int rows = (int)sparse.nrow;
            int columns = (int)sparse.ncol;

            var matrix = new SparseMatrix(rows, columns);

            var ap = new int[sparse.ncol + 1];

            Marshal.Copy(sparse.p, ap, 0, columns + 1);

            int nnz = ap[columns];

            var ai = new int[nnz];
            var ax = new double[nnz];

            Marshal.Copy(sparse.i, ai, 0, nnz);
            Marshal.Copy(sparse.x, ax, 0, nnz);

            matrix.ColumnPointers = ap;
            matrix.RowIndices = ai;
            matrix.Values = ax;

            return matrix;
        }

        public static DenseColumnMajorStorage<double> GetDenseMatrix(CholmodDense dense)
        {
            int rows = (int)dense.nrow;
            int columns = (int)dense.ncol;

            var matrix = new DenseMatrix(rows, columns);

            Marshal.Copy(dense.x, matrix.Values, 0, rows * columns);

            return matrix;
        }

        public static CholmodDense CreateDense(DenseColumnMajorStorage<double> matrix, List<GCHandle> handles)
        {
            var A = new CholmodDense();

            A.nrow = (uint)matrix.RowCount;
            A.ncol = (uint)matrix.ColumnCount;
            A.nzmax = (uint)(matrix.RowCount * matrix.ColumnCount);

            A.dtype = Dtype.Double;
            A.xtype = Xtype.Real;

            A.x = InteropHelper.Pin(matrix.Values, handles);
            A.z = IntPtr.Zero;
            A.d = (uint)matrix.RowCount; // TODO: cholmod_dense leading dimension?

            return A;
        }

        public static CholmodSparse CreateSparse(CompressedColumnStorage<double> matrix, Stype stype, List<GCHandle> handles)
        {
            var A = new CholmodSparse();

            A.nrow = (uint)matrix.RowCount;
            A.ncol = (uint)matrix.ColumnCount;

            A.dtype = Dtype.Double;
            A.xtype = Xtype.Real;
            A.stype = stype;

            A.itype = Constants.CHOLMOD_INT;

            A.nzmax = (uint)matrix.Values.Length;
            A.packed = 1;
            A.sorted = 1;

            A.nz = IntPtr.Zero;
            A.p = InteropHelper.Pin(matrix.ColumnPointers, handles);
            A.i = InteropHelper.Pin(matrix.RowIndices, handles);
            A.x = InteropHelper.Pin(matrix.Values, handles);
            A.z = IntPtr.Zero;

            return A;
        }
    }
}
