// -----------------------------------------------------------------------
// <copyright file="CompressedColumnStorage.cs">
// Original CSparse code by Tim Davis, http://www.cise.ufl.edu/research/sparse/CXSparse/
// CSparse.NET code by Christian Woltering, http://csparse.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;

namespace CSparse.Double
{
    using System;
    using CSparse.Storage;

    [Serializable]
    /// <inheritdoc />
    public class CompressedColumnStorage : CompressedColumnStorage<double>
    {
        /// <inheritdoc />
        public CompressedColumnStorage(int rowCount, int columnCount)
            : base(rowCount, columnCount)
        {
        }

        /// <inheritdoc />
        public CompressedColumnStorage(int rowCount, int columnCount, int valueCount)
            : base(rowCount, columnCount, valueCount)
        {
        }

        #region Public functions

        /// <summary>
        /// Removes numerically zero entries from a matrix.
        /// </summary>
        /// <param name="tolerance">Drop tolerance (default is 0.0)</param>
        /// <returns>The new number of nonzero entries.</returns>
        public override int DropZeros(double tolerance = 0.0)
        {
            Func<int, int, double, double, bool> func;

            if (tolerance <= 0.0)
            {
                func = (i, j, aij, other) =>
                {
                    return (aij != 0.0);
                };
            }
            else
            {
                func = (i, j, aij, tol) =>
                {
                    return (Math.Abs(aij) > tol);
                };
            }

            return Keep(func, tolerance);
        }

        /// <summary>
        /// Drops entries from a sparse matrix
        /// </summary>
        /// <param name="func">drop aij if fkeep.fkeep(i,j,aij,other) is false</param>
        /// <param name="tolerance">optional parameter to fkeep</param>
        /// <returns>nz, new number of entries in A, -1 on error</returns>
        public override int Keep(Func<int, int, double, double, bool> func, double tolerance)
        {
            int i, j, nz = 0;

            for (j = 0; j < ncols; j++)
            {
                i = ColumnPointers[j];

                // Record new location of col j.
                ColumnPointers[j] = nz;

                for (; i < ColumnPointers[j + 1]; i++)
                {
                    if (func(RowIndices[i], j, Values[i], tolerance))
                    {
                        // Keep A(i,j).
                        Values[nz] = Values[i];
                        RowIndices[nz] = RowIndices[i];
                        nz++;
                    }
                }
            }

            // Record new nonzero count.
            ColumnPointers[ncols] = nz;

            // Remove extra space.
            this.Resize(0);

            return nz;
        }

        /// <summary>
        /// Computes the norm of a sparse matrix.
        /// </summary>
        /// <returns>The requested norm of the matrix.</returns>
        public override double Norm(MatrixNorm which)
        {
            int nz = this.NonZerosCount;

            double sum, norm = 0.0;

            if (which == MatrixNorm.OneNorm)
            {
                for (int j = 0; j < ncols; j++)
                {
                    sum = 0.0;
                    for (int i = ColumnPointers[j]; i < ColumnPointers[j + 1]; i++)
                    {
                        sum += Math.Abs(Values[i]);
                    }
                    norm = Math.Max(norm, sum);
                }
            }
            else if (which == MatrixNorm.FrobeniusNorm)
            {
                sum = 0.0;
                for (int i = 0; i < nz; i++)
                {
                    sum = Math.Abs(Values[i]);
                    norm += sum * sum;
                }
                norm = Math.Sqrt(norm);

            }
            else if (which == MatrixNorm.InfinityNorm)
            {
                var work = new double[nrows];
                for (int j = 0; j < ncols; j++)
                {
                    for (int i = ColumnPointers[j]; i < ColumnPointers[j + 1]; i++)
                    {
                        work[RowIndices[i]] += Math.Abs(Values[i]);
                    }
                }
                for (int j = 0; j < nrows; j++)
                {
                    norm = Math.Max(norm, work[j]);
                }
            }
            else
            {
                for (int i = 0; i < nz; i++)
                {
                    norm = Math.Max(norm, Math.Abs(Values[i]));
                }
            }

            return norm;
        }

        #endregion
        
        #region Linear Algebra (Vector)

        /// <summary>
        /// Multiplies a (m-by-n) matrix by a vector, y = A*x + y. 
        /// </summary>
        /// <param name="x">Vector of length n (column count).</param>
        /// <param name="y">Vector of length m (row count), containing the result.</param>
        /// <remarks>
        /// Input values of vector y will be accumulated.
        /// </remarks>
        public void Multiply(double[] x, double[] y)
        {
            if (this.ColumnCount != x.Length || this.RowCount != y.Length)
                throw new Exception("Dimension Mismatch");

            var ax = this.Values;
            var ap = this.ColumnPointers;
            var ai = this.RowIndices;
            var tmp = ai.Max();

            int end;

            for (int j = 0; j < ncols; j++)
            {
                end = ap[j + 1];

                // Loop over the rows
                for (int k = ap[j]; k < end; k++)
                {
                    var tt = ai[k];
                    y[ai[k]] += x[j] * ax[k];
                }
            }
        }




        /// <summary>
        /// Multiplies a (m-by-n) matrix by a vector, y = alpha*A*x + beta*y. 
        /// </summary>
        /// <param name="x">Vector of length n (column count).</param>
        /// <param name="y">Vector of length m (row count), containing the result.</param>
        /// <param name="alpha">Scalar to multiply with matrix.</param>
        /// <param name="beta">Scalar to multiply with vector y.</param>
        /// <remarks>
        /// Input values of vector y will be accumulated.
        /// </remarks>
        public override void Multiply(double alpha, double[] x, double beta, double[] y)
        {
            var ax = this.Values;
            var ap = this.ColumnPointers;
            var ai = this.RowIndices;

            // Scale y by beta
            for (int j = 0; j < nrows; j++)
            {
                y[j] = beta * y[j];
            }

            int end;
            double xi;

            for (int i = 0; i < ncols; i++)
            {
                xi = alpha * x[i];

                end = ap[i + 1];

                for (int k = ap[i]; k < end; k++)
                {
                    y[ai[k]] += ax[k] * xi;
                }
            }
        }

        /// <summary>
        /// Multiplies the transpose of a (m-by-n) matrix by a vector, y = A'*x + y. 
        /// </summary>
        /// <param name="x">Vector of length m (column count of A').</param>
        /// <param name="y">Vector of length n (row count of A'), containing the result.</param>
        /// <remarks>
        /// Input values of vector y will be accumulated.
        /// </remarks>
        public void TransposeMultiply(double[] x, double[] y)
        {
            var ax = this.Values;
            var ap = this.ColumnPointers;
            var ai = this.RowIndices;

            double yi;

            for (int i = 0; i < ncols; i++)
            {
                yi = 0.0;

                // Compute the inner product of row i with vector x
                for (int k = ap[i]; k < ap[i + 1]; k++)
                {
                    yi += ax[k] * x[ai[k]];
                }

                // Store result in y(i) 
                y[i] += yi;
            }
        }

        /// <summary>
        /// Multiplies the transpose of a (m-by-n) matrix by a vector, y = alpha*A'*x + beta*y. 
        /// </summary>
        /// <param name="x">Vector of length m (column count of A').</param>
        /// <param name="y">Vector of length n (row count of A'), containing the result.</param>
        /// <param name="alpha">Scalar to multiply with matrix.</param>
        /// <param name="beta">Scalar to multiply with vector y.</param>
        /// <remarks>
        /// Input values of vector y will be accumulated.
        /// </remarks>
        public override void TransposeMultiply(double alpha, double[] x, double beta, double[] y)
        {
            var ax = this.Values;
            var ap = this.ColumnPointers;
            var ai = this.RowIndices;

            double yi;

            int end, start = ap[0];

            for (int i = 0; i < ncols; i++)
            {
                end = ap[i + 1];

                yi = beta * y[i];
                for (int k = start; k < end; k++)
                {
                    yi += alpha * ax[k] * x[ai[k]];
                }
                y[i] = yi;

                start = end;
            }
        }

        #endregion

        /// <summary>
        /// Adds two matrices in CSC format, C = A + B, where A is current instance.
        /// </summary>
        public CompressedColumnStorage Add(CompressedColumnStorage other)
        {
            int m = this.nrows;
            int n = this.ncols;

            // check inputs
            if (m != other.RowCount || n != other.ColumnCount)
            {
                throw new ArgumentException(); // TODO: ex
            }

            var result = new CompressedColumnStorage(m, n, this.NonZerosCount + other.NonZerosCount);

            Add(1.0, 1.0, other, result);

            return result;
        }

        /// <summary>
        /// Sums two matrices.
        /// </summary>
        /// <param name="alpha">Scalar factor for (this).</param>
        /// <param name="beta">Scalar factor for (other).</param>
        /// <param name="other">The matrix added to this instance.</param>
        /// <param name="result">Contains the sum: alpha*(this) + beta*(other).</param>
        /// <remarks>
        /// The (result) matrix has to be fully initialized and provide enough space for
        /// the nonzero entries of the sum. An upper bound is the sum of the nonzeros count
        /// of (this) and (other).
        /// </remarks>
        public void Add(double alpha, double beta, CompressedColumnStorage<double> other,
            CompressedColumnStorage<double> result)
        {
            int p, j, nz = 0;

            int m = this.nrows;
            int n = this.ncols;

            // check inputs
            if (m != other.RowCount || n != other.ColumnCount)
            {
                throw new ArgumentException(); // TODO: ex
            }

            var bi = other.ColumnPointers;
            var bx = other.Values;

            int anz = this.ColumnPointers[ncols];
            int bnz = bi[n];

            // Workspace
            var w = new int[m];
            var x = new double[m];

            // Allocate result: (anz + bnz) is an upper bound

            var ci = result.ColumnPointers;
            var cj = result.RowIndices;
            var cx = result.Values;

            for (j = 0; j < n; j++)
            {
                ci[j] = nz; // column j of C starts here
                nz = this.Scatter(j, alpha, w, x, j + 1, result, nz); // alpha*A(:,j)
                nz = other.Scatter(j, beta, w, x, j + 1, result, nz); // beta*B(:,j)

                for (p = ci[j]; p < nz; p++)
                {
                    cx[p] = x[cj[p]];
                }
            }

            // Finalize the last column
            ci[n] = nz;

            // Remove extra space
            result.Resize(0);
            result.SortIndices();
        }

        /// <summary>
        /// Sparse matrix multiplication, C = A*B
        /// </summary>
        /// <param name="other">column-compressed matrix</param>
        /// <returns>C = A*B, null on error</returns>
        public CompressedColumnStorage<double> Multiply(CompressedColumnStorage<double> other)
        {
            int m = this.nrows;
            int n = other.ColumnCount;

            int anz = this.NonZerosCount;
            int bnz = other.NonZerosCount;
            
            int p, j, nz = 0;
            int[] cp, ci, bp, bi;
            double[] bx, cx;

            // check inputs
            if (other == null) throw new Exception();
            if (this.ColumnCount != other.RowCount) throw new Exception();

            bp = other.ColumnPointers;
            bi = other.RowIndices;
            bx = other.Values;

            // Workspace
            var w = new int[m];
            var x = new double[m];

            var result = new CompressedColumnStorage(m, n, anz + bnz);

            cp = result.ColumnPointers;
            for (j = 0; j < n; j++)
            {
                if (nz + m > result.Values.Length && !result.Resize(2 * (result.Values.Length) + m))
                {
                    throw new Exception(); // out of memory
                }
                ci = result.RowIndices;
                cx = result.Values; // C.i and C.x may be reallocated
                cp[j] = nz; // column j of C starts here
                for (p = bp[j]; p < bp[j + 1]; p++)
                {
                    nz = this.Scatter(bi[p], bx[p], w, x, j + 1, result, nz);
                }

                for (p = cp[j]; p < nz; p++)
                {
                    cx[p] = x[ci[p]];
                }
            }
            cp[n] = nz; // finalize the last column of C
            result.Resize(0); // remove extra space from C
            result.SortIndices();

            return result; // success
        }

        public override bool Equals(ISparseMatrixStorage<double> other, double tolerance)
        {
            var o = other as CompressedColumnStorage;

            if (o == null)
            {
                return false;
            }

            int nz = this.NonZerosCount;

            if (this.ncols != o.ColumnCount || this.nrows != o.RowCount || nz != o.NonZerosCount)
            {
                return false;
            }

            for (int i = 0; i < this.ncols; i++)
            {
                if (this.ColumnPointers[i] != o.ColumnPointers[i])
                {
                    return false;
                }
            }

            for (int i = 0; i < nz; i++)
            {
                if (this.RowIndices[i] != o.RowIndices[i])
                {
                    return false;
                }

                // TODO: should compare relative values!
                if (Math.Abs(this.Values[i] - o.Values[i]) > tolerance)
                {
                    return false;
                }
            }

            return true;
        }

        #region Internal methods

        internal override void Cleanup()
        {
            int i, j, p, q, nnz = 0;
            int[] marker = new int[nrows];

            for (j = 0; j < nrows; j++)
            {
                marker[j] = -1; // Row j not yet seen.
            }

            for (i = 0; i < ncols; i++)
            {
                q = nnz; // Column i will start at q
                for (p = ColumnPointers[i]; p < ColumnPointers[i + 1]; p++)
                {
                    j = RowIndices[p]; // A(i,j) is nonzero
                    if (marker[j] >= q)
                    {
                        Values[marker[j]] += Values[p]; // A(i,j) is a duplicate
                    }
                    else
                    {
                        marker[j] = nnz; // Record where column j occurs
                        RowIndices[nnz] = j; // Keep A(i,j)
                        Values[nnz] = Values[p];

                        nnz += 1;
                    }
                }
                ColumnPointers[i] = q; // Record start of row i
            }

            this.ColumnPointers[ncols] = nnz;

            // Remove extra space from arrays
            this.Resize(0);
        }

        /// <summary>
        /// Scatters and sums a sparse vector A(:,j) into a dense vector, x = x + beta * A(:,j).
        /// </summary>
        /// <param name="j">the column of A to use</param>
        /// <param name="beta">scalar multiplied by A(:,j)</param>
        /// <param name="w">size m, node i is marked if w[i] = mark</param>
        /// <param name="x">size m, ignored if null</param>
        /// <param name="mark">mark value of w</param>
        /// <param name="mat">pattern of x accumulated in C.i</param>
        /// <param name="nz">pattern of x placed in C starting at C.i[nz]</param>
        /// <returns>new value of nz, -1 on error</returns>
        internal override int Scatter(int j, double beta, int[] w, double[] x, int mark,
            CompressedColumnStorage<double> mat, int nz)
        {
            int i, p;

            if (w == null || mat == null) return -1; // check inputs

            var cj = mat.RowIndices;

            for (p = ColumnPointers[j]; p < ColumnPointers[j + 1]; p++)
            {
                i = RowIndices[p]; // A(i,j) is nonzero

                if (w[i] < mark)
                {
                    w[i] = mark; // i is new entry in column j
                    if (x != null) x[i] = beta * Values[p]; // x(i) = beta*A(i,j)
                    cj[nz++] = i; // add i to pattern of C(:,j)
                }
                else if (x != null)
                {
                    x[i] += beta * Values[p]; // i exists in C(:,j) already
                }
            }

            return nz;
        }

        #endregion

        public static CSparse.Double.CompressedColumnStorage operator *(
            CSparse.Double.CompressedColumnStorage m1, CSparse.Double.CompressedColumnStorage m2)
        {
            return (CSparse.Double.CompressedColumnStorage)m1.Multiply(m2);
        }

        public static CSparse.Double.CompressedColumnStorage operator +(
            CSparse.Double.CompressedColumnStorage m1, CSparse.Double.CompressedColumnStorage m2)
        {
            return (CSparse.Double.CompressedColumnStorage)m1.Add(m2);
        }

        public static CSparse.Double.CompressedColumnStorage operator -(
            CSparse.Double.CompressedColumnStorage m1)
        {
            var newM1 = (CSparse.Double.CompressedColumnStorage) m1.Clone();

            for (int i = 0; i < newM1.Values.Length; i++)
            {
                newM1.Values[i] = -newM1.Values[i];
            }

            return newM1;
        }
    }
}
