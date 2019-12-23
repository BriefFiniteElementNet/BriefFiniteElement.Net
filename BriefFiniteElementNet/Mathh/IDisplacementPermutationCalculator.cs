using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSparse;
using CSparse.Double;
using CCS = CSparse.Double.SparseMatrix;

namespace BriefFiniteElementNet.Mathh
{
    /// <summary>
    /// interface to calculating the displacement permutation matrix P_d
    /// for more information on what is displacement permute, have a look at documentation, section 'solving procedures'
    /// </summary>
    public interface IDisplacementPermutationCalculator
    {
        /// <summary>
        /// Calculates the displacement permutation matrix P_d based on input <see cref="a"/> matrix.
        /// </summary>
        /// <param name="a">the conditions matrix, size m by n+1 where m is arbitrary count of equations and n is total number of DoFs, note that right side is inserted into  parameter</param>
        /// <returns></returns>
        Tuple<CCS, double[]> CalculateDisplacementPermutation(CCS a);
    }
}
