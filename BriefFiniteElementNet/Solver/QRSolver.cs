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
    /// Direct solver using QR.
    /// </summary>
    public class QRSolver : ISolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QRSolver"/> class.
        /// </summary>
        /// <param name="a">A.</param>
        public QRSolver(SparseMatrix a)
        {
            A = a;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QRSolver"/> class.
        /// </summary>
        public QRSolver()
        {
        }

        public Model Target { get; private set; }

        /// <inheritdoc />
        public SparseMatrix A { get; set; }


        ISparseFactorization<double> qr;

        /// <inheritdoc />
        public bool IsInitialized
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public BuiltInSolverType SolverType
        {
            get { return BuiltInSolverType.Qr; }
        }

        /// <inheritdoc />
        public void Initialize()
        {
            var matrix = A;
            var sp = new Stopwatch();
            sp.Start();

            qr =
                SparseQR.Create(matrix, ColumnOrdering.MinimumDegreeAtPlusA);
            
            
            IsInitialized = true;

            sp.Stop();

            if (Target != null)
                Target.Trace.Write(TraceRecord.Create(BriefFiniteElementNet.Common.TraceLevel.Info,
                    string.Format("QR decomposition of matrix took about {0:#,##0} ms",
                        sp.ElapsedMilliseconds)));
        }

        /// <inheritdoc />
        public void Solve(double[] input, double[] result)
        {
            if (!IsInitialized)
            {
                Initialize();
            }

            qr.Solve(input, result);
        }
    }
}
