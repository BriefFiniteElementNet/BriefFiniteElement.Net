using BriefFiniteElementNet.Common;
using CSparse.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Solvers
{
    public class MklSolverFactory : BriefFiniteElementNet.Common.ISolverFactory
    {
        public ISolver CreateSolver(SparseMatrix A)
        {
            var buf = new MklSolver();
            buf.A = A;
            return buf;
        }
    }
}
