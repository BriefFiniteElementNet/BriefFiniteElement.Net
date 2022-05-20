﻿
namespace BriefFiniteElementNet.Common
{
    using CSparse.Double;

    /// <summary>
    /// Represents a factory for <see cref="ISolver"/> interface.
    /// </summary>
    public interface ISolverFactory
    {
        /// <summary>
        /// Creates the solver.
        /// </summary>
        /// <returns></returns>
        ISolver CreateSolver(SparseMatrix A);
    }
}
