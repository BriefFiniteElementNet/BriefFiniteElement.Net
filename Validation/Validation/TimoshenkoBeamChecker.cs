using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.ElementHelpers.Bar;
using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Mathh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation
{
    public static class TimoshenkoBeamChecker
    {
        public static void Test1()
        {
            //if phi = 0 then shapes are equal to euler bernouly

            //var dir = BeamDirection.Y;

            foreach (BeamDirection dir in Enum.GetValues(typeof(BeamDirection)))
            {
                var l = 2.15484984;

                var node1 = new Node(0, 0, 0);
                var node2 = new Node(l, 0, 0);

                var elm = new BarElement(node1, node2);

                var h1 = new EulerBernoulliBeamHelper2Node(dir, elm);
                var h2 = new TimoshenkoBeamHelper(dir, elm);

                var phi = 0.0;

                SingleVariablePolynomial[] nt, mt, ne, me;//brenouly and timoshenko

                TimoshenkoBeamHelper.GetYShapeFunctions(l, phi, dir, out nt, out mt);
                EulerBernouly2NodeShapeFunction.GetShapeFunctions(l, DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Fixed, dir, out ne, out me);

                var ep = 1e-6;

                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (SingleVariablePolynomial.GetMaxDiff(ne[i], nt[i]) > ep)
                            throw new Exception();

                        if (SingleVariablePolynomial.GetMaxDiff(me[i], mt[i]) > ep)
                            throw new Exception();
                    }
                }
            }

            Guid.NewGuid();
        }

        public static void Test2()
        {
            //if phi = 0 then theta = y'
            foreach (BeamDirection dir in Enum.GetValues(typeof(BeamDirection)))
            {

                var l = 2.156519849840;

                var node1 = new Node(0, 0, 0);
                var node2 = new Node(l, 0, 0);

                var elm = new BarElement(node1, node2);

                var phi = 0.0;

                SingleVariablePolynomial[] nt, mt, npt, mpt;//brenouly and timoshenko

                TimoshenkoBeamHelper.GetYShapeFunctions(l, phi, dir, out nt, out mt);
                TimoshenkoBeamHelper.GetThetaShapeFunctions(l, phi, dir, out npt, out mpt);


                var ep = 1e-6;


                var j = BaseBar2NodeHelper.GetJ(elm);

                {
                    for (int i = 0; i < 2; i++)
                    {
                        {
                            var a = nt[i].GetDerivative(1);
                            var b = npt[i];

                            //b.MultiplyByConstant(j);

                            if (SingleVariablePolynomial.GetMaxDiff(a, b) > ep)
                                throw new Exception();
                        }

                        {
                            var a = mt[i].GetDerivative(1);
                            var b = mpt[i];

                            //b.MultiplyByConstant(j);

                            if (SingleVariablePolynomial.GetMaxDiff(a, b) > ep)
                                throw new Exception();
                        }

                    }
                }

            }


            Guid.NewGuid();
        }
    }
}
