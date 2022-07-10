
namespace CSparse.Interop.MKL.Pardiso
{
    /// <summary>
    /// PARDISO matrix type.
    /// </summary>
    public class PardisoMatrixType
    {
        /// <summary>
        /// Real and structurally symmetric matrix.
        /// </summary>
        public const int RealStructurallySymmetric = 1;

        /// <summary>
        /// Real and symmetric positive definite matrix.
        /// </summary>
        public const int RealSymmetricPositiveDefinite = 2;

        /// <summary>
        /// Real and symmetric indefinite matrix.
        /// </summary>
        public const int RealSymmetricIndefinite = -2;

        /// <summary>
        /// Complex and structurally symmetric matrix.
        /// </summary>
        public const int ComplexStructurallySymmetric = 3;

        /// <summary>
        /// Complex and Hermitian positive definite matrix.
        /// </summary>
        public const int ComplexHermitianPositiveDefinite = 4;

        /// <summary>
        /// Complex and Hermitian indefinite matrix.
        /// </summary>
        public const int ComplexHermitianIndefinite = -4;
        
        /// <summary>
        /// Complex and symmetric matrix.
        /// </summary>
        public const int ComplexSymmetric = 6;

        /// <summary>
        /// Real and nonsymmetric matrix.
        /// </summary>
        public const int RealNonsymmetric = 11;

        /// <summary>
        /// Complex and nonsymmetric matrix.
        /// </summary>
        public const int ComplexNonsymmetric = 13;
    }
}
