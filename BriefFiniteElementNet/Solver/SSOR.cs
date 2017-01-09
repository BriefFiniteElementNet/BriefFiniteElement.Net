// -----------------------------------------------------------------------
// <copyright file="SSOR.cs">
// Copyright (c) 2003-2006 Bjørn-Ove Heimsund
// Copyright (c) 2006-2014 Samuel Halliday
// Copyright (c) 2014 Christian Woltering, C# version
// </copyright>
// -----------------------------------------------------------------------

using CSparse.Storage;

namespace BriefFiniteElementNet.Solver
{
    using System;

    /// <summary>
    /// SSOR preconditioner. Uses symmetrical successive overrelaxation as a
    /// preconditioner. Meant for symmetrical, positive definite matrices.
    /// For best performance, omega must be carefully chosen (between 0 and 2).
    /// </summary>
    /// <remarks>
    /// From matrix-toolkits-java (LGPL): https://github.com/fommil/matrix-toolkits-java
    /// </remarks>
    public sealed class SSOR : IPreconditioner<double>
    {
        private CompressedColumnStorage<double> matrix;
        private int[] diag; // Position of diagonal entries.
        private double[] work;

        private double omegaF;
        private double omegaR;
        private bool reverse;

        /// <summary>
        /// Gets or sets the overrelaxation parameter for the forward
        /// sweep (between 0 and 2).
        /// </summary>
        public double OmegaF
        {
            get { return omegaF; }
            set
            {
                if (value < 0 || value > 2)
                {
                    throw new ArgumentException("OmegaF must be between 0 and 2");
                }

                omegaF = value;
            }
        }

        /// <summary>
        /// Gets or sets the overrelaxation parameter for the backward
        /// sweep (between 0 and 2).
        /// </summary>
        public double OmegaR
        {
            get { return omegaR; }
            set
            {
                if (value < 0 || value > 2)
                {
                    throw new ArgumentException("OmegaR must be between 0 and 2");
                }

                omegaR = value;
            }
        }

        /// <summary>
        /// Indicates, whether the reverse (backward) sweep is to be done.
        /// Without this, the method is SOR instead of SSOR.
        /// </summary>
        public bool Reverse
        {
            get { return reverse; }
            set { reverse = value; }
        }

        /// <inheritdoc />
        public bool IsInitialized
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public void Initialize(CompressedColumnStorage<double> matrix)
        {
            this.matrix = matrix;
            this.reverse = true;
            this.omegaF = 1.0;
            this.omegaR = 1.0;

            // TODO: check matrix is square

            int n = matrix.RowCount;

            this.diag = new int[n];
            this.work = new double[n];

            IsInitialized = true;

            var ap = matrix.ColumnPointers;
            var ai = matrix.RowIndices;

            // Find the indices to the diagonal entries
            for (int k = 0; k < n; ++k)
            {
                diag[k] = Array.BinarySearch(ai, ap[k], ap[k + 1] - ap[k], k);

                if (diag[k] < 0)
                {
                    throw new Exception("Missing diagonal on row " + (k + 1));
                }
            }
        }

        /// <inheritdoc />
        public void Solve(double[] input, double[] result)
        {
            var ap = matrix.ColumnPointers;
            var ai = matrix.RowIndices;
            var ax = matrix.Values;

            int n = matrix.RowCount;
            double sigma;

            Array.Copy(result, 0, work, 0, n);

            // Forward sweep (result = oldest, work = halfiterate)
            for (int i = 0; i < n; ++i)
            {
                sigma = 0.0;

                for (int j = ap[i]; j < diag[i]; ++j)
                {
                    sigma += ax[j] * work[ai[j]];
                }

                for (int j = diag[i] + 1; j < ap[i + 1]; ++j)
                {
                    sigma += ax[j] * result[ai[j]];
                }

                sigma = (input[i] - sigma) / ax[diag[i]];

                work[i] = result[i] + omegaF * (sigma - result[i]);
            }

            // Stop here if the reverse sweep was not requested
            if (!reverse)
            {
                Array.Copy(work, 0, result, 0, n);
                return;
            }

            // Backward sweep (work = oldest, result = halfiterate)
            for (int i = n - 1; i >= 0; --i)
            {
                sigma = 0.0;
                for (int j = ap[i]; j < diag[i]; ++j)
                {
                    sigma += ax[j] * work[ai[j]];
                }

                for (int j = diag[i] + 1; j < ap[i + 1]; ++j)
                {
                    sigma += ax[j] * result[ai[j]];
                }

                sigma = (input[i] - sigma) / ax[diag[i]];

                result[i] = work[i] + omegaR * (sigma - work[i]);
            }
        }
    }
}
