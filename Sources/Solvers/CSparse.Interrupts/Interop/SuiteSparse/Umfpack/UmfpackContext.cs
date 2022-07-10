namespace CSparse.Interop.SuiteSparse.Umfpack
{
    using CSparse.Factorization;
    using CSparse.Storage;
    using System;

    /// <summary>
    /// UMFPACK context wrapping native factorization.
    /// </summary>
    public abstract class UmfpackContext<T> : IDisposableSolver<T>
        where T : struct, IEquatable<T>, IFormattable
    {
        protected readonly CompressedColumnStorage<T> matrix;

        protected readonly UmfpackInfo info;
        protected readonly UmfpackControl control;

        protected bool factorized;

        protected IntPtr symbolic, numeric;

        /// <summary>
        /// Gets the UMFPACK info.
        /// </summary>
        public UmfpackInfo Info { get { return info; } }

        /// <summary>
        /// Gets the UMFPACK control.
        /// </summary>
        public UmfpackControl Control { get { return control; } }

        /// <summary>
        /// Initializes a new instance of the UmfpackContext class.
        /// </summary>
        /// <param name="matrix">The sparse matrix to factorize.</param>
        public UmfpackContext(CompressedColumnStorage<T> matrix)
        {
            this.matrix = matrix;

            info = new UmfpackInfo();
            control = new UmfpackControl();

            DoInitialize();
        }

        ~UmfpackContext()
        {
            Dispose(false);
        }

        /// <summary>
        /// Factorizes the matrix associated to this UMFPACK instance.
        /// </summary>
        public void Factorize()
        {
            int status = DoFactorize();

            if (status != Constants.UMFPACK_OK)
            {
                throw new UmfpackException(status);
            }
            
            factorized = true;
        }

        /// <summary>
        /// Solves a system of linear equations, Ax = b.
        /// </summary>
        /// <param name="input">Right hand side vector b.</param>
        /// <param name="result">Solution vector x.</param>
        public void Solve(T[] input, T[] result)
        {
            Solve(UmfpackSolve.A, input, result);
        }

        /// <summary>
        /// Solves the transpose system of linear equations, A'x = b.
        /// </summary>
        /// <param name="input">Right hand side vector b</param>
        /// <param name="result">Solution vector x.</param>
        public void SolveTranspose(T[] input, T[] result)
        {
            Solve(UmfpackSolve.At, input, result);
        }

        /// <summary>
        /// Solves a system of linear equations for multiple right-hand sides, AX = B.
        /// </summary>
        /// <param name="input">Right hand side matrix B.</param>
        /// <param name="result">Solution matrix X.</param>
        public void Solve(DenseColumnMajorStorage<T> input, DenseColumnMajorStorage<T> result)
        {
            Solve(UmfpackSolve.A, input, result);
        }

        private void Solve(UmfpackSolve sys, T[] input, T[] result)
        {
            if (!factorized)
            {
                Factorize();
            }

            int status = DoSolve(sys, input, result);

            if (status != Constants.UMFPACK_OK)
            {
                throw new UmfpackException(status);
            }
        }

        private void Solve(UmfpackSolve sys, DenseColumnMajorStorage<T> input, DenseColumnMajorStorage<T> result)
        {
            if (!factorized)
            {
                Factorize();
            }

            // The number of right-hand sides.
            int count = input.ColumnCount;

            if (count != result.ColumnCount)
            {
                throw new ArgumentException("result");
            }

            int rows = matrix.RowCount;
            int columns = matrix.ColumnCount;

            var wi = new int[columns];
            var wx = CreateWorkspace(columns, Control.IterativeRefinement > 0);

            var x = new T[columns];
            var b = new T[rows];

            for (int i = 0; i < count; i++)
            {
                input.Column(i, b);

                int status = DoSolve(sys, b, x, wi, wx);

                if (status != Constants.UMFPACK_OK)
                {
                    throw new UmfpackException(status);
                }

                result.SetColumn(i, x);
            }
        }

        /// <summary>
        /// Do initialization for current type.
        /// </summary>
        protected abstract void DoInitialize();

        /// <summary>
        /// Do symbolic factorization for current type.
        /// </summary>
        protected abstract int DoSymbolic();

        /// <summary>
        /// Do numeric factorization for current type.
        /// </summary>
        protected abstract int DoNumeric();

        /// <summary>
        /// Do symbolic and numeric factorization for current type.
        /// </summary>
        protected abstract int DoFactorize();

        /// <summary>
        /// Solve system of linear equations.
        /// </summary>
        /// <param name="sys">The system to solve.</param>
        /// <param name="input">Right-hand side b.</param>
        /// <param name="result">The solution x.</param>
        /// <returns></returns>
        protected abstract int DoSolve(UmfpackSolve sys, T[] input, T[] result);

        /// <summary>
        /// Solve system of linear equations using given workspace.
        /// </summary>
        /// <param name="sys">The system to solve.</param>
        /// <param name="input">Right-hand side b.</param>
        /// <param name="result">The solution x.</param>
        /// <param name="wi">Integer workspace.</param>
        /// <param name="wx">Double workspace.</param>
        /// <returns></returns>
        protected abstract int DoSolve(UmfpackSolve sys, T[] input, T[] result, int[] wi, double[] wx);

        /// <summary>
        /// Create workspace for solving multiple right-hand sides.
        /// </summary>
        /// <param name="n">Size of the linear system.</param>
        /// <param name="refine">Perform iterative refinement.</param>
        /// <returns></returns>
        protected abstract double[] CreateWorkspace(int n, bool refine);

        #region IDisposable

        // See https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);

        #endregion
    }
}
