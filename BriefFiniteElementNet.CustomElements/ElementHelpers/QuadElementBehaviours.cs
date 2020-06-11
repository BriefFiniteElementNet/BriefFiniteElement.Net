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
