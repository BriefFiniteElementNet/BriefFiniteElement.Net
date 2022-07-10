
namespace CSparse.Solvers
{
    static class Job
    {
        internal static bool Validate(bool symmetric, Spectrum job)
        {
            return symmetric ? ValidateSymmetric(job) : ValidateGeneral(job);
        }

        internal static bool ValidateSymmetric(Spectrum job)
        {
            return job == Spectrum.LargestMagnitude
                || job == Spectrum.LargestAlgebraic
                || job == Spectrum.SmallestMagnitude
                || job == Spectrum.SmallestAlgebraic
                || job == Spectrum.BothEnds;
        }

        internal static bool ValidateGeneral(Spectrum job)
        {
            return job == Spectrum.LargestMagnitude
                || job == Spectrum.LargestRealPart
                || job == Spectrum.LargestImaginaryPart
                || job == Spectrum.SmallestMagnitude
                || job == Spectrum.LargestRealPart
                || job == Spectrum.SmallestImaginaryPart;
        }
    }
}
