using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents the formulation type of mass matrix
    /// </summary>
    public enum MassFormulation
    {
        /// <summary>
        /// The lumped mass formulation
        /// </summary>
        Lumped,

        /// <summary>
        /// The consistent mass formulation
        /// </summary>
        Consistent
    }
}
