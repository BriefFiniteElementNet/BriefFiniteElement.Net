using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers.LoadHandlers.CstHelper
{
    public class Concentrated_UF_Handler : ILoadHandler
    {
        public bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            if (!(hlpr is BriefFiniteElementNet.ElementHelpers.CstHelper))
                return false;

            if (!(load is ConcentratedLoad))
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
            var ul = load as ConcentratedLoad;

            var u = ul.Force;// new Vector();


            if (ul.CoordinationSystem == CoordinationSystem.Global)
            {
                var trans = elm.GetTransformationManager();
                u = trans.TransformGlobalToLocal(u); //local to global
            }

            if (u.Mz != 0.0)
                throw new Exception("Drilling dof moment not implemented for concentrated load");//drilling DoF will cause Fx and Fy on nodes which need to be implemented

            var shp = hlpr.GetNMatrixAt(elm, ul.ForceIsoLocation.ToArray());

            var buf = new Force[] { u, u, u };



            for (int i = 0; i < 3; i++)
            {
                var frc = buf[i];

                frc = shp[i, 0] * frc;

                frc.Moments = Vector.Zero;//cst not transfer moments

                frc.Fz = 0;//cst not transfer local Fz 

                buf[i] = frc;
            }

            return buf;
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
