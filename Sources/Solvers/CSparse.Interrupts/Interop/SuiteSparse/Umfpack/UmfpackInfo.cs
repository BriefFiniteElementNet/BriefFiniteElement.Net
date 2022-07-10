namespace CSparse.Interop.SuiteSparse.Umfpack
{
    using System;

    /* symbolic factorization:
     
            Info [UMFPACK_STATUS]: status code.  This is also the return value,
                whether or not Info is present.

                UMFPACK_OK

                Each column of the input matrix contained row indices
                in increasing order, with no duplicates.  Only in this case
                does umfpack_*_symbolic compute a valid symbolic factorization.
                For the other cases below, no Symbolic object is created
                (*Symbolic is (void *) NULL).

                UMFPACK_ERROR_n_nonpositive

                n is less than or equal to zero.

                UMFPACK_ERROR_invalid_matrix

                Number of entries in the matrix is negative, Ap [0] is nonzero,
                a column has a negative number of entries, a row index is out of
                bounds, or the columns of input matrix were jumbled (unsorted
                columns or duplicate entries).

                UMFPACK_ERROR_out_of_memory

                Insufficient memory to perform the symbolic analysis.  If the
                analysis requires more than 2GB of memory and you are using
                the 32-bit ("int") version of UMFPACK, then you are guaranteed
                to run out of memory.  Try using the 64-bit version of UMFPACK.

                UMFPACK_ERROR_argument_missing

                One or more required arguments is missing.

                UMFPACK_ERROR_internal_error

                Something very serious went wrong.  This is a bug.
                Please contact the author (DrTimothyAldenDavis@gmail.com).

            Info [UMFPACK_NROW]:  the value of the input argument n_row.

            Info [UMFPACK_NCOL]:  the value of the input argument n_col.

            Info [UMFPACK_NZ]:  the number of entries in the input matrix
                (Ap [n_col]).

            Info [UMFPACK_SIZE_OF_UNIT]:  the number of bytes in a Unit,
                for memory usage statistics below.

            Info [UMFPACK_SIZE_OF_INT]:  the number of bytes in an int.

            Info [UMFPACK_SIZE_OF_LONG]:  the number of bytes in a SuiteSparse_long.

            Info [UMFPACK_SIZE_OF_POINTER]:  the number of bytes in a void *
                pointer.

            Info [UMFPACK_SIZE_OF_ENTRY]:  the number of bytes in a numerical entry.

            Info [UMFPACK_NDENSE_ROW]:  number of "dense" rows in A.  These rows are
                ignored when the column pre-ordering is computed in COLAMD.  They
                are also treated differently during numeric factorization.  If > 0,
                then the matrix had to be re-analyzed by UMF_analyze, which does
                not ignore these rows.

            Info [UMFPACK_NEMPTY_ROW]:  number of "empty" rows in A, as determined
                These are rows that either have no entries, or whose entries are
                all in pivot columns of zero-Markowitz-cost pivots.

            Info [UMFPACK_NDENSE_COL]:  number of "dense" columns in A.  COLAMD
                orders these columns are ordered last in the factorization, but
                before "empty" columns.

            Info [UMFPACK_NEMPTY_COL]:  number of "empty" columns in A.  These are
                columns that either have no entries, or whose entries are all in
                pivot rows of zero-Markowitz-cost pivots.  These columns are
                ordered last in the factorization, to the right of "dense" columns.

            Info [UMFPACK_SYMBOLIC_DEFRAG]:  number of garbage collections
                performed during ordering and symbolic pre-analysis.

            Info [UMFPACK_SYMBOLIC_PEAK_MEMORY]:  the amount of memory (in Units)
                required for umfpack_*_symbolic to complete.  This count includes
                the size of the Symbolic object itself, which is also reported in
                Info [UMFPACK_SYMBOLIC_SIZE].

            Info [UMFPACK_SYMBOLIC_SIZE]: the final size of the Symbolic object (in
                Units).  This is fairly small, roughly 2*n to 13*n integers,
                depending on the matrix.

            Info [UMFPACK_VARIABLE_INIT_ESTIMATE]: the Numeric object contains two
                parts.  The first is fixed in size (O (n_row+n_col)).  The
                second part holds the sparse LU factors and the contribution blocks
                from factorized frontal matrices.  This part changes in size during
                factorization.  Info [UMFPACK_VARIABLE_INIT_ESTIMATE] is the exact
                size (in Units) required for this second variable-sized part in
                order for the numerical factorization to start.

            Info [UMFPACK_VARIABLE_PEAK_ESTIMATE]: the estimated peak size (in
                Units) of the variable-sized part of the Numeric object.  This is
                usually an upper bound, but that is not guaranteed.

            Info [UMFPACK_VARIABLE_FINAL_ESTIMATE]: the estimated final size (in
                Units) of the variable-sized part of the Numeric object.  This is
                usually an upper bound, but that is not guaranteed.  It holds just
                the sparse LU factors.

            Info [UMFPACK_NUMERIC_SIZE_ESTIMATE]:  an estimate of the final size (in
                Units) of the entire Numeric object (both fixed-size and variable-
                sized parts), which holds the LU factorization (including the L, U,
                P and Q matrices).

            Info [UMFPACK_PEAK_MEMORY_ESTIMATE]:  an estimate of the total amount of
                memory (in Units) required by umfpack_*_symbolic and
                umfpack_*_numeric to perform both the symbolic and numeric
                factorization.  This is the larger of the amount of memory needed
                in umfpack_*_numeric itself, and the amount of memory needed in
                umfpack_*_symbolic (Info [UMFPACK_SYMBOLIC_PEAK_MEMORY]).  The
                count includes the size of both the Symbolic and Numeric objects
                themselves.  It can be a very loose upper bound, particularly when
                the symmetric strategy is used.

            Info [UMFPACK_FLOPS_ESTIMATE]:  an estimate of the total floating-point
                operations required to factorize the matrix.  This is a "true"
                theoretical estimate of the number of flops that would be performed
                by a flop-parsimonious sparse LU algorithm.  It assumes that no
                extra flops are performed except for what is strictly required to
                compute the LU factorization.  It ignores, for example, the flops
                    performed by umfpack_di_numeric to add contribution blocks of
                frontal matrices together.  If L and U are the upper bound on the
                pattern of the factors, then this flop count estimate can be
                represented in MATLAB (for real matrices, not complex) as:

                Lnz = full (sum (spones (L))) - 1 ;	% nz in each col of L
                Unz = full (sum (spones (U')))' - 1 ;	% nz in each row of U
                flops = 2*Lnz*Unz + sum (Lnz) ;

                The actual "true flop" count found by umfpack_*_numeric will be
                less than this estimate.

                For the real version, only (+ - * /) are counted.  For the complex
                version, the following counts are used:

                operation	flops
                    c = 1/b		6
                c = a*b		6
                c -= a*b	8

            Info [UMFPACK_LNZ_ESTIMATE]:  an estimate of the number of nonzeros in
                L, including the diagonal.  Since L is unit-diagonal, the diagonal
                of L is not stored.  This estimate is a strict upper bound on the
                actual nonzeros in L to be computed by umfpack_*_numeric.

            Info [UMFPACK_UNZ_ESTIMATE]:  an estimate of the number of nonzeros in
                U, including the diagonal.  This estimate is a strict upper bound on
                the actual nonzeros in U to be computed by umfpack_*_numeric.

            Info [UMFPACK_MAX_FRONT_SIZE_ESTIMATE]: estimate of the size of the
                largest frontal matrix (# of entries), for arbitrary partial
                pivoting during numerical factorization.

            Info [UMFPACK_SYMBOLIC_TIME]:  The CPU time taken, in seconds.

            Info [UMFPACK_SYMBOLIC_WALLTIME]:  The wallclock time taken, in seconds.

            Info [UMFPACK_STRATEGY_USED]: The ordering strategy used:
                UMFPACK_STRATEGY_SYMMETRIC or UMFPACK_STRATEGY_UNSYMMETRIC

            Info [UMFPACK_ORDERING_USED]:  The ordering method used:
                    UMFPACK_ORDERING_AMD
                    UMFPACK_ORDERING_GIVEN
                    UMFPACK_ORDERING_NONE
                    UMFPACK_ORDERING_METIS
                    UMFPACK_ORDERING_USER

            Info [UMFPACK_QFIXED]: 1 if the column pre-ordering will be refined
                during numerical factorization, 0 if not.

            Info [UMFPACK_DIAG_PREFERED]: 1 if diagonal pivoting will be attempted,
                0 if not.

            Info [UMFPACK_COL_SINGLETONS]:  the matrix A is analyzed by first
                eliminating all pivots with zero Markowitz cost.  This count is the
                number of these pivots with exactly one nonzero in their pivot
                column.

            Info [UMFPACK_ROW_SINGLETONS]:  the number of zero-Markowitz-cost
                pivots with exactly one nonzero in their pivot row.

            Info [UMFPACK_PATTERN_SYMMETRY]: the symmetry of the pattern of S.

            Info [UMFPACK_NZ_A_PLUS_AT]: the number of off-diagonal entries in S+S'.

            Info [UMFPACK_NZDIAG]:  the number of entries on the diagonal of S.

            Info [UMFPACK_N2]:  if S is square, and nempty_row = nempty_col, this
                is equal to n_row - n1 - nempty_row.

            Info [UMFPACK_S_SYMMETRIC]: 1 if S is square and its diagonal has been
                preserved, 0 otherwise.


            Info [UMFPACK_MAX_FRONT_NROWS_ESTIMATE]: estimate of the max number of
                rows in any frontal matrix, for arbitrary partial pivoting.

            Info [UMFPACK_MAX_FRONT_NCOLS_ESTIMATE]: estimate of the max number of
                columns in any frontal matrix, for arbitrary partial pivoting.

            ------------------------------------------------------------------------
            The next four statistics are computed only if AMD is used:
            ------------------------------------------------------------------------

            Info [UMFPACK_SYMMETRIC_LUNZ]: The number of nonzeros in L and U,
                assuming no pivoting during numerical factorization, and assuming a
                zero-free diagonal of U.  Excludes the entries on the diagonal of
                L.  If the matrix has a purely symmetric nonzero pattern, this is
                often a lower bound on the nonzeros in the actual L and U computed
                in the numerical factorization, for matrices that fit the criteria
                for the "symmetric" strategy.

            Info [UMFPACK_SYMMETRIC_FLOPS]: The floating-point operation count in
                the numerical factorization phase, assuming no pivoting.  If the
                pattern of the matrix is symmetric, this is normally a lower bound
                on the floating-point operation count in the actual numerical
                factorization, for matrices that fit the criteria for the symmetric
                strategy.

            Info [UMFPACK_SYMMETRIC_NDENSE]: The number of "dense" rows/columns of
                S+S' that were ignored during the AMD ordering.  These are placed
                last in the output order.  If > 0, then the
                Info [UMFPACK_SYMMETRIC_*] statistics, above are rough upper bounds.

            Info [UMFPACK_SYMMETRIC_DMAX]: The maximum number of nonzeros in any
                column of L, if no pivoting is performed during numerical
                factorization.  Excludes the part of the LU factorization for
                pivots with zero Markowitz cost.

            At the start of umfpack_*_symbolic, all of Info is set of -1, and then
            after that only the above listed Info [...] entries are accessed.
            Future versions might modify different parts of Info.
     */

    /* numeric factorization:
     
            Info [UMFPACK_STATUS]: status code.  This is also the return value,
                whether or not Info is present.

                UMFPACK_OK

                Numeric factorization was successful.  umfpack_*_numeric
                computed a valid numeric factorization.

                UMFPACK_WARNING_singular_matrix

                Numeric factorization was successful, but the matrix is
                singular.  umfpack_*_numeric computed a valid numeric
                factorization, but you will get a divide by zero in
                umfpack_*_*solve.  For the other cases below, no Numeric object
                is created (*Numeric is (void *) NULL).

                UMFPACK_ERROR_out_of_memory

                Insufficient memory to complete the numeric factorization.

                UMFPACK_ERROR_argument_missing

                One or more required arguments are missing.

                UMFPACK_ERROR_invalid_Symbolic_object

                Symbolic object provided as input is invalid.

                UMFPACK_ERROR_different_pattern

                The pattern (Ap and/or Ai) has changed since the call to
                umfpack_*_*symbolic which produced the Symbolic object.

            Info [UMFPACK_NROW]:  the value of n_row stored in the Symbolic object.

            Info [UMFPACK_NCOL]:  the value of n_col stored in the Symbolic object.

            Info [UMFPACK_NZ]:  the number of entries in the input matrix.
                This value is obtained from the Symbolic object.

            Info [UMFPACK_SIZE_OF_UNIT]:  the number of bytes in a Unit, for memory
                usage statistics below.

            Info [UMFPACK_VARIABLE_INIT]: the initial size (in Units) of the
                variable-sized part of the Numeric object.  If this differs from
                Info [UMFPACK_VARIABLE_INIT_ESTIMATE], then the pattern (Ap and/or
                Ai) has changed since the last call to umfpack_*_*symbolic, which is
                an error condition.

            Info [UMFPACK_VARIABLE_PEAK]: the peak size (in Units) of the
                variable-sized part of the Numeric object.  This size is the amount
                of space actually used inside the block of memory, not the space
                allocated via UMF_malloc.  You can reduce UMFPACK's memory
                requirements by setting Control [UMFPACK_ALLOC_INIT] to the ratio
                Info [UMFPACK_VARIABLE_PEAK] / Info[UMFPACK_VARIABLE_PEAK_ESTIMATE].
                This will ensure that no memory reallocations occur (you may want to
                add 0.001 to make sure that integer roundoff does not lead to a
                memory size that is 1 Unit too small; otherwise, garbage collection
                and reallocation will occur).

            Info [UMFPACK_VARIABLE_FINAL]: the final size (in Units) of the
                variable-sized part of the Numeric object.  It holds just the
                sparse LU factors.

            Info [UMFPACK_NUMERIC_SIZE]:  the actual final size (in Units) of the
                entire Numeric object, including the final size of the variable
                part of the object.  Info [UMFPACK_NUMERIC_SIZE_ESTIMATE],
                an estimate, was computed by umfpack_*_*symbolic.  The estimate is
                normally an upper bound on the actual final size, but this is not
                guaranteed.

            Info [UMFPACK_PEAK_MEMORY]:  the actual peak memory usage (in Units) of
                both umfpack_*_*symbolic and umfpack_*_numeric.  An estimate,
                Info [UMFPACK_PEAK_MEMORY_ESTIMATE], was computed by
                umfpack_*_*symbolic.  The estimate is normally an upper bound on the
                actual peak usage, but this is not guaranteed.  With testing on
                hundreds of matrix arising in real applications, I have never
                observed a matrix where this estimate or the Numeric size estimate
                was less than the actual result, but this is theoretically possible.
                Please send me one if you find such a matrix.

            Info [UMFPACK_FLOPS]:  the actual count of the (useful) floating-point
                operations performed.  An estimate, Info [UMFPACK_FLOPS_ESTIMATE],
                was computed by umfpack_*_*symbolic.  The estimate is guaranteed to
                be an upper bound on this flop count.  The flop count excludes
                "useless" flops on zero values, flops performed during the pivot
                search (for tentative updates and assembly of candidate columns),
                and flops performed to add frontal matrices together.

                For the real version, only (+ - * /) are counted.  For the complex
                version, the following counts are used:

                operation	flops
                    c = 1/b		6
                c = a*b		6
                c -= a*b	8

            Info [UMFPACK_LNZ]: the actual nonzero entries in final factor L,
                including the diagonal.  This excludes any zero entries in L,
                although some of these are stored in the Numeric object.  The
                Info [UMFPACK_LU_ENTRIES] statistic does account for all
                explicitly stored zeros, however.  Info [UMFPACK_LNZ_ESTIMATE],
                an estimate, was computed by umfpack_*_*symbolic.  The estimate is
                guaranteed to be an upper bound on Info [UMFPACK_LNZ].

            Info [UMFPACK_UNZ]: the actual nonzero entries in final factor U,
                including the diagonal.  This excludes any zero entries in U,
                although some of these are stored in the Numeric object.  The
                Info [UMFPACK_LU_ENTRIES] statistic does account for all
                explicitly stored zeros, however.  Info [UMFPACK_UNZ_ESTIMATE],
                an estimate, was computed by umfpack_*_*symbolic.  The estimate is
                guaranteed to be an upper bound on Info [UMFPACK_UNZ].

            Info [UMFPACK_NUMERIC_DEFRAG]:  The number of garbage collections
                performed during umfpack_*_numeric, to compact the contents of the
                variable-sized workspace used by umfpack_*_numeric.  No estimate was
                computed by umfpack_*_*symbolic.  In the current version of UMFPACK,
                garbage collection is performed and then the memory is reallocated,
                so this statistic is the same as Info [UMFPACK_NUMERIC_REALLOC],
                below.  It may differ in future releases.

            Info [UMFPACK_NUMERIC_REALLOC]:  The number of times that the Numeric
                object was increased in size from its initial size.  A rough upper
                bound on the peak size of the Numeric object was computed by
                umfpack_*_*symbolic, so reallocations should be rare.  However, if
                umfpack_*_numeric is unable to allocate that much storage, it
                reduces its request until either the allocation succeeds, or until
                it gets too small to do anything with.  If the memory that it
                finally got was small, but usable, then the reallocation count
                could be high.  No estimate of this count was computed by
                umfpack_*_*symbolic.

            Info [UMFPACK_NUMERIC_COSTLY_REALLOC]:  The number of times that the
                system realloc library routine (or mxRealloc for the mexFunction)
                had to move the workspace.  Realloc can sometimes increase the size
                of a block of memory without moving it, which is much faster.  This
                statistic will always be <= Info [UMFPACK_NUMERIC_REALLOC].  If your
                memory space is fragmented, then the number of "costly" realloc's
                will be equal to Info [UMFPACK_NUMERIC_REALLOC].

            Info [UMFPACK_COMPRESSED_PATTERN]:  The number of integers used to
                represent the pattern of L and U.

            Info [UMFPACK_LU_ENTRIES]:  The total number of numerical values that
                are stored for the LU factors.  Some of the values may be explicitly
                zero in order to save space (allowing for a smaller compressed
                pattern).

            Info [UMFPACK_NUMERIC_TIME]:  The CPU time taken, in seconds.

            Info [UMFPACK_RCOND]:  A rough estimate of the condition number, equal
                to min (abs (diag (U))) / max (abs (diag (U))), or zero if the
                diagonal of U is all zero.

            Info [UMFPACK_UDIAG_NZ]:  The number of numerically nonzero values on
                the diagonal of U.

            Info [UMFPACK_UMIN]:  the smallest absolute value on the diagonal of U.

            Info [UMFPACK_UMAX]:  the smallest absolute value on the diagonal of U.

            Info [UMFPACK_MAX_FRONT_SIZE]: the size of the
                largest frontal matrix (number of entries).

            Info [UMFPACK_NUMERIC_WALLTIME]:  The wallclock time taken, in seconds.

            Info [UMFPACK_MAX_FRONT_NROWS]: the max number of
                rows in any frontal matrix.

            Info [UMFPACK_MAX_FRONT_NCOLS]: the max number of
                columns in any frontal matrix.

            Info [UMFPACK_WAS_SCALED]:  the scaling used, either UMFPACK_SCALE_NONE,
                UMFPACK_SCALE_SUM, or UMFPACK_SCALE_MAX.

            Info [UMFPACK_RSMIN]: if scaling is performed, the smallest scale factor
                for any row (either the smallest sum of absolute entries, or the
                smallest maximum of absolute entries).

            Info [UMFPACK_RSMAX]: if scaling is performed, the largest scale factor
                for any row (either the largest sum of absolute entries, or the
                largest maximum of absolute entries).

            Info [UMFPACK_ALLOC_INIT_USED]:  the initial allocation parameter used.

            Info [UMFPACK_FORCED_UPDATES]:  the number of BLAS-3 updates to the
                frontal matrices that were required because the frontal matrix
                grew larger than its current working array.

            Info [UMFPACK_NOFF_DIAG]: number of off-diagonal pivots selected, if the
                symmetric strategy is used.

            Info [UMFPACK_NZDROPPED]: the number of entries smaller in absolute
                value than Control [UMFPACK_DROPTOL] that were dropped from L and U.
                Note that entries on the diagonal of U are never dropped.

            Info [UMFPACK_ALL_LNZ]: the number of entries in L, including the
                diagonal, if no small entries are dropped.

            Info [UMFPACK_ALL_UNZ]: the number of entries in U, including the
                diagonal, if no small entries are dropped.

            Only the above listed Info [...] entries are accessed.  The remaining
            entries of Info are not accessed or modified by umfpack_*_numeric.
            Future versions might modify different parts of Info.
     */

    /* solve 
     
            Contains statistics about the solution factorization.  If a
            (double *) NULL pointer is passed, then no statistics are returned in
            Info (this is not an error condition).  The following statistics are
            computed in umfpack_*_solve:

            Info [UMFPACK_STATUS]: status code.  This is also the return value,
                whether or not Info is present.

                UMFPACK_OK

                The linear system was successfully solved.

                UMFPACK_WARNING_singular_matrix

                A divide-by-zero occurred.  Your solution will contain Inf's
                and/or NaN's.  Some parts of the solution may be valid.  For
                example, solving Ax=b with

                A = [2 0]  b = [ 1 ]  returns x = [ 0.5 ]
                    [0 0]      [ 0 ]              [ Inf ]

                UMFPACK_ERROR_out_of_memory

                Insufficient memory to solve the linear system.

                UMFPACK_ERROR_argument_missing

                One or more required arguments are missing.  The B, X, (or
                Bx and Xx for the complex versions) arguments
                are always required.  Info and Control are not required.  Ap,
                Ai, Ax are required if Ax=b,
                A'x=b, A.'x=b is to be solved, the (default) iterative
                refinement is requested, and the matrix A is nonsingular.

                UMFPACK_ERROR_invalid_system

                The sys argument is not valid, or the matrix A is not square.

                UMFPACK_ERROR_invalid_Numeric_object

                The Numeric object is not valid.

            Info [UMFPACK_NROW], Info [UMFPACK_NCOL]:
                The dimensions of the matrix A (L is n_row-by-n_inner and
                U is n_inner-by-n_col, with n_inner = min(n_row,n_col)).

            Info [UMFPACK_NZ]:  the number of entries in the input matrix, Ap [n],
                if iterative refinement is requested (Ax=b, A'x=b, or A.'x=b is
                being solved, Control [UMFPACK_IRSTEP] >= 1, and A is nonsingular).

            Info [UMFPACK_IR_TAKEN]:  The number of iterative refinement steps
                effectively taken.  The number of steps attempted may be one more
                than this; the refinement algorithm backtracks if the last
                refinement step worsens the solution.

            Info [UMFPACK_IR_ATTEMPTED]:   The number of iterative refinement steps
                attempted.  The number of times a linear system was solved is one
                more than this (once for the initial Ax=b, and once for each Ay=r
                solved for each iterative refinement step attempted).

            Info [UMFPACK_OMEGA1]:  sparse backward error estimate, omega1, if
                iterative refinement was performed, or -1 if iterative refinement
                not performed.

            Info [UMFPACK_OMEGA2]:  sparse backward error estimate, omega2, if
                iterative refinement was performed, or -1 if iterative refinement
                not performed.

            Info [UMFPACK_SOLVE_FLOPS]:  the number of floating point operations
                performed to solve the linear system.  This includes the work
                taken for all iterative refinement steps, including the backtrack
                (if any).

            Info [UMFPACK_SOLVE_TIME]:  The time taken, in seconds.

                Info [UMFPACK_SOLVE_WALLTIME]:  The wallclock time taken, in seconds.

            Only the above listed Info [...] entries are accessed.  The remaining
            entries of Info are not accessed or modified by umfpack_*_solve.
            Future versions might modify different parts of Info.
     */
     
    /// <summary>
    /// UMFPACK info.
    /// </summary>
    public class UmfpackInfo
    {
        internal double[] Raw;

        public UmfpackInfo()
        {
            Raw = new double[Constants.UMFPACK_INFO];
        }

        #region Public properties

        /// <summary>
        /// Gets or sets the print level
        /// </summary>
        public int PrintLevel
        {
            get { return (int)Raw[0]; }
            set { Raw[0] = value; }
        }

        /* -------------------------------------------------------------------------- */
        /* contents of Info */
        /* -------------------------------------------------------------------------- */

        /* Note that umfpack_report.m must coincide with these definitions.  S is
         * the submatrix of A after removing row/col singletons and empty rows/cols. */

        /* returned by all routines that use Info: */

        /// <summary>
        /// Status code. This is also the return value, whether or not Info is present.
        /// </summary>
        public int STATUS
        {
            get { return (int)Raw[UMFPACK_STATUS]; }
        }

        /// <summary>
        /// The value of the input argument n_row.
        /// </summary>
        public int NROW
        {
            get { return (int)Raw[UMFPACK_NROW]; }
        }

        /// <summary>
        /// The value of the input argument n_col.
        /// </summary>
        public int NCOL
        {
            get { return (int)Raw[UMFPACK_NCOL]; }
        }

        /// <summary>
        /// # of entries in A
        /// </summary>
        public int NZ
        {
            get { return (int)Raw[UMFPACK_NZ]; }
        }

        /* computed in UMFPACK_*symbolic and UMFPACK_numeric: */

        /// <summary>
        /// sizeof (Unit)
        /// </summary>
        public int SIZE_OF_UNIT
        {
            get { return (int)Raw[UMFPACK_SIZE_OF_UNIT]; }
        }

        /* computed in UMFPACK_*symbolic: */

        /// <summary>
        /// sizeof (int)
        /// </summary>
        public int SIZE_OF_INT
        {
            get { return (int)Raw[UMFPACK_SIZE_OF_INT]; }
        }

        /// <summary>
        /// sizeof (SuiteSparse_long)
        /// </summary>
        public int SIZE_OF_LONG
        {
            get { return (int)Raw[UMFPACK_SIZE_OF_LONG]; }
        }

        /// <summary>
        /// sizeof (void *)
        /// </summary>
        public int SIZE_OF_POINTER
        {
            get { return (int)Raw[UMFPACK_SIZE_OF_POINTER]; }
        }

        /// <summary>
        /// sizeof (Entry), real or complex
        /// </summary>
        public int SIZE_OF_ENTRY
        {
            get { return (int)Raw[UMFPACK_SIZE_OF_ENTRY]; }
        }

        /// <summary>
        /// number of dense rows
        /// </summary>
        public int NDENSE_ROW
        {
            get { return (int)Raw[UMFPACK_NDENSE_ROW]; }
        }

        /// <summary>
        /// number of empty rows
        /// </summary>
        public int NEMPTY_ROW
        {
            get { return (int)Raw[UMFPACK_NEMPTY_ROW]; }
        }

        /// <summary>
        /// number of dense rows
        /// </summary>
        public int NDENSE_COL
        {
            get { return (int)Raw[UMFPACK_NDENSE_COL]; }
        }

        /// <summary>
        /// number of empty rows
        /// </summary>
        public int NEMPTY_COL
        {
            get { return (int)Raw[UMFPACK_NEMPTY_COL]; }
        }

        /// <summary>
        /// # of memory compactions
        /// </summary>
        public int SYMBOLIC_DEFRAG
        {
            get { return (int)Raw[UMFPACK_SYMBOLIC_DEFRAG]; }
        }

        /// <summary>
        /// memory used by symbolic analysis
        /// </summary>
        public int SYMBOLIC_PEAK_MEMORY
        {
            get { return (int)Raw[UMFPACK_SYMBOLIC_PEAK_MEMORY]; }
        }

        /// <summary>
        /// size of Symbolic object, in Units
        /// </summary>
        public int SYMBOLIC_SIZE
        {
            get { return (int)Raw[UMFPACK_SYMBOLIC_SIZE]; }
        }

        /// <summary>
        /// time (sec.) for symbolic analysis
        /// </summary>
        public double SYMBOLIC_TIME
        {
            get { return Raw[UMFPACK_SYMBOLIC_TIME]; }
        }

        /// <summary>
        /// wall clock time for sym. analysis
        /// </summary>
        public double SYMBOLIC_WALLTIME
        {
            get { return Raw[UMFPACK_SYMBOLIC_WALLTIME]; }
        }

        /// <summary>
        /// strategy used: sym, unsym
        /// </summary>
        public int STRATEGY_USED
        {
            get { return (int)Raw[UMFPACK_STRATEGY_USED]; }
        }

        /// <summary>
        /// ordering used: colamd, amd, given
        /// </summary>
        public int ORDERING_USED
        {
            get { return (int)Raw[UMFPACK_ORDERING_USED]; }
        }

        /// <summary>
        /// whether Q is fixed or refined
        /// </summary>
        public double QFIXED
        {
            get { return Raw[UMFPACK_QFIXED]; }
        }

        /// <summary>
        /// whether diagonal pivoting attempted
        /// </summary>
        public double DIAG_PREFERRED
        {
            get { return Raw[UMFPACK_DIAG_PREFERRED]; }
        }

        /// <summary>
        /// symmetry of pattern of S
        /// </summary>
        public double PATTERN_SYMMETRY
        {
            get { return Raw[UMFPACK_PATTERN_SYMMETRY]; }
        }

        /// <summary>
        /// nnz (S+S'), excl. diagonal
        /// </summary>
        public double NZ_A_PLUS_AT
        {
            get { return Raw[UMFPACK_NZ_A_PLUS_AT]; }
        }

        /// <summary>
        /// nnz (diag (S))
        /// </summary>
        public int NZDIAG
        {
            get { return (int)Raw[UMFPACK_NZDIAG]; }
        }

        /* AMD statistics, computed in UMFPACK_*symbolic: */

        /// <summary>
        /// nz in L+U, if AMD ordering used
        /// </summary>
        public int SYMMETRIC_LUNZ
        {
            get { return (int)Raw[UMFPACK_SYMMETRIC_LUNZ]; }
        }

        /// <summary>
        /// flops for LU, if AMD ordering used
        /// </summary>
        public double SYMMETRIC_FLOPS
        {
            get { return Raw[UMFPACK_SYMMETRIC_FLOPS]; }
        }

        /// <summary>
        /// # of "dense" rows/cols in S+S'
        /// </summary>
        public int SYMMETRIC_NDENSE
        {
            get { return (int)Raw[UMFPACK_SYMMETRIC_NDENSE]; }
        }

        /// <summary>
        /// max nz in cols of L, for AMD
        /// </summary>
        public int SYMMETRIC_DMAX
        {
            get { return (int)Raw[UMFPACK_SYMMETRIC_DMAX]; }
        }

        /* 51:55 unused */

        /* statistcs for singleton pruning */

        /// <summary>
        /// # of column singletons
        /// </summary>
        public int COL_SINGLETONS
        {
            get { return (int)Raw[UMFPACK_COL_SINGLETONS]; }
        }

        /// <summary>
        /// # of row singletons
        /// </summary>
        public int ROW_SINGLETONS
        {
            get { return (int)Raw[UMFPACK_ROW_SINGLETONS]; }
        }

        /// <summary>
        /// size of S
        /// </summary>
        public int N2
        {
            get { return (int)Raw[UMFPACK_N2]; }
        }

        /// <summary>
        /// 1 if S square and symmetricly perm.
        /// </summary>
        public double S_SYMMETRIC
        {
            get { return Raw[UMFPACK_S_SYMMETRIC]; }
        }

        /* estimates computed in UMFPACK_*symbolic: */

        /// <summary>
        /// final size of Numeric->Memory
        /// </summary>
        public double NUMERIC_SIZE_ESTIMATE
        {
            get { return Raw[UMFPACK_NUMERIC_SIZE_ESTIMATE]; }
        }

        /// <summary>
        /// for symbolic & numeric
        /// </summary>
        public double PEAK_MEMORY_ESTIMATE
        {
            get { return Raw[UMFPACK_PEAK_MEMORY_ESTIMATE]; }
        }

        /// <summary>
        /// flop count
        /// </summary>
        public double FLOPS_ESTIMATE
        {
            get { return Raw[UMFPACK_FLOPS_ESTIMATE]; }
        }

        /// <summary>
        /// nz in L, incl. diagonal
        /// </summary>
        public int LNZ_ESTIMATE
        {
            get { return (int)Raw[UMFPACK_LNZ_ESTIMATE]; }
        }

        /// <summary>
        /// nz in U, incl. diagonal
        /// </summary>
        public int UNZ_ESTIMATE
        {
            get { return (int)Raw[UMFPACK_UNZ_ESTIMATE]; }
        }

        /// <summary>
        /// initial size of Numeric->Memory
        /// </summary>
        public double VARIABLE_INIT_ESTIMATE
        {
            get { return Raw[UMFPACK_VARIABLE_INIT_ESTIMATE]; }
        }

        /// <summary>
        /// peak size of Numeric->Memory
        /// </summary>
        public double VARIABLE_PEAK_ESTIMATE
        {
            get { return Raw[UMFPACK_VARIABLE_PEAK_ESTIMATE]; }
        }

        /// <summary>
        /// final size of Numeric->Memory
        /// </summary>
        public double VARIABLE_FINAL_ESTIMATE
        {
            get { return Raw[UMFPACK_VARIABLE_FINAL_ESTIMATE]; }
        }

        /// <summary>
        /// max frontal matrix size
        /// </summary>
        public int MAX_FRONT_SIZE_ESTIMATE
        {
            get { return (int)Raw[UMFPACK_MAX_FRONT_SIZE_ESTIMATE]; }
        }

        /// <summary>
        /// max # rows in any front
        /// </summary>
        public int MAX_FRONT_NROWS_ESTIMATE
        {
            get { return (int)Raw[UMFPACK_MAX_FRONT_NROWS_ESTIMATE]; }
        }

        /// <summary>
        /// max # columns in any front
        /// </summary>
        public int MAX_FRONT_NCOLS_ESTIMATE
        {
            get { return (int)Raw[UMFPACK_MAX_FRONT_NCOLS_ESTIMATE]; }
        }

        /* exact values, (estimates shown above) computed in UMFPACK_numeric: */

        /// <summary>
        /// final size of Numeric->Memory
        /// </summary>
        public double NUMERIC_SIZE
        {
            get { return Raw[UMFPACK_NUMERIC_SIZE]; }
        }

        /// <summary>
        /// for symbolic & numeric
        /// </summary>
        public double PEAK_MEMORY
        {
            get { return Raw[UMFPACK_PEAK_MEMORY]; }
        }

        /// <summary>
        /// flop count
        /// </summary>
        public double FLOPS
        {
            get { return Raw[UMFPACK_FLOPS]; }
        }

        /// <summary>
        /// nz in L, incl. diagonal
        /// </summary>
        public int LNZ
        {
            get { return (int)Raw[UMFPACK_LNZ]; }
        }

        /// <summary>
        /// nz in U, incl. diagonal
        /// </summary>
        public int UNZ
        {
            get { return (int)Raw[UMFPACK_UNZ]; }
        }

        /// <summary>
        /// initial size of Numeric->Memory
        /// </summary>
        public double VARIABLE_INIT
        {
            get { return Raw[UMFPACK_VARIABLE_INIT]; }
        }

        /// <summary>
        /// peak size of Numeric->Memory
        /// </summary>
        public double VARIABLE_PEAK
        {
            get { return Raw[UMFPACK_VARIABLE_PEAK]; }
        }

        /// <summary>
        /// final size of Numeric->Memory
        /// </summary>
        public double VARIABLE_FINAL
        {
            get { return Raw[UMFPACK_VARIABLE_FINAL]; }
        }

        /// <summary>
        /// max frontal matrix size
        /// </summary>
        public int MAX_FRONT_SIZE
        {
            get { return (int)Raw[UMFPACK_MAX_FRONT_SIZE]; }
        }

        /// <summary>
        /// max # rows in any front
        /// </summary>
        public int MAX_FRONT_NROWS
        {
            get { return (int)Raw[UMFPACK_MAX_FRONT_NROWS]; }
        }

        /// <summary>
        /// max # columns in any front
        /// </summary>
        public int MAX_FRONT_NCOLS
        {
            get { return (int)Raw[UMFPACK_MAX_FRONT_NCOLS]; }
        }

        /* computed in UMFPACK_numeric: */

        /// <summary>
        /// # of garbage collections
        /// </summary>
        public int NUMERIC_DEFRAG
        {
            get { return (int)Raw[UMFPACK_NUMERIC_DEFRAG]; }
        }

        /// <summary>
        /// # of memory reallocations
        /// </summary>
        public int NUMERIC_REALLOC
        {
            get { return (int)Raw[UMFPACK_NUMERIC_REALLOC]; }
        }

        /// <summary>
        /// # of costlly memory realloc's
        /// </summary>
        public int NUMERIC_COSTLY_REALLOC
        {
            get { return (int)Raw[UMFPACK_NUMERIC_COSTLY_REALLOC]; }
        }

        /// <summary>
        /// # of integers in LU pattern
        /// </summary>
        public int COMPRESSED_PATTERN
        {
            get { return (int)Raw[UMFPACK_COMPRESSED_PATTERN]; }
        }

        /// <summary>
        /// # of reals in LU factors
        /// </summary>
        public int LU_ENTRIES
        {
            get { return (int)Raw[UMFPACK_LU_ENTRIES]; }
        }

        /// <summary>
        /// numeric factorization time
        /// </summary>
        public double NUMERIC_TIME
        {
            get { return Raw[UMFPACK_NUMERIC_TIME]; }
        }

        /// <summary>
        /// nz on diagonal of U
        /// </summary>
        public int UDIAG_NZ
        {
            get { return (int)Raw[UMFPACK_UDIAG_NZ]; }
        }

        /// <summary>
        /// est. reciprocal condition #
        /// </summary>
        public double RCOND
        {
            get { return Raw[UMFPACK_RCOND]; }
        }

        /// <summary>
        /// none, max row, or sum row
        /// </summary>
        public double WAS_SCALED
        {
            get { return Raw[UMFPACK_WAS_SCALED]; }
        }

        /// <summary>
        /// min (max row) or min (sum row)
        /// </summary>
        public double RSMIN
        {
            get { return Raw[UMFPACK_RSMIN]; }
        }

        /// <summary>
        /// max (max row) or max (sum row)
        /// </summary>
        public double RSMAX
        {
            get { return Raw[UMFPACK_RSMAX]; }
        }

        /// <summary>
        /// min abs diagonal entry of U
        /// </summary>
        public double UMIN
        {
            get { return Raw[UMFPACK_UMIN]; }
        }

        /// <summary>
        /// max abs diagonal entry of U
        /// </summary>
        public double UMAX
        {
            get { return Raw[UMFPACK_UMAX]; }
        }

        /// <summary>
        /// alloc_init parameter used
        /// </summary>
        public double ALLOC_INIT_USED
        {
            get { return Raw[UMFPACK_ALLOC_INIT_USED]; }
        }

        /// <summary>
        /// # of forced updates
        /// </summary>
        public double FORCED_UPDATES
        {
            get { return Raw[UMFPACK_FORCED_UPDATES]; }
        }

        /// <summary>
        /// numeric wall clock time
        /// </summary>
        public double NUMERIC_WALLTIME
        {
            get { return Raw[UMFPACK_NUMERIC_WALLTIME]; }
        }

        /// <summary>
        /// number of off-diagonal pivots
        /// </summary>
        public int NOFF_DIAG
        {
            get { return (int)Raw[UMFPACK_NOFF_DIAG]; }
        }


        /// <summary>
        /// nz in L, if no dropped entries
        /// </summary>
        public int ALL_LNZ
        {
            get { return (int)Raw[UMFPACK_ALL_LNZ]; }
        }

        /// <summary>
        /// nz in U, if no dropped entries
        /// </summary>
        public int ALL_UNZ
        {
            get { return (int)Raw[UMFPACK_ALL_UNZ]; }
        }

        /// <summary>
        /// # of dropped small entries
        /// </summary>
        public int NZDROPPED
        {
            get { return (int)Raw[UMFPACK_NZDROPPED]; }
        }

        /* computed in UMFPACK_solve: */

        /// <summary>
        /// # of iterative refinement steps taken
        /// </summary>
        public int IR_TAKEN
        {
            get { return (int)Raw[UMFPACK_IR_TAKEN]; }
        }

        /// <summary>
        /// # of iter. refinement steps attempted
        /// </summary>
        public int IR_ATTEMPTED
        {
            get { return (int)Raw[UMFPACK_IR_ATTEMPTED]; }
        }

        /// <summary>
        /// omega1, sparse backward error estimate
        /// </summary>
        public double OMEGA1
        {
            get { return Raw[UMFPACK_OMEGA1]; }
        }

        /// <summary>
        /// omega2, sparse backward error estimate
        /// </summary>
        public double OMEGA2
        {
            get { return Raw[UMFPACK_OMEGA2]; }
        }

        /// <summary>
        /// flop count for solve
        /// </summary>
        public double SOLVE_FLOPS
        {
            get { return Raw[UMFPACK_SOLVE_FLOPS]; }
        }

        /// <summary>
        /// solve time (seconds)
        /// </summary>
        public double SOLVE_TIME
        {
            get { return Raw[UMFPACK_SOLVE_TIME]; }
        }

        /// <summary>
        /// solve time (wall clock, seconds)
        /// </summary>
        public double SOLVE_WALLTIME
        {
            get { return Raw[UMFPACK_SOLVE_WALLTIME]; }
        }

        #endregion

        #region Constants

        /* -------------------------------------------------------------------------- */
        /* contents of Info */
        /* -------------------------------------------------------------------------- */

        /* Note that umfpack_report.m must coincide with these definitions.  S is
         * the submatrix of A after removing row/col singletons and empty rows/cols. */

        /* returned by all routines that use Info: */
        private const int UMFPACK_STATUS = 0; /* UMFPACK_OK, or other result */
        private const int UMFPACK_NROW = 1; /* n_row input value */
        private const int UMFPACK_NCOL = 16; /* n_col input value */
        private const int UMFPACK_NZ = 2; /* # of entries in A */

        /* computed in UMFPACK_*symbolic and UMFPACK_numeric: */
        private const int UMFPACK_SIZE_OF_UNIT = 3; /* sizeof (Unit) */

        /* computed in UMFPACK_*symbolic: */
        private const int UMFPACK_SIZE_OF_INT = 4; /* sizeof (int) */
        private const int UMFPACK_SIZE_OF_LONG = 5; /* sizeof (SuiteSparse_long) */
        private const int UMFPACK_SIZE_OF_POINTER = 6; /* sizeof (void *) */
        private const int UMFPACK_SIZE_OF_ENTRY = 7; /* sizeof (Entry), real or complex */
        private const int UMFPACK_NDENSE_ROW = 8; /* number of dense rows */
        private const int UMFPACK_NEMPTY_ROW = 9; /* number of empty rows */
        private const int UMFPACK_NDENSE_COL = 10; /* number of dense rows */
        private const int UMFPACK_NEMPTY_COL = 11; /* number of empty rows */
        private const int UMFPACK_SYMBOLIC_DEFRAG = 12; /* # of memory compactions */
        private const int UMFPACK_SYMBOLIC_PEAK_MEMORY = 13; /* memory used by symbolic analysis */
        private const int UMFPACK_SYMBOLIC_SIZE = 14; /* size of Symbolic object, in Units */
        private const int UMFPACK_SYMBOLIC_TIME = 15; /* time (sec.) for symbolic analysis */
        private const int UMFPACK_SYMBOLIC_WALLTIME = 17; /* wall clock time for sym. analysis */
        private const int UMFPACK_STRATEGY_USED = 18; /* strategy used: sym, unsym */
        private const int UMFPACK_ORDERING_USED = 19; /* ordering used: colamd, amd, given */
        private const int UMFPACK_QFIXED = 31; /* whether Q is fixed or refined */
        private const int UMFPACK_DIAG_PREFERRED = 32; /* whether diagonal pivoting attempted*/
        private const int UMFPACK_PATTERN_SYMMETRY = 33; /* symmetry of pattern of S */
        private const int UMFPACK_NZ_A_PLUS_AT = 34; /* nnz (S+S'), excl. diagonal */
        private const int UMFPACK_NZDIAG = 35; /* nnz (diag (S)) */

        /* AMD statistics, computed in UMFPACK_*symbolic: */
        private const int UMFPACK_SYMMETRIC_LUNZ = 36; /* nz in L+U, if AMD ordering used */
        private const int UMFPACK_SYMMETRIC_FLOPS = 37; /* flops for LU, if AMD ordering used */
        private const int UMFPACK_SYMMETRIC_NDENSE = 38; /* # of "dense" rows/cols in S+S' */
        private const int UMFPACK_SYMMETRIC_DMAX = 39; /* max nz in cols of L, for AMD */

        /* 51:55 unused */

        /* statistcs for singleton pruning */
        private const int UMFPACK_COL_SINGLETONS = 56; /* # of column singletons */
        private const int UMFPACK_ROW_SINGLETONS = 57; /* # of row singletons */
        private const int UMFPACK_N2 = 58; /* size of S */
        private const int UMFPACK_S_SYMMETRIC = 59; /* 1 if S square and symmetricly perm.*/

        /* estimates computed in UMFPACK_*symbolic: */
        private const int UMFPACK_NUMERIC_SIZE_ESTIMATE = 20; /* final size of Numeric->Memory */
        private const int UMFPACK_PEAK_MEMORY_ESTIMATE = 21; /* for symbolic & numeric */
        private const int UMFPACK_FLOPS_ESTIMATE = 22; /* flop count */
        private const int UMFPACK_LNZ_ESTIMATE = 23; /* nz in L, incl. diagonal */
        private const int UMFPACK_UNZ_ESTIMATE = 24; /* nz in U, incl. diagonal */
        private const int UMFPACK_VARIABLE_INIT_ESTIMATE = 25; /* initial size of Numeric->Memory*/
        private const int UMFPACK_VARIABLE_PEAK_ESTIMATE = 26; /* peak size of Numeric->Memory */
        private const int UMFPACK_VARIABLE_FINAL_ESTIMATE = 27; /* final size of Numeric->Memory */
        private const int UMFPACK_MAX_FRONT_SIZE_ESTIMATE = 28; /* max frontal matrix size */
        private const int UMFPACK_MAX_FRONT_NROWS_ESTIMATE = 29; /* max # rows in any front */
        private const int UMFPACK_MAX_FRONT_NCOLS_ESTIMATE = 30; /* max # columns in any front */

        /* exact values, (estimates shown above) computed in UMFPACK_numeric: */
        private const int UMFPACK_NUMERIC_SIZE = 40; /* final size of Numeric->Memory */
        private const int UMFPACK_PEAK_MEMORY = 41; /* for symbolic & numeric */
        private const int UMFPACK_FLOPS = 42; /* flop count */
        private const int UMFPACK_LNZ = 43; /* nz in L, incl. diagonal */
        private const int UMFPACK_UNZ = 44; /* nz in U, incl. diagonal */
        private const int UMFPACK_VARIABLE_INIT = 45; /* initial size of Numeric->Memory*/
        private const int UMFPACK_VARIABLE_PEAK = 46; /* peak size of Numeric->Memory */
        private const int UMFPACK_VARIABLE_FINAL = 47; /* final size of Numeric->Memory */
        private const int UMFPACK_MAX_FRONT_SIZE = 48; /* max frontal matrix size */
        private const int UMFPACK_MAX_FRONT_NROWS = 49; /* max # rows in any front */
        private const int UMFPACK_MAX_FRONT_NCOLS = 50; /* max # columns in any front */

        /* computed in UMFPACK_numeric: */
        private const int UMFPACK_NUMERIC_DEFRAG = 60; /* # of garbage collections */
        private const int UMFPACK_NUMERIC_REALLOC = 61; /* # of memory reallocations */
        private const int UMFPACK_NUMERIC_COSTLY_REALLOC = 62; /* # of costlly memory realloc's */
        private const int UMFPACK_COMPRESSED_PATTERN = 63; /* # of integers in LU pattern */
        private const int UMFPACK_LU_ENTRIES = 64; /* # of reals in LU factors */
        private const int UMFPACK_NUMERIC_TIME = 65; /* numeric factorization time */
        private const int UMFPACK_UDIAG_NZ = 66; /* nz on diagonal of U */
        private const int UMFPACK_RCOND = 67; /* est. reciprocal condition # */
        private const int UMFPACK_WAS_SCALED = 68; /* none, max row, or sum row */
        private const int UMFPACK_RSMIN = 69; /* min (max row) or min (sum row) */
        private const int UMFPACK_RSMAX = 70; /* max (max row) or max (sum row) */
        private const int UMFPACK_UMIN = 71; /* min abs diagonal entry of U */
        private const int UMFPACK_UMAX = 72; /* max abs diagonal entry of U */
        private const int UMFPACK_ALLOC_INIT_USED = 73; /* alloc_init parameter used */
        private const int UMFPACK_FORCED_UPDATES = 74; /* # of forced updates */
        private const int UMFPACK_NUMERIC_WALLTIME = 75; /* numeric wall clock time */
        private const int UMFPACK_NOFF_DIAG = 76; /* number of off-diagonal pivots */

        private const int UMFPACK_ALL_LNZ = 77; /* nz in L, if no dropped entries */
        private const int UMFPACK_ALL_UNZ = 78; /* nz in U, if no dropped entries */
        private const int UMFPACK_NZDROPPED = 79; /* # of dropped small entries */

        /* computed in UMFPACK_solve: */
        private const int UMFPACK_IR_TAKEN = 80; /* # of iterative refinement steps taken */
        private const int UMFPACK_IR_ATTEMPTED = 81; /* # of iter. refinement steps attempted */
        private const int UMFPACK_OMEGA1 = 82; /* omega1, sparse backward error estimate */
        private const int UMFPACK_OMEGA2 = 83; /* omega2, sparse backward error estimate */
        private const int UMFPACK_SOLVE_FLOPS = 84; /* flop count for solve */
        private const int UMFPACK_SOLVE_TIME = 85; /* solve time (seconds) */
        private const int UMFPACK_SOLVE_WALLTIME = 86; /* solve time (wall clock, seconds) */

        /* Info [87, 88, 89] unused */

        /* Unused parts of Info may be used in future versions of UMFPACK. */

        #endregion
    }
}
