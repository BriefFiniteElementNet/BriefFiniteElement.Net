
using System;

namespace BriefFiniteElementNet.CSparse.Storage
{
    /// <summary>
    /// Converter for different types of storages.
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Convert a coordinate storage to compressed sparse column (CSC) format.
        /// </summary>
        /// <param name="storage">Coordinate storage.</param>
        /// <param name="cleanup">Remove and sum duplicate entries.</param>
        /// <returns>Compressed sparse column storage.</returns>
        public static CompressedColumnStorage<T> ToCompressedColumnStorage<T>(CoordinateStorage<T> storage,
            bool cleanup = true) where T : struct, IEquatable<T>, IFormattable
        {
            int nrows = storage.RowCount;
            int ncols = storage.ColumnCount;

            var values = storage.Values;
            var rowind = storage.RowIndices;
            var colind = storage.ColumnIndices;

            int p, k, nz = storage.NonZerosCount;

            var columnPointers = new int[ncols + 1];
            var columnCounts = new int[ncols];

            for (k = 0; k < nz; k++)
            {
                // Count columns
                columnCounts[colind[k]]++;
            }

            // Get row pointers
            int valueCount = Helper.CumulativeSum(columnPointers, columnCounts, ncols);

            var result = CompressedColumnStorage<T>.Create(nrows, ncols);

            var rowIndices = new int[valueCount];
            var storageValues = new T[valueCount];

            for (k = 0; k < nz; k++)
            {
                p = columnCounts[colind[k]]++;
                rowIndices[p] = rowind[k];
                storageValues[p] = values[k];
            }

            result.RowIndices = rowIndices;
            result.ColumnPointers = columnPointers;
            result.Values = storageValues;

            result.SortIndices();

            if (cleanup)
            {
                result.Cleanup();
            }

            return result;
        }
                
        /// <summary>
        /// Convert a 2D array to coordinate storage.
        /// </summary>
        /// <param name="array">2D array storage.</param>
        /// <returns>Coordinate storage.</returns>
        public static CoordinateStorage<T> FromDenseArray<T>(T[,] array)
            where T : struct, IEquatable<T>, IFormattable
        {
            int rowCount = array.GetLength(0);
            int columnCount = array.GetLength(1);

            var storage = new CoordinateStorage<T>(rowCount, columnCount, Math.Max(rowCount, columnCount));

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    storage.At(i, j, array[i, j]);
                }
            }

            return storage;
        }

        /// <summary>
        /// Convert a column major array to coordinate storage.
        /// </summary>
        /// <param name="array">Column major array storage.</param>
        /// <param name="rowCount">Number of rows.</param>
        /// <param name="columnCount">Number of columns.</param>
        /// <returns>Coordinate storage.</returns>
        public static CoordinateStorage<T> FromColumnMajorArray<T>(T[] array, int rowCount, int columnCount)
            where T : struct, IEquatable<T>, IFormattable
        {
            var storage = new CoordinateStorage<T>(rowCount, columnCount, Math.Max(rowCount, columnCount));

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    storage.At(i, j, array[i + j * rowCount]);
                }
            }

            return storage;
        }

        /// <summary>
        /// Convert a row major array to coordinate storage.
        /// </summary>
        /// <param name="array">Row major array storage.</param>
        /// <param name="rowCount">Number of rows.</param>
        /// <param name="columnCount">Number of columns.</param>
        /// <returns>Coordinate storage.</returns>
        public static CoordinateStorage<T> FromRowMajorArray<T>(T[] array, int rowCount, int columnCount)
            where T : struct, IEquatable<T>, IFormattable
        {
            var storage = new CoordinateStorage<T>(rowCount, columnCount, Math.Max(rowCount, columnCount));

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    storage.At(i, j, array[i * columnCount + j]);
                }
            }

            return storage;
        }

        /// <summary>
        /// Transpose operation.
        /// </summary>
        /// <remarks>
        /// We use a separate transpose method here, because for complex storage, transpose would include
        /// complex conjugation, which we don't want here!
        /// </remarks>
        internal static void Transpose<T>(int m, int n, int[] ap, int[] ai, T[] ax, int[] cp, int[] ci, T[] cx)
            where T : struct, IEquatable<T>, IFormattable
        {
            int[] w = new int[m];

            int i, j, p;

            for (p = 0; p < ap[n]; p++)
            {
                w[ai[p]]++;
            }

            Helper.CumulativeSum(cp, w, m);

            for (i = 0; i < n; i++)
            {
                for (p = ap[i]; p < ap[i + 1]; p++)
                {
                    j = w[ai[p]]++;

                    ci[j] = i;
                    cx[j] = ax[p];
                }
            }
        }
    }
}
