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
    public class Uniform_UF_Handler : ILoadHandler
    {
        public bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            if (!(hlpr is BriefFiniteElementNet.ElementHelpers.TetrahedronHelper))
                return false;

            if (!(load is UniformLoad))
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
            var cl = load as UniformLoad;
            var thr = hlpr as ElementHelpers.TetrahedronHelper;

            double volume;

            {
                //volume of tetrahedron
                //https://math.stackexchange.com/a/3616774

                var mtx = new Matrix(4, 4);

                for (int i = 0; i < 4; i++)
                {
                    mtx[i, 0] = tetr.Nodes[i].Location.X;
                    mtx[i, 1] = tetr.Nodes[i].Location.Y;
                    mtx[i, 2] = tetr.Nodes[i].Location.Z;
                    mtx[i, 3] = 1;
                }

                volume = mtx.Determinant() / 6.0;
            }
            
            var buf = new Force[4];

            for (var i = 0; i < 4; i++)
            {
                var frc = volume / 4.0 * cl.Magnitude * cl.Direction;
                buf[i] = new Force(frc, Vector.Zero);
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
