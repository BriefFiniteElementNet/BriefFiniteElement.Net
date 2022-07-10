
namespace CSparse.Interop.MKL.Feast
{
    using System;

    public class FeastException : Exception
    {
        public int ErrorCode { get; private set; }

        public FeastException(int code)
            : base(GetErrorMessage(code))
        {
            this.ErrorCode = code;
        }

        private static string GetErrorMessage(int code)
        {
            switch (code)
            {
                case 1:
                    return "Warning: No eigenvalue found in the search interval.";
                case 2:
                    return "Warning: No Convergence (increase number of iteration loops)."; // #iteration loops>fpm(4)
                case 3:
                    return "Warning: Size of the subspace m0 is too small (m0 < m).";
                case 4:
                    return "Successful return of only the computed subspace after call with fpm[13]=1";
                case 5:
                    return "Successful return of only stochastic estimation of number of eigenvalues after call with fpm[13]=2";
                case 6:
                    return "FEAST converges but subspace is not bi-orthonormal";
                case 200:
                    return "Problem with emin, emax (emin >= emax).";
                case 201:
                    return "Problem with size of initial subspace m0 (m0 <= 0 or m0 > n).";
                case 202:
                    return "Problem with size of the system n (n <= 0).";
                case -1:
                    return "Internal error for allocation memory.";
                case -2:
                    return "Internal error of the inner system solver. Possible reasons: not enough memory for inner linear system solver or inconsistent input.";
                case -3:
                    return "Internal error of the reduced eigenvalue solver. Possible cause: matrix B may not be positive definite.";
                case -4:
                    return "Matrix B is not positive definite.";
                default:
                    break;
            }

            if (code >= 100)
            {
                return "Problem with i-th value of the input FEAST parameter (fpm[" + (code % 100) + "].";
            }

            if (code <= -100)
            {
                return "Problem with the i-th argument of the FEAST interface (" + (-code % 100) + ").";
            }

            return "Unknown error.";
        }
    }
}
