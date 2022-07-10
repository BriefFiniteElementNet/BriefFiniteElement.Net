namespace CSparse.Interop.SuiteSparse.Cholmod
{
    using System;

#if X64
    using size_t = System.UInt64;
#else
    using size_t = System.UInt32;
#endif

    /// <summary>
    /// A symbolic and numeric factorization, either simplicial or supernodal.
    /// </summary>
    /// <remarks>
    /// There are 8 types of factor objects that cholmod_factor can represent
    /// (only 6 are used):
    ///
    /// Numeric types (xtype is not CHOLMOD_PATTERN)
    /// --------------------------------------------
    ///
    /// simplicial LDL':  (is_ll FALSE, is_super FALSE).  Stored in compressed
    ///	    column form, using the simplicial components above (nzmax, p, i,
    ///	    x, z, nz, next, and prev).  The unit diagonal of L is not stored,
    ///	    and D is stored in its place.  There are no supernodes.
    ///
    /// simplicial LL': (is_ll TRUE, is_super FALSE).  Uses the same storage
    ///	    scheme as the simplicial LDL', except that D does not appear.
    ///	    The first entry of each column of L is the diagonal entry of
    ///	    that column of L.
    ///
    /// supernodal LDL': (is_ll FALSE, is_super TRUE).  Not used.
    ///	    FUTURE WORK:  add support for supernodal LDL'
    ///
    /// supernodal LL': (is_ll TRUE, is_super TRUE).  A supernodal factor,
    ///	    using the supernodal components described above (nsuper, ssize,
    ///	    xsize, maxcsize, maxesize, super, pi, px, s, x, and z).
    ///
    ///
    /// Symbolic types (xtype is CHOLMOD_PATTERN)
    /// -----------------------------------------
    ///
    /// simplicial LDL': (is_ll FALSE, is_super FALSE).  Nothing is present
    ///	    except Perm and ColCount.
    ///
    /// simplicial LL': (is_ll TRUE, is_super FALSE).  Identical to the
    ///	    simplicial LDL', except for the is_ll flag.
    ///
    /// supernodal LDL': (is_ll FALSE, is_super TRUE).  Not used.
    ///	    FUTURE WORK:  add support for supernodal LDL'
    ///
    /// supernodal LL': (is_ll TRUE, is_super TRUE).  A supernodal symbolic
    ///	    factorization.  The simplicial symbolic information is present
    ///	    (Perm and ColCount), as is all of the supernodal factorization
    ///	    except for the numerical values (x and z).
    /// </remarks>
    public struct CholmodFactor
    {
        //
        // for both simplicial and supernodal factorizations
        //

        /// <summary>
        /// L is n-by-n
        /// </summary>
        public size_t n;

        /// <summary>
        /// If the factorization failed, L->minor is the column at which it failed.
        /// </summary>
        public size_t minor;

        //
        // symbolic ordering and analysis
        //

        /// <summary>
        /// size n, permutation used
        /// </summary>
        public IntPtr Perm;
        /// <summary>
        /// size n, column counts for simplicial L
        /// </summary>
        public IntPtr ColCount;

        /// <summary>
        /// size n, inverse permutation.
        /// </summary>
        public IntPtr IPerm;

        //
        // simplicial factorization
        //

        /// <summary>
        /// size of i and x
        /// </summary>
        public size_t nzmax;

        /// <summary>
        /// p [0..ncol], the column pointers
        /// </summary>
        public IntPtr p;
        /// <summary>
        /// i [0..nzmax-1], the row indices
        /// </summary>
        public IntPtr i;
        /// <summary>
        /// x [0..nzmax-1], the numerical values
        /// </summary>
        public IntPtr x;
        public IntPtr z;
        /// <summary>
        /// nz [0..ncol-1], the # of nonzeros in each column.
        /// </summary>
        public IntPtr nz;

        /// <summary>
        /// size ncol+2. next [j] is the next column in i/x
        /// </summary>
        public IntPtr next;
        /// <summary>
        /// size ncol+2. prev [j] is the prior column in i/x.
        /// </summary>
        public IntPtr prev;

        //
        // supernodal factorization
        //

        /// <summary>
        /// number of supernodes
        /// </summary>
        public size_t nsuper;
        /// <summary>
        /// size of s, integer part of supernodes
        /// </summary>
        public size_t ssize;
        /// <summary>
        /// size of x, real part of supernodes
        /// </summary>
        public size_t xsize;
        /// <summary>
        /// size of largest update matrix
        /// </summary>
        public size_t maxcsize;
        /// <summary>
        /// max # of rows in supernodes, excl. triangular part
        /// </summary>
        public size_t maxesize;

        /// <summary>
        /// size nsuper+1, first col in each supernode
        /// </summary>
        public IntPtr super;
        /// <summary>
        /// size nsuper+1, pointers to integer patterns
        /// </summary>
        public IntPtr pi;
        /// <summary>
        /// size nsuper+1, pointers to real parts
        /// </summary>
        public IntPtr px;
        /// <summary>
        /// size ssize, integer part of supernodes
        /// </summary>
        public IntPtr s;

        //
        // Factorization type
        //

        /// <summary>
        /// ordering method used
        /// </summary>
        public int ordering;

        /// <summary>
        /// TRUE if LL', FALSE if LDL'
        /// </summary>
        public int is_ll;
        /// <summary>
        /// TRUE if supernodal, FALSE if simplicial
        /// </summary>
        public int is_super;
        /// <summary>
        /// TRUE if columns of L appear in order 0..n-1.
        /// </summary>
        public int is_monotonic;

        /// <summary>
        /// integer type
        /// </summary>
        public int itype;
        /// <summary>
        /// pattern, real, complex, or zomplex
        /// </summary>
        public Xtype xtype;
        /// <summary>
        /// x and z double or float
        /// </summary>
        public Dtype dtype;

        /// <summary>
        /// Indicates the symbolic factorization supports GPU acceleration
        /// </summary>
        public int useGPU;
    }
}
