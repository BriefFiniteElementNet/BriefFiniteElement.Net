using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents the end connection type of bar element to either of its end nodes
    /// </summary>
    [Obsolete("use two node approach for partial connection")]
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

        /// <summary>
        /// Gets the fixed connection.
        /// </summary>
        /// <value>
        /// A totally fixed connection.
        /// </value>
        public static BarElementEndConnection Fixed
        {
            get
            {
                return new BarElementEndConnection()
                {
                    Dx = DofConstraint.Fixed,
                    Dy = DofConstraint.Fixed,
                    Dz = DofConstraint.Fixed,
                    Rx = DofConstraint.Fixed,
                    Ry = DofConstraint.Fixed,
                    Rz = DofConstraint.Fixed,
                };
            }
        }

        /// <summary>
        /// Gets the fixed connection.
        /// </summary>
        /// <value>
        /// A totally fixed connection.
        /// </value>
        public static BarElementEndConnection TotallyHinged
        {
            get
            {
                return new BarElementEndConnection()
                {
                    Dx = DofConstraint.Fixed,
                    Dy = DofConstraint.Fixed,
                    Dz = DofConstraint.Fixed,
                    Rx = DofConstraint.Released,
                    Ry = DofConstraint.Released,
                    Rz = DofConstraint.Released,
                };
            }
        }


    }
}
