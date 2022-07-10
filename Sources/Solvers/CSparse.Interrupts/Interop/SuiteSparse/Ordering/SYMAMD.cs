namespace CSparse.Interop.SuiteSparse.Ordering
{
    using CSparse.Storage;
    using System;

    /// <summary>
    /// SYMAMD is a symmetric minimum degree ordering method based on COLAMD. It
    /// constructs a matrix M such that M'*M has the same pattern as A, and then
    /// uses COLAMD to compute a column ordering of M.
    /// </summary>
    public class SYMAMD
    {
        #region Publi properties (SYMAMD options)

        /// <summary>
        /// Gets or sets a value used to determine whether or not a given input row is "dense" (default = 10.0).
        /// </summary>
        /// <remarks>
        /// Rows with more than max(16, DenseRow * sqrt(n_col)) entries are removed prior to ordering.
        /// </remarks>
        public double DenseRow { get; set; } = 10.0;

        /// <summary>
        /// Gets or sets a value used to determine whether or not aggressive absorption is to be performed (default = true).
        /// </summary>
        public bool Agressive { get; set; } = true;

        #endregion

        /// <summary>
        /// Generate a fill-reducing ordering using the SYMAMD algorithm.
        /// </summary>
        /// <param name="A">The matrix.</param>
        /// <returns>The permutation vector or NULL, if an error occurred.</returns>
        public static int[] Generate<T>(CompressedColumnStorage<T> A)
            where T : struct, IEquatable<T>, IFormattable
        {
            int n = A.RowCount;

            if (n != A.ColumnCount)
            {
                throw new ArgumentException("Matrix must be square.", nameof(A));
            }

            var p = new int[n + 1];

            var amd = new SYMAMD();

            var info = amd.Order(n, A.RowIndices, A.ColumnPointers, p);

            if (info.Status < COLAMD.COLAMD_OK)
            {
                // error
                return null;
            }

            return p;
        }

        /// <summary>
        /// Generate a fill-reducing ordering using the CSYMAMD algorithm.
        /// </summary>
        /// <param name="A">The matrix.</param>
        /// <param name="constraints">Constraint set of A (of size ncol).</param>
        /// <param name="stype">0: use both parts, &gt;0: upper, &lt;0: lower.</param>
        /// <returns>The permutation vector or NULL, if an error occurred.</returns>
        public static int[] Generate<T>(CompressedColumnStorage<T> A, int[] constraints, int stype)
            where T : struct, IEquatable<T>, IFormattable
        {
            int n = A.RowCount;

            if (n != A.ColumnCount)
            {
                throw new ArgumentException("Matrix must be square.", nameof(A));
            }

            var p = new int[n + 1];

            var amd = new SYMAMD();

            var info = amd.Order(n, A.RowIndices, A.ColumnPointers, p, constraints, stype);

            if (info.Status < COLAMD.COLAMD_OK)
            {
                // error
                return null;
            }

            return p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n">Number of rows and columns in the symmetrix matrix A.</param>
        /// <param name="A">Row indices of A (of size nnz).</param>
        /// <param name="p">Column pointers of A (of size n+1).</param>
        /// <param name="perm">The permutation vector (of size n+1).</param>
        /// <returns></returns>
        /// <remarks>
        /// The array <paramref name="perm"/> is used as a workspace during the ordering,
		/// which is why it must be of length n+1, not just n.
        /// </remarks>
        public COLAMD.Info Order(int n, int[] A, int[] p, int[] perm)
        {
            var amd = new COLAMD();

            return amd.SymOrder(n, A, p, perm, GetControl());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n">Number of rows and columns in the symmetrix matrix A.</param>
        /// <param name="A">Row indices of A (of size nnz).</param>
        /// <param name="p">Column pointers of A (of size n+1).</param>
        /// <param name="perm">The permutation vector (of size n+1).</param>
        /// <param name="cmember">Constraint set of A (of size n).</param>
        /// <param name="stype">0: use both parts, &gt;0: upper, &lt;0: lower.</param>
        /// <returns></returns>
        /// <remarks>
        /// The array <paramref name="perm"/> is used as a workspace during the ordering,
		/// which is why it must be of length n+1, not just n.
        /// 
        /// If <paramref name="stype"/> &lt; 0, then only the strictly lower triangular
        /// part of A is accessed. The upper triangular part is assumed to be the transpose
        /// of the lower triangular part.  This is the same as SYMAMD, which did not have
        /// an stype parameter.
        ///
        /// If <paramref name="stype"/> &gt; 0, only the strictly upper triangular part
        /// of A is accessed. The lower triangular part is assumed to be the transpose
        /// of the upper triangular part.
        /// 
        /// If <paramref name="stype"/> == 0, then the nonzero pattern of A+A' is ordered.
        /// </remarks>
        public COLAMD.Info Order(int n, int[] A, int[] p, int[] perm, int[] cmember, int stype)
        {
            var amd = new COLAMD();

            return amd.SymOrder(n, A, p, perm, GetControl(), cmember, stype);
        }

        private double[] GetControl()
        {
            var a = new double[COLAMD.COLAMD_KNOBS];

            a[0] = DenseRow;
            a[2] = Agressive ? 1.0 : 0.0;

            return a;
        }
    }
}
