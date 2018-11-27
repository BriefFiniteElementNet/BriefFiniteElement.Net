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
        public ISolver CreateSolver(SparseMatrix A)
        {
            return new PCG(new SSOR()) {A = A};
        }
    }
}
