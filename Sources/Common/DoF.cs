using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a DoF in global system
    /// Remarks: Do not use in FiniteElement.NET project!
    /// Only is used in SDoF mass which is not useful
    /// 
    /// </summary>
    public enum DoF
    {
        /// <summary>
        /// Displacement in X direction
        /// </summary>
        Dx = 0,

        /// <summary>
        /// Displacement in Y direction
        /// </summary>
        Dy = 1,

        /// <summary>
        /// Displacement in Z direction
        /// </summary>
        Dz = 2,

        /// <summary>
        /// Rotation in X direction
        /// </summary>
        Rx = 3,

        /// <summary>
        /// Rotation in Y direction
        /// </summary>
        Ry = 4,

        /// <summary>
        /// Rotation in Z direction
        /// </summary>
        Rz = 5
    }
}