
using System.Runtime.Serialization;

namespace BriefFiniteElementNet
{
    using CSparse.Double;
    using CSparse.Double.Factorization;
    using CSparse.Storage;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a dense real matrix
    /// </summary>
    [DebuggerDisplay("Matrix {RowCount} x {ColumnCount}")]
    [Serializable]
    public class Matrix : DenseMatrix,ISerializable
    {
        #region Matrix pool

        [NonSerialized]
        public MatrixPool Pool;

        /// <summary>
        /// Gets or sets a value indicating whether pool is used for this object or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use pool]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// If pool used for this object, on distruction values array will return to pool.
        /// </remarks>
        public bool UsePool { get; set; }

        [NonSerialized]
        private bool Disposed = false;

        /// <summary>
        /// Returns the matrix into pool and invalidates the matrix
        /// </summary>
        /// <returns></returns>
        public void ReturnToPool()
        {
            if (Disposed)
                return;

            this.Disposed = true;

            lock (this)
            {
                if (Pool != null)
                    Pool.Free(this);
            }
        }

        public double[] CoreArray
        {
            get
            {
                return base.Values;
            }
        }

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
        /// Creates a new identity matrix.
        /// </summary>
        /// <param name="n">The matrix size.</param>
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
            return m1.Multiply(vec);
        }

        public static Matrix operator *(double coeff, Matrix mat)
        {
            var values = new double[mat.RowCount * mat.ColumnCount];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = coeff * mat.Values[i];
            }

            return new Matrix(mat.RowCount, mat.ColumnCount, values);
        }

        public static Matrix operator -(Matrix mat)
        {
            var result = new Matrix(mat.RowCount, mat.ColumnCount);

            for (int i = 0; i < result.Values.Length; i++)
            {
                result.Values[i] = -mat.Values[i];
            }

            return result;
        }

        public static Matrix operator +(Matrix mat1, Matrix mat2)
        {
            MatrixException.ThrowIf(mat1.RowCount != mat2.RowCount || mat1.ColumnCount != mat2.ColumnCount,
                "Inconsistent matrix sizes");

            var result = new Matrix(mat1.RowCount, mat1.ColumnCount);

            for (int i = 0; i < result.Values.Length; i++)
            {
                result.Values[i] = mat1.Values[i] + mat2.Values[i];
            }

            return result;
        }

        public static Matrix operator -(Matrix mat1, Matrix mat2)
        {
            MatrixException.ThrowIf(mat1.RowCount != mat2.RowCount || mat1.ColumnCount != mat2.ColumnCount,
                "Inconsistent matrix sizes");

            var result = new Matrix(mat1.RowCount, mat1.ColumnCount);

            for (int i = 0; i < result.Values.Length; i++)
            {
                result.Values[i] = mat1.Values[i] - mat2.Values[i];
            }

            return result;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class. Vector
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="values">The values array (not cloned, ownership is taken).</param>
        public Matrix(double[] values)
            : base(values.Length, 1, values)
        {
        }

        #endregion

        public Matrix Clone()
        {
            return new Matrix(this.rows, this.columns, (double[]) this.Values.Clone());
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var mtx = this;

            var epsi1on = mtx.Values.Select(i => Math.Abs(i)).Min() * 1e-9;

            if (epsi1on == 0)
                epsi1on = 1e-9;

            for (var i = 0; i < mtx.RowCount; i++)
            {
                for (var j = 0; j < mtx.ColumnCount; j++)
                {
                    if (Math.Abs(mtx[i, j]) < epsi1on)
                        sb.AppendFormat(CultureInfo.CurrentCulture, "0\t", mtx[i, j]);
                    else
                        sb.AppendFormat("{0:0.00}\t", mtx[i, j]);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("rows", base.rows);
            info.AddValue("columns", base.columns);
            info.AddValue("Values", base.Values);
        }

        public Matrix(SerializationInfo info, StreamingContext context):
            base(info.GetInt32("rows"), info.GetInt32("columns"), (double[])info.GetValue("Values",typeof(double[])))
        {
        }



        /// <summary>
        /// Substitutes the defined row with defined values.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="values">The values.</param>
        public static void InplacePlus(Matrix matrix, Matrix toAdd)
        {

            if (matrix.RowCount != matrix.ColumnCount)
                throw new ArgumentOutOfRangeException("invalid");

            for (var i = 0; i < matrix.RowCount; i++)
            for (var j = 0; j < matrix.ColumnCount; j++)
            {
                matrix[i, j] = matrix[i, j] + toAdd[i, j];
            }
        }

        public static Matrix DotDivide(Matrix mat1, Matrix mat2)
        {
            MatrixException.ThrowIf(mat1.RowCount != mat2.RowCount || mat1.ColumnCount != mat2.ColumnCount,
                "Inconsistent matrix sizes");

            var result = new Matrix(mat1.RowCount, mat1.ColumnCount);

            for (int i = 0; i < result.Values.Length; i++)
            {
                result.Values[i] = mat1.Values[i] / mat2.Values[i];
            }

            return result;
        }
    }

    public static class DenseMatrixExtensions
    {
        

        /// <summary>
        /// Retrieves row vector at specfifed index and deletes it from MatrixOld.
        /// </summary>
        /// <param name="i">One-based index at which to extract.</param>
        /// <returns>Row vector.</returns>
        public static Matrix ExtractRow(this Matrix matx,int i)
        {
            if (i >= matx.RowCount || i < 0)
                throw new ArgumentOutOfRangeException("i");

            var mtx = new Matrix(1, matx.ColumnCount);


            for (int j = 0; j < mtx.ColumnCount; j++)
            {
                mtx.CoreArray[j] = matx.CoreArray[j * mtx.RowCount + i];
            }

            return mtx;
        }

        public static Matrix RemoveRows(this Matrix mtx, params int[] rows)
        {
            var m2 = new Matrix(mtx.RowCount - rows.Length, mtx.ColumnCount);

            var map = new int[mtx.RowCount - rows.Length];

            var cnt = 0;

            for (var i = 0; i < mtx.RowCount; i++)
                if (!rows.Contains(i))
                    map[cnt++] = i;

            for (var i = 0; i < mtx.RowCount; i++)
                for (var j = 0; j < mtx.ColumnCount; j++)
                {
                    m2[map[i], j] = mtx[i, j];
                }

            return m2;
        }

        /// <summary>
        /// Retrieves column vector at specfifed index and deletes it from MatrixOld.
        /// </summary>
        /// <param name="j">One-based index at which to extract.</param>
        /// <returns>Row vector.</returns>
        public static Matrix ExtractColumn(this Matrix matx, int j)
        {
            if (j >= matx.ColumnCount || j < 0)
                throw new ArgumentOutOfRangeException("j");

            var mtx = new Matrix(matx.RowCount, 1);


            for (int i = 0; i < matx.RowCount; i++)
            {
                mtx.CoreArray[i] = matx.CoreArray[j * matx.RowCount + i];
            }

            return mtx;
        }

        /// <summary>
        /// Retrieves column vector at specfifed index and deletes it from MatrixOld.
        /// </summary>
        /// <param name="j">One-based index at which to extract.</param>
        /// <returns>Row vector.</returns>
        public static void MultiplyByConstant(this DenseColumnMajorStorage<double> matx, double coef)
        {

            for (int i = 0; i < matx.RowCount; i++)
            {
                matx.Values[i] *= coef;
            }
        }



       

        /// <summary>
        /// Substitutes the defined row with defined values.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="values">The values.</param>
        public static void SetRow(this Matrix matrix,int i, params double[] values)
        {

            if (values.Count() != matrix.ColumnCount)
                throw new ArgumentOutOfRangeException("values");

            for (int j = 0; j < matrix.ColumnCount; j++)
            {
                matrix.CoreArray[j * matrix.RowCount + i] = values[j];
            }
        }

        /// <summary>
        /// Substitutes the defined column with defined values.
        /// </summary>
        /// <param name="j">The j.</param>
        /// <param name="values">The values.</param>
        public static void SetColumn(this Matrix matrix, int j, params double[] values)
        {
            if (values.Count() != matrix.RowCount)
                throw new ArgumentOutOfRangeException("values");


            for (int i = 0; i < matrix.RowCount; i++)
            {
                matrix.CoreArray[j * matrix.RowCount + i] = values[i];
            }
        }

        /// <summary>
        /// Helper method to cast the storage to a <see cref="Matrix"/> instance.
        /// </summary>
        public static Matrix AsMatrix(this DenseColumnMajorStorage<double> matrix)
        {
            return new Matrix(matrix.RowCount, matrix.ColumnCount, matrix.Values);
        }

        public static Matrix RepeatDiagonally(this DenseColumnMajorStorage<double> matrix, int n)
        {
            int rows = matrix.RowCount;
            int columns = matrix.ColumnCount;

            var buf = new Matrix(rows * n, columns * n);

            RepeatDiagonally(matrix, n, buf);

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

            var A = matrix.Values;
            var B = target.Values;

            int nxr = n * r;

            for (var i = 0; i < n; i++)
            {
                int rxi = r * i;
                int cxi = c * i;

                for (var ii = 0; ii < r; ii++)
                {
                    for (var jj = 0; jj < c; jj++)
                    {
                        //target[i * r + ii, i * c + jj] = matrix[ii, jj];
                        B[(cxi + jj) * nxr + (rxi + ii)] = A[jj * r + ii];
                    }
                }
            }
        }

        /// <summary>
        /// Pointwise division of two matrices (Matlab dot syntax A./B).
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="other"></param>
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

        /// <summary>
        /// Pointwise multiplcation of two matrices (Matlab dot syntax A.*B).
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="other"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Computes the sum A = A + B.
        /// </summary>
        /// <param name="matrix"></param>
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
        /// Computes the sum A = A + s * B.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="other"></param>
        /// <param name="s"></param>
        public static void AddToThis(this DenseColumnMajorStorage<double> matrix, Matrix other, double s)
        {
            int rows = matrix.RowCount;
            int columns = matrix.ColumnCount;

            MatrixException.ThrowIf(rows != other.RowCount || columns != other.ColumnCount, "Inconsistent matrix sizes");

            int n = rows * columns;
            var values = matrix.Values;

            for (int i = 0; i < n; i++)
                values[i] = values[i] + other.Values[i] * s;
        }

        /// <summary>
        /// Multiplies the specified <see cref="Matrix"/> with specified Vector <paramref name="vec"/>.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="vec">The vec.</param>
        /// <returns></returns>
        public static double[] Multiply(this DenseColumnMajorStorage<double> matrix, double[] vec)
        {
            var buf = new double[vec.Length];

            matrix.Multiply(vec, buf);

            return buf;
        }

        /// <summary>
        /// Computes the matrix product A^T * B.
        /// </summary>
        /// <param name="matrix">The matrix A.</param>
        /// <param name="other">The other matrix B.</param>
        /// <param name="result">The result matrix.</param>
        public static void TransposeMultiply(this DenseColumnMajorStorage<double> matrix, Matrix other, Matrix result)
        {
            int rowsA = matrix.RowCount;
            int columnsA = matrix.ColumnCount;

            int rowsB = other.RowCount;
            int columnsB = other.ColumnCount;

            if (rowsA != rowsB)
            {
                throw new InvalidOperationException("No consistent dimensions");
            }

            if (result.RowCount != columnsA || result.ColumnCount != columnsB)
            {
                throw new Exception("result dimension mismatch");
            }

            var A = matrix.Values;
            var B = other.Values;
            var C = result.Values;

            for (int i = 0; i < columnsA; i++)
            {
                for (int j = 0; j < columnsB; j++)
                {
                    double sum = 0.0;

                    // Column major order.
                    int strideA = i * rowsA;
                    int strideB = j * rowsB;

                    for (int k = 0; k < rowsA; k++)
                    {
                        //sum += A[k, i] * B[k, j];
                        sum += A[strideA + k] * B[strideB + k];
                    }

                    //C[i, j] = sum;
                    C[j * columnsA + i] = sum;
                }
            }
        }


        public static void InPlaceTranspose(this DenseColumnMajorStorage<double> matrix)
        {
            TransposeInPlace(matrix);
        }
        /// <summary>
        /// Transposes the matrix in place (matrix must be square).
        /// </summary>
        /// <param name="matrix"></param>
        public static void TransposeInPlace(this DenseColumnMajorStorage<double> matrix)
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
                int nxi = i * columns;

                for (int j = i; j < columns; j++)
                {
                    double tmp = values[nxi + j];

                    values[nxi + j] = values[j * columns + i];

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
        /// Gets the determinant of the matrix.
        /// </summary>
        /// <returns></returns>
        public static double Determinant(this DenseColumnMajorStorage<double> matrix)
        {
            return DenseLU.Create(matrix).Determinant();
        }

        /// <summary>
        /// Gets the inverse of the matrix.
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
        /// Solves the linear system Ax=b.
        /// </summary>
        /// <param name="input">The right hand side vector.</param>
        /// <returns></returns>
        public static double[] Solve(this DenseColumnMajorStorage<double> matrix, double[] input)
        {
            int columns = matrix.ColumnCount;

            var lu = DenseLU.Create(matrix);
            var x = new double[columns];

            lu.Solve(input, x);

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
        /// Multiplies the matrix by a constant value.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="constant">The constant value.</param>
        public static void Scale(this DenseColumnMajorStorage<double> matrix, double constant)
        {
            var values = matrix.Values;

            for (var i = 0; i < values.Length; i++)
            {
                values[i] *= constant;
            }
        }

        /// <summary>
        /// Multiplies a matrix row by a constant value.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="i">The row index.</param>
        /// <param name="constant">The constant value.</param>
        public static void ScaleRow(this DenseColumnMajorStorage<double> matrix, int i, double constant)
        {
            int rows = matrix.RowCount;
            int columns = matrix.ColumnCount;
            var values = matrix.Values;

            for (int j = 0; j < columns; j++)
            {
                values[j * rows + i] *= constant;
            }
        }

        public static void FillMatrixRowise(this DenseColumnMajorStorage<double> matrix, params double[] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                var row = i / matrix.ColumnCount;
                var col = i % matrix.ColumnCount;

                matrix[row, col] = values[i];
            }
        }
    }
}