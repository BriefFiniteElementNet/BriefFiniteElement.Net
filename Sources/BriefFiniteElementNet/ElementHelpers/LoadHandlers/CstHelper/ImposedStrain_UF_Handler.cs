using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers.LoadHandlers.CstHelper
{
    public class ImposedStrain_UF_Handler : ILoadHandler
    {
        public bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            throw new NotImplementedException();
        }

        public Force[] GetLocalEquivalentNodalLoads(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            throw new NotImplementedException();
        }

        public Displacement GetLocalLoadDisplacementAt(Element elm, IElementHelper hlpr, ElementalLoad load, IsoPoint loc)
        {
            throw new NotImplementedException();
        }

        public CauchyStressTensor GetLocalLoadInternalForceAt(Element elm, IElementHelper hlpr, ElementalLoad load, IsoPoint loc)
        {
            throw new NotImplementedException();
        }
    }
}
