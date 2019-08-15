using BriefFiniteElementNet.Common;
using CSparse.Double;

namespace BriefFiniteElementNet.Solver
{
    /// <summary>
    /// Represents a factory for <see cref="LuSolver"/>.
    /// </summary>
    public class LuSolverFactory : ISolverFactory
    {
        /// <inheritDoc />
        public ISolver CreateSolver(SparseMatrix A)
        {
            return new LuSolver(A);
        }
    }
}
