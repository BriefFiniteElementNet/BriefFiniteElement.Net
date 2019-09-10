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
        public static readonly BarElementBehaviour Truss = BarElementBehaviour.Truss;

        /// <summary>
        /// The Beam in local Y direction
        /// </summary>
        public static readonly BarElementBehaviour BeamY = BarElementBehaviour.BeamYEulerBernoulli;

        /// <summary>
        /// The Beam in local Y direction, considering shear deformations
        /// </summary>
        public static readonly BarElementBehaviour BeamYWithShearDeformation = BarElementBehaviour.BeamYTimoshenko;

        /// <summary>
        /// The Beam in local Z direction
        /// </summary>
        public static readonly BarElementBehaviour BeamZ = BarElementBehaviour.BeamZEulerBernoulli;

        /// <summary>
        /// The Beam in local Z direction, considering shear deformations
        /// </summary>
        public static readonly BarElementBehaviour BeamZWithShearDefomation = BarElementBehaviour.BeamZTimoshenko;

        /// <summary>
        /// The full beam: beam in both local Y and Z direction
        /// </summary>
        public static readonly BarElementBehaviour FullBeam = BarElementBehaviour.BeamYEulerBernoulli | BarElementBehaviour.BeamZEulerBernoulli;

        /// <summary>
        /// The full beam considering shear deformation: beam in both local Y and Z direction considering shear deformations
        /// </summary>
        public static readonly BarElementBehaviour FullBeamWithShearDefomation = BarElementBehaviour.BeamYTimoshenko | BarElementBehaviour.BeamZTimoshenko;

        /// <summary>
        /// The full frame: truss + shaft + beam in both local Y and Z direction
        /// </summary>
        public static readonly BarElementBehaviour FullFrame = BarElementBehaviour.Truss | BarElementBehaviour.BeamYEulerBernoulli | BarElementBehaviour.BeamZEulerBernoulli | BarElementBehaviour.Shaft;

        /// <summary>
        /// The full frame considering shear deformation: truss + shaft + beam in both local Y and Z direction considering shear deformations
        /// </summary>
        public static readonly BarElementBehaviour FullFrameWithShearDeformation = BarElementBehaviour.Truss | BarElementBehaviour.BeamYTimoshenko | BarElementBehaviour.BeamZTimoshenko | BarElementBehaviour.Shaft;
    }
}
