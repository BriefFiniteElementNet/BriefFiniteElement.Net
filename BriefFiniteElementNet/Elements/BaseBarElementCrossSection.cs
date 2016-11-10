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
        /// Gets the number of Gaussian points that integration should be sampled there.
        /// </summary>
        /// <returns>the number of Gaussian integration points needed</returns>
        public abstract int GetGaussianIntegrationPoints();
    }
}
