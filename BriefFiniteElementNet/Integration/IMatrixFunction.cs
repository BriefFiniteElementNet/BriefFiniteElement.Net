using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BriefFiniteElementNet.Integration
{
    /// <summary>
    /// Represents an interface for a class that gets three parameters ξ, η and γ and computes a matrix as result (function of ξ, η and γ)
    /// </summary>
    public interface IMatrixFunction
    {
        /// <summary>
        /// Gets three parameters ξ, η and γ. then computes output matrix.
        /// </summary>
        /// <param name="xi">The ξ.</param>
        /// <param name="eta">The η.</param>
        /// <param name="gamma">The γ.</param>
        /// <returns>result matrix</returns>
        Matrix GetMatrix(double xi, double eta, double gamma);
    }
}
