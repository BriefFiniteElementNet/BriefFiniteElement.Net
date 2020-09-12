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

        public static Matrix DiagonallyRepeat(Matrix mtx, int n)
        {
            var r = mtx.rowCount;
            var c = mtx.columnCount;

            var buf = new Matrix(r*n, c*n);

            for (var i = 0; i < n; i++)
                //for (var j = 0; j < n; j++)
                for (var ii = 0; ii < mtx.rowCount; ii++)
                    for (var jj = 0; jj < mtx.columnCount; jj++)
                        buf[i*r + ii, i*c + jj] = mtx[ii, jj];

            return buf;
        }

        /// <summary>
        /// Creates a matrix from rowcount, colcount and core array
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="cols">The cols.</param>
        /// <param name="coreArr">The core arr.</param>
        /// <remarks>
        /// Unlike constructor, do not clones the <see cref="coreArr"/> for better performance</remarks>
        /// <returns></returns>
        public static Matrix FromRowColCoreArray(int rows, int cols, double[] coreArr)
        {
            return new Matrix() {columnCount = cols, rowCount = rows, coreArray = coreArr};
        }

        public static Matrix RandomMatrix(int m, int n)
        {
            var buf = new Matrix(m, n);

            var rnd = new Random(0);

            for (int i = 0; i < m*n; i++)
            {
                buf.CoreArray[i] = rnd.NextDouble()*100;
            }


            return buf;
        }

        public double[] CoreArray
        {
            get { return coreArray; }
            internal set { coreArray = value; }
        }

        private double[] coreArray;

        /// <summary>
        /// Number of rows of the matrix.
        /// </summary>
        public int RowCount
        {
            get { return rowCount; }
        }

        /// <summary>
        /// Number of columns of the matrix.
        /// </summary>
        public int ColumnCount
        {
            get { return columnCount; }
        }


        /// <summary>
        /// Number of rows of the matrix.
        /// </summary>
        private int rowCount;

        /// <summary>
        /// Number of columns of the matrix.
        /// </summary>
        private int columnCount;

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
            rowCount = m;
            columnCount = n;

            if (rowCount <= 0)
                throw new ArgumentException("row");

            if (columnCount <= 0)
                throw new ArgumentException("n");

            this.CoreArray = new double[m*n];
        }

        /// <summary>
        /// Initializes a new square matrix
        /// </summary>
        /// <param name="n">The matrix dimension.</param>
        public Matrix(int n) : this()
        {
            rowCount = n;
            columnCount = n;

            if (n <= 0)
                throw new ArgumentException("n");

            this.CoreArray = new double[n*n];
        }


        public static Matrix From2DArray(double[,] vals)
        {
            
            var rows = vals.GetLength(0);
            var cols = vals.GetLength(1);

            var buf = new Matrix(rows, cols);
            buf.rowCount = rows;
            buf.columnCount = cols;

            buf.coreArray = new double[rows * cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    buf.CoreArray[j * rows + i] = vals[i, j];


            return buf;
        }

        public static Matrix FromJaggedArray(double[][] vals)
        {
            var rows = vals.Length;
            var cols = vals.Select(i => i.Length).Max();

            var buf = new Matrix(rows, cols);
            buf.rowCount = rows;
            buf.columnCount = cols;

            buf.coreArray = new double[rows * cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (vals[i].Length > j)
                        buf.CoreArray[j * rows + i] = vals[i][j];

            return buf;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class as a column vector.
        /// </summary>
        /// <param name="vals">The vals.</param>
        public Matrix(double[] vals) : this()
        {
            this.rowCount = vals.Length;
            this.columnCount = 1;
            this.CoreArray = (double[]) vals.Clone();
        }

        /// <summary>
        /// creates
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="coreArray"></param>
        public Matrix(int rows, int cols, ref double[] coreArray) :this()
        {
            rowCount = rows;
            columnCount = cols;

            if (rowCount <= 0)
                throw new ArgumentException("rows");

            if (columnCount <= 0)
                throw new ArgumentException("cols");

            this.CoreArray = coreArray;

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
                return GetMember(row, column);
            }

            set
            {
                SetMember(row, column, value);
            }
        }

        public static Matrix Multiply(Matrix m1, Matrix m2)
        {
            if (m1.ColumnCount != m2.RowCount)
                throw new InvalidOperationException("No consistent dimensions");

            var res = new Matrix(m1.RowCount, m2.ColumnCount);

            for (int i = 0; i < m1.rowCount; i++)
                for (int j = 0; j < m2.columnCount; j++)
                    for (int k = 0; k < m1.columnCount; k++)
                    {
                        res.CoreArray[j*res.rowCount + i] +=
                            m1.CoreArray[k*m1.rowCount + i]*
                            m2.CoreArray[j*m2.rowCount + k];
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
            if (m1.rowCount != m2.RowCount)
                throw new InvalidOperationException("No consistent dimensions");

            var res = result;

            if (res.rowCount != m1.columnCount || res.ColumnCount != m2.columnCount)
            {
                throw new Exception("result dimension mismatch");
            }

            var a = m1;
            var b = m2;


            var a_arr = a.coreArray;
            var b_arr = b.coreArray;

            var vecLength = a.rowCount;

            for (var i = 0; i < a.columnCount; i++)
                for (var j = 0; j < b.columnCount; j++)
                {
                    var t = 0.0;

                    var a_st = i * a.rowCount;
                    var b_st = j * b.rowCount;

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
            if (m.columnCount != vec.Length)
                throw new MatrixException();

            var c = m.columnCount;
            var r = m.rowCount;

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
        public void SetMember(int row, int column, double value)
        {
            MatrixException.ThrowIf(row >= this.RowCount || column >= this.ColumnCount,
                   "Invalid column or row specified");

            this.CoreArray[column * this.rowCount + row] = value;
        }

        /// <summary>
        /// Gets the member at defined row and column.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public double GetMember(int row, int column)
        {
            MatrixException.ThrowIf(row >= this.RowCount || column >= this.ColumnCount,
                   "Invalid column or row specified");

            return this.CoreArray[column * this.rowCount + row];
        }

        /// <summary>
        /// Substitutes the defined row with defined values.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="values">The values.</param>
        public void SetRow(int i, params double[] values)
        {
            
            if (values.Count() != this.ColumnCount)
                throw new ArgumentOutOfRangeException("values");

            for (int j = 0; j < this.ColumnCount; j++)
            {
                this.CoreArray[j*this.RowCount + i] = values[j];
            }
        }

        public static Dictionary<int, int> dists = new Dictionary<int, int>();


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
        /// Substitutes the defined column with defined values.
        /// </summary>
        /// <param name="j">The j.</param>
        /// <param name="values">The values.</param>
        public void SetColumn(int j, params double[] values)
        {
            if (values.Count() != this.RowCount)
                throw new ArgumentOutOfRangeException("values");


            for (int i = 0; i < this.RowCount; i++)
            {
                this.CoreArray[j*this.RowCount + i] = values[i];
            }
        }

        /// <summary>
        /// Provides a shallow copy of this matrix in O(row).
        /// </summary>
        /// <returns></returns>
        public Matrix Clone()
        {
            var buf = new Matrix(this.RowCount, this.ColumnCount);

            buf.CoreArray = (double[]) this.CoreArray.Clone();
            return buf;
        }

        /// <summary>
        /// Swaps each matrix entry A[i, j] with A[j, i].
        /// </summary>
        /// <returns>A transposed matrix.</returns>
        public Matrix Transpose()
        {
            var buf = new Matrix(this.ColumnCount, this.RowCount);

            var newMatrix = buf.CoreArray;

            for (int row = 0; row < this.RowCount; row++)
                for (int column = 0; column < this.ColumnCount; column++)
                    //newMatrix[column*this.RowCount + row] = this.CoreArray[row*this.RowCount + column];
                    buf[column, row] = this[row, column];

            buf.CoreArray = newMatrix;
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

            for (int i = 0; i < other.CoreArray.Length; i++)
            {
                if (!MathUtil.Equals(this.CoreArray[i], other.CoreArray[i]))
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
            return (this.columnCount == this.rowCount);
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
            if (i1 < 0 || i1 >= rowCount || i2 < 0 || i2 >= rowCount)
                throw new ArgumentException("Indices must be positive and <= number of rows.");

            if (i1 == i2)
                return;

            for (int i = 0; i < columnCount; i++)
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
                mtx.CoreArray[j] = this.CoreArray[j*this.RowCount + i];
            }

            return mtx;
        }

        /// <summary>
        /// Gets the determinant of matrix
        /// </summary>
        /// <returns></returns>
        public double Determinant()
        {
            if (rowCount == columnCount && columnCount == 1)
                return this[0, 0];
            //Seems working good!

            if (!IsSquare())
                throw new InvalidOperationException();

            var clone = this.Clone();

            var n = this.rowCount;

            var sign = 1.0;

            var epsi1on = 1e-10*clone.CoreArray.Select(System.Math.Abs).Min();

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
                    var alfa = (clone.CoreArray[j*n + i]/clone.CoreArray[i*n + i]);

                    for (var k = i; k < n; k++)
                    {
                        clone.CoreArray[j*n + k] -= alfa*clone.CoreArray[i*n + k];
                    }
                }
            }

            var buf = sign;

            var arr = new double[n];

            for (var i = 0; i < n; i++)
                arr[i] = clone.CoreArray[i*n + i];

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
            var n = this.rowCount;
            var clone = this.Clone();
            var eye = Eye(n);

            var epsi1on = 1e-10*clone.CoreArray.Select(System.Math.Abs).Min();

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
            var n = this.rowCount;
            var clone = this.Clone();
            var eye = Eye(n);

            var epsi1on = 1e-10*clone.CoreArray.Select(System.Math.Abs).Min();

            if (epsi1on == 0)
                epsi1on = 1e-9;

            /**/

            var perm = new List<int>();

            var clonea = clone.CoreArray;
            var eyea = eye.CoreArray;

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

            var n = this.rowCount;

            var clone = Pool != null ? Pool.Allocate(n, n) : new Matrix(n, n);

            this.coreArray.CopyTo(clone.coreArray,0);

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
                buf.CoreArray[i*n + i] = 1.0;

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
            var m2 = new Matrix(vec);

            var res = Matrix.Multiply(m1, m2);

            return res.coreArray;
        }

        public static Matrix operator *(
            double coeff, Matrix mat)
        {
            var newMat = new double[mat.RowCount*mat.ColumnCount];


            for (int i = 0; i < newMat.Length; i++)
            {
                newMat[i] = coeff*mat.CoreArray[i];
            }

            var buf = new Matrix(mat.RowCount, mat.ColumnCount);
            buf.CoreArray = newMat;

            return buf;
        }

        public static Matrix operator -(
            Matrix mat)
        {
            var buf = new Matrix(mat.RowCount, mat.ColumnCount);
            ;

            for (int i = 0; i < buf.CoreArray.Length; i++)
            {
                buf.CoreArray[i] = -mat.CoreArray[i];
            }

            return buf;
        }

        public static Matrix operator +(
            Matrix mat1, Matrix mat2)
        {
            MatrixException.ThrowIf(mat1.RowCount != mat2.RowCount || mat1.ColumnCount != mat2.ColumnCount,
                "Inconsistent matrix sizes");

            var buf = new Matrix(mat1.RowCount, mat1.ColumnCount);

            for (int i = 0; i < buf.CoreArray.Length; i++)
            {
                buf.CoreArray[i] = mat1.CoreArray[i] + mat2.CoreArray[i];
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

            var n = mat1.rowCount * mat1.columnCount;

            for (int i = 0; i < n; i++)
            {
                result.CoreArray[i] = mat1.CoreArray[i] + mat2.CoreArray[i];
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

            var n = mat1.rowCount * mat1.columnCount;

            for (int i = 0; i < n; i++)
            {
                mat1.CoreArray[i] = mat1.CoreArray[i] + mat2.CoreArray[i];
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

            var n = mat1.rowCount * mat1.columnCount;

            for (int i = 0; i < n; i++)
                mat1.CoreArray[i] = mat1.CoreArray[i] + mat2.CoreArray[i] * coefficient;
        }

        public static Matrix operator -(
            Matrix mat1, Matrix mat2)
        {
            MatrixException.ThrowIf(mat1.RowCount != mat2.RowCount || mat1.ColumnCount != mat2.ColumnCount,
                "Inconsistent matrix sizes");

            var buf = new Matrix(mat1.RowCount, mat1.ColumnCount);

            for (int i = 0; i < buf.CoreArray.Length; i++)
            {
                buf.CoreArray[i] = mat1.CoreArray[i] - mat2.CoreArray[i];
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
            for (int i = 0; i < coreArray.Length; i++)
            {
                if (coreArray[i].Equals(oldValue))
                    coreArray[i] = newValue;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var mtx = this;

            var epsi1on = mtx.CoreArray.Select(i => System.Math.Abs(i)).Min()*1e-9;

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

        public void FillRow(int rowNum,params double[] values)
        {
            if (values.Length != this.columnCount)
                throw new Exception();

            for (var j = 0; j < this.columnCount; j++)
            {
                this[rowNum, j] = values[j];
            }
        }

        public static Matrix DotDivide(Matrix a1, Matrix a2)
        {
            if (a1.rowCount != a2.rowCount || a1.columnCount != a2.columnCount)
                throw new Exception();

            var buf = new Matrix(a1.rowCount, a1.columnCount);

            for (int i = 0; i < a1.coreArray.Length; i++)
            {
                buf.coreArray[i] = a1.coreArray[i] / a2.coreArray[i];
            }

            return buf;
        }

        public static Matrix DotMultiply(Matrix a1, Matrix a2)
        {
            if (a1.rowCount != a2.rowCount || a1.columnCount != a2.columnCount)
                throw new Exception();

            var buf = new Matrix(a1.rowCount, a1.columnCount);

            for (int i = 0; i < a1.coreArray.Length; i++)
            {
                buf.coreArray[i] = a1.coreArray[i] * a2.coreArray[i];
            }

            return buf;
        }

        public void FillColumn(int colNum, params double[] values)
        {
            if (values.Length != this.rowCount)
                throw new Exception();

            for (var j = 0; j < this.rowCount; j++)
            {
                this[j, colNum] = values[j];
            }
        }

        #region Serialization stuff

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("rowCount", rowCount);
            info.AddValue("columnCount", columnCount);
            info.AddValue("coreArray", coreArray);
        }

        protected Matrix(
            SerializationInfo info,
            StreamingContext context)
        {
            this.rowCount = (int) info.GetValue("rowCount", typeof (int));
            this.columnCount = (int) info.GetValue("columnCount", typeof (int));
            this.coreArray = (double[]) info.GetValue("coreArray", typeof (double[]));
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
            return (IEnumerator<double>) new List<double>(coreArray).GetEnumerator();
        }


        /// <summary>
        /// Fills the matrix rowise (all rows of new matrix are beside each other).
        /// </summary>
        /// <param name="members">The members.</param>
        /// <exception cref="System.Exception"></exception>
        public void FillMatrixRowise(params double[] members)
        {
            if (members.Length != this.coreArray.Length)
                throw new Exception();

            for (int i = 0; i < this.rowCount; i++)
            {
                for (int j = 0; j < this.columnCount; j++)
                {
                    //column * this.rowCount + row
                    this[i, j] = members[this.columnCount*i + j];
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
                RowNonzeros = new List<int>[rowCount];

            for (int i = 0; i < rowCount; i++)
            {
                if (RowNonzeros[i] == null)
                    RowNonzeros[i] = new List<int>();
                else
                    RowNonzeros[i].Clear();


                for (int j = 0; j < columnCount; j++)
                {
                    if (!this[i, j].Equals(0.0))
                        RowNonzeros[i].Add(j);
                }
            }

            #endregion

            #region col nonzeros

            if (ColumnNonzeros == null)
                ColumnNonzeros = new List<int>[columnCount];

            for (int j = 0; j < columnCount; j++)
            {
                if (ColumnNonzeros[j] == null)
                    ColumnNonzeros[j] = new List<int>();
                else
                    ColumnNonzeros[j].Clear();


                for (int i = 0; i < rowCount; i++)
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