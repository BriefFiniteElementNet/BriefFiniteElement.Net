using BriefFiniteElementNet.Common;
using CSparse.Double;

namespace BriefFiniteElementNet.Solver
{
    /// <summary>
    /// Represents a factory for <see cref="QRSolver"/>.
    /// </summary>
    public class QrSolverFactory : ISolverFactory
    {
        /// <inheritDoc />
        public ISolver CreateSolver(SparseMatrix A)
        {
            return new QRSolver(A);
        }
    }
}
