
using CSparse.Double;
using CSparse.Storage;

namespace BriefFiniteElementNet.Solver
{
    using System;

    /// <summary>
    /// Base class for iterative solver implementations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class IterativeSolver : IIterativeSolver
    {
        public CompressedColumnStorage A { get; set; }

        public Model Target { get; private set; }

        public void Initialize()
        {
            if (this.Preconditioner != null)
            {
                this.Preconditioner.Initialize(A);
            }
        }

        public abstract void Solve(double[] input, double[] result);

        /// <summary>
        /// Number of iterations.
        /// </summary>
        protected int numIterations;

        /// <inheritdoc />
        public double RelativeTolerance { get; set; }

        /// <inheritdoc />
        public double AbsoluteTolerance { get; set; }

        /// <inheritdoc />
        public double ConvergenceFactorTolerance { get; set; }

        /// <inheritdoc />
        public int MaxIterations { get; set; }

        /// <inheritdoc />
        public int NumberOfIterations
        {
            get { return numIterations; }
        }

        /// <summary>
        /// Gets or sets the preconditioner.
        /// </summary>
        public IPreconditioner<double> Preconditioner
        {
            get;
            set;
        }

        /// <inheritdoc />
        public IterativeSolver()
        {
            RelativeTolerance = 1e-8;
            AbsoluteTolerance = 0.0;
            ConvergenceFactorTolerance = 0.0;
            MaxIterations = 500;

            numIterations = 0;
        }

        /// <inheritdoc />
        public virtual bool IsInitialized
        {
            get { return this.Preconditioner == null ? true : Preconditioner.IsInitialized; }
        }

        /// <inheritdoc />
        public abstract BuiltInSolverType SolverType { get; }

        /// <inheritdoc />
        public virtual void Initialize(CompressedColumnStorage<double> matrix)
        {
            if (this.Preconditioner != null)
            {
                this.Preconditioner.Initialize(matrix);
            }
        }

        // /// <inheritdoc />
        //public abstract SolverResult Solve(CompressedColumnStorage<double> A, double[] input, double[] result);
    }
}
