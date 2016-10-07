using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents the possible behavior of Bar Element
    /// </summary>
    [Flags]
    public enum BarElementBehaviour
    {
        /// <summary>
        /// The truss behavior
        /// </summary>
        Truss = 1,
        /// <summary>
        /// The beam in local Y direction behavior
        /// </summary>
        BeamY = 2,
        /// <summary>
        /// The beam in local Y direction behavior considering shear deformations
        /// </summary>
        BeamYWithShearDefomation = 4,
        /// <summary>
        /// The beam in local Z direction behavior
        /// </summary>
        BeamZ = 8,
        /// <summary>
        /// The beam in local Z direction behavior considering shear deformations
        /// </summary>
        BeamZWithShearDefomation = 16,
        /// <summary>
        /// The shaft (only torsion)
        /// </summary>
        Shaft = 32,

    }
}
