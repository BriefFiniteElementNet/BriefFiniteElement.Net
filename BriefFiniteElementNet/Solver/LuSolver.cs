using System;
using BriefFiniteElementNet.Common;
using CSparse;
using CSparse.Double;
using CSparse.Double.Factorization;
using CSparse.Factorization;
using CSparse.Storage;
using System.Diagnostics;

namespace BriefFiniteElementNet.Solver
{
    /// <summary>
    /// Direct solver using Cholesky decomposition.
    /// </summary>
    public class LuSolver : ISolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LuSolver"/> class.
        /// </summary>
        /// <param name="a">A.</param>
        public LuSolver(SparseMatrix a)
        {
            A = a;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LuSolver"/> class.
        /// </summary>
        public LuSolver()
        {
        }

        public Model Target { get; private set; }

        /// <inheritdoc />
        public SparseMatrix A { get; set; }


        ISparseFactorization<double> lu;

        /// <inheritdoc />
        public bool IsInitialized
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public BuiltInSolverType SolverType
        {
            get { return BuiltInSolverType.Lu; }
        }

        /// <inheritdoc />
        public void Initialize()
        {
            var matrix = A;
            var sp = new Stopwatch();
            sp.Start();

            lu =
                SparseLU.Create(matrix, ColumnOrdering.MinimumDegreeAtPlusA, 0.01);
            
            
            IsInitialized = true;

            sp.Stop();

            if (Target != null)
                Target.Trace.Write(TraceRecord.Create(BriefFiniteElementNet.Common.TraceLevel.Info,
                    string.Format("LU decomposition of matrix took about {0:#,##0} ms",
                        sp.ElapsedMilliseconds)));
        }

        /// <inheritdoc />
        public void Solve(double[] input, double[] result)
        {
            if (!IsInitialized)
            {
                Initialize();
            }

            lu.Solve(input, result);
        }
    }
}
