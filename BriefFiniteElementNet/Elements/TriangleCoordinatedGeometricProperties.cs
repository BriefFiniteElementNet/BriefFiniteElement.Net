using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents the geometric properties of triangle element in specific point
    /// </summary>
    [Obsolete]
    public struct TriangleCoordinatedGeometricProperties
    {
        private double _t;

        /// <summary>
        /// Gets or sets the thickness of section.
        /// </summary>
        /// <value>
        /// The t.
        /// </value>
        public double Thickness
        {
            get { return _t; }
            set { _t = value; }
        }
    }
}
