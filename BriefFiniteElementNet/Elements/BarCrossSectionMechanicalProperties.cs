using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents a base class that represents mechanical of a specific location of cross section.
    /// </summary>
    public class BarCrossSectionMechanicalProperties
    {
        private double e;
        private double g;

        /// <summary>
        /// Gets or sets the Elastic modulus in [Pas] dimension.
        /// </summary>
        /// <value>
        /// The elastic (young) modulus.
        /// </value>
        public double E
        {
            get { return e; }
            set { e = value; }
        }

        /// <summary>
        /// Gets or sets the shear modulus in [Pas] dimension.
        /// </summary>
        /// <value>
        /// The shear modulus.
        /// </value>
        public double G
        {
            get { return g; }
            set { g = value; }
        }
    }
}
