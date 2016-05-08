using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet
{
    [Serializable]
    public sealed class CustomLinearLoad1D : Load1D
    {
        /// <summary>
        /// Gets the equivalent nodal loads of this <see cref="Load" /> when applied to <see cref="element" />.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// Concentrated loads appropriated with this <see cref="Load" />.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks>
        /// Because of every <see cref="Load" /> on an <see cref="Element" /> body have to be converted to concentrated nodal loads, this method will be used to consider <see cref="Load" /> on <see cref="Element" /> body
        /// </remarks>
        public override Force[] GetGlobalEquivalentNodalLoads(Element element)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the internal force at.
        /// </summary>
        /// <param name="elm">The elm.</param>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override Force GetInternalForceAt(Element1D elm, double x)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomLinearLoad1D"/> class.
        /// </summary>
        public CustomLinearLoad1D()
        {
        }

        #region Serialization Stuff

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Load2D"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private CustomLinearLoad1D(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
