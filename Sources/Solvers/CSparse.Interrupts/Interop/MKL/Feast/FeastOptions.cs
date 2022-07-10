
namespace CSparse.Interop.MKL.Feast
{
    using CSparse.Interop.MKL.Pardiso;
    using System;

    public enum FeastMode
    {
        /// <summary>
        /// Normal execution.
        /// </summary>
        Standard = 0,
        /// <summary>
        /// Return the computed eigenvectors subspace after one single contour integration.
        /// </summary>
        SingleIntegration = 1,
        /*
        /// <summary>
        /// Estimate the number of eigenvalues inside search interval.
        /// </summary>
        Estimate = 2 // Requires FEAST version 3.0
        //*/
    }

    public class FeastOptions
    {
        private const int ARRAY_SIZE = 128;

        #region Public properties

        /// <summary>
        /// Specifies whether Extended Eigensolver routines print runtime status. 
        /// </summary>
        public bool PrintStatus
        {
            get
            {
                return fpm[0] == 1;
            }
            set
            {
                fpm[0] = value ? 1 : 0;
            }
        }

        /// <summary>
        /// The number of contour points (default = 8).
        /// </summary>
        /// <remarks>
        /// Must be one of {3,4,5,6,8,10,12,16,20,24,32,40,48}.
        /// </remarks>
        public int ContourPoints
        {
            get
            {
                return fpm[1];
            }
            set
            {
                fpm[1] = value;
            }
        }

        /// <summary>
        /// Error trace double precision stopping criteria (10^-k, default = 12). 
        /// </summary>
        public int TraceThreshold
        {
            get
            {
                return fpm[2];
            }
            set
            {
                fpm[2] = value;
            }
        }

        /// <summary>
        /// Maximum number of Extended Eigensolver refinement loops allowed. 
        /// </summary>
        /// <remarks>
        /// If no convergence is reached within specified refinement loops,
        /// Extended Eigensolver routines return info=2.
        /// </remarks>
        public int Refinement
        {
            get
            {
                return fpm[3];
            }
            set
            {
                fpm[3] = value;
            }
        }

        /// <summary>
        /// Use custom initial subspace (default = false). 
        /// </summary>
        /// <remarks>
        /// If false, then Extended Eigensolver routines generate initial subspace,
        /// if true the user supplied initial subspace is used.
        /// </remarks>
        public bool CustomInitialSubspace
        {
            get
            {
                return fpm[4] == 1;
            }
            set
            {
                fpm[4] = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Use residual or trace stopping test.
        /// </summary>
        /// <remarks>
        /// If true, Extended Eigensolvers are stopped if residual stopping
        /// test is satisfied, otherwise trace stopping test is applied.
        /// </remarks>
        public bool UseTraceStoppingTest
        {
            get
            {
                return fpm[5] == 1;
            }
            set
            {
                fpm[5] = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Error trace single precision stopping criteria (10^-k, default = 5). 
        /// </summary>
        public int SingleTraceThreshold
        {
            get
            {
                return fpm[6];
            }
            set
            {
                fpm[6] = value;
            }
        }

        /// <summary>
        /// Standard use for Extended Eigensolver routines.
        /// </summary>
        public FeastMode Usage
        {
            get
            {
                return (FeastMode)fpm[13];
            }
            set
            {
                fpm[13] = (int)value;
            }
        }



        /// <summary>
        /// Specifies whether Extended Eigensolver routines check input
        /// matrices (applies to CSR format only).
        /// </summary>
        public bool CheckInput
        {
            get
            {
                return fpm[26] == 1;
            }
            set
            {
                fpm[26] = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Check if matrix B is positive definite.
        /// </summary>
        public bool CheckPositiveDefinite
        {
            get
            {
                return fpm[27] == 1;
            }
            set
            {
                fpm[27] = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Use the PARDISO solver with the user-defined options.
        /// </summary>
        public bool CustomParadiso
        {
            get
            {
                return fpm[63] == 1;
            }
            set
            {
                fpm[63] = value ? 1 : 0;
            }
        }

        #endregion

        /// <summary>
        /// The underlying FEAST options array.
        /// </summary>
        /// <remarks>
        /// See https://software.intel.com/en-us/mkl-developer-reference-c-extended-eigensolver-input-parameters.
        /// </remarks>
        internal int[] fpm;

        public FeastOptions()
        {
            fpm = new int[ARRAY_SIZE];

            SetDefaultOptions();
        }

        public void SetPardisoOptions(PardisoOptions paradiso)
        {
            CustomParadiso = true;

            Array.Copy(paradiso.iparm, 0, fpm, 64, 64);
        }

        private void SetDefaultOptions()
        {
            fpm[0] = 0; // print runtime status.
            fpm[1] = 8; // number of contour points.
            fpm[2] = 12; // double precision stopping criteria.
            fpm[3] = 0; // refinement loops.
            fpm[4] = 0; // custom initial subspace.
            fpm[5] = 0; // stopping test.
            fpm[6] = 5; // single precision stopping criteria.
            fpm[13] = 1; // standard use.
            fpm[26] = 0; // check input.
            fpm[27] = 0; // check positive definite.
            fpm[63] = 0; // use custom paradiso options.
        }
    }
}
