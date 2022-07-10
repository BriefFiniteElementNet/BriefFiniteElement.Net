
namespace CSparse.Interop.SuperLU
{
    public enum UpdateLevel
    {
        SamePattern = 2,
        SamePatternSameRowPerm = 3
    }

    internal class Constants
    {
        internal const int DOFACT = 0;
        internal const int EQUILIBRATE = 1;
        internal const int FACTORED_MT = 2; 
        internal const int FACTORED = 3;

        // Dropping rules
        internal const int NODROP = (0x0000);
        internal const int DROP_BASIC = (0x0001); // ILU(tau)
        internal const int DROP_PROWS = (0x0002); // ILUTP: keep p maximum rows
        internal const int DROP_COLUMN = (0x0004); // ILUTP: for j-th column, p = gamma * nnz(A(:,j))
        internal const int DROP_AREA = (0x0008); // ILUTP: for j-th column, use nnz(F(:,1:j)) / nnz(A(:,1:j)) to limit memory growth 
        internal const int DROP_SECONDARY = (0x000E); // PROWS | COLUMN | AREA
        internal const int DROP_DYNAMIC = (0x0010); // adaptive tau
        internal const int DROP_INTERP = (0x0100); // use interpolation

        /*
        enum fact_mt_t { DOFACT, EQUILIBRATE, FACTORED };

        internal enum fact_t { DOFACT, SamePattern, SamePattern_SameRowPerm, FACTORED };
        internal enum rowperm_t { NOROWPERM, LargeDiag, MY_PERMR };
        internal enum colperm_t
        {
            NATURAL, MMD_ATA, MMD_AT_PLUS_A, COLAMD,
            METIS_AT_PLUS_A, PARMETIS, ZOLTAN, MY_PERMC
        };
        internal enum trans_t { NOTRANS, TRANS, CONJ };
        internal enum DiagScale_t { NOEQUIL, ROW, COL, BOTH };
        internal enum IterRefine_t { NOREFINE, SLU_SINGLE = 1, SLU_DOUBLE, SLU_EXTRA };
        internal enum norm_t { ONE_NORM, TWO_NORM, INF_NORM };
        internal enum milu_t { SILU, SMILU_1, SMILU_2, SMILU_3 };
        //*/
    }
}
