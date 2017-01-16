
using BriefFiniteElementNet.Common;

namespace BriefFiniteElementNet.Solver
{
    using System;

    /// <summary>
    /// Iterative solver interface.
    /// </summary>
    public interface IIterativeSolver: ISolver
    {
        /// <summary>
        /// Gets or sets the relative tolerance iteration stop criterium.
        /// </summary>
        double RelativeTolerance { get; set; }

        /// <summary>
        /// Gets or sets the absolute tolerance iteration stop criterium.
        /// </summary>
        double AbsoluteTolerance { get; set; }

        /// <summary>
        /// Gets or sets the convergence factor tolerance iteration stop criterium.
        /// </summary>
        double ConvergenceFactorTolerance { get; set; }

        /// <summary>
        /// Gets or sets the iteration count stop criterium.
        /// </summary>
        int MaxIterations { get; set; }

        /// <summary>
        /// Gets the number of iterations.
        /// </summary>
        int NumberOfIterations { get; }
    }
}
