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
        public static TriangleElementBehaviour Shell = TriangleElementBehaviour.DrillingDof | TriangleElementBehaviour.Membrane | TriangleElementBehaviour.Bending;

        public static TriangleElementBehaviour Membrane = TriangleElementBehaviour.Membrane;

        public static TriangleElementBehaviour PlateBending = TriangleElementBehaviour.Bending;
    }
}
