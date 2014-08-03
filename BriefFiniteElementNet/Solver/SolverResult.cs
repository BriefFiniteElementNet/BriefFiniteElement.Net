
namespace BriefFiniteElementNet.Solver
{
    /// <summary>
    /// Result of an iterative solver.
    /// </summary>
    public enum SolverResult
    {
        /// <summary>
        /// The iteration converged.
        /// </summary>
        Convergence,
        /// <summary>
        /// The iteration diverged.
        /// </summary>
        Divergence,
        /// <summary>
        /// Numerical breakdown.
        /// </summary>
        BreakDown,
        /// <summary>
        /// General failure case.
        /// </summary>
        Failure,
        /// <summary>
        /// General success case.
        /// </summary>
        Success
    }
}
