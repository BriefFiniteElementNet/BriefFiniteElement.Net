using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents the constraints on a single degree of freedom
    /// </summary>
    public enum DofConstraint : byte
    {
        /// <summary>
        /// Indicates the degree of freedom is released
        /// </summary>
        Released = 0,

        /// <summary>
        /// Indicates the degree of freedom is Fixed
        /// </summary>
        Fixed = 1
    }
}
