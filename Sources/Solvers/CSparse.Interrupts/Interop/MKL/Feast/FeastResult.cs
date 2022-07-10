
namespace CSparse.Interop.MKL.Feast
{
    using CSparse.Storage;
    using System;
    using System.Numerics;

    /// <summary>
    /// FEAST result.
    /// </summary>
    public abstract class FeastResult<T>
        where T : struct, IEquatable<T>, IFormattable
    {
        protected int size;

        /// <summary>
        /// Initializes a new instance of the FeastResult class.
        /// </summary>
        /// <param name="info">The status returned by FEAST.</param>
        /// <param name="m0">The number of eigenvalues requested.</param>
        /// <param name="size">The matrix size.</param>
        /// <param name="loops">The number of refinement loops executed.</param>
        /// <param name="error">The relative error on the trace.</param>
        /// <param name="m">The number of eigenvalues found (m &lt; m0).</param>
        /// <param name="e">Array of length m0. The first m entries of e are eigenvalues found in the interval.</param>
        /// <param name="x">Matrix with m0 columns containing the orthonormal eigenvectors corresponding to the
        /// computed eigenvalues e, with the i-th column of x holding the eigenvector associated with e[i].</param>
        /// <param name="r">Array of length m0 containing the relative residual vector (in the first m components).</param>
        public FeastResult(int info, int m0, int size, int loops, double error, int m, double[] e, DenseColumnMajorStorage<T> x, double[] r)
        {
            this.size = size;

            Status = info;
            SubspaceDimension = m0;
            RefinementLoops = loops;
            RelativeTraceError = error;

            ConvergedEigenvalues = m;

            EigenValues = CreateEigenValues(e, m);
            EigenVectors = x;

            RelativeResiduals = r;
        }

        /// <summary>
        /// Gets the status code returned by FEAST.
        /// </summary>
        public int Status { get; protected set; }

        /// <summary>
        /// Gets the subspace dimension.
        /// </summary>
        public int SubspaceDimension { get; protected set; }

        /// <summary>
        /// Gets the number of refinement loop executed.
        /// </summary>
        public int RefinementLoops { get; protected set; }

        /// <summary>
        /// Gets the relative error on the trace.
        /// </summary>
        public double RelativeTraceError { get; protected set; }

        /// <summary>
        /// Gets the number of converged eigenvalues.
        /// </summary>
        public int ConvergedEigenvalues { get; protected set; }

        /// <summary>
        /// Gets the dense matrix of eigenvectors stored in column major order.
        /// </summary>
        public DenseColumnMajorStorage<T> EigenVectors { get; protected set; }

        /// <summary>
        /// Gets the eigenvalues.
        /// </summary>
        public Complex[] EigenValues { get; protected set; }

        /// <summary>
        /// Gets the relative residuals vector.
        /// </summary>
        public double[] RelativeResiduals { get; protected set; }

        protected virtual Complex[] CreateEigenValues(double[] x, int length)
        {
            var result = new Complex[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = x[i];
            }

            return result;
        }
    }
}
