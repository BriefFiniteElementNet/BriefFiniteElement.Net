using BriefFiniteElementNet.Common;
using CSparse.Double;
using CSparse.Double.Factorization.SuiteSparse;
using System;

namespace BriefFiniteElementNet.Solvers
{
    //Cholmod in suitsparse
    public class CholModSolverFactory : ISolverFactory
    {
        public ISolver CreateSolver(SparseMatrix A)
        {
            return new CholModSolver(A);
        }

        public class CholModSolver : ISolver
        {
            public SparseMatrix A { get; set ; }

            public bool IsInitialized { get; private set; } = false;

            Cholmod cholmod;

            public CholModSolver(SparseMatrix a)
            {
                this.A = a;
            }

            public void Initialize()
            {
                cholmod = new Cholmod(A);
            }

            public void Solve(double[] b, double[] x)
            {
                if (!IsInitialized)
                {
                    Initialize();
                }

                cholmod.Solve(b, x);
            }
        }
    }


}
