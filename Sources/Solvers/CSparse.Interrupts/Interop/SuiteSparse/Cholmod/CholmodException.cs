namespace CSparse.Interop.SuiteSparse.Cholmod
{
    using System;

    /// <summary>
    /// CHOLMOD exception.
    /// </summary>
    public class CholmodException : Exception
    {
        /// <summary>
        /// Zero means success, negative means a fatal error, positive is a warning.
        /// </summary>
        public int ErrorCode { get; private set; }

        public CholmodException(int error)
            : base(GetErrorMessage(error))
        {
            ErrorCode = error;
        }

        public static string GetErrorMessage(int error)
        {
            switch (error)
            {
                case CHOLMOD_OK:
                    return string.Empty;
                case CHOLMOD_NOT_INSTALLED:
                    return "Failure: method not installed.";
                case CHOLMOD_OUT_OF_MEMORY:
                    return "Failure: out of memory.";
                case CHOLMOD_TOO_LARGE:
                    return "Failure: integer overflow occured.";
                case CHOLMOD_INVALID:
                    return "Failure: invalid input.";
                case CHOLMOD_GPU_PROBLEM:
                    return "Failure: GPU fatal error.";
                case CHOLMOD_NOT_POSDEF:
                    return "Warning: matrix not pos. definite.";
                case CHOLMOD_DSMALL:
                    return "Warning: D for LDL'  or diag(L) or LL' has tiny absolute value.";
                default:
                    return "There is something wrong.";
            }
        }

        #region Error codes
        
        private const int CHOLMOD_OK = 0;
        private const int CHOLMOD_NOT_INSTALLED = -1;
        private const int CHOLMOD_OUT_OF_MEMORY = -2;
        private const int CHOLMOD_TOO_LARGE = -3;
        private const int CHOLMOD_INVALID = -4;
        private const int CHOLMOD_GPU_PROBLEM = -5;
        private const int CHOLMOD_NOT_POSDEF = 1;
        private const int CHOLMOD_DSMALL = 2;

        #endregion
    }
}
