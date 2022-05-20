using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSparse.Double;
using CSparse.Storage;
using CCS = CSparse.Double.SparseMatrix;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represent a zone divided matrix, for storing Stiffness, Mass or Damp matrix fixed and released parts.
    /// _        _
    /// | rr , rf|
    /// | fr , ff|
    /// -        -
    /// Where
    ///     r: released
    ///     f: fixed
    /// </summary>
    [Obsolete("not used with Solve_MPC() approach ")]
    public class ZoneDevidedMatrix
    {
        /// <summary>
        /// The released released part
        /// </summary>
        private CCS releasedReleasedPart;

        /// <summary>
        /// The released fixed part
        /// </summary>
        private CCS releasedFixedPart;

        /// <summary>
        /// The fixed released part
        /// </summary>
        private CCS fixedReleasedPart;

        /// <summary>
        /// The fixed fixed part
        /// </summary>
        private CCS fixedFixedPart;

        public CCS ReleasedReleasedPart
        {
            get { return releasedReleasedPart; }
            set
            {
                releasedReleasedPart = value;
            }
        }
        public CCS ReleasedFixedPart
        {
            get { return releasedFixedPart; }
            set
            {
                releasedFixedPart = value;
            }
        }
        public CCS FixedReleasedPart
        {
            get { return fixedReleasedPart; }
            set
            {
                fixedReleasedPart = value;
            }
        }
        public CCS FixedFixedPart
        {
            get { return fixedFixedPart; }
            set
            {
                fixedFixedPart = value;
            }
        }

        /*
        /// <summary>
        /// The free map
        /// </summary>
        /// <remarks>
        /// FreeMap.Length = count of free DoFs
        /// FreeMap[i] = original DOF number of i'th DoF in frees (in ReleasedReleasedPart and ReleasedFixedPart )
        /// </remarks>
        public int[] FreeMap;
        
        /// <summary>
        /// The fixed map
        /// </summary>
        /// <remarks>
        /// FixedMap.Length = count of fixed DoFs
        /// FixedMap[i] = original DOF number of i'th DoF in fixes (in FixedFixedPart and ReleasedFixedPart )
        /// </remarks>
        public int[] FixedMap;
        */

    }
}
