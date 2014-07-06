using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a base class for loads that are applicable to 1D elements like frame element
    /// </summary>
    public abstract class Load1D : Load
    {
        protected Direction direction;
        protected CoordinationSystem coordinationSystem;

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>
        /// The direction of load.
        /// </value>
        public Direction Direction
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
    }
}
