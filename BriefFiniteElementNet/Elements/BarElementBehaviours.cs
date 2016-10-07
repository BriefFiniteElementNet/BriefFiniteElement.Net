using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// represents some predefined most used behaviors of bar element
    /// </summary>
    public static class BarElementBehaviours
    {
        /// <summary>
        /// The truss behavior
        /// </summary>
        public static BarElementBehaviour Truss = BarElementBehaviour.Truss;

        /// <summary>
        /// The Beam in local Y direction
        /// </summary>
        public static BarElementBehaviour BeamY = BarElementBehaviour.BeamY;

        /// <summary>
        /// The Beam in local Y direction, considering shear deformations
        /// </summary>
        public static BarElementBehaviour BeamYWithShearDeformation = BarElementBehaviour.BeamYWithShearDefomation;

        /// <summary>
        /// The Beam in local Z direction
        /// </summary>
        public static BarElementBehaviour BeamZ = BarElementBehaviour.BeamZ;

        /// <summary>
        /// The Beam in local Z direction, considering shear deformations
        /// </summary>
        public static BarElementBehaviour BeamZWithShearDefomation = BarElementBehaviour.BeamZWithShearDefomation;

        /// <summary>
        /// The full beam: beam in both local Y and Z direction
        /// </summary>
        public static BarElementBehaviour FullBeam = BarElementBehaviour.BeamY & BarElementBehaviour.BeamZ;

        /// <summary>
        /// The full beam considering shear deformation: beam in both local Y and Z direction considering shear deformations
        /// </summary>
        public static BarElementBehaviour FullBeamWithShearDefomation = BarElementBehaviour.BeamYWithShearDefomation & BarElementBehaviour.BeamZWithShearDefomation;

        /// <summary>
        /// The full frame: truss + shaft + beam in both local Y and Z direction
        /// </summary>
        public static BarElementBehaviour FullFrame = BarElementBehaviour.Truss & BarElementBehaviour.BeamY & BarElementBehaviour.BeamZ & BarElementBehaviour.Shaft;

        /// <summary>
        /// The full frame considering shear deformation: truss + shaft + beam in both local Y and Z direction considering shear deformations
        /// </summary>
        public static BarElementBehaviour FullFrameWithShearDeformation = BarElementBehaviour.Truss & BarElementBehaviour.BeamYWithShearDefomation & BarElementBehaviour.BeamZWithShearDefomation & BarElementBehaviour.Shaft;
    }
}
