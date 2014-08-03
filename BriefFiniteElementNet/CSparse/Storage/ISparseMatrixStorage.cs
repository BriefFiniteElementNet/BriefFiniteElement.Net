
using System;

namespace BriefFiniteElementNet.CSparse.Storage
{
    /// <summary>
    /// Sparse matrix storage interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISparseMatrixStorage<T> : IEquatable<ISparseMatrixStorage<T>>
        where T : struct, IEquatable<T>, IFormattable
    {
        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        int RowCount { get; }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        int ColumnCount { get; }
        
        /// <summary>
        /// Gets the number of non-zero entries.
        /// </summary>
        int NonZerosCount { get; }

        /// <summary>
        /// Retrieves the requested element.
        /// </summary>
        /// <param name="i">The row index.</param>
        /// <param name="j">The column index.</param>
        /// <returns>The requested element.</returns>
        /// <remarks>
        /// Retrieving the value at a given index is usually a slow operation for a sparse
        /// matrix storage, so avoid using this method in performance critical code.
        /// </remarks>
        T At(int i, int j);

        /// <summary>
        /// Sets all matrix values to zero.
        /// </summary>
        void Clear();

        /// <summary>
        /// Return the norm of the matrix.
        /// </summary>
        /// <param name="which">The desired matrix norm.</param>
        /// <returns>The norm of the matrix</returns>
        double Norm(MatrixNorm which);

        /// <summary>
        /// Filter matrix values.
        /// </summary>
        /// <param name="func">Filter function returning true if value should be kept,
        /// false if value should be discarded.</param>
        /// <param name="tolerance">Tolerance parameter.</param>
        /// <returns>New number of non-zeros.</returns>
        /// <remarks>
        /// Filter function arguments:
        /// 
        /// 1 = Row index i
        /// 2 = Column index j
        /// 3 = Value of entry (i,j)
        /// 4 = Tolerance (optional parameter)
        /// 
        /// Element a_{i,j} is dropped, if func(i, j, aij, tol) returns false.</param>
        /// </remarks>
        int Keep(Func<int, int, T, double, bool> func, double tolerance);

        /// <summary>
        /// Enumerates the sparse matrix storage.
        /// </summary>
        /// <returns>Enumeration of all storage entries (i, j, a_{ij}).</returns>
        /// <remarks>
        /// This enumerates the storage. It's not the same as MathNet's EnumerateIndexed, but
        /// more like EnumerateNonZeroIndexed (but stored zero's aren't skipped).
        /// </remarks>
        //IEnumerable<Tuple<int, int, T>> EnumerateIndexed();

        /// <summary>
        /// Computes the matrix-vector product y = alpha * A * x + beta * y.
        /// </summary>
        void Multiply(T alpha, T[] x, T beta, T[] y);

        /// <summary>
        /// Computes the matrix-vector product y = alpha * A^t * x + beta * y.
        /// </summary>
        void TransposeMultiply(T alpha, T[] x, T beta, T[] y);
    }
}
