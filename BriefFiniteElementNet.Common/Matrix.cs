
namespace BriefFiniteElementNet
{
    using CSparse.Double;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Globalization;
    using CSparse.Double.Factorization;
    using CSparse.Storage;

    /// <summary>
    /// Represents a dense real matrix
    /// </summary>
    [DebuggerDisplay("Matrix {RowCount} x {ColumnCount}")]
    [Serializable]
    public class Matrix : DenseMatrix
    {
        #region Matrix pool

        public MatrixPool Pool;

        /// <summary>
        /// Returns the matrix into pool and invalidates the matrix
        /// </summary>
        /// <returns></returns>
        public void ReturnToPool()
        {
            //return;
            if (Disposed)
                return;

            this.Disposed = true;

            lock (this)
            {
                if (Pool != null)
                    Pool.Free(this);
            }

        }

        [NonSerialized]
        private bool Disposed = false;
        //[NonSerialized]
        //private string GenerateCallStack_Temp;

        /// <summary>
        /// Gets or sets a value indicating whether pool is used for this object or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use pool]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// If pool used for this object, on distruction corearray will return to pool
        /// </remarks>
        public bool UsePool { get; set; }

        #endregion

        #region Static Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class as a column vector.
        /// </summary>
        /// <param name="vals">The vals.</param>
        public static Matrix OfVector(double[] vals)
        {
            return new Matrix(vals.Length, 1, (double[])vals.Clone());
        }

        public static Matrix Random(int rows, int columns, int seed = 3165077)
        {
            var buf = new Matrix(rows, columns);

            var rnd = new Random(seed);

            for (int i = 0; i < rows * columns; i++)
            {
                buf.Values[i] = rnd.NextDouble() * 100;
            }


            return buf;
        }

        /// <summary>
        /// Creates a new Identity matrix.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns></returns>
        public static Matrix Eye(int n)
        {
            var buf = new Matrix(n, n);

            for (int i = 0; i < n; i++)
                buf.Values[i * n + i] = 1.0;

            return buf;
        }

        #endregion

        #region Operators

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            return m1.Multiply(m2).AsMatrix();
        }

        public static Matrix operator *(DenseColumnMajorStorage<double> m1, Matrix m2)
        {
            return m1.Multiply(m2).AsMatrix();
        }

        public static Matrix operator *(Matrix m1, DenseColumnMajorStorage<double> m2)
        {
            return m1.Multiply(m2).AsMatrix();
        }

        public static double[] operator *(Matrix m1, double[] vec)
        {
            var res = m1.Multiply(vec);

            return res;
        }

        public static Matrix operator *(double coeff, Matrix mat)
        {
            var newMat = new double[mat.RowCount * mat.ColumnCount];

            for (int i = 0; i < newMat.Length; i++)
            {
                newMat[i] = coeff * mat.Values[i];
            }

            var buf = new Matrix(mat.RowCount, mat.ColumnCount);
            buf.Values = newMat;

            return buf;
        }

        public static Matrix operator -(Matrix mat)
        {
            var buf = new Matrix(mat.RowCount, mat.ColumnCount);

            for (int i = 0; i < buf.Values.Length; i++)
            {
                buf.Values[i] = -mat.Values[i];
            }

            return buf;
        }

        public static Matrix operator +(Matrix mat1, Matrix mat2)
        {
            MatrixException.ThrowIf(mat1.RowCount != mat2.RowCount || mat1.ColumnCount != mat2.ColumnCount,
                "Inconsistent matrix sizes");

            var buf = new Matrix(mat1.RowCount, mat1.ColumnCount);

            for (int i = 0; i < buf.Values.Length; i++)
            {
                buf.Values[i] = mat1.Values[i] + mat2.Values[i];
            }

            return buf;
        }

        public static Matrix operator -(Matrix mat1, Matrix mat2)
        {
            MatrixException.ThrowIf(mat1.RowCount != mat2.RowCount || mat1.ColumnCount != mat2.ColumnCount,
                "Inconsistent matrix sizes");

            var buf = new Matrix(mat1.RowCount, mat1.ColumnCount);

            for (int i = 0; i < buf.Values.Length; i++)
            {
                buf.Values[i] = mat1.Values[i] - mat2.Values[i];
            }

            return buf;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="columns">The columns.</param>
        public Matrix(int rows, int columns)
            : base(rows, columns, new double[rows * columns])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// </summary>
        /// <param name="size">The size of the square matrix.</param>
        public Matrix(int size)
            : base(size, size, new double[size * size])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="values">The values array (not cloned, ownership is taken).</param>
        public Matrix(int rows, int columns, double[] values)
            : base(rows, columns, values)
        {
        }

        #endregion

        public override string ToString()
        {
            var sb = new StringBuilder();

            var mtx = this;

            var epsi1on = mtx.Values.Select(i => System.Math.Abs(i)).Min() * 1e-9;

            if (epsi1on == 0)
                epsi1on = 1e-9;

            for (var i = 0; i < mtx.RowCount; i++)
            {
                for (var j = 0; j < mtx.ColumnCount; j++)
                {
                    if (System.Math.Abs(mtx[i, j]) < epsi1on)
                        sb.AppendFormat(CultureInfo.CurrentCulture, "0\t", mtx[i, j]);
                    else
                        sb.AppendFormat("{0:0.00}\t", mtx[i, j]);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }

    public static class DenseMatrixExtensions
    {
        /// <summary>
        /// Helper method to cast the storage to a Matrix instance.
        /// </summary>
        public static Matrix AsMatrix(this DenseColumnMajorStorage<double> matrix)
        {
            // NOTE: this method is mainly used to simpify the usage operator overloads
            //       which aren't supported by CSparse matrix types.

            return new Matrix(matrix.RowCount, matrix.ColumnCount, matrix.Values);
        }

        public static Matrix RepeatDiagonally(this DenseColumnMajorStorage<double> matrix, int n)
        {
            int rows = matrix.RowCount;
            int columns = matrix.ColumnCount;

            var buf = new Matrix(rows * n, columns * n);

            RepeatDiagonally(matrix, n, new Matrix(rows * n, columns * n));

            return buf;
        }

        public static void RepeatDiagonally(this DenseColumnMajorStorage<double> matrix, int n, Matrix target)
        {
            int r = matrix.RowCount;
            int c = matrix.ColumnCount;

            if (target.RowCount != n * r || target.ColumnCount != n * c)
            {
                throw new ArgumentException("Dimensions don't match.");
            }

            var buf = new Matrix(r * n, c * n);

            for (var i = 0; i < n; i++)
            {
                for (var ii = 0; ii < r; ii++)
                {
                    for (var jj = 0; jj < c; jj++)
                    {
                        // TODO: MAT - direct access
                        buf[i * r + ii, i * c + jj] = matrix[ii, jj];
                    }
                }
            }
        }

        public static Matrix PointwiseDivide(this DenseColumnMajorStorage<double> matrix, Matrix other)
        {
            int rows = matrix.RowCount;
            int columns = matrix.ColumnCount;

            if (rows != other.RowCount || columns != other.ColumnCount)
                throw new Exception();

            var buf = new Matrix(rows, columns);
            var values = matrix.Values;

            for (int i = 0; i < values.Length; i++)
            {
                buf.Values[i] = values[i] / other.Values[i];
            }

            return buf;
        }

        public static Matrix PointwiseMultiply(this DenseColumnMajorStorage<double> matrix, Matrix other)
        {
            int rows = matrix.RowCount;
            int columns = matrix.ColumnCount;

            if (rows != other.RowCount || columns != other.ColumnCount)
                throw new Exception();

            var buf = new Matrix(rows, columns);
            var values = matrix.Values;

            for (int i = 0; i < values.Length; i++)
            {
                buf.Values[i] = values[i] * other.Values[i];
            }

            return buf;
        }

        public static void Add(this DenseColumnMajorStorage<double> matrix, Matrix other, Matrix result)
        {
            int rows = matrix.RowCount;
            int columns = matrix.ColumnCount;

            MatrixException.ThrowIf(
                rows != other.RowCount || other.RowCount != result.RowCount ||
                columns != other.ColumnCount ||  other.ColumnCount != result.ColumnCount,
                "Inconsistent matrix sizes");

            int n = rows * columns;
            var values = matrix.Values;

            for (int i = 0; i < n; i++)
            {
                result.Values[i] = values[i] + other.Values[i];
            }
        }


        /// <summary>
        /// mat1 = mat1 + mat2
        /// </summary>
        /// <param name="other"></param>
        public static void AddToThis(this DenseColumnMajorStorage<double> matrix, Matrix other)
        {
            int rows = matrix.RowCount;
            int columns = matrix.ColumnCount;

            MatrixException.ThrowIf(rows != other.RowCount || columns != other.ColumnCount, "Inconsistent matrix sizes");

            int n = rows * columns;
            var values = matrix.Values;

            for (int i = 0; i < n; i++)
            {
                values[i] = values[i] + other.Values[i];
            }
        }

        /// <summary>
        /// mat1 = mat1 + mat2 * coefficient
        /// </summary>
        /// <param name="mat1"></param>
        /// <param name="other"></param>
        /// <param name="coefficient"></param>
        public static void AddToThis(this DenseColumnMajorStorage<double> matrix, Matrix other, double coefficient)
        {
            int rows = matrix.RowCount;
            int columns = matrix.ColumnCount;

            MatrixException.ThrowIf(rows != other.RowCount || columns != other.ColumnCount, "Inconsistent matrix sizes");

            int n = rows * columns;
            var values = matrix.Values;

            for (int i = 0; i < n; i++)
                values[i] = values[i] + other.Values[i] * coefficient;
        }

        /// <summary>
        /// Multiplies the specified <see cref="Matrix"/> with specified Vector <see cref="vec"/>.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="vec">The vec.</param>
        /// <returns></returns>
        /// <exception cref="BriefFiniteElementNet.MatrixException"></exception>
        public static double[] Multiply(this DenseColumnMajorStorage<double> matrix, double[] vec)
        {
            var buf = new double[vec.Length];

            matrix.Multiply(vec, buf);

            return buf;
        }

        /*
        public static Matrix Multiply(this DenseColumnMajorStorage<double> matrix, Matrix other)
        {
            int rows = matrix.RowCount;
            int columns = matrix.ColumnCount;

            if (columns != other.RowCount)
                throw new InvalidOperationException("No consistent dimensions");

            int columnsB = other.ColumnCount;

            var res = new Matrix(rows, columnsB);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columnsB; j++)
                    for (int k = 0; k < columns; k++)
                    {
                        res.Values[j * res.RowCount + i] +=
                            matrix.Values[k * rows + i] *
                            other.Values[j * other.RowCount + k];
                    }


            return res;
        }
        //*/

        /// <summary>
        /// calculates the [this.transpose] * [other] and stores the value into result.
        /// </summary>
        /// <param name="other">The m2.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static void TransposeMultiply(this DenseColumnMajorStorage<double> matrix, Matrix other, Matrix result)
        {
            int rowsA = matrix.RowCount;
            int columnsA = matrix.ColumnCount;

            int rowsB = other.RowCount;
            int columnsB = other.ColumnCount;

            if (rowsA != rowsB)
                throw new InvalidOperationException("No consistent dimensions");

            var res = result;

            if (res.RowCount != columnsA || res.ColumnCount != columnsB)
            {
                throw new Exception("result dimension mismatch");
            }

            var a_arr = matrix.Values;
            var b_arr = other.Values;

            int vecLength = rowsA;

            for (int i = 0; i < rowsA; i++)
                for (int j = 0; j < rowsB; j++)
                {
                    double t = 0.0;

                    // column major order.
                    int a_st = i * columnsA;
                    int b_st = j * columnsB;

                    for (int k = 0; k < vecLength; k++)
                        t += a_arr[a_st + k] * b_arr[b_st + k];

                    res[i, j] = t;
                }
        }

        public static void InPlaceTranspose(this DenseColumnMajorStorage<double> matrix)
        {
            int rows = matrix.RowCount;
            int columns = matrix.ColumnCount;

            if (rows != columns)
            {
                throw new InvalidOperationException("matrix must be square");
            }

            var values = matrix.Values;

            for (int i = 0; i < rows; i++)
            {
                for (int j = i; j < columns; j++)
                {
                    double tmp = values[i * columns + j];

                    values[i * columns + j] = values[j * columns + i];

                    values[j * columns + i] = tmp;
                }
            }
        }

        /// <summary>
        /// Checks if number of rows equals number of columns.
        /// </summary>
        /// <returns>True iff matrix is n by n.</returns>
        public static bool IsSquare(this DenseColumnMajorStorage<double> matrix)
        {
            return (matrix.RowCount == matrix.ColumnCount);
        }

        /// <summary>
        /// Retrieves row vector at specfifed index and deletes it from matrix.
        /// </summary>
        /// <param name="i">One-based index at which to extract.</param>
        /// <returns>Row vector.</returns>
        public static Matrix ExtractRow(this DenseColumnMajorStorage<double> matrix, int i)
        {
            return new Matrix(1, matrix.ColumnCount, matrix.Row(i));
        }

        /// <summary>
        /// Gets the determinant of matrix
        /// </summary>
        /// <returns></returns>
        public static double Determinant(this DenseColumnMajorStorage<double> matrix)
        {
            return DenseLU.Create(matrix).Determinant();
        }

        /// <summary>
        /// Gets the inverse of matrix
        /// </summary>
        /// <returns></returns>
        public static Matrix Inverse(this DenseColumnMajorStorage<double> matrix)
        {
            int rows = matrix.RowCount;
            int columns = matrix.ColumnCount;

            var inv = new Matrix(rows, columns);

            DenseLU.Create(matrix).Inverse(inv);

            return inv;
        }

        /// <summary>
        /// return the value that this * value = rightSide
        /// </summary>
        /// <param name="rightSide"></param>
        /// <returns></returns>
        public static double[] Solve(this DenseColumnMajorStorage<double> matrix, double[] rightSide)
        {
            int columns = matrix.ColumnCount;

            var lu = DenseLU.Create(matrix);
            var x = new double[columns];

            lu.Solve(rightSide, x);

            return x;
        }

        public static void Replace(this DenseColumnMajorStorage<double> matrix, double oldValue, double newValue)
        {
            var values = matrix.Values;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Equals(oldValue))
                    values[i] = newValue;
            }
        }

        /// <summary>
        /// Multiplies the <see cref="matrix"/> by a constant value.
        /// </summary>
        /// <param name="matrix">The Matrix</param>
        /// <param name="constant">The constant value</param>
        public static void Scale(this DenseColumnMajorStorage<double> matrix, double constant)
        {
            var values = matrix.Values;

            for (var i = 0; i < values.Length; i++)
            {
                values[i] *= constant;
            }
        }

        public static void ScaleRow(this DenseColumnMajorStorage<double> matrix, int i, double constant)
        {
            int columns = matrix.ColumnCount;
            var values = matrix.Values;

            int cxi = i * columns;

            for (int j = 0; j < columns; j++)
            {
                values[cxi + j] *= constant;
            }
        }
    }
}