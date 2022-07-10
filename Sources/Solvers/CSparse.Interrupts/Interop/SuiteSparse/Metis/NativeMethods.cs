
namespace CSparse.Interop.SuiteSparse.Metis
{
    using System;
    using System.Runtime.InteropServices;

    public enum MetisStatus
    {
        /// <summary>
        /// Returned normally
        /// </summary>
        OK = 1,
        /// <summary>
        /// Returned due to erroneous inputs and/or options
        /// </summary>
        ERROR_INPUT = -2,
        /// <summary>
        /// Returned due to insufficient memory
        /// </summary>
        ERROR_MEMORY = -3,
        /// <summary>
        /// Some other errors
        /// </summary>
        ERROR = -4
    }

    internal static class NativeMethods
    {
#if SUITESPARSE_AIO
        const string METIS_DLL = "libsuitesparse";
#else
        const string METIS_DLL = "libmetis";
#endif
        
        /// <summary>
        /// Partition a graph into k parts using multilevel recursive bisection partitioning.
        /// </summary>
        /// <param name="nvtxs">The number of vertices in the graph.</param>
        /// <param name="ncon">The number of balancing constraints. It should be at least 1.</param>
        /// <param name="xadj">The adjacency structure of the graph.</param>
        /// <param name="adjncy">The adjacency structure of the graph.</param>
        /// <param name="vwgt">The weights of the vertices (may be null pointer).</param>
        /// <param name="vsize">The size of the vertices for computing the total communication
        /// volume (may be null pointer).</param>
        /// <param name="adjwgt">The weights of the edges as described (may be null pointer).</param>
        /// <param name="nparts">The number of parts to partition the graph.</param>
        /// <param name="tpwgts">Array of size (nparts x ncon) that specifies the desired weight for 
        /// each partition and constraint (may be null pointer).</param>
        /// <param name="ubvec">Floating point array of size (ncon) that specifies the allowed load
        /// imbalance tolerance for each constraint (may be null pointer).</param>
        /// <param name="options">Array of options (may be null pointer).</param>
        /// <param name="objval">Upon successful completion, this variable stores the edge-cut or 
        /// the total communication volume of the partitioning solution.</param>
        /// <param name="part">This is a vector of size nvtxs that upon successful completion stores 
        /// the partition vector of the graph.</param>
        /// <returns>Error indicator.</returns>
        [DllImport(METIS_DLL, EntryPoint = "METIS_PartGraphRecursive", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PartGraphRecursive(ref int nvtxs, ref int ncon, IntPtr xadj, IntPtr adjncy,
            IntPtr vwgt, IntPtr vsize, IntPtr adjwgt, ref int nparts, IntPtr tpwgts, IntPtr ubvec,
            IntPtr options, ref int objval, IntPtr part);

        /// <summary>
        /// Partition a graph into k parts using multilevel k-way partitioning.
        /// </summary>
        /// <param name="nvtxs">The number of vertices in the graph.</param>
        /// <param name="ncon">The number of balancing constraints. It should be at least 1.</param>
        /// <param name="xadj">The adjacency structure of the graph.</param>
        /// <param name="adjncy">The adjacency structure of the graph.</param>
        /// <param name="vwgt">The weights of the vertices (may be null pointer).</param>
        /// <param name="vsize">The size of the vertices for computing the total communication
        /// volume (may be null pointer).</param>
        /// <param name="adjwgt">The weights of the edges as described (may be null pointer).</param>
        /// <param name="nparts">The number of parts to partition the graph.</param>
        /// <param name="tpwgts">Floating point array of size (nparts x ncon) that specifies the 
        /// desired weight for each partition and constraint (may be null pointer).</param>
        /// <param name="ubvec">Floating point array of size (ncon) that specifies the allowed load 
        /// imbalance tolerance for each constraint (may be null pointer).</param>
        /// <param name="options">Array of options (may be null pointer).</param>
        /// <param name="objval">Upon successful completion, this variable stores the edge-cut or 
        /// the total communication volume of the partitioning solution.</param>
        /// <param name="part">This is a vector of size nvtxs that upon successful completion stores 
        /// the partition vector of the graph.</param>
        /// <returns>Error indicator.</returns>
        [DllImport(METIS_DLL, EntryPoint = "METIS_PartGraphKway", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PartGraphKway(ref int nvtxs, ref int ncon, IntPtr xadj, IntPtr adjncy,
            IntPtr vwgt, IntPtr vsize, IntPtr adjwgt, ref int nparts, IntPtr tpwgts, IntPtr ubvec,
            IntPtr options, ref int objval, IntPtr part);

        /// <summary>
        /// Partition a mesh into k parts based on a partitioning of the mesh's dual graph.
        /// </summary>
        /// <param name="ne">The number of elements in the mesh.</param>
        /// <param name="nn">The number of nodes in the mesh.</param>
        /// <param name="eptr">Array storing the mesh.</param>
        /// <param name="eind">Array storing the mesh.</param>
        /// <param name="vwgt">Array of size (ne) specifying the weights of the elements (may be 
        /// null pointer).</param>
        /// <param name="vsize">Array of size (ne) specifying the size of the elements that is used 
        /// for computing the total communication volume (may be null pointer).</param>
        /// <param name="ncommon">Specifies the number of common nodes that two elements must have in 
        /// order to put an edge between them in the dual graph.</param>
        /// <param name="nparts">The number of parts to partition the mesh.</param>
        /// <param name="tpwgts">Array of size nparts that specifies the desired weight for each 
        /// partition (may be null pointer).</param>
        /// <param name="options">Array of options (may be null pointer).</param>
        /// <param name="objval">Upon successful completion, this variable stores either the edgecut 
        /// or the total communication volume of the dual graph's partitioning.</param>
        /// <param name="epart">Vector of size ne that upon successful completion stores the partition 
        /// vector for the elements of the mesh.</param>
        /// <param name="npart">Vector of size nn that upon successful completion stores the partition 
        /// vector for the nodes of the mesh.</param>
        /// <returns>Error indicator.</returns>
        [DllImport(METIS_DLL, EntryPoint = "METIS_PartMeshDual", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PartMeshDual(ref int ne, ref int nn, IntPtr eptr, IntPtr eind, IntPtr vwgt,
            IntPtr vsize, ref int ncommon, ref int nparts, IntPtr tpwgts, IntPtr options, ref int objval,
            IntPtr epart, IntPtr npart);

        /// <summary>
        /// Partition a mesh into k parts based on a partitioning of the mesh's nodal graph.
        /// </summary>
        /// <param name="ne">The number of elements in the mesh.</param>
        /// <param name="nn">The number of nodes in the mesh.</param>
        /// <param name="eptr">Array storing the mesh.</param>
        /// <param name="eind">Array storing the mesh.</param>
        /// <param name="vwgt">Array of size (nn) specifying the weights of the nodes (may be 
        /// null pointer).</param>
        /// <param name="vsize">Array of size (nn) specifying the size of the nodes that is used 
        /// for computing the total communication volume (may be null pointer).</param>
        /// <param name="nparts">The number of parts to partition the mesh.</param>
        /// <param name="tpwgts">Array of size (nparts) that specifies the desired weight for each 
        /// partition (may be null pointer).</param>
        /// <param name="options">Array of options (may be null pointer).</param>
        /// <param name="objval">Upon successful completion, this variable stores either the edgecut 
        /// or the total communication volume of the nodal graph's partitioning.</param>
        /// <param name="epart">Vector of size ne that upon successful completion stores the partition 
        /// vector for the elements of the mesh.</param>
        /// <param name="npart">Vector of size nn that upon successful completion stores the partition 
        /// vector for the nodes of the mesh.</param>
        /// <returns>Error indicator.</returns>
        [DllImport(METIS_DLL, EntryPoint = "METIS_PartMeshNodal", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PartMeshNodal(ref int ne, ref int nn, IntPtr eptr, IntPtr eind, IntPtr vwgt,
            IntPtr vsize, ref int nparts, IntPtr tpwgts, IntPtr options, ref int objval, IntPtr epart, IntPtr npart);

        /// <summary>
        /// Computes fill reducing orderings of sparse matrices using the multilevel nested
        /// dissection algorithm.
        /// </summary>
        /// <param name="nvtxs">The number of vertices in the graph.</param>
        /// <param name="xadj">Adjacency structure of the graph.</param>
        /// <param name="adjncy">Adjacency structure of the graph.</param>
        /// <param name="vwgt">Array of size (nvtxs) specifying the weights of the vertices (may be 
        /// null pointer).</param>
        /// <param name="options">Array of options (may be null pointer).</param>
        /// <param name="perm">Upon successful completion, vectors of size (nvtxs) storing the 
        /// fill-reducing permutation.</param>
        /// <param name="iperm">Upon successful completion, vectors of size (nvtxs) storing the 
        /// fill-reducing inverse-permutation.</param>
        /// <returns>Error indicator.</returns>
        [DllImport(METIS_DLL, EntryPoint = "METIS_NodeND", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NodeND(ref int nvtxs, IntPtr xadj, IntPtr adjncy, IntPtr vwgt, IntPtr options,
            IntPtr perm, IntPtr iperm);

        /// <summary>
        /// Generate the dual graph of a mesh.
        /// </summary>
        /// <param name="ne">The number of elements in the mesh.</param>
        /// <param name="nn">The number of nodes in the mesh.</param>
        /// <param name="eptr">Array storing the mesh.</param>
        /// <param name="eind">Array storing the mesh.</param>
        /// <param name="ncommon">Specifies the number of common nodes that two elements must have in
        /// order to put an edge between them in the dual graph.</param>
        /// <param name="numflag">Used to indicate which numbering scheme is used for eptr and eind.</param>
        /// <param name="xadj">Arrays storing the adjacency structure of the generated dual graph.</param>
        /// <param name="adjncy">Arrays storing the adjacency structure of the generated dual graph.</param>
        /// <returns>Error indicator.</returns>
        [DllImport(METIS_DLL, EntryPoint = "METIS_MeshToDual", CallingConvention = CallingConvention.Cdecl)]
        public static extern int MeshToDual(ref int ne, ref int nn, IntPtr eptr, IntPtr eind, ref int ncommon,
            ref int numflag, out IntPtr xadj, out IntPtr adjncy);

        /// <summary>
        /// Generate the nodal graph of a mesh.
        /// </summary>
        /// <param name="ne">The number of elements in the mesh.</param>
        /// <param name="nn">The number of nodes in the mesh.</param>
        /// <param name="eptr">Array storing the mesh.</param>
        /// <param name="eind">Array storing the mesh.</param>
        /// <param name="numflag">Used to indicate which numbering scheme is used for eptr and eind.</param>
        /// <param name="xadj">Arrays storing the adjacency structure of the generated dual graph.</param>
        /// <param name="adjncy">Arrays storing the adjacency structure of the generated dual graph.</param>
        /// <returns>Error indicator.</returns>
        [DllImport(METIS_DLL, EntryPoint = "METIS_MeshToNodal", CallingConvention = CallingConvention.Cdecl)]
        public static extern int MeshToNodal(ref int ne, ref int nn, IntPtr eptr, IntPtr eind,
            ref int numflag, out IntPtr xadj, out IntPtr adjncy);

        /// <summary>
        /// Initializes the options array into its default values.
        /// </summary>
        /// <param name="options">The array of options that will be initialized.</param>
        /// <returns>Error indicator.</returns>
        [DllImport(METIS_DLL, EntryPoint = "METIS_SetDefaultOptions", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SetDefaultOptions(int[] options);

        /// <summary>
        /// Frees the memory that was allocated by either the MeshToDual or the MeshToNodal
        /// routines for returning the dual or nodal graph of a mesh.
        /// </summary>
        /// <param name="ptr">The pointer to be freed. This pointer should be one of the xadj or adjncy 
        /// returned by METIS API routines.</param>
        /// <returns>Error indicator.</returns>
        [DllImport(METIS_DLL, EntryPoint = "METIS_Free", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Free(IntPtr ptr);

        /*
        [DllImport(METIS_DLL, EntryPoint = "METIS_NodeNDP", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NodeNDP(int nvtxs, IntPtr xadj, IntPtr adjncy, IntPtr vwgt,
                   int npes, IntPtr options, IntPtr perm, IntPtr iperm, IntPtr sizes);

        [DllImport(METIS_DLL, EntryPoint = "METIS_ComputeVertexSeparator", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ComputeVertexSeparator(ref int nvtxs, IntPtr xadj, IntPtr adjncy,
                   IntPtr vwgt, IntPtr options, IntPtr sepsize, IntPtr part);

        [DllImport(METIS_DLL, EntryPoint = "METIS_NodeRefine", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NodeRefine(int nvtxs, IntPtr xadj, IntPtr vwgt, IntPtr adjncy,
                   IntPtr where, IntPtr hmarker, double ubfactor);
        //*/
    }
}
