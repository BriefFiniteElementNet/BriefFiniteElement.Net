using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSC = CSparse.Double.SparseMatrix;

namespace BriefFiniteElementNet.Mathh
{
    public interface IEquationSystemReducer
    {
        /// <summary>
        /// Calculates a permutation matrix for reducing the system of eqations
        /// </summary>
        /// <param name="extraEquation"></param>
        /// <param name="rightSide"></param>
        /// <returns>
        /// 
        /// </returns>
        SSC GetPermutationMatrix(SSC extraEquation, double[] rightSide);

    }
}
