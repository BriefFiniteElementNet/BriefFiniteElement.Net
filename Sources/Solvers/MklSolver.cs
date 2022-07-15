using CSparse.Double;
using System;
using BriefFiniteElementNet.Common;
using System.Text;

namespace BriefFiniteElementNet.Solvers
{
    public class MklSolver : ISolver,IDisposable
    {
        public SparseMatrix A { get; set; }

        public bool IsInitialized { get; private set; }


        private CSparse.Double.Factorization.MKL.Pardiso pardiso;

        public void Initialize()
        {
            pardiso = new CSparse.Double.Factorization.MKL.Pardiso(A);
            pardiso.Factorize();
            IsInitialized = true;
        }

        public void Solve(double[] b, double[] x)
        {
            if (!IsInitialized)
                Initialize();

            pardiso.Solve(b, x);
        }

        public void Dispose()
        {
            if (pardiso != null)
                pardiso.Dispose();
        }
    }
}
