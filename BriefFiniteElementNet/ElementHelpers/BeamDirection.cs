using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.ElementHelpers
{
    /// <summary>
    /// Represents a direction for <see cref="BarElement"/>'s beam behavior in it's local coordination system.
    /// </summary>
    public enum BeamDirection
    {
        /// <summary>
        /// Beam in Y direction, which means start and end nodes does rotate in Y direction
        /// </summary>
        Y,

        /// <summary>
        /// Beam in Z direction, which means start and end nodes does rotate in Z direction
        /// </summary>
        Z
    }
}
