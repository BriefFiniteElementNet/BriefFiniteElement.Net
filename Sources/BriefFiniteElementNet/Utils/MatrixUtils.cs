using System;
using System.Collections.Generic;
using System.Linq;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Mathh;
using BriefFiniteElementNet.Solver;
using CSparse.Double;
using CSparse.Storage;
using System.Globalization;
using CSparse.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Utils
{
    public static class MatrixUtils
    {

        // TODO: EXTENSION - move to Extensions class?
        public static int EmptyRowCount(this SparseMatrix matrix)
        {
            var buf = new bool[matrix.RowCount];

            matrix.EnumerateIndexed((row, col, val) =>
            {
                buf[row] = true;
            });

            return buf.Count(ii => !ii);
        }


        /// <summary>
        /// Gets the reduced zone divided matrix.
        /// </summary>
        /// <param name="reducedMatrix">The reduced matrix.</param>
        /// <param name="map">The map.</param>
        /// <returns></returns>
        public static ZoneDevidedMatrix GetReducedZoneDividedMatrix(SparseMatrix reducedMatrix, DofMappingManager map)
        {
            var m = map.M;
            var n = map.N;
            var r = reducedMatrix;

            if (r.ColumnCount != r.RowCount || r.RowCount != 6 * m)
                throw new InvalidOperationException();

            var ff = new CoordinateStorage<double>(map.RMap2.Length, map.RMap2.Length, 1);
            var fs = new CoordinateStorage<double>(map.RMap2.Length, map.RMap3.Length, 1);
            var sf = new CoordinateStorage<double>(map.RMap3.Length, map.RMap2.Length, 1);
            var ss = new CoordinateStorage<double>(map.RMap3.Length, map.RMap3.Length, 1);

            for (var i = 0; i < 6 * m; i++)
            {
                var st = r.ColumnPointers[i];
                var en = r.ColumnPointers[i + 1];

                var col = i;

                for (var j = st; j < en; j++)
                {
                    var row = r.RowIndices[j];
                    var val = r.Values[j];

                    if (map.Fixity[map.RMap1[row]] == DofConstraint.Released &&
                        map.Fixity[map.RMap1[col]] == DofConstraint.Released)
                        ff.At(map.Map2[row], map.Map2[col], val);

                    if (map.Fixity[map.RMap1[row]] == DofConstraint.Released &&
                        map.Fixity[map.RMap1[col]] != DofConstraint.Released)
                        fs.At(map.Map2[row], map.Map3[col], val);

                    if (map.Fixity[map.RMap1[row]] != DofConstraint.Released &&
                        map.Fixity[map.RMap1[col]] == DofConstraint.Released)
                        sf.At(map.Map3[row], map.Map2[col], val);

                    if (map.Fixity[map.RMap1[row]] != DofConstraint.Released &&
                        map.Fixity[map.RMap1[col]] != DofConstraint.Released)
                        ss.At(map.Map3[row], map.Map3[col], val);
                }
            }

            var buf = new ZoneDevidedMatrix();

            buf.ReleasedReleasedPart = ff.ToCCs();
            buf.ReleasedFixedPart = fs.ToCCs();
            buf.FixedReleasedPart = sf.ToCCs();
            buf.FixedFixedPart = ss.ToCCs();

            return buf;
        }

        // TODO: EXTENSION - ToCCs could be removed (CSparse should handle this case well)
        public static SparseMatrix ToCCs(this CoordinateStorage<double> crd)
        {
            //https://brieffiniteelementnet.codeplex.com/workitem/6
            if (crd.RowCount == 0 || crd.ColumnCount == 0)
                return new SparseMatrix(0, 0) { ColumnPointers = new int[0], RowIndices = new int[0], Values = new double[0] };

            return (SparseMatrix)SparseMatrix.OfIndexed(crd, true);
        }

        // TODO: EXTENSION - move to Extensions class?
        public static IEnumerable<Tuple<int, double>> EnumerateColumnMembers(this CompressedColumnStorage<double> matrix, int columnNumber)
        {
            var i = columnNumber;

            var ap = matrix.ColumnPointers;
            var ai = matrix.RowIndices;
            var ax = matrix.Values;

            var end = ap[i + 1];

            for (int j = ap[i]; j < end; j++)
            {
                yield return Tuple.Create(ai[j], ax[j]);
            }
        }

        // TODO: EXTENSION - move to Extensions class?
        internal static void EnumerateColumnMembers(this CompressedColumnStorage<double> matrix, int columnNumber, Action<int, int, double> action)
        {
            var i = columnNumber;

            var ap = matrix.ColumnPointers;
            var ai = matrix.RowIndices;
            var ax = matrix.Values;

            var end = ap[i + 1];

            for (int j = ap[i]; j < end; j++)
            {
                action(ai[j], i, ax[j]);
            }
        }

        // TODO: EXTENSION - move to Extensions class?
        internal static int GetNnzcForColumn(this CompressedColumnStorage<double> matrix, int column)
        {
            var st = matrix.ColumnPointers[column];
            var en = matrix.ColumnPointers[column + 1];

            return en - st;
        }


    }
}
