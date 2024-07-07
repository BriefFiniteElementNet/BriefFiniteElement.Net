using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using CSparse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers.LoadHandlers.ShaftHelper
{
    public class GeneralHandler : ZeroLoadHandler
    {
        public override bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            if (!(hlpr is BriefFiniteElementNet.ElementHelpers.BarHelpers.ShaftHelper2Node))
                return false;

            if (!(elm is BarElement))
                return false;



            if ((load is ConcentratedLoad))
                return false;

            if ((load is UniformLoad))
                return true;

            if ((load is PartialNonUniformLoad))
                return true;

            if ((load is ImposedStrainLoad))
                return true;


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
