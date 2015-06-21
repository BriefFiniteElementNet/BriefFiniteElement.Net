using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class Load:ISerializable
    {
        protected Load()
        {
        }

        private LoadCase _case;

        /// <summary>
        /// Gets or sets the case.
        /// </summary>
        /// <value>
        /// The LoadCase of <see cref="Load"/> object.
        /// </value>
        public LoadCase Case
        {
            get { return _case; }
            set { _case = value; }
        }

        /// <summary>
        /// Gets the equivalent nodal loads of this <see cref="Load"/> when applied to <see cref="element"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <remarks>Because of every <see cref="Load"/> on an <see cref="Element"/> body have to be converted to concentrated nodal loads, this method will be used to consider <see cref="Load"/> on <see cref="Element"/> body</remarks>
        /// <returns>Concentrated loads appropriated with this <see cref="Load"/>.</returns>
        public abstract Force[] GetGlobalEquivalentNodalLoads(Element element);

        #region Serialization stuff

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("case", _case);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Load"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected Load(SerializationInfo info, StreamingContext context)
        {
            _case = info.GetValue<LoadCase>("case");
        }

        #endregion



    }
}