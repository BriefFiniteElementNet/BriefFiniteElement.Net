namespace CSparse.Interop.SuiteSparse.Umfpack
{
    using System;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
#if SUITESPARSE_AIO
        const string UMFPACK_DLL = "libsuitesparse";
#else
        const string UMFPACK_DLL = "libumfpack";
#endif

        #region Double / int

        /// <summary>
        /// Converts a column-oriented matrix to a triplet form.
        /// </summary>
        /// <param name="n_col">Number of columns (A is an n_row-by-n_col matrix.  Restriction: n_col > 0.).</param>
        /// <param name="Ap">The column pointers of the column-oriented form of the matrix.</param>
        /// <param name="Tj">Integer array of size nz on input, where nz = Ap [n_col].</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_col_to_triplet", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_di_col_to_triplet(int n_col, int[] Ap, int[] Tj);

        /// <summary>
        /// Sets the default control parameter settings.
        /// </summary>
        /// <param name="Control">Control is set to the default control parameter settings.</param>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_defaults", CallingConvention = CallingConvention.Cdecl)]
        public static extern void umfpack_di_defaults(
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_CONTROL)] double[] Control
        );

        /// <summary>
        /// Deallocates the Numeric object and sets the Numeric handle to NULL. This
        /// routine is the only valid way of destroying the Numeric object.
        /// </summary>
        /// <param name="Numeric">Numeric points to a valid Numeric object, computed by umfpack_*_numeric.</param>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_free_numeric", CallingConvention = CallingConvention.Cdecl)]
        public static extern void umfpack_di_free_numeric(ref IntPtr Numeric);

        /// <summary>
        /// Deallocates the Symbolic object and sets the Symbolic handle to NULL. This
        /// routine is the only valid way of destroying the Symbolic object.
        /// </summary>
        /// <param name="Symbolic">Points to a valid Symbolic object computed by umfpack_*_symbolic.</param>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_free_symbolic", CallingConvention = CallingConvention.Cdecl)]
        public static extern void umfpack_di_free_symbolic(ref IntPtr Symbolic);

        /// <summary>
        /// Using the LU factors and the permutation vectors contained in the Numeric
        /// object, calculate the determinant of the matrix A.
        /// </summary>
        /// <param name="Mx">Array of size 1.</param>
        /// <param name="Ex">Array of size 1 (optional).</param>
        /// <param name="Numeric">Numeric must point to a valid Numeric object.</param>
        /// <param name="Info">Contains information about the calculation of the determinant.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_get_determinant", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_di_get_determinant(
            double[] Mx, double[] Ex, IntPtr Numeric,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_INFO)] double[] Info
        );

        /// <summary>
        /// Determines the size and number of nonzeros in the LU factors held by the Numeric object.
        /// </summary>
        /// <param name="lnz">The number of nonzeros in L, including the diagonal (which is all one's).</param>
        /// <param name="unz">The number of nonzeros in U, including the diagonal.</param>
        /// <param name="n_row"></param>
        /// <param name="n_col"></param>
        /// <param name="nz_udiag">The number of numerically nonzero values on the diagonal of U.</param>
        /// <param name="Numeric">Numeric must point to a valid Numeric object.</param>
        /// <returns>UMFPACK status code.</returns>
        /// <remarks>
        /// These are also the sizes of the output arrays required by umfpack_*_get_numeric.
        /// 
        /// The matrix L is n_row -by- min(n_row,n_col), with lnz nonzeros, including
        /// the entries on the unit diagonal of L.
        /// 
        /// The matrix U is min(n_row,n_col) -by- n_col, with unz nonzeros, including
        /// nonzeros on the diagonal of U.
        /// 
        /// The matrix is singular if nz_diag &lt; min(n_row,n_col). A divide-by-zero
        /// will occur if nz_diag &lt; n_row == n_col when solving a sparse system
        /// involving the matrix U in umfpack_*_*solve.
        /// </remarks>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_get_lunz", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_di_get_lunz(out int lnz, out int unz, out int n_row, out int n_col,
            out int nz_udiag, IntPtr Numeric);

        /// <summary>
        /// This routine copies the LU factors and permutation vectors from the Numeric
        /// object into user-accessible arrays.
        /// </summary>
        /// <param name="Lp">Lp [n_row+1]</param>
        /// <param name="Lj">Lj [lnz]</param>
        /// <param name="Lx">Lx [lnz]</param>
        /// <param name="Up">Up [n_col+1]</param>
        /// <param name="Ui">Ui [unz]</param>
        /// <param name="Ux">Ux [unz]</param>
        /// <param name="P">The permutation vector P is defined as P [k] = i, where the original row i of A is the kth pivot row in PAQ</param>
        /// <param name="Q">The permutation vector Q is defined as Q [k] = j, where the original column j of A is the kth pivot column in PAQ.</param>
        /// <param name="Dx">The diagonal of U is also returned in Dx.</param>
        /// <param name="do_recip">This argument defines how the scale factors Rs are to be interpretted.</param>
        /// <param name="Rs">The row scale factors are returned in Rs [0..n_row-1].</param>
        /// <param name="Numeric">Numeric must point to a valid Numeric object.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_get_numeric", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_di_get_numeric(
            int[] Lp, int[] Lj, double[] Lx,
            int[] Up, int[] Ui, double[] Ux,
            int[] P, int[] Q, double[] Dx,
            out int do_recip, double[] Rs, IntPtr Numeric
        );

        /// <summary>
        /// Copies the contents of the Symbolic object into simple integer arrays accessible to the user.
        /// </summary>
        /// <param name="n_row"></param>
        /// <param name="n_col"></param>
        /// <param name="n1">The number of pivots with zero Markowitz cost.</param>
        /// <param name="nz">The number of nonzeros in A.</param>
        /// <param name="nfr">The number of frontal matrices that will be used to factorize the matrix A.</param>
        /// <param name="nchains"></param>
        /// <param name="P">The initial row permutation P [n_row].</param>
        /// <param name="Q">The initial column permutation Q [n_col].</param>
        /// <param name="Front_npivcol">Front_npivcol [n_col+1]</param>
        /// <param name="Front_parent">Front_parent [n_col+1]</param>
        /// <param name="Front_1strow">Front_1strow [n_col+1]</param>
        /// <param name="Front_leftmostdesc">Front_leftmostdesc [n_col+1]</param>
        /// <param name="Chain_start">Chain_start [n_col+1]</param>
        /// <param name="Chain_maxrows">Chain_maxrows [n_col+1]</param>
        /// <param name="Chain_maxcols">Chain_maxcols [n_col+1]</param>
        /// <param name="Symbolic">The Symbolic object, which holds the symbolic factorization.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_get_symbolic", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_di_get_symbolic(
            out int n_row, out int n_col, out int n1, out int nz, out int nfr, out int nchains,
            int[] P, int[] Q,
            int[] Front_npivcol, int[] Front_parent, int[] Front_1strow, int[] Front_leftmostdesc,
            int[] Chain_start, int[] Chain_maxrows, int[] Chain_maxcols,
            IntPtr Symbolic
        );

        /// <summary>
        /// Loads a Numeric object from a file created by umfpack_*_save_numeric.
        /// </summary>
        /// <param name="Numeric">On output, this variable holds a pointer to the Numeric
        /// object (if successful), or NULL if a failure occurred.</param>
        /// <param name="filename">A string that contains the filename from which to read the Numeric object.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_load_numeric", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_di_load_numeric(out IntPtr Numeric, /*char* */ string filename);

        /// <summary>
        /// Loads a Symbolic object from a file created by umfpack_*_save_symbolic.
        /// </summary>
        /// <param name="Symbolic">On output, this variable holds a pointer to the Symbolic
        /// object (if successful), or NULL if a failure occurred.</param>
        /// <param name="filename">A string that contains the filename from which to read the Symbolic object.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_load_symbolic", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_di_load_symbolic(out IntPtr Symbolic, /*char* */ string filename);

        /// <summary>
        /// Given a sparse matrix A in column-oriented form, and a symbolic analysis
        /// computed by umfpack_*_*symbolic, the umfpack_*_numeric routine performs the
        /// numerical factorization
        /// </summary>
        /// <param name="Ap">This must be identical to the Ap array passed to umfpack_*_*symbolic.</param>
        /// <param name="Ai">This must be identical to the Ai array passed to umfpack_*_*symbolic.</param>
        /// <param name="Ax">The numerical values of the sparse matrix A.</param>
        /// <param name="Symbolic">The Symbolic object, which holds the symbolic factorization
        /// computed by umfpack_*_*symbolic.</param>
        /// <param name="Numeric">On output, this variable holds a pointer to the Numeric
        /// object (if successful), or NULL if a failure occurred.</param>
        /// <param name="Control">If a NULL pointer is passed, then the default control
        /// settings are used.  Otherwise, the settings are determined from the Control array.</param>
        /// <param name="Info"></param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_numeric", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_di_numeric(
            int[] Ap, int[] Ai, double[] Ax, IntPtr Symbolic, out IntPtr Numeric,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_CONTROL)] double[] Control,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_INFO)] double[] Info
        );

        /// <summary>
        /// Given the nonzero pattern of a sparse matrix A in column-oriented form, and
        /// a sparsity preserving column pre-ordering Q, umfpack_*_qsymbolic performs
        /// the symbolic factorization of A*Q.
        /// </summary>
        /// <param name="n_row"></param>
        /// <param name="n_col"></param>
        /// <param name="Ap"></param>
        /// <param name="Ai"></param>
        /// <param name="Ax"></param>
        /// <param name="Qinit">The user's fill-reducing initial column pre-ordering.</param>
        /// <param name="Symbolic"></param>
        /// <param name="Control"></param>
        /// <param name="Info"></param>
        /// <returns>UMFPACK status code.</returns>
        /// <remarks>
        /// This is identical to umfpack_*_symbolic, except that neither
        /// COLAMD nor AMD are called and the user input column order Qinit is used
        /// instead.  Note that in general, the Qinit passed to umfpack_*_qsymbolic
        /// can differ from the final Q found in umfpack_*_numeric.  The unsymmetric
        /// strategy will perform a column etree postordering done in
        /// umfpack_*_qsymbolic and sparsity-preserving modifications are made within
        /// each frontal matrix during umfpack_*_numeric.  The symmetric
        /// strategy will preserve Qinit, unless the matrix is structurally singular.
        ///
        /// *** WARNING ***  A poor choice of Qinit can easily cause umfpack_*_numeric
        /// to use a huge amount of memory and do a lot of work.  The "default" symbolic
        /// analysis method is umfpack_*_symbolic, not this routine.  If you use this
        /// routine, the performance of UMFPACK is your responsibility;  UMFPACK will
        /// not try to second-guess a poor choice of Qinit.
        /// </remarks>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_qsymbolic", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_di_qsymbolic(
            int n_row, int n_col, int[] Ap, int[] Ai, double[] Ax, int[] Qinit, out IntPtr Symbolic,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_CONTROL)] double[] Control,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_INFO)] double[] Info
        );

        /// <summary>
        /// Saves a Numeric object to a file, which can later be read by
        /// umfpack_*_load_numeric.  The Numeric object is not modified.
        /// </summary>
        /// <param name="Numeric">Numeric must point to a valid Numeric object</param>
        /// <param name="filename">A string that contains the filename to which the Numeric object is written.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_save_numeric", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_di_save_numeric(IntPtr Numeric, /*char* */ string filename);

        /// <summary>
        /// Saves a Symbolic object to a file, which can later be read by
        /// umfpack_*_load_symbolic.  The Symbolic object is not modified.
        /// </summary>
        /// <param name="Symbolic">Symbolic must point to a valid Symbolic object.</param>
        /// <param name="filename">A string that contains the filename to which the Symbolic object is written.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_save_symbolic", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_di_save_symbolic(IntPtr Symbolic, /*char* */ string filename);

        /// <summary>
        /// Given LU factors computed by umfpack_*_numeric (PAQ=LU, PRAQ=LU, or
        /// P(R\A)Q=LU), and a vector B, this routine computes X = B, X = R*B, or
        /// X = R\B, as appropriate.  X and B must be vectors equal in length to the
        /// number of rows of A.
        /// </summary>
        /// <param name="X">The output vector X [n_row].</param>
        /// <param name="B">The input vector B [n_row].</param>
        /// <param name="Numeric">Numeric must point to a valid Numeric object.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_scale", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_di_scale(double[] X, double[] B, IntPtr Numeric);

        /// <summary>
        /// Given LU factors computed by umfpack_*_numeric (PAQ=LU, PRAQ=LU, or
        /// P(R\A)Q=LU) and the right-hand-side, B, solve a linear system for the
        /// solution X.  Iterative refinement is optionally performed.
        /// </summary>
        /// <param name="sys">Defines which system to solve.</param>
        /// <param name="Ap">Ap [n+1]</param>
        /// <param name="Ai">Ai [nz]</param>
        /// <param name="Ax">Ax [nz]</param>
        /// <param name="X">X [n] The solution to the linear system, where n = n_row = n_col is the dimension of the matrices A, L, and U.</param>
        /// <param name="B">B [n] The right-hand side vector.</param>
        /// <param name="Numeric">Numeric must point to a valid Numeric object</param>
        /// <param name="Control"></param>
        /// <param name="Info"></param>
        /// <returns>UMFPACK status code.</returns>
        /// <remarks>
        /// Only square systems are handled. Singular matrices result in a divide-by-zero for all
        /// systems except those involving just the matrix L.  Iterative refinement is not performed
        /// for singular matrices. In the discussion below, n is equal to n_row and n_col, because
        /// only square systems are handled.
        /// 
        /// 
        /// The sys paramater defines which system to solve.  (') is the linear algebraic transpose
        /// (complex conjugate if A is complex), and (.') is the array transpose.
        ///
        ///     sys value	    system solved
        ///     UMFPACK_A       Ax=b
        ///     UMFPACK_At      A'x=b
        ///     UMFPACK_Aat     A.'x=b
        ///     UMFPACK_Pt_L    P'Lx=b
        ///     UMFPACK_L       Lx=b
        ///     UMFPACK_Lt_P    L'Px=b
        ///     UMFPACK_Lat_P   L.'Px=b
        ///     UMFPACK_Lt      L'x=b
        ///     UMFPACK_U_Qt    UQ'x=b
        ///     UMFPACK_U       Ux=b
        ///     UMFPACK_Q_Ut    QU'x=b
        ///     UMFPACK_Q_Uat   QU.'x=b
        ///     UMFPACK_Ut      U'x=b
        ///     UMFPACK_Uat     U.'x=b
        ///
        /// Iterative refinement can be optionally performed when sys is any of
        /// the following:
        ///
        ///     UMFPACK_A       Ax=b
        ///     UMFPACK_At      A'x=b
        ///     UMFPACK_Aat     A.'x=b
        ///
        /// For the other values of the sys argument, iterative refinement is not
        /// performed (Control [UMFPACK_IRSTEP], Ap, Ai, Ax, and Az are ignored).
        /// </remarks>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_solve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_di_solve(
            int sys, int[] Ap, int[] Ai, double[] Ax, double[] X, double[] B, IntPtr Numeric,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_CONTROL)] double[] Control,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_INFO)] double[] Info
        );

        /// <summary>
        /// Given nonzero pattern of a sparse matrix A in column-oriented form,
        /// umfpack_*_symbolic performs a column pre-ordering to reduce fill-in
        /// (using COLAMD, AMD or METIS) and a symbolic factorization.
        /// </summary>
        /// <param name="n_row">A is an n_row-by-n_col matrix.</param>
        /// <param name="n_col">A is an n_row-by-n_col matrix.</param>
        /// <param name="Ap">Ap is an integer array of size n_col+1.</param>
        /// <param name="Ai">The row indices Ai [nz].</param>
        /// <param name="Ax">The numerical values Ax [nz].</param>
        /// <param name="Symbolic">On output, this variable holds a pointer to the Symbolic
        /// object (if successful), or NULL if a failure occurred.</param>
        /// <param name="Control"></param>
        /// <param name="Info"></param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_symbolic", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_di_symbolic(
            int n_row, int n_col, int[] Ap, int[] Ai, double[] Ax, out IntPtr Symbolic,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_CONTROL)] double[] Control,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_INFO)] double[] Info
        );

        /// <summary>
        /// Transposes and optionally permutes a sparse matrix in row or column-form, R = (PAQ)'.
        /// </summary>
        /// <param name="n_row">A is an n_row-by-n_col matrix.</param>
        /// <param name="n_col">A is an n_row-by-n_col matrix.</param>
        /// <param name="Ap">Ap [n_col+1] The column pointers of the column-oriented form of the matrix A.</param>
        /// <param name="Ai">The row indices Ai [nz].</param>
        /// <param name="Ax">If present, these are the numerical values of the sparse matrix A.</param>
        /// <param name="P">The row permutation vector P [n_row] (optional).</param>
        /// <param name="Q">The column permutation vector Q [n_col] (optional).</param>
        /// <param name="Rp">The column pointers of the matrix R.</param>
        /// <param name="Ri">The row indices of the matrix R.</param>
        /// <param name="Rx">If present, these are the numerical values of the sparse matrix R.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_transpose", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_di_transpose(
            int n_row, int n_col, int[] Ap, int[] Ai, double[] Ax, int[] P, int[] Q,
            int[] Rp, int[] Ri, double[] Rx
        );

        /// <summary>
        /// Converts a sparse matrix from "triplet" form to compressed-column form.
        /// </summary>
        /// <param name="n_row">A is an n_row-by-n_col matrix.</param>
        /// <param name="n_col">A is an n_row-by-n_col matrix.</param>
        /// <param name="nz">The number of entries in the triplet form of the matrix.</param>
        /// <param name="Ti">The row indices of the "triplet" form of a sparse matrix.</param>
        /// <param name="Tj">The column indices of the "triplet" form of a sparse matrix.</param>
        /// <param name="Tx">The values of the "triplet" form of a sparse matrix.</param>
        /// <param name="Ap">Array of size n_col+1. On output, Ap holds the "pointers" for
        /// the column form of the sparse matrix A.</param>
        /// <param name="Ai">Array of size nz. Note that only the first Ap [n_col] entries are used.</param>
        /// <param name="Ax">Array of size nz. Note that only the first Ap [n_col] entries are used (optional).</param>
        /// <param name="Map">Array of size nz. If present, then on output it holds the position of the triplets in the column-form matrix.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_triplet_to_col", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_di_triplet_to_col
        (
            int n_row, int n_col, int nz,
            int[] Ti, int[] Tj, double[] Tx,
            int[] Ap, int[] Ai, double[] Ax,
            int[] Map
        );

        /// <summary>
        /// Given LU factors computed by umfpack_*_numeric (PAQ=LU) and the
        /// right-hand-side, B, solve a linear system for the solution X.  Iterative
        /// refinement is optionally performed.
        /// </summary>
        /// <param name="sys"></param>
        /// <param name="Ap"></param>
        /// <param name="Ai"></param>
        /// <param name="Ax"></param>
        /// <param name="X"></param>
        /// <param name="B"></param>
        /// <param name="Numeric"></param>
        /// <param name="Control"></param>
        /// <param name="Info"></param>
        /// <param name="Wi">Workspace size n.</param>
        /// <param name="W">Workspace size c*n, where c is defined below.</param>
        /// <returns>UMFPACK status code.</returns>
        /// <remarks>
        /// This routine is identical to umfpack_*_solve, except that it does not dynamically
        /// allocate any workspace. When you have many linear systems to solve, this routine is
        /// faster than umfpack_*_solve, since the workspace (Wi, W) needs to be allocated only
        /// once, prior to calling umfpack_*_wsolve.
        /// The size of W is given as:
        ///
        ///                no iter.	with iter.
        ///                refinement	refinement
        ///    real     	n		5*n
        ///    complex  	4*n		10*n
        /// </remarks>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_di_wsolve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_di_wsolve
        (
            int sys, int[] Ap, int[] Ai, double[] Ax, double[] X, double[] B, IntPtr Numeric,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_CONTROL)] double[] Control,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_INFO)] double[] Info,
            int[] Wi, double[] W
        );

        #endregion

        #region Complex / int

        /// <summary>
        /// Converts a column-oriented matrix to a triplet form.
        /// </summary>
        /// <param name="n_col">Number of columns (A is an n_row-by-n_col matrix.  Restriction: n_col > 0.).</param>
        /// <param name="Ap">The column pointers of the column-oriented form of the matrix.</param>
        /// <param name="Tj">Integer array of size nz on input, where nz = Ap [n_col].</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_col_to_triplet", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_col_to_triplet(int n_col, int[] Ap, int[] Tj);

        /// <summary>
        /// Sets the default control parameter settings.
        /// </summary>
        /// <param name="Control">Control is set to the default control parameter settings.</param>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_defaults", CallingConvention = CallingConvention.Cdecl)]
        public static extern void umfpack_zi_defaults(
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_CONTROL)] double[] Control
        );

        /// <summary>
        /// Deallocates the Numeric object and sets the Numeric handle to NULL. This
        /// routine is the only valid way of destroying the Numeric object.
        /// </summary>
        /// <param name="Numeric">Numeric points to a valid Numeric object, computed by umfpack_*_numeric.</param>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_free_numeric", CallingConvention = CallingConvention.Cdecl)]
        public static extern void umfpack_zi_free_numeric(ref IntPtr Numeric);

        /// <summary>
        /// Deallocates the Symbolic object and sets the Symbolic handle to NULL. This
        /// routine is the only valid way of destroying the Symbolic object.
        /// </summary>
        /// <param name="Symbolic">Points to a valid Symbolic object computed by umfpack_*_symbolic.</param>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_free_symbolic", CallingConvention = CallingConvention.Cdecl)]
        public static extern void umfpack_zi_free_symbolic(ref IntPtr Symbolic);

        /// <summary>
        /// Using the LU factors and the permutation vectors contained in the Numeric
        /// object, calculate the determinant of the matrix A.
        /// </summary>
        /// <param name="Mx">Array of size 1 or 2.</param>
        /// <param name="Mz">Array of size 1 (optional).</param>
        /// <param name="Ex">Array of size 1 (optional).</param>
        /// <param name="Numeric">Numeric must point to a valid Numeric object.</param>
        /// <param name="Info">Contains information about the calculation of the determinant.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_get_determinant", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_get_determinant(
            double[] Mx, double[] Mz, double[] Ex, IntPtr Numeric,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_INFO)] double[] Info
        );

        /// <summary>
        /// Determines the size and number of nonzeros in the LU factors held by the Numeric object.
        /// </summary>
        /// <param name="lnz">The number of nonzeros in L, including the diagonal (which is all one's).</param>
        /// <param name="unz">The number of nonzeros in U, including the diagonal.</param>
        /// <param name="n_row"></param>
        /// <param name="n_col"></param>
        /// <param name="nz_udiag">The number of numerically nonzero values on the diagonal of U.</param>
        /// <param name="Numeric">Numeric must point to a valid Numeric object.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_get_lunz", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_get_lunz(out int lnz, out int unz, out int n_row, out int n_col,
            out int nz_udiag, IntPtr Numeric);

        /// <summary>
        /// This routine copies the LU factors and permutation vectors from the Numeric
        /// object into user-accessible arrays.
        /// </summary>
        /// <param name="Lp">Lp [n_row+1]</param>
        /// <param name="Lj">Lj [lnz]</param>
        /// <param name="Lx">Lx [lnz]</param>
        /// <param name="Lz"></param>
        /// <param name="Up">Up [n_col+1]</param>
        /// <param name="Ui">Ui [unz]</param>
        /// <param name="Ux">Ux [unz]</param>
        /// <param name="Uz"></param>
        /// <param name="P">The permutation vector P is defined as P [k] = i, where the original row i of A is the kth pivot row in PAQ</param>
        /// <param name="Q">The permutation vector Q is defined as Q [k] = j, where the original column j of A is the kth pivot column in PAQ.</param>
        /// <param name="Dx">The diagonal of U is also returned in Dx.</param>
        /// <param name="Dz"></param>
        /// <param name="do_recip">This argument defines how the scale factors Rs are to be interpretted.</param>
        /// <param name="Rs">The row scale factors are returned in Rs [0..n_row-1].</param>
        /// <param name="Numeric">Numeric must point to a valid Numeric object.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_get_numeric", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_get_numeric(
            int[] Lp, int[] Lj, double[] Lx, double[] Lz,
            int[] Up, int[] Ui, double[] Ux, double[] Uz,
            int[] P, int[] Q, double[] Dx, double[] Dz,
            out int do_recip, double[] Rs, IntPtr Numeric
        );

        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_get_numeric", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_get_numeric(
            int[] Lp, int[] Lj, IntPtr Lx, IntPtr Lz,
            int[] Up, int[] Ui, IntPtr Ux, IntPtr Uz,
            int[] P, int[] Q, IntPtr Dx, IntPtr Dz,
            out int do_recip, double[] Rs, IntPtr Numeric
        );

        /// <summary>
        /// Copies the contents of the Symbolic object into simple integer arrays accessible to the user.
        /// </summary>
        /// <param name="n_row"></param>
        /// <param name="n_col"></param>
        /// <param name="n1">The number of pivots with zero Markowitz cost.</param>
        /// <param name="nz">The number of nonzeros in A.</param>
        /// <param name="nfr">The number of frontal matrices that will be used to factorize the matrix A.</param>
        /// <param name="nchains"></param>
        /// <param name="P">The initial row permutation P [n_row].</param>
        /// <param name="Q">The initial column permutation Q [n_col].</param>
        /// <param name="Front_npivcol">Front_npivcol [n_col+1]</param>
        /// <param name="Front_parent">Front_parent [n_col+1]</param>
        /// <param name="Front_1strow">Front_1strow [n_col+1]</param>
        /// <param name="Front_leftmostdesc">Front_leftmostdesc [n_col+1]</param>
        /// <param name="Chain_start">Chain_start [n_col+1]</param>
        /// <param name="Chain_maxrows">Chain_maxrows [n_col+1]</param>
        /// <param name="Chain_maxcols">Chain_maxcols [n_col+1]</param>
        /// <param name="Symbolic">The Symbolic object, which holds the symbolic factorization.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_get_symbolic", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_get_symbolic
        (
            out int n_row, out int n_col, out int n1, out int nz, out int nfr, out int nchains,
            int[] P, int[] Q,
            int[] Front_npivcol, int[] Front_parent, int[] Front_1strow, int[] Front_leftmostdesc,
            int[] Chain_start, int[] Chain_maxrows, int[] Chain_maxcols,
            IntPtr Symbolic
        );

        /// <summary>
        /// Loads a Numeric object from a file created by umfpack_*_save_numeric.
        /// </summary>
        /// <param name="Numeric">On output, this variable holds a pointer to the Numeric
        /// object (if successful), or NULL if a failure occurred.</param>
        /// <param name="filename">A string that contains the filename from which to read the Numeric object.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_load_numeric", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_load_numeric(out IntPtr Numeric, /*char* */ string filename);

        /// <summary>
        /// Loads a Symbolic object from a file created by umfpack_*_save_symbolic.
        /// </summary>
        /// <param name="Symbolic">On output, this variable holds a pointer to the Symbolic
        /// object (if successful), or NULL if a failure occurred.</param>
        /// <param name="filename">A string that contains the filename from which to read the Symbolic object.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_load_symbolic", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_load_symbolic(out IntPtr Symbolic, /*char* */ string filename);

        /// <summary>
        /// Given a sparse matrix A in column-oriented form, and a symbolic analysis
        /// computed by umfpack_*_*symbolic, the umfpack_*_numeric routine performs the
        /// numerical factorization
        /// </summary>
        /// <param name="Ap">This must be identical to the Ap array passed to umfpack_*_*symbolic.</param>
        /// <param name="Ai">This must be identical to the Ai array passed to umfpack_*_*symbolic.</param>
        /// <param name="Ax">The numerical values of the sparse matrix A.</param>
        /// <param name="Az"></param>
        /// <param name="Symbolic">The Symbolic object, which holds the symbolic factorization
        /// computed by umfpack_*_*symbolic.</param>
        /// <param name="Numeric">On output, this variable holds a pointer to the Numeric
        /// object (if successful), or NULL if a failure occurred.</param>
        /// <param name="Control">If a NULL pointer is passed, then the default control
        /// settings are used.  Otherwise, the settings are determined from the Control array.</param>
        /// <param name="Info"></param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_numeric", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_numeric(
            int[] Ap, int[] Ai, double[] Ax, double[] Az, IntPtr Symbolic, out IntPtr Numeric,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_CONTROL)] double[] Control,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_INFO)] double[] Info
        );

        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_numeric", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_numeric(
            int[] Ap, int[] Ai, IntPtr Ax, IntPtr Az, IntPtr Symbolic, out IntPtr Numeric,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_CONTROL)] double[] Control,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_INFO)] double[] Info
        );

        /// <summary>
        /// Given the nonzero pattern of a sparse matrix A in column-oriented form, and
        /// a sparsity preserving column pre-ordering Q, umfpack_*_qsymbolic performs
        /// the symbolic factorization of A*Q.
        /// </summary>
        /// <param name="n_row"></param>
        /// <param name="n_col"></param>
        /// <param name="Ap"></param>
        /// <param name="Ai"></param>
        /// <param name="Ax"></param>
        /// <param name="Az"></param>
        /// <param name="Qinit">The user's fill-reducing initial column pre-ordering.</param>
        /// <param name="Symbolic"></param>
        /// <param name="Control"></param>
        /// <param name="Info"></param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_qsymbolic", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_qsymbolic(
            int n_row, int n_col, int[] Ap, int[] Ai, double[] Ax, double[] Az, int[] Qinit, out IntPtr Symbolic,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_CONTROL)] double[] Control,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_INFO)] double[] Info
        );

        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_qsymbolic", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_qsymbolic(
            int n_row, int n_col, int[] Ap, int[] Ai, IntPtr Ax, IntPtr Az, int[] Qinit, out IntPtr Symbolic,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_CONTROL)] double[] Control,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_INFO)] double[] Info
        );

        /// <summary>
        /// Saves a Numeric object to a file, which can later be read by
        /// umfpack_*_load_numeric.  The Numeric object is not modified.
        /// </summary>
        /// <param name="Numeric">Numeric must point to a valid Numeric object</param>
        /// <param name="filename">A string that contains the filename to which the Numeric object is written.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_save_numeric", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_save_numeric(IntPtr Numeric, /*char* */ string filename);

        /// <summary>
        /// Saves a Symbolic object to a file, which can later be read by
        /// umfpack_*_load_symbolic.  The Symbolic object is not modified.
        /// </summary>
        /// <param name="Symbolic">Symbolic must point to a valid Symbolic object.</param>
        /// <param name="filename">A string that contains the filename to which the Symbolic object is written.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_save_symbolic", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_save_symbolic(IntPtr Symbolic, /*char* */ string filename);

        /// <summary>
        /// Given LU factors computed by umfpack_*_numeric (PAQ=LU, PRAQ=LU, or
        /// P(R\A)Q=LU), and a vector B, this routine computes X = B, X = R*B, or
        /// X = R\B, as appropriate.  X and B must be vectors equal in length to the
        /// number of rows of A.
        /// </summary>
        /// <param name="Xx">The output vector X [n_row].</param>
        /// <param name="Xz"></param>
        /// <param name="Bx">The input vector B [n_row].</param>
        /// <param name="Bz"></param>
        /// <param name="Numeric">Numeric must point to a valid Numeric object.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_scale", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_scale(double[] Xx, double[] Xz, double[] Bx, double[] Bz, IntPtr Numeric);

        /// <summary>
        /// Given LU factors computed by umfpack_*_numeric (PAQ=LU, PRAQ=LU, or
        /// P(R\A)Q=LU) and the right-hand-side, B, solve a linear system for the
        /// solution X.  Iterative refinement is optionally performed.
        /// </summary>
        /// <param name="sys">Defines which system to solve.</param>
        /// <param name="Ap">Ap [n+1]</param>
        /// <param name="Ai">Ai [nz]</param>
        /// <param name="Ax">Ax [nz]</param>
        /// <param name="Az"></param>
        /// <param name="Xx">X [n] The solution to the linear system, where n = n_row = n_col is the dimension of the matrices A, L, and U.</param>
        /// <param name="Xz"></param>
        /// <param name="Bx">B [n] The right-hand side vector.</param>
        /// <param name="Bz"></param>
        /// <param name="Numeric">Numeric must point to a valid Numeric object</param>
        /// <param name="Control"></param>
        /// <param name="Info"></param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_solve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_solve(
            int sys, int[] Ap, int[] Ai, double[] Ax, double[] Az,
            double[] Xx, double[] Xz, double[] Bx, double[] Bz,
            IntPtr Numeric,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_CONTROL)] double[] Control,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_INFO)] double[] Info
        );

        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_solve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_solve(
            int sys, int[] Ap, int[] Ai, IntPtr Ax, IntPtr Az,
            IntPtr Xx, IntPtr Xz, IntPtr Bx, IntPtr Bz,
            IntPtr Numeric,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_CONTROL)] double[] Control,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_INFO)] double[] Info
        );

        /// <summary>
        /// Given nonzero pattern of a sparse matrix A in column-oriented form,
        /// umfpack_*_symbolic performs a column pre-ordering to reduce fill-in
        /// (using COLAMD, AMD or METIS) and a symbolic factorization.
        /// </summary>
        /// <param name="n_row">A is an n_row-by-n_col matrix.</param>
        /// <param name="n_col">A is an n_row-by-n_col matrix.</param>
        /// <param name="Ap">Ap is an integer array of size n_col+1.</param>
        /// <param name="Ai">The row indices Ai [nz].</param>
        /// <param name="Ax">The numerical values Ax [nz].</param>
        /// <param name="Az"></param>
        /// <param name="Symbolic">On output, this variable holds a pointer to the Symbolic
        /// object (if successful), or NULL if a failure occurred.</param>
        /// <param name="Control"></param>
        /// <param name="Info"></param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_symbolic", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_symbolic(
            int n_row, int n_col, int[] Ap, int[] Ai, double[] Ax, double[] Az,
            out IntPtr Symbolic,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_CONTROL)] double[] Control,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_INFO)] double[] Info
        );

        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_symbolic", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_symbolic(
            int n_row, int n_col, int[] Ap, int[] Ai, IntPtr Ax, IntPtr Az,
            out IntPtr Symbolic,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_CONTROL)] double[] Control,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_INFO)] double[] Info
        );

        /// <summary>
        /// Transposes and optionally permutes a sparse matrix in row or column-form, R = (PAQ)'.
        /// </summary>
        /// <param name="n_row">A is an n_row-by-n_col matrix.</param>
        /// <param name="n_col">A is an n_row-by-n_col matrix.</param>
        /// <param name="Ap">Ap [n_col+1] The column pointers of the column-oriented form of the matrix A.</param>
        /// <param name="Ai">The row indices Ai [nz].</param>
        /// <param name="Ax">If present, these are the numerical values of the sparse matrix A.</param>
        /// <param name="Az"></param>
        /// <param name="P">The row permutation vector P [n_row] (optional).</param>
        /// <param name="Q">The column permutation vector Q [n_col] (optional).</param>
        /// <param name="Rp">The column pointers of the matrix R.</param>
        /// <param name="Ri">The row indices of the matrix R.</param>
        /// <param name="Rx">If present, these are the numerical values of the sparse matrix R.</param>
        /// <param name="Rz"></param>
        /// <param name="do_conjugate">If true, and if Ax and Rx are present, then the
        /// linear algebraic transpose is computed (complex conjugate).  If false, the
        /// array transpose is computed instead.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_transpose", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_transpose
        (
            int n_row, int n_col, int[] Ap, int[] Ai, double[] Ax, double[] Az,
            int[] P, int[] Q,
            int[] Rp, int[] Ri, double[] Rx, double[] Rz, int do_conjugate
        );

        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_transpose", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_transpose
        (
            int n_row, int n_col, int[] Ap, int[] Ai, IntPtr Ax, IntPtr Az,
            int[] P, int[] Q,
            int[] Rp, int[] Ri, IntPtr Rx, IntPtr Rz, int do_conjugate
        );

        /// <summary>
        /// Converts a sparse matrix from "triplet" form to compressed-column form.
        /// </summary>
        /// <param name="n_row">A is an n_row-by-n_col matrix.</param>
        /// <param name="n_col">A is an n_row-by-n_col matrix.</param>
        /// <param name="nz">The number of entries in the triplet form of the matrix.</param>
        /// <param name="Ti">The row indices of the "triplet" form of a sparse matrix.</param>
        /// <param name="Tj">The column indices of the "triplet" form of a sparse matrix.</param>
        /// <param name="Tx">The values of the "triplet" form of a sparse matrix.</param>
        /// <param name="Tz"></param>
        /// <param name="Ap">Array of size n_col+1. On output, Ap holds the "pointers" for
        /// the column form of the sparse matrix A.</param>
        /// <param name="Ai">Array of size nz. Note that only the first Ap [n_col] entries are used.</param>
        /// <param name="Ax">Array of size nz. Note that only the first Ap [n_col] entries are used (optional).</param>
        /// <param name="Az"></param>
        /// <param name="Map">Array of size nz. If present, then on output it holds the position of the triplets in the column-form matrix.</param>
        /// <returns>UMFPACK status code.</returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_triplet_to_col", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_triplet_to_col
        (
            int n_row, int n_col, int nz,
            int[] Ti, int[] Tj, double[] Tx, double[] Tz,
            int[] Ap, int[] Ai, double[] Ax, double[] Az,
            int[] Map
        );

        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_triplet_to_col", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_triplet_to_col
        (
            int n_row, int n_col, int nz,
            int[] Ti, int[] Tj, IntPtr Tx, IntPtr Tz,
            int[] Ap, int[] Ai, IntPtr Ax, IntPtr Az,
            int[] Map
        );

        /// <summary>
        /// Given LU factors computed by umfpack_*_numeric (PAQ=LU) and the
        /// right-hand-side, B, solve a linear system for the solution X.  Iterative
        /// refinement is optionally performed.
        /// </summary>
        /// <param name="sys"></param>
        /// <param name="Ap"></param>
        /// <param name="Ai"></param>
        /// <param name="Ax"></param>
        /// <param name="Az"></param>
        /// <param name="Xx"></param>
        /// <param name="Xz"></param>
        /// <param name="Bx"></param>
        /// <param name="Bz"></param>
        /// <param name="Numeric"></param>
        /// <param name="Control"></param>
        /// <param name="Info"></param>
        /// <param name="Wi"></param>
        /// <param name="W"></param>
        /// <returns></returns>
        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_wsolve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_wsolve
        (
            int sys, int[] Ap, int[] Ai, double[] Ax, double[] Az,
            double[] Xx, double[] Xz,
            double[] Bx, double[] Bz,
            IntPtr Numeric,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_CONTROL)] double[] Control,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_INFO)] double[] Info,
            int[] Wi, double[] W
        );

        [DllImport(UMFPACK_DLL, EntryPoint = "umfpack_zi_wsolve", CallingConvention = CallingConvention.Cdecl)]
        public static extern int umfpack_zi_wsolve
        (
            int sys, int[] Ap, int[] Ai, IntPtr Ax, IntPtr Az,
            IntPtr Xx, IntPtr Xz,
            IntPtr Bx, IntPtr Bz,
            IntPtr Numeric,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_CONTROL)] double[] Control,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = Constants.UMFPACK_INFO)] double[] Info,
            int[] Wi, double[] W
        );

        #endregion
    }
}
