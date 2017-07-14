using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents an interface for finders of RREF (Reduced Row Echelon Form)
    /// </summary>
    public interface IRrefFinder
    {
        /// <summary>
        /// Calculates the RREF (Reduced Row Echelon Form) of the matrix <see cref="a"/>
        /// </summary>
        /// <param name="a">the matrix</param>
        /// <returns>RREF form</returns>
        CSparse.Double.CompressedColumnStorage CalculateRref(CSparse.Double.CompressedColumnStorage a);
    }
}
