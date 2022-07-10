namespace CSparse.Interop.ARPACK
{
    using System;

    /// <summary>
    /// Arpack++ exception (see arerror.h header file).
    /// </summary>
    public class ArpackException : Exception
    {
        public int ErrorCode { get; private set; }

        public ArpackException(int error)
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
                case NOT_IMPLEMENTED:
                    return "This function was not implemented yet";
                case MEMORY_OVERFLOW:
                    return "Memory overflow";
                case GENERIC_SEVERE:
                    return "Severe error";
                case PARAMETER_ERROR:
                    return "Some parameters were not correctly defined";
                case N_SMALLER_THAN_2:
                    return "'n' must be greater than one";
                case NEV_OUT_OF_BOUNDS:
                    return "'nev' is out of bounds";
                case WHICH_UNDEFINED:
                    return "'which' was not correctly defined";
                case PART_UNDEFINED:
                    return "'part' must be one of 'R' or 'I'";
                case INVMODE_UNDEFINED:
                    return "'InvertMode' must be one of 'S' or 'B'";
                case RANGE_ERROR:
                    return "Range error";
                case LAPACK_ERROR:
                    return "Could not perform LAPACK eigenvalue calculation";
                case START_RESID_ZERO:
                    return "Starting vector is zero";
                case NOT_ACCURATE_EIG:
                    return "Could not find any eigenvalue to sufficient accuracy";
                case REORDERING_ERROR:
                    return "Reordering of Schur form was not possible";
                case ARNOLDI_NOT_BUILD:
                    return "Could not build an Arnoldi factorization";
                case AUPP_ERROR:
                    return "Error in ARPACK Aupd fortran code";
                case EUPP_ERROR:
                    return "Error in ARPACK Eupd fortran code";
                case CANNOT_PREPARE:
                    return "Could not correctly define internal variables";
                case CANNOT_FIND_BASIS:
                    return "Could not find an Arnoldi basis";
                case CANNOT_FIND_VALUES:
                    return "Could not find any eigenvalue";
                case CANNOT_FIND_VECTORS:
                    return "Could not find any eigenvector";
                case CANNOT_FIND_SCHUR:
                    return "Could not find any Schur vector";
                case SCHUR_UNDEFINED:
                    return "FindEigenvectors must be used instead of FindSchurVectors";
                case CANNOT_GET_VECTOR:
                    return "Vector is not already available";
                case CANNOT_GET_PROD:
                    return "Matrix-vector product is not already available";
                case CANNOT_PUT_VECTOR:
                    return "Could not store vector";
                case PREPARE_NOT_OK:
                    return "DefineParameters must be called prior to this function";
                case BASIS_NOT_OK:
                    return "An Arnoldi basis is not available";
                case VALUES_NOT_OK:
                    return "Eigenvalues are not available";
                case VECTORS_NOT_OK:
                    return "Eigenvectors are not available";
                case SCHUR_NOT_OK:
                    return "Schur vectors are not available";
                case RESID_NOT_OK:
                    return "Residual vector is not available";
                case MATRIX_IS_SINGULAR:
                    return "Matrix is singular and could not be factored";
                case DATA_UNDEFINED:
                    return "Matrix data was not defined";
                case INSUFICIENT_MEMORY:
                    return "fill-in factor must be increased";
                case NOT_SQUARE_MATRIX:
                    return "Matrix must be square to be factored";
                case NOT_FACTORED_MATRIX:
                    return "Matrix must be factored before solving a system";
                case INCOMPATIBLE_SIZES:
                    return "Matrix dimensions must agree";
                case DIFFERENT_TRIANGLES:
                    return "A.uplo and B.uplo must be equal";
                case INCONSISTENT_DATA:
                    return "Matrix data contain inconsistencies";
                case CANNOT_READ_FILE:
                    return "Data file could not be read";
                case CANNOT_OPEN_FILE:
                    return "Invalid path or filename";
                case WRONG_MATRIX_TYPE:
                    return "Wrong matrix type";
                case WRONG_DATA_TYPE:
                    return "Wrong data type";
                case RHS_IGNORED:
                    return "RHS vector will be ignored";
                case UNEXPECTED_EOF:
                    return "Unexpected end of file";
                case NCV_OUT_OF_BOUNDS:
                    return "'ncv' is out of bounds";
                case MAXIT_NON_POSITIVE:
                    return "'maxit' must be greater than zero";
                case MAX_ITERATIONS:
                    return "Maximum number of iterations taken";
                case NO_SHIFTS_APPLIED:
                    return "No shifts could be applied during a cycle of IRAM iteration";
                case CHANGING_AUTOSHIFT:
                    return "Turning to automatic selection of implicit shifts";
                case DISCARDING_FACTORS:
                    return "Factors L and U were not copied. Matrix must be factored";
                case GENERIC_WARNING:
                default:
                    return "There is something wrong";
            }
        }

        #region Error codes

        // Innocuous error type.

        private const int NO_ERRORS = 0;

        // Errors in parameter definitions.

        private const int PARAMETER_ERROR = -101;
        private const int N_SMALLER_THAN_2 = -102;
        private const int NEV_OUT_OF_BOUNDS = -103;
        private const int WHICH_UNDEFINED = -104;
        private const int PART_UNDEFINED = -105;
        private const int INVMODE_UNDEFINED = -106;
        private const int RANGE_ERROR = -107;

        // Errors in Aupp and Eupp functions.

        private const int LAPACK_ERROR = -201;
        private const int START_RESID_ZERO = -202;
        private const int NOT_ACCURATE_EIG = -203;
        private const int REORDERING_ERROR = -204;
        private const int ARNOLDI_NOT_BUILD = -205;
        private const int AUPP_ERROR = -291;
        private const int EUPP_ERROR = -292;

        // Errors in main functions.

        private const int CANNOT_PREPARE = -301;
        private const int CANNOT_FIND_BASIS = -302;
        private const int CANNOT_FIND_VALUES = -303;
        private const int CANNOT_FIND_VECTORS = -304;
        private const int CANNOT_FIND_SCHUR = -305;
        private const int SCHUR_UNDEFINED = -306;

        // Errors due to incorrect function calling sequence.

        private const int CANNOT_GET_VECTOR = -401;
        private const int CANNOT_GET_PROD = -402;
        private const int CANNOT_PUT_VECTOR = -403;
        private const int PREPARE_NOT_OK = -404;
        private const int BASIS_NOT_OK = -405;
        private const int VALUES_NOT_OK = -406;
        private const int VECTORS_NOT_OK = -407;
        private const int SCHUR_NOT_OK = -408;
        private const int RESID_NOT_OK = -409;

        // Errors in classes that perform LU decompositions.

        private const int MATRIX_IS_SINGULAR = -501;
        private const int DATA_UNDEFINED = -502;
        private const int INSUFICIENT_MEMORY = -503;
        private const int NOT_SQUARE_MATRIX = -504;
        private const int NOT_FACTORED_MATRIX = -505;
        private const int INCOMPATIBLE_SIZES = -506;
        private const int DIFFERENT_TRIANGLES = -507;
        private const int INCONSISTENT_DATA = -508;
        private const int CANNOT_READ_FILE = -509;

        // Errors in matrix files.

        private const int CANNOT_OPEN_FILE = -551;
        private const int WRONG_MATRIX_TYPE = -552;
        private const int WRONG_DATA_TYPE = -553;
        private const int RHS_IGNORED = -554;
        private const int UNEXPECTED_EOF = -555;

        // Other severe errors.

        private const int NOT_IMPLEMENTED = -901;
        private const int MEMORY_OVERFLOW = -902;
        private const int GENERIC_SEVERE = -999;

        // Warnings.

        private const int NCV_OUT_OF_BOUNDS = 101;
        private const int MAXIT_NON_POSITIVE = 102;
        private const int MAX_ITERATIONS = 201;
        private const int NO_SHIFTS_APPLIED = 202;
        private const int CHANGING_AUTOSHIFT = 301;
        private const int DISCARDING_FACTORS = 401;
        private const int GENERIC_WARNING = 999;

        #endregion
    }
}
