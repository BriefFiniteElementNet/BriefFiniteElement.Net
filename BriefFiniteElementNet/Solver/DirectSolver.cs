
using BriefFiniteElementNet.CSparse;
using BriefFiniteElementNet.CSparse.Double.Factorization;
using BriefFiniteElementNet.CSparse.Storage;

namespace BriefFiniteElementNet.Solver
{
    using System.Diagnostics;

    public class DirectSolver : ISolver<double>
    {
        bool factorized;

        ISparseFactorization<double> factor;

        /// <inheritdoc />
        public bool IsInitialized
        {
            get { return factorized; }
        }

        /// <inheritdoc />
        public bool IsDirect
        {
            get { return true; }
        }

        /// <inheritdoc />
        public void Initialize(CompressedColumnStorage<double> matrix)
        {
            var sp = new Stopwatch();
            sp.Start();

            factor = new SparseCholesky(matrix, ColumnOrdering.MinimumDegreeAtPlusA);
            factorized = true;

            sp.Stop();

            TraceUtil.WritePerformanceTrace("cholesky decomposition of Kff took about {0:#,##0} ms",
                sp.ElapsedMilliseconds);
        }

        /// <inheritdoc />
        public SolverResult Solve(CompressedColumnStorage<double> A, double[] input, double[] result)
        {
            factor.Solve(input, result);

            return SolverResult.Success;
        }
    }
}
