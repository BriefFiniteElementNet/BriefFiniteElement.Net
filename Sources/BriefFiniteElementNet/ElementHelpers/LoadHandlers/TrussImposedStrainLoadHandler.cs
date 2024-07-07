using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers.LoadHandlers
{

    /// <summary>
    /// Handles ImposedStrainLoad on TrussHelper element and uniform material and section
    /// </summary>
    public class TrussImposedStrainLoadHandler : ILoadHandler
    {
        public bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            if (!(hlpr is TrussHelper2Node))
                return false;

            if (!(load is ImposedStrainLoad))
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

        public Force[] GetLocalEquivalentNodalLoads(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            var isLoad /*is: Imposed Strain*/= load as ImposedStrainLoad;

            var bar = elm as BarElement;
            var mat = bar.Material;
            var sec = bar.Section;

            var E = mat.GetMaterialPropertiesAt(IsoPoint.Origins, bar).Ex;
            var A = sec.GetCrossSectionPropertiesAt(0, bar).A;
            var strain = isLoad.ImposedStrainMagnitude;

            var F = strain * E * A;

            throw new NotImplementedException();
        }

        public Displacement GetLocalLoadDisplacementAt(Element elm, IElementHelper hlpr, ElementalLoad load, IsoPoint loc)
        {
            return Displacement.Zero;
        }

        public object GetLocalLoadInternalForceAt(Element elm, IElementHelper hlpr, ElementalLoad load, IsoPoint loc)
        {
            var isLoad = load as ImposedStrainLoad;

            var bar = elm as BarElement;
            var mat = bar.Material;
            var sec = bar.Section;

            var E = mat.GetMaterialPropertiesAt(0).Ex;
            var A = sec.GetCrossSectionPropertiesAt(0).A;
            var strain = isLoad.ImposedStrainMagnitude;

            var F = strain * E * A;

            throw new NotImplementedException();
        }
    }
}



