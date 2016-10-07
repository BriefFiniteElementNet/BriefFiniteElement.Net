using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    public abstract class BaseBarElemenetCrossSection
    {
        /// <summary>
        /// Gets the cross section properties at defined location.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="A">a.</param>
        /// <param name="Iy">The iy.</param>
        public abstract void GetCrossSectionPropertiesAt(double x, out double A, out double Iy);//x in [0,1] range

        /// <summary>
        /// Gets the list of points that integration should be sampled there.
        /// </summary>
        /// <returns>the list of points, all in [0,1] range</returns>
        public abstract double[] GetIntegrationPoints();
    }
}
