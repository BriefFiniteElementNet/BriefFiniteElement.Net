using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.Materials
{
    /// <summary>
    /// Represents a base class for TriangleElement's materials
    /// </summary>
    [Obsolete("Use BaseMaterial instead")]
    public abstract class BaseTriangleMaterial
    {
        /// <summary>
        /// Gets the material properties at defined length.
        /// </summary>
        /// <param name="isoCoords">The location, in isoparametric coordination</param>
        /// <returns>the mechanical properties of element</returns>
        public abstract TriangleCoordinatedMechanicalProperties GetMaterialPropertiesAt
            (TriangleElement targetElement, params double[] isoCoords);


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
