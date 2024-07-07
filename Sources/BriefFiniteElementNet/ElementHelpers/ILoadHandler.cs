using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers
{

    /// <summary>
    /// represents a load handler which handles a particular load applied on a particular element
    /// </summary>
    public interface ILoadHandler
    {
        /// <summary>
        /// Gets the equivalent nodal loads because of applying <see cref="load" /> on the <see cref="targetElement" /> in local coordination system.
        /// </summary>
        /// <param name="elm">The target element.</param>
        /// <param name="hlpr"></param>
        /// <returns>The equivaled nodal load in local coordination system</returns>
        /// <param name="load">The load.</param>
        Force[] GetLocalEquivalentNodalLoads(Element elm, IElementHelper hlpr, ElementalLoad load);

        /// <summary>
        /// Gets the displacement of element, only due to applying specified load. where all nodes are assumed as fixed
        /// and inverse of eq nodal loads are applied to the element.
        /// </summary>
        /// <param name="elm">The target element.</param>
        /// <param name="hlpr"></param>
        /// <param name="load">The load.</param>
        /// <param name="loc">The iso location.</param>
        Displacement GetLocalLoadDisplacementAt(Element elm, IElementHelper hlpr, ElementalLoad load, IsoPoint loc);

        /// <summary>
        /// Gets the stress at defined location in local coordination system.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="load">The load.</param>
        /// <param name="isoLocation">The iso location.</param>
        /// <returns>load internal force at defined iso coordination</returns>
        /// <summary>
        /// Gets the internal force of element, only due to applying specified load.
        /// </summary>
        /// <remarks>
        /// This gives back internal force of element assuming no nodal displacements there are, and only the <see cref="load"/> is applied to it.
        /// Element under inverse of eq nodal loads of load and under the specified load itself
        /// 
        /// Return type is object. return type for bar element is instance of Force, for triangle is bending and cauchy
        /// tensor combined and for tetrahedron it is cauchy tensor. return type is then casted from object datatype 
        /// to appropriated type by caller of this method which is IElementHelper.GetExactInternalForce() method for
        /// each type of helper
        /// </remarks>
        object GetLocalLoadInternalForceAt(Element elm, IElementHelper hlpr, ElementalLoad load, IsoPoint loc);

        /// <summary>
        /// Determines wether this handler can handle the combination of load and element or not
        /// </summary>
        /// <param name="elm"></param>
        /// <param name="hlpr"></param>
        /// <param name="load"></param>
        /// <returns></returns>
        bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load);
    }
}
