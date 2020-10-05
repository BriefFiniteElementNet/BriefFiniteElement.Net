using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Utility class for PlateElementBehaviour 
    /// </summary>
    [Obsolete("use PlaneElementBehaviours instead")]
    public static class PlateElementBehaviours
    {
        /// <summary>
        /// The full shell: bending + membrane + drilling dof
        /// </summary>
        public static readonly PlateElementBehaviours Shell = PlateElementBehaviour.DrillingDof | PlateElementBehaviour.Membrane | PlateElementBehaviour.Bending;

        /// <summary>
        /// Only Membrane behaviour
        /// </summary>
        public static readonly PlateElementBehaviours Membrane = PlateElementBehaviour.Membrane;

        /// <summary>
        /// Only Plate Bending behaviour
        /// </summary>
        public static readonly PlateElementBehaviours PlateBending = PlateElementBehaviour.Bending;
    }
}
