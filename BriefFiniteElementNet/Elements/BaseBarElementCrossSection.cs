using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents a base class for cross sections for bar element.
    /// </summary>
    public abstract class BaseBarElementCrossSection
    {
        /// <summary>
        /// Gets the cross section properties at defined location.
        /// </summary>
        /// <param name="x">The location, [-1,1] range.</param>
        /// <returns></returns>
        public abstract BarCrossSectionGeometricProperties GetCrossSectionPropertiesAt(double x);

        /// <summary>
        /// Gets the list of points that integration should be sampled there.
        /// </summary>
        /// <returns>the list of points, all in [-1,1] range</returns>
        public abstract double[] GetIntegrationPoints();
    }
}
