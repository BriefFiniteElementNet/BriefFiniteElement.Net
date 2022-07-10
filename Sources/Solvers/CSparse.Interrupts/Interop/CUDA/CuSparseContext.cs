
namespace CSparse.Interop.CUDA
{
    using CSparse.Interop.Common;
    using CSparse.Storage;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The CuSparse context represents a sparse matrix in device memory (storage format CSR).
    /// </summary>
    public class CuSparseContext<T> : IDisposable
        where T : struct, IEquatable<T>, IFormattable
    {
        IntPtr _p;
        IntPtr _matDescr;

        // Matrix stored in device memory.

        IntPtr d_ap; // Column pointers
        IntPtr d_ai; // Row indices
        IntPtr d_ax; // Values

        public IntPtr MatrixDescriptor { get { return _matDescr; } }

        public IntPtr ColumnPointers { get { return d_ap; } }

        public IntPtr RowIndices { get { return d_ai; } }

        public IntPtr Values { get { return d_ax; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="CuSparseContext{T}"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="CudaStream"/>.</param>
        /// <param name="A">The sparse matrix.</param>
        /// <param name="type">The matrix type.</param>
        /// <param name="transpose">A value indicating, whether the storage should be transposed.</param>
        public CuSparseContext(CudaStream stream, CompressedColumnStorage<T> A, MatrixType type, bool transpose)
        {
            Check(NativeMethods.cusparseCreate(ref _p));
            Check(NativeMethods.cusparseSetStream(_p, stream.Pointer));
            Check(NativeMethods.cusparseCreateMatDescr(ref _matDescr));
            Check(NativeMethods.cusparseSetMatType(_matDescr, type));
            Check(NativeMethods.cusparseSetMatIndexBase(_matDescr, IndexBase.Zero));

            var sizeT = Marshal.SizeOf(typeof(T));

            int rows = A.RowCount;
            int nnz = A.NonZerosCount;

            Cuda.Malloc(ref d_ap, sizeof(int) * (rows + 1));
            Cuda.Malloc(ref d_ai, sizeof(int) * nnz);
            Cuda.Malloc(ref d_ax, sizeT * nnz);

            var handles = new List<GCHandle>();

            try
            {
                // Convert storage to CSR format.
                var C = transpose ? A.Transpose(true) : A;

                var h_ap = InteropHelper.Pin(C.ColumnPointers, handles);
                var h_ai = InteropHelper.Pin(C.RowIndices, handles);
                var h_ax = InteropHelper.Pin(C.Values, handles);

                Cuda.CopyToDevice(d_ap, h_ap, sizeof(int) * (rows + 1));
                Cuda.CopyToDevice(d_ai, h_ai, sizeof(int) * nnz);
                Cuda.CopyToDevice(d_ax, h_ax, sizeT * nnz);
            }
            finally
            {
                InteropHelper.Free(handles);
            }
        }

        ~CuSparseContext()
        {
            Dispose(false);
        }

        private void Check(SparseStatus status)
        {
            if (status != SparseStatus.Success)
            {
                throw new CuSparseException(status);
            }
        }

        #region IDisposable

        // See https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose

        bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposed) return;

            // Free unmanaged objects.

            if (_p != IntPtr.Zero)
            {
                Check(NativeMethods.cusparseDestroy(_p));
                _p = IntPtr.Zero;
            }

            if (_matDescr != IntPtr.Zero)
            {
                Check(NativeMethods.cusparseDestroyMatDescr(_matDescr));
                _matDescr = IntPtr.Zero;
            }

            if (d_ap != IntPtr.Zero) Cuda.Free(d_ap);
            if (d_ai != IntPtr.Zero) Cuda.Free(d_ai);
            if (d_ax != IntPtr.Zero) Cuda.Free(d_ax);

            disposed = true;
        }

        #endregion
    }
}
