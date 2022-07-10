
namespace CSparse.Interop.CUDA
{
    using System;

    public class CuSolverException : Exception
    {
        public SolverStatus Status { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CuSolverException"/> class.
        /// </summary>
        /// <param name="status"></param>
        public CuSolverException(SolverStatus status)
        {
            Status = status;
        }
    }
}