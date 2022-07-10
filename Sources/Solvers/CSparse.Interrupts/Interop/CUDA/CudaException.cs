
namespace CSparse.Interop.CUDA
{
    using System;

    public class CudaException : Exception
    {
        public CudaResult Result { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CudaException"/> class.
        /// </summary>
        /// <param name="result"></param>
        public CudaException(CudaResult result)
        {
            Result = result;
        }
    }
}
