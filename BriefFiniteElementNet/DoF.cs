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
        Dx,

        /// <summary>
        /// Displacement in Y direction
        /// </summary>
        Dy,

        /// <summary>
        /// Displacement in Z direction
        /// </summary>
        Dz,

        /// <summary>
        /// Rotation in X direction
        /// </summary>
        Rx,

        /// <summary>
        /// Rotation in Y direction
        /// </summary>
        Ry,

        /// <summary>
        /// Rotation in Z direction
        /// </summary>
        Rz
    }
}