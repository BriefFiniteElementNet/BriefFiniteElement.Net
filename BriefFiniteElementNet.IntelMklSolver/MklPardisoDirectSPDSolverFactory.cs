using System;
using BriefFiniteElementNet.Common;
using CSparse.Double;

namespace BriefFiniteElementNet.IntelMklSolver
{
    /// <summary>
    /// Represents a factory for <see cref="MklPardisoDirectSPDSolver"/>
    /// </summary>
    public class MklPardisoDirectSPDSolverFactory : ISolverFactory
    {
        public ISolver CreateSolver(CompressedColumnStorage A)
        {
            var buf = new IntelMklSolver.MklPardisoDirectSPDSolver();
            buf.A = A;
            return buf;
        }
    }
}