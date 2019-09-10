using System.Runtime.Serialization;
using BriefFiniteElementNet.Elements;
using System;
using System.Security.Permissions;

namespace BriefFiniteElementNet.Sections
{
    /// <summary>
    /// Represents a base class for cross sections for 1D elements like bar etc.
    /// </summary>
    [Serializable]
    public abstract class Base1DSection:ISerializable
    {
        protected Base1DSection()
        {

        }

        /// <summary>
        /// Gets the cross section properties at defined location.
        /// </summary>
        /// <param name="x">The location, [-1,1] range.</param>
        /// <returns></returns>
        public abstract _1DCrossSectionGeometricProperties GetCrossSectionPropertiesAt(double xi);

        /// <summary>
        /// Gets the maximum order (degree) of members of material regarding xi.
        /// </summary>
        /// <remarks>
        /// Will use for determining Gaussian sampling count.
        /// </remarks>
        /// <example>
        /// for example if Iy in length is form of a*xi^2 + b*xi + c and A is d*xi + e, then max order will be 2.
        /// if section is constant and uniform along the length and does not change, then Iy and other paramters do not vary then maximum order will be zero.
        /// </example>
        /// <returns>the maximum order of variable function </returns>
        public abstract int[] GetMaxFunctionOrder();



        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        protected Base1DSection(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
