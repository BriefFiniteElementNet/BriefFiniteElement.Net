using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    public static class TriangleElementBehaviours
    {
        /// <summary>
        /// The full shell: bending + membrane + drilling dof
        /// </summary>
        public static readonly TriangleElementBehaviour Shell = TriangleElementBehaviour.DrillingDof | TriangleElementBehaviour.Membrane | TriangleElementBehaviour.Bending;

        /// <summary>
        /// Only Membrane behaviour
        /// </summary>
        public static readonly TriangleElementBehaviour Membrane = TriangleElementBehaviour.Membrane;

        /// <summary>
        /// Only Plate Bending behaviour
        /// </summary>
        public static readonly TriangleElementBehaviour PlateBending = TriangleElementBehaviour.Bending;
    }
}
