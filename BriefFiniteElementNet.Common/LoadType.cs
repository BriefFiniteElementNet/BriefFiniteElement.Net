using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a loadType.
    /// </summary>
    public enum LoadType
    {
        /// <summary>
        /// The default load
        /// </summary>
        Default = 0,
        /// <summary>
        /// The dead
        /// </summary>
        Dead,
        /// <summary>
        /// The live
        /// </summary>
        Live,
        /// <summary>
        /// The snow
        /// </summary>
        Snow,
        /// <summary>
        /// The wind
        /// </summary>
        Wind,
        /// <summary>
        /// The quake
        /// </summary>
        Quake,
        /// <summary>
        /// The crane
        /// </summary>
        Crane,
        /// <summary>
        /// Any type which cannot categorized in above items
        /// </summary>
        Other
    }
}
