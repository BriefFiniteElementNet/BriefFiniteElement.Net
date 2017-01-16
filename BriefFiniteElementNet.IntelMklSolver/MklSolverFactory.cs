using System;
using BriefFiniteElementNet.Common;
using CSparse.Double;

namespace BriefFiniteElementNet.IntelMklSolver
{
    /// <summary>
    /// Represents a factory for <see cref="MklDirectPosdefSolver"/>
    /// </summary>
    public class MklDirectPosdefSolverFactory : ISolverFactory
    {
        public ISolver CreateSolver(CompressedColumnStorage A)
        {
            throw new NotImplementedException();
        }
    }
}