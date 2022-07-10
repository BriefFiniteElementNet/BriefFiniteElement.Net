
namespace CSparse.Interop.MKL.Pardiso
{
    /// <summary>
    /// PARDISO options.
    /// </summary>
    public class PardisoOptions
    {
        private const int ARRAY_SIZE = 64;

        #region Public properties

        /// <summary>
        /// The method used to permute the columns of the input matrix. 
        /// </summary>
        public PardisoOrdering ColumnOrderingMethod
        {
            get
            {
                return (PardisoOrdering)iparm[1];
            }
            set
            {
                iparm[1] = (int)value;
            }
        }

        /// <summary>
        /// The iterative refinement option.
        /// </summary>
        public int IterativeRefinement
        {
            get
            {
                return iparm[7];
            }
            set
            {
                iparm[7] = value;
            }
        }

        /// <summary>
        /// The threshold used for a diagonal entry to be an acceptable pivot. 
        /// </summary>
        public int PivotingPerturbation
        {
            get
            {
                return iparm[9];
            }
            set
            {
                iparm[9] = value;
            }
        }

        /// <summary>
        /// Specifies whether input matrix A should be scaled. 
        /// </summary>
        public bool Scaling
        {
            get
            {
                return iparm[10] == 1;
            }
            set
            {
                iparm[10] = value ? 1 : 0;
            }
        }

        /// <summary>
        /// The transpose mode option. 
        /// </summary>
        public bool Transpose
        {
            get
            {
                return iparm[11] > 0;
            }
            set
            {
                iparm[11] = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Permute large elements close the diagonal. 
        /// </summary>
        public bool WeightedMatching
        {
            get
            {
                return iparm[12] > 0;
            }
            set
            {
                iparm[12] = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Check the sparse matrix representation for errors.
        /// </summary>
        public bool CheckMatrix
        {
            get
            {
                return iparm[26] > 0;
            }
            set
            {
                iparm[26] = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Input arrays must be presented in single precision.
        /// </summary>
        public bool SinglePrecision
        {
            get
            {
                return iparm[27] > 0;
            }
            set
            {
                iparm[27] = value ? 1 : 0;
            }
        }

        /// <summary>
        /// One- or zero-based indexing of columns and rows.
        /// </summary>
        public bool ZeroBasedIndexing
        {
            get
            {
                return iparm[34] > 0;
            }
            set
            {
                iparm[34] = value ? 1 : 0;
            }
        }

        #endregion

        /// <summary>
        /// The underlying Pardiso options array.
        /// </summary>
        /// <remarks>
        /// Having public access to this object is essential for a finer control
        /// over the factorization process. Make sure you understand all the options
        /// you are setting.
        /// 
        /// See https://software.intel.com/en-us/mkl-developer-reference-c-pardiso-iparm-parameter and
        ///     https://software.intel.com/en-us/articles/pardiso-parameter-table
        /// </remarks>
        internal int[] iparm;

        //  [0] input  Use default values.
        //  [1] input  Fill-in reducing ordering for the input matrix.
        //      0 The minimum degree algorithm.
        //      2 The nested dissection algorithm from the METIS package.
        //      3 The parallel (OpenMP) version of the nested dissection algorithm.
        //  [2] (reserved)
        //  [3] input  Preconditioned CGS/CG.
        //  [4] input  User permutation.
        //      0 User permutation in the perm array is ignored.
        //      1 Use the user supplied fill-in reducing permutation from the perm array.
        //      2 Returns the permutation vector computed at phase 1 in the perm array.
        //  [5] input  Write solution on x (1 = overwrite b).
        //  [6] output Number of iterative refinement steps performed.
        //  [7] input  Iterative refinement step.
        //  [8] (reserved)  
        //  [9] input  Pivoting perturbation.
        // [10] input  Scaling vectors.
        // [11] input  Solve with transposed (2) or conjugate transposed (1) matrix A.
        // [12] input  Improved accuracy using (non-) symmetric weighted matching.
        // [13] output Number of perturbed pivots.
        // [14] output Peak memory on symbolic factorization.
        // [15] output Permanent memory on symbolic factorization.
        // [16] output Peak memory on numerical factorization and solution.
        // [17] in/out Report the number of non-zero elements in the factors.
        // [18] in/out Report number of floating point operations.
        // [19] output Report CG/CGS diagnostics.
        // [20] input  Pivoting for symmetric indefinite matrices.
        // [21] output Inertia: number of positive eigenvalues.
        // [22] output Inertia: number of negative eigenvalues.
        // [23] input  Parallel factorization control.
        // [24] input  Parallel forward/backward solve control.
        // [25] (reserved)
        // [26] input  Matrix checker.
        // [27] input  Single or double precision (1 = single).
        // [28] (reserved) 
        // [29] output Number of zero or negative pivots.
        // [30] input  Partial solve and computing selected components of the solution vectors.
        // [31] - [32] (reserved)
        // [33] input  Optimal number of OpenMP threads for CNR mode.
        // [34] input  One- or zero-based indexing of columns and rows (1 = zero-based).
        // [35] input  Schur complement matrix computation control.
        // [36] input  Format for matrix storage.
        // [37] - [54] (reserved)
        // [55] input  Diagonal and pivoting control.
        // [56] - [58] (reserved)
        // [59] input  Switches between in-core (IC) and out-of-core (OOC).
        // [60] - [61] (reserved)
        // [62] output Size of the minimum OOC memory for numerical factorization and solution.
        // [63] (reserved)

        /// <summary>
        /// Initializes a new instance of the <see cref="PardisoOptions"/> class.
        /// </summary>
        public PardisoOptions()
        {
            iparm = new int[ARRAY_SIZE];
        }

        /// <summary>
        /// Set default options (zero-based indexing, re-ordering using nested dissection, 2 steps of iterative refinement).
        /// </summary>
        public void SetDefault()
        {
            iparm[0] = 1; // No solver default.
            iparm[1] = 0; // Fill-in reordering from METIS.
            iparm[3] = 0; // No iterative-direct algorithm.
            iparm[4] = 0; // No user fill-in reducing permutation.
            iparm[5] = 0; // Write solution into x.
            iparm[6] = 0; // Not in use.
            iparm[7] = 2; // Max numbers of iterative refinement steps.
            iparm[8] = 0; // Not in use.
            iparm[9] = 13; // Perturb the pivot elements with 1E-13.
            iparm[10] = 1; // Use nonsymmetric permutation and scaling MPS.
            iparm[11] = 0; // Conjugate transposed/transpose solve.
            iparm[12] = 1; // Maximum weighted matching algorithm is switched-on (default for non-symmetric).
            iparm[13] = 0; // Output: Number of perturbed pivots.
            iparm[14] = 0; // Not in use.
            iparm[15] = 0; // Not in use.
            iparm[16] = 0; // Not in use.
            iparm[17] = -1; // Output: Number of nonzeros in the factor LU.
            iparm[18] = -1; // Output: Mflops for LU factorization.
            iparm[19] = 0; // Output: Numbers of CG Iterations.
            iparm[34] = 1; // Zero-based indexing.
        }
    }
}
