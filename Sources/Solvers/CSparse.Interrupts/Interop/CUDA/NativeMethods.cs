
namespace CSparse.Interop.CUDA
{
    using System;
    using System.Numerics;
    using System.Runtime.InteropServices;

    using size_t = System.Int32; // TODO: x64

    internal class NativeMethods
    {
        #region Core

        const string CUDART_DLL = "cudart64_92";

        [DllImport(CUDART_DLL)]
        public static extern CudaResult cudaDriverGetVersion(ref int driverVersion);

        [DllImport(CUDART_DLL)]
        public static extern CudaResult cudaRuntimeGetVersion(ref int runtimeVersion);

        #region Device

        [DllImport(CUDART_DLL)]
        public static extern CudaResult cudaGetDeviceCount(ref int count);

        [DllImport(CUDART_DLL)]
        public static extern CudaResult cudaGetDeviceProperties(ref IntPtr count, int device);

        [DllImport(CUDART_DLL)]
        public static extern CudaResult cudaDeviceGetAttribute(ref int value, DeviceAttribute attr, int device);

        [DllImport(CUDART_DLL)]
        public static extern CudaResult cudaSetDevice(int device);

        #endregion

        #region Memory

        [DllImport(CUDART_DLL)]
        public static extern CudaResult cudaMalloc(ref IntPtr devPtr, size_t size);

        [DllImport(CUDART_DLL)]
        public static extern CudaResult cudaFree(IntPtr devPtr);

        [DllImport(CUDART_DLL)]
        public static extern CudaResult cudaMemcpy(IntPtr dest, IntPtr src, size_t size, MemcpyKind kind);

        #endregion

        #region Streams

        /// <summary>
        /// Creates a stream and returns a handle in <c>phStream</c>. The <c>Flags</c> argument
        [DllImport(CUDART_DLL)]
        public static extern CudaResult cudaStreamCreate(ref IntPtr phStream);

        [DllImport(CUDART_DLL)]
        public static extern CudaResult cudaStreamCreateWithFlags(ref IntPtr phStream, StreamFlags Flags);

        [DllImport(CUDART_DLL)]
        public static extern CudaResult cudaStreamCreateWithPriority(ref IntPtr phStream, StreamFlags Flags, int priority);

        /// <summary>
        /// Destroys the stream specified by hStream.
        /// </summary>
        [DllImport(CUDART_DLL)]
        public static extern CudaResult cudaStreamDestroy(IntPtr hStream);

        #endregion

        #endregion

        #region Sparse

        const string CUSPARSE_DLL = "cusparse64_92";

        [DllImport(CUSPARSE_DLL)]
        public static extern SparseStatus cusparseCreate(ref IntPtr handle);

        [DllImport(CUSPARSE_DLL)]
        public static extern SparseStatus cusparseDestroy(IntPtr handle);

        [DllImport(CUSPARSE_DLL)]
        public static extern SparseStatus cusparseGetVersion(IntPtr handle, ref int version);

        [DllImport(CUSPARSE_DLL)]
        public static extern SparseStatus cusparseSetStream(IntPtr handle, IntPtr streamId);

        [DllImport(CUSPARSE_DLL)]
        public static extern SparseStatus cusparseGetStream(IntPtr handle, ref IntPtr streamId);


        [DllImport(CUSPARSE_DLL)]
        public static extern SparseStatus cusparseCreateMatDescr(ref IntPtr descrA);

        [DllImport(CUSPARSE_DLL)]
        public static extern SparseStatus cusparseDestroyMatDescr(IntPtr descrA);

        [DllImport(CUSPARSE_DLL)]
        public static extern SparseStatus cusparseCopyMatDescr(IntPtr dest, IntPtr src);


        [DllImport(CUSPARSE_DLL)]
        public static extern SparseStatus cusparseSetMatType(IntPtr descrA, MatrixType type);

        [DllImport(CUSPARSE_DLL)]
        public static extern MatrixType cusparseGetMatType(IntPtr descrA);


        [DllImport(CUSPARSE_DLL)]
        public static extern SparseStatus cusparseSetMatFillMode(IntPtr descrA, FillMode fillMode);

        [DllImport(CUSPARSE_DLL)]
        public static extern FillMode cusparseGetMatFillMode(IntPtr descrA);


        [DllImport(CUSPARSE_DLL)]
        public static extern SparseStatus cusparseSetMatDiagType(IntPtr descrA, DiagonalType diagType);

        [DllImport(CUSPARSE_DLL)]
        public static extern DiagonalType cusparseGetMatDiagType(IntPtr descrA);


        [DllImport(CUSPARSE_DLL)]
        public static extern SparseStatus cusparseSetMatIndexBase(IntPtr descrA, IndexBase indexBase);

        [DllImport(CUSPARSE_DLL)]
        public static extern IndexBase cusparseGetMatIndexBase(IntPtr descrA);


        //[DllImport(DLL)]
        //public static extern SparseStatus cusparseScsr2csc(IntPtr handle, int m, int n, int nnz, IntPtr csrVal, IntPtr csrRowPtr, IntPtr csrColInd, IntPtr cscVal, IntPtr cscRowInd, IntPtr cscColPtr, int copyValues, IndexBase idxBase);

        [DllImport(CUSPARSE_DLL)]
        public static extern SparseStatus cusparseDcsr2csc(IntPtr handle, int m, int n, int nnz, IntPtr csrVal, IntPtr csrRowPtr, IntPtr csrColInd, IntPtr cscVal, IntPtr cscRowInd, IntPtr cscColPtr, int copyValues, IndexBase idxBase);

        //[DllImport(DLL)]
        //public static extern SparseStatus cusparseCcsr2csc(IntPtr handle, int m, int n, int nnz, IntPtr csrVal, IntPtr csrRowPtr, IntPtr csrColInd, IntPtr cscVal, IntPtr cscRowInd, IntPtr cscColPtr, int copyValues, IndexBase idxBase);

        [DllImport(CUSPARSE_DLL)]
        public static extern SparseStatus cusparseZcsr2csc(IntPtr handle, int m, int n, int nnz, IntPtr csrVal, IntPtr csrRowPtr, IntPtr csrColInd, IntPtr cscVal, IntPtr cscRowInd, IntPtr cscColPtr, int copyValues, IndexBase idxBase);

        #endregion

        #region Solver

        const string CUSOLVER_DLL = "cusolver64_92";

        /// <summary>
        /// This function initializes the cuSolverSP library and creates a handle on the cuSolver
        /// context. It must be called before any other cuSolverSP API function is invoked. It
        /// allocates hardware resources necessary for accessing the GPU.
        /// </summary>
        /// <param name="handle">the pointer to the handle to the cuSolverSP context.</param>
        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpCreate(ref IntPtr handle);

        /// <summary>
        /// This function releases CPU-side resources used by the cuSolverSP library.
        /// </summary>
        /// <param name="handle">the handle to the cuSolverSP context.</param>
        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDestroy(IntPtr handle);

        /// <summary>
        /// This function sets the stream to be used by the cuSolverSP library to execute its routines.
        /// </summary>
        /// <param name="handle">the handle to the cuSolverSP context.</param>
        /// <param name="streamId">the stream to be used by the library.</param>
        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpSetStream(IntPtr handle, IntPtr streamId);

        /// <summary>
        /// This function gets the stream to be used by the cuSolverSP library to execute its routines.
        /// </summary>
        /// <param name="handle">the handle to the cuSolverSP context.</param>
        /// <param name="streamId">the stream to be used by the library.</param>
        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpGetStream(IntPtr handle, ref IntPtr streamId);

        #region High level interface

        #region LU (Host)

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpScsrlsvluHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr b, float tol, int reorder, IntPtr x, ref int singularity);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrlsvluHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr b, double tol, int reorder, IntPtr x, ref int singularity);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpCcsrlsvluHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr b, float tol, int reorder, IntPtr x, ref int singularity);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrlsvluHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr b, double tol, int reorder, IntPtr x, ref int singularity);

        #endregion

        #region Cholesky (Host)

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpScsrlsvcholHost(IntPtr handle, int m, int nnz, IntPtr descrA, IntPtr csrVal, IntPtr csrRowPtr, IntPtr csrColInd, IntPtr b, float tol, int reorder, IntPtr x, ref int singularity);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrlsvcholHost(IntPtr handle, int m, int nnz, IntPtr descrA, IntPtr csrVal, IntPtr csrRowPtr, IntPtr csrColInd, IntPtr b, double tol, int reorder, IntPtr x, ref int singularity);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpCcsrlsvcholHost(IntPtr handle, int m, int nnz, IntPtr descrA, IntPtr csrVal, IntPtr csrRowPtr, IntPtr csrColInd, IntPtr b, float tol, int reorder, IntPtr x, ref int singularity);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrlsvcholHost(IntPtr handle, int m, int nnz, IntPtr descrA, IntPtr csrVal, IntPtr csrRowPtr, IntPtr csrColInd, IntPtr b, double tol, int reorder, IntPtr x, ref int singularity);

        #endregion

        #region Cholesky (GPU)

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpScsrlsvchol(IntPtr handle, int m, int nnz, IntPtr descrA, IntPtr csrVal, IntPtr csrRowPtr, IntPtr csrColInd, IntPtr b, float tol, int reorder, IntPtr x, ref int singularity);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrlsvchol(IntPtr handle, int m, int nnz, IntPtr descrA, IntPtr csrVal, IntPtr csrRowPtr, IntPtr csrColInd, IntPtr b, double tol, int reorder, IntPtr x, ref int singularity);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpCcsrlsvchol(IntPtr handle, int m, int nnz, IntPtr descrA, IntPtr csrVal, IntPtr csrRowPtr, IntPtr csrColInd, IntPtr b, float tol, int reorder, IntPtr x, ref int singularity);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrlsvchol(IntPtr handle, int m, int nnz, IntPtr descrA, IntPtr csrVal, IntPtr csrRowPtr, IntPtr csrColInd, IntPtr b, double tol, int reorder, IntPtr x, ref int singularity);

        #endregion

        #region QR (GPU)

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpScsrlsvqr(IntPtr handle, int m, int nnz, IntPtr descrA, IntPtr csrVal, IntPtr csrRowPtr, IntPtr csrColInd, IntPtr b, float tol, int reorder, IntPtr x, ref int singularity);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrlsvqr(IntPtr handle, int m, int nnz, IntPtr descrA, IntPtr csrVal, IntPtr csrRowPtr, IntPtr csrColInd, IntPtr b, double tol, int reorder, IntPtr x, ref int singularity);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpCcsrlsvqr(IntPtr handle, int m, int nnz, IntPtr descrA, IntPtr csrVal, IntPtr csrRowPtr, IntPtr csrColInd, IntPtr b, float tol, int reorder, IntPtr x, ref int singularity);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrlsvqr(IntPtr handle, int m, int nnz, IntPtr descrA, IntPtr csrVal, IntPtr csrRowPtr, IntPtr csrColInd, IntPtr b, double tol, int reorder, IntPtr x, ref int singularity);

        #endregion

        #region QR (Host)

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpScsrlsvqrHost(IntPtr handle, int m, int nnz, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr b, float tol, int reorder, IntPtr x, ref int singularity);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrlsvqrHost(IntPtr handle, int m, int nnz, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr b, double tol, int reorder, IntPtr x, ref int singularity);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpCcsrlsvqrHost(IntPtr handle, int m, int nnz, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr b, float tol, int reorder, IntPtr x, ref int singularity);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrlsvqrHost(IntPtr handle, int m, int nnz, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr b, double tol, int reorder, IntPtr x, ref int singularity);

        #endregion

        #region QR least squares (Host)

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpScsrlsqvqrHost(IntPtr handle, int m, int n, int nnz, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr b, float tol, ref int rankA, IntPtr x, IntPtr p, ref float min_norm);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrlsqvqrHost(IntPtr handle, int m, int n, int nnz, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr b, double tol, ref int rankA, IntPtr x, IntPtr p, ref double min_norm);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpCcsrlsqvqrHost(IntPtr handle, int m, int n, int nnz, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr b, float tol, ref int rankA, IntPtr x, IntPtr p, ref float min_norm);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrlsqvqrHost(IntPtr handle, int m, int n, int nnz, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr b, double tol, ref int rankA, IntPtr x, IntPtr p, ref double min_norm);

        #endregion

        #region Ordering

        /// <summary>
        /// Symmetric reverse Cuthill McKee permutation. 
        /// </summary>
        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpXcsrsymrcmHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr p);
        
        /// <summary>
        /// Symmetric minimum degree algorithm by quotient graph.
        /// </summary>
        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpXcsrsymmdqHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr p);
        
        /// <summary>
        /// Symmetric Approximate minimum degree algorithm by quotient graph.
        /// </summary>
        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpXcsrsymamdHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr p);
        
        /// <summary>
        /// Symmetric reordering nested dissection (METIS).
        /// </summary>
        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpXcsrmetisndHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrRowPtrA, IntPtr csrColIndA,
            IntPtr options, // int64*
            IntPtr p);
        
        /// <summary>
        /// Calculate buffer size for P*A*Q^T.
        /// </summary>
        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpXcsrperm_bufferSizeHost(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr p, IntPtr q, ref size_t bufferSizeInBytes);

        /// <summary>
        /// Apply permutation P*A*Q^T.
        /// </summary>
        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpXcsrpermHost(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr p, IntPtr q, IntPtr map, IntPtr pBuffer);

        #endregion

        #endregion

        #region Low level interface

        #region LU (Host)

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpCreateCsrluInfoHost(ref IntPtr info);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDestroyCsrluInfoHost(IntPtr info);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpXcsrluAnalysisHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpXcsrluNnzHost(IntPtr handle, IntPtr nnzLRef, IntPtr nnzURef, IntPtr info);

        /*
        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrluBufferInfoHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrluFactorHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, float pivothreshold, IntPtr pBuffer);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrluZeroPivotHost(IntPtr handle, IntPtr info, float tol, ref int position);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrluSolveHost(IntPtr handle, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrluExtractHost(IntPtr handle, IntPtr P, IntPtr Q, IntPtr descrL, IntPtr csrValL, IntPtr csrRowPtrL, IntPtr csrColIndL, IntPtr descrU, IntPtr csrValU, IntPtr csrRowPtrU, IntPtr csrColIndU, IntPtr info, IntPtr pBuffer);
        //*/

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrluBufferInfoHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrluFactorHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, double pivothreshold, IntPtr pBuffer);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrluZeroPivotHost(IntPtr handle, IntPtr info, double tol, ref int position);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrluSolveHost(IntPtr handle, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrluExtractHost(IntPtr handle, IntPtr P, IntPtr Q, IntPtr descrL, IntPtr csrValL, IntPtr csrRowPtrL, IntPtr csrColIndL, IntPtr descrU, IntPtr csrValU, IntPtr csrRowPtrU, IntPtr csrColIndU, IntPtr info, IntPtr pBuffer);

        /*
        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrluBufferInfoHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrluFactorHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, float pivothreshold, IntPtr pBuffer);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrluZeroPivotHost(IntPtr handle, IntPtr info, float tol, ref int position);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrluSolveHost(IntPtr handle, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrluExtractHost(IntPtr handle, IntPtr P, IntPtr Q, IntPtr descrL, IntPtr csrValL, IntPtr csrRowPtrL, IntPtr csrColIndL, IntPtr descrU, IntPtr csrValU, IntPtr csrRowPtrU, IntPtr csrColIndU, IntPtr info, IntPtr pBuffer);
        //*/

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrluBufferInfoHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrluFactorHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, double pivothreshold, IntPtr pBuffer);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrluZeroPivotHost(IntPtr handle, IntPtr info, double tol, ref int position);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrluSolveHost(IntPtr handle, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrluExtractHost(IntPtr handle, IntPtr P, IntPtr Q, IntPtr descrL, IntPtr csrValL, IntPtr csrRowPtrL, IntPtr csrColIndL, IntPtr descrU, IntPtr csrValU, IntPtr csrRowPtrU, IntPtr csrColIndU, IntPtr info, IntPtr pBuffer);

        #endregion

        #region Cholesky (GPU)


        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpCreateCsrcholInfo(ref IntPtr info);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDestroyCsrcholInfo(IntPtr info);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpXcsrcholAnalysis(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpScsrcholBufferInfo(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        /*
        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrcholFactor(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, IntPtr pBuffer);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrcholZeroPivot(IntPtr handle, IntPtr info, float tol, ref int position);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrcholSolve(IntPtr handle, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);
        //*/

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrcholBufferInfo(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrcholFactor(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, IntPtr pBuffer);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrcholZeroPivot(IntPtr handle, IntPtr info, double tol, ref int position);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrcholSolve(IntPtr handle, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        /*
        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrcholBufferInfo(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrcholFactor(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, IntPtr pBuffer);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrcholZeroPivot(IntPtr handle, IntPtr info, float tol, ref int position);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrcholSolve(IntPtr handle, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);
        //*/

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrcholBufferInfo(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrcholFactor(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, IntPtr pBuffer);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrcholZeroPivot(IntPtr handle, IntPtr info, double tol, ref int position);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrcholSolve(IntPtr handle, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        #endregion

        #region Cholesky (Host)

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpCreateCsrcholInfoHost(ref IntPtr info);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDestroyCsrcholInfoHost(IntPtr info);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpXcsrcholAnalysisHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info);

        /*
        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrcholBufferInfoHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrcholFactorHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, IntPtr pBuffer);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrcholZeroPivotHost(IntPtr handle, IntPtr info, float tol, ref int position);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrcholSolveHost(IntPtr handle, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);
        //*/

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrcholBufferInfoHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrcholFactorHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, IntPtr pBuffer);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrcholZeroPivotHost(IntPtr handle, IntPtr info, double tol, ref int position);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrcholSolveHost(IntPtr handle, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        /*
        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrcholBufferInfoHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrcholFactorHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, IntPtr pBuffer);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrcholZeroPivotHost(IntPtr handle, IntPtr info, float tol, ref int position);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrcholSolveHost(IntPtr handle, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);
        //*/

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrcholBufferInfoHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrcholFactorHost(IntPtr handle, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, IntPtr pBuffer);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrcholZeroPivotHost(IntPtr handle, IntPtr info, double tol, ref int position);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrcholSolveHost(IntPtr handle, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        #endregion

        #region QR (GPU)

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpCreateCsrqrInfo(ref IntPtr info);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDestroyCsrqrInfo(IntPtr info);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpXcsrqrAnalysis(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info);

        /*
        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrqrBufferInfo(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrqrSetup(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, float mu, IntPtr info);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrqrFactor(IntPtr handle, int m, int n, int nnzA, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrqrZeroPivot(IntPtr handle, IntPtr info, float tol, ref int position);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrqrSolve(IntPtr handle, int m, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);
        //*/

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrqrBufferInfo(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrqrSetup(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, double mu, IntPtr info);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrqrFactor(IntPtr handle, int m, int n, int nnzA, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrqrZeroPivot(IntPtr handle, IntPtr info, double tol, ref int position);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrqrSolve(IntPtr handle, int m, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        /*
        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrqrBufferInfo(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrqrSetup(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, Complex32 mu, IntPtr info);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrqrFactor(IntPtr handle, int m, int n, int nnzA, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrqrZeroPivot(IntPtr handle, IntPtr info, float tol, ref int position);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrqrSolve(IntPtr handle, int m, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);
        //*/

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrqrBufferInfo(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrqrSetup(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, Complex mu, IntPtr info);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrqrFactor(IntPtr handle, int m, int n, int nnzA, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrqrZeroPivot(IntPtr handle, IntPtr info, double tol, ref int position);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrqrSolve(IntPtr handle, int m, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        #endregion

        #region QR (Host)

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpCreateCsrqrInfoHost(ref IntPtr info);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDestroyCsrqrInfoHost(IntPtr info);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpXcsrqrAnalysisHost(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info);

        /*
        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrqrBufferInfoHost(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrqrSetupHost(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, float mu, IntPtr info);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrqrFactorHost(IntPtr handle, int m, int n, int nnzA, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrqrZeroPivotHost(IntPtr handle, IntPtr info, float tol, ref int position);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpScsrqrSolveHost(IntPtr handle, int m, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);
        //*/

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrqrBufferInfoHost(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrqrSetupHost(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, double mu, IntPtr info);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrqrFactorHost(IntPtr handle, int m, int n, int nnzA, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrqrZeroPivotHost(IntPtr handle, IntPtr info, double tol, ref int position);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpDcsrqrSolveHost(IntPtr handle, int m, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        /*
        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrqrBufferInfoHost(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrqrSetupHost(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, Complex32 mu, IntPtr info);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrqrFactorHost(IntPtr handle, int m, int n, int nnzA, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrqrZeroPivotHost(IntPtr handle, IntPtr info, float tol, ref int position);

        [DllImport(DLL)]
        public static extern SolverStatus cusolverSpCcsrqrSolveHost(IntPtr handle, int m, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);
        //*/

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrqrBufferInfoHost(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, IntPtr info, ref size_t internalDataInBytes, ref size_t workspaceInBytes);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrqrSetupHost(IntPtr handle, int m, int n, int nnzA, IntPtr descrA, IntPtr csrValA, IntPtr csrRowPtrA, IntPtr csrColIndA, Complex mu, IntPtr info);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrqrFactorHost(IntPtr handle, int m, int n, int nnzA, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrqrZeroPivotHost(IntPtr handle, IntPtr info, double tol, ref int position);

        [DllImport(CUSOLVER_DLL)]
        public static extern SolverStatus cusolverSpZcsrqrSolveHost(IntPtr handle, int m, int n, IntPtr b, IntPtr x, IntPtr info, IntPtr pBuffer);

        #endregion

        #endregion

        #endregion
    }
}
