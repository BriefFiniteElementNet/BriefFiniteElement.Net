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
    public enum BuiltInSolverType
    {
        /// <summary>
        /// Represents the Cholesky Decomposition Method
        /// </summary>
        [Description("Cholesky Decomposition (Direct)")] 
        CholeskyDecomposition,
        /// <summary>
        /// Represents the Conjugate Gradient Method
        /// </summary>
        [Description("Conjugate Gradient (Iterative)")] 
        ConjugateGradient
    }
}
