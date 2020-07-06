namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// /// represents some predefined most used behaviors of triangle element
    /// </summary>
    public static class PlaneElementBehaviours
    {
        /// <summary>
        /// The plate bending behavior
        /// </summary>
        public static PlaneElementBehaviour PlateBending = PlaneElementBehaviour.ThinPlate;

        /// <summary>
        /// The membrane behavior
        /// </summary>
        public static PlaneElementBehaviour Membrane = PlaneElementBehaviour.Membrane;

        /// <summary>
        /// The full thin shell, membrane + thin plate + drilling DoF
        /// </summary>
        public static PlaneElementBehaviour FullThinShell =
            PlaneElementBehaviour.Membrane |
            PlaneElementBehaviour.ThinPlate |
            PlaneElementBehaviour.DrillingDof;
    }

}