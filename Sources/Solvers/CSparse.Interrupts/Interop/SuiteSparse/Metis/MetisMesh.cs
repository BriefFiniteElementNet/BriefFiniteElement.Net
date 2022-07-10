
namespace CSparse.Interop.SuiteSparse.Metis
{
    using CSparse.Interop.Common;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Mesh adjacency data structure.
    /// </summary>
    /// <remarks>
    /// All of the mesh partitioning and mesh conversion routines in METIS take as input the
    /// element node array of a mesh. This element node array is stored using a pair of arrays
    /// called eptr and eind, which are similar to the xadj and adjncy arrays used for storing
    /// the adjacency structure of a graph.
    /// 
    /// The size of the eptr array is n+1, where n is the number of elements in the mesh. The
    /// size of the eind array is of size equal to the sum of the number of nodes in all the
    /// elements of the mesh. The list of nodes belonging to the ith element of the mesh are
    /// stored in consecutive locations of eind starting at position eptr[i] up to (but not
    /// including) position eptr[i+1].
    /// 
    /// This format makes it easy to specify meshes of any type of elements, including meshes with
    /// mixed element types that have different number of nodes per element. The ordering of the
    /// nodes in each element is not important.
    /// </remarks>
    public class MetisMesh
    {
        private readonly int nn; // The # of nodes in the mesh
        private readonly int ne; // The # of elements in the mesh
        private readonly int ncon;   // The # of element balancing constraints (element weights)

        private readonly int[] eptr; // The CSR-structure storing the nodes in the elements
        private readonly int[] eind; // The CSR-structure storing the nodes in the elements
        private readonly int[] ewgt; // The weights of the elements

        #region Public properties

        /// <summary>
        /// Gets the number of nodes.
        /// </summary>
        public int NodeCount { get { return nn; } }

        /// <summary>
        /// Gets the number of elements.
        /// </summary>
        public int ElementCount { get { return ne; } }

        /// <summary>
        /// Gets the element pointers.
        /// </summary>
        public IReadOnlyList<int> ElementPointers { get { return eptr; } }

        /// <summary>
        /// Gets the element indices.
        /// </summary>
        public IReadOnlyList<int> ElementIndices { get { return eind; } }

        /// <summary>
        /// Gets the element weights.
        /// </summary>
        public IReadOnlyList<int> ElementWeights { get { return ewgt; } }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MetisMesh"/> class.
        /// </summary>
        /// <param name="nn">The number of nodes in the mesh</param>
        /// <param name="ne">The number of elements in the mesh</param>
        /// <param name="eptr">The element pointers.</param>
        /// <param name="eind">The element indices.</param>
        public MetisMesh(int nn, int ne, int[] eptr, int[] eind)
            : this(nn, ne, eptr, eind, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetisMesh"/> class.
        /// </summary>
        /// <param name="nn">The number of nodes in the mesh</param>
        /// <param name="ne">The number of elements in the mesh</param>
        /// <param name="eptr">The element pointers.</param>
        /// <param name="eind">The element indices.</param>
        /// <param name="ewgt">The weights of the elements.</param>
        public MetisMesh(int nn, int ne, int[] eptr, int[] eind, int[] ewgt)
        {
            // mesh size constants
            this.ne = ne;
            this.nn = nn;
            this.ncon = 1;

            // memory for the mesh structure
            this.eptr = eptr ?? throw new ArgumentNullException("eptr");
            this.eind = eind ?? throw new ArgumentNullException("eind");
            this.ewgt = ewgt;
        }

        /// <summary>
        /// Partition a mesh into k parts based on a partitioning of the mesh's dual graph.
        /// </summary>
        /// <param name="k">The number of partitions.</param>
        /// <param name="epart">Target array storing the element partition (size ne).</param>
        /// <param name="npart">Target array storing the node partition (size nn).</param>
        /// <param name="options">Partitioning options.</param>
        /// <returns></returns>
        public MetisStatus PartitionDual(int k, int[] epart, int[] npart, MetisOptions options = null)
        {
            return Partition(k, epart, npart, options, true);
        }

        /// <summary>
        /// Partition a mesh into k parts based on a partitioning of the mesh's nodal graph.
        /// </summary>
        /// <param name="k">The number of partitions.</param>
        /// <param name="epart">Target array storing the element partition (size ne).</param>
        /// <param name="npart">Target array storing the node partition (size nn).</param>
        /// <param name="options">Partitioning options.</param>
        /// <returns></returns>
        public MetisStatus PartitionNodal(int k, int[] epart, int[] npart, MetisOptions options = null)
        {
            return Partition(k, epart, npart, options, false);
        }

        /// <summary>
        /// Generate the dual graph of a mesh.
        /// </summary>
        /// <param name="ncommon">Specifies the number of common nodes that two elements must have in
        /// order to put an edge between them in the dual graph.</param>
        /// <returns></returns>
        public MetisGraph ToDualGraph(int ncommon = 2)
        {
            return ToGraph(true, ncommon);
        }

        /// <summary>
        /// Generate the nodal graph of a mesh.
        /// </summary>
        /// <returns></returns>
        public MetisGraph ToNodalGraph()
        {
            return ToGraph(false, 0);
        }

        private MetisStatus Partition(int nparts, int[] epart, int[] npart, MetisOptions options, bool dual)
        {
            if (ne <= 0)
            {
                return MetisStatus.ERROR_INPUT;
            }

            if (epart == null || epart.Length < ne)
            {
                return MetisStatus.ERROR_INPUT;
            }

            if (npart == null || npart.Length < nn)
            {
                return MetisStatus.ERROR_INPUT;
            }

            int objval = 0;
            int ncommon = 2; // for triangles

            float[] tpwgts = GetWeights(nparts, ncon);
            
            var handles = new List<GCHandle>();

            // Pin array data and get pointers.
            var p_eptr = InteropHelper.Pin(eptr, handles);
            var p_eind = InteropHelper.Pin(eind, handles);
            var p_ewgt = InteropHelper.Pin(ewgt, handles);
            var p_tpwgts = InteropHelper.Pin(tpwgts, handles);
            var p_epart = InteropHelper.Pin(epart, handles);
            var p_npart = InteropHelper.Pin(npart, handles);

            var p_opts = options == null ? IntPtr.Zero : InteropHelper.Pin(options.raw, handles);

            int l_ne = ne;
            int l_nn = nn;

            int status = 0;

            try
            {
                if (dual)
                {
                    status = NativeMethods.PartMeshDual(ref l_ne, ref l_nn, p_eptr, p_eind, p_ewgt, IntPtr.Zero,
                        ref ncommon, ref nparts, p_tpwgts, p_opts, ref objval, p_epart, p_npart);
                }
                else
                {
                    status = NativeMethods.PartMeshNodal(ref l_ne, ref l_nn, p_eptr, p_eind, IntPtr.Zero, IntPtr.Zero,
                        ref nparts, p_tpwgts, p_opts, ref objval, p_epart, p_npart);
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

        private MetisGraph ToGraph(bool dual, int ncommon)
        {
            int numflag = 0;

            IntPtr p_xadj;
            IntPtr p_adjncy;

            var handles = new List<GCHandle>();

            // Pin array data.
            var p_eptr = InteropHelper.Pin(eptr, handles);
            var p_eind = InteropHelper.Pin(eind, handles);

            int status = 0;

            int l_ne = ne;
            int l_nn = nn;

            MetisGraph graph = null;

            try
            {
                if (dual)
                {
                    status = NativeMethods.MeshToDual(ref l_ne, ref l_nn, p_eptr, p_eind, ref ncommon,
                        ref numflag, out p_xadj, out p_adjncy);
                }
                else
                {
                    status = NativeMethods.MeshToNodal(ref l_ne, ref l_nn, p_eptr, p_eind,
                        ref numflag, out p_xadj, out p_adjncy);
                }

                if (status > 0)
                {
                    // Number of vertices
                    int nv = dual ? this.ne : this.nn;
                    var xadj = new int[nv + 1];

                    Marshal.Copy(p_xadj, xadj, 0, nv + 1);

                    // Number of edges
                    int ne = xadj[nv];
                    var adjncy = new int[ne];

                    Marshal.Copy(p_adjncy, adjncy, 0, ne);

                    graph = new MetisGraph(nv, ne / 2, xadj, adjncy);

                    // Free native memory.
                    NativeMethods.Free(p_xadj);
                    NativeMethods.Free(p_adjncy);
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

            return graph;
        }

        private static float[] GetWeights(int nparts, int ncon)
        {
            float[] tpwgts = new float[nparts * ncon];

            for (int i = 0; i < nparts; i++)
            {
                for (int j = 0; j < ncon; j++)
                {
                    tpwgts[i * ncon + j] = 1.0f / nparts;
                }
            }

            return tpwgts;
        }
    }
}
