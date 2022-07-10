namespace CSparse.Interop.SuiteSparse.SPQR
{
    internal static class Constants
    {
        // From SuiteSparseQR_definitions.h
        
        /* ordering options */
        public const int SPQR_ORDERING_FIXED = 0;
        public const int SPQR_ORDERING_NATURAL = 1;
        public const int SPQR_ORDERING_COLAMD = 2;
        public const int SPQR_ORDERING_GIVEN = 3;       /* only used for C/C++ interface */
        public const int SPQR_ORDERING_CHOLMOD = 4;     /* */
        public const int SPQR_ORDERING_AMD = 5;         /* AMD(A'*A) */
        public const int SPQR_ORDERING_METIS = 6;       /* metis(A'*A) */
        public const int SPQR_ORDERING_DEFAULT = 7;     /* SuiteSparseQR default ordering */
        public const int SPQR_ORDERING_BEST = 8;        /* try COLAMD, AMD, and METIS; pick best */
        public const int SPQR_ORDERING_BESTAMD = 9;     /* try COLAMD and AMD; pick best */

        /* Let [m n] = size of the matrix after pruning singletons.  The default
         * ordering strategy is to use COLAMD if m <= 2*n.  Otherwise, AMD(A'A) is
         * tried.  If there is a high fill-in with AMD then try METIS(A'A) and take
         * the best of AMD and METIS.  METIS is not tried if it isn't installed. */

        /* tol options */
        public const int SPQR_DEFAULT_TOL = -2;       /* if tol <= -2, the default tol is used */
        public const int SPQR_NO_TOL = -1;            /* if -2 < tol < 0, then no tol is used */

        /* for qmult, method can be 0,1,2,3: */
        public const int SPQR_QTX = 0;
        public const int SPQR_QX = 1;
        public const int SPQR_XQT = 2;
        public const int SPQR_XQ = 3;

        /* system can be 0,1,2,3:  Given Q*R=A*E from SuiteSparseQR_factorize: */
        public const int SPQR_RX_EQUALS_B = 0;       /* solve R*X=B      or X = R\B          */
        public const int SPQR_RETX_EQUALS_B = 1;       /* solve R*E'*X=B   or X = E*(R\B)      */
        public const int SPQR_RTX_EQUALS_B = 2;       /* solve R'*X=B     or X = R'\B         */
        public const int SPQR_RTX_EQUALS_ETB = 3;       /* solve R'*X=E'*B  or X = R'\(E'*B)    */
        
        public const int CHOLMOD_OK = 0;			/* success */
        public const int TRUE = 1;
    }
}
