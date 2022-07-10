using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CSparse.Interop.MKL
{
    internal static class NativeMethods
    {
        const string MKL_DLL = @"C:\Program Files (x86)\Intel\oneAPI\mkl\2022.1.0\redist\intel64\mkl_rt.2.dll";

        #region Info

        /// <summary>
        /// Returns the Intel MKL version.
        /// </summary>
        /// <param name="version">MKL version</param>
        [DllImport(MKL_DLL, EntryPoint = "mkl_get_version", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void mkl_get_version(ref MKLVersion version);

        /// <summary>
        /// Returns the Intel MKL version in a character string.
        /// </summary>
        /// <param name="buf">Source string</param>
        /// <param name="len">Length of the source string</param>
        [DllImport(MKL_DLL, EntryPoint = "mkl_get_version_string", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void mkl_get_version_string(StringBuilder buf, int len);

        /// <summary>
        /// Specifies the number of OpenMP threads to use.
        /// </summary>
        /// <param name="nt">The number of threads suggested by the user.</param>
        [DllImport(MKL_DLL, EntryPoint = "mkl_set_num_threads", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void mkl_set_num_threads(int nt);

        /// <summary>
        /// Specifies the number of OpenMP threads for all Intel MKL functions on the current execution thread.
        /// </summary>
        /// <param name="nt">The number of threads for Intel MKL functions to use on the current execution thread.</param>
        [DllImport(MKL_DLL, EntryPoint = "mkl_set_num_threads_local", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void mkl_set_num_threads_local(int nt);

        /// <summary>
        /// Gets the number of OpenMP threads targeted for parallelism.
        /// </summary>
        /// <returns>The maximum number of threads for Intel MKL functions to use in internal parallel regions.</returns>
        [DllImport(MKL_DLL, EntryPoint = "mkl_get_max_threads", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mkl_get_max_threads();

        /// <summary>
        /// Configures the CNR mode of Intel MKL.
        /// </summary>
        /// <param name="setting">CNR branch to set.</param>
        [DllImport(MKL_DLL, EntryPoint = "mkl_cbwr_set", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mkl_cbwr_set(int setting);

        /// <summary>
        /// Returns the current CNR settings.
        /// </summary>
        /// <returns></returns>
        [DllImport(MKL_DLL, EntryPoint = "mkl_cbwr_get", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mkl_cbwr_get(int option);

        #endregion

        #region Sparse matrix
        
        // compressed sparse row format (4-arrays version),
        // SPARSE_MATRIX_TYPE_GENERAL by default, pointers to input arrays are stored in the handle
        //
        // https://software.intel.com/en-us/mkl-developer-reference-c-mkl-sparse-create-csr

        [DllImport(MKL_DLL, EntryPoint = "mkl_sparse_s_create_csr", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SparseStatus mkl_sparse_s_create_csr(/* sparse_matrix_t* */ ref IntPtr A,
                                                 SparseIndexBase indexing,
                                                 int rows,
                                                 int cols,
                                                 /* MKL_INT* */ IntPtr rows_start,
                                                 /* MKL_INT* */ IntPtr rows_end,
                                                 /* MKL_INT* */ IntPtr col_indx,
                                                 /* IntPtr */ IntPtr values);

        [DllImport(MKL_DLL, EntryPoint = "mkl_sparse_d_create_csr", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SparseStatus mkl_sparse_d_create_csr(/* sparse_matrix_t* */ ref IntPtr A,
                                                 SparseIndexBase indexing,
                                                 int rows,
                                                 int cols,
                                                 /* MKL_INT* */ IntPtr rows_start,
                                                 /* MKL_INT* */ IntPtr rows_end,
                                                 /* MKL_INT* */ IntPtr col_indx,
                                                 /* IntPtr */ IntPtr values);

        [DllImport(MKL_DLL, EntryPoint = "mkl_sparse_c_create_csr", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SparseStatus mkl_sparse_c_create_csr(/* sparse_matrix_t* */ ref IntPtr A,
                                                 SparseIndexBase indexing,
                                                 int rows,
                                                 int cols,
                                                 /* MKL_INT* */ IntPtr rows_start,
                                                 /* MKL_INT* */ IntPtr rows_end,
                                                 /* MKL_INT* */ IntPtr col_indx,
                                                 /* MKL_Complex8* */ IntPtr values);

        [DllImport(MKL_DLL, EntryPoint = "mkl_sparse_z_create_csr", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SparseStatus mkl_sparse_z_create_csr(/* sparse_matrix_t* */ ref IntPtr A,
                                                 SparseIndexBase indexing,
                                                 int rows,
                                                 int cols,
                                                 /* MKL_INT* */ IntPtr rows_start,
                                                 /* MKL_INT* */ IntPtr rows_end,
                                                 /* MKL_INT* */ IntPtr col_indx,
                                                 /* MKL_Complex16* */ IntPtr values);

        // compressed sparse column format (4-arrays version),
        // SPARSE_MATRIX_TYPE_GENERAL by default, pointers to input arrays are stored in the handle
        //
        // https://software.intel.com/en-us/mkl-developer-reference-c-mkl-sparse-create-csc

        [DllImport(MKL_DLL, EntryPoint = "mkl_sparse_s_create_csc", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SparseStatus mkl_sparse_s_create_csc(/* sparse_matrix_t* */ ref IntPtr A,
                                                 SparseIndexBase indexing,
                                                 int rows,
                                                 int cols,
                                                 /* MKL_INT* */ IntPtr rows_start,
                                                 /* MKL_INT* */ IntPtr rows_end,
                                                 /* MKL_INT* */ IntPtr col_indx,
                                                 /* IntPtr */ IntPtr values);

        [DllImport(MKL_DLL, EntryPoint = "mkl_sparse_d_create_csc", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SparseStatus mkl_sparse_d_create_csc(/* sparse_matrix_t* */ ref IntPtr A,
                                                 SparseIndexBase indexing,
                                                 int rows,
                                                 int cols,
                                                 /* MKL_INT* */ IntPtr rows_start,
                                                 /* MKL_INT* */ IntPtr rows_end,
                                                 /* MKL_INT* */ IntPtr col_indx,
                                                 /* IntPtr */ IntPtr values);

        [DllImport(MKL_DLL, EntryPoint = "sparse_index_base", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SparseStatus mkl_sparse_c_create_csc(/* sparse_matrix_t* */ ref IntPtr A,
                                                 SparseIndexBase indexing,
                                                 int rows,
                                                 int cols,
                                                 /* MKL_INT* */ IntPtr rows_start,
                                                 /* MKL_INT* */ IntPtr rows_end,
                                                 /* MKL_INT* */ IntPtr col_indx,
                                                 /* MKL_Complex8**/ IntPtr values);

        [DllImport(MKL_DLL, EntryPoint = "mkl_sparse_z_create_csc", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SparseStatus mkl_sparse_z_create_csc(/* sparse_matrix_t* */ ref IntPtr A,
                                                 SparseIndexBase indexing,
                                                 int rows,
                                                 int cols,
                                                 /* MKL_INT* */ IntPtr rows_start,
                                                 /* MKL_INT* */ IntPtr rows_end,
                                                 /* MKL_INT* */ IntPtr col_indx,
                                                 /* MKL_Complex16* */ IntPtr values);

        // destroy matrix handle; if sparse matrix was stored inside the handle it also deallocates the matrix
        // It is user's responsibility not to delete the handle with the matrix, if this matrix is shared with other handles

        [DllImport(MKL_DLL, EntryPoint = "mkl_sparse_destroy", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SparseStatus mkl_sparse_destroy(/* sparse_matrix_t*/ IntPtr A);

        #endregion

        #region Extended Eigensolver

        [DllImport(MKL_DLL, EntryPoint = "mkl_sparse_ee_init", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SparseStatus mkl_sparse_ee_init(int[] pm);

        [DllImport(MKL_DLL, EntryPoint = "mkl_sparse_d_gv", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SparseStatus mkl_sparse_d_gv(StringBuilder which, int[] pm, /* sparse_matrix_t */ IntPtr A, MatrixDescriptor descrA, /* sparse_matrix_t */ IntPtr B, MatrixDescriptor descrB, int k0, ref int k, IntPtr E, IntPtr X, IntPtr res);

        [DllImport(MKL_DLL, EntryPoint = "mkl_sparse_s_gv", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SparseStatus mkl_sparse_s_gv(StringBuilder which, int[] pm, /* sparse_matrix_t */ IntPtr A, MatrixDescriptor descrA, /* sparse_matrix_t */ IntPtr B, MatrixDescriptor descrB, int k0, ref int k, IntPtr E, IntPtr X, IntPtr res);

        [DllImport(MKL_DLL, EntryPoint = "mkl_sparse_d_ev", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SparseStatus mkl_sparse_d_ev(StringBuilder which, int[] pm, /* sparse_matrix_t */ IntPtr A, MatrixDescriptor descrA, int k0, ref int k, IntPtr E, IntPtr X, IntPtr res);

        [DllImport(MKL_DLL, EntryPoint = "mkl_sparse_s_ev", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SparseStatus mkl_sparse_s_ev(StringBuilder which, int[] pm, /* sparse_matrix_t */ IntPtr A, MatrixDescriptor descrA, int k0, ref int k, IntPtr E, IntPtr X, IntPtr res);

        [DllImport(MKL_DLL, EntryPoint = "mkl_sparse_d_svd", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SparseStatus mkl_sparse_d_svd(StringBuilder whichE, StringBuilder whichV, int[] pm, /* sparse_matrix_t */ IntPtr A, MatrixDescriptor descrA, int k0, ref int k, IntPtr E, IntPtr XL, IntPtr XR, IntPtr res);

        [DllImport(MKL_DLL, EntryPoint = "mkl_sparse_s_svd", CallingConvention = CallingConvention.Cdecl)]
        internal static extern SparseStatus mkl_sparse_s_svd(StringBuilder whichE, StringBuilder whichV, int[] pm, /* sparse_matrix_t */ IntPtr A, MatrixDescriptor descrA, int k0, ref int k, IntPtr E, IntPtr X, IntPtr XR, IntPtr res);
        
        #endregion
    }
}
