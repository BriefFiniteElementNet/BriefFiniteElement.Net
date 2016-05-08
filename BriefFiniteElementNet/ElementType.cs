using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents an enumeration for determining type of <see cref="Element"/>s.
    /// </summary>
    public enum ElementType
    {
        /// <summary>
        /// Default value.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// <see cref="ElementType"/> associated with <see cref="BriefFiniteElementNet.FrameElement2Node"/>
        /// </summary>
        FrameElement2Node = 1,

        /// <summary>
        /// <see cref="ElementType"/> associated with <see cref="TrussElement2Node"/>
        /// </summary>
        TrussElement2Noded = 2,


        /// <summary>
        /// <see cref="ElementType"/> associated with <see cref="TetrahedralIso"/>
        /// </summary>
        TetrahedralIso = 3,

        /// <summary>
        /// <see cref="ElementType"/> associated with <see cref="ConcentratedMass"/>
        /// </summary>
        ConcentratedMass = 4,

        /// <summary>
        /// <see cref="ElementType"/> associated with <see cref="DktElement"/>
        /// </summary>
        Dkt = 5,

        /// <summary>
        /// <see cref="ElementType"/> associated with <see cref="DkqElement"/>
        /// </summary>
        Dkq=6,

        /// <summary>
        /// <see cref="ElementType"/> associated with <see cref="CstElement"/>
        /// </summary>
        Cst=7
    }
}
