using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers.LoadHandlers.Truss2Node
{
    /// <summary>
    /// Handles ImposedStrainLoad on truss element and uniform material and section
    /// </summary>
    /// <remarks>
    /// Imposed strain do not affect euler bernauly</remarks>
    public class ImposedStrain_UF_Handler : ILoadHandler
    {
        public bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            if (!(hlpr is BriefFiniteElementNet.ElementHelpers.BarHelpers.TrussHelper2Node))
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
            var bar = elm as BarElement;
            var istLoad = load as ImposedStrainLoad;

            var E = bar.Material.GetMaterialPropertiesAt(0).Ex;
            var A = bar.Material.GetMaterialPropertiesAt(0).Ex;
            var L = bar.GetLength();
            var strain = istLoad.ImposedStrainMagnitude;

            //F = K . D, K = E.A/L, strain = D/L, F=strain*E*A

            var f = strain * E * A;

            var f1 = new Force();
            f1.Fx = -f;

            var f2 = new Force();
            f2.Fx = f;

            return new Force[] { f1, f2 };

            return new Force[2];//zero
        }

        public StrainTensor GetLocalLoadDisplacementAt(Element elm, IElementHelper hlpr, ElementalLoad load, IsoPoint loc)
        {
            return new StrainTensor();//zero
        }

        public CauchyStressTensor GetLocalLoadInternalForceAt(Element elm, IElementHelper hlpr, ElementalLoad load, IsoPoint loc)
        {
            var bar = elm as BarElement;
            var istLoad = load as ImposedStrainLoad;

            var E = bar.Material.GetMaterialPropertiesAt(0).Ex;
            var A = bar.Material.GetMaterialPropertiesAt(0).Ex;
            var L = bar.GetLength();
            var strain = istLoad.ImposedStrainMagnitude;

            //F = K . D, K = E.A/L, strain = D/L, F=strain*E*A

            var f = strain * E * A;

            throw new NotImplementedException();
            return new CauchyStressTensor();//zero
        }
    }
}
