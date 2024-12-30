//internal class copied
//https://github.com/wo80/CSparse.NET/blob/master/CSparse/Storage/SymbolicColumnStorage.cs


/* Unmerged change from project 'BriefFiniteElementNet (net6.0)'
Before:
namespace CSparse.Storage
After:
namespace BriefFiniteElementNet.Mathh.StiffnessPermutationMatrix.Csparsenet
*/

namespace BriefFiniteElementNet.Mathh.StiffnessPermutationMatrix.Csparsenet
{
    using CSparse;
    using CSparse.Properties;
    using CSparse.Storage;
    using System;
    using System.Resources;

    /// <summary>
    /// Represents the nonzero pattern of a column-compressed matrix.
    /// </summary>
    /// <remarks>
    /// Used for ordering and symbolic factorization.
    /// </remarks>
    internal class SymbolicColumnStorage
    {
        private int rowCount;
        private int columnCount;

        /// <summary>
        /// Column pointers with last entry equal number of non-zeros (size = ColumnCount + 1)
        /// </summary>
        public int[] ColumnPointers;

        /// <summary>
        /// Row indices (size = NonZerosCount)
        /// </summary>
        public int[] RowIndices;

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        public int RowCount
        {
            get { return rowCount; }
        }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        public int ColumnCount
        {
            get { return columnCount; }
        }

        /// <summary>
        /// Gets the number of non-zero entries.
        /// </summary>
        public int NonZerosCount
        {
            get { return ColumnPointers[columnCount]; }
        }

        public SymbolicColumnStorage(int rowCount, int columnCount, int valueCount, bool allocate)
        {
            // Explicitly allow m or n = 0 (may occur in Dulmage-Mendelsohn decomposition).
            if (rowCount < 0 || columnCount < 0)
            {
                throw new ArgumentOutOfRangeException("s");
            }

            this.rowCount = rowCount;
            this.columnCount = columnCount;

            if (allocate)
            {
                ColumnPointers = new int[columnCount + 1];
                RowIndices = new int[valueCount];
            }
        }

        /// <summary>
        /// Change the shape of the matrix (only used by Dulmage-Mendelsohn decomposition).
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="columnCount"></param>
        internal void Reshape(int rowCount, int columnCount)
        {
            if (rowCount >= 0)
            {
                this.rowCount = rowCount;
            }
            if (columnCount >= 0)
            {
                this.columnCount = columnCount;
                //Array.Resize(ref this.ColumnPointers, columnCount + 1);
            }
        }

        /// <summary>
        /// Change the max # of entries sparse matrix
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool Resize(int size)
        {
            if (size <= 0)
            {
                size = ColumnPointers[columnCount];
            }

            Array.Resize(ref RowIndices, size);

            return true;
        }

        /// <summary>
        /// Sort column indices using insertion sort.
        /// </summary>
        public void Sort()
        {
            int from, size, to, p, q, idx;

            for (int i = 0; i < columnCount; i++)
            {
                from = ColumnPointers[i];
                to = ColumnPointers[i + 1] - 1;

                size = to - from + 1;

                if (size > 16)
                {
                    // Quicksort
                    Array.Sort(RowIndices, from, size);
                }
                else
                {
                    // Insertion sort
                    for (p = from + 1; p <= to; p++)
                    {
                        idx = RowIndices[p];
                        q = p - 1;
                        while (q >= from && RowIndices[q] > idx)
                        {
                            RowIndices[q + 1] = RowIndices[q];
                            q--;
                        }
                        RowIndices[q + 1] = idx;
                    }
                }
            }
        }

        public SymbolicColumnStorage Clone()
        {
            int m = RowCount;
            int n = ColumnCount;
            int nnz = NonZerosCount;

            var result = new SymbolicColumnStorage(m, n, nnz, true);

            Buffer.BlockCopy(ColumnPointers, 0, result.ColumnPointers, 0, (n + 1) * Constants.SizeOfInt);
            Buffer.BlockCopy(RowIndices, 0, result.RowIndices, 0, nnz * Constants.SizeOfInt);

            return result;
        }

        #region Linear Algebra (symbolic matrix operations)

        /// <summary>
        /// Computes the transpose of a sparse matrix, C = A';
        /// </summary>
        /// <returns>Transposed matrix, C = A'</returns>
        public virtual SymbolicColumnStorage Transpose()
        {
            int j, k, p;

            int m = RowCount;
            int n = ColumnCount;

            var result = new SymbolicColumnStorage(n, m, 0, false);

            var ci = new int[m + 1];
            var cj = new int[RowIndices.Length];

            int[] w = new int[rowCount];

            for (p = 0; p < ColumnPointers[columnCount]; p++)
            {
                // Row counts.
                w[RowIndices[p]]++;
            }

            // Column pointers.
            Helper.CumulativeSum(ci, w, rowCount);

            for (j = 0; j < columnCount; j++)
            {
                for (p = ColumnPointers[j]; p < ColumnPointers[j + 1]; p++)
                {
                    k = w[RowIndices[p]]++;

                    // Place A(i,j) as entry C(j,i)
                    cj[k] = j;
                }
            }

            result.ColumnPointers = ci;
            result.RowIndices = cj;

            return result;
        }

        /// <summary>
        /// Symbolic sum C = A + B
        /// </summary>
        /// <param name="other">column-compressed matrix</param>
        /// <returns>Sum C = A + B</returns>
        public SymbolicColumnStorage Add(SymbolicColumnStorage other)
        {
            int j, nz = 0;

            // check inputs
            if (rowCount != other.rowCount || columnCount != other.columnCount)
            {
                throw new ArgumentException();
            }

            int m = rowCount;
            int n = other.columnCount;

            var bi = other.ColumnPointers;

            int anz = ColumnPointers[columnCount];
            int bnz = bi[n];

            // Workspace
            var w = new int[m];

            // Allocate result: (anz + bnz) is an upper bound
            var cp = new int[bi.Length];
            var ci = new int[anz + bnz];

            for (j = 0; j < n; j++)
            {
                // Column j of result starts here
                cp[j] = nz;
                nz = Scatter(j, w, j + 1, ci, nz); // A(:,j)
                nz = other.Scatter(j, w, j + 1, ci, nz); // B(:,j)
            }

            // Finalize the last column
            cp[n] = nz;

            // Remove extra space
            Array.Resize(ref ci, nz);

            var result = new SymbolicColumnStorage(m, n, 0, false);

            result.ColumnPointers = cp;
            result.RowIndices = ci;

            return result;
        }

        /// <summary>
        /// Sparse matrix multiplication, C = A*B
        /// </summary>
        /// <param name="other">column-compressed matrix</param>
        /// <returns>C = A*B</returns>
        public SymbolicColumnStorage Multiply(SymbolicColumnStorage other)
        {
            int p, j, nz = 0;

            if (columnCount != other.rowCount)
            {
                throw new ArgumentException();
            }

            int m = rowCount;
            int n = other.columnCount;

            int anz = NonZerosCount;
            int bnz = other.NonZerosCount;

            var bp = other.ColumnPointers;
            var bi = other.RowIndices;

            var result = new SymbolicColumnStorage(m, n, 0, false);

            var cp = new int[n + 1];
            var ci = new int[2 * Math.Max(anz, bnz)];

            int[] work = new int[m];

            for (j = 0; j < n; j++)
            {
                if (nz + m > ci.Length)
                {
                    Array.Resize(ref ci, 2 * ci.Length + m);
                }

                // Column j of C starts here
                cp[j] = nz;

                for (p = bp[j]; p < bp[j + 1]; p++)
                {
                    nz = Scatter(bi[p], work, j + 1, ci, nz);
                }
            }

            cp[n] = nz;

            // Remove extra space from C
            Array.Resize(ref ci, nz);

            result.ColumnPointers = cp;
            result.RowIndices = ci;

            return result;
        }

        //public abstract SparseColumnStorage<T> Permute(int[] perm, int[] qperm = null, bool symbolic = false);

        /// <summary>
        /// Permutes a sparse matrix, C = PAQ.
        /// </summary>
        /// <param name="pinv">Permutation vector of length m.</param>
        /// <param name="q">Permutation vector of length n.</param>
        /// <param name="result">Permuted matrix, C = PAQ.</param>
        public virtual void Permute(int[] pinv, int[] q, SymbolicColumnStorage result)
        {
            int i, j, k, nz = 0;

            int n = columnCount;

            int[] ap = ColumnPointers;
            int[] ai = RowIndices;

            // Allocate memory if needed.
            if (result.ColumnPointers == null)
            {
                result.ColumnPointers = new int[ap.Length];
                result.RowIndices = new int[ai.Length];
            }

            int[] cp = result.ColumnPointers;
            int[] ci = result.RowIndices;

            for (k = 0; k < n; k++)
            {
                cp[k] = nz; // column k of C is column q[k] of A
                j = q != null ? q[k] : k;
                for (i = ap[j]; i < ap[j + 1]; i++)
                {
                    ci[nz++] = pinv != null ? pinv[ai[i]] : ai[i];
                }
            }

            // Finalize the last column of result matrix.
            cp[n] = nz;
        }

        #endregion

        /// <summary>
        /// Drops entries from a sparse matrix
        /// </summary>
        /// <param name="func">Drop element a_{i,j} if func(i, j) is false.</param>
        /// <returns>New number of entries in A.</returns>
        internal int Keep(Func<int, int, bool> func)
        {
            int i, j, nz = 0;

            for (j = 0; j < columnCount; j++)
            {
                i = ColumnPointers[j];

                // Record new location of col j.
                ColumnPointers[j] = nz;

                for (; i < ColumnPointers[j + 1]; i++)
                {
                    if (func(RowIndices[i], j))
                    {
                        // Keep A(i,j).
                        RowIndices[nz] = RowIndices[i];
                        nz++;
                    }
                }
            }

            // Record new nonzero count.
            ColumnPointers[columnCount] = nz;

            // Remove extra space.
            Array.Resize(ref RowIndices, nz);

            return nz;
        }

        /// <summary>
        /// Scatters and sums a sparse vector A(:,j) into a dense vector, x = x + beta * A(:,j).
        /// </summary>
        /// <param name="j">the column of A to use</param>
        /// <param name="work">size m, node i is marked if w[i] = mark</param>
        /// <param name="mark">mark value of w</param>
        /// <param name="ci">pattern of x accumulated in ci</param>
        /// <param name="nz">pattern of x placed in C starting at C.i[nz]</param>
        /// <returns>new value of nz</returns>
        private int Scatter(int j, int[] work, int mark, int[] ci, int nz)
        {
            int i, p;

            for (p = ColumnPointers[j]; p < ColumnPointers[j + 1]; p++)
            {
                i = RowIndices[p]; // A(i,j) is nonzero
                if (work[i] < mark)
                {
                    work[i] = mark; // i is new entry in column j
                    ci[nz++] = i; // add i to pattern of C(:,j)
                }
            }

            return nz;
        }

        internal static SymbolicColumnStorage Create<T>(CompressedColumnStorage<T> mat, bool allocate = true)
             where T : struct, IEquatable<T>, IFormattable
        {
            int m = mat.RowCount;
            int n = mat.ColumnCount;
            int nnz = mat.NonZerosCount;

            var result = new SymbolicColumnStorage(m, n, nnz, allocate);

            if (allocate)
            {
                Buffer.BlockCopy(mat.ColumnPointers, 0, result.ColumnPointers, 0, (n + 1) * Constants.SizeOfInt);
                Buffer.BlockCopy(mat.RowIndices, 0, result.RowIndices, 0, nnz * Constants.SizeOfInt);
            }
            else
            {
                result.ColumnPointers = mat.ColumnPointers;
                result.RowIndices = mat.RowIndices;
            }

            return result;
        }
    }
}
