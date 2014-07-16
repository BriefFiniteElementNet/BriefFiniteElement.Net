using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents an enumeration for determining type of <see cref="Element"/>s.
    /// </summary>
    public enum ElementType
    {
        /// <summary>
        /// Defauls value.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// <see cref="ElementType"/> associated with <see cref="BriefFiniteElementNet.FrameElement2Node"/>
        /// </summary>
        FrameElement2Node = 1,

        /// <summary>
        /// <see cref="ElementType"/> associated with <see cref="TrussElement2Node"/>
        /// </summary>
        TrussElement2Noded = 2
    }
}
