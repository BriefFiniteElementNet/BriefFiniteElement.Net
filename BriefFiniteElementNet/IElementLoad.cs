using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    public interface IElementLoad
    {
        /// <summary>
        /// Gets or sets the case.
        /// </summary>
        /// <value>
        /// The LoadCase of <see cref="Load"/> object.
        /// </value>
        LoadCase Case { get; set; }



        /// <summary>
        /// Gets the equivalent nodal loads of this <see cref="Load"/> when applied to <see cref="element"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <remarks>Because of every <see cref="Load"/> on an <see cref="Element"/> body have to be converted to concentrated nodal loads, this method will be used to consider <see cref="Load"/> on <see cref="Element"/> body</remarks>
        /// <returns>Concentrated loads appropriated with this <see cref="Load"/>.</returns>
        Force[] GetEquivalentNodalLoads(Element element);
    }
}
