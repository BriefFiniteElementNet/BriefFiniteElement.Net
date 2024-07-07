using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;
using BriefFiniteElementNet.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers.LoadHandlers.Truss2Node
{
    public class PartialNonuniform_UF_Handler : ILoadHandler
    {
        public bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            if (!(hlpr is BriefFiniteElementNet.ElementHelpers.BarHelpers.TrussHelper2Node))
                return false;

            if (!(load is PartialNonUniformLoad))
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

            var mat = bar.Material;
            var sec = bar.Section;

            var tr = bar.GetTransformationManager();


            var thiss = hlpr as TrussHelper2Node;

            {

                Func<double, double> magnitude;
                Vector localDir;

                double xi0, xi1;
                int degree;//polynomial degree of magnitude function

                #region inits
                
                //if (load is PartialNonUniformLoad)
                {
                    var uld = load as PartialNonUniformLoad;

                    magnitude = xi => uld.GetMagnitudeAt(bar, new IsoPoint(xi));
                    localDir = uld.Direction;

                    if (uld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    localDir = localDir.GetUnit();

                    xi0 = uld.StartLocation.Xi;
                    xi1 = uld.EndLocation.Xi;

                    degree = uld.SeverityFunction.Degree[0];// Coefficients.Length; 
                }

                localDir = localDir.GetUnit();
                #endregion

                {

                    var nOrd = thiss.GetNMaxOrder(bar).Max();

                    var gpt = (nOrd + degree) / 2 + 1;//gauss point count

                    var intg = GaussianIntegrator.CreateFor1DProblem(xi =>
                    {
                        var shp = thiss.GetNMatrixAt(bar, xi, 0, 0);
                        var q__ = magnitude(xi);
                        var j = thiss.GetJMatrixAt(bar, xi, 0, 0);
                        shp.Scale(j.Determinant());

                        var q_ = localDir * q__;

                        shp.Scale(q_.X);

                        return shp;
                    }, xi0, xi1, gpt);

                    var res = intg.Integrate();

                    var localForces = new Force[2];

                    var fx0 = res[0, 0];
                    var fx1 = res[0, 1];

                    localForces[0] = new Force(fx0, 0, 0, 0, 0, 0);
                    localForces[1] = new Force(fx1, 0, 0, 0, 0, 0);

                    return localForces;
                }
            }

        }

        public Displacement GetLocalLoadDisplacementAt(Element elm, IElementHelper hlpr, ElementalLoad load, IsoPoint loc)
        {
            throw new NotImplementedException();
        }

        public CauchyStressTensor GetLocalLoadInternalForceAt(Element elm, IElementHelper hlpr, ElementalLoad load, IsoPoint loc)
        {
            throw new NotImplementedException();
        }
    }
}
