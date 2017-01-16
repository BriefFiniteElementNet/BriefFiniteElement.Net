using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSparse.Double;

namespace BriefFiniteElementNet.Common
{
    /// <summary>
    /// Represents a factory for <see cref="ISolver"/> interface.
    /// </summary>
    public interface ISolverFactory
    {
        /// <summary>
        /// Creates the solver.
        /// </summary>
        /// <returns></returns>
        ISolver CreateSolver(CompressedColumnStorage A);
    }
}
