using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers.LoadHandlers
{

    /// <summary>
    /// represents a zero load handler which it's output is always zero
    /// for code reuse porpuse only
    /// </summary>
    public abstract class ZeroLoadHandler : ILoadHandler
    {
        public abstract bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load);

        public Force[] GetLocalEquivalentNodalLoads(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            return new Force[elm.Nodes.Length];
        }

        public Displacement GetLocalLoadDisplacementAt(Element elm, IElementHelper hlpr, ElementalLoad load, IsoPoint loc)
        {
            return Displacement.Zero;
        }

        public object GetLocalLoadInternalForceAt(Element targetElement, IElementHelper hlpr, ElementalLoad load, IsoPoint loc)
        {
            return new CauchyStressTensor();//zero
        }
    }
}
