
namespace CSparse.Interop.SuperLU
{
    using System;

    public class SuperLUException : Exception
    {
        private const string ErrorArgument = "Argument at position {0} had an illegal value.";
        private const string ErrorConditon = "U is nonsingular, but RCOND is less than machine precision, meaning that the matrix is singular to working precision.";
        private const string ErrorMemory = "Memory allocation failure occurred.";
        private const string ErrorSingular = "The factorization has been completed, but the factor U is singular at position {0}.";
        
        public int ErrorCode { get; private set; }

        public SuperLUException(int code, int ncol)
            : base(GetErrorMessage(code, ncol))
        {
            this.ErrorCode = code;
        }

        private static string GetErrorMessage(int code, int ncol)
        {
            if (code < 0)
            {
                return string.Format(ErrorArgument, code);
            }

            if (code < ncol)
            {
                return string.Format(ErrorSingular, code);
            }

            if (code == ncol)
            {
                return string.Format(ErrorConditon, code);
            }

            return string.Format(ErrorMemory, code);
        }
    }
}
