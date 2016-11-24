using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.Materials
{
    /// <summary>
    /// Represents a base class for BarElement's materials
    /// </summary>
    public abstract class BaseBarElementMaterial
    {
        /// <summary>
        /// Gets the material properties at defined length.
        /// </summary>
        /// <param name="x">The location, in [-1,1] range. -1 for beginning of element (begin node), 1 for end of element (end node)</param>
        /// <returns>the mechanical properties of element</returns>
        public abstract BarCrossSectionMechanicalProperties GetMaterialPropertiesAt(double x);


        /// <summary>
        /// Gets the number of Gaussian points that integration should be sampled there.
        /// </summary>
        /// <returns>the number of Gaussian integration points needed</returns>
        public abstract int GetGaussianIntegrationPoints();
    }
}
