using System;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents the possible behaviors of shell element
    /// </summary>
    [Flags]
    public enum FlatShellBehaviour
    {

        /// <summary>
        /// The thin plate, based on discrete Kirchhoff theory, only bending behavior
        /// </summary>
        ThinPlate = 1,

        /// <summary>
        /// The membrane, only in-plane forces, no moments. Only membrane behavior.
        /// </summary>
        Membrane = 2,

        /// <summary>
        /// The drilling dof
        /// </summary>
        DrillingDof = 4

    }


    /// <summary>
    /// /// represents some predefined most used behaviors of triangle element
    /// </summary>
    public static class FlatShellBehaviours
    {
        /// <summary>
        /// The plate bending behavior
        /// </summary>
        public static FlatShellBehaviour PlateBending = FlatShellBehaviour.ThinPlate;

        /// <summary>
        /// The membrane behavior
        /// </summary>
        public static FlatShellBehaviour Membrane = FlatShellBehaviour.Membrane;

        /// <summary>
        /// The full thin shell, membrane + thin plate + drilling DoF
        /// </summary>
        public static FlatShellBehaviour FullThinShell =
            FlatShellBehaviour.Membrane |
            FlatShellBehaviour.ThinPlate |
            FlatShellBehaviour.DrillingDof;
    }

}