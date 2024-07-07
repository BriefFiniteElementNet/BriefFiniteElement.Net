using MathNet.Numerics.RootFinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using BriefFiniteElementNet.Mathh;
using BriefFiniteElementNet.ElementHelpers.BarHelpers;

namespace BriefFiniteElementNet.ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node
{
    public class Uniform_NUF_Handler : ILoadHandler
    {
        public bool CanHandle(Element elm, IElementHelper hlpr, ElementalLoad load)
        {
            if (!(hlpr is BriefFiniteElementNet.ElementHelpers.BarHelpers.EulerBernoulliBeamHelper2Node))
                return false;

            if (!(load is Loads.UniformLoad))
                return false;

            if (!(elm is BarElement))
                return false;

            var bar = elm as BarElement;

            var mat = bar.Material;
            var sec = bar.Section;

            if (mat.GetMaxFunctionOrder()[0] == 0)//non constant uniform material through length
                return false;

            if (sec.GetMaxFunctionOrder()[0] == 0)//non constant uniform section through length
                return false;

            return true;
        }

        public Force[] GetLocalEquivalentNodalLoads(Element elm, IElementHelper hlpr, ElementalLoad ld)
        {
            //where load is uniform and section or material is non uniform

            var bar = elm as BarElement;
            var load = ld as UniformLoad;
            var thiss = hlpr as BarHelpers.EulerBernoulliBeamHelper2Node;

            Polynomial1D oneOverEi, xOverEi, x2OverEi;

            int c;
            double L;
            double w0;

            {
                var n = bar.Material.GetMaxFunctionOrder()[0];
                var m = bar.Section.GetMaxFunctionOrder()[0];
                var p = 2;
                c = 2 * n + 2 * m + p;
                //c += 3;
                L = bar.GetLength();
            }

            {
                var localDir = load.Direction;

                var tr = bar.GetTransformationManager();

                if (load.CoordinationSystem == CoordinationSystem.Global)
                    localDir = tr.TransformGlobalToLocal(localDir);

                localDir = localDir.GetUnit();

                switch (thiss.Direction)
                {
                    case BeamDirection.Y:
                        w0 = localDir.Z * load.Magnitude;
                        break;

                    case BeamDirection.Z:
                        w0 = localDir.Y * load.Magnitude;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            double[] xs;

            {
                xs = CalcUtil.DivideSpan(0, L, c);

                var y1s = new double[xs.Length];
                var y2s = new double[xs.Length];
                var y3s = new double[xs.Length];

                for (var i = 0; i < xs.Length; i++)
                {
                    var x = xs[i];
                    var xi = bar.LocalCoordsToIsoCoords(x)[0];

                    var mech = bar.Material.GetMaterialPropertiesAt(new IsoPoint(xi), bar);
                    var geo = bar.Section.GetCrossSectionPropertiesAt(xi, bar);

                    var I = thiss.Direction == BeamDirection.Y ? geo.Iy : geo.Iz;

                    var ei = mech.Ex * I;

                    y1s[i] = 1 / ei;
                    y2s[i] = x / ei;
                    y3s[i] = x * x / ei;
                }

                oneOverEi = Polynomial1D.FromPoints(xs, y1s);
                xOverEi = Polynomial1D.FromPoints(xs, y2s);
                x2OverEi = Polynomial1D.FromPoints(xs, y3s);

            }

            double y11, y22, y33;
            double y1, y2, y3;

            {
                y1 = oneOverEi.EvaluateNthIntegral(1, 0)[0] - oneOverEi.EvaluateNthIntegral(1, L)[0];
                y2 = xOverEi.EvaluateNthIntegral(1, 0)[0] - xOverEi.EvaluateNthIntegral(1, L)[0];
                y3 = x2OverEi.EvaluateNthIntegral(1, 0)[0] - x2OverEi.EvaluateNthIntegral(1, L)[0];

                y11 = oneOverEi.EvaluateNthIntegral(2, 0)[0] - oneOverEi.EvaluateNthIntegral(2, L)[0];
                y22 = xOverEi.EvaluateNthIntegral(2, 0)[0] - xOverEi.EvaluateNthIntegral(2, L)[0];
                y33 = x2OverEi.EvaluateNthIntegral(2, 0)[0] - x2OverEi.EvaluateNthIntegral(2, L)[0];
            }

            {
                if (bar.StartReleaseCondition != Constraints.Fixed || bar.EndReleaseCondition != Constraints.Fixed)
                    throw new Exception("Not supported end releases");
            }

            double f0, m0;

            {
                CalcUtil.Solve2x2(y1, y2, -w0 / 2 * y3,
                    y11, y22, -w0 / 2 * y33, out m0, out f0);
            }

            double f1, m1;

            {//for uniform load
                f1 = -(f0 + w0 * L);
                m1 = -(m0 + f0 * L + w0 * L * L / 2);
            }

            var p0 = new Force();
            var p1 = new Force();

            if (thiss.Direction == BeamDirection.Y)
            {
                p0.Fz = f0;
                p1.Fz = f1;

                p0.My = m0;//possible negative
                p1.My = m1;//possible negative
            }
            else
            {
                p0.Fy = f0;
                p1.Fy = f1;

                p0.Mz = -m0;//possible negative
                p1.Mz = -m1;//possible negative
            }
            /** /
            {//scale the output to reduce error

                var pt = (f1+f0) / (w0 * L );

                p0 = 1 / pt * p0 ;
                p1 = 1 / pt * p1 ;
            }
            /**/
            return new Force[] { -p0, -p1 };
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

