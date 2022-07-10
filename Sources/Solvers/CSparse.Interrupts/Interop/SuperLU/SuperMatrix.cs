
namespace CSparse.Interop.SuperLU
{
    using System;

    // Taken from supermatrix.h file.

    #region Matrix type enums

    /// <summary>
    /// Storage type.
    /// </summary>
    public enum Stype
    {
        /// <summary>
        /// Column-wise, no supernode.
        /// </summary>
        SLU_NC,
        /// <summary>
        /// Column-wise, column-permuted, no supernode.
        /// </summary>
        SLU_NCP,
        /// <summary>
        /// Row-wise, no supernode.
        /// </summary>
        SLU_NR,
        /// <summary>
        /// Column-wise, supernode.
        /// </summary>
        SLU_SC,
        /// <summary>
        /// Supernode, column-wise, permuted.
        /// </summary>
        SLU_SCP,
        /// <summary>
        /// Row-wise, supernode.
        /// </summary>
        SLU_SR,
        /// <summary>
        /// Fortran style column-wise storage for dense matrix.
        /// </summary>
        SLU_DN,
        /// <summary>
        /// Distributed compressed row format.
        /// </summary>
        SLU_NR_loc
    }

    /// <summary>
    /// Data type.
    /// </summary>
    public enum Dtype
    {
        /// <summary>
        /// Single precision.
        /// </summary>
        SLU_S,
        /// <summary>
        /// Double precision.
        /// </summary>
        SLU_D,
        /// <summary>
        /// Single precision complex.
        /// </summary>
        SLU_C,
        /// <summary>
        /// Double precision complex.
        /// </summary>
        SLU_Z
    }

    /// <summary>
    /// Mathematical type.
    /// </summary>
    public enum Mtype
    {
        /// <summary>
        /// General.
        /// </summary>
        SLU_GE,
        /// <summary>
        /// Lower triangular, unit diagonal.
        /// </summary>
        SLU_TRLU,
        /// <summary>
        /// Upper triangular, unit diagonal.
        /// </summary>
        SLU_TRUU,
        /// <summary>
        /// Lower triangular.
        /// </summary>
        SLU_TRL,
        /// <summary>
        /// Upper triangular.
        /// </summary>
        SLU_TRU,
        /// <summary>
        /// Symmetric, store lower half.
        /// </summary>
        SLU_SYL,
        /// <summary>
        /// Symmetric, store upper half.
        /// </summary>
        SLU_SYU,
        /// <summary>
        /// Hermitian, store lower half.
        /// </summary>
        SLU_HEL,
        /// <summary>
        /// Hermitian, store upper half.
        /// </summary>
        SLU_HEU
    }

    #endregion

    #region Storage format structs

    /// <summary>
    /// Stype == SLU_NC (Also known as Harwell-Boeing sparse matrix format).
    /// </summary>
    public struct NCformat
    {
        /// <summary>
        /// Number of nonzeros in the matrix.
        /// </summary>
        public int nnz;
        /// <summary>
        /// Pointer to array of nonzero values, packed by column.
        /// </summary>
        public IntPtr nzval;
        /// <summary>
        /// Pointer to array of row indices of the nonzeros.
        /// </summary>
        public IntPtr rowind;
        /// <summary>
        /// Pointer to array of beginning of columns in nzval[] and rowind[].
        /// </summary>
        public IntPtr colptr;
    }

    /// <summary>
    /// Stype == SLU_NR (compressed sparse row format).
    /// </summary>
    public struct NRformat
    {
        /// <summary>
        /// Number of nonzeros in the matrix.
        /// </summary>
        public int nnz;
        /// <summary>
        /// Pointer to array of nonzero values, packed by row.
        /// </summary>
        public IntPtr nzval;
        /// <summary>
        /// Pointer to array of columns indices of the nonzeros.
        /// </summary>
        public IntPtr colind;
        /// <summary>
        /// Pointer to array of beginning of rows in nzval[] and colind[].
        /// </summary>
        public IntPtr rowptr;
    }

    /// <summary>
    /// Stype == SLU_SC (column-wise, supernode).
    /// </summary>
    public struct SCformat
    {
        /// <summary>
        /// Number of nonzeros in the matrix.
        /// </summary>
        public int nnz;
        /// <summary>
        /// Number of supernodes (minus 1).
        /// </summary>
        public int nsuper;
        /// <summary>
        /// Pointer to array of nonzero values, packed by column.
        /// </summary>
        public IntPtr nzval;
        /// <summary>
        /// Pointer to array of beginning of columns in nzval[].
        /// </summary>
        public IntPtr nzval_colptr;
        /// <summary>
        /// Pointer to array of compressed row indices of rectangular supernodes.
        /// </summary>
        public IntPtr rowind;
        /// <summary>
        /// Pointer to array of beginning of columns in rowind[].
        /// </summary>
        public IntPtr rowind_colptr;
        /// <summary>
        /// col_to_sup[j] is the supernode number to which column j belongs
        /// (mapping from column to supernode number).
        /// </summary>
        public IntPtr col_to_sup;
        /// <summary>
        /// sup_to_col[s] points to the start of the s-th supernode
        /// (mapping from supernode number to column).
        /// </summary>
        public IntPtr sup_to_col;
    }

    /// <summary>
    /// Stype == SLU_DN (dense matrix).
    /// </summary>
    public struct DNformat
    {
        /// <summary>
        /// Leading dimension.
        /// </summary>
        /// <remarks>
        /// Assuming the Fortran column-major ordering, the LDA is used to define the
        /// distance in memory between elements of two consecutive columns which have
        /// the same row index.
        /// </remarks>
        public int lda;
        /// <summary>
        /// Array of size lda*ncol to represent a dense matrix.
        /// </summary>
        public IntPtr nzval;
    }

    #endregion

    /// <summary>
    /// SuperMatrix struct.
    /// </summary>
    public struct SuperMatrix
    {
        /// <summary>
        /// Storage type: interprets the storage structure pointed to by Store.
        /// </summary>
        public Stype Stype;

        /// <summary>
        /// Data type.
        /// </summary>
        public Dtype Dtype;

        /// <summary>
        /// Matrix type: describes the mathematical property of the matrix.
        /// </summary>
        public Mtype Mtype;

        /// <summary>
        /// Number of rows.
        /// </summary>
        public int nrow;

        /// <summary>
        /// Number of columns.
        /// </summary>
        public int ncol;

        /// <summary>
        /// Pointer to the actual storage of the matrix.
        /// </summary>
        public IntPtr Store;
    }
}
