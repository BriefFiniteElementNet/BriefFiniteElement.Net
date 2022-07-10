
namespace CSparse.Interop.SuiteSparse.Metis
{
    using CSparse.Interop.Common;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Adjacency structure of the graph.
    /// </summary>
    /// <remarks>
    /// The adjacency structure of the graph is stored using the compressed storage format (CSR).
    /// The CSR format is a widely used scheme for storing sparse graphs. In this format the
    /// adjacency structure of a graph with n vertices and m edges is represented using two arrays
    /// xadj and adjncy. The xadj array is of size n + 1 whereas the adjncy array is of size 2m
    /// (this is because for each edge between vertices v and u we actually store both (v; u) and
    /// (u; v)).
    /// 
    /// The adjacency structure of the graph is stored as follows. Assuming that vertex numbering
    /// starts from 0 (C style), then the adjacency list of vertex i is stored in array adjncy
    /// starting at index xadj[i] and ending at (but not including) index xadj[i+1] (i.e.,
    /// adjncy[xadj[i]] through and including adjncy[xadj[i+1]-1]). That is, for each vertex i,
    /// its adjacency list is stored in consecutive locations in the array adjncy, and the array
    /// xadj is used to point to where it begins and where it ends.
    /// 
    /// The weights of the vertices (if any) are stored in an additional array called vwgt. If ncon
    /// is the number of weights associated with each vertex, the array vwgt contains n * ncon
    /// elements (recall that n is the number of vertices). The weights of the ith vertex are
    /// stored in ncon consecutive entries starting at location vwgt[i * ncon]. Note that if each
    /// vertex has only a single weight, then vwgt will contain n elements, and vwgt[i] will store
    /// the weight of the ith vertex. The vertex-weights must be integers greater or equal to zero.
    /// If all the vertices of the graph have the same weight (i.e., the graph is unweighted), then
    /// the vwgt can be set to NULL.
    /// 
    /// The weights of the edges (if any) are stored in an additional array called adjwgt. This
    /// array contains 2m elements, and the weight of edge adjncy[j] is stored at location adjwgt[j].
    /// The edge-weights must be integers greater than zero. If all the edges of the graph have the
    /// same weight (i.e., the graph is unweighted), then the adjwgt can be set to NULL.
    /// </remarks>
    public class MetisGraph
    {
        private readonly int nvtxs; // The # of vertices in the graph
        private readonly int nedges; // The # of edges in the graph
        private readonly int ncon; // The # of balancing constraints (should be at least 1).

        private readonly int[] xadj;   // Pointers to the locally stored vertices
        private readonly int[] adjncy; // Array that stores the adjacency lists of nvtxs
        private readonly int[] adjwgt; // Array that stores the weights of the adjacency lists

        private int[] vwgt; // Vertex weights
        private int[] vsize; // Vertex sizes for min-volume formulation

        #region Public properties

        /// <summary>
        /// Gets the number of vertices in the graph.
        /// </summary>
        public int VertexCount { get { return nvtxs; } }

        /// <summary>
        /// Gets the number of edges in the graph.
        /// </summary>
        public int EdgeCount { get { return nedges; } }

        /// <summary>
        /// Gets the adjacency pointers.
        /// </summary>
        public IReadOnlyList<int> Pointers { get { return xadj; } }

        /// <summary>
        /// Gets the adjacency indices.
        /// </summary>
        public IReadOnlyList<int> Indices { get { return adjncy; } }

        /// <summary>
        /// Gets the vertex weights.
        /// </summary>
        public IReadOnlyList<int> VertexWeights { get { return vwgt; } }

        /// <summary>
        /// Gets the weights of the adjacency lists.
        /// </summary>
        public IReadOnlyList<int> EdgeWeights { get { return adjwgt; } }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MetisGraph"/> class.
        /// </summary>
        /// <param name="nvtxs">The number of vertices.</param>
        /// <param name="nedges">The number of edges.</param>
        /// <param name="xadj">Pointers to the locally stored vertices.</param>
        /// <param name="adjncy">The adjacency lists.</param>
        public MetisGraph(int nvtxs, int nedges, int[] xadj, int[] adjncy)
            : this(nvtxs, nedges, xadj, adjncy, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetisGraph"/> class.
        /// </summary>
        /// <param name="nvtxs">The number of vertices.</param>
        /// <param name="nedges">The number of edges.</param>
        /// <param name="xadj">Pointers to the locally stored vertices.</param>
        /// <param name="adjncy">The adjacency lists.</param>
        /// <param name="vwgt">Vertex weights.</param>
        public MetisGraph(int nvtxs, int nedges, int[] xadj, int[] adjncy, int[] vwgt)
            : this(nvtxs, nedges, xadj, adjncy, vwgt, null)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MetisGraph"/> class.
        /// </summary>
        /// <param name="nvtxs">The number of vertices.</param>
        /// <param name="nedges">The number of edges.</param>
        /// <param name="xadj">Pointers to the locally stored vertices.</param>
        /// <param name="adjncy">The adjacency lists.</param>
        /// <param name="vwgt">Vertex weights.</param>
        /// <param name="adjwgt">The weights of the adjacency lists.</param>
        public MetisGraph(int nvtxs, int nedges, int[] xadj, int[] adjncy, int[] vwgt, int[] adjwgt)
        {
            this.nvtxs = nvtxs;
            this.nedges = nedges;

            this.xadj = xadj ?? throw new ArgumentNullException("xadj");
            this.adjncy = adjncy ?? throw new ArgumentNullException("adjncy");

            if (vwgt == null)
            {
                this.ncon = 1;
            }
            else if (vwgt.Length % nvtxs != 0)
            {
                throw new ArgumentException("Expected length of the array to be a multiple of nvtxs.", "vwgt");
            }
            else
            {
                this.vwgt = vwgt;
                this.ncon = vwgt.Length / nvtxs;
            }

            if (adjwgt != null)
            {
                if (adjwgt.Length != 2 * nedges)
                {
                    throw new ArgumentException("Expected length of the array to be 2 * nedges.", "adjwgt");
                }

                this.adjwgt = adjwgt;
            }
        }

        /// <summary>
        /// Partition a graph into k parts using multilevel k-way partitioning.
        /// </summary>
        /// <param name="k">The number of partitions.</param>
        /// <param name="part">Target array storing the partition of the graph (size nvtxs).</param>
        /// <param name="options">Partitioning options.</param>
        /// <returns></returns>
        public MetisStatus PartitionKway(int k, int[] part, MetisOptions options = null)
        {
            return Partition(k, part, options, true);
        }

        /// <summary>
        /// Partition a graph into k parts using multilevel recursive bisection partitioning.
        /// </summary>
        /// <param name="k">The number of partitions.</param>
        /// <param name="part">Target array storing the partition of the graph (size nvtxs).</param>
        /// <param name="options">Partitioning options.</param>
        /// <returns></returns>
        public MetisStatus PartitionRecursive(int k, int[] part, MetisOptions options = null)
        {
            return Partition(k, part, options, false);
        }

        /// <summary>
        /// Computes fill reducing orderings of sparse matrices using the multilevel nested dissection algorithm.
        /// </summary>
        /// <param name="perm">Target array storing the fill-reducing permutaion (size nvtxs).</param>
        /// <param name="iperm">Target array storing the fill-reducing inverse permutaion (size nvtxs).</param>
        /// <param name="options">Partitioning options.</param>
        /// <returns></returns>
        public MetisStatus NestedDissection(int[] perm, int[] iperm, MetisOptions options = null)
        {
            if (perm.Length != nvtxs || iperm.Length != nvtxs)
            {
                return MetisStatus.ERROR_INPUT;
            }

            var handles = new List<GCHandle>();

            // Pin array data.
            var p_xadj = InteropHelper.Pin(xadj, handles);
            var p_adjncy = InteropHelper.Pin(adjncy, handles);
            var p_vwgt = InteropHelper.Pin(vwgt, handles);

            var p_opts = options == null ? IntPtr.Zero : InteropHelper.Pin(options.raw, handles);

            var p_perm = InteropHelper.Pin(perm, handles);
            var p_iperm = InteropHelper.Pin(iperm, handles);

            int l_nv = nvtxs;

            int status = 0;

            try
            {
                status = NativeMethods.NodeND(ref l_nv, p_xadj, p_adjncy, p_vwgt, p_opts, p_perm, p_iperm);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                InteropHelper.Free(handles);
            }

            return (MetisStatus)status;
        }

        private MetisStatus Partition(int nparts, int[] part, MetisOptions options, bool kway)
        {
            int objval = 0;

            if (part == null || part.Length < this.nvtxs)
            {
                return MetisStatus.ERROR_INPUT;
            }
            
            var handles = new List<GCHandle>();

            // Pin array data.
            var p_part = InteropHelper.Pin(part, handles);
            var p_xadj = InteropHelper.Pin(xadj, handles);
            var p_adjncy = InteropHelper.Pin(adjncy, handles);
            var p_vwgt = InteropHelper.Pin(vwgt, handles);
            var p_ewgt = InteropHelper.Pin(adjwgt, handles);
            var p_vsize = InteropHelper.Pin(vsize, handles);

            var p_opts = options == null ? IntPtr.Zero : InteropHelper.Pin(options.raw, handles);

            int l_nv = nvtxs;
            int l_nw = ncon;

            int status = 0;

            try
            {
                if (kway)
                {
                    status = NativeMethods.PartGraphKway(ref l_nv, ref l_nw, p_xadj, p_adjncy,
                        p_vwgt, p_vsize, p_ewgt, ref nparts, IntPtr.Zero, IntPtr.Zero,
                        p_opts, ref objval, p_part);
                }
                else
                {
                    status = NativeMethods.PartGraphRecursive(ref l_nv, ref l_nw, p_xadj, p_adjncy,
                        p_vwgt, p_vsize, p_ewgt, ref nparts, IntPtr.Zero, IntPtr.Zero,
                        p_opts, ref objval, p_part);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                InteropHelper.Free(handles);
            }

            return (MetisStatus)status;
        }
    }
}
