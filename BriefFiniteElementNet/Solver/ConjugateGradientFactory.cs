using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Common;
using CSparse.Double;

namespace BriefFiniteElementNet.Solver
{
    /// <summary>
    /// Represents a factory for <see cref="PCG"/>.
    /// </summary>
    public class ConjugateGradientFactory : ISolverFactory
    {
        /// <inheritDoc />
        public ISolver CreateSolver(CompressedColumnStorage A)
        {
            return new PCG(new SSOR()) {A = A};
        }
    }
}
