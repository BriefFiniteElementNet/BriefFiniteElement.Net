
namespace CSparse.IO
{
    using System;
    using System.Globalization;
    using System.IO;
    //using System.Numerics;
    using CSparse.Storage;

    /// <summary>
    /// Structure of a matrix (i.e. symmetric etc.).
    /// </summary>
    public enum MatrixStructure
    {
        /// <summary>
        /// The matrix does not have a specific structure.
        /// </summary>
        General,
        /// <summary>
        /// The matrix is symmetric.
        /// </summary>
        Symmetric,
        /// <summary>
        /// The matrix is skew-symmetric.
        /// </summary>
        SkewSymmetric,
        /// <summary>
        /// The complex matrix is hermitian.
        /// </summary>
        Hermitian,
        /// <summary>
        /// The matrix is a diagonal matrix. 
        /// </summary>
        Diagonal,
        /// <summary>
        /// The matrix is lower triangular.
        /// </summary>
        LowerTriangular,
        /// <summary>
        /// The matrix is upper triangular.
        /// </summary>
        UpperTriangular
    }

    /// <summary>
    /// Read files in Matrix Market format.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MatrixMarketReader<T> where T : struct, IEquatable<T>, IFormattable
    {
        private static readonly char[] seperator = new char[] { ' ', '\t' };

        /// <summary>
        /// The name of the file to read from.
        /// </summary>
        private readonly string filename;

        int rowCount, columnCount, nonZerosCount, offset;

        MatrixStructure matrixStructure;

        public bool IsSymmetric()
        {
            return matrixStructure == MatrixStructure.Symmetric ||
                matrixStructure == MatrixStructure.Hermitian;
        }

        /// <summary>
        /// Check whether the MatrixMarket file contains complex data.
        /// </summary>
        /// <param name="filename">Name of the file to read from.</param>
        /// <returns></returns>
        /// <remarks>
        /// Since this method only reads the MatrixMarket header, it doesn't
        /// matter if you call "MatrixMarketReader&lt;T>.IsComplex(...)" with
        /// datatype T = double or T = Complex.
        /// </remarks>
        public static bool IsComplex(string filename)
        {
            using (var stream = new StreamReader(filename))
            {
                string line = stream.ReadLine();

                if (!line.StartsWith("%%"))
                {
                    throw new FormatException("Expected MatrixMarket header.");
                }

                string[] token = line.Split(seperator, StringSplitOptions.RemoveEmptyEntries);

                if (token.Length != 5)
                {
                    throw new FormatException("Error in MatrixMarket header.");
                }

                return token[3].ToLowerInvariant() == "complex";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixMarketReader{T}"/> class.
        /// </summary>
        /// <param name="filename">Name of the file to read from.</param>
        public MatrixMarketReader(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException(); // TODO: Resources.StringNullOrEmpty, "filename");
            }

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(); // TODO: Resources.FileDoesNotExist, "filename");
            }

            this.filename = filename;

            this.rowCount = -1;
            this.offset = 1;
            this.matrixStructure = MatrixStructure.General;
        }

        /// <summary>
        /// Read matrix from file.
        /// </summary>
        public CoordinateStorage<T> ReadMatrix()
        {
            var nfi = CultureInfo.InvariantCulture.NumberFormat;

            using (var reader = new StreamReader(filename))
            {
                ReadHeader(reader);

                if (typeof(T) == typeof(double))
                {
                    return ReadMatrixDouble(reader, nfi) as CoordinateStorage<T>;
                }
                else if (typeof(T) == typeof(Complex))
                {
                    return ReadMatrixComplex(reader, nfi) as CoordinateStorage<T>;
                }
            }

            return null;
        }

        private void ReadHeader(StreamReader reader)
        {
            string line;

            if (!GetNextLine(reader, out line))
            {
                throw new FormatException();
            }

            if (line.StartsWith("%%"))
            {
                ReadMatrixMarketBanner(line.Substring(2));

                // Used for header detection in GetNextLine()
                this.rowCount = 0;
            }
            else
            {
                throw new FormatException("Missing header");
            }

            // Skip comment
            while (GetNextLine(reader, out line))
            {
                if (!line.Trim().StartsWith("%"))
                {
                    break;
                }
            }

            string[] vals = line.Split(seperator, StringSplitOptions.RemoveEmptyEntries);

            if (vals.Length != 3)
            {
                throw new FormatException();
            }

            rowCount = int.Parse(vals[0].TrimStart('%'));
            columnCount = int.Parse(vals[1]);
            nonZerosCount = int.Parse(vals[2]);
        }

        private void ReadMatrixMarketBanner(string line)
        {
            // line = MatrixMarket matrix coordinate real general

            string[] token = line.Split(seperator, StringSplitOptions.RemoveEmptyEntries);

            if (token.Length != 5)
            {
                throw new FormatException();
            }

            if (token[0] != "MatrixMarket")
            {
                throw new FormatException(); // Expected MatrixMarket format
            }

            if (token[1] != "matrix")
            {
                throw new FormatException(); // Expected matrix content
            }

            if (token[2] != "coordinate")
            {
                throw new FormatException(); // Expected matrix to be in coordinate format
            }

            // Type may be: real | complex | integer | pattern
            var type = token[3].ToLowerInvariant();
            var expectedType = typeof(T);

            if (expectedType == typeof(double) && type != "real")
            {
                throw new FormatException(); // Expected matrix to have real entries
            }
            else if (expectedType == typeof(Complex) && type != "complex")
            {
                throw new FormatException(); // Expected matrix to have complex entries
            }

            string structure = token[4].ToLowerInvariant();

            if (structure == "symmetric")
            {
                this.matrixStructure = MatrixStructure.Symmetric;
            }
            else if (structure == "skew-symmetric")
            {
                this.matrixStructure = MatrixStructure.SkewSymmetric;
            }
            else if (structure == "hermitian")
            {
                this.matrixStructure = MatrixStructure.Hermitian;
            }
        }

        private CoordinateStorage<double> ReadMatrixDouble(StreamReader reader, NumberFormatInfo nfi)
        {
            var storage = new CoordinateStorage<double>(rowCount, columnCount, nonZerosCount);

            int i, j, count = 0;
            double value;

            string line;
            string[] values;

            while (GetNextLine(reader, out line))
            {
                values = line.Split(seperator, StringSplitOptions.RemoveEmptyEntries);

                if (values.Length != 3)
                {
                    throw new FormatException(); // TODO: Exception text
                }

                i = int.Parse(values[0]) - offset;
                j = int.Parse(values[1]) - offset;

                //FloatingPoint.TryParse(values[2], out value);
                value = double.Parse(values[2], nfi);

                storage.At(i, j, value);

                if (matrixStructure == MatrixStructure.Symmetric && i != j)
                {
                    // Fill in upper part (symmetric MM format only stores lower).
                    storage.At(j, i, value);
                }

                count++;
            }

            return storage;
        }

        private CoordinateStorage<Complex> ReadMatrixComplex(StreamReader reader, NumberFormatInfo nfi)
        {
            var storage = new CoordinateStorage<Complex>(rowCount, columnCount, nonZerosCount);

            int i, j, count = 0;
            double valRe, valIm;

            string line;
            string[] vals;

            while (GetNextLine(reader, out line))
            {
                vals = line.Split(seperator, StringSplitOptions.RemoveEmptyEntries);

                if (vals.Length != 4)
                {
                    throw new FormatException(); // TODO: Exception text
                }

                i = int.Parse(vals[0]) - offset;
                j = int.Parse(vals[1]) - offset;

                valRe = double.Parse(vals[2], nfi);
                valIm = double.Parse(vals[3], nfi);

                storage.At(i, j, new Complex(valRe, valIm));

                count++;
            }

            return storage;
        }

        private bool GetNextLine(StreamReader reader, out string line)
        {
            line = null;

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine().Trim();

                if (string.IsNullOrEmpty(line) || line.Length < 2)
                {
                    continue;
                }

                if (line[0] != '%')
                {
                    return true;
                }

                if (this.rowCount < 0 && line[1] == '%')
                {
                    // Expect MatrixMarket format header
                    return true;
                }

                if (this.rowCount <= 0)
                {
                    // Expect matrix dimensions header
                    return true;
                }
            }

            return false;
        }
    }
}
