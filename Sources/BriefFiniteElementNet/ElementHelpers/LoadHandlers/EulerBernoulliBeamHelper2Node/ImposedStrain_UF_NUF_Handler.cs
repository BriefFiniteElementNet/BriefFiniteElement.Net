using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node
{
    /// <summary>
    /// Handles ImposedStrainLoad on equaler bernaully element and uniform material and section
    /// </summary>
    /// <remarks>
    /// Imposed strain do not affect euler bernauly, output always zero so inherit from ZeroLoadHandler</remarks>
    public class ImposedStrain_UF_NUF_Handler : ZeroLoadHandler
    {
        public override bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            if (!(hlpr is BriefFiniteElementNet.ElementHelpers.BarHelpers.EulerBernoulliBeamHelper2Node))
                return false;

            if (!(load is ImposedStrainLoad))
                return false;

            if (!(elm is BarElement))
                return false;

            var bar = elm as BarElement;
            
            //regardless of mat and section, output is always zero 

            //var mat = bar.Material;
            //var sec = bar.Section;

            //if (mat.GetMaxFunctionOrder()[0] != 0)//constant uniform material through length
            //    return false;

            //if (sec.GetMaxFunctionOrder()[0] != 0)//constant uniform section through length
            //    return false;

            return true;
        }

    }
}


