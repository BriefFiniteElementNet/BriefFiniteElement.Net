
namespace CSparse.Solvers
{
    /// <summary>
    /// Selection rule for the spectrum to compute.
    /// </summary>
    public enum Spectrum
    {
        /// <summary>
        /// Largest algebraic (ARPACK which "LA").
        /// </summary>
        LargestAlgebraic,
        /// <summary>
        /// Largest magnitude (ARPACK which "LM").
        /// </summary>
        LargestMagnitude,
        /// <summary>
        /// Largest real part (ARPACK which "LR").
        /// </summary>
        LargestRealPart,
        /// <summary>
        /// Largest imaginary part (ARPACK which "LI").
        /// </summary>
        LargestImaginaryPart,
        /// <summary>
        /// Smallest algebraic (ARPACK which "SA").
        /// </summary>
        SmallestAlgebraic,
        /// <summary>
        /// Smallest magnitude (ARPACK which "SM").
        /// </summary>
        SmallestMagnitude,
        /// <summary>
        /// Smallest real part (ARPACK which "SR").
        /// </summary>
        SmallestRealPart,
        /// <summary>
        /// Smallest imaginary part (ARPACK which "SI").
        /// </summary>
        SmallestImaginaryPart,
        /// <summary>
        /// Both ends of the spectrum (ARPACK which "BE").
        /// </summary>
        BothEnds,
    }
}
