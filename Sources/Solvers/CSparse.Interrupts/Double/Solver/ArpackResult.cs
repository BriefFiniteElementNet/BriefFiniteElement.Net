
namespace CSparse.Double.Solver
{
    using CSparse.Interop.ARPACK;
    using CSparse.Storage;
    using System;
    using System.Numerics;

    /// <inheritdoc />
    public class ArpackResult : ArpackResult<double>
    {
        private bool symmetric;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArpackResult"/> class.
        /// </summary>
        /// <param name="k">The number of eigenvalues requested.</param>
        /// <param name="size">The problem size.</param>
        /// <param name="computeEigenVectors">A value, indicating whether eigenvectors are requested.</param>
        /// <param name="symmetric">A value, indicating whether problem is symmetric.</param>
        public ArpackResult(int k, int size, bool computeEigenVectors, bool symmetric)
            : base(k, size)
        {
            this.symmetric = symmetric;

            CreateWorkspace(computeEigenVectors, symmetric);
        }

        /// <inheritdoc />
        public override double[] EigenValuesReal()
        {
            return (double[])eigvalr;
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

            var rp = (double[])eigvalr;
            var ip = (double[])eigvali;

            for (int i = 0; i < k; i++)
            {
                result[i] = new Complex(rp[i], ip[i]);
            }

            return result;
        }

        /// <inheritdoc />
        protected override Matrix<Complex> CreateEigenVectorsMatrix()
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

            // Imaginary part of eigenvalues.
            var eim = (double[])eigvali;

            int length = values.Length;

            for (int i = 0; i < k; i++)
            {
                int current = i * size; // Current column offset.

                if (eim[i] == 0.0)
                {
                    for (int j = 0; j < size; j++)
                    {
                        values[current + j] = rp[current + j];
                    }
                }
                else
                {
                    int next = (i + 1) * size; // Next column.

                    for (int j = 0; j < size; j++)
                    {
                        var a = new Complex(rp[current + j], rp[next + j]);

                        values[current + j] = a;

                        // Check if next column is available (alternatively, allocate space for
                        // one additional column, see CreateWorkspace() method below).
                        if (next < length)
                        {
                            values[next + j] = Complex.Conjugate(a);
                        }
                    }

                    i++;
                }
            }

            return result;
        }

        private void CreateWorkspace(bool computeEigenVectors, bool symmetric)
        {
            int n = this.size;
            int k = this.Count;

            // For complex eigenvalues of non-symmetric problems, the complex conjugate will also be
            // an eigenvalue. ARPACK will always compute both eigenvalues. This means that though k
            // eigenvalues might be requested, k+1 eigenvalues will be computed. We have to allocate
            // enough memory to handle this case.

            int s = symmetric ? k : k + 1;

            // For symmetric problems, eigvali isn't used. We initialize it anyway.

            eigvalr = new double[s];
            eigvali = new double[s];

            if (computeEigenVectors)
            {
                // For complex eigenvalues of non-symmetric problems, the eigenvector of the
                // conjugate eigenvalue will not be computed explicitly. The real and imaginary
                // parts of the eigenvectors are stored in consecutive columns.
                eigvec = new double[n * s];

                // HACK: this only works because the array values are stored in column major order.
            }
        }
    }
}
