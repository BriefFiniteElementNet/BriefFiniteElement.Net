
namespace CSparse.Double.Solver
{
    using CSparse.Interop.Spectra;
    using CSparse.Storage;
    using System;
    using System.Numerics;

    /// <inheritdoc />
    public class SpectraResult : SpectraResult<double>
    {
        bool symmetric;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpectraResult"/> class.
        /// </summary>
        /// <param name="k">The number of eigenvalues requested.</param>
        /// <param name="size">The problem size.</param>
        /// <param name="computeEigenVectors">A value, indicating whether eigenvectors are requested.</param>
        /// <param name="symmetric">A value, indicating whether problem is symmetric.</param>
        public SpectraResult(int k, int size, bool computeEigenVectors, bool symmetric)
            : base(k, size)
        {
            this.symmetric = symmetric;

            CreateWorkspace(computeEigenVectors, symmetric);
        }

        /// <inheritdoc />
        public override double[] EigenValuesReal()
        {
            return (double[])eigval;
        }

        /// <inheritdoc />
        public override Matrix<double> EigenVectorsReal()
        {
            int k = this.Count;

            if (symmetric)
            {
                return new DenseMatrix(size, k, (double[])eigvec);
            }

            var result = new DenseMatrix(size, k);

            var e = this.CreateEigenVectorsMatrix();

            for (int j = 0; j < k; j++)
            {
                for (int i = 0; i < size; i++)
                {
                    result.At(i, j, e.At(i, j).Real);
                }
            }

            return result;
        }

        /// <inheritdoc />
        protected override Complex[] CreateEigenValuesArray()
        {
            int k = this.Count;

            var result = new Complex[k];

            var rp = (double[])eigval;

            for (int i = 0; i < k; i++)
            {
                result[i] = new Complex(rp[2 * i], rp[2 * i + 1]);
            }

            return result;
        }

        /// <inheritdoc />
        protected override DenseColumnMajorStorage<Complex> CreateEigenVectorsMatrix()
        {
            // Number of requested eigenvalues.
            int k = this.Count;

            // Matrix with k columns of complex eigenvectors.
            var result = new CSparse.Complex.DenseMatrix(size, k);

            var values = result.Values;

            // Raw eigenvector storage.
            var rp = (double[])eigvec;

            if (symmetric)
            {
                for (int i = 0; i < k; i++)
                {
                    int column = i * size;

                    for (int j = 0; j < size; j++)
                    {
                        values[column + j] = rp[column + j];
                    }
                }

                return result;
            }

            for (int i = 0; i < k; i++)
            {
                int current = i * size; // Current column offset.

                for (int j = 0; j < size; j++)
                {
                    values[current + j] = new Complex(rp[2 * (current + j)], rp[2 * (current + j) + 1]);
                }
            }

            return result;
        }

        private void CreateWorkspace(bool computeEigenVectors, bool symmetric)
        {
            int n = this.size;
            int k = this.Count;

            // In contrast to ARPACK, Spectra uses internal memory for eigenvalues/-vectors
            // and will always return the correct size.

            int s = symmetric ? k : 2 * k;

            eigval = new double[s];

            if (computeEigenVectors)
            {
                eigvec = new double[n * s];
            }
        }
    }
}
