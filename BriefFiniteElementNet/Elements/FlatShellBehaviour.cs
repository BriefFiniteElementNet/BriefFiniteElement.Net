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

}