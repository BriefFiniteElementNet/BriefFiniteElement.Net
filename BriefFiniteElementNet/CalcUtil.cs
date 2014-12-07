using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.CSparse.Storage;
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

        public static ZoneDevidedMatrix GetReucedZoneDevidedMatrix(CCS reducedMatrix, DofMappingManager map)
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
    }
}