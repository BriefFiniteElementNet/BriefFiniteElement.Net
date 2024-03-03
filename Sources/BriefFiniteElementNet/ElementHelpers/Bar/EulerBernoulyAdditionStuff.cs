using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;
using BriefFiniteElementNet.Loads;
using BriefFiniteElementNet.Mathh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers.BarHelpers
{

    //utility for getting internal force  and eq nodal load for non uniform section and material and/or nonuniform (like partial or concentrated) loads
    public class EulerBernoulliBeamHelper2NodeInternalXUtil
    {

        public EulerBernoulliBeamHelper2NodeInternalXUtil(BarElement bar, ElementalLoad load, BeamDirection dir)
        {

        }

        BarElement TargetElement;
        ElementalLoad TargetLoad;
        BeamDirection TargetDir;


        Polynomial1D oneOverEi, xOverEi, x2OverEi, wOverEi;

        double x1, x2, L;

        Polynomial1D W1;
        double F1, M1;

        public Force[] GetEqNodalLoads()
        {
            throw new NotImplementedException();
        }


        
        public void Init()
        {
            L = TargetElement.GetLength();

            Vector localDir;

            {//local dir, w1, f1, m1
                if (TargetLoad is PartialNonUniformLoad pnl)
                {
                    x1 = TargetElement.IsoCoordsToLocalCoords(pnl.StartLocation.Xi)[0];
                    x2 = TargetElement.IsoCoordsToLocalCoords(pnl.StartLocation.Xi)[0];
                    W1 = Polynomial1D.FromIPolynomial(pnl.SeverityFunction);

                    {
                        localDir = pnl.Direction;

                        var tr = TargetElement.GetTransformationManager();

                        if (pnl.CoordinationSystem == CoordinationSystem.Global)
                            localDir = tr.TransformGlobalToLocal(localDir);

                        localDir = localDir.GetUnit();
                    }

                }
                else if (TargetLoad is UniformLoad ul)
                {
                    x1 = 0;
                    x2 = L;
                    W1 = Polynomial1D.FromPoints(0, ul.Magnitude);//constant

                    {
                        localDir = ul.Direction;

                        var tr = TargetElement.GetTransformationManager();

                        if (ul.CoordinationSystem == CoordinationSystem.Global)
                            localDir = tr.TransformGlobalToLocal(localDir);

                        localDir = localDir.GetUnit();
                    }

                }
                else if (TargetLoad is ConcentratedLoad cl)
                {
                    x1 = 0;
                    x2 = L;

                    var F = cl.Force;

                    x1 = TargetElement.IsoCoordsToLocalCoords(cl.ForceIsoLocation.Xi)[0];

                    {
                        var tr = TargetElement.GetTransformationManager();

                        if (cl.CoordinationSystem == CoordinationSystem.Global)
                            F = tr.TransformGlobalToLocal(F);
                    }

                    {
                        if (this.TargetDir == BeamDirection.Y)
                        {
                            F1 = F.Fz;
                            M1 = F.My;
                        }
                        else
                        {
                            F1 = F.Fy;
                            M1 = F.Mz;
                        }
                    }

                }
                else
                {
                    throw new NotImplementedException();
                }
            }
           
            var mat = TargetElement.Material;
            var sec = TargetElement.Section;

            int c1,c;

            {
                c1 = mat.GetMaxFunctionOrder()[0] + sec.GetMaxFunctionOrder()[0];
                c = 2 * c1 + W1.Degree[0];
            }


            {
                var ei = new Func<double, double>(x =>
                {
                    var xi = TargetElement.LocalCoordsToIsoCoords(x)[0];
                    var e = mat.GetMaterialPropertiesAt(new IsoPoint(xi), TargetElement).Ex;

                    var ss = sec.GetCrossSectionPropertiesAt(xi, TargetElement);

                    var i = this.TargetDir == BeamDirection.Y ? ss.Iy : ss.Iz;

                    return e * i;
                });

                var ksis = CalcUtil.DivideSpan(-1, 1, c);
                var xs = ksis.Select(xi => TargetElement.IsoCoordsToLocalCoords(xi)[0]).ToArray();

                if (W1 != null)
                {
                    var intg = new StepFunctionIntegralCalculator();
                    intg.Polynomial = W1;

                    var ys = xs.Select(x =>
                    {
                        var w = intg.CalculateIntegralAt(x1, x, 2) - intg.CalculateIntegralAt(x2, x, 2);
                        var ei_ = ei(x);
                        return w / ei_;
                    }).ToArray();

                    wOverEi = Polynomial1D.FromPoints(xs, ys);
                }
                else
                {
                    wOverEi = Polynomial1D.Zero;
                }

                var oneOverEis = xs.Select(x => 1 / ei(x)).ToArray();
                var xOverEis = xs.Select(x => x / ei(x)).ToArray();
                var x2OverEis = xs.Select(x => x * x / ei(x)).ToArray();

                if (c1 == 0)//EI is constant
                {
                    oneOverEi = Polynomial1D.FromPoints(xs.Take(1).ToArray(), xOverEis.Take(1).ToArray());//constant need one sampling point
                    xOverEi = Polynomial1D.FromPoints(xs.Take(2).ToArray(), xOverEis.Take(2).ToArray());//line need two points
                    x2OverEi = Polynomial1D.FromPoints(xs.Take(3).ToArray(), x2OverEis.Take(3).ToArray());// parabolic need three points
                }
                else
                {
                    oneOverEi = Polynomial1D.FromPoints(oneOverEis, xs);
                    xOverEi = Polynomial1D.FromPoints(xOverEis, xs);
                    x2OverEi = Polynomial1D.FromPoints(x2OverEis, xs);
                }

            }
            

        }

    }
}
