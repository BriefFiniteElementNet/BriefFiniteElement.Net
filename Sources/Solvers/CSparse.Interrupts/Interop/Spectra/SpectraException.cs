namespace CSparse.Interop.Spectra
{
    using System;

    /// <summary>
    /// Spectra exception.
    /// </summary>
    public class SpectraException : Exception
    {
        public int ErrorCode { get; private set; }

        public SpectraException(int error)
            : base(GetErrorMessage(error))
        {
            ErrorCode = error;
        }

        public static string GetErrorMessage(int error)
        {
            switch (error)
            {
                case NO_ERRORS:
                    return string.Empty;
                case NOT_COMPUTED:
                    return "Computation has not been conducted. Call compute() member function.";
                case NOT_CONVERGING:
                    return "Some eigenvalues did not converge.";
                case NUMERICAL_ISSUE:
                    return "Cholesky decomposition failed, the matrix is not positive definite.";
                case EXCEPTION_STD:
                    return "C++ std::exception was thrown.";
                case EXCEPTION_UNKNOWN:
                    return "C++ unkown exception occurred.";
                default:
                    return "There is something wrong (" + error + ")";
            }
        }

        #region Error codes

        // Innocuous error type.

        private const int NO_ERRORS = 0;

        // Errors in parameter definitions.

        private const int NOT_COMPUTED = 1;
        private const int NOT_CONVERGING = 2;
        private const int NUMERICAL_ISSUE = 3;
        
        // Other severe errors.
        
        private const int EXCEPTION_STD = -1000;
        private const int EXCEPTION_UNKNOWN = -1001;

        #endregion
    }
}
