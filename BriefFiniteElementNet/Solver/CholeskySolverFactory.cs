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
        public ISolver CreateSolver(SparseMatrix A)
        {
            return new CholeskySolver(A);
        }
    }
}
