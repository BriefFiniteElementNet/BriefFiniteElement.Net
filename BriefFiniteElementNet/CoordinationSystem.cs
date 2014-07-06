using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a coordinations system (majority for defining the position of a load)
    /// </summary>
    public enum CoordinationSystem
    {
        /// <summary>
        /// The local coordination system
        /// </summary>
        Local,

        /// <summary>
        /// The global coordination system
        /// </summary>
        Global
    }
}
