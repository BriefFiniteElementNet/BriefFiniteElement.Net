using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.CSparse.Storage;
using BriefFiniteElementNet.Solver;
using CCS = BriefFiniteElementNet.CSparse.Double.CompressedColumnStorage;
using Coord = BriefFiniteElementNet.CSparse.Storage.CoordinateStorage<double>;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a utility for graphs!
    /// </summary>
    public static class CalcUtil
    {
        /// <summary>
        /// Gets the transformation matrix for converting local coordinate to global coordinate for a two node straight element.
        /// </summary>
        /// <param name="v">The [ end - start ] vector.</param>
        /// <param name="webR">The web rotation in radian.</param>
        /// <returns>
        /// transformation matrix
        /// </returns>
        public static Matrix Get2NodeElementTransformationMatrix(Vector v, double webR)
        {
            var cxx = 0.0;
            var cxy = 0.0;
            var cxz = 0.0;

            var cyx = 0.0;
            var cyy = 0.0;
            var cyz = 0.0;

            var czx = 0.0;
            var czy = 0.0;
            var czz = 0.0;


            var teta = webR;

            var s = webR.Equals(0.0) ? 0.0 : Math.Sin(teta);
            var c = webR.Equals(0.0) ? 1.0 : Math.Cos(teta);

            if (MathUtil.Equals(0, v.X) && MathUtil.Equals(0, v.Y))
            {
                if (v.Z > 0)
                {
                    czx = 1;
                    cyy = 1;
                    cxz = -1;
                }
                else
                {
                    czx = -1;
                    cyy = 1;
                    cxz = 1;
                }
            }
            else
            {
                var l = v.Length;
                cxx = v.X/l;
                cyx = v.Y/l;
                czx = v.Z/l;
                var d = Math.Sqrt(cxx*cxx + cyx*cyx);
                cxy = -cyx/d;
                cyy = cxx/d;
                cxz = -cxx*czx/d;
                cyz = -cyx*czx/d;
                czz = d;
            }

            var t = new Matrix(3, 3);

            t[0, 0] = cxx;
            t[0, 1] = cxy*c + cxz*s;
            t[0, 2] = -cxy*s + cxz*c;

            t[1, 0] = cyx;
            t[1, 1] = cyy*c + cyz*s;
            t[1, 2] = -cyy*s + cyz*c;

            t[2, 0] = czx;
            t[2, 1] = czy*c + czz*s;
            t[2, 2] = -czy*s + czz*c;

            return t;
        }


        /// <summary>
        /// Gets the transformation matrix for converting local coordinate to global coordinate for a two node straight element.
        /// </summary>
        /// <param name="v">The [ end - start ] vector.</param>
        /// <returns>
        /// transformation matrix
        /// </returns>
        public static Matrix Get2NodeElementTransformationMatrix(Vector v)
        {
            return Get2NodeElementTransformationMatrix(v, 0);
        }

        /// <summary>
        /// Creates a built in solver appropriated with <see cref="tp"/>.
        /// </summary>
        /// <param name="type">The solver type.</param>
        /// <returns></returns>
        public static ISolver CreateBuiltInSolver(BuiltInSolverType type)
        {
            switch (type)
            {
                case BuiltInSolverType.CholeskyDecomposition:
                    return new CholeskySolver();
                    break;
                case BuiltInSolverType.ConjugateGradient:
                    return new PCG(new SSOR());
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        public static int GetHashCode(params Point[] objects)
        {
            if (objects == null)
                throw new ArgumentNullException("objects");

            if (objects.Length == 0)
                return 0;

            var buf = objects[0].GetHashCode();

            for (var i = 1; i < objects.Length; i++)
            {
                buf = (buf*397) ^ GetPointHashCode(objects[i]);
            }

            buf = (buf*397) ^ objects.Length;

            return buf;
        }

        public static int GetPointHashCode(Point pt)
        {
            unchecked
            {
                var hashCode = pt.X.GetHashCode();
                hashCode = (hashCode * 397) ^ pt.Y.GetHashCode();
                hashCode = (hashCode * 397) ^ pt.Z.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Enumerates the discrete parts of the graph.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <returns></returns>
        public static List<List<int>> EnumerateGraphParts(CompressedColumnStorage<double> graph)
        {
            var buf = new List<List<int>>();

            var n = graph.ColumnCount;

            var visited = new bool[n];

            var ri = graph.RowIndices;
            var cp = graph.ColumnPointers;

            for (var i = 0; i < n; i++)
            {
                if (cp[i] == cp[i + 1])
                    visited[i] = true;
            }

            while (true)
            {
                var startPoint = visited.FirstIndexOf(false);

                if (startPoint == -1)
                    break;

                var part = DepthFirstSearch(graph, visited, startPoint);

                buf.Add(part);
            }

            return buf;
        }

        /// <summary>
        /// Does the Depth first search, return connected nodes to <see cref="startNode"/>.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="visited">The visited map.</param>
        /// <param name="startNode">The start node.</param>
        /// <returns>List of connected nodes to <see cref="startNode"/>.</returns>
        public static List<int> DepthFirstSearch(CompressedColumnStorage<double> graph, bool[] visited, int startNode)
        {
            var buf = new List<int>();

            var ri = graph.RowIndices;
            var cp = graph.ColumnPointers;

            var q = new Queue<int>();

            q.Enqueue(startNode);

            visited[startNode] = true;

            while (q.Count > 0)
            {
                var v = q.Dequeue();

                visited[v] = true;

                buf.Add(v);

                for (var i = cp[v]; i < cp[v + 1]; i++)
                {
                    var neighbor = ri[i];

                    if (!visited[neighbor])
                    {
                        visited[neighbor] = true;
                        q.Enqueue(neighbor);
                        buf.Add(neighbor);
                    }
                }
            }


            return buf;
        }


        public static int[] GetMasterMapping(Model model, LoadCase cse)
        {
            for (int i = 0; i < model.Nodes.Count; i++)
                model.Nodes[i].Index = i;


            var n = model.Nodes.Count;
            var masters = new int[n];

            for (var i = 0; i < n; i++)
            {
                masters[i] = i;
            }

            var masterCount = 0;

            #region filling the masters

            var distinctElements = GetDistinctRigidElements(model, cse);

            var centralNodePrefreation = new bool[n];


            foreach (var elm in model.RigidElements)
            {
                if (elm.CentralNode != null)
                    centralNodePrefreation[elm.CentralNode.Index] = true;
            }


            foreach (var elm in distinctElements)
            {
                if (elm.Count == 0)
                    continue;

                var elmMasterIndex = GetMasterNodeIndex(model, elm, centralNodePrefreation);

                for (var i = 0; i < elm.Count; i++)
                {
                    masters[elm[i]] = elmMasterIndex;
                }
            }

            #endregion

            return masters;
        }

        private static List<List<int>> GetDistinctRigidElements(Model model, LoadCase loadCase)
        {
            for (int i = 0; i < model.Nodes.Count; i++)
                model.Nodes[i].Index = i;

            var n = model.Nodes.Count;

            var crd = new CoordinateStorage<double>(n, n, 1);

            foreach (var elm in model.RigidElements)
            {
                if (IsAppliableRigidElement(elm, loadCase))
                {
                    for (var i = 0; i < elm.Nodes.Count; i++)
                    {
                        crd.At(elm.Nodes[i].Index, elm.Nodes[i].Index, 1.0);
                    }

                    for (var i = 0; i < elm.Nodes.Count - 1; i++)
                    {
                        crd.At(elm.Nodes[i].Index, elm.Nodes[i + 1].Index, 1.0);
                        crd.At(elm.Nodes[i + 1].Index, elm.Nodes[i].Index, 1.0);
                    }
                }
            }

            var graph = Converter.ToCompressedColumnStorage(crd);

            var buf = CalcUtil.EnumerateGraphParts(graph);

            return buf;
        }

        private static bool IsAppliableRigidElement(RigidElement elm, LoadCase loadCase)
        {
            if (elm.UseForAllLoads)
                return true;

            if (elm.AppliedLoadTypes.Contains(loadCase.LoadType))
                return true;

            if (elm.AppliedLoadCases.Contains(loadCase))
                return true;

            return false;
        }

        private static int GetMasterNodeIndex(Model model, List<int> nodes, bool[] preferation)
        {
            var buf = -1;

            foreach (var node in nodes)
                if (preferation[node])
                {
                    buf = node;
                    break;
                }

            if (buf == -1)
                foreach (var node in nodes)
                    if (model.Nodes[node].Constraints != Constraint.Released)
                    {
                        buf = node;
                        break;
                    }

            foreach (var node in nodes)
                if (model.Nodes[node].Constraints != Constraint.Released)
                    if (node != buf)
                        ExceptionHelper.Throw("MA20000");

            if (buf == -1)
                buf = nodes[0];


            return buf;
        }

        /// <summary>
        /// Fills the whole <see cref="array"/> with -1.
        /// </summary>
        /// <param name="array">The array.</param>
        public static void FillNegative(this int[] array)
        {
            for (var i = array.Length - 1; i >= 0; i--)
            {
                array[i] = -1;
            }
        }

        public static int Sum(this Constraint ctx)
        {
            return (int) ctx.DX + (int) ctx.DY + (int) ctx.DZ +
                   (int) ctx.RX + (int) ctx.RY + (int) ctx.RZ;
        }

        public static DofConstraint[] ToArray(this Constraint ctx)
        {
            return new DofConstraint[] {ctx.DX, ctx.DY, ctx.DZ, ctx.RX, ctx.RY, ctx.RZ};
        }

        public static double[] Add(double[] a, double[] b)
        {
            if (a.Length != b.Length)
                throw new InvalidOperationException();

            var buf = new double[a.Length];

            for (int i = 0; i < b.Length; i++)
            {
                buf[i] = a[i] + b[i];
            }

            return buf;
        }


        public static double[] Subtract(double[] a, double[] b)
        {
            if (a.Length != b.Length)
                throw new InvalidOperationException();

            var buf = new double[a.Length];

            for (int i = 0; i < b.Length; i++)
            {
                buf[i] = a[i] - b[i];
            }

            return buf;
        }

        /// <summary>
        /// Gets the reduced zone divided matrix.
        /// </summary>
        /// <param name="reducedMatrix">The reduced matrix.</param>
        /// <param name="map">The map.</param>
        /// <returns></returns>
        public static ZoneDevidedMatrix GetReducedZoneDividedMatrix(CCS reducedMatrix, DofMappingManager map)
        {
            var m = map.M;
            var n = map.N;
            var r = reducedMatrix;

            if (r.ColumnCount != r.RowCount || r.RowCount != 6*m)
                throw new InvalidOperationException();

            var ff = new Coord(map.RMap2.Length, map.RMap2.Length);
            var fs = new Coord(map.RMap2.Length, map.RMap3.Length);
            var sf = new Coord(map.RMap3.Length, map.RMap2.Length);
            var ss = new Coord(map.RMap3.Length, map.RMap3.Length);

            for (var i = 0; i < 6*m; i++)
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

        public static CCS ToCCs(this Coord crd)
        {
            return (CCS) Converter.ToCompressedColumnStorage(crd);
        }

        /// <summary>
        /// Determines whether defined matrix is diagonal matrix or not.
        /// Diagonal matrix is a matrix that only have nonzero elements on its main diagonal.
        /// </summary>
        /// <param name="mtx">The MTX.</param>
        /// <returns></returns>
        public static bool IsDiagonalMatrix(this CCS mtx)
        {
            var n = mtx.ColumnCount;

            if (n != mtx.RowCount)
                return false;

            if (mtx.Values.Length > n)
                return false;

            for (int i = 0; i < n; i++)
            {
                var col = i;

                var st = mtx.ColumnPointers[i];
                var en = mtx.ColumnPointers[i+1];

                for (int j = st; j < en; j++)
                {
                    var row = mtx.RowIndices[j];

                    if (row != col)
                        return false;
                }
            }


            return true;
        }

        /// <summary>
        /// Applies the release matrix to calculated local end forces.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="localEndForces">The local end forces.</param>
        /// <returns></returns>
        /// <remarks>
        /// When <see cref="FrameElement2Node"/> has one or two hinged ends, then local end forces due to element interior loads (like distributed loads) 
        /// will be different than normal ones (both ends fixed). This method will apply end releases...
        /// </remarks>
        /// <exception cref="System.NotImplementedException"></exception>
        public static Force[] ApplyReleaseMatrixToEndForces(FrameElement2Node element, Force[] localEndForces)
        {
            if (localEndForces.Length != 2)
                throw new NotImplementedException();

            var fullLoadVector = new double[12];//for applying release matrix

            
            {
                fullLoadVector[00] = localEndForces[0].Fx;
                fullLoadVector[01] = localEndForces[0].Fy;
                fullLoadVector[02] = localEndForces[0].Fz;
                fullLoadVector[03] = localEndForces[0].Mx;
                fullLoadVector[04] = localEndForces[0].My;
                fullLoadVector[05] = localEndForces[0].Mz;

                fullLoadVector[06] = localEndForces[1].Fx;
                fullLoadVector[07] = localEndForces[1].Fy;
                fullLoadVector[08] = localEndForces[1].Fz;
                fullLoadVector[09] = localEndForces[1].Mx;
                fullLoadVector[10] = localEndForces[1].My;
                fullLoadVector[11] = localEndForces[1].Mz;
            }

            var ld = new Matrix(fullLoadVector);
            var rsm = element.GetReleaseMatrix();
            ld = rsm*ld;

            var buf = new Force[2];

            buf[0] = Force.FromVector(ld.CoreArray, 0);
            buf[1] = Force.FromVector(ld.CoreArray, 6);

            return buf;
        }
    }
}