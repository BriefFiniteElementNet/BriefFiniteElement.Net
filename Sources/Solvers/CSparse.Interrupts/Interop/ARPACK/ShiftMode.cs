namespace CSparse.Interop.ARPACK
{
    public enum ShiftMode
    {
        /// <summary>
        /// No shift applied.
        /// </summary>
        None,
        /// <summary>
        /// Regular shift-invert mode.
        /// </summary>
        Regular,
        /// <summary>
        /// Buckling mode.
        /// </summary>
        Buckling,
        /// <summary>
        /// Cayley mode.
        /// </summary>
        Cayley
    }
}
