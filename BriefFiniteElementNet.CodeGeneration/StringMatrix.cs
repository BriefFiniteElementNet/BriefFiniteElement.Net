using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.CodeGeneration
{
    public class StringMatrix
    {
        public string this[int i, int j]
        {
            get { return Core[i, j]; }
            set { Core[i, j] = value; }
        }

        public string this[int i]
        {
            get
            {
                if (this.RowCount != 1)
                    throw new Exception();

                return Core[0, i];
            }
            set
            {
                if (this.RowCount != 1)
                    throw new Exception();

                Core[0,i] = value;
            }
        }

        public StringMatrix(int rows,int cols)
        {
            Core = new string[rows, cols];
        }

        public string[,] Core;

        public int RowCount
        {
            get { return Core.GetLength(0); }
        }

        public int ColCount
        {
            get { return Core.GetLength(1); }
        }

        public string AsString
        {
            get { return this.ToString(); }
        }

        public static StringMatrix operator *(StringMatrix a, StringMatrix b)
        {
            var sb = new StringBuilder();

            var buf = new StringMatrix(a.RowCount, b.ColCount);

            for (int i = 0; i < buf.RowCount; i++)
            {
                for (int j = 0; j < buf.ColCount; j++)
                {
                    //c[i, j] = 0;
                    for (int k = 0; k < a.ColCount; k++) // OR k<b.GetLength(0)
                    {
                        if (!string.IsNullOrEmpty(a[i, k]) && !string.IsNullOrEmpty(b[k, j]))
                            buf[i, j] = buf[i, j] + "+" + a[i, k] + "*" + b[k, j];
                    }
                    if (buf[i, j] != null)
                        buf[i, j] = buf[i, j].TrimStart('+');
                }
            }


            return buf;
        }

        public static StringMatrix operator -(StringMatrix a)
        {
            var sb = new StringBuilder();

            var buf = new StringMatrix(a.RowCount, a.ColCount);

            for (int i = 0; i < buf.RowCount; i++)
            {
                for (int j = 0; j < buf.ColCount; j++)
                {
                    if (!string.IsNullOrEmpty(a[i, j]))
                        buf[i, j] = string.Format("-({0})", a[i, j]);
                }
            }


            return buf;
        }

        public static StringMatrix operator *(string a, StringMatrix b)
        {
            var sb = new StringBuilder();

            var buf = new StringMatrix(b.RowCount, b.ColCount);

            for (var i = 0; i < buf.RowCount; i++)
            {
                for (var j = 0; j < buf.ColCount; j++)
                {
                    if (!string.IsNullOrEmpty(b[i, j]))
                        buf[i, j] = string.Format("({0})*({1})", a, b[i, j]);
                }
            }


            return buf;
        }

        public string GetFillCode(string matrixNAme)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < this.RowCount; i++)
            {
                for (int j = 0; j < this.ColCount; j++)
                {
                    if (!string.IsNullOrEmpty(this[i, j]))
                    {
                        sb.AppendFormat("{0}[{1:00},{2:00}]={3};{4}", matrixNAme, i, j, this[i, j], Environment.NewLine);
                    }
                }
            }

            return sb.ToString().Replace("--", "+");
        }

        public static StringMatrix MatrixFromStringArray(string[] arr)
        {
            var dim = (int) Math.Sqrt(arr.Length);

            if (dim*dim != arr.Length)
                throw new Exception();

            var buf = new StringMatrix(dim, dim);

            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    buf[i, j] = arr[i + j * dim];
                }
            }



            return buf;
        }

        public static StringMatrix VectorFromStringArray(params string[] arr)
        {
            var buf = new StringMatrix(1, arr.Length);

            for (var i = 0; i < arr.Length; i++)
                buf[0, i] = arr[i];

            return buf;
        }

        public static StringMatrix Diag(params string[] arr)
        {
            var buf = new StringMatrix(arr.Length, arr.Length);

            for (var i = 0; i < arr.Length; i++)
                buf[i, i] = arr[i];

            return buf;
        }

        public StringMatrix Transpose()
        {
            var buf = new StringMatrix(this.ColCount, this.RowCount);

            for (int i = 0; i < this.RowCount; i++)
            {
                for (int j = 0; j < ColCount; j++)
                {
                    buf[j, i] = this[i, j];
                }
            }

            return buf;
        }

        public static StringMatrix HorzCat(StringMatrix m1, StringMatrix m2)
        {
            if (m1.RowCount != m2.RowCount)
                throw new NotImplementedException();

            var buf = new StringMatrix(m1.RowCount, m1.ColCount + m2.ColCount);

            for (int i = 0; i < m1.RowCount; i++)
            {
                for (int j = 0; j < m1.ColCount; j++)
                {
                    buf[i, j] = m1[i, j];
                }
            }

            for (int i = 0; i < m2.RowCount; i++)
            {
                for (int j = 0; j < m2.ColCount; j++)
                {
                    buf[i, j + m1.ColCount] = m2[i, j];
                }
            }

            return buf;
        }


        public static StringMatrix VertCat(StringMatrix m1, StringMatrix m2)
        {
            if (m1.ColCount != m2.ColCount)
                throw new NotImplementedException();

            var buf = new StringMatrix(m1.RowCount + m2.RowCount, m1.ColCount);

            for (int i = 0; i < m1.RowCount; i++)
            {
                for (int j = 0; j < m1.ColCount; j++)
                {
                    buf[i, j] = m1[i, j];
                }
            }

            for (int i = 0; i < m2.RowCount; i++)
            {
                for (int j = 0; j < m2.ColCount; j++)
                {
                    buf[i+m1.RowCount, j ] = m2[i, j];
                }
            }

            return buf;
        }


        public void FillLowerTriangleFromUpperTriangle()
        {
            var mtx = this;

            var c = mtx.RowCount;

            for (var i = 0; i < c; i++)
                for (var j = 0; j < i; j++)
                    mtx[i, j] = mtx[j, i];
        }



        public override string ToString()
        {
            var sb = new StringBuilder();
            var maxL = 0;

            for (int i = 0; i < this.RowCount; i++)
                for (int j = 0; j < this.ColCount; j++)
                    maxL = Math.Max(maxL, this[i, j] == null ? 0 : this[i, j].Length);


            for (int i = 0; i < this.RowCount; i++)
            {
                for (int j = 0; j < this.ColCount; j++)
                {
                    sb.AppendFormat(",{0}\t", MakeItLengty(this[i, j], maxL));
                }
                sb.AppendLine();
                sb.AppendLine();
            }

            return sb.ToString();
        }


        private string MakeItLengty(string st, int l)
        {
            if (st == null)
                st = "";


            var l2 = l-st.Length ;

            return st + new string(' ', l2);
        }


        public static StringMatrix Join(StringMatrix[,] mtxs)
        {
            var rows = mtxs.GetLength(0);
            var cols = mtxs.GetLength(1);

            StringMatrix buf = null;

            for (var i = 0; i < rows; i++)
            {
                var iThRow = mtxs[i, 0];

                for (var j = 1; j < cols; j++)
                {
                    iThRow = HorzCat(iThRow, mtxs[i, j]);
                }

                if (buf == null)
                    buf = iThRow;
                else
                    buf = VertCat(buf, iThRow);
            }

            return buf;
        }
    }
}
