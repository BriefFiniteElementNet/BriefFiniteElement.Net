namespace CSparse.Interop.SuiteSparse.Ordering
{
    using CSparse.Storage;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// AMD is a set of routines that implements the approximate minimum degree ordering
    /// algorithm to permute sparse matrices prior to numerical factorization.
    /// </summary>
    public class AMD
    {
        #region AMD constants

        private const int AMD_CONTROL = 5;
        private const int AMD_INFO = 20;

        private const int AMD_OK = 0;
        private const int AMD_OUT_OF_MEMORY = -1;
        private const int AMD_INVALID = -2;
        private const int AMD_OK_BUT_JUMBLED = 1;

        #endregion

        #region Publi properties (AMD options)

        /// <summary>
        /// Gets or sets a value used to determine whether or not a given input row is "dense" (default = 10.0).
        /// </summary>
        /// <remarks>
        /// A row is "dense" if the number of entries in the row exceeds (Dense * sqrt(n)), except that rows
        /// with 16 or fewer entries are never considered "dense".   To turn off the detection of dense rows,
        /// set (Dense) to a negative number, or to a number larger than sqrt(n).
        /// </remarks>
        public double Dense { get; set; } = 10.0;

        /// <summary>
        /// Gets or sets a value used to determine whether or not aggressive absorption is to be performed (default = true).
        /// </summary>
        public bool Agressive { get; set; } = true;

        #endregion

        #region Statistics

        public class Info
        {
            internal double[] data = new double[AMD_INFO];

            /// <summary>
            /// Gets the return value of amd_order.
            /// </summary>
            /// <remarks>
            ///  0 = success
            /// -1 = malloc failed, or problem too large
            /// -2 = input arguments are not valid
            ///  1 = input matrix is OK for amd_order, but columns were not sorted,
            ///      and/or duplicate entries were present. AMD had to do extra work
            ///      before ordering the matrix. This is a warning, not an error.
            /// </remarks>
            public int Status => (int)data[0];

            /// <summary>
            /// Gets the matrix size (A is n-by-n).
            /// </summary>
            public int N => (int)data[1];

            /// <summary>
            /// Gets the number of nonzeros in A.
            /// </summary>
            public int Nonzeros => (int)data[2];

            /// <summary>
            /// Gets the symmetry of pattern (1 = symmetric, 0 = unsymmetric).
            /// </summary>
            public double Symmetry => data[3];

            /// <summary>
            /// Gets the number of entries on diagonal.
            /// </summary>
            public int Diagonal => (int)data[4];

            /// <summary>
            /// 
            /// Gets the number of nonzeros in A+A'.
            /// </summary>
            public int NonzerosAplusAt => (int)data[5];

            /// <summary>
            /// Gets the number of "dense" rows/columns in A.
            /// </summary>
            public int Dense => (int)data[6];

            /// <summary>
            /// Gets the amount of memory used by AMD.
            /// </summary>
            public int Memory => (int)data[7];

            /// <summary>
            /// Gets the number of garbage collections in AMD.
            /// </summary>
            public int NCMPA => (int)data[8];

            /// <summary>
            /// Gets the approx. number of nonzeros in L, excluding the diagonal.
            /// </summary>
            public int LNZ => (int)data[9];

            /// <summary>
            /// Gets the number of floating point divides for LU and LDL'.
            /// </summary>
            public int NDIV => (int)data[10];

            /// <summary>
            /// Gets the number of floating point (*,-) pairs for LDL'.
            /// </summary>
            public int NMultSubsLDL => (int)data[11];

            /// <summary>
            /// Gets the number of floating point (*,-) pairs for LU.
            /// </summary>
            public int NMultSubsLU => (int)data[12];

            /// <summary>
            /// Gets the max. number of nonzeros in any column of L (incl. diagonal).
            /// </summary>
            public int DMAX => (int)data[13];
        }

        #endregion

        /// <summary>
        /// Generate a fill-reducing ordering using the AMD algorithm.
        /// </summary>
        /// <param name="A">The matrix.</param>
        /// <returns>The permutation vector or NULL, if an error occurred.</returns>
        public static int[] Generate<T>(CompressedColumnStorage<T> A)
            where T : struct, IEquatable<T>, IFormattable
        {
            int n = A.RowCount;

            if (n != A.ColumnCount)
            {
                throw new ArgumentException("Matrix must be square.", nameof(A));
            }

            var p = new int[n];

            var amd = new AMD();

            var info = amd.Order(n, A.ColumnPointers, A.RowIndices, p);

            if (info.Status < AMD_OK)
            {
                // error
                return null;
            }

            return p;
        }

        /// <summary>
        /// Generate a fill-reducing ordering using the CAMD algorithm.
        /// </summary>
        /// <param name="A">The matrix.</param>
        /// <param name="constraints">Constraint set of A.</param>
        /// <returns>The permutation vector or NULL, if an error occurred.</returns>
        public static int[] Generate<T>(CompressedColumnStorage<T> A, int[] constraints)
            where T : struct, IEquatable<T>, IFormattable
        {
            int n = A.RowCount;

            if (n != A.ColumnCount)
            {
                throw new ArgumentException("Matrix must be square.", nameof(A));
            }

            var p = new int[n];

            var amd = new AMD();

            var info = amd.Order(n, A.ColumnPointers, A.RowIndices, p, constraints);

            if (info.Status < AMD_OK)
            {
                // error
                return null;
            }

            return p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n">Number of rows and columns in A.</param>
        /// <param name="ai">Row indices of A.</param>
        /// <param name="ap">Column pointers of A.</param>
        /// <param name="p">The permutation vector (of size n).</param>
        /// <returns></returns>
        public Info Order(int n, int[] ap, int[] ai, int[] p)
        {
            var info = new Info();

            NativeMethods.amd_order(n, ap, ai, p, GetControl(), info.data);

            return info;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n">Number of rows and columns in A.</param>
        /// <param name="ai">Row indices of A.</param>
        /// <param name="ap">Column pointers of A.</param>
        /// <param name="p">The permutation vector (of size n).</param>
        /// <param name="c">Constraint set of A (of size n).</param>
        /// <returns></returns>
        public Info Order(int n, int[] ap, int[] ai, int[] p, int[] c)
        {
            var info = new Info();

            NativeMethods.camd_order(n, ap, ai, p, GetControl(), info.data, c);

            return info;
        }

        private double[] GetControl()
        {
            var a = new double[AMD_CONTROL];

            a[0] = Dense;
            a[1] = Agressive ? 1.0 : 0.0;

            return a;
        }

        static class NativeMethods
        {
#if SUITESPARSE_AIO
        const string AMD_DLL = "libsuitesparse";
#else
            const string AMD_DLL = "libamd";
#endif

            #region AMD

            /// <summary>
            /// Computes the approximate minimum degree ordering of an n-by-n matrix A.
            /// </summary>
            /// <param name="n">A is n-by-n.  n must be >= 0.</param>
            /// <param name="Ap">column pointers for A, of size n+1 (const)</param>
            /// <param name="Ai">row indices of A, of size nz = Ap [n] (const)</param>
            /// <param name="P">output permutation, of size n</param>
            /// <param name="Control">input Control settings, of size AMD_CONTROL</param>
            /// <param name="Info">output Info statistics, of size AMD_INFO</param>
            /// <returns></returns>
            [DllImport(AMD_DLL, EntryPoint = "amd_order", CallingConvention = CallingConvention.Cdecl)]
            public static extern int amd_order(
                int n,
                [In] int[] Ap,
                [In] int[] Ai,
                [In, Out] int[] P,
                [MarshalAs(UnmanagedType.LPArray, SizeConst = AMD_CONTROL)] double[] Control,
                [MarshalAs(UnmanagedType.LPArray, SizeConst = AMD_INFO)] double[] Info
            );

            /// <summary>
            /// Sets the default control parameters in the Control array.
            /// </summary>
            /// <param name="Control">Control array</param>
            /// <returns></returns>
            [DllImport(AMD_DLL, EntryPoint = "amd_defaults", CallingConvention = CallingConvention.Cdecl)]
            public static extern int amd_defaults(
                [MarshalAs(UnmanagedType.LPArray, SizeConst = AMD_CONTROL)]
                double[] Control
            );

            /// <summary>
            /// Prints a description of the control parameters, and their values.
            /// </summary>
            /// <param name="Control">Control array</param>
            /// <returns></returns>
            [DllImport(AMD_DLL, EntryPoint = "amd_control", CallingConvention = CallingConvention.Cdecl)]
            public static extern int amd_control(
                [MarshalAs(UnmanagedType.LPArray, SizeConst = AMD_CONTROL)]
                double[] Control
            );

            /// <summary>
            /// Prints a description of the statistics computed by AMD, and their values.
            /// </summary>
            /// <param name="Info">Info array</param>
            /// <returns></returns>
            [DllImport(AMD_DLL, EntryPoint = "amd_info", CallingConvention = CallingConvention.Cdecl)]
            public static extern int amd_info(
                [MarshalAs(UnmanagedType.LPArray, SizeConst = AMD_INFO)]
                double[] Info
            );

            /// <summary>
            /// Returns AMD_OK or AMD_OK_BUT_JUMBLED if the matrix is valid as input to amd order.
            /// </summary>
            /// <param name="n_row">A is n_row-by-n_col</param>
            /// <param name="n_col">A is n_row-by-n_col</param>
            /// <param name="Ap">column pointers of A, of size n_col+1 (const)</param>
            /// <param name="Ai">row indices of A, of size nz = Ap [n_col] (const)</param>
            /// <returns></returns>
            [DllImport(AMD_DLL, EntryPoint = "amd_valid", CallingConvention = CallingConvention.Cdecl)]
            public static extern int amd_valid(
                int n_row,
                int n_col,
                [In] int[] Ap,
                [In] int[] Ai
            );

            #endregion

            #region CAMD

            /// <summary>
            /// Computes the approximate minimum degree ordering of an n-by-n matrix A.
            /// </summary>
            /// <param name="n">A is n-by-n.  n must be >= 0.</param>
            /// <param name="Ap">column pointers for A, of size n+1 (const)</param>
            /// <param name="Ai">row indices of A, of size nz = Ap [n] (const)</param>
            /// <param name="P">output permutation, of size n</param>
            /// <param name="Control">input Control settings, of size AMD_CONTROL</param>
            /// <param name="Info">output Info statistics, of size AMD_INFO</param>
            /// <param name="C">Constraint set of A, of size n; can be NULL (const)</param>
            /// <returns></returns>
            [DllImport(AMD_DLL, EntryPoint = "camd_order", CallingConvention = CallingConvention.Cdecl)]
            public static extern int camd_order(
                int n,
                [In] int[] Ap,
                [In] int[] Ai,
                [In, Out] int[] P,
                [MarshalAs(UnmanagedType.LPArray, SizeConst = AMD_CONTROL)] double[] Control,
                [MarshalAs(UnmanagedType.LPArray, SizeConst = AMD_INFO)] double[] Info,
                [In, Out] int[] C
            );

            /// <summary>
            /// Sets the default control parameters in the Control array.
            /// </summary>
            /// <param name="Control">Control array</param>
            /// <returns></returns>
            [DllImport(AMD_DLL, EntryPoint = "camd_defaults", CallingConvention = CallingConvention.Cdecl)]
            public static extern int camd_defaults(
                [MarshalAs(UnmanagedType.LPArray, SizeConst = AMD_CONTROL)]
                double[] Control
            );

            /// <summary>
            /// Prints a description of the control parameters, and their values.
            /// </summary>
            /// <param name="Control">Control array</param>
            /// <returns></returns>
            [DllImport(AMD_DLL, EntryPoint = "camd_control", CallingConvention = CallingConvention.Cdecl)]
            public static extern int camd_control(
                [MarshalAs(UnmanagedType.LPArray, SizeConst = AMD_CONTROL)]
                double[] Control
            );

            /// <summary>
            /// Prints a description of the statistics computed by AMD, and their values.
            /// </summary>
            /// <param name="Info">Info array</param>
            /// <returns></returns>
            [DllImport(AMD_DLL, EntryPoint = "camd_info", CallingConvention = CallingConvention.Cdecl)]
            public static extern int camd_info(
                [MarshalAs(UnmanagedType.LPArray, SizeConst = AMD_INFO)]
                double[] Info
            );

            /// <summary>
            /// Returns AMD_OK or AMD_OK_BUT_JUMBLED if the matrix is valid as input to amd order.
            /// </summary>
            /// <param name="n_row">A is n_row-by-n_col</param>
            /// <param name="n_col">A is n_row-by-n_col</param>
            /// <param name="Ap">column pointers of A, of size n_col+1 (const)</param>
            /// <param name="Ai">row indices of A, of size nz = Ap [n_col] (const)</param>
            /// <returns></returns>
            [DllImport(AMD_DLL, EntryPoint = "camd_valid", CallingConvention = CallingConvention.Cdecl)]
            public static extern int camd_valid(
                int n_row,
                int n_col,
                [In] int[] Ap,
                [In] int[] Ai
            );

            /// <summary>
            /// Returns TRUE if the constraint set is valid as input to camd_order, FALSE otherwise.
            /// </summary>
            /// <param name="n"></param>
            /// <param name="C">Constraint set of A, of size n; can be NULL (const)</param>
            /// <returns></returns>
            [DllImport(AMD_DLL, EntryPoint = "camd_cvalid", CallingConvention = CallingConvention.Cdecl)]
            public static extern int camd_cvalid(
                int n,
                [In, Out] int[] C
            );

            #endregion
        }
    }
}
