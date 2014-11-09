using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.CSparse.Storage;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represent a zone divided matrix, for storing Stiffness, Mass or Damp matrix fixed and released parts.
    /// </summary>
    public class ZoneDevidedMatrix
    {
        /// <summary>
        /// The released released part
        /// </summary>
        public CompressedColumnStorage<double> ReleasedReleasedPart;

        /// <summary>
        /// The released fixed part
        /// </summary>
        public CompressedColumnStorage<double> ReleasedFixedPart;

        /// <summary>
        /// The fixed released part
        /// </summary>
        public CompressedColumnStorage<double> FixedReleasedPart;

        /// <summary>
        /// The fixed fixed part
        /// </summary>
        public CompressedColumnStorage<double> FixedFixedPart;
    }
}
