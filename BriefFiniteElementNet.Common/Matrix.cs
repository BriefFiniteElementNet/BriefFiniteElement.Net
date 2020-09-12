using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Globalization;
using BriefFiniteElementNet.Common;


namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a dense real matrix
    /// </summary>
    [DebuggerDisplay("Matrix {RowCount} x {ColumnCount}")]
    [Serializable]
    public class Matrix : ISerializable, IEnumerable<double>
    {
        public MatrixPool Pool;

        public static Matrix RepeatDiagonally(Matrix mtx, int n)
        {
            var r = mtx.rows;
            var c = mtx.columns;

            var buf = new Matrix(r*n, c*n);

            for (var i = 0; i < n; i++)
                //for (var j = 0; j < n; j++)
                for (var ii = 0; ii < mtx.rows; ii++)
                    for (var jj = 0; jj < mtx.columns; jj++)
                        buf[i*r + ii, i*c + jj] = mtx[ii, jj];

            return buf;
        }

        public static Matrix RandomMatrix(int m, int n)
        {
            var buf = new Matrix(m, n);

            var rnd = new Random(0);

            for (int i = 0; i < m*n; i++)
            {
                buf.Values[i] = rnd.NextDouble()*100;
            }


            return buf;
        }

        public double[] Values
        {
            get { return values; }
            internal set { values = value; }
        }

        private double[] values;

        /// <summary>
        /// Number of rows of the matrix.
        /// </summary>
        public int RowCount
        {
            get { return rows; }
        }

        /// <summary>
        /// Number of columns of the matrix.
        /// </summary>
        public int ColumnCount
        {
            get { return columns; }
        }


        /// <summary>
        /// Number of rows of the matrix.
        /// </summary>
        private int rows;

        /// <summary>
        /// Number of columns of the matrix.
        /// </summary>
        private int columns;

        public static long CreateCount,DistructCount;

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="Matrix"/> class from being created.
        /// </summary>
        private Matrix()
        {
            CreateCount++;
           // GenerateCallStack_Temp = Environment.StackTrace;
        }


        ~Matrix()
        {
            /*
            if (coreArray != null)
            {
                if (coreArray.Length == 2)
                    Guid.NewGuid();

                int i;

                lock (dists)
                {
                    if (dists.TryGetValue(coreArray.Length, out i))
                        dists[coreArray.Length]++;
                    else
                        dists.Add(coreArray.Length, 1);
                }

            }
            */

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// </summary>
        /// <param name="m">The row.</param>
        /// <param name="n">The columns.</param>
        /// <exception cref="System.ArgumentException">
        /// row
        /// or
        /// n
        /// </exception>
        public Matrix(int m, int n):this()
        {
            rows = m;
            columns = n;

            if (rows <= 0)
                throw new ArgumentException("row");

            if (columns <= 0)
                throw new ArgumentException("n");

            this.Values = new double[m*n];
        }

        /// <summary>
        /// Initializes a new square matrix
        /// </summary>
        /// <param name="n">The matrix dimension.</param>
        public Matrix(int n) : this()
        {
            rows = n;
            columns = n;

            if (n <= 0)
                throw new ArgumentException("n");

            this.Values = new double[n*n];
        }

        public static Matrix OfJaggedArray(double[][] vals)
        {
            var rows = vals.Length;
            var cols = vals.Select(i => i.Length).Max();

            var buf = new Matrix(rows, cols);
            buf.rows = rows;
            buf.columns = cols;

            buf.values = new double[rows * cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (vals[i].Length > j)
                        buf.Values[j * rows + i] = vals[i][j];

            return buf;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class as a column vector.
        /// </summary>
        /// <param name="vals">The vals.</param>
        public static Matrix OfVector(double[] vals)
        {
            return new Matrix(vals.Length, 1, (double[])vals.Clone());
        }

        /// <summary>
        /// creates
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="coreArray"></param>
        public Matrix(int rows, int cols, double[] coreArray) :this()
        {
            this.rows = rows;
            columns = cols;

            if (this.rows <= 0)
                throw new ArgumentException("rows");

            if (columns <= 0)
                throw new ArgumentException("cols");

            this.Values = coreArray;

            //CreateCount--;
        }

        #endregion

        /// <summary>
        /// Gets or sets the specified member.
        /// </summary>
        /// <value>
        /// The <see cref="System.Double"/>.
        /// </value>
        /// <param name="row">The row (zero based).</param>
        /// <param name="column">The column (zero based).</param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.IndexerName("TheMember")]
        public double this[int row, int column]
        {
            get
            {
                return At(row, column);
            }

            set
            {
                At(row, column, value);
            }
        }

        public static Matrix Multiply(Matrix m1, Matrix m2)
        {
            if (m1.ColumnCount != m2.RowCount)
                throw new InvalidOperationException("No consistent dimensions");

            var res = new Matrix(m1.RowCount, m2.ColumnCount);

            for (int i = 0; i < m1.rows; i++)
                for (int j = 0; j < m2.columns; j++)
                    for (int k = 0; k < m1.columns; k++)
                    {
                        res.Values[j*res.rows + i] +=
                            m1.Values[k*m1.rows + i]*
                            m2.Values[j*m2.rows + k];
                    }


            return res;
        }

        /// <summary>
        /// calculates the m1.transpose * m2 and stores the value into result.
        /// </summary>
        /// <param name="m1">The m1.</param>
        /// <param name="m2">The m2.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static void TransposeMultiply(Matrix m1, Matrix m2, Matrix result)
        {
            if (m1.rows != m2.RowCount)
                throw new InvalidOperationException("No consistent dimensions");

            var res = result;

            if (res.rows != m1.columns || res.ColumnCount != m2.columns)
            {
                throw new Exception("result dimension mismatch");
            }

            var a = m1;
            var b = m2;


            var a_arr = a.values;
            var b_arr = b.values;

            var vecLength = a.rows;

            for (var i = 0; i < a.columns; i++)
                for (var j = 0; j < b.columns; j++)
                {
                    var t = 0.0;

                    var a_st = i * a.rows;
                    var b_st = j * b.rows;

                    for (var k = 0; k < vecLength; k++)
                        t += a_arr[a_st + k] * b_arr[b_st + k];

                    res[i, j] = t;
                }

        }

        /// <summary>
        /// Multiplies the specified <see cref="Matrix"/> with specified Vector <see cref="vec"/>.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="vec">The vec.</param>
        /// <returns></returns>
        /// <exception cref="BriefFiniteElementNet.MatrixException"></exception>
        public static double[] Multiply(Matrix m, double[] vec)
        {
            if (m.columns != vec.Length)
                throw new MatrixException();

            var c = m.columns;
            var r = m.rows;

            var buf = new double[vec.Length];

            for (var i = 0; i < r; i++)
            {
                var tmp = 0.0;

                for (var j = 0; j < c; j++)
                {
                    tmp += m[i, j]*vec[j];
                }

                buf[i] = tmp;
            }

            return buf;
        }

        #region Dynamic Functions

        /// <summary>
        /// Sets the member at defined row and column to defined value.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        public void At(int row, int column, double value)
        {
            MatrixException.ThrowIf(row >= this.RowCount || column >= this.ColumnCount,
                   "Invalid column or row specified");

            this.Values[column * this.rows + row] = value;
        }

        /// <summary>
        /// Gets the member at defined row and column.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public double At(int row, int column)
        {
            MatrixException.ThrowIf(row >= this.RowCount || column >= this.ColumnCount,
                   "Invalid column or row specified");

            return this.Values[column * this.rows + row];
        }

        /// <summary>
        /// Substitutes the defined row with defined values.
        /// </summary>
        /// <param name="i">The row.</param>
        /// <param name="values">The values.</param>
        public void SetRow(int i, params double[] values)
        {
            
            if (values.Count() != this.ColumnCount)
                throw new ArgumentOutOfRangeException("values");

            for (int j = 0; j < this.ColumnCount; j++)
            {
                this.Values[j*this.RowCount + i] = values[j];
            }
        }

        /// <summary>
        /// Substitutes the defined column with defined values.
        /// </summary>
        /// <param name="j">The column.</param>
        /// <param name="values">The values.</param>
        public void SetColumn(int j, params double[] values)
        {
            if (values.Count() != this.RowCount)
                throw new ArgumentOutOfRangeException("values");


            for (int i = 0; i < this.RowCount; i++)
            {
                this.Values[j * this.RowCount + i] = values[i];
            }
        }

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
        
        /// <summary>
        /// Provides a shallow copy of this matrix in O(row).
        /// </summary>
        /// <returns></returns>
        public Matrix Clone()
        {
            var buf = new Matrix(this.RowCount, this.ColumnCount);

            buf.Values = (double[]) this.Values.Clone();
            return buf;
        }

        /// <summary>
        /// Swaps each matrix entry A[i, j] with A[j, i].
        /// </summary>
        /// <returns>A transposed matrix.</returns>
        public Matrix Transpose()
        {
            var buf = new Matrix(this.ColumnCount, this.RowCount);

            var newMatrix = buf.Values;

            for (int row = 0; row < this.RowCount; row++)
                for (int column = 0; column < this.ColumnCount; column++)
                    //newMatrix[column*this.RowCount + row] = this.CoreArray[row*this.RowCount + column];
                    buf[column, row] = this[row, column];

            buf.Values = newMatrix;
            return buf;
        }

        public void InPlaceTranspose()
        {
            //var buf = new Matrix(this.ColumnCount, this.RowCount);

            //var newMatrix = buf.CoreArray;

            for (int row = 0; row < this.RowCount; row++)
                for (int column = row; column < this.ColumnCount; column++)
                {
                    var tmp = this[row, column];

                    this[row, column] = this[column, row];

                    this[column, row] = tmp;
                }

            //buf.CoreArray = newMatrix;
            //return buf;
        }

        #region Equality Members

        protected bool Equals(Matrix other)
        {
            if (other.RowCount != this.RowCount)
                return false;

            if (other.ColumnCount != this.ColumnCount)
                return false;

            for (int i = 0; i < other.Values.Length; i++)
            {
                if (!MathUtil.Equals(this.Values[i], other.Values[i]))
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Matrix) obj);
        }

        #endregion

        #region Checks

        /// <summary>
        /// Checks if number of rows equals number of columns.
        /// </summary>
        /// <returns>True iff matrix is n by n.</returns>
        public bool IsSquare()
        {
            return (this.columns == this.rows);
        }

        #endregion

        /// <summary>
        /// Swaps rows at specified indices. The latter do not have to be ordered.
        /// When equal, nothing is done.
        /// </summary>
        /// <param name="i1">One-based index of first row.</param>
        /// <param name="i2">One-based index of second row.</param>        
        public void SwapRows(int i1, int i2)
        {
            if (i1 < 0 || i1 >= rows || i2 < 0 || i2 >= rows)
                throw new ArgumentException("Indices must be positive and <= number of rows.");

            if (i1 == i2)
                return;

            for (int i = 0; i < columns; i++)
            {
                var tmp = this[i1, i];

                this[i1, i] = this[i2, i];
                this[i2, i] = tmp;
            }
        }

        /// <summary>
        /// Retrieves row vector at specfifed index and deletes it from matrix.
        /// </summary>
        /// <param name="i">One-based index at which to extract.</param>
        /// <returns>Row vector.</returns>
        public Matrix ExtractRow(int i)
        {
            if (i >= this.RowCount || i < 0)
                throw new ArgumentOutOfRangeException("i");

            var mtx = new Matrix(1, this.ColumnCount);


            for (int j = 0; j < this.ColumnCount; j++)
            {
                mtx.Values[j] = this.Values[j*this.RowCount + i];
            }

            return mtx;
        }

        /// <summary>
        /// Gets the determinant of matrix
        /// </summary>
        /// <returns></returns>
        public double Determinant()
        {
            if (rows == columns && columns == 1)
                return this[0, 0];
            //Seems working good!

            if (!IsSquare())
                throw new InvalidOperationException();

            var clone = this.Clone();

            var n = this.rows;

            var sign = 1.0;

            var epsi1on = 1e-10*clone.Values.Select(System.Math.Abs).Min();

            if (epsi1on == 0)
                epsi1on = 1e-9;


            //this[row,column] = this.CoreArray[column*this.rowCount + row]
            for (var i = 0; i < n - 1; i++)
            {
                if (System.Math.Abs(clone[i, i]) < epsi1on)
                {
                    var firstNonZero = -1;

                    for (var k = i + 1; k < n; k++)
                        if (System.Math.Abs(clone[k, i]) > epsi1on)
                            firstNonZero = k;

                    if (firstNonZero == -1)
                        throw new OperationCanceledException();
                    else
                    {
                        clone.SwapRows(firstNonZero, i);
                        sign = -sign;
                    }
                }


                for (var j = i + 1; j < n; j++)
                {
                    var alfa = (clone.Values[j*n + i]/clone.Values[i*n + i]);

                    for (var k = i; k < n; k++)
                    {
                        clone.Values[j*n + k] -= alfa*clone.Values[i*n + k];
                    }
                }
            }

            var buf = sign;

            var arr = new double[n];

            for (var i = 0; i < n; i++)
                arr[i] = clone.Values[i*n + i];

            Array.Sort(arr);

            for (var i = 0; i < n; i++)
                buf = buf*arr[n - i - 1];

            return buf;
        }

        /// <summary>
        /// Gets the inverse of matrix
        /// </summary>
        /// <returns></returns>
        public Matrix Inverse()
        {
            if (!IsSquare())
                throw new InvalidOperationException();

            //seems working good!
            var n = this.rows;
            var clone = this.Clone();
            var eye = Eye(n);

            var epsi1on = 1e-10*clone.Values.Select(System.Math.Abs).Min();

            if (epsi1on == 0)
                epsi1on = 1e-9;

            /**/

            var perm = new List<int>();

            for (var j = 0; j < n - 1; j++)
            {
                for (var i = j + 1; i < n; i++)
                {
                    if (System.Math.Abs(clone[j, j]) < epsi1on)
                    {
                        var firstNonZero = -1;

                        for (var k = j + 1; k < n; k++)
                            if (System.Math.Abs(clone[k, j]) > epsi1on)
                                firstNonZero = k;

                        if (firstNonZero == -1)
                            throw new OperationCanceledException();
                        else
                        {
                            clone.SwapRows(firstNonZero, j);
                            eye.SwapRows(firstNonZero, j);

                            perm.Add(j);
                            perm.Add(firstNonZero);
                        }
                    }

                    var alfa = clone[i, j]/clone[j, j];

                    for (var k = 0; k < n; k++)
                    {
                        clone[i, k] -= alfa*clone[j, k];
                        eye[i, k] -= alfa*eye[j, k];
                    }
                }
            }

            /**/

            for (var j = n - 1; j > 0; j--)
            {
                for (var i = j - 1; i >= 0; i--)
                {
                    if (System.Math.Abs(clone[j, j]) < epsi1on)
                    {
                        var firstNonZero = -1;

                        for (var k = j - 1; k >= 0; k--)
                            if (System.Math.Abs(clone[k, j]) > epsi1on)
                                firstNonZero = k;

                        if (firstNonZero == -1)
                            throw new OperationCanceledException();
                        else
                        {
                            clone.SwapRows(firstNonZero, j);
                            eye.SwapRows(firstNonZero, j);

                            perm.Add(j);
                            perm.Add(firstNonZero);
                        }
                    }

                    var alfa = clone[i, j]/clone[j, j];

                    for (var k = n - 1; k >= 0; k--)
                    {
                        clone[i, k] -= alfa*clone[j, k];
                        eye[i, k] -= alfa*eye[j, k];
                    }
                }
            }

            /**/

            for (var i = 0; i < n; i++)
            {
                var alfa = 1/clone[i, i];

                for (var j = 0; j < n; j++)
                {
                    clone[i, j] *= alfa;
                    eye[i, j] *= alfa;
                }
            }

            /**/

            return eye;
        }


        public Matrix Inverse2()
        {
            if (!IsSquare())
                throw new InvalidOperationException();

            //seems working good!
            var n = this.rows;
            var clone = this.Clone();
            var eye = Eye(n);

            var epsi1on = 1e-10*clone.Values.Select(System.Math.Abs).Min();

            if (epsi1on == 0)
                epsi1on = 1e-9;

            /**/

            var perm = new List<int>();

            var clonea = clone.Values;
            var eyea = eye.Values;

            for (var j = 0; j < n - 1; j++)
            {
                for (var i = j + 1; i < n; i++)
                {
                    if (System.Math.Abs(clonea[j + j*n]) < epsi1on)
                    {
                        var firstNonZero = -1;

                        for (var k = j + 1; k < n; k++)
                            if (System.Math.Abs(clonea[k + j*n]) > epsi1on)
                                firstNonZero = k;

                        if (firstNonZero == -1)
                            throw new OperationCanceledException();
                        else
                        {
                            clone.SwapRows(firstNonZero, j);
                            eye.SwapRows(firstNonZero, j);

                            perm.Add(j);
                            perm.Add(firstNonZero);
                        }
                    }

                    var alfa = clonea[i + j*n]/clonea[j + j*n];

                    for (var k = 0; k < n; k++)
                    {
                        clonea[i + k*n] -= alfa*clonea[j + k*n];
                        eyea[i + k*n] -= alfa*eyea[j + k*n];
                    }
                }
            }

            /**/

            for (var j = n - 1; j > 0; j--)
            {
                for (var i = j - 1; i >= 0; i--)
                {
                    if (System.Math.Abs(clonea[j + j*n]) < epsi1on)
                    {
                        var firstNonZero = -1;

                        for (var k = j - 1; k >= 0; k--)
                            if (System.Math.Abs(clonea[k + j*n]) > epsi1on)
                                firstNonZero = k;

                        if (firstNonZero == -1)
                            throw new OperationCanceledException();
                        else
                        {
                            clone.SwapRows(firstNonZero, j);
                            eye.SwapRows(firstNonZero, j);

                            perm.Add(j);
                            perm.Add(firstNonZero);
                        }
                    }

                    var alfa = clonea[i + j*n]/clonea[j + j*n];

                    for (var k = n - 1; k >= 0; k--)
                    {
                        clonea[i + k*n] -= alfa*clonea[j + k*n];
                        eyea[i + k*n] -= alfa*eyea[j + k*n];
                    }
                }
            }

            /**/

            for (var i = 0; i < n; i++)
            {
                var alfa = 1/clonea[i + i*n];

                for (var j = 0; j < n; j++)
                {
                    clonea[i + j*n] *= alfa;
                    eyea[i + j*n] *= alfa;
                }
            }

            /**/

            return eye;
        }

        /// <summary>
        /// return the value that this * value = rightSide
        /// </summary>
        /// <param name="rightSide"></param>
        /// <returns></returns>
        public double[] Solve(double[] rightSide)
        {
            if (!IsSquare())
                throw new Exception("Matrix must be square");

            var buf = (double[])rightSide.Clone();

            var n = this.rows;

            var clone = Pool != null ? Pool.Allocate(n, n) : new Matrix(n, n);

            this.values.CopyTo(clone.values,0);

            var canditates = new bool[n];//if true, then row is canditate for pivot
            var pivoted = new int[n];//if nonNegative, then row is pivoted once, cannot be used again as pivot

            pivoted.FillWith(-1);

            

            {//find next pivot

                for (var j = 0; j < n; j++) // do for each column
                {
                    canditates.FillWith(false);
                    for (var i = 0; i < n; i++) // find pivot canditate
                    {
                        if (pivoted[i] != -1)
                            continue;

                        if (clone[i, j] != 0)
                            canditates[i] = true;
                    }

                    var pivot = canditates.FirstIndexOf(true);

                    if (pivot == -1)
                        throw new Exception("singular matrix");

                    pivoted[pivot] = j;

                    buf[pivot] *= 1 / clone[pivot, j];

                    clone.MultiplyRowByConstant(pivot, 1 / clone[pivot, j]);
                    

                    for (var i = 0; i < n; i++) //eliminate each row with pivot
                    {
                        if (i == pivot)
                            continue;

                        var alpha = -clone[i, j];

                        if (clone[i, j] == 0)
                            continue;

                        for (var jj = 0; jj < n; jj++)
                        {
                            clone[i, jj] += alpha * clone[pivot, jj];
                        }

                        buf[i] += alpha * buf[pivot];
                    }
                }
            }

            var buf2 = new double[n];// Matrix.Multiply(clone.Transpose(), buf);

            for (var i = 0; i < n; i++)
                buf2[pivoted[i]] = buf[i];

            if (Pool != null)
                clone.ReturnToPool();

            return buf2;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Creates a new Identity matrix.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns></returns>
        public static Matrix Eye(int n)
        {
            var buf = new Matrix(n, n);

            for (int i = 0; i < n; i++)
                buf.Values[i*n + i] = 1.0;

            return buf;
        }

        #region Operators

        public static Matrix operator *(
            Matrix m1, Matrix m2)
        {
            return Matrix.Multiply(m1, m2);
        }

        public static double[] operator *(
            Matrix m1, double[] vec)
        {
            var m2 = Matrix.OfVector(vec);

            var res = Matrix.Multiply(m1, m2);

            return res.values;
        }

        public static Matrix operator *(
            double coeff, Matrix mat)
        {
            var newMat = new double[mat.RowCount*mat.ColumnCount];


            for (int i = 0; i < newMat.Length; i++)
            {
                newMat[i] = coeff*mat.Values[i];
            }

            var buf = new Matrix(mat.RowCount, mat.ColumnCount);
            buf.Values = newMat;

            return buf;
        }

        public static Matrix operator -(
            Matrix mat)
        {
            var buf = new Matrix(mat.RowCount, mat.ColumnCount);
            ;

            for (int i = 0; i < buf.Values.Length; i++)
            {
                buf.Values[i] = -mat.Values[i];
            }

            return buf;
        }

        public static Matrix operator +(
            Matrix mat1, Matrix mat2)
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

        public static void Plus(Matrix mat1, Matrix mat2, Matrix result)
        {
            MatrixException.ThrowIf(
                mat1.RowCount != mat2.RowCount ||
                mat2.RowCount != result.RowCount ||

                mat1.ColumnCount != mat2.ColumnCount ||
                mat2.ColumnCount != result.ColumnCount,
                "Inconsistent matrix sizes");

            var n = mat1.rows * mat1.columns;

            for (int i = 0; i < n; i++)
            {
                result.Values[i] = mat1.Values[i] + mat2.Values[i];
            }
        }


        /// <summary>
        /// mat1 = mat1 + mat2
        /// </summary>
        /// <param name="mat1"></param>
        /// <param name="mat2"></param>
        public static void InplacePlus(Matrix mat1, Matrix mat2)
        {
            MatrixException.ThrowIf(
                mat1.RowCount != mat2.RowCount || mat1.ColumnCount != mat2.ColumnCount
                , "Inconsistent matrix sizes");

            var n = mat1.rows * mat1.columns;

            for (int i = 0; i < n; i++)
            {
                mat1.Values[i] = mat1.Values[i] + mat2.Values[i];
            }
        }

        /// <summary>
        /// mat1 = mat1 + mat2 * coefficient
        /// </summary>
        /// <param name="mat1"></param>
        /// <param name="mat2"></param>
        /// <param name="coefficient"></param>
        public static void InplacePlus(Matrix mat1, Matrix mat2,double coefficient)
        {
            MatrixException.ThrowIf(
                mat1.RowCount != mat2.RowCount || mat1.ColumnCount != mat2.ColumnCount
                , "Inconsistent matrix sizes");

            var n = mat1.rows * mat1.columns;

            for (int i = 0; i < n; i++)
                mat1.Values[i] = mat1.Values[i] + mat2.Values[i] * coefficient;
        }

        public static Matrix operator -(
            Matrix mat1, Matrix mat2)
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

        public static bool operator ==(Matrix left, Matrix right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Matrix left, Matrix right)
        {
            return !Equals(left, right);
        }

        #endregion

        #endregion


        public void Replace(double oldValue, double newValue)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Equals(oldValue))
                    values[i] = newValue;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var mtx = this;

            var epsi1on = mtx.Values.Select(i => System.Math.Abs(i)).Min()*1e-9;

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

        public static Matrix DotDivide(Matrix a1, Matrix a2)
        {
            if (a1.rows != a2.rows || a1.columns != a2.columns)
                throw new Exception();

            var buf = new Matrix(a1.rows, a1.columns);

            for (int i = 0; i < a1.values.Length; i++)
            {
                buf.values[i] = a1.values[i] / a2.values[i];
            }

            return buf;
        }

        public static Matrix DotMultiply(Matrix a1, Matrix a2)
        {
            if (a1.rows != a2.rows || a1.columns != a2.columns)
                throw new Exception();

            var buf = new Matrix(a1.rows, a1.columns);

            for (int i = 0; i < a1.values.Length; i++)
            {
                buf.values[i] = a1.values[i] * a2.values[i];
            }

            return buf;
        }

        #region Serialization stuff

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("rowCount", rows);
            info.AddValue("columnCount", columns);
            info.AddValue("coreArray", values);
        }

        protected Matrix(
            SerializationInfo info,
            StreamingContext context)
        {
            this.rows = (int) info.GetValue("rowCount", typeof (int));
            this.columns = (int) info.GetValue("columnCount", typeof (int));
            this.values = (double[]) info.GetValue("coreArray", typeof (double[]));
        }

        #endregion

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<double> GetEnumerator()
        {
            return (IEnumerator<double>) new List<double>(values).GetEnumerator();
        }


        /// <summary>
        /// Fills the matrix rowise (all rows of new matrix are beside each other).
        /// </summary>
        /// <param name="members">The members.</param>
        /// <exception cref="System.Exception"></exception>
        public void FillMatrixRowise(params double[] members)
        {
            if (members.Length != this.values.Length)
                throw new Exception();

            for (int i = 0; i < this.rows; i++)
            {
                for (int j = 0; j < this.columns; j++)
                {
                    //column * this.rowCount + row
                    this[i, j] = members[this.columns*i + j];
                }
            }
        }

        #region nonzero Pattern

        /// <summary>
        /// The nonzero pattern for each column
        /// </summary>
        [NonSerialized]
        internal List<int>[] ColumnNonzeros;

        /// <summary>
        /// The nonzero pattern for each row
        /// </summary>
        [NonSerialized]
        internal List<int>[] RowNonzeros;


        public void UpdateNonzeroPattern()
        {
            #region row nonzeros

            if (RowNonzeros == null)
                RowNonzeros = new List<int>[rows];

            for (int i = 0; i < rows; i++)
            {
                if (RowNonzeros[i] == null)
                    RowNonzeros[i] = new List<int>();
                else
                    RowNonzeros[i].Clear();


                for (int j = 0; j < columns; j++)
                {
                    if (!this[i, j].Equals(0.0))
                        RowNonzeros[i].Add(j);
                }
            }

            #endregion

            #region col nonzeros

            if (ColumnNonzeros == null)
                ColumnNonzeros = new List<int>[columns];

            for (int j = 0; j < columns; j++)
            {
                if (ColumnNonzeros[j] == null)
                    ColumnNonzeros[j] = new List<int>();
                else
                    ColumnNonzeros[j].Clear();


                for (int i = 0; i < rows; i++)
                {
                    if (!this[i, j].Equals(0.0))
                        ColumnNonzeros[j].Add(i);
                }
            }

            #endregion
        }

        #endregion
    }
}