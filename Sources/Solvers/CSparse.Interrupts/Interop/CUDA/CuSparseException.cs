
namespace CSparse.Interop.CUDA
{
    using System;

    public class CuSparseException : Exception
    {
        public SparseStatus Status { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CuSparseException"/> class.
        /// </summary>
        /// <param name="status"></param>
        public CuSparseException(SparseStatus status)
        {
            Status = status;
        }
    }
}