
namespace CSparse.Complex.Factorization.SuiteSparse
{
    using CSparse.Interop.Common;
    using CSparse.Interop.SuiteSparse.Cholmod;
    using CSparse.Storage;
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Runtime.InteropServices;

    internal static class CholmodHelper
    {
        public static CompressedColumnStorage<Complex> GetSparseMatrix(CholmodSparse sparse, ref double[] buffer)
        {
            int rows = (int)sparse.nrow;
            int columns = (int)sparse.ncol;

            var matrix = new SparseMatrix(rows, columns);

            var ap = new int[sparse.ncol + 1];

            Marshal.Copy(sparse.p, ap, 0, columns + 1);

            int nnz = ap[columns];

            var ai = new int[nnz];
            var ax = new Complex[nnz];

            Marshal.Copy(sparse.i, ai, 0, nnz);
            CopyArray(2 * (int)sparse.nzmax, sparse.x, matrix.Values, ref buffer);

            matrix.ColumnPointers = ap;
            matrix.RowIndices = ai;
            matrix.Values = ax;

            return matrix;
        }

        public static DenseColumnMajorStorage<Complex> GetDenseMatrix(CholmodDense dense, ref double[] buffer)
        {
            int rows = (int)dense.nrow;
            int columns = (int)dense.ncol;

            var matrix = new DenseMatrix(rows, columns);

            CopyArray(2 * (int)dense.nzmax, dense.x, matrix.Values, ref buffer);

            return matrix;
        }

        public static CholmodDense CreateDense(DenseColumnMajorStorage<Complex> matrix, List<GCHandle> handles)
        {
            var A = new CholmodDense();

            A.nrow = (uint)matrix.RowCount;
            A.ncol = (uint)matrix.ColumnCount;
            A.nzmax = (uint)(matrix.RowCount * matrix.ColumnCount);

            A.dtype = Dtype.Double;
            A.xtype = Xtype.Complex;

            A.x = InteropHelper.Pin(matrix.Values, handles);
            A.z = IntPtr.Zero;
            A.d = (uint)matrix.RowCount; // TODO: cholmod_dense leading dimension?

            return A;
        }

        public static CholmodSparse CreateSparse(CompressedColumnStorage<Complex> matrix, Stype stype, List<GCHandle> handles)
        {
            var A = new CholmodSparse();

            A.nrow = (uint)matrix.RowCount;
            A.ncol = (uint)matrix.ColumnCount;

            A.dtype = Dtype.Double;
            A.xtype = Xtype.Complex;
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

        public static void CopyArray(int count, IntPtr source, Complex[] target, ref double[] buffer)
        {
            if (buffer == null || buffer.Length < count)
            {
                buffer = new double[count];
            }

            Marshal.Copy(source, buffer, 0, count);
            
            count = count / 2;

            for (int i = 0; i < count; i++)
            {
                target[i] = new Complex(buffer[2 * i], buffer[2 * i + 1]);
            }
        }
    }
}
