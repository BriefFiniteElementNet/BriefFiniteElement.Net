using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.CSparse.Double;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represent some stuff for checking the stiffness matrix and predict potentially possible errors in solve process (like not pos def error)
    /// </summary>
    public class StiffnessMatrixCheckingUtility
    {
        public bool HaveError;

        public string Error;

        /// <summary>
        /// Checks the stiffness matrix for errors.
        /// </summary>
        /// <param name="kff">The Kff.</param>
        /// <param name="model">The model.</param>
        public void CheckStiffnessMatrix(CompressedColumnStorage kff, Model model)
        {
            throw new NotImplementedException();
        }
    }
}
