
namespace CSparse.Interop.SuperLU
{
    using System;

    public class SuperLUOptions
    {
        #region Public properties

        /// <summary>
        /// The method used to permute the columns of the input matrix. 
        /// </summary>
        public OrderingMethod ColumnOrderingMethod
        {
            get
            {
                return (OrderingMethod)Raw.ColPerm;
            }
            set
            {
                Raw.ColPerm = (int)value;
            }
        }

        /// <summary>
        /// Specifies whether to compute an estimate of the reciprocal condition number of the matrix A. 
        /// </summary>
        public bool ConditionNumber
        {
            get
            {
                return Raw.ConditionNumber == 1;
            }
            set
            {
                Raw.ConditionNumber = value ? 1 : 0;
            }
        }

        /// <summary>
        /// The threshold used for a diagonal entry to be an acceptable pivot. 
        /// </summary>
        public double DiagonalPivotThreshold
        {
            get
            {
                return Raw.DiagPivotThresh;
            }
            set
            {
                Raw.DiagPivotThresh = value;
            }
        }

        /// <summary>
        /// Specifies if input matrix A is equilibrated before factorization. 
        /// </summary>
        public bool Equilibrate
        {
            get
            {
                return Raw.Equil == 1;
            }
            set
            {
                Raw.Equil = value ? 1 : 0;
            }
        }

        /// <summary>
        /// The iterative refinement option.
        /// </summary>
        public bool IterativeRefinement
        {
            get
            {
                return Raw.IterRefine > 0;
            }
            set
            {
                Raw.IterRefine = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Specifies whether to compute the reciprocal pivot growth factor. 
        /// </summary>
        public bool PivotGrowth
        {
            get
            {
                return Raw.PivotGrowth == 1;
            }
            set
            {
                Raw.PivotGrowth = value ? 1 : 0;
            }
        }

        /// <summary>
        /// The symmetric mode option.
        /// </summary>
        public bool SymmetricMode
        {
            get
            {
                return Raw.SymmetricMode == 1;
            }
            set
            {
                Raw.SymmetricMode = value ? 1 : 0;
            }
        }

        /// <summary>
        /// The transpose mode option. 
        /// </summary>
        public bool Transpose
        {
            get
            {
                return Raw.Trans > 0;
            }
            set
            {
                Raw.Trans = value ? 1 : 0;
            }
        }

        #endregion
        
        /// <summary>
        /// Returns the underlying SuperLU options structure. 
        /// </summary>
        internal superlu_options Raw;
        
        public SuperLUOptions()
            : this(false)
        {
        }

        public SuperLUOptions(bool incomplete)
        {
            SetDefaultOptions();

            if (incomplete)
            {
                // Further options for incomplete factorization.
                Raw.DiagPivotThresh = 0.1;
                Raw.RowPerm = 1; // rowperm_t.LargeDiag
                Raw.ILU_DropRule = Constants.DROP_BASIC | Constants.DROP_AREA;
                Raw.ILU_DropTol = 1e-4;
                Raw.ILU_FillFactor = 10.0;
                Raw.ILU_Norm = 2; // norm_t.INF_NORM
                Raw.ILU_MILU = 0; // milu_t.SILU
                Raw.ILU_MILU_Dim = 3.0; // -log(n)/log(h) is perfect
                Raw.ILU_FillTol = 1e-2;
            }
        }

        private void SetDefaultOptions()
        {
            Raw.Fact = Constants.DOFACT;
            Raw.Equil = 1;
            Raw.ColPerm = 3; // colperm_t.COLAMD
            Raw.Trans = 0; // trans_t.NOTRANS
            Raw.IterRefine = 0; // IterRefine_t.NOREFINE
            Raw.DiagPivotThresh = 1.0;
            Raw.SymmetricMode = 0;
            Raw.PivotGrowth = 0;
            Raw.ConditionNumber = 0;
            Raw.PrintStat = 0;
        }
    }

    #region superlu_options struct

    // Taken from slu_util.h file.

    /// <summary>
    /// Options used to control the solution process
    /// </summary>
    public struct superlu_options
    {
        /// <summary>
        /// Specifies whether or not the factored form of the matrix A is supplied on entry (fact_t)
        /// </summary>
        public int Fact;
        /// <summary>
        /// Specifies whether to equilibrate the system (yes_no_t)
        /// </summary>
        public int Equil;
        /// <summary>
        /// Specifies what type of column permutation to use to reduce fill (colperm_t)
        /// </summary>
        public int ColPerm;
        /// <summary>
        /// Specifies the form of the system of equations (trans_t)
        /// </summary>
        public int Trans;
        /// <summary>
        /// Specifies whether to perform iterative refinement (IterRefine_t)
        /// </summary>
        public int IterRefine;
        /// <summary>
        /// Specifies the threshold used for a diagonal entry to be an acceptable pivot.
        /// </summary>
        public double DiagPivotThresh;
        /// <summary>
        /// Specifies whether to use symmetric mode (yes_no_t)
        /// </summary>
        public int SymmetricMode;
        /// <summary>
        /// Specifies whether to compute the reciprocal pivot growth (yes_no_t)
        /// </summary>
        public int PivotGrowth;
        /// <summary>
        /// Specifies whether to compute the reciprocal condition number (yes_no_t)
        /// </summary>
        public int ConditionNumber;
        /// <summary>
        /// Specifies whether to permute rows of the original matrix (rowperm_t)
        /// </summary>
        public int RowPerm;
        /// <summary>
        /// Specifies the dropping rule
        /// </summary>
        public int ILU_DropRule;
        /// <summary>
        /// numerical threshold for dropping
        /// </summary>
        public double ILU_DropTol;
        /// <summary>
        /// gamma in the secondary dropping
        /// </summary>
        public double ILU_FillFactor;
        /// <summary>
        /// Specify which norm to use to measure the row size in a supernode (norm_t)
        /// </summary>
        public int ILU_Norm;
        /// <summary>
        /// numerical threshold for zero pivot perturbation
        /// </summary>
        public double ILU_FillTol;
        /// <summary>
        /// Specifies which version of MILU to use (milu_t)
        /// </summary>
        public int ILU_MILU;
        /// <summary>
        /// Dimension of PDE (if available)
        /// </summary>
        public double ILU_MILU_Dim;
        /// <summary>
        /// used in SuperLU_DIST(yes_no_t)
        /// </summary>
        public int ParSymbFact;
        /// <summary>
        /// Specifies whether to replace the tiny diagonals by sqrt(epsilon)*||A||
        /// during LU factorization (yes_no_t)
        /// </summary>
        public int ReplaceTinyPivot;
        /// <summary>
        /// Specifies whether the initialization has been performed to the triangular solve (yes_no_t)
        /// </summary>
        public int SolveInitialized;
        /// <summary>
        /// Specifies whether the initialization has been performed to the sparse
        /// matrix-vector multiplication routine needed in iterative refinement (yes_no_t)
        /// </summary>
        public int RefineInitialized;
        /// <summary>
        /// Specifies whether to print the solver's statistics (yes_no_t).
        /// </summary>
        public int PrintStat;
        /// <summary>
        /// used to store nnzs for now
        /// </summary>
        public int nnzL;
        /// <summary>
        /// used to store nnzs for now
        /// </summary>
        public int nnzU;
        /// <summary>
        /// num of levels in look-ahead
        /// </summary>
        public int num_lookaheads;
        /// <summary>
        /// use etree computed from the serial symbolic factorization (yes_no_t)
        /// </summary>
        public int lookahead_etree;
        /// <summary>
        /// symmetric factorization (yes_no_t)
        /// </summary>
        public int SymPattern;
    }

    #endregion
}
