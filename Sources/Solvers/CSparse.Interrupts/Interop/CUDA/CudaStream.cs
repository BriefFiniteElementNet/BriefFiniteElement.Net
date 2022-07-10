
namespace CSparse.Interop.CUDA
{
    using System;

    public class CudaStream : IDisposable
    {
        IntPtr _p;

        public IntPtr Pointer { get { return _p; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="CudaStream"/> class.
        /// </summary>
        public CudaStream()
        {
            Check(NativeMethods.cudaStreamCreate(ref _p));
        }

        ~CudaStream()
        {
            Dispose(false);
        }

        private void Check(CudaResult result)
        {
            if (result != CudaResult.Success)
            {
                throw new CudaException(result);
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            // Free unmanaged objects.

            if (_p != IntPtr.Zero)
            {
                Check(NativeMethods.cudaStreamDestroy(_p));
                _p = IntPtr.Zero;
            }

            disposed = true;
        }

        #endregion
    }
}
