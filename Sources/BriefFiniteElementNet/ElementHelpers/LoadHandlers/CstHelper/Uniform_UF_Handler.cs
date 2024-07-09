using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers.LoadHandlers.CstHelper
{
    public class Uniform_UF_Handler : ILoadHandler
    {
        public bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            if (!(hlpr is BriefFiniteElementNet.ElementHelpers.CstHelper))
                return false;

            if (!(load is UniformLoad))
                return false;

            if (!(elm is TriangleElement))
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
            var ul = load as UniformLoad;

            var u = ul.Direction;// new Vector();

            if (ul.CoordinationSystem == CoordinationSystem.Global)
            {
                var trans = elm.GetTransformationManager();
                u = trans.TransformGlobalToLocal(u); //local to global
            }

            var nodes = elm.Nodes;

            var area = CalcUtil.GetTriangleArea(nodes[0].Location, nodes[1].Location, nodes[2].Location);

            var f = u * (area / 3.0);

            f.Z = 0;//force component in z directions
            
            var frc = new Force(f, Vector.Zero);

            return new[] { frc, frc, frc };
        }

        public Displacement GetLocalLoadDisplacementAt(Element elm, IElementHelper hlpr, ElementalLoad load, IsoPoint loc)
        {
            throw new NotImplementedException();
        }

        public object GetLocalLoadInternalForceAt(Element elm, IElementHelper hlpr, ElementalLoad load, IsoPoint loc)
        {
            throw new NotImplementedException();
        }
    }
}
