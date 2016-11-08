using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents a base class for BarElement's materials
    /// </summary>
    public abstract class BaseBarElementMaterial
    {
        /// <summary>
        /// Gets the material properties at defined length.
        /// </summary>
        /// <param name="x">The location, in [-1,1] range. -1 for beginning of element, 1 for end of element</param>
        /// <returns>the mechanical properties of element</returns>
        public abstract BarCrossSectionMechanicalProperties GetMaterialPropertiesAt(double x);


        /// <summary>
        /// Gets the list of points that integration should be sampled there.
        /// </summary>
        /// <returns>the list of points, all in [0,1] range</returns>
        public abstract double[] GetIntegrationPoints();
    }
}
