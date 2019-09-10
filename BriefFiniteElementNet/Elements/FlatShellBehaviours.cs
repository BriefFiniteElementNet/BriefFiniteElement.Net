namespace BriefFiniteElementNet.Elements
{
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