
namespace CSparse.Interop.MKL.Pardiso
{
    using System;

    public class PardisoException : Exception
    {
        public int ErrorCode { get; private set; }

        public PardisoException(int code)
            : base(GetErrorMessage(code))
        {
            this.ErrorCode = code;
        }

        private static string GetErrorMessage(int code)
        {
            switch (code)
            {
                case -1:
                    return "Input inconsistent.";
                case -2:
                    return "Not enough memory.";
                case -3:
                    return "Reordering problem.";
                case -4:
                    return "Zero pivot, numerical factorization or iterative refinement problem.";
                case -5:
                    return "Unclassified (internal) error.";
                case -6:
                    return "Reordering failed (matrix types 11 and 13 only).";
                case -7:
                    return "Diagonal matrix is singular.";
                case -8:
                    return "32-bit integer overflow problem.";
                case -9:
                    return "Not enough memory for OOC.";
                case -10:
                    return "Problems with opening OOC temporary files.";
                case -11:
                    return "Read/write problems with the OOC data file.";
                default:
                    break;
            }

            return "Unknown error.";
        }
    }
}
