
namespace CSparse.Interop.MKL
{
    using System;
    using System.Runtime.InteropServices;

    #region Enums

    /// <summary>
    /// status of the routines
    /// </summary>
    public enum SparseStatus
    {
        /// <summary>
        /// the operation was successful
        /// </summary>
        Success = 0,
        /// <summary>
        /// empty handle or matrix arrays
        /// </summary>
        NotInitialized = 1,
        /// <summary>
        /// internal error: memory allocation failed
        /// </summary>
        AllocFailed = 2,
        /// <summary>
        /// invalid input value
        /// </summary>
        InvalidValue = 3,
        /// <summary>
        /// e.g. 0-diagonal element for triangular solver, etc.
        /// </summary>
        ExecutionFailed = 4,
        /// <summary>
        /// internal error
        /// </summary>
        InternalError = 5,
        /// <summary>
        /// operation for double precision doesn't support other types
        /// </summary>
        NotSupported = 6
    }
    
    /// <summary>
    /// supported matrix types
    /// </summary>
    enum SparseMatrixType
    {
        General = 20,
        Symmetric = 21,
        Hermitian = 22,
        Triangular = 23,
        Diagonal = 24,
        BlockTriangular = 25,
        BlockDiagonal = 26
    }
    
    /// <summary>
    /// sparse matrix indexing: C-style or Fortran-style
    /// </summary>
    enum SparseIndexBase
    {
        Zero = 0,
        One = 1
    }
    
    /// <summary>
    /// applies to triangular matrices only ( SYMMETRIC, HERMITIAN, TRIANGULAR )
    /// </summary>
    enum SparseFillMode
    {
        Lower = 40,
        Upper = 41,
        Full = 42
    }
    
    /// <summary>
    /// applies to triangular matrices only ( SYMMETRIC, HERMITIAN, TRIANGULAR )
    /// </summary>
    enum SparseDiagType
    {
        NonUnit = 50,
        Unit = 51
    }

    #endregion

    [StructLayout(LayoutKind.Sequential)]
    internal struct MKLVersion
    {
        public int MajorVersion;
        public int MinorVersion;
        public int BuildNumber;
        IntPtr ProductStatus; // char*
        IntPtr Build; // char*
        IntPtr Processor; // char*
        IntPtr Platform; // char*
    }
    
    [StructLayout(LayoutKind.Sequential)]
    internal struct MatrixDescriptor
    {
        public SparseMatrixType type;
        public SparseFillMode mode;
        public SparseDiagType diag;
    }
}
