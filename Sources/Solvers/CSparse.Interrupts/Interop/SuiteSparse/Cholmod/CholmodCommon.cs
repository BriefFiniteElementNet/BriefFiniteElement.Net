
namespace CSparse.Interop.SuiteSparse.Cholmod
{
    using System;
    using System.Runtime.InteropServices;

#if X64
    using size_t = System.UInt64;
    using SuiteSparse_long = System.Int64;
#else
    using size_t = System.UInt32;
    using SuiteSparse_long = System.Int32;
#endif

    /* ... so make them void * pointers if the GPU is not being used */
    using CHOLMOD_CUBLAS_HANDLE = System.IntPtr;
    using CHOLMOD_CUDASTREAM = System.IntPtr;
    using CHOLMOD_CUDAEVENT = System.IntPtr;

    public delegate void ErrorHandler(int status, string file, int line, string message);

    [StructLayout(LayoutKind.Sequential)]
    public struct CholmodMethod
    {
        /* statistics for this method */
        public double lnz;     /* nnz(L) excl. zeros from supernodal amalgamation,
			     * for a "pure" L */

        public double fl;      /* flop count for a "pure", real simplicial LL'
			     * factorization, with no extra work due to
	    * amalgamation.  Subtract n to get the LDL' flop count.   Multiply
	    * by about 4 if the matrix is complex or zomplex. */

        /* ordering method parameters */
        public double prune_dense;/* dense row/col control for AMD, SYMAMD, CSYMAMD,
			     * and NESDIS (cholmod_nested_dissection).  For a
	    * symmetric n-by-n matrix, rows/columns with more than
	    * MAX (16, prune_dense * sqrt (n)) entries are removed prior to
	    * ordering.  They appear at the end of the re-ordered matrix.
	    *
	    * If prune_dense < 0, only completely dense rows/cols are removed.
	    *
	    * This paramater is also the dense column control for COLAMD and
	    * CCOLAMD.  For an m-by-n matrix, columns with more than
	    * MAX (16, prune_dense * sqrt (MIN (m,n))) entries are removed prior
	    * to ordering.  They appear at the end of the re-ordered matrix.
	    * CHOLMOD factorizes A*A', so it calls COLAMD and CCOLAMD with A',
	    * not A.  Thus, this parameter affects the dense *row* control for
	    * CHOLMOD's matrix, and the dense *column* control for COLAMD and
	    * CCOLAMD.
	    *
	    * Removing dense rows and columns improves the run-time of the
	    * ordering methods.  It has some impact on ordering quality
	    * (usually minimal, sometimes good, sometimes bad).
	    *
	    * Default: 10. */

        public double prune_dense2;/* dense row control for COLAMD and CCOLAMD.
			    *  Rows with more than MAX (16, dense2 * sqrt (n))
	    * for an m-by-n matrix are removed prior to ordering.  CHOLMOD's
	    * matrix is transposed before ordering it with COLAMD or CCOLAMD,
	    * so this controls the dense *columns* of CHOLMOD's matrix, and
	    * the dense *rows* of COLAMD's or CCOLAMD's matrix.
	    *
	    * If prune_dense2 < 0, only completely dense rows/cols are removed.
	    *
	    * Default: -1.  Note that this is not the default for COLAMD and
	    * CCOLAMD.  -1 is best for Cholesky.  10 is best for LU.  */

        public double nd_oksep;   /* in NESDIS, when a node separator is computed, it
			     * discarded if nsep >= nd_oksep*n, where nsep is
	    * the number of nodes in the separator, and n is the size of the
	    * graph being cut.  Valid range is 0 to 1.  If 1 or greater, the
	    * separator is discarded if it consists of the entire graph.
	    * Default: 1 */

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        internal double[] other_1; /* future expansion */

        public size_t nd_small;    /* do not partition graphs with fewer nodes than
			     * nd_small, in NESDIS.  Default: 200 (same as METIS) */

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        internal size_t[] other_2; /* future expansion */

        public int aggressive;    /* Aggresive absorption in AMD, COLAMD, SYMAMD,
			     * CCOLAMD, and CSYMAMD.  Default: TRUE */

        public int order_for_lu;  /* CCOLAMD can be optimized to produce an ordering
			     * for LU or Cholesky factorization.  CHOLMOD only
	    * performs a Cholesky factorization.  However, you may wish to use
	    * CHOLMOD as an interface for CCOLAMD but use it for your own LU
	    * factorization.  In this case, order_for_lu should be set to FALSE.
	    * When factorizing in CHOLMOD itself, you should *** NEVER *** set
	    * this parameter FALSE.  Default: TRUE. */

        public int nd_compress;   /* If TRUE, compress the graph and subgraphs before
			     * partitioning them in NESDIS.  Default: TRUE */

        public int nd_camd;        /* If 1, follow the nested dissection ordering
			     * with a constrained minimum degree ordering that
	    * respects the partitioning just found (using CAMD).  If 2, use
	    * CSYMAMD instead.  If you set nd_small very small, you may not need
	    * this ordering, and can save time by setting it to zero (no
	    * constrained minimum degree ordering).  Default: 1. */

        public int nd_components; /* The nested dissection ordering finds a node
			     * separator that splits the graph into two parts,
	    * which may be unconnected.  If nd_components is TRUE, each of
	    * these connected components is split independently.  If FALSE,
	    * each part is split as a whole, even if it consists of more than
	    * one connected component.  Default: FALSE */

        /* fill-reducing ordering to use */
        public int ordering;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        internal size_t[] other_3; /* future expansion */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CholmodCommon
    {
        /* ---------------------------------------------------------------------- */
        /* parameters for symbolic/numeric factorization and update/downdate */
        /* ---------------------------------------------------------------------- */

        public double dbound;  /* Smallest absolute value of diagonal entries of D
			 * for LDL' factorization and update/downdate/rowadd/
	* rowdel, or the diagonal of L for an LL' factorization.
	* Entries in the range 0 to dbound are replaced with dbound.
	* Entries in the range -dbound to 0 are replaced with -dbound.  No
	* changes are made to the diagonal if dbound <= 0.  Default: zero */

        public double grow0;   /* For a simplicial factorization, L->i and L->x can
			 * grow if necessary.  grow0 is the factor by which
	* it grows.  For the initial space, L is of size MAX (1,grow0) times
	* the required space.  If L runs out of space, the new size of L is
	* MAX(1.2,grow0) times the new required space.   If you do not plan on
	* modifying the LDL' factorization in the Modify module, set grow0 to
	* zero (or set grow2 to 0, see below).  Default: 1.2 */

        public double grow1;

        public size_t grow2;   /* For a simplicial factorization, each column j of L
			 * is initialized with space equal to
	* grow1*L->ColCount[j] + grow2.  If grow0 < 1, grow1 < 1, or grow2 == 0,
	* then the space allocated is exactly equal to L->ColCount[j].  If the
	* column j runs out of space, it increases to grow1*need + grow2 in
	* size, where need is the total # of nonzeros in that column.  If you do
	* not plan on modifying the factorization in the Modify module, set
	* grow2 to zero.  Default: grow1 = 1.2, grow2 = 5. */

        public size_t maxrank; /* rank of maximum update/downdate.  Valid values:
			 * 2, 4, or 8.  A value < 2 is set to 2, and a
	* value > 8 is set to 8.  It is then rounded up to the next highest
	* power of 2, if not already a power of 2.  Workspace (Xwork, below) of
	* size nrow-by-maxrank double's is allocated for the update/downdate.
	* If an update/downdate of rank-k is requested, with k > maxrank,
	* it is done in steps of maxrank.  Default: 8, which is fastest.
	* Memory usage can be reduced by setting maxrank to 2 or 4.
	*/

        public double supernodal_switch;   /* supernodal vs simplicial factorization */
        public int supernodal;     /* If Common->supernodal <= CHOLMOD_SIMPLICIAL
				 * (0) then cholmod_analyze performs a
	* simplicial analysis.  If >= CHOLMOD_SUPERNODAL (2), then a supernodal
	* analysis is performed.  If == CHOLMOD_AUTO (1) and
	* flop/nnz(L) < Common->supernodal_switch, then a simplicial analysis
	* is done.  A supernodal analysis done otherwise.
	* Default:  CHOLMOD_AUTO.  Default supernodal_switch = 40 */

        public int final_asis; /* If TRUE, then ignore the other final_* parameters
			 * (except for final_pack).
			 * The factor is left as-is when done.  Default: TRUE.*/

        public int final_super;    /* If TRUE, leave a factor in supernodal form when
			 * supernodal factorization is finished.  If FALSE,
			 * then convert to a simplicial factor when done.
			 * Default: TRUE */

        public int final_ll;   /* If TRUE, leave factor in LL' form when done.
			 * Otherwise, leave in LDL' form.  Default: FALSE */

        public int final_pack; /* If TRUE, pack the columns when done.  If TRUE, and
			 * cholmod_factorize is called with a symbolic L, L is
	* allocated with exactly the space required, using L->ColCount.  If you
	* plan on modifying the factorization, set Common->final_pack to FALSE,
	* and each column will be given a little extra slack space for future
	* growth in fill-in due to updates.  Default: TRUE */

        public int final_monotonic;   /* If TRUE, ensure columns are monotonic when done.
			 * Default: TRUE */

        public int final_resymbol;/* if cholmod_factorize performed a supernodal
			 * factorization, final_resymbol is true, and
	* final_super is FALSE (convert a simplicial numeric factorization),
	* then numerically zero entries that resulted from relaxed supernodal
	* amalgamation are removed.  This does not remove entries that are zero
	* due to exact numeric cancellation, since doing so would break the
	* update/downdate rowadd/rowdel routines.  Default: FALSE. */

        /* supernodal relaxed amalgamation parameters: */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] zrelax;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public size_t[] nrelax;

        /* Let ns be the total number of columns in two adjacent supernodes.
         * Let z be the fraction of zero entries in the two supernodes if they
         * are merged (z includes zero entries from prior amalgamations).  The
         * two supernodes are merged if:
         *    (ns <= nrelax [0]) || (no new zero entries added) ||
         *    (ns <= nrelax [1] && z < zrelax [0]) ||
         *    (ns <= nrelax [2] && z < zrelax [1]) || (z < zrelax [2])
         *
         * Default parameters result in the following rule:
         *    (ns <= 4) || (no new zero entries added) ||
         *    (ns <= 16 && z < 0.8) || (ns <= 48 && z < 0.1) || (z < 0.05)
         */

        public int prefer_zomplex;    /* X = cholmod_solve (sys, L, B, Common) computes
			     * x=A\b or solves a related system.  If L and B are
	 * both real, then X is real.  Otherwise, X is returned as
	 * CHOLMOD_COMPLEX if Common->prefer_zomplex is FALSE, or
	 * CHOLMOD_ZOMPLEX if Common->prefer_zomplex is TRUE.  This parameter
	 * is needed because there is no supernodal zomplex L.  Suppose the
	 * caller wants all complex matrices to be stored in zomplex form
	 * (MATLAB, for example).  A supernodal L is returned in complex form
	 * if A is zomplex.  B can be real, and thus X = cholmod_solve (L,B)
	 * should return X as zomplex.  This cannot be inferred from the input
	 * arguments L and B.  Default: FALSE, since all data types are
	 * supported in CHOLMOD_COMPLEX form and since this is the native type
	 * of LAPACK and the BLAS.  Note that the MATLAB/cholmod.c mexFunction
	 * sets this parameter to TRUE, since MATLAB matrices are in
	 * CHOLMOD_ZOMPLEX form.
	 */

        public int prefer_upper;       /* cholmod_analyze and cholmod_factorize work
			     * fastest when a symmetric matrix is stored in
	 * upper triangular form when a fill-reducing ordering is used.  In
	 * MATLAB, this corresponds to how x=A\b works.  When the matrix is
	 * ordered as-is, they work fastest when a symmetric matrix is in lower
	 * triangular form.  In MATLAB, R=chol(A) does the opposite.  This
	 * parameter affects only how cholmod_read returns a symmetric matrix.
	 * If TRUE (the default case), a symmetric matrix is always returned in
	 * upper-triangular form (A->stype = 1).  */

        public int quick_return_if_not_posdef; /* if TRUE, the supernodal numeric
					 * factorization will return quickly if
	* the matrix is not positive definite.  Default: FALSE. */

        public int prefer_binary;      /* cholmod_read_triplet converts a symmetric
			     * pattern-only matrix into a real matrix.  If
	* prefer_binary is FALSE, the diagonal entries are set to 1 + the degree
	* of the row/column, and off-diagonal entries are set to -1 (resulting
	* in a positive definite matrix if the diagonal is zero-free).  Most
	* symmetric patterns are the pattern a positive definite matrix.  If
	* this parameter is TRUE, then the matrix is returned with a 1 in each
	* entry, instead.  Default: FALSE.  Added in v1.3. */

        /* ---------------------------------------------------------------------- */
        /* printing and error handling options */
        /* ---------------------------------------------------------------------- */

        public int print;      /* print level. Default: 3 */
        public int precise;    /* if TRUE, print 16 digits.  Otherwise print 5 */

        /* CHOLMOD print_function replaced with SuiteSparse_config.print_func */

        public int try_catch;  /* if TRUE, then ignore errors; CHOLMOD is in the middle
			 * of a try/catch block.  No error message is printed
	 * and the Common->error_handler function is not called. */

        public ErrorHandler error_handler;

        /* Common->error_handler is the user's error handling routine.  If not
         * NULL, this routine is called if an error occurs in CHOLMOD.  status
         * can be CHOLMOD_OK (0), negative for a fatal error, and positive for
         * a warning. file is a string containing the name of the source code
         * file where the error occured, and line is the line number in that
         * file.  message is a string describing the error in more detail. */

        /* ---------------------------------------------------------------------- */
        /* ordering options */
        /* ---------------------------------------------------------------------- */

        /* The cholmod_analyze routine can try many different orderings and select
         * the best one.  It can also try one ordering method multiple times, with
         * different parameter settings.  The default is to use three orderings,
         * the user's permutation (if provided), AMD which is the fastest ordering
         * and generally gives good fill-in, and METIS.  CHOLMOD's nested dissection
         * (METIS with a constrained AMD) usually gives a better ordering than METIS
         * alone (by about 5% to 10%) but it takes more time.
         *
         * If you know the method that is best for your matrix, set Common->nmethods
         * to 1 and set Common->method [0] to the set of parameters for that method.
         * If you set it to 1 and do not provide a permutation, then only AMD will
         * be called.
         *
         * If METIS is not available, the default # of methods tried is 2 (the user
         * permutation, if any, and AMD).
         *
         * To try other methods, set Common->nmethods to the number of methods you
         * want to try.  The suite of default methods and their parameters is
         * described in the cholmod_defaults routine, and summarized here:
         *
         *	    Common->method [i]:
         *	    i = 0: user-provided ordering (cholmod_analyze_p only)
         *	    i = 1: AMD (for both A and A*A')
         *	    i = 2: METIS
         *	    i = 3: CHOLMOD's nested dissection (NESDIS), default parameters
         *	    i = 4: natural
         *	    i = 5: NESDIS with nd_small = 20000
         *	    i = 6: NESDIS with nd_small = 4, no constrained minimum degree
         *	    i = 7: NESDIS with no dense node removal
         *	    i = 8: AMD for A, COLAMD for A*A'
         *
         * You can modify the suite of methods you wish to try by modifying
         * Common.method [...] after calling cholmod_start or cholmod_defaults.
         *
         * For example, to use AMD, followed by a weighted postordering:
         *
         *	    Common->nmethods = 1 ;
         *	    Common->method [0].ordering = CHOLMOD_AMD ;
         *	    Common->postorder = TRUE ;
         *
         * To use the natural ordering (with no postordering):
         *
         *	    Common->nmethods = 1 ;
         *	    Common->method [0].ordering = CHOLMOD_NATURAL ;
         *	    Common->postorder = FALSE ;
         *
         * If you are going to factorize hundreds or more matrices with the same
         * nonzero pattern, you may wish to spend a great deal of time finding a
         * good permutation.  In this case, try setting Common->nmethods to 9.
         * The time spent in cholmod_analysis will be very high, but you need to
         * call it only once.
         *
         * cholmod_analyze sets Common->current to a value between 0 and nmethods-1.
         * Each ordering method uses the set of options defined by this parameter.
         */

        public int nmethods;   /* The number of ordering methods to try.  Default: 0.
			 * nmethods = 0 is a special case.  cholmod_analyze
	* will try the user-provided ordering (if given) and AMD.  Let fl and
	* lnz be the flop count and nonzeros in L from AMD's ordering.  Let
	* anz be the number of nonzeros in the upper or lower triangular part
	* of the symmetric matrix A.  If fl/lnz < 500 or lnz/anz < 5, then this
	* is a good ordering, and METIS is not attempted.  Otherwise, METIS is
	* tried.   The best ordering found is used.  If nmethods > 0, the
	* methods used are given in the method[ ] array, below.  The first
	* three methods in the default suite of orderings is (1) use the given
	* permutation (if provided), (2) use AMD, and (3) use METIS.  Maximum
	* allowed value is CHOLMOD_MAXMETHODS.  */

        public int current;    /* The current method being tried.  Default: 0.  Valid
			 * range is 0 to nmethods-1. */

        public int selected;   /* The best method found. */

        /* The suite of ordering methods and parameters: */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.CHOLMOD_MAXMETHODS + 1)]
        public CholmodMethod[] method;

        public int postorder;  /* If TRUE, cholmod_analyze follows the ordering with a
			 * weighted postorder of the elimination tree.  Improves
	* supernode amalgamation.  Does not affect fundamental nnz(L) and
	* flop count.  Default: TRUE. */

        public int default_nesdis;    /* Default: FALSE.  If FALSE, then the default
			     * ordering strategy (when Common->nmethods == 0)
	* is to try the given ordering (if present), AMD, and then METIS if AMD
	* reports high fill-in.  If Common->default_nesdis is TRUE then NESDIS
	* is used instead in the default strategy. */

        /* ---------------------------------------------------------------------- */
        /* memory management, complex divide, and hypot function pointers moved */
        /* ---------------------------------------------------------------------- */

        /* Function pointers moved from here (in CHOLMOD 2.2.0) to
           SuiteSparse_config.[ch].  See CHOLMOD/Include/cholmod_back.h
           for a set of macros that can be #include'd or copied into your
           application to define these function pointers on any version of CHOLMOD.
           */

        /* ---------------------------------------------------------------------- */
        /* METIS workarounds */
        /* ---------------------------------------------------------------------- */

        /* These workarounds were put into place for METIS 4.0.1.  They are safe
           to use with METIS 5.1.0, but they might not longer be necessary. */

        public double metis_memory;   /* This is a parameter for CHOLMOD's interface to
			     * METIS, not a parameter to METIS itself.  METIS
	* uses an amount of memory that is difficult to estimate precisely
	* beforehand.  If it runs out of memory, it terminates your program.
	* All routines in CHOLMOD except for CHOLMOD's interface to METIS
	* return an error status and safely return to your program if they run
	* out of memory.  To mitigate this problem, the CHOLMOD interface
	* can allocate a single block of memory equal in size to an empirical
	* upper bound of METIS's memory usage times the Common->metis_memory
	* parameter, and then immediately free it.  It then calls METIS.  If
	* this pre-allocation fails, it is possible that METIS will fail as
	* well, and so CHOLMOD returns with an out-of-memory condition without
	* calling METIS.
	*
	* METIS_NodeND (used in the CHOLMOD_METIS ordering option) with its
	* default parameter settings typically uses about (4*nz+40n+4096)
	* times sizeof(int) memory, where nz is equal to the number of entries
	* in A for the symmetric case or AA' if an unsymmetric matrix is
	* being ordered (where nz includes both the upper and lower parts
	* of A or AA').  The observed "upper bound" (with 2 exceptions),
	* measured in an instrumented copy of METIS 4.0.1 on thousands of
	* matrices, is (10*nz+50*n+4096) * sizeof(int).  Two large matrices
	* exceeded this bound, one by almost a factor of 2 (Gupta/gupta2).
	*
	* If your program is terminated by METIS, try setting metis_memory to
	* 2.0, or even higher if needed.  By default, CHOLMOD assumes that METIS
	* does not have this problem (so that CHOLMOD will work correctly when
	* this issue is fixed in METIS).  Thus, the default value is zero.
	* This work-around is not guaranteed anyway.
	*
	* If a matrix exceeds this predicted memory usage, AMD is attempted
	* instead.  It, too, may run out of memory, but if it does so it will
	* not terminate your program.
	*/

        public double metis_dswitch;   /* METIS_NodeND in METIS 4.0.1 gives a seg */
        public size_t metis_nswitch;   /* fault with one matrix of order n = 3005 and
				 * nz = 6,036,025.  This is a very dense graph.
     * The workaround is to use AMD instead of METIS for matrices of dimension
     * greater than Common->metis_nswitch (default 3000) or more and with
     * density of Common->metis_dswitch (default 0.66) or more.
     * cholmod_nested_dissection has no problems with the same matrix, even
     * though it uses METIS_ComputeVertexSeparator on this matrix.  If this
     * seg fault does not affect you, set metis_nswitch to zero or less,
     * and CHOLMOD will not switch to AMD based just on the density of the
     * matrix (it will still switch to AMD if the metis_memory parameter
     * causes the switch).
     */

        /* ---------------------------------------------------------------------- */
        /* workspace */
        /* ---------------------------------------------------------------------- */

        /* CHOLMOD has several routines that take less time than the size of
         * workspace they require.  Allocating and initializing the workspace would
         * dominate the run time, unless workspace is allocated and initialized
         * just once.  CHOLMOD allocates this space when needed, and holds it here
         * between calls to CHOLMOD.  cholmod_start sets these pointers to NULL
         * (which is why it must be the first routine called in CHOLMOD).
         * cholmod_finish frees the workspace (which is why it must be the last
         * call to CHOLMOD).
         */

        public size_t nrow;    /* size of Flag and Head */
        public SuiteSparse_long mark;  /* mark value for Flag array */
        public size_t iworksize;   /* size of Iwork.  Upper bound: 6*nrow+ncol */
        public size_t xworksize;   /* size of Xwork,  in bytes.
			 * maxrank*nrow*sizeof(double) for update/downdate.
			 * 2*nrow*sizeof(double) otherwise */

        /* initialized workspace: contents needed between calls to CHOLMOD */
        internal IntPtr Flag; /* size nrow, an integer array.  Kept cleared between
			 * calls to cholmod rouines (Flag [i] < mark) */

        internal IntPtr Head; /* size nrow+1, an integer array. Kept cleared between
			 * calls to cholmod routines (Head [i] = EMPTY) */

        internal IntPtr Xwork;    /* a double array.  Its size varies.  It is nrow for
			 * most routines (cholmod_rowfac, cholmod_add,
	* cholmod_aat, cholmod_norm, cholmod_ssmult) for the real case, twice
	* that when the input matrices are complex or zomplex.  It is of size
	* 2*nrow for cholmod_rowadd and cholmod_rowdel.  For cholmod_updown,
	* its size is maxrank*nrow where maxrank is 2, 4, or 8.  Kept cleared
	* between calls to cholmod (set to zero). */

        /* uninitialized workspace, contents not needed between calls to CHOLMOD */
        internal IntPtr Iwork;    /* size iworksize, 2*nrow+ncol for most routines,
			 * up to 6*nrow+ncol for cholmod_analyze. */

        public int itype;      /* If CHOLMOD_LONG, Flag, Head, and Iwork are
                         * SuiteSparse_long.  Otherwise all three are int. */

        public int dtype;      /* double or float */

        /* Common->itype and Common->dtype are used to define the types of all
         * sparse matrices, triplet matrices, dense matrices, and factors
         * created using this Common struct.  The itypes and dtypes of all
         * parameters to all CHOLMOD routines must match.  */

        public int no_workspace_reallocate;   /* this is an internal flag, used as a
	* precaution by cholmod_analyze.  It is normally false.  If true,
	* cholmod_allocate_work is not allowed to reallocate any workspace;
	* they must use the existing workspace in Common (Iwork, Flag, Head,
	* and Xwork).  Added for CHOLMOD v1.1 */

        /* ---------------------------------------------------------------------- */
        /* statistics */
        /* ---------------------------------------------------------------------- */

        /* fl and lnz are set only in cholmod_analyze and cholmod_rowcolcounts,
         * in the Cholesky modudle.  modfl is set only in the Modify module. */

        public int status;     /* error code */
        public double fl;          /* LL' flop count from most recent analysis */
        public double lnz;     /* fundamental nz in L */
        public double anz;     /* nonzeros in tril(A) if A is symmetric/lower,
			     * triu(A) if symmetric/upper, or tril(A*A') if
			     * unsymmetric, in last call to cholmod_analyze. */
        public double modfl;       /* flop count from most recent update/downdate/
			     * rowadd/rowdel (excluding flops to modify the
			     * solution to Lx=b, if computed) */
        public size_t malloc_count;   /* # of objects malloc'ed minus the # free'd*/
        public size_t memory_usage;   /* peak memory usage in bytes */
        public size_t memory_inuse;   /* current memory usage in bytes */

        public double nrealloc_col;   /* # of column reallocations */
        public double nrealloc_factor;/* # of factor reallocations due to col. reallocs */
        public double ndbounds_hit;   /* # of times diagonal modified by dbound */

        public double rowfacfl;        /* # of flops in last call to cholmod_rowfac */
        public double aatfl;       /* # of flops to compute A(:,f)*A(:,f)' */

        public int called_nd;      /* TRUE if the last call to
			     * cholmod_analyze called NESDIS or METIS. */
        public int blas_ok;           /* FALSE if BLAS int overflow; TRUE otherwise */

        /* ---------------------------------------------------------------------- */
        /* SuiteSparseQR control parameters: */
        /* ---------------------------------------------------------------------- */

        public double SPQR_grain;      /* task size is >= max (total flops / grain) */
        public double SPQR_small;      /* task size is >= small */
        public int SPQR_shrink;        /* controls stack realloc method */
        public int SPQR_nthreads;      /* number of TBB threads, 0 = auto */

        /* ---------------------------------------------------------------------- */
        /* SuiteSparseQR statistics */
        /* ---------------------------------------------------------------------- */

        /* was other1 [0:3] */
        public double SPQR_flopcount;         /* flop count for SPQR */
        public double SPQR_analyze_time;      /* analysis time in seconds for SPQR */
        public double SPQR_factorize_time;    /* factorize time in seconds for SPQR */
        public double SPQR_solve_time;        /* backsolve time in seconds */

        /* was SPQR_xstat [0:3] */
        public double SPQR_flopcount_bound;   /* upper bound on flop count */
        public double SPQR_tol_used;          /* tolerance used */
        public double SPQR_norm_E_fro;        /* Frobenius norm of dropped entries */

        /* was SPQR_istat [0:9] */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public SuiteSparse_long[] SPQR_istat;

        /* ---------------------------------------------------------------------- */
        /* GPU configuration and statistics */
        /* ---------------------------------------------------------------------- */

        /*  useGPU:  1 if gpu-acceleration is requested */
        /*           0 if gpu-acceleration is prohibited */
        /*          -1 if gpu-acceleration is undefined in which case the */
        /*             environment CHOLMOD_USE_GPU will be queried and used. */
        /*             useGPU=-1 is only used by CHOLMOD and treated as 0 by SPQR */
        public int useGPU;

        /* for CHOLMOD: */
        public size_t maxGpuMemBytes;
        public double maxGpuMemFraction;

        /* for SPQR: */
        public size_t gpuMemorySize;       /* Amount of memory in bytes on the GPU */
        public double gpuKernelTime;       /* Time taken by GPU kernels */
        public SuiteSparse_long gpuFlops;  /* Number of flops performed by the GPU */
        public int gpuNumKernelLaunches;   /* Number of GPU kernel launches */

        /* If not using the GPU, these items are not used, but they should be
           present so that the CHOLMOD Common has the same size whether the GPU
           is used or not.  This way, all packages will agree on the size of
           the CHOLMOD Common, regardless of whether or not they are compiled
           with the GPU libraries or not */


        internal CHOLMOD_CUBLAS_HANDLE cublasHandle;

        /* a set of streams for general use */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.CHOLMOD_HOST_SUPERNODE_BUFFERS)]
        internal CHOLMOD_CUDASTREAM[] gpuStream;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        internal CHOLMOD_CUDAEVENT[] cublasEventPotrf;
        internal CHOLMOD_CUDAEVENT updateCKernelsComplete;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.CHOLMOD_HOST_SUPERNODE_BUFFERS)]
        internal CHOLMOD_CUDAEVENT[] updateCBuffersFree;

        internal IntPtr dev_mempool;    /* pointer to single allocation of device memory */
        internal size_t dev_mempool_size;

        internal IntPtr host_pinned_mempool;  /* pointer to single allocation of pinned mem */
        internal size_t host_pinned_mempool_size;

        internal size_t devBuffSize;
        internal int ibuffer;

        internal double syrkStart;          /* time syrk started */

        /* run times of the different parts of CHOLMOD (GPU and CPU) */
        internal double cholmod_cpu_gemm_time;
        internal double cholmod_cpu_syrk_time;
        internal double cholmod_cpu_trsm_time;
        internal double cholmod_cpu_potrf_time;
        internal double cholmod_gpu_gemm_time;
        internal double cholmod_gpu_syrk_time;
        internal double cholmod_gpu_trsm_time;
        internal double cholmod_gpu_potrf_time;
        internal double cholmod_assemble_time;
        internal double cholmod_assemble_time2;

        /* number of times the BLAS are called on the CPU and the GPU */
        internal size_t cholmod_cpu_gemm_calls;
        internal size_t cholmod_cpu_syrk_calls;
        internal size_t cholmod_cpu_trsm_calls;
        internal size_t cholmod_cpu_potrf_calls;
        internal size_t cholmod_gpu_gemm_calls;
        internal size_t cholmod_gpu_syrk_calls;
        internal size_t cholmod_gpu_trsm_calls;
        internal size_t cholmod_gpu_potrf_calls;

        public void Initialize()
        {
            zrelax = new double[3];
            nrelax = new size_t[3];

            method = new CholmodMethod[Constants.CHOLMOD_MAXMETHODS + 1];

            for (int i = 0; i < Constants.CHOLMOD_MAXMETHODS + 1; i++)
            {
                method[i].other_1 = new double[4];
                method[i].other_2 = new size_t[4];
                method[i].other_3 = new size_t[4];
            }

            SPQR_istat = new SuiteSparse_long[10];

            gpuStream = new IntPtr[Constants.CHOLMOD_HOST_SUPERNODE_BUFFERS];
            cublasEventPotrf = new IntPtr[3];
            updateCBuffersFree = new IntPtr[Constants.CHOLMOD_HOST_SUPERNODE_BUFFERS];
        }
    }
}
