// -----------------------------------------------------------------------
// <copyright file="CompressedColumnStorage.cs">
// Original CSparse code by Tim Davis, http://www.cise.ufl.edu/research/sparse/CXSparse/
// CSparse.NET code by Christian Woltering, http://csparse.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace BriefFiniteElementNet.CSparse.Storage
{
    //using System.Numerics;

    
    /// <summary>
    /// Compressed sparse column storage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class CompressedColumnStorage<T> : SparseMatrixStorage<T>
        where T : struct, IEquatable<T>, IFormattable
    {
        /// <summary>
        /// Row pointers with last entry equal number of non-zeros (size = RowCount + 1)
        /// </summary>
        public int[] ColumnPointers;

        /// <summary>
        /// Column indices (size >= NonZerosCount)
        /// </summary>
        public int[] RowIndices;

        /// <summary>
        /// Numerical values (size >= NonZerosCount)
        /// </summary>
        public T[] Values;

        /// <summary>
        /// Gets the number of non-zero entries.
        /// </summary>
        public override int NonZerosCount
        {
            get { return ColumnPointers[ncols]; }
        }

        /// <summary>
        /// Initializes a new instance of the CompressedColumnStorage class.
        /// </summary>
        public CompressedColumnStorage(int rowCount, int columnCount)
            : base(rowCount, columnCount)
        {
            // No array initialization here
        }

        /// <summary>
        /// Initializes a new instance of the CompressedColumnStorage class.
        /// </summary>
        public CompressedColumnStorage(int rowCount, int columnCount, int valueCount)
            : base(rowCount, columnCount)
        {
            this.ColumnPointers = new int[columnCount + 1];
            this.RowIndices = new int[valueCount];

            if (valueCount > 0)
            {
                this.Values = new T[valueCount];
            }
        }

        /// <summary>
        /// Return the matrix value at position (row, column).
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <returns>Matrix value</returns>
        public override T At(int row, int column)
        {
            int index = ColumnPointers[column];
            int length = ColumnPointers[column + 1] - index;
            int pos = Array.BinarySearch(RowIndices, index, length, row);
            return pos >= 0 ? Values[pos] : Zero;
        }

        /// <summary>
        /// Clears all values form the matrix.
        /// </summary>
        /// <remarks>
        /// The method does not release memory.
        /// </remarks>
        public override void Clear()
        {
            Array.Clear(ColumnPointers, 0, ColumnPointers.Length);
            Array.Clear(Values, 0, Values.Length);
        }

        /// <summary>
        /// Returns the transpose of this matrix.
        /// </summary>
        public virtual CompressedColumnStorage<T> Transpose()
        {
            var result = CompressedColumnStorage<T>.Create(ncols, nrows, this.NonZerosCount);
            this.Transpose(result);
            return result;
        }

        /// <summary>
        /// Transpose this matrix and store the result in given matrix.
        /// </summary>
        public virtual void Transpose(CompressedColumnStorage<T> result)
        {
            int i, j, p;

            var cx = result.Values;
            var cp = result.ColumnPointers;
            var ci = result.RowIndices;

            int[] w = new int[nrows];

            for (p = 0; p < ColumnPointers[ncols]; p++)
            {
                // Row counts.
                w[RowIndices[p]]++;
            }

            // Row pointers.
            Helper.CumulativeSum(cp, w, nrows);

            for (i = 0; i < ncols; i++)
            {
                for (p = ColumnPointers[i]; p < ColumnPointers[i + 1]; p++)
                {
                    j = w[RowIndices[p]]++;

                    // Place A(i,j) as entry C(j,i)
                    ci[j] = i;
                    cx[j] = Values[p];
                }
            }
        }

        /// <summary>
        /// Returns a clone of this matrix.
        /// </summary>
        /// <param name="values">If true (default), the values are copied.</param>
        public CompressedColumnStorage<T> Clone(bool values = true)
        {
            int m = this.RowCount;
            int n = this.ColumnCount;
            int nnz = this.NonZerosCount;

            var ap = this.ColumnPointers;
            var ai = this.RowIndices;

            var result = CompressedColumnStorage<T>.Create(m, n, values ? nnz : 0);

            if (values)
            {
                Buffer.BlockCopy(ap, 0, result.ColumnPointers, 0, (m + 1) * Constants.SizeOfInt);
                Buffer.BlockCopy(ai, 0, result.RowIndices, 0, nnz * Constants.SizeOfInt);

                Array.Copy(this.Values, 0, result.Values, 0, nnz);
            }
            else
            {
                result.RowIndices = new int[nnz];

                Buffer.BlockCopy(ap, 0, result.ColumnPointers, 0, (m + 1) * Constants.SizeOfInt);
                Buffer.BlockCopy(ai, 0, result.RowIndices, 0, nnz * Constants.SizeOfInt);
            }

            return result;
        }


        /// <summary>
        /// TODO: doc
        /// </summary>
        /// <param name="perm"></param>
        public void PermuteRows(int[] perm)
        {
            var ax = this.Values;
            var ap = this.ColumnPointers;
            var ai = this.RowIndices;

            // TODO: invert perm?
            PermuteRows(ax, ap, ai, ax, ap, ai, perm);

            SortIndices();
        }

        /// <summary>
        /// TODO: doc
        /// </summary>
        /// <param name="perm"></param>
        public void PermuteColumns(int[] perm)
        {
            var ax = this.Values;
            var ap = this.ColumnPointers;
            var ai = this.RowIndices;

            // TODO: is cloning needed?
            var bx = (T[])ax.Clone();
            var bp = (int[])ap.Clone();
            var bi = (int[])ai.Clone();

            PermuteColumns(ax, ap, ai, bx, bp, bi, perm);

            this.Values = bx;
            this.ColumnPointers = bp;
            this.RowIndices = bi;

            SortIndices();
        }

        /// <summary>
        /// Returns the positions of the diagonal elements of a sparse matrix.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="throwOnMissingDiag"></param>
        /// <returns></returns>
        public int[] FindDiagonalIndices(bool throwOnMissingDiag = false)
        {
            int n = this.ColumnCount;

            var ap = this.ColumnPointers;
            var ai = this.RowIndices;

            int[] diag = new int[n];

            for (int i = 0; i < n; i++)
            {
                diag[i] = Array.BinarySearch(ai, ap[i], ap[i + 1] - ap[i], i);

                if (diag[i] < 0 && throwOnMissingDiag)
                {
                    throw new Exception("Missing diagonal entry on row " + (i + 1));
                }
            }

            return diag;
        }

        #region Permutation methods

        /// <summary>
        /// Permutes the columns of a matrix in CSC format, B = A * P, where P represents
        /// a permutation matrix.
        /// </summary>
        /// <param name="ax">Input matrix values.</param>
        /// <param name="ai">Input matrix row pointers.</param>
        /// <param name="aj">Input matrix column indices.</param>
        /// <param name="bx">Output matrix values.</param>
        /// <param name="bi">Output matrix row pointers.</param>
        /// <param name="bj">Output matrix column indices.</param>
        /// <param name="perm">Permutation array of length ColumnCount.</param>
        /// <remarks>
        /// The permutation P is defined through the array perm: for each j,
        /// perm(j) represents the destination row number of row number j:
        /// 
        /// a(i,j) in the original matrix becomes a(perm(i),j) in the output matrix.
        /// </remarks>
        protected void PermuteColumns(T[] ax, int[] ai, int[] aj, T[] bx, int[] bi, int[] bj, int[] perm)
        {
            int k;

            // Determine pointers for output matix. 
            for (int i = 0; i < ncols; i++)
            {
                k = perm[i];
                bi[k + 1] = ai[i + 1] - ai[i];
            }

            // Get pointers from lengths
            bi[0] = 0;
            for (int i = 0; i < ncols; i++)
            {
                bi[i + 1] = bi[i + 1] + bi[i];
            }

            // Copying
            for (int i = 0; i < ncols; i++)
            {
                // Old row = i, new row = perm(i), k = new pointer
                k = bi[perm[i]];
                for (int j = ai[i]; j < ai[i + 1]; j++)
                {
                    bj[k] = aj[j];
                    bx[k] = ax[j];
                    k = k + 1;
                }
            }
        }

        /// <summary>
        /// Permute the rows of a matrix in CSC format, B = P * A, where P represents
        /// a permutation matrix. 
        /// </summary>
        /// <param name="ax">Input matrix values.</param>
        /// <param name="ai">Input matrix row pointers.</param>
        /// <param name="aj">Input matrix column indices.</param>
        /// <param name="bx">Output matrix values.</param>
        /// <param name="bi">Output matrix row pointers.</param>
        /// <param name="bj">Output matrix column indices.</param>
        /// <param name="perm">Permutation array of length RowCount.</param>
        /// <param name="copy">Copy matrix values (not needed if used 'in place').</param>
        /// <remarks>
        /// The permutation matrix P maps column j into column perm(j), i.e., 
        /// on return a(i,j) in the original matrix becomes a(i,perm(j)) in the
        /// output matrix.
        /// 
        /// Notes:
        /// 
        /// 1. This routine is in place: aj, bj can be the same.
        /// 2. If the matrix is initially sorted (by increasing column number) 
        ///    then bx, bi, bj may not be on return.
        /// </remarks>
        protected void PermuteRows(T[] ax, int[] ai, int[] aj, T[] bx, int[] bi, int[] bj,
            int[] perm, bool copy = false)
        {
            int i, nnz = ai[ncols];

            for (i = 0; i < nnz; i++)
            {
                bj[i] = perm[aj[i]];
            }

            if (copy)
            {
                Array.Copy(ax, bx, nnz);
                Array.Copy(ai, bi, ncols);
            }
        }

        #endregion

        #region Internal methods

        internal static CompressedColumnStorage<T> Create(int rowCount, int columnCount)
        {
            if (typeof(T) == typeof(double))
            {
                return new CSparse.Double.CompressedColumnStorage(rowCount, columnCount)
                    as CompressedColumnStorage<T>;
            }

            

            throw new NotSupportedException();
        }

        internal static CompressedColumnStorage<T> Create(int rowCount, int columnCount, int valueCount)
        {
            if (typeof(T) == typeof(double))
            {
                return new CSparse.Double.CompressedColumnStorage(rowCount, columnCount, valueCount)
                    as CompressedColumnStorage<T>;
            }

            

            throw new NotSupportedException();
        }
        
        /// <summary>
        /// Change the max # of entries sparse matrix
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        internal bool Resize(int size)
        {
            if (size <= 0)
            {
                size = this.ColumnPointers[ncols];
            }

            Array.Resize<int>(ref this.RowIndices, size);
            Array.Resize<T>(ref this.Values, size);

            return true;
        }

        internal void SortIndices()
        {
            int from, size, to, p, q, idx;
            T val;

            for (int i = 0; i < ncols; i++)
            {
                from = ColumnPointers[i];
                to = ColumnPointers[i + 1] - 1;

                size = to - from + 1;

                if (size > 16)
                {
                    // Quicksort
                    Array.Sort(RowIndices, Values, from, size);
                }
                else
                {
                    // Insertion sort
                    for (p = from + 1; p <= to; p++)
                    {
                        idx = RowIndices[p];
                        val = Values[p];
                        q = p - 1;
                        while (q >= from && RowIndices[q] > idx)
                        {
                            RowIndices[q + 1] = RowIndices[q];
                            Values[q + 1] = Values[q];
                            q--;
                        }
                        RowIndices[q + 1] = idx;
                        Values[q + 1] = val;
                    }
                }
            }
        }

        internal abstract void Cleanup();

        internal abstract int Scatter(int j, T beta, int[] w, T[] x, int mark, CompressedColumnStorage<T> mat, int nzz);

        #endregion

        #region Storage equality

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            var hashNum = Math.Min(this.NonZerosCount, 25);
            int hash = 17;
            int i, p, j, k = 0;
            unchecked
            {
                for (i = 0; i < ncols; i++)
                {
                    for (p = ColumnPointers[i]; p < ColumnPointers[i + 1]; p++)
                    {
                        j = RowIndices[p];

                        hash = hash * 31 + Values[p].GetHashCode();

                        if (++k > hashNum)
                        {
                            return hash;
                        }
                    }
                }
            }
            return hash;
        }

        #endregion

        
    }
}
