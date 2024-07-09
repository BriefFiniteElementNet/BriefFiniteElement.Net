using BriefFiniteElementNet.ElementHelpers.Bar;
using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;
using BriefFiniteElementNet.Loads;
using MathNet.Numerics.RootFinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BriefFiniteElementNet.ElementHelpers.LoadHandlers.TimoshenkoBeamHelper
{
    public class ImposedStrain_UFNUF_Handler : ZeroLoadHandler
    {
        /// <inheritdoc/>
        public override bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            if (!(hlpr is Bar.TimoshenkoBeamHelper))
                return false;

            if (!(load is Loads.ImposedStrainLoad))
                return false;

            if (!(elm is BarElement))
                return false;

            var bar = elm as BarElement;

            var mat = bar.Material;
            var sec = bar.Section;

            if (mat.GetMaxFunctionOrder()[0] != 0)//constant uniform material through length
                return false;

            if (sec.GetMaxFunctionOrder()[0] != 0)//constant uniform section through length
                return false;


            return true;
        }

    }
}
