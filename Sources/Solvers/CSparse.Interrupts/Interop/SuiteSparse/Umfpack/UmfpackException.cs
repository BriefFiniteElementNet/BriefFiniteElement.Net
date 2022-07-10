namespace CSparse.Interop.SuiteSparse.Umfpack
{
    using System;

    /// <summary>
    /// UMFPACK exception.
    /// </summary>
    public class UmfpackException : Exception
    {
        /// <summary>
        /// Zero means success, negative means a fatal error, positive is a warning.
        /// </summary>
        public int ErrorCode { get; private set; }

        public UmfpackException(int error)
            : base(GetErrorMessage(error))
        {
            ErrorCode = error;
        }

        public static string GetErrorMessage(int error)
        {
            switch (error)
            {
                case UMFPACK_OK:
                    return string.Empty;
                case UMFPACK_WARNING_singular_matrix:
                    return "WARNING: matrix is singular";
                case UMFPACK_ERROR_out_of_memory:
                    return "ERROR: out of memory";
                case UMFPACK_ERROR_invalid_Numeric_object:
                    return "ERROR: Numeric object is invalid";
                case UMFPACK_ERROR_invalid_Symbolic_object:
                    return "ERROR: Symbolic object is invalid";
                case UMFPACK_ERROR_argument_missing:
                    return "ERROR: required argument(s) missing";
                case UMFPACK_ERROR_n_nonpositive:
                    return "ERROR: dimension (n_row or n_col) must be > 0";
                case UMFPACK_ERROR_invalid_matrix:
                    return "ERROR: input matrix is invalid";
                case UMFPACK_ERROR_invalid_system:
                    return "ERROR: system argument invalid";
                case UMFPACK_ERROR_invalid_permutation:
                    return "ERROR: invalid permutation";
                case UMFPACK_ERROR_different_pattern:
                    return "ERROR: pattern of matrix (Ap and/or Ai) has changed";
                case UMFPACK_ERROR_ordering_failed:
                    return "ERROR: ordering failed";
                case UMFPACK_ERROR_internal_error:
                    return "INTERNAL ERROR! Input arguments might be corrupted or aliased, or an internal error has occurred.";
                default:
                    return "ERROR: Unrecognized error code: " + error;
            }
        }

        #region Error codes

        // status codes (see umfpack.h)

        private const int UMFPACK_OK = 0;

        /* status > 0 means a warning, but the method was successful anyway. */
        /* A Symbolic or Numeric object was still created. */
        private const int UMFPACK_WARNING_singular_matrix = 1;

        /* The following warnings were added in umfpack_*_get_determinant */
        private const int UMFPACK_WARNING_determinant_underflow = 2;
        private const int UMFPACK_WARNING_determinant_overflow = 3;

        /* status < 0 means an error, and the method was not successful. */
        /* No Symbolic of Numeric object was created. */
        private const int UMFPACK_ERROR_out_of_memory = -1;
        private const int UMFPACK_ERROR_invalid_Numeric_object = -3;
        private const int UMFPACK_ERROR_invalid_Symbolic_object = -4;
        private const int UMFPACK_ERROR_argument_missing = -5;
        private const int UMFPACK_ERROR_n_nonpositive = -6;
        private const int UMFPACK_ERROR_invalid_matrix = -8;
        private const int UMFPACK_ERROR_different_pattern = -11;
        private const int UMFPACK_ERROR_invalid_system = -13;
        private const int UMFPACK_ERROR_invalid_permutation = -15;
        private const int UMFPACK_ERROR_internal_error = -911; /* yes, call me if you get this! */
        private const int UMFPACK_ERROR_file_IO = -17;

        private const int UMFPACK_ERROR_ordering_failed = -18;

        #endregion
    }
}
