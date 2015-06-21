using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a base class for loads that are applicable to 1D elements like frame element
    /// </summary>
    [Serializable]
    public abstract class Load1D : Load
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Load1D"/> class.
        /// </summary>
        protected Load1D()
        {
        }

        protected LoadDirection direction;
        protected CoordinationSystem coordinationSystem;

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>
        /// The direction of load.
        /// </value>
        public LoadDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        /// <summary>
        /// Gets or sets the coordination system.
        /// </summary>
        /// <value>
        /// The coordination system that load should be applied.
        /// </value>
        public CoordinationSystem CoordinationSystem
        {
            get { return coordinationSystem; }
            set { coordinationSystem = value; }
        }


        public abstract Force GetInternalForceAt(Element1D elm, double x);


         #region Serialization stuff

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("direction", (int)direction);
            info.AddValue("coordinationSystem", (int)coordinationSystem);

            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Load1D"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        internal Load1D(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.direction = (LoadDirection) info.GetInt32("direction");
            this.coordinationSystem = (CoordinationSystem) info.GetInt32("coordinationSystem");
        }

        #endregion
    }
}
