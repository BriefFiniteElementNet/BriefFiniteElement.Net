using System;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.Materials
{
    /// <summary>
    /// Represents a base class for BarElement's materials
    /// </summary>
    [Obsolete("Use BaseMaterial instead")]
    public abstract class BaseBarMaterial
    {
        /// <summary>
        /// Gets the material properties at defined length.
        /// </summary>
        /// <param name="x">The location, in [-1,1] range. -1 for beginning of element (begin node), 1 for end of element (end node)</param>
        /// <returns>the mechanical properties of element</returns>
        public abstract BarCrossSectionMechanicalProperties GetMaterialPropertiesAt(double x);


        /// <summary>
        /// Gets the maximum order (degree) of members of material regarding xi.
        /// </summary>
        /// <remarks>
        /// Will use for determining Gaussian sampling count.
        /// </remarks>
        /// <example>
        /// for example if E is a*xi^2 + b*xi + c then max order will be 2.
        /// </example>
        /// <returns>the number of Gaussian integration points needed</returns>
        public abstract int GetMaxFunctionOrder();
    }
}
