using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers.LoadHandlers.TriangleDrillingDofHelper
{
    public class GeneralHandler : ZeroLoadHandler
    {
        public override bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            throw new NotImplementedException();
        }
    }
}
