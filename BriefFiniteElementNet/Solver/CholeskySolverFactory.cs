using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Common;
using CSparse.Double;

namespace BriefFiniteElementNet.Solver
{
    /// <summary>
    /// Represents a factory for <see cref="CholeskySolver"/>.
    /// </summary>
    public class CholeskySolverFactory:ISolverFactory
    {
        /// <inheritDoc />
        public ISolver CreateSolver(CompressedColumnStorage A)
        {
            return new CholeskySolver(A);
        }
    }
}
