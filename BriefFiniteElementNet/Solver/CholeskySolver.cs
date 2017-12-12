
using System;
using BriefFiniteElementNet.Common;
using CSparse;
using CSparse.Double;
using CSparse.Double.Factorization;
using CSparse.Factorization;
using CSparse.Storage;


namespace BriefFiniteElementNet.Solver
{
    using System.Diagnostics;

    /// <summary>
    /// Direct solver using Cholesky decomposition.
    /// </summary>
    public class CholeskySolver : ISolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CholeskySolver"/> class.
        /// </summary>
        /// <param name="a">A.</param>
        public CholeskySolver(CompressedColumnStorage a)
        {
            A = a;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CholeskySolver"/> class.
        /// </summary>
        public CholeskySolver()
        {
        }

        public Model Target { get; private set; }

        /// <inheritdoc />
        public CompressedColumnStorage A { get; set; }


        ISparseFactorization<double> cholesky;

        /// <inheritdoc />
        public bool IsInitialized
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public BuiltInSolverType SolverType
        {
            get { return BuiltInSolverType.CholeskyDecomposition; }
        }

        /// <inheritdoc />
        public void Initialize()
        {
            var matrix = A;
            var sp = new Stopwatch();
            sp.Start();

            cholesky =
                SparseCholesky.Create(matrix, ColumnOrdering.MinimumDegreeAtPlusA);
                

            IsInitialized = true;

            sp.Stop();

            if (Target != null)
                Target.Trace.Write(TraceRecord.Create(BriefFiniteElementNet.Common.TraceLevel.Info,
                    string.Format("cholesky decomposition of Kff took about {0:#,##0} ms",
                        sp.ElapsedMilliseconds)));
        }

        /// <inheritdoc />
        public void Solve(double[] input, double[] result)
        {
            if (!IsInitialized)
            {
                Initialize();
            }

            cholesky.Solve(input, result);
        }
    }
}
