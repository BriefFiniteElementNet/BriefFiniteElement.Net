
namespace CSparse.Complex.Solver
{
    using CSparse.Interop.ARPACK;
    using CSparse.Storage;
    using System.Numerics;

    /// <inheritdoc />
    public class ArpackResult : ArpackResult<Complex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArpackResult"/> class.
        /// </summary>
        /// <param name="k">The number of eigenvalues requested.</param>
        /// <param name="size">The problem size.</param>
        /// <param name="computeEigenVectors">A value, indicating whether eigenvectors are requested.</param>
        public ArpackResult(int k, int size, bool computeEigenVectors)
            : base(k, size)
        {
            CreateWorkspace(computeEigenVectors);
        }

        /// <inheritdoc />
        public override double[] EigenValuesReal()
        {
            int k = this.Count;

            var result = new double[k];

            var e = this.CreateEigenValuesArray();

            for (int i = 0; i < k; i++)
            {
                result[i] = e[i].Real;
            }

            return result;
        }

        /// <inheritdoc />
        public override Matrix<double> EigenVectorsReal()
        {
            int k = this.Count;

            var result = new Double.DenseMatrix(size, k);

            var e = this.CreateEigenVectorsMatrix();

            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    result.At(i, j, e.At(i, j).Real);
                }
            }

            return result;
        }

        /// <inheritdoc />
        protected override Complex[] CreateEigenValuesArray()
        {
            return (Complex[])eigvalr;
        }

        /// <inheritdoc />
        protected override Matrix<Complex> CreateEigenVectorsMatrix()
        {
            return new DenseMatrix(size, this.Count, (Complex[])eigvec);
        }

        private void CreateWorkspace(bool computeEigenVectors)
        {
            int k = this.Count;

            // For complex matrices all eigenvalues are stored in
            // eigvalr with interleaved real and imaginary part.

            eigvalr = new Complex[k];

            if (computeEigenVectors)
            {
                eigvec = new Complex[k * size];
            }
        }
    }
}
