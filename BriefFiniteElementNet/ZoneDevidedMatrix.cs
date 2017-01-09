using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSparse.Storage;
using CCS = CSparse.Double.CompressedColumnStorage;

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
    public class ZoneDevidedMatrix
    {
        /// <summary>
        /// The released released part
        /// </summary>
        public CCS ReleasedReleasedPart;

        /// <summary>
        /// The released fixed part
        /// </summary>
        public CCS ReleasedFixedPart;

        /// <summary>
        /// The fixed released part
        /// </summary>
        public CCS FixedReleasedPart;

        /// <summary>
        /// The fixed fixed part
        /// </summary>
        public CCS FixedFixedPart;

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
