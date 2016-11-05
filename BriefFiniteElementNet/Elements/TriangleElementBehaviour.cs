using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents the possible behaviors of Triangle Element
    /// </summary>
    [Flags]
    public enum TriangleElementBehaviour
    {
        /// <summary>
        /// The thin plate bending behavior, based on DKT (discrete Kirchoff triangle).
        /// </summary>
        ThinBending = 1,

        /// <summary>
        /// The membrane behavior with Plane Stress assumption. based on CST (Constant Stress/Strain Triangle) element.
        /// </summary>
        PlaneStressMembrane = 2,

        /// <summary>
        /// The membrane behavior with Plane Strain assumption. based on CST (Constant Stress/Strain Triangle) element.
        /// </summary>
        PlaneStrainMembrane = 4,

        /// <summary>
        /// Add drilling DoF to the element
        /// </summary>
        DrillingDof = 4
    }
}
