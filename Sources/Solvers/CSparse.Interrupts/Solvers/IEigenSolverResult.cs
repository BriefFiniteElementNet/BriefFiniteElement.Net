
namespace CSparse.Solvers
{
    using System.Numerics;

    /// <summary>
    /// Interface for result returned by an <see cref="IEigenSolver{T}"/>.
    /// </summary>
    public interface IEigenSolverResult
    {
        /// <summary>
        /// Gets the number of requested eigenvalues.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the number of converged eigenvalues.
        /// </summary>
        int ConvergedEigenValues { get; }

        /// <summary>
        /// Gets the number of iterations taken.
        /// </summary>
        int IterationsTaken { get; }

        /// <summary>
        /// Gets the number of Arnoldi vectors computed.
        /// </summary>
        int ArnoldiCount { get; }

        /// <summary>
        /// Gets the error code returned by the solver.
        /// </summary>
        int ErrorCode { get; }

        /// <summary>
        /// Throws an exception, if the eigensolver failed to solve the problem.
        /// </summary>
        void EnsureSuccess();

        /// <summary>
        /// Gets the dense matrix of eigenvectors stored in column major order.
        /// </summary>
        /// <remarks>
        /// For real symmetric matrices, eigenvectors will be real. Use <see cref="EigenVectorsReal()"/>.
        /// </remarks>
        Matrix<Complex> EigenVectors { get; }

        /// <summary>
        /// Gets the eigenvalues.
        /// </summary>
        /// <remarks>
        /// For real symmetric matrices, eigenvalues will be real. Use <see cref="EigenValuesReal()"/>.
        /// </remarks>
        Complex[] EigenValues { get; }

        /// <summary>
        /// Gets the real part of the eigenvalues.
        /// </summary>
        double[] EigenValuesReal();

        /// <summary>
        /// Gets the real part of the eigenvectors.
        /// </summary>
        Matrix<double> EigenVectorsReal();
    }
}
