using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Common;
using CSparse.Double;

namespace BriefFiniteElementNet.CudaSolver
{
    /// <summary>
    /// Represents a factory for <see cref="CuSparseDirectSpdSolver"/>
    /// </summary>
    public class CuSparseDirectSpdSolverFactory: ISolverFactory
    {
        public ISolver CreateSolver(CompressedColumnStorage A)
        {
            var buf = new CuSparseDirectSpdSolver();
            buf.A = A;
            return buf;
        }
    }
}
