using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents the end connection type of bar element to either of its end nodes
    /// </summary>
    public struct BarElementEndConnection
    {
        /// <summary>
        /// The dx connection constrain
        /// </summary>
        public DofConstraint Dx;
        /// <summary>
        /// The dy connection constrain
        /// </summary>
        public DofConstraint Dy;
        /// <summary>
        /// The dz connection constrain
        /// </summary>
        public DofConstraint Dz;

        /// <summary>
        /// The rx connection constrain
        /// </summary>
        public DofConstraint Rx;
        /// <summary>
        /// The ry connection constrain
        /// </summary>
        public DofConstraint Ry;
        /// <summary>
        /// The rz connection constrain
        /// </summary>
        public DofConstraint Rz;
    }
}
