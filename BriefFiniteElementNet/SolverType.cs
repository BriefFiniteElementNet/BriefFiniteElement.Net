using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents the type of solver for use in solving equation system
    /// </summary>
    public enum SolverType
    {
        /// <summary>
        /// Represents the Cholesky Decomposition Method
        /// </summary>
        [Description("Cholesky Decomposition")] 
        CholeskyDecomposition,
        /// <summary>
        /// Represents the Conjugate Gradient Method
        /// </summary>
        [Description("Conjugate Gradient")] 
        ConjugateGradient
    }
}
