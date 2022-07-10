
namespace CSparse.Interop.SuperLU
{
    using System;
    using System.Runtime.InteropServices;
    using System.Numerics;

    #region Global LU

    // Facilitate multiple factorizations with same pattern and row perm.

    struct GlobalLU
    {
        IntPtr xsup;    /* supernode and column mapping */
        IntPtr supno;
        IntPtr lsub;    /* compressed L subscripts */
        IntPtr xlsub;
        IntPtr lusup;   /* L supernodes */
        IntPtr xlusup;
        IntPtr ucol;    /* U columns */
        IntPtr usub;
        IntPtr xusub;
        int nzlmax;   /* current max size of lsub */
        int nzumax;   /*    "    "    "      ucol */
        int nzlumax;  /*    "    "    "     lusup */
        int n;        /* number of columns in the matrix */
        int MemModel; /* 0 - system malloc'd; 1 - user provided */
        int num_expansions;
        IntPtr expanders; /* Array of pointers to 4 types of memory */
        LU_stack stack;     /* use user supplied memory */
    }

    struct LU_stack
    {
        int size;
        int used;
        int top1;  /* grow upward, relative to &array[0] */
        int top2;  /* grow downward */
        IntPtr array;
    }

    #endregion

    struct mem_usage
    {
        float for_lu;
        float total_needed;
    }

    struct SuperLUStat
    {
        public IntPtr panel_histo; /* int* histogram of panel size distribution */
        public IntPtr utime;       /* double* running time at various phases */
        public IntPtr ops;         /* flops_t* operation count at various phases */
        public int TinyPivots;   /* number of tiny pivots */
        public int RefineSteps;  /* number of iterative refinement steps */
        public int expansions;   /* number of memory expansions */
    }

    internal static class NativeMethods
    {
        #region Sequential (SuperLU)

        const string SuperLU = "libsuperlu";

        [DllImport(SuperLU, EntryPoint = "StatInit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void StatInit(ref SuperLUStat A);

        [DllImport(SuperLU, EntryPoint = "StatFree", CallingConvention = CallingConvention.Cdecl)]
        public static extern void StatFree(ref SuperLUStat A);

        [DllImport(SuperLU, EntryPoint = "Destroy_SuperMatrix_Store", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Destroy_SuperMatrix_Store(ref SuperMatrix A);

        [DllImport(SuperLU, EntryPoint = "Destroy_CompCol_Matrix", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Destroy_CompCol_Matrix(ref SuperMatrix A);

        [DllImport(SuperLU, EntryPoint = "Destroy_CompRow_Matrix", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Destroy_CompRow_Matrix(ref SuperMatrix A);

        [DllImport(SuperLU, EntryPoint = "Destroy_SuperNode_Matrix", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Destroy_SuperNode_Matrix(ref SuperMatrix A);

        [DllImport(SuperLU, EntryPoint = "Destroy_CompCol_Permuted", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Destroy_CompCol_Permuted(ref SuperMatrix A);

        [DllImport(SuperLU, EntryPoint = "Destroy_Dense_Matrix", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Destroy_Dense_Matrix(ref SuperMatrix A);

        [DllImport(SuperLU, EntryPoint = "get_perm_c", CallingConvention = CallingConvention.Cdecl)]
        public static extern void get_perm_c(int n, ref SuperMatrix A, int[] p);

        [DllImport(SuperLU, EntryPoint = "set_default_options", CallingConvention = CallingConvention.Cdecl)]
        public static extern void set_default_options(ref superlu_options options);

        [DllImport(SuperLU, EntryPoint = "ilu_set_default_options", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ilu_set_default_options(ref superlu_options options);

        #region Double precision

        /*! \brief
         *
         * <pre>
         * Purpose
         * =======
         *
         * SGSSV solves the system of linear equations A*X=B, using the
         * LU factorization from SGSTRF. It performs the following steps:
         *
         *   1. If A is stored column-wise (A->Stype = SLU_NC):
         *
         *      1.1. Permute the columns of A, forming A*Pc, where Pc
         *           is a permutation matrix. For more details of this step, 
         *           see sp_preorder.c.
         *
         *      1.2. Factor A as Pr*A*Pc=L*U with the permutation Pr determined
         *           by Gaussian elimination with partial pivoting.
         *           L is unit lower triangular with offdiagonal entries
         *           bounded by 1 in magnitude, and U is upper triangular.
         *
         *      1.3. Solve the system of equations A*X=B using the factored
         *           form of A.
         *
         *   2. If A is stored row-wise (A->Stype = SLU_NR), apply the
         *      above algorithm to the transpose of A:
         *
         *      2.1. Permute columns of transpose(A) (rows of A),
         *           forming transpose(A)*Pc, where Pc is a permutation matrix. 
         *           For more details of this step, see sp_preorder.c.
         *
         *      2.2. Factor A as Pr*transpose(A)*Pc=L*U with the permutation Pr
         *           determined by Gaussian elimination with partial pivoting.
         *           L is unit lower triangular with offdiagonal entries
         *           bounded by 1 in magnitude, and U is upper triangular.
         *
         *      2.3. Solve the system of equations A*X=B using the factored
         *           form of A.
         *
         *   See supermatrix.h for the definition of 'SuperMatrix' structure.
         * 
         * Arguments
         * =========
         *
         * options (input) superlu_options_t*
         *         The structure defines the input parameters to control
         *         how the LU decomposition will be performed and how the
         *         system will be solved.
         *
         * A       (input) SuperMatrix*
         *         Matrix A in A*X=B, of dimension (A->nrow, A->ncol). The number
         *         of linear equations is A->nrow. Currently, the type of A can be:
         *         Stype = SLU_NC or SLU_NR; Dtype = SLU_S; Mtype = SLU_GE.
         *         In the future, more general A may be handled.
         *
         * perm_c  (input/output) int*
         *         If A->Stype = SLU_NC, column permutation vector of size A->ncol
         *         which defines the permutation matrix Pc; perm_c[i] = j means 
         *         column i of A is in position j in A*Pc.
         *         If A->Stype = SLU_NR, column permutation vector of size A->nrow
         *         which describes permutation of columns of transpose(A) 
         *         (rows of A) as described above.
         * 
         *         If options->ColPerm = MY_PERMC or options->Fact = SamePattern or
         *            options->Fact = SamePattern_SameRowPerm, it is an input argument.
         *            On exit, perm_c may be overwritten by the product of the input
         *            perm_c and a permutation that postorders the elimination tree
         *            of Pc'*A'*A*Pc; perm_c is not changed if the elimination tree
         *            is already in postorder.
         *         Otherwise, it is an output argument.
         * 
         * perm_r  (input/output) int*
         *         If A->Stype = SLU_NC, row permutation vector of size A->nrow, 
         *         which defines the permutation matrix Pr, and is determined 
         *         by partial pivoting.  perm_r[i] = j means row i of A is in 
         *         position j in Pr*A.
         *         If A->Stype = SLU_NR, permutation vector of size A->ncol, which
         *         determines permutation of rows of transpose(A)
         *         (columns of A) as described above.
         *
         *         If options->RowPerm = MY_PERMR or
         *            options->Fact = SamePattern_SameRowPerm, perm_r is an
         *            input argument.
         *         otherwise it is an output argument.
         *
         * L       (output) SuperMatrix*
         *         The factor L from the factorization 
         *             Pr*A*Pc=L*U              (if A->Stype = SLU_NC) or
         *             Pr*transpose(A)*Pc=L*U   (if A->Stype = SLU_NR).
         *         Uses compressed row subscripts storage for supernodes, i.e.,
         *         L has types: Stype = SLU_SC, Dtype = SLU_S, Mtype = SLU_TRLU.
         *         
         * U       (output) SuperMatrix*
         *	   The factor U from the factorization 
         *             Pr*A*Pc=L*U              (if A->Stype = SLU_NC) or
         *             Pr*transpose(A)*Pc=L*U   (if A->Stype = SLU_NR).
         *         Uses column-wise storage scheme, i.e., U has types:
         *         Stype = SLU_NC, Dtype = SLU_S, Mtype = SLU_TRU.
         *
         * B       (input/output) SuperMatrix*
         *         B has types: Stype = SLU_DN, Dtype = SLU_S, Mtype = SLU_GE.
         *         On entry, the right hand side matrix.
         *         On exit, the solution matrix if info = 0;
         *
         * stat   (output) SuperLUStat_t*
         *        Record the statistics on runtime and floating-point operation count.
         *        See util.h for the definition of 'SuperLUStat_t'.
         *
         * info    (output) int*
         *	   = 0: successful exit
         *         > 0: if info = i, and i is
         *             <= A->ncol: U(i,i) is exactly zero. The factorization has
         *                been completed, but the factor U is exactly singular,
         *                so the solution could not be computed.
         *             > A->ncol: number of bytes allocated when memory allocation
         *                failure occurred, plus A->ncol.
         * </pre>
         */
        [DllImport(SuperLU, EntryPoint = "dgssv", CallingConvention = CallingConvention.Cdecl)]
        public static extern void dgssv(ref superlu_options options, ref SuperMatrix A, int[] perm_c, int[] perm_r, ref SuperMatrix L,
              ref SuperMatrix U, ref SuperMatrix B, ref SuperLUStat stat, out int info);

        /*! \brief
         *
         * <pre>
         * Purpose
         * =======
         *
         * SGSSVX solves the system of linear equations A*X=B or A'*X=B, using
         * the LU factorization from sgstrf(). Error bounds on the solution and
         * a condition estimate are also provided. It performs the following steps:
         *
         *   1. If A is stored column-wise (A->Stype = SLU_NC):
         *  
         *      1.1. If options->Equil = YES, scaling factors are computed to
         *           equilibrate the system:
         *           options->Trans = NOTRANS:
         *               diag(R)*A*diag(C) *inv(diag(C))*X = diag(R)*B
         *           options->Trans = TRANS:
         *               (diag(R)*A*diag(C))**T *inv(diag(R))*X = diag(C)*B
         *           options->Trans = CONJ:
         *               (diag(R)*A*diag(C))**H *inv(diag(R))*X = diag(C)*B
         *           Whether or not the system will be equilibrated depends on the
         *           scaling of the matrix A, but if equilibration is used, A is
         *           overwritten by diag(R)*A*diag(C) and B by diag(R)*B
         *           (if options->Trans=NOTRANS) or diag(C)*B (if options->Trans
         *           = TRANS or CONJ).
         *
         *      1.2. Permute columns of A, forming A*Pc, where Pc is a permutation
         *           matrix that usually preserves sparsity.
         *           For more details of this step, see sp_preorder.c.
         *
         *      1.3. If options->Fact != FACTORED, the LU decomposition is used to
         *           factor the matrix A (after equilibration if options->Equil = YES)
         *           as Pr*A*Pc = L*U, with Pr determined by partial pivoting.
         *
         *      1.4. Compute the reciprocal pivot growth factor.
         *
         *      1.5. If some U(i,i) = 0, so that U is exactly singular, then the
         *           routine returns with info = i. Otherwise, the factored form of 
         *           A is used to estimate the condition number of the matrix A. If
         *           the reciprocal of the condition number is less than machine
         *           precision, info = A->ncol+1 is returned as a warning, but the
         *           routine still goes on to solve for X and computes error bounds
         *           as described below.
         *
         *      1.6. The system of equations is solved for X using the factored form
         *           of A.
         *
         *      1.7. If options->IterRefine != NOREFINE, iterative refinement is
         *           applied to improve the computed solution matrix and calculate
         *           error bounds and backward error estimates for it.
         *
         *      1.8. If equilibration was used, the matrix X is premultiplied by
         *           diag(C) (if options->Trans = NOTRANS) or diag(R)
         *           (if options->Trans = TRANS or CONJ) so that it solves the
         *           original system before equilibration.
         *
         *   2. If A is stored row-wise (A->Stype = SLU_NR), apply the above algorithm
         *      to the transpose of A:
         *
         *      2.1. If options->Equil = YES, scaling factors are computed to
         *           equilibrate the system:
         *           options->Trans = NOTRANS:
         *               diag(R)*A*diag(C) *inv(diag(C))*X = diag(R)*B
         *           options->Trans = TRANS:
         *               (diag(R)*A*diag(C))**T *inv(diag(R))*X = diag(C)*B
         *           options->Trans = CONJ:
         *               (diag(R)*A*diag(C))**H *inv(diag(R))*X = diag(C)*B
         *           Whether or not the system will be equilibrated depends on the
         *           scaling of the matrix A, but if equilibration is used, A' is
         *           overwritten by diag(R)*A'*diag(C) and B by diag(R)*B 
         *           (if trans='N') or diag(C)*B (if trans = 'T' or 'C').
         *
         *      2.2. Permute columns of transpose(A) (rows of A), 
         *           forming transpose(A)*Pc, where Pc is a permutation matrix that 
         *           usually preserves sparsity.
         *           For more details of this step, see sp_preorder.c.
         *
         *      2.3. If options->Fact != FACTORED, the LU decomposition is used to
         *           factor the transpose(A) (after equilibration if 
         *           options->Fact = YES) as Pr*transpose(A)*Pc = L*U with the
         *           permutation Pr determined by partial pivoting.
         *
         *      2.4. Compute the reciprocal pivot growth factor.
         *
         *      2.5. If some U(i,i) = 0, so that U is exactly singular, then the
         *           routine returns with info = i. Otherwise, the factored form 
         *           of transpose(A) is used to estimate the condition number of the
         *           matrix A. If the reciprocal of the condition number
         *           is less than machine precision, info = A->nrow+1 is returned as
         *           a warning, but the routine still goes on to solve for X and
         *           computes error bounds as described below.
         *
         *      2.6. The system of equations is solved for X using the factored form
         *           of transpose(A).
         *
         *      2.7. If options->IterRefine != NOREFINE, iterative refinement is
         *           applied to improve the computed solution matrix and calculate
         *           error bounds and backward error estimates for it.
         *
         *      2.8. If equilibration was used, the matrix X is premultiplied by
         *           diag(C) (if options->Trans = NOTRANS) or diag(R) 
         *           (if options->Trans = TRANS or CONJ) so that it solves the
         *           original system before equilibration.
         *
         *   See supermatrix.h for the definition of 'SuperMatrix' structure.
         *
         * Arguments
         * =========
         *
         * options (input) superlu_options_t*
         *         The structure defines the input parameters to control
         *         how the LU decomposition will be performed and how the
         *         system will be solved.
         *
         * A       (input/output) SuperMatrix*
         *         Matrix A in A*X=B, of dimension (A->nrow, A->ncol). The number
         *         of the linear equations is A->nrow. Currently, the type of A can be:
         *         Stype = SLU_NC or SLU_NR, Dtype = SLU_D, Mtype = SLU_GE.
         *         In the future, more general A may be handled.
         *
         *         On entry, If options->Fact = FACTORED and equed is not 'N', 
         *         then A must have been equilibrated by the scaling factors in
         *         R and/or C.  
         *         On exit, A is not modified if options->Equil = NO, or if 
         *         options->Equil = YES but equed = 'N' on exit.
         *         Otherwise, if options->Equil = YES and equed is not 'N',
         *         A is scaled as follows:
         *         If A->Stype = SLU_NC:
         *           equed = 'R':  A := diag(R) * A
         *           equed = 'C':  A := A * diag(C)
         *           equed = 'B':  A := diag(R) * A * diag(C).
         *         If A->Stype = SLU_NR:
         *           equed = 'R':  transpose(A) := diag(R) * transpose(A)
         *           equed = 'C':  transpose(A) := transpose(A) * diag(C)
         *           equed = 'B':  transpose(A) := diag(R) * transpose(A) * diag(C).
         *
         * perm_c  (input/output) int*
         *	   If A->Stype = SLU_NC, Column permutation vector of size A->ncol,
         *         which defines the permutation matrix Pc; perm_c[i] = j means
         *         column i of A is in position j in A*Pc.
         *         On exit, perm_c may be overwritten by the product of the input
         *         perm_c and a permutation that postorders the elimination tree
         *         of Pc'*A'*A*Pc; perm_c is not changed if the elimination tree
         *         is already in postorder.
         *
         *         If A->Stype = SLU_NR, column permutation vector of size A->nrow,
         *         which describes permutation of columns of transpose(A) 
         *         (rows of A) as described above.
         * 
         * perm_r  (input/output) int*
         *         If A->Stype = SLU_NC, row permutation vector of size A->nrow, 
         *         which defines the permutation matrix Pr, and is determined
         *         by partial pivoting.  perm_r[i] = j means row i of A is in 
         *         position j in Pr*A.
         *
         *         If A->Stype = SLU_NR, permutation vector of size A->ncol, which
         *         determines permutation of rows of transpose(A)
         *         (columns of A) as described above.
         *
         *         If options->Fact = SamePattern_SameRowPerm, the pivoting routine
         *         will try to use the input perm_r, unless a certain threshold
         *         criterion is violated. In that case, perm_r is overwritten by a
         *         new permutation determined by partial pivoting or diagonal
         *         threshold pivoting.
         *         Otherwise, perm_r is output argument.
         * 
         * etree   (input/output) int*,  dimension (A->ncol)
         *         Elimination tree of Pc'*A'*A*Pc.
         *         If options->Fact != FACTORED and options->Fact != DOFACT,
         *         etree is an input argument, otherwise it is an output argument.
         *         Note: etree is a vector of parent pointers for a forest whose
         *         vertices are the integers 0 to A->ncol-1; etree[root]==A->ncol.
         *
         * equed   (input/output) char*
         *         Specifies the form of equilibration that was done.
         *         = 'N': No equilibration.
         *         = 'R': Row equilibration, i.e., A was premultiplied by diag(R).
         *         = 'C': Column equilibration, i.e., A was postmultiplied by diag(C).
         *         = 'B': Both row and column equilibration, i.e., A was replaced 
         *                by diag(R)*A*diag(C).
         *         If options->Fact = FACTORED, equed is an input argument,
         *         otherwise it is an output argument.
         *
         * R       (input/output) float*, dimension (A->nrow)
         *         The row scale factors for A or transpose(A).
         *         If equed = 'R' or 'B', A (if A->Stype = SLU_NC) or transpose(A)
         *             (if A->Stype = SLU_NR) is multiplied on the left by diag(R).
         *         If equed = 'N' or 'C', R is not accessed.
         *         If options->Fact = FACTORED, R is an input argument,
         *             otherwise, R is output.
         *         If options->zFact = FACTORED and equed = 'R' or 'B', each element
         *             of R must be positive.
         * 
         * C       (input/output) float*, dimension (A->ncol)
         *         The column scale factors for A or transpose(A).
         *         If equed = 'C' or 'B', A (if A->Stype = SLU_NC) or transpose(A)
         *             (if A->Stype = SLU_NR) is multiplied on the right by diag(C).
         *         If equed = 'N' or 'R', C is not accessed.
         *         If options->Fact = FACTORED, C is an input argument,
         *             otherwise, C is output.
         *         If options->Fact = FACTORED and equed = 'C' or 'B', each element
         *             of C must be positive.
         *         
         * L       (output) SuperMatrix*
         *	   The factor L from the factorization
         *             Pr*A*Pc=L*U              (if A->Stype SLU_= NC) or
         *             Pr*transpose(A)*Pc=L*U   (if A->Stype = SLU_NR).
         *         Uses compressed row subscripts storage for supernodes, i.e.,
         *         L has types: Stype = SLU_SC, Dtype = SLU_S, Mtype = SLU_TRLU.
         *
         * U       (output) SuperMatrix*
         *	   The factor U from the factorization
         *             Pr*A*Pc=L*U              (if A->Stype = SLU_NC) or
         *             Pr*transpose(A)*Pc=L*U   (if A->Stype = SLU_NR).
         *         Uses column-wise storage scheme, i.e., U has types:
         *         Stype = SLU_NC, Dtype = SLU_S, Mtype = SLU_TRU.
         *
         * work    (workspace/output) void*, size (lwork) (in bytes)
         *         User supplied workspace, should be large enough
         *         to hold data structures for factors L and U.
         *         On exit, if fact is not 'F', L and U point to this array.
         *
         * lwork   (input) int
         *         Specifies the size of work array in bytes.
         *         = 0:  allocate space internally by system malloc;
         *         > 0:  use user-supplied work array of length lwork in bytes,
         *               returns error if space runs out.
         *         = -1: the routine guesses the amount of space needed without
         *               performing the factorization, and returns it in
         *               mem_usage->total_needed; no other side effects.
         *
         *         See argument 'mem_usage' for memory usage statistics.
         *
         * B       (input/output) SuperMatrix*
         *         B has types: Stype = SLU_DN, Dtype = SLU_S, Mtype = SLU_GE.
         *         On entry, the right hand side matrix.
         *         If B->ncol = 0, only LU decomposition is performed, the triangular
         *                         solve is skipped.
         *         On exit,
         *            if equed = 'N', B is not modified; otherwise
         *            if A->Stype = SLU_NC:
         *               if options->Trans = NOTRANS and equed = 'R' or 'B',
         *                  B is overwritten by diag(R)*B;
         *               if options->Trans = TRANS or CONJ and equed = 'C' of 'B',
         *                  B is overwritten by diag(C)*B;
         *            if A->Stype = SLU_NR:
         *               if options->Trans = NOTRANS and equed = 'C' or 'B',
         *                  B is overwritten by diag(C)*B;
         *               if options->Trans = TRANS or CONJ and equed = 'R' of 'B',
         *                  B is overwritten by diag(R)*B.
         *
         * X       (output) SuperMatrix*
         *         X has types: Stype = SLU_DN, Dtype = SLU_S, Mtype = SLU_GE. 
         *         If info = 0 or info = A->ncol+1, X contains the solution matrix
         *         to the original system of equations. Note that A and B are modified
         *         on exit if equed is not 'N', and the solution to the equilibrated
         *         system is inv(diag(C))*X if options->Trans = NOTRANS and
         *         equed = 'C' or 'B', or inv(diag(R))*X if options->Trans = 'T' or 'C'
         *         and equed = 'R' or 'B'.
         *
         * recip_pivot_growth (output) float*
         *         The reciprocal pivot growth factor max_j( norm(A_j)/norm(U_j) ).
         *         The infinity norm is used. If recip_pivot_growth is much less
         *         than 1, the stability of the LU factorization could be poor.
         *
         * rcond   (output) float*
         *         The estimate of the reciprocal condition number of the matrix A
         *         after equilibration (if done). If rcond is less than the machine
         *         precision (in particular, if rcond = 0), the matrix is singular
         *         to working precision. This condition is indicated by a return
         *         code of info > 0.
         *
         * FERR    (output) float*, dimension (B->ncol)   
         *         The estimated forward error bound for each solution vector   
         *         X(j) (the j-th column of the solution matrix X).   
         *         If XTRUE is the true solution corresponding to X(j), FERR(j) 
         *         is an estimated upper bound for the magnitude of the largest 
         *         element in (X(j) - XTRUE) divided by the magnitude of the   
         *         largest element in X(j).  The estimate is as reliable as   
         *         the estimate for RCOND, and is almost always a slight   
         *         overestimate of the true error.
         *         If options->IterRefine = NOREFINE, ferr = 1.0.
         *
         * BERR    (output) float*, dimension (B->ncol)
         *         The componentwise relative backward error of each solution   
         *         vector X(j) (i.e., the smallest relative change in   
         *         any element of A or B that makes X(j) an exact solution).
         *         If options->IterRefine = NOREFINE, berr = 1.0.
         *
         * Glu      (input/output) GlobalLU_t *
         *          If options->Fact == SamePattern_SameRowPerm, it is an input;
         *              The matrix A will be factorized assuming that a 
         *              factorization of a matrix with the same sparsity pattern
         *              and similar numerical values was performed prior to this one.
         *              Therefore, this factorization will reuse both row and column
         *		scaling factors R and C, both row and column permutation
         *		vectors perm_r and perm_c, and the L & U data structures
         *		set up from the previous factorization.
         *          Otherwise, it is an output.
         *
         * mem_usage (output) mem_usage_t*
         *         Record the memory usage statistics, consisting of following fields:
         *         - for_lu (float)
         *           The amount of space used in bytes for L\U data structures.
         *         - total_needed (float)
         *           The amount of space needed in bytes to perform factorization.
         *         - expansions (int)
         *           The number of memory expansions during the LU factorization.
         *
         * stat   (output) SuperLUStat_t*
         *        Record the statistics on runtime and floating-point operation count.
         *        See slu_util.h for the definition of 'SuperLUStat_t'.
         *
         * info    (output) int*
         *         = 0: successful exit   
         *         < 0: if info = -i, the i-th argument had an illegal value   
         *         > 0: if info = i, and i is   
         *              <= A->ncol: U(i,i) is exactly zero. The factorization has   
         *                    been completed, but the factor U is exactly   
         *                    singular, so the solution and error bounds   
         *                    could not be computed.   
         *              = A->ncol+1: U is nonsingular, but RCOND is less than machine
         *                    precision, meaning that the matrix is singular to
         *                    working precision. Nevertheless, the solution and
         *                    error bounds are computed because there are a number
         *                    of situations where the computed solution can be more
         *                    accurate than the value of RCOND would suggest.   
         *              > A->ncol+1: number of bytes allocated when memory allocation
         *                    failure occurred, plus A->ncol.
         * </pre>
         */
        [DllImport(SuperLU, EntryPoint = "dgssvx", CallingConvention = CallingConvention.Cdecl)]
        public static extern void dgssvx(ref superlu_options options, ref SuperMatrix A, int[] perm_c, int[] perm_r, int[] etree,
               byte[] equed, double[] R, double[] C, ref SuperMatrix L, ref SuperMatrix U,
               IntPtr work, int lwork, ref SuperMatrix _B, ref SuperMatrix X,
               out double recip_pivot_growth, out double rcond, double[] ferr, double[] berr,
               ref GlobalLU global, ref mem_usage mem, ref SuperLUStat stat, out int info);

        /* ILU */

        /*! \brief
         *
         * <pre>
         * Purpose
         * =======
         *
         * SGSISX computes an approximate solutions of linear equations
         * A*X=B or A'*X=B, using the ILU factorization from sgsitrf().
         * An estimation of the condition number is provided. 
         * The routine performs the following steps:
         *
         *   1. If A is stored column-wise (A->Stype = SLU_NC):
         *  
         *	1.1. If options->Equil = YES or options->RowPerm = LargeDiag, scaling
         *	     factors are computed to equilibrate the system:
         *	     options->Trans = NOTRANS:
         *		 diag(R)*A*diag(C) *inv(diag(C))*X = diag(R)*B
         *	     options->Trans = TRANS:
         *		 (diag(R)*A*diag(C))**T *inv(diag(R))*X = diag(C)*B
         *	     options->Trans = CONJ:
         *		 (diag(R)*A*diag(C))**H *inv(diag(R))*X = diag(C)*B
         *	     Whether or not the system will be equilibrated depends on the
         *	     scaling of the matrix A, but if equilibration is used, A is
         *	     overwritten by diag(R)*A*diag(C) and B by diag(R)*B
         *	     (if options->Trans=NOTRANS) or diag(C)*B (if options->Trans
         *	     = TRANS or CONJ).
         *
         *	1.2. Permute columns of A, forming A*Pc, where Pc is a permutation
         *	     matrix that usually preserves sparsity.
         *	     For more details of this step, see sp_preorder.c.
         *
         *	1.3. If options->Fact != FACTORED, the LU decomposition is used to
         *	     factor the matrix A (after equilibration if options->Equil = YES)
         *	     as Pr*A*Pc = L*U, with Pr determined by partial pivoting.
         *
         *	1.4. Compute the reciprocal pivot growth factor.
         *
         *	1.5. If some U(i,i) = 0, so that U is exactly singular, then the
         *	     routine fills a small number on the diagonal entry, that is
         *		U(i,i) = ||A(:,i)||_oo * options->ILU_FillTol ** (1 - i / n),
         *	     and info will be increased by 1. The factored form of A is used
         *	     to estimate the condition number of the preconditioner. If the
         *	     reciprocal of the condition number is less than machine precision,
         *	     info = A->ncol+1 is returned as a warning, but the routine still
         *	     goes on to solve for X.
         *
         *	1.6. The system of equations is solved for X using the factored form
         *	     of A.
         *
         *	1.7. options->IterRefine is not used
         *
         *	1.8. If equilibration was used, the matrix X is premultiplied by
         *	     diag(C) (if options->Trans = NOTRANS) or diag(R)
         *	     (if options->Trans = TRANS or CONJ) so that it solves the
         *	     original system before equilibration.
         *
         *	1.9. options for ILU only
         *	     1) If options->RowPerm = LargeDiag, MC64 is used to scale and
         *		permute the matrix to an I-matrix, that is Pr*Dr*A*Dc has
         *		entries of modulus 1 on the diagonal and off-diagonal entries
         *		of modulus at most 1. If MC64 fails, dgsequ() is used to
         *		equilibrate the system.
         *              ( Default: LargeDiag )
         *	     2) options->ILU_DropTol = tau is the threshold for dropping.
         *		For L, it is used directly (for the whole row in a supernode);
         *		For U, ||A(:,i)||_oo * tau is used as the threshold
         *	        for the	i-th column.
         *		If a secondary dropping rule is required, tau will
         *	        also be used to compute the second threshold.
         *              ( Default: 1e-4 )
         *	     3) options->ILU_FillFactor = gamma, used as the initial guess
         *		of memory growth.
         *		If a secondary dropping rule is required, it will also
         *              be used as an upper bound of the memory.
         *              ( Default: 10 )
         *	     4) options->ILU_DropRule specifies the dropping rule.
         *		Option	      Meaning
         *		======	      ===========
         *		DROP_BASIC:   Basic dropping rule, supernodal based ILUTP(tau).
         *		DROP_PROWS:   Supernodal based ILUTP(p,tau), p = gamma*nnz(A)/n.
         *		DROP_COLUMN:  Variant of ILUTP(p,tau), for j-th column,
         *			      p = gamma * nnz(A(:,j)).
         *		DROP_AREA:    Variation of ILUTP, for j-th column, use
         *			      nnz(F(:,1:j)) / nnz(A(:,1:j)) to control memory.
         *		DROP_DYNAMIC: Modify the threshold tau during factorizaion:
         *			      If nnz(L(:,1:j)) / nnz(A(:,1:j)) > gamma
         *				  tau_L(j) := MIN(tau_0, tau_L(j-1) * 2);
         *			      Otherwise
         *				  tau_L(j) := MAX(tau_0, tau_L(j-1) / 2);
         *			      tau_U(j) uses the similar rule.
         *			      NOTE: the thresholds used by L and U are separate.
         *		DROP_INTERP:  Compute the second dropping threshold by
         *			      interpolation instead of sorting (default).
         *			      In this case, the actual fill ratio is not
         *			      guaranteed smaller than gamma.
         *		DROP_PROWS, DROP_COLUMN and DROP_AREA are mutually exclusive.
         *		( Default: DROP_BASIC | DROP_AREA )
         *	     5) options->ILU_Norm is the criterion of measuring the magnitude
         *		of a row in a supernode of L. ( Default is INF_NORM )
         *		options->ILU_Norm	RowSize(x[1:n])
         *		=================	===============
         *		ONE_NORM		||x||_1 / n
         *		TWO_NORM		||x||_2 / sqrt(n)
         *		INF_NORM		max{|x[i]|}
         *	     6) options->ILU_MILU specifies the type of MILU's variation.
         *		= SILU: do not perform Modified ILU;
         *		= SMILU_1 (not recommended):
         *		    U(i,i) := U(i,i) + sum(dropped entries);
         *		= SMILU_2:
         *		    U(i,i) := U(i,i) + SGN(U(i,i)) * sum(dropped entries);
         *		= SMILU_3:
         *		    U(i,i) := U(i,i) + SGN(U(i,i)) * sum(|dropped entries|);
         *		NOTE: Even SMILU_1 does not preserve the column sum because of
         *		late dropping.
         *              ( Default: SILU )
         *	     7) options->ILU_FillTol is used as the perturbation when
         *		encountering zero pivots. If some U(i,i) = 0, so that U is
         *		exactly singular, then
         *		   U(i,i) := ||A(:,i)|| * options->ILU_FillTol ** (1 - i / n).
         *              ( Default: 1e-2 )
         *
         *   2. If A is stored row-wise (A->Stype = SLU_NR), apply the above algorithm
         *	to the transpose of A:
         *
         *	2.1. If options->Equil = YES or options->RowPerm = LargeDiag, scaling
         *	     factors are computed to equilibrate the system:
         *	     options->Trans = NOTRANS:
         *		 diag(R)*A*diag(C) *inv(diag(C))*X = diag(R)*B
         *	     options->Trans = TRANS:
         *		 (diag(R)*A*diag(C))**T *inv(diag(R))*X = diag(C)*B
         *	     options->Trans = CONJ:
         *		 (diag(R)*A*diag(C))**H *inv(diag(R))*X = diag(C)*B
         *	     Whether or not the system will be equilibrated depends on the
         *	     scaling of the matrix A, but if equilibration is used, A' is
         *	     overwritten by diag(R)*A'*diag(C) and B by diag(R)*B
         *	     (if trans='N') or diag(C)*B (if trans = 'T' or 'C').
         *
         *	2.2. Permute columns of transpose(A) (rows of A),
         *	     forming transpose(A)*Pc, where Pc is a permutation matrix that
         *	     usually preserves sparsity.
         *	     For more details of this step, see sp_preorder.c.
         *
         *	2.3. If options->Fact != FACTORED, the LU decomposition is used to
         *	     factor the transpose(A) (after equilibration if
         *	     options->Fact = YES) as Pr*transpose(A)*Pc = L*U with the
         *	     permutation Pr determined by partial pivoting.
         *
         *	2.4. Compute the reciprocal pivot growth factor.
         *
         *	2.5. If some U(i,i) = 0, so that U is exactly singular, then the
         *	     routine fills a small number on the diagonal entry, that is
         *		 U(i,i) = ||A(:,i)||_oo * options->ILU_FillTol ** (1 - i / n).
         *	     And info will be increased by 1. The factored form of A is used
         *	     to estimate the condition number of the preconditioner. If the
         *	     reciprocal of the condition number is less than machine precision,
         *	     info = A->ncol+1 is returned as a warning, but the routine still
         *	     goes on to solve for X.
         *
         *	2.6. The system of equations is solved for X using the factored form
         *	     of transpose(A).
         *
         *	2.7. If options->IterRefine is not used.
         *
         *	2.8. If equilibration was used, the matrix X is premultiplied by
         *	     diag(C) (if options->Trans = NOTRANS) or diag(R)
         *	     (if options->Trans = TRANS or CONJ) so that it solves the
         *	     original system before equilibration.
         *
         *   See supermatrix.h for the definition of 'SuperMatrix' structure.
         *
         * Arguments
         * =========
         *
         * options (input) superlu_options_t*
         *	   The structure defines the input parameters to control
         *	   how the LU decomposition will be performed and how the
         *	   system will be solved.
         *
         * A	   (input/output) SuperMatrix*
         *	   Matrix A in A*X=B, of dimension (A->nrow, A->ncol). The number
         *	   of the linear equations is A->nrow. Currently, the type of A can be:
         *	   Stype = SLU_NC or SLU_NR, Dtype = SLU_S, Mtype = SLU_GE.
         *	   In the future, more general A may be handled.
         *
         *	   On entry, If options->Fact = FACTORED and equed is not 'N',
         *	   then A must have been equilibrated by the scaling factors in
         *	   R and/or C.
         *	   On exit, A is not modified
         *         if options->Equil = NO, or
         *         if options->Equil = YES but equed = 'N' on exit, or
         *         if options->RowPerm = NO.
         *
         *	   Otherwise, if options->Equil = YES and equed is not 'N',
         *	   A is scaled as follows:
         *	   If A->Stype = SLU_NC:
         *	     equed = 'R':  A := diag(R) * A
         *	     equed = 'C':  A := A * diag(C)
         *	     equed = 'B':  A := diag(R) * A * diag(C).
         *	   If A->Stype = SLU_NR:
         *	     equed = 'R':  transpose(A) := diag(R) * transpose(A)
         *	     equed = 'C':  transpose(A) := transpose(A) * diag(C)
         *	     equed = 'B':  transpose(A) := diag(R) * transpose(A) * diag(C).
         *
         *         If options->RowPerm = LargeDiag, MC64 is used to scale and permute
         *            the matrix to an I-matrix, that is A is modified as follows:
         *            P*Dr*A*Dc has entries of modulus 1 on the diagonal and 
         *            off-diagonal entries of modulus at most 1. P is a permutation
         *            obtained from MC64.
         *            If MC64 fails, sgsequ() is used to equilibrate the system,
         *            and A is scaled as above, but no permutation is involved.
         *            On exit, A is restored to the orginal row numbering, so
         *            Dr*A*Dc is returned.
         *
         * perm_c  (input/output) int*
         *	   If A->Stype = SLU_NC, Column permutation vector of size A->ncol,
         *	   which defines the permutation matrix Pc; perm_c[i] = j means
         *	   column i of A is in position j in A*Pc.
         *	   On exit, perm_c may be overwritten by the product of the input
         *	   perm_c and a permutation that postorders the elimination tree
         *	   of Pc'*A'*A*Pc; perm_c is not changed if the elimination tree
         *	   is already in postorder.
         *
         *	   If A->Stype = SLU_NR, column permutation vector of size A->nrow,
         *	   which describes permutation of columns of transpose(A) 
         *	   (rows of A) as described above.
         *
         * perm_r  (input/output) int*
         *	   If A->Stype = SLU_NC, row permutation vector of size A->nrow, 
         *	   which defines the permutation matrix Pr, and is determined
         *	   by MC64 first then followed by partial pivoting.
         *         perm_r[i] = j means row i of A is in position j in Pr*A.
         *
         *	   If A->Stype = SLU_NR, permutation vector of size A->ncol, which
         *	   determines permutation of rows of transpose(A)
         *	   (columns of A) as described above.
         *
         *	   If options->Fact = SamePattern_SameRowPerm, the pivoting routine
         *	   will try to use the input perm_r, unless a certain threshold
         *	   criterion is violated. In that case, perm_r is overwritten by a
         *	   new permutation determined by partial pivoting or diagonal
         *	   threshold pivoting.
         *	   Otherwise, perm_r is output argument.
         *
         * etree   (input/output) int*,  dimension (A->ncol)
         *	   Elimination tree of Pc'*A'*A*Pc.
         *	   If options->Fact != FACTORED and options->Fact != DOFACT,
         *	   etree is an input argument, otherwise it is an output argument.
         *	   Note: etree is a vector of parent pointers for a forest whose
         *	   vertices are the integers 0 to A->ncol-1; etree[root]==A->ncol.
         *
         * equed   (input/output) char*
         *	   Specifies the form of equilibration that was done.
         *	   = 'N': No equilibration.
         *	   = 'R': Row equilibration, i.e., A was premultiplied by diag(R).
         *	   = 'C': Column equilibration, i.e., A was postmultiplied by diag(C).
         *	   = 'B': Both row and column equilibration, i.e., A was replaced 
         *		  by diag(R)*A*diag(C).
         *	   If options->Fact = FACTORED, equed is an input argument,
         *	   otherwise it is an output argument.
         *
         * R	   (input/output) float*, dimension (A->nrow)
         *	   The row scale factors for A or transpose(A).
         *	   If equed = 'R' or 'B', A (if A->Stype = SLU_NC) or transpose(A)
         *	       (if A->Stype = SLU_NR) is multiplied on the left by diag(R).
         *	   If equed = 'N' or 'C', R is not accessed.
         *	   If options->Fact = FACTORED, R is an input argument,
         *	       otherwise, R is output.
         *	   If options->Fact = FACTORED and equed = 'R' or 'B', each element
         *	       of R must be positive.
         *
         * C	   (input/output) float*, dimension (A->ncol)
         *	   The column scale factors for A or transpose(A).
         *	   If equed = 'C' or 'B', A (if A->Stype = SLU_NC) or transpose(A)
         *	       (if A->Stype = SLU_NR) is multiplied on the right by diag(C).
         *	   If equed = 'N' or 'R', C is not accessed.
         *	   If options->Fact = FACTORED, C is an input argument,
         *	       otherwise, C is output.
         *	   If options->Fact = FACTORED and equed = 'C' or 'B', each element
         *	       of C must be positive.
         *
         * L	   (output) SuperMatrix*
         *	   The factor L from the factorization
         *	       Pr*A*Pc=L*U		(if A->Stype SLU_= NC) or
         *	       Pr*transpose(A)*Pc=L*U	(if A->Stype = SLU_NR).
         *	   Uses compressed row subscripts storage for supernodes, i.e.,
         *	   L has types: Stype = SLU_SC, Dtype = SLU_S, Mtype = SLU_TRLU.
         *
         * U	   (output) SuperMatrix*
         *	   The factor U from the factorization
         *	       Pr*A*Pc=L*U		(if A->Stype = SLU_NC) or
         *	       Pr*transpose(A)*Pc=L*U	(if A->Stype = SLU_NR).
         *	   Uses column-wise storage scheme, i.e., U has types:
         *	   Stype = SLU_NC, Dtype = SLU_S, Mtype = SLU_TRU.
         *
         * work    (workspace/output) void*, size (lwork) (in bytes)
         *	   User supplied workspace, should be large enough
         *	   to hold data structures for factors L and U.
         *	   On exit, if fact is not 'F', L and U point to this array.
         *
         * lwork   (input) int
         *	   Specifies the size of work array in bytes.
         *	   = 0:  allocate space internally by system malloc;
         *	   > 0:  use user-supplied work array of length lwork in bytes,
         *		 returns error if space runs out.
         *	   = -1: the routine guesses the amount of space needed without
         *		 performing the factorization, and returns it in
         *		 mem_usage->total_needed; no other side effects.
         *
         *	   See argument 'mem_usage' for memory usage statistics.
         *
         * B	   (input/output) SuperMatrix*
         *	   B has types: Stype = SLU_DN, Dtype = SLU_S, Mtype = SLU_GE.
         *	   On entry, the right hand side matrix.
         *	   If B->ncol = 0, only LU decomposition is performed, the triangular
         *			   solve is skipped.
         *	   On exit,
         *	      if equed = 'N', B is not modified; otherwise
         *	      if A->Stype = SLU_NC:
         *		 if options->Trans = NOTRANS and equed = 'R' or 'B',
         *		    B is overwritten by diag(R)*B;
         *		 if options->Trans = TRANS or CONJ and equed = 'C' of 'B',
         *		    B is overwritten by diag(C)*B;
         *	      if A->Stype = SLU_NR:
         *		 if options->Trans = NOTRANS and equed = 'C' or 'B',
         *		    B is overwritten by diag(C)*B;
         *		 if options->Trans = TRANS or CONJ and equed = 'R' of 'B',
         *		    B is overwritten by diag(R)*B.
         *
         * X	   (output) SuperMatrix*
         *	   X has types: Stype = SLU_DN, Dtype = SLU_S, Mtype = SLU_GE.
         *	   If info = 0 or info = A->ncol+1, X contains the solution matrix
         *	   to the original system of equations. Note that A and B are modified
         *	   on exit if equed is not 'N', and the solution to the equilibrated
         *	   system is inv(diag(C))*X if options->Trans = NOTRANS and
         *	   equed = 'C' or 'B', or inv(diag(R))*X if options->Trans = 'T' or 'C'
         *	   and equed = 'R' or 'B'.
         *
         * recip_pivot_growth (output) float*
         *	   The reciprocal pivot growth factor max_j( norm(A_j)/norm(U_j) ).
         *	   The infinity norm is used. If recip_pivot_growth is much less
         *	   than 1, the stability of the LU factorization could be poor.
         *
         * rcond   (output) float*
         *	   The estimate of the reciprocal condition number of the matrix A
         *	   after equilibration (if done). If rcond is less than the machine
         *	   precision (in particular, if rcond = 0), the matrix is singular
         *	   to working precision. This condition is indicated by a return
         *	   code of info > 0.
         *
         * mem_usage (output) mem_usage_t*
         *	   Record the memory usage statistics, consisting of following fields:
         *	   - for_lu (float)
         *	     The amount of space used in bytes for L\U data structures.
         *	   - total_needed (float)
         *	     The amount of space needed in bytes to perform factorization.
         *	   - expansions (int)
         *	     The number of memory expansions during the LU factorization.
         *
         * stat   (output) SuperLUStat_t*
         *	  Record the statistics on runtime and floating-point operation count.
         *	  See slu_util.h for the definition of 'SuperLUStat_t'.
         *
         * info    (output) int*
         *	   = 0: successful exit
         *	   < 0: if info = -i, the i-th argument had an illegal value
         *	   > 0: if info = i, and i is
         *		<= A->ncol: number of zero pivots. They are replaced by small
         *		      entries due to options->ILU_FillTol.
         *		= A->ncol+1: U is nonsingular, but RCOND is less than machine
         *		      precision, meaning that the matrix is singular to
         *		      working precision. Nevertheless, the solution and
         *		      error bounds are computed because there are a number
         *		      of situations where the computed solution can be more
         *		      accurate than the value of RCOND would suggest.
         *		> A->ncol+1: number of bytes allocated when memory allocation
         *		      failure occurred, plus A->ncol.
         * </pre>
         */
        [DllImport(SuperLU, EntryPoint = "dgsisx", CallingConvention = CallingConvention.Cdecl)]
        public static extern void dgsisx(ref superlu_options options, ref SuperMatrix A, int[] perm_c, int[] perm_r, int[] etree,
               byte[] equed, double[] R, double[] C, ref SuperMatrix L, ref SuperMatrix U,
               IntPtr work, int lwork, ref SuperMatrix B, ref SuperMatrix X, out double recip_pivot_growth, out double rcond,
               ref GlobalLU global, ref mem_usage mem, ref SuperLUStat stat, out int info);

        /* Util */

        [DllImport(SuperLU, EntryPoint = "dCreate_CompCol_Matrix", CallingConvention = CallingConvention.Cdecl)]
        public static extern void dCreate_CompCol_Matrix(ref SuperMatrix A, int m, int n, int nnz, double[] nzval,
                       int[] rowind, int[] colptr, Stype stype, Dtype dtype, Mtype mtype);

        [DllImport(SuperLU, EntryPoint = "dCreate_CompRow_Matrix", CallingConvention = CallingConvention.Cdecl)]
        public static extern void dCreate_CompRow_Matrix(ref SuperMatrix A, int m, int n, int nnz, double[] nzval,
                       int[] colind, int[] rowptr, Stype stype, Dtype dtype, Mtype mtype);

        [DllImport(SuperLU, EntryPoint = "dCreate_Dense_Matrix", CallingConvention = CallingConvention.Cdecl)]
        public static extern void dCreate_Dense_Matrix(ref SuperMatrix X, int m, int n, double[] a, int ldx,
                       Stype stype, Dtype dtype, Mtype mtype);

        [DllImport(SuperLU, EntryPoint = "dCreate_SuperNode_Matrix", CallingConvention = CallingConvention.Cdecl)]
        public static extern void dCreate_SuperNode_Matrix(ref SuperMatrix L, int m, int n, int nnz, double[] nzval,
                       int[] nzval_colptr, int[] rowind, int[] rowind_colptr, int[] col_to_sup, int[] sup_to_col,
                       Stype stype, Dtype dtype, Mtype mtype);

        #endregion

        #region Double precision complex

        [DllImport(SuperLU, EntryPoint = "zgssv", CallingConvention = CallingConvention.Cdecl)]
        public static extern void zgssv(ref superlu_options options, ref SuperMatrix A, int[] perm_c, int[] perm_r, ref SuperMatrix L,
              ref SuperMatrix U, ref SuperMatrix B, ref SuperLUStat stat, out int info);

        [DllImport(SuperLU, EntryPoint = "zgssvx", CallingConvention = CallingConvention.Cdecl)]
        public static extern void zgssvx(ref superlu_options options, ref SuperMatrix A, int[] perm_c, int[] perm_r, int[] etree,
               byte[] equed, double[] R, double[] C, ref SuperMatrix L, ref SuperMatrix U,
               IntPtr work, int lwork, ref SuperMatrix _B, ref SuperMatrix X,
               out double recip_pivot_growth, out double rcond, double[] ferr, double[] berr,
               ref GlobalLU global, ref mem_usage mem, ref SuperLUStat stat, out int info);

        /* ILU */

        [DllImport(SuperLU, EntryPoint = "zgsisx", CallingConvention = CallingConvention.Cdecl)]
        public static extern void zgsisx(ref superlu_options options, ref SuperMatrix A, int[] perm_c, int[] perm_r, int[] etree,
               byte[] equed, double[] R, double[] C, ref SuperMatrix L, ref SuperMatrix U,
               IntPtr work, int lwork, ref SuperMatrix B, ref SuperMatrix X, out double recip_pivot_growth, out double rcond,
               ref GlobalLU global, ref mem_usage mem, ref SuperLUStat stat, out int info);

        /* Util */

        [DllImport(SuperLU, EntryPoint = "zCreate_CompCol_Matrix", CallingConvention = CallingConvention.Cdecl)]
        public static extern void zCreate_CompCol_Matrix(ref SuperMatrix A, int m, int n, int nnz, Complex[] nzval,
                       int[] rowind, int[] colptr, Stype stype, Dtype dtype, Mtype mtype);

        [DllImport(SuperLU, EntryPoint = "zCreate_CompRow_Matrix", CallingConvention = CallingConvention.Cdecl)]
        public static extern void zCreate_CompRow_Matrix(ref SuperMatrix A, int m, int n, int nnz, Complex[] nzval,
                       int[] colind, int[] rowptr, Stype stype, Dtype dtype, Mtype mtype);

        [DllImport(SuperLU, EntryPoint = "zCreate_Dense_Matrix", CallingConvention = CallingConvention.Cdecl)]
        public static extern void zCreate_Dense_Matrix(ref SuperMatrix X, int m, int n, Complex[] a, int ldx,
                       Stype stype, Dtype dtype, Mtype mtype);

        [DllImport(SuperLU, EntryPoint = "zCreate_SuperNode_Matrix", CallingConvention = CallingConvention.Cdecl)]
        public static extern void zCreate_SuperNode_Matrix(ref SuperMatrix L, int m, int n, int nnz, Complex[] nzval,
                       int[] nzval_colptr, int[] rowind, int[] rowind_colptr, int[] col_to_sup, int[] sup_to_col,
                       Stype stype, Dtype dtype, Mtype mtype);

        #endregion

        #endregion

        #region Parallel (SuperLU MT)

        const string ParSuperLU = "libsuperlu_mt";

        #region Double precision

        #endregion

        #region Double precision complex

        #endregion

        #endregion
    }
}
