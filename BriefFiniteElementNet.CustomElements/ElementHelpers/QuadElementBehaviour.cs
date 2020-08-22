//---------------------------------------------------------------------------------------
//
// Project: VIT-V
//
// Program: BriefFiniteElement.Net - QuadElementBehaviour.cs
//
// Revision History
//
// Date          Author          	            Description
// 11.06.2020    T.Thaler, M.Mischke     	    v1.0  
// 
//---------------------------------------------------------------------------------------
// Copyleft 2017-2020 by Brandenburg University of Technology. Intellectual proprerty 
// rights reserved in terms of LGPL license. This work is a research work of Brandenburg
// University of Technology. Use or copying requires an indication of the authors reference.
//---------------------------------------------------------------------------------------

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
    [Obsolete("Use PlateElementBehaviour instead")]
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

