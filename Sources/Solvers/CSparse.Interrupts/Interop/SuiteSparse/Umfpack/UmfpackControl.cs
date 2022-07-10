namespace CSparse.Interop.SuiteSparse.Umfpack
{
    #region Enums

    /// <summary>
    /// Factorization strategy (auto, symmetric, or unsymmetric).
    /// </summary>
    public enum UmfpackStrategy
    {
        /// <summary>
        /// Use symmetric or unsymmetric strategy.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// COLAMD(A), coletree postorder, not prefer diagonal.
        /// </summary>
        Unsymmetric = 1,
        /// <summary>
        /// AMD(A+A'), no coletree postorder, prefer diagonal.
        /// </summary>
        Symmetric = 3
    }

    /// <summary>
    /// Row scaling.
    /// </summary>
    public enum UmfpackScale
    {
        /// <summary>
        /// No scaling.
        /// </summary>
        None = 0,
        /// <summary>
        /// Default: divide each row by sum (abs(row)).
        /// </summary>
        Sum = 1,
        /// <summary>
        /// Divide each row by max (abs(row)).
        /// </summary>
        Max = 2
    }

    /// <summary>
    /// Ordering method to use.
    /// </summary>
    /// <remarks>
    /// AMD/COLAMD means: use AMD for symmetric strategy, COLAMD for unsymmetric.
    /// </remarks>
    public enum UmfpackOrdering
    {
        /// <summary>
        /// Use CHOLMOD (AMD/COLAMD then METIS)
        /// </summary>
        CHOLMOD = 0,
        /// <summary>
        /// Use AMD/COLAMD
        /// </summary>
        AMD = 1,
        /// <summary>
        /// User-provided Qinit
        /// </summary>
        Given = 2,
        /// <summary>
        /// Use METIS
        /// </summary>
        METIS = 3,
        /// <summary>
        /// Try many orderings, pick best
        /// </summary>
        Best = 4,
        /// <summary>
        /// Natural ordering
        /// </summary>
        None = 5
    }

    #endregion

    /* symbolic factorization:
     
            Control [UMFPACK_STRATEGY]:  This is the most important control
                parameter.  It determines what kind of ordering and pivoting
                strategy that UMFPACK should use.  There are 4 options:

                UMFPACK_STRATEGY_AUTO:  This is the default.  The input matrix is
                analyzed to determine how symmetric the nonzero pattern is, and
                how many entries there are on the diagonal.  It then selects one
                of the following strategies.  Refer to the User Guide for a
                description of how the strategy is automatically selected.

                UMFPACK_STRATEGY_UNSYMMETRIC:  Use the unsymmetric strategy.  COLAMD
                is used to order the columns of A, followed by a postorder of
                the column elimination tree.  No attempt is made to perform
                diagonal pivoting.  The column ordering is refined during
                factorization.

                In the numerical factorization, the
                Control [UMFPACK_SYM_PIVOT_TOLERANCE] parameter is ignored.  A
                pivot is selected if its magnitude is >=
                Control [UMFPACK_PIVOT_TOLERANCE] (default 0.1) times the
                largest entry in its column.

                UMFPACK_STRATEGY_SYMMETRIC:  Use the symmetric strategy
                In this method, the approximate minimum degree
                ordering (AMD) is applied to A+A', followed by a postorder of
                the elimination tree of A+A'.  UMFPACK attempts to perform
                diagonal pivoting during numerical factorization.  No refinement
                of the column pre-ordering is performed during factorization.

                In the numerical factorization, a nonzero entry on the diagonal
                is selected as the pivot if its magnitude is >= Control
                [UMFPACK_SYM_PIVOT_TOLERANCE] (default 0.001) times the largest
                entry in its column.  If this is not acceptable, then an
                off-diagonal pivot is selected with magnitude >= Control
                [UMFPACK_PIVOT_TOLERANCE] (default 0.1) times the largest entry
                in its column.

            Control [UMFPACK_ORDERING]:  The ordering method to use:
                    UMFPACK_ORDERING_CHOLMOD    try AMD/COLAMD, then METIS if needed
                    UMFPACK_ORDERING_AMD        just AMD or COLAMD
                    UMFPACK_ORDERING_GIVEN      just Qinit (umfpack_*_qsymbolic only)
                    UMFPACK_ORDERING_NONE       no fill-reducing ordering
                    UMFPACK_ORDERING_METIS      just METIS(A+A') or METIS(A'A)
                    UMFPACK_ORDERING_BEST       try AMD/COLAMD, METIS, and NESDIS
                    UMFPACK_ORDERING_USER       just user function (*_fsymbolic only)

                Control [UMFPACK_SINGLETONS]: If false (0), then singletons are
                    not removed prior to factorization.  Default: true (1).

            Control [UMFPACK_DENSE_COL]:
                If COLAMD is used, columns with more than
                max (16, Control [UMFPACK_DENSE_COL] * 16 * sqrt (n_row)) entries
                are placed placed last in the column pre-ordering.  Default: 0.2.

            Control [UMFPACK_DENSE_ROW]:
                Rows with more than max (16, Control [UMFPACK_DENSE_ROW] * 16 *
                sqrt (n_col)) entries are treated differently in the COLAMD
                pre-ordering, and in the internal data structures during the
                subsequent numeric factorization.  Default: 0.2.

            Control [UMFPACK_AMD_DENSE]:  rows/columns in A+A' with more than
                max (16, Control [UMFPACK_AMD_DENSE] * sqrt (n)) entries
                (where n = n_row = n_col) are ignored in the AMD pre-ordering.
                Default: 10.

            Control [UMFPACK_BLOCK_SIZE]:  the block size to use for Level-3 BLAS
                in the subsequent numerical factorization (umfpack_*_numeric).
                A value less than 1 is treated as 1.  Default: 32.  Modifying this
                parameter affects when updates are applied to the working frontal
                matrix, and can indirectly affect fill-in and operation count.
                Assuming the block size is large enough (8 or so), this parameter
                has a modest effect on performance.

            Control [UMFPACK_FIXQ]:  If > 0, then the pre-ordering Q is not modified
                during numeric factorization.  If < 0, then Q may be modified.  If
                zero, then this is controlled automatically (the unsymmetric
                strategy modifies Q, the others do not).  Default: 0.

            Control [UMFPACK_AGGRESSIVE]:  If nonzero, aggressive absorption is used
                in COLAMD and AMD.  Default: 1.
     */

    /* numeric factorization:
            Control [UMFPACK_PIVOT_TOLERANCE]:  relative pivot tolerance for
                threshold partial pivoting with row interchanges.  In any given
                column, an entry is numerically acceptable if its absolute value is
                greater than or equal to Control [UMFPACK_PIVOT_TOLERANCE] times
                the largest absolute value in the column.  A value of 1.0 gives true
                partial pivoting.  If less than or equal to zero, then any nonzero
                entry is numerically acceptable as a pivot.  Default: 0.1.

                Smaller values tend to lead to sparser LU factors, but the solution
                to the linear system can become inaccurate.  Larger values can lead
                to a more accurate solution (but not always), and usually an
                increase in the total work.

                For complex matrices, a cheap approximate of the absolute value
                is used for the threshold partial pivoting test (|a_real| + |a_imag|
                instead of the more expensive-to-compute exact absolute value
                sqrt (a_real^2 + a_imag^2)).

            Control [UMFPACK_SYM_PIVOT_TOLERANCE]:
                If diagonal pivoting is attempted (the symmetric
                strategy is used) then this parameter is used to control when the
                diagonal entry is selected in a given pivot column.  The absolute
                value of the entry must be >= Control [UMFPACK_SYM_PIVOT_TOLERANCE]
                times the largest absolute value in the column.  A value of zero
                will ensure that no off-diagonal pivoting is performed, except that
                zero diagonal entries are not selected if there are any off-diagonal
                nonzero entries.

                If an off-diagonal pivot is selected, an attempt is made to restore
                symmetry later on.  Suppose A (i,j) is selected, where i != j.
                If column i has not yet been selected as a pivot column, then
                the entry A (j,i) is redefined as a "diagonal" entry, except that
                the tighter tolerance (Control [UMFPACK_PIVOT_TOLERANCE]) is
                applied.  This strategy has an effect similar to 2-by-2 pivoting
                for symmetric indefinite matrices.  If a 2-by-2 block pivot with
                nonzero structure

                       i j
                    i: 0 x
                    j: x 0

                is selected in a symmetric indefinite factorization method, the
                2-by-2 block is inverted and a rank-2 update is applied.  In
                UMFPACK, this 2-by-2 block would be reordered as

                       j i
                    i: x 0
                    j: 0 x

                In both cases, the symmetry of the Schur complement is preserved.

            Control [UMFPACK_SCALE]:  Note that the user's input matrix is
                never modified, only an internal copy is scaled.

                There are three valid settings for this parameter.  If any other
                value is provided, the default is used.

                UMFPACK_SCALE_NONE:  no scaling is performed.

                UMFPACK_SCALE_SUM:  each row of the input matrix A is divided by
                the sum of the absolute values of the entries in that row.
                The scaled matrix has an infinity norm of 1.

                UMFPACK_SCALE_MAX:  each row of the input matrix A is divided by
                the maximum the absolute values of the entries in that row.
                In the scaled matrix the largest entry in each row has
                a magnitude exactly equal to 1.

                Note that for complex matrices, a cheap approximate absolute value
                is used, |a_real| + |a_imag|, instead of the exact absolute value
                sqrt ((a_real)^2 + (a_imag)^2).

                Scaling is very important for the "symmetric" strategy when
                diagonal pivoting is attempted.  It also improves the performance
                of the "unsymmetric" strategy.

                Default: UMFPACK_SCALE_SUM.

            Control [UMFPACK_ALLOC_INIT]:

                When umfpack_*_numeric starts, it allocates memory for the Numeric
                object.  Part of this is of fixed size (approximately n double's +
                12*n integers).  The remainder is of variable size, which grows to
                hold the LU factors and the frontal matrices created during
                factorization.  A estimate of the upper bound is computed by
                umfpack_*_*symbolic, and returned by umfpack_*_*symbolic in
                Info [UMFPACK_VARIABLE_PEAK_ESTIMATE] (in Units).

                If Control [UMFPACK_ALLOC_INIT] is >= 0, umfpack_*_numeric initially
                allocates space for the variable-sized part equal to this estimate
                times Control [UMFPACK_ALLOC_INIT].  Typically, for matrices for
                which the "unsymmetric" strategy applies, umfpack_*_numeric needs
                only about half the estimated memory space, so a setting of 0.5 or
                0.6 often provides enough memory for umfpack_*_numeric to factorize
                the matrix with no subsequent increases in the size of this block.

                If the matrix is ordered via AMD, then this non-negative parameter
                is ignored.  The initial allocation ratio computed automatically,
                as 1.2 * (nz + Info [UMFPACK_SYMMETRIC_LUNZ]) /
                (Info [UMFPACK_LNZ_ESTIMATE] + Info [UMFPACK_UNZ_ESTIMATE] -
                min (n_row, n_col)).

                If Control [UMFPACK_ALLOC_INIT] is negative, then umfpack_*_numeric
                allocates a space with initial size (in Units) equal to
                (-Control [UMFPACK_ALLOC_INIT]).

                Regardless of the value of this parameter, a space equal to or
                greater than the the bare minimum amount of memory needed to start
                the factorization is always initially allocated.  The bare initial
                memory required is returned by umfpack_*_*symbolic in
                Info [UMFPACK_VARIABLE_INIT_ESTIMATE] (an exact value, not an
                estimate).

                If the variable-size part of the Numeric object is found to be too
                small sometime after numerical factorization has started, the memory
                is increased in size by a factor of 1.2.   If this fails, the
                request is reduced by a factor of 0.95 until it succeeds, or until
                it determines that no increase in size is possible.  Garbage
                collection then occurs.

                The strategy of attempting to "malloc" a working space, and
                re-trying with a smaller space, may not work when UMFPACK is used
                as a mexFunction MATLAB, since mxMalloc aborts the mexFunction if it
                fails.  This issue does not affect the use of UMFPACK as a part of
                the built-in x=A\b in MATLAB 6.5 and later.

                If you are using the umfpack mexFunction, decrease the magnitude of
                Control [UMFPACK_ALLOC_INIT] if you run out of memory in MATLAB.

                Default initial allocation size: 0.7.  Thus, with the default
                control settings and the "unsymmetric" strategy, the upper-bound is
                reached after two reallocations (0.7 * 1.2 * 1.2 = 1.008).

                Changing this parameter has little effect on fill-in or operation
                count.  It has a small impact on run-time (the extra time required
                to do the garbage collection and memory reallocation).

            Control [UMFPACK_FRONT_ALLOC_INIT]:

                When UMFPACK starts the factorization of each "chain" of frontal
                matrices, it allocates a working array to hold the frontal matrices
                as they are factorized.  The symbolic factorization computes the
                size of the largest possible frontal matrix that could occur during
                the factorization of each chain.

                If Control [UMFPACK_FRONT_ALLOC_INIT] is >= 0, the following
                strategy is used.  If the AMD ordering was used, this non-negative
                parameter is ignored.  A front of size (d+2)*(d+2) is allocated,
                where d = Info [UMFPACK_SYMMETRIC_DMAX].  Otherwise, a front of
                size Control [UMFPACK_FRONT_ALLOC_INIT] times the largest front
                possible for this chain is allocated.

                If Control [UMFPACK_FRONT_ALLOC_INIT] is negative, then a front of
                size (-Control [UMFPACK_FRONT_ALLOC_INIT]) is allocated (where the
                size is in terms of the number of numerical entries).  This is done
                regardless of the ordering method or ordering strategy used.

                Default: 0.5.

            Control [UMFPACK_DROPTOL]:

                Entries in L and U with absolute value less than or equal to the
                drop tolerance are removed from the data structures (unless leaving
                them there reduces memory usage by reducing the space required
                for the nonzero pattern of L and U).

                Default: 0.0.
     */

    /// <summary>
    /// UMFPACK control.
    /// </summary>
    public class UmfpackControl
    {
        internal double[] Raw;

        public UmfpackControl()
        {
            Raw = new double[Constants.UMFPACK_CONTROL];

            LoadDefaults();
        }

        #region Public properties

        /// <summary>
        /// Gets or sets the print level.
        /// </summary>
        public int PrintLevel
        {
            get { return (int)Raw[0]; }
            set { Raw[0] = (int)value; }
        }

        // Used in UMFPACK_*symbolic only:

        /// <summary>
        /// Gets or sets the dense row parameter for COLAMD.
        /// </summary>
        public double DenseRow
        {
            get { return Raw[1]; }
            set { Raw[1] = value; }
        }

        /// <summary>
        /// Gets or sets the dense column parameter for COLAMD.
        /// </summary>
        public double DenseColumn
        {
            get { return Raw[2]; }
            set { Raw[2] = value; }
        }

        /// <summary>
        /// Gets or sets the BLAS-3 block size.
        /// </summary>
        public int BlockSize
        {
            get { return (int)Raw[4]; }
            set { Raw[4] = (int)value; }
        }

        /// <summary>
        /// Gets or sets the factorization strategy.
        /// </summary>
        public UmfpackStrategy Strategy
        {
            get { return (UmfpackStrategy)(int)Raw[5]; }
            set { Raw[5] = (int)value; }
        }

        /// <summary>
        /// Gets or sets the ordering method to use.
        /// </summary>
        public UmfpackOrdering Ordering
        {
            get { return (UmfpackOrdering)(int)Raw[10]; }
            set { Raw[10] = (int)value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pre-ordering Q may be modified during factorization (-1: no fixQ, 0: default, 1: fixQ).
        /// </summary>
        public int FixQ
        {
            get { return (int)Raw[13]; }
            set { Raw[13] = (int)value; }
        }

        /// <summary>
        /// Gets or sets the dense row/column threshold for AMD ordering.
        /// </summary>
        public double AMD_Dense
        {
            get { return Raw[14]; }
            set { Raw[14] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether aggressive absorption is used in COLAMD and AMD.
        /// </summary>
        public bool Aggressive
        {
            get { return Raw[19] > 0; }
            set { Raw[19] = value ? 1 : 0; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether singletons are removed prior to factorization.
        /// </summary>
        public bool Singletons
        {
            get { return Raw[11] > 0; }
            set { Raw[11] = value ? 1 : 0; }
        }

        // Used in UMFPACK_numeric only:

        /// <summary>
        /// Gets or sets the partial pivoting tolerance.
        /// </summary>
        public double PivotTolerance
        {
            get { return Raw[3]; }
            set { Raw[3] = value; }
        }

        /// <summary>
        /// Gets or sets the initial allocation ratio.
        /// </summary>
        public double AllocInit
        {
            get { return Raw[6]; }
            set { Raw[6] = value; }
        }

        /// <summary>
        /// Gets or sets the threshold, if diagonal pivoting is attempted (symmetric strategy).
        /// </summary>
        public double SymPivotTolerance
        {
            get { return Raw[15]; }
            set { Raw[15] = value; }
        }

        /// <summary>
        /// Gets or sets the row scaling strategy.
        /// </summary>
        public UmfpackScale Scale
        {
            get { return (UmfpackScale)(int)Raw[16]; }
            set { Raw[16] = (int)value; }
        }

        /// <summary>
        /// Gets or sets the frontal matrix allocation ratio.
        /// </summary>
        public double FrontAllocInit
        {
            get { return Raw[17]; }
            set { Raw[17] = value; }
        }

        /// <summary>
        /// Gets or sets the drop tolerance for entries in L and U.
        /// </summary>
        public double DropTolerance
        {
            get { return Raw[18]; }
            set { Raw[18] = value; }
        }

        // used in UMFPACK_*solve only:

        /// <summary>
        /// Gets or sets the maximum number of iterative refinements.
        /// </summary>
        public int IterativeRefinement
        {
            get { return (int)Raw[7]; }
            set { Raw[7] = (int)value; }
        }

        #endregion

        private void LoadDefaults()
        {
            Raw[UMFPACK_PRL] = 1;
            Raw[UMFPACK_DENSE_ROW] = 0.2;
            Raw[UMFPACK_DENSE_COL] = 0.2;
            Raw[UMFPACK_PIVOT_TOLERANCE] = 0.1;
            Raw[UMFPACK_SYM_PIVOT_TOLERANCE] = 0.001;
            Raw[UMFPACK_BLOCK_SIZE] = 32;
            Raw[UMFPACK_ALLOC_INIT] = 0.7;
            Raw[UMFPACK_FRONT_ALLOC_INIT] = 0.5;
            Raw[UMFPACK_IRSTEP] = 2;
            Raw[UMFPACK_SCALE] = (int)UmfpackScale.Sum;
            Raw[UMFPACK_STRATEGY] = (int)UmfpackStrategy.Auto;
            Raw[UMFPACK_AMD_DENSE] = 10.0; // See amd.h: AMD_DEFAULT_DENSE
            Raw[UMFPACK_FIXQ] = 0;
            Raw[UMFPACK_AGGRESSIVE] = 1;
            Raw[UMFPACK_DROPTOL] = 0;
            Raw[UMFPACK_ORDERING] = (int)UmfpackOrdering.AMD;
            Raw[UMFPACK_SINGLETONS] = 1;
        }

        #region Constants

        private const int UMFPACK_PRL = 0;
        private const int UMFPACK_DENSE_ROW = 1;
        private const int UMFPACK_DENSE_COL = 2;
        private const int UMFPACK_BLOCK_SIZE = 4;
        private const int UMFPACK_STRATEGY = 5;
        private const int UMFPACK_ORDERING = 10;
        private const int UMFPACK_FIXQ = 13;
        private const int UMFPACK_AMD_DENSE = 14;
        private const int UMFPACK_AGGRESSIVE = 19;
        private const int UMFPACK_SINGLETONS = 11;
        private const int UMFPACK_PIVOT_TOLERANCE = 3;
        private const int UMFPACK_ALLOC_INIT = 6;
        private const int UMFPACK_SYM_PIVOT_TOLERANCE = 15;
        private const int UMFPACK_SCALE = 16;
        private const int UMFPACK_FRONT_ALLOC_INIT = 17;
        private const int UMFPACK_DROPTOL = 18;
        private const int UMFPACK_IRSTEP = 7;

        #endregion
    }
}
