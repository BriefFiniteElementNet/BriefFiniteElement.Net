using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents the possible behaviors of 2d Plate Elements (like triangle)
    /// </summary>
    [Flags]
    [Obsolete("use PlaneElementBehaviour instead")]
    public enum PlateElementBehaviour
    {
        /// <summary>
        /// The plate bending behavior.
        /// </summary>
        Bending = 1,

        /// <summary>
        /// The membrane behavior.
        /// </summary>
        Membrane = 2,

        /// <summary>
        /// Add drilling DoF to the element
        /// </summary>
        DrillingDof = 4
    }
}
