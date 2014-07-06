using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace BriefFiniteElementNet
{
    [DebuggerDisplay("Matrix {RowCount} x {ColumnCount}")]
    [Serializable]
    public class Matrix : ISerializable, IEnumerable<double>
    {
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

            for (int i = 0; i < m * n; i++)
            {
                buf.CoreArray[i] = rnd.NextDouble() * 100;
            }


            return buf;
        }

        public static Matrix CholeskyDecomposeSymmetric(Matrix a)
        {
            if (a.rowCount != a.columnCount)
                throw new InvalidOperationException();

            var n = a.columnCount;

            var l = new Matrix(n, n);

            for (var i = 0; i < n; i++)
            {
                var lii = a[i, i];

                for (int t = 0; t < i; t++)
                {
                    lii -= l[i, t] * l[i, t];
                }

                lii = System.Math.Sqrt(lii);
                l[i, i] = lii;


                for (var j = i + 1; j < n; j++)
                {
                    var lji = a[i, j];

                    for (int t = 0; t < i; t++)
                    {
                        lji -= l[i, t] * l[j, t];
                    }

                    lji = lji / lii;

                    l[j, i] = lji;
                }
            }

            return l;
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

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="Matrix"/> class from being created.
        /// </summary>
        private Matrix()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// </summary>
        /// <param name="m">The row.</param>
        /// <param name="n">The n.</param>
        /// <exception cref="System.ArgumentException">
        /// row
        /// or
        /// n
        /// </exception>
        public Matrix(int m, int n)
        {
            rowCount = m;
            columnCount = n;

            if (rowCount <= 0)
                throw new ArgumentException("row");

            if (columnCount <= 0)
                throw new ArgumentException("n");

            this.CoreArray = new double[m * n];
        }

        /// <summary>
        /// Initializes a new square matrix
        /// </summary>
        /// <param name="n">The matrix dimension.</param>
        public Matrix(int n)
        {
            rowCount = n;
            columnCount = n;

            if (n <= 0)
                throw new ArgumentException("n");

            this.CoreArray = new double[n * n];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class with a 2-d double array.
        /// </summary>
        /// <param name="vals">The values.</param>
        public Matrix(double[,] vals)
        {
            var rows = vals.GetLength(0);
            var cols = vals.GetLength(1);

            //var buf = new Matrix(rows, cols);
            this.rowCount = rows;
            this.columnCount = cols;

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    this.CoreArray[j * rows + i] = vals[i, j];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class with a 2-d double array.
        /// </summary>
        /// <param name="vals">The values.</param>
        public Matrix(double[][] vals)
        {
            var rows = vals.Length;
            var cols = vals.Select(i => i.Length).Max();

            //var buf = new Matrix(rows, cols);
            this.rowCount = rows;
            this.columnCount = cols;

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (vals[i].Length > j)
                        this.CoreArray[j * rows + i] = vals[i][j];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class as a column vector.
        /// </summary>
        /// <param name="vals">The vals.</param>
        public Matrix(double[] vals)
        {
            this.rowCount = vals.Length;
            this.columnCount = 1;
            this.CoreArray = (double[])vals.Clone();
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
                //MatrixException.ThrowIf(row >= this.RowCount || column >= this.ColumnCount,
                //                        "Invalid column or row specified");

                return this.CoreArray[column * this.rowCount + row];
            }

            set
            {
                //MatrixException.ThrowIf(row >= this.RowCount || column >= this.ColumnCount,
                //                       "Invalid column or row specified");

                this.CoreArray[column * this.rowCount + row] = value;
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
                        res.CoreArray[j * res.rowCount + i] +=
                            m1.CoreArray[k * m1.rowCount + i] *
                            m2.CoreArray[j * m2.rowCount + k];
                    }


            return res;
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

        public void SetRow(int i, params double[] values)
        {
            if (values.Count() != this.ColumnCount)
                throw new ArgumentOutOfRangeException("values");

            for (int j = 0; j < this.ColumnCount; j++)
            {
                this.CoreArray[j * this.RowCount + i] = values[j];
            }
        }

        public void SetColumn(int j, params double[] values)
        {
            if (values.Count() != this.RowCount)
                throw new ArgumentOutOfRangeException("values");


            for (int i = 0; i < this.RowCount; i++)
            {
                this.CoreArray[j * this.RowCount + i] = values[i];
            }
        }

        /// <summary>
        /// Provides a shallow copy of this matrix in O(row).
        /// </summary>
        /// <returns></returns>
        public Matrix Clone()
        {
            var buf = new Matrix(this.RowCount, this.ColumnCount);

            buf.CoreArray = (double[])this.CoreArray.Clone();
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
            return Equals((Matrix)obj);
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

        public bool IsSymmetric()
        {
            for (int i = 0; i < this.rowCount; i++)
                for (int j = i+1; j < this.columnCount; j++)
                    if (!MathUtil.Equals(this[i, j], this[j, i], 1e-3))
                    {
                        return false;
                    }

            return true;
        }

        /// <summary>
        /// Checks if A[i, j] == 0 for i < j.
        /// </summary>
        /// <returns>True iff matrix is upper trapeze.</returns>
        public bool IsUpperTrapeze()
        {
            for (int j = 1; j <= columnCount; j++)
                for (int i = j + 1; i <= rowCount; i++)
                    if (!MathUtil.Equals(this.CoreArray[j * this.RowCount + i], 0))
                        return false;

            return true;
        }

        /// <summary>
        /// Checks if A[i, j] == 0 for i > j.
        /// </summary>
        /// <returns>True iff matrix is lower trapeze.</returns>
        public bool IsLowerTrapeze()
        {
            for (int i = 1; i <= rowCount; i++)
                for (int j = i + 1; j <= columnCount; j++)
                    if (!MathUtil.Equals(this.CoreArray[j * this.RowCount + i], 0))
                        return false;

            return true;
        }

        /// <summary>
        /// Checks if matrix is lower or upper trapeze.
        /// </summary>
        /// <returns>True iff matrix is trapeze.</returns>
        public bool IsTrapeze()
        {
            return (this.IsUpperTrapeze() || this.IsLowerTrapeze());
        }

        /// <summary>
        /// Checks if matrix is trapeze and square.
        /// </summary>
        /// <returns>True iff matrix is triangular.</returns>
        public bool IsTriangular()
        {
            return (this.IsLowerTriangular() || this.IsUpperTriangular());
        }

        /// <summary>
        /// Checks if matrix is square and upper trapeze.
        /// </summary>
        /// <returns>True iff matrix is upper triangular.</returns>
        public bool IsUpperTriangular()
        {
            return (this.IsSquare() && this.IsUpperTrapeze());
        }

        /// <summary>
        /// Checks if matrix is square and lower trapeze.
        /// </summary>
        /// <returns>True iff matrix is lower triangular.</returns>
        public bool IsLowerTriangular()
        {
            return (this.IsSquare() && this.IsLowerTrapeze());
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
        /// Swaps columns at specified indices. The latter do not have to be ordered.
        /// When equal, nothing is done.
        /// </summary>
        /// <param name="j1">One-based index of first col.</param>
        /// <param name="j2">One-based index of second col.</param>       
        public void SwapColumns(int j1, int j2)
        {
            if (j1 <= 0 || j1 > columnCount || j2 <= 0 || j2 > columnCount)
                throw new ArgumentException("Indices must be positive and <= number of cols.");

            if (j1 == j2)
                return;

            var j1Col = this.ExtractColumn(j1).CoreArray;
            var j2Col = this.ExtractColumn(j2).CoreArray;

            this.SetRow(j1, j2Col);
            this.SetRow(j2, j1Col);
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
                mtx.CoreArray[j] = this.CoreArray[j * this.RowCount + i];
            }

            return mtx;
        }

        /// <summary>
        /// Retrieves column vector at specfifed index and deletes it from matrix.
        /// </summary>
        /// <param name="j">One-based index at which to extract.</param>
        /// <returns>Row vector.</returns>
        public Matrix ExtractColumn(int j)
        {
            if (j >= this.ColumnCount || j < 0)
                throw new ArgumentOutOfRangeException("j");

            var mtx = new Matrix(this.RowCount, 1);


            for (int i = 0; i < this.RowCount; i++)
            {
                mtx.CoreArray[i] = this.CoreArray[j * this.RowCount + i];
            }

            return mtx;
        }

        /// <summary>
        /// Gets the determinant of matrix
        /// </summary>
        /// <returns></returns>
        public double Determinant()
        {
            //Seems working good!

            if (!IsSquare())
                throw new InvalidOperationException();

            var clone = this.Clone();

            var n = this.rowCount;

            var sign = 1.0;

            var epsi1on = 1e-10 * clone.CoreArray.Select(System.Math.Abs).Min();

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
                    var alfa = (clone.CoreArray[j * n + i] / clone.CoreArray[i * n + i]);

                    for (var k = i; k < n; k++)
                    {
                        clone.CoreArray[j * n + k] -= alfa * clone.CoreArray[i * n + k];
                    }
                }
            }

            var buf = sign;

            var arr = new double[n];

            for (var i = 0; i < n; i++)
                arr[i] = clone.CoreArray[i * n + i];

            Array.Sort(arr);

            for (var i = 0; i < n; i++)
                buf = buf * arr[n - i - 1];

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

            var epsi1on = 1e-10 * clone.CoreArray.Select(System.Math.Abs).Min();

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

                    var alfa = clone[i, j] / clone[j, j];

                    for (var k = 0; k < n; k++)
                    {
                        clone[i, k] -= alfa * clone[j, k];
                        eye[i, k] -= alfa * eye[j, k];
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

                    var alfa = clone[i, j] / clone[j, j];

                    for (var k = n - 1; k >= 0; k--)
                    {
                        clone[i, k] -= alfa * clone[j, k];
                        eye[i, k] -= alfa * eye[j, k];
                    }
                }
            }

            /**/

            for (var i = 0; i < n; i++)
            {
                var alfa = 1 / clone[i, i];

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

            var epsi1on = 1e-10 * clone.CoreArray.Select(System.Math.Abs).Min();

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
                    if (System.Math.Abs(clonea[j + j * n]) < epsi1on)
                    {
                        var firstNonZero = -1;

                        for (var k = j + 1; k < n; k++)
                            if (System.Math.Abs(clonea[k + j * n]) > epsi1on)
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

                    var alfa = clonea[i + j * n] / clonea[j + j * n];

                    for (var k = 0; k < n; k++)
                    {
                        clonea[i + k * n] -= alfa * clonea[j + k * n];
                        eyea[i + k * n] -= alfa * eyea[j + k * n];
                    }
                }
            }

            /**/

            for (var j = n - 1; j > 0; j--)
            {
                for (var i = j - 1; i >= 0; i--)
                {
                    if (System.Math.Abs(clonea[j + j * n]) < epsi1on)
                    {
                        var firstNonZero = -1;

                        for (var k = j - 1; k >= 0; k--)
                            if (System.Math.Abs(clonea[k + j * n]) > epsi1on)
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

                    var alfa = clonea[i + j * n] / clonea[j + j * n];

                    for (var k = n - 1; k >= 0; k--)
                    {
                        clonea[i + k * n] -= alfa * clonea[j + k * n];
                        eyea[i + k * n] -= alfa * eyea[j + k * n];
                    }
                }
            }

            /**/

            for (var i = 0; i < n; i++)
            {
                var alfa = 1 / clonea[i + i * n];

                for (var j = 0; j < n; j++)
                {
                    clonea[i + j * n] *= alfa;
                    eyea[i + j * n] *= alfa;
                }
            }

            /**/

            return eye;
        }

        #endregion

        #region Static Methods

        public static double[,] To2DDoubleArray(Matrix mtx)
        {
            var buf = new double[mtx.RowCount, mtx.ColumnCount];

            for (int i = 0; i < mtx.RowCount; i++)
                for (int j = 0; j < mtx.ColumnCount; j++)
                    buf[i, j] = mtx.CoreArray[j * mtx.RowCount + i];

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
                buf.CoreArray[i * n + i] = 1.0;

            return buf;
        }

        /// <summary>
        /// Creates row by n matrix filled with zeros.
        /// </summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        /// <returns>row by n matrix filled with zeros.</returns>
        public static Matrix Zeros(int m, int n)
        {
            return new Matrix(m, n);
        }

        /// <summary>
        /// Creates n by n matrix filled with zeros.
        /// </summary>       
        /// <param name="n">Number of rows and columns, resp.</param>
        /// <returns>n by n matrix filled with zeros.</returns>
        public static Matrix Zeros(int n)
        {
            return new Matrix(n);
        }

        /// <summary>
        /// Creates row by n matrix filled with ones.
        /// </summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        /// <returns>row by n matrix filled with zeros.</returns>
        public static Matrix Ones(int m, int n)
        {
            var buf = new Matrix(m, n);

            for (int i = 0; i < m * n; i++)
                buf.CoreArray[i] = 1.0;

            return buf;
        }

        /// <summary>
        /// Creates n by n matrix filled with ones.
        /// </summary>        
        /// <param name="n">Number of columns.</param>
        /// <returns>n by n matrix filled with ones.</returns>        
        public static Matrix Ones(int n)
        {
            var buf = new Matrix(n);

            for (int i = 0; i < n * n; i++)
                buf.CoreArray[i] = 1.0;

            return buf;
        }

        /// <summary>
        /// Computes product of main diagonal entries.
        /// </summary>
        /// <returns>Product of diagonal elements</returns>
        public double DiagProd()
        {
            var buf = 1.0;
            int dim = System.Math.Min(this.rowCount, this.columnCount);

            for (int i = 0; i < dim; i++)
            {
                buf *= this.CoreArray[i * this.RowCount + i];
            }

            return buf;
        }

        /// <summary>
        /// Checks if matrix is n by one or one by n.
        /// </summary>
        /// <returns>Length, if vector; zero else.</returns>
        public int VectorLength()
        {
            if (columnCount > 1 && rowCount > 1)
                return 0;

            return System.Math.Max(columnCount, rowCount);
        }

        /// <summary>
        /// Generates diagonal matrix
        /// </summary>
        /// <param name="diag_vector">column vector containing the diag elements</param>
        /// <returns></returns>
        public static Matrix Diag(Matrix diag_vector)
        {
            int dim = diag_vector.VectorLength();

            if (dim == 0)
                throw new ArgumentException("diag_vector must be 1xN or Nx1");

            var M = new Matrix(dim, dim);

            for (int i = 0; i < dim; i++)
                M.CoreArray[i * dim + i] = diag_vector.CoreArray[i];

            return M;
        }

        /// <summary>
        /// Creates n by n identity matrix.
        /// </summary>
        /// <param name="n">Number of rows and columns respectively.</param>
        /// <returns>n by n identity matrix.</returns>
        public static Matrix Identity(int n)
        {
            return Eye(n);
        }

        #region Operators

        public static Matrix operator *(
            Matrix m1, Matrix m2)
        {
            return Matrix.Multiply(m1, m2);
        }

        public static Matrix operator *(
            double coeff, Matrix mat)
        {
            var newMat = new double[mat.RowCount * mat.ColumnCount];


            for (int i = 0; i < newMat.Length; i++)
            {
                newMat[i] = coeff * mat.CoreArray[i];
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

        public override string ToString()
        {
            var sb = new StringBuilder();

            var mtx = this;

            var epsi1on = mtx.CoreArray.Select(i => System.Math.Abs(i)).Min() * 1e-9;

            if (epsi1on == 0)
                epsi1on = 1e-9;

            for (var i = 0; i < mtx.RowCount; i++)
            {
                for (var j = 0; j < mtx.ColumnCount; j++)
                {
                    if (System.Math.Abs(mtx[i, j]) < epsi1on)
                        sb.AppendFormat("0\t", mtx[i, j]);
                    else
                        sb.AppendFormat("{0:0.00}\t", mtx[i, j]);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Fills the matrix in more straight way!
        /// it is assumed that vals are columns and mappind 2d array to 1d is by placing matrix columns under each other
        /// </summary>
        /// <param name="vals"></param>
        public void FillWith(params double[] vals)
        {
            Array.Copy(vals, this.CoreArray, System.Math.Max(vals.Length, this.CoreArray.Length));
        }

        public void RemoveRow(int m)
        {
            var rc = rowCount - 1;

            var arr2 = new double[(rowCount - 1) * columnCount];

            var ext = 0;

            for (var i = 0; i < rowCount; i++)
            {
                if (i == m)
                {
                    ext = -1;
                }
                else
                    for (var j = 0; j < columnCount; j++)
                    {
                        arr2[i + ext + j * rc] = coreArray[i + j * rowCount];
                    }


            }

            this.coreArray = arr2;
            this.rowCount--;

        }

        public void RemoveColumn(int n)
        {
            var l = this.rowCount * (this.columnCount - 1);

            for (var k = n * this.rowCount; k < l; k++)
            {
                this.coreArray[k] = this.coreArray[k + rowCount];
            }

            Array.Resize(ref coreArray, l);
            this.columnCount--;
        }

        public void SetRowToValue(int m, double val)
        {
            for (int i = 0; i < columnCount; i++)
            {
                this[m, i] = val;
            }
        }

        public void SetColumnToValue(int n, double val)
        {
            for (int j = 0; j < rowCount; j++)
            {
                this[j, n] = val;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("rowCount", rowCount);
            info.AddValue("columnCount", columnCount);
            info.AddValue("CoreArray", CoreArray);
        }

        protected Matrix(
           SerializationInfo info,
           StreamingContext context)
        {
            this.rowCount = (int)info.GetValue("rowCount", typeof(int));
            this.columnCount = (int)info.GetValue("columnCount", typeof(int));
            this.CoreArray = (double[])info.GetValue("CoreArray", typeof(double[]));
        }


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
            return (IEnumerator<double>)new List<double>(coreArray).GetEnumerator();
        }
    }

    public class MatrixException : Exception
    {
        public MatrixException()
        {
        }

        public MatrixException(string message)
            : base(message)
        {
        }

        public MatrixException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MatrixException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        [DebuggerHidden]
        public static void ThrowIf(bool condition, string message)
        {
            if (condition)
                throw new MatrixException(message);
        }
    }
}
