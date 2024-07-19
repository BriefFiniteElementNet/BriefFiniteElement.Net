using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace BriefFiniteElementNet.Utils
{
    public static class GraphUtils
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

                var part = DepthFirstSearch(graph, visited, startPoint).Distinct().ToList();

                buf.Add(part);
            }

            return buf;
        }

        /// <summary>
        /// gives a group number to each node 
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static int[] EnumerateGraphPartsAsGroups(CompressedColumnStorage<double> graph)
        {
            var buf = new List<List<int>>();

            var n = graph.ColumnCount;

            var groups = Enumerable.Repeat(0, n).ToArray();// new int[n];

            var visited = new bool[n];

            var ri = graph.RowIndices;
            var cp = graph.ColumnPointers;

            for (var i = 0; i < n; i++)
            {
                if (cp[i] == cp[i + 1])
                    visited[i] = true;
            }

            var cntr = 1;

            while (true)
            {
                var startPoint = visited.FirstIndexOf(false);

                if (startPoint == -1)
                    break;

                var part = DepthFirstSearch(graph, visited, startPoint).Distinct().ToList();

                buf.Add(part);
                foreach (var nde in part)
                {
                    groups[nde] = cntr;
                }

                cntr++;
            }

            return groups;
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
                        //buf.Add(neighbor);
                    }
                }
            }


            return buf;
        }

    }
}
