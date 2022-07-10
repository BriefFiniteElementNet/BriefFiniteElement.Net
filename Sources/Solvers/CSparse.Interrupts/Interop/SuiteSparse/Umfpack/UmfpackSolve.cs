
namespace CSparse.Interop.SuiteSparse.Umfpack
{
    /// <summary>
    /// Umfpack solve codes.
    /// </summary>
    /// <remarks>
    /// Solve the system ( )x=b, where ( ) is defined below.  "t" refers to the
    /// linear algebraic transpose (complex conjugate if A is complex), or the (')
    /// operator in MATLAB.  "at" refers to the array transpose, or the (.')
    /// operator in MATLAB.
    /// </remarks>
    public enum UmfpackSolve
    {
        /// <summary>
        /// Ax=b
        /// </summary>
        A = 0,
        /// <summary>
        /// A'x=b
        /// </summary>
        At = 1,
        /// <summary>
        /// A.'x=b 
        /// </summary>
        Aat = 2,

        /// <summary>
        /// P'Lx=b
        /// </summary>
        Pt_L = 3,
        /// <summary>
        /// Lx=b
        /// </summary>
        L = 4,
        /// <summary>
        /// L'Px=b
        /// </summary>
        Lt_P = 5,
        /// <summary>
        /// L.'Px=b
        /// </summary>
        Lat_P = 6,
        /// <summary>
        /// L'x=b
        /// </summary>
        Lt = 7,
        /// <summary>
        /// L.'x=b
        /// </summary>
        Lat = 8,

        /// <summary>
        /// UQ'x=b
        /// </summary>
        U_Qt = 9,
        /// <summary>
        /// Ux=b
        /// </summary>
        U = 10,
        /// <summary>
        /// QU'x=b
        /// </summary>
        Q_Ut = 11,
        /// <summary>
        /// QU.'x=b
        /// </summary>
        Q_Uat = 12,
        /// <summary>
        /// U'x=b
        /// </summary>
        Ut = 13,
        /// <summary>
        /// U.'x=b
        /// </summary>
        Uat = 14
    }
}
