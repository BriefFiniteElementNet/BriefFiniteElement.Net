using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using CSparse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers.LoadHandlers.TetrahedronHelper
{
    public class Concentrated_UF_Handler : ILoadHandler
    {
        public bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            if (!(hlpr is BriefFiniteElementNet.ElementHelpers.BarHelpers.ShaftHelper2Node))
                return false;

            if (!(load is ConcentratedLoad))
                return false;

            if (!(elm is TetrahedronElement))
                return false;

            var tt = elm as TetrahedronElement;

            var mat = tt.Material;

            if (mat.GetMaxFunctionOrder()[0] != 0)//constant uniform material through length
                return false;

            return true;
        }

        public Force[] GetLocalEquivalentNodalLoads(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            var tetr = elm as TetrahedronElement;
            var cl = load as ConcentratedLoad;
            var thr = hlpr as ElementHelpers.TetrahedronHelper;

            var f = cl.Force;

            f.Moments = Vector.Zero;//tetrahedron not transfer moment

            var ns = thr.GetNMatrixAt(tetr, cl.ForceIsoLocation.ToArray());

            var buf = new Force[4];

            for (var i = 0; i < 4; i++)
            {
                buf[i] = ns[i,0] * f;
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
