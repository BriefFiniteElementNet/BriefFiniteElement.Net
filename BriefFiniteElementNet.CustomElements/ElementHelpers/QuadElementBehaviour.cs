using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using System.Runtime.Serialization;
using System.Security.Permissions;
using BriefFiniteElementNet.Elements.ElementHelpers;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents the possible behaviors of Quad Element
    /// </summary>
    [Flags]
    public enum QuadElementBehaviour
    {
        /// <summary>
        /// The plate bending behavior.
        /// </summary>
        Bending = 1,

        /*
        /// <summary>
        /// The membrane behavior with Plane Stress assumption. based on CST (Constant Stress/Strain Triangle) element.
        /// </summary>
        PlaneStressMembrane = 2,


        /// <summary>
        /// The membrane behavior with Plane Strain assumption. based on CST (Constant Stress/Strain Triangle) element.
        /// </summary>
        PlaneStrainMembrane = 4,
        */

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

