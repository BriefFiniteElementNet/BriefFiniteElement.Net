//---------------------------------------------------------------------------------------
//
// Project: VIT-V
//
// Program: BriefFiniteElement.Net (Quadrilateral Elements)
//
// Revision History
//
// Date          Author          	            Description
// 10.06.2020    T.Thaler, M.Mischke     	    v1.0  
// 
//---------------------------------------------------------------------------------------
// Copyright 2017-2020 by Brandenburg University of Technology. All rights reserved.
// This work is a research work and trade secret of the Brandenburg University of Technology. 
// Unauthorized use or copying requires an indication of the authors reference.
//---------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    public static class QuadElementBehaviours
    {
        /// <summary>
        /// The full shell: bending + membrane + drilling dof
        /// </summary>
        public static readonly QuadElementBehaviour Shell = QuadElementBehaviour.DrillingDof | QuadElementBehaviour.Membrane | QuadElementBehaviour.Bending;

        /// <summary>
        /// Only Membrane behaviour
        /// </summary>
        public static readonly QuadElementBehaviour Membrane = QuadElementBehaviour.Membrane;

        /// <summary>
        /// Only Plate Bending behaviour
        /// </summary>
        public static readonly QuadElementBehaviour PlateBending = QuadElementBehaviour.Bending;
    }
}
