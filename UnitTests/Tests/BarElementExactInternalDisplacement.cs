using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Tests
{
    public class BarElementExactInternalDisplacement
    {
        [Test]
        public void TestEulerBernouly_Distributed_diry()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var w = 2.0;
            var L = 4;//[m]
            var I = 1e-4;
            var E = 210e9;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0));
            nodes[1] = (new Node(L, 0, 0));

            var elm = new BarElement(nodes[0], nodes[1]);
            elm.Section = new UniformParametric1DSection(I, I, I);
            elm.Material = UniformIsotropicMaterial.CreateFromYoungPoisson(E, 0.25);

            var u1 = new Loads.UniformLoad();

            u1.Case = LoadCase.DefaultLoadCase;
            u1.Direction = Vector.J;
            u1.CoordinationSystem = CoordinationSystem.Global;
            u1.Magnitude = w;

            //u1.ForceIsoLocation = new IsoPoint(elm.LocalCoordsToIsoCoords(forceLocation)[0]);

            var hlpr = new EulerBernoulliBeamHelper2Node(BeamDirection.Z, elm);


            foreach (var x in CalcUtil.Divide(L, 10))
            {
                var xi = elm.LocalCoordsToIsoCoords(x);

                var current = hlpr.GetLoadDisplacementAt(elm, u1, xi);

                var expected = (w * x * x) / (24 * E * I) * (L - x) * (L - x);

                var ratio = expected / current.DY;

                Guid.NewGuid();
                /**/

                /*
                var ends = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

                var testFrc = hlpr.GetLoadInternalForceAt(elm, u1, new double[] { xi[0] * (1 - 1e-9) }).ToForce();

                var exactFrc = new Force(fx: 0, fy: -vi, fz: 0, mx: 0, my: 0, mz: +mi);

                var d = exactFrc - testFrc;

                var dm = d.Mz;
                var df = d.Fy;

                Assert.IsTrue(Math.Abs(dm) < 1e-5, "invalid value");
                Assert.IsTrue(Math.Abs(df) < 1e-5, "invalid value");
                */

            }


            /*
            {
                var end1 = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

                var f0 = hlpr.GetLoadInternalForceAt(elm, u1, new double[] { -1 + 1e-9 }).ToForce(); ;

                var sum = end1[0] - f0;

                Assert.IsTrue(Math.Abs(sum.Forces.Length) < 1e-5, "invalid value");
                Assert.IsTrue(Math.Abs(sum.Moments.Length) < 1e-5, "invalid value");


            }

            */
        }
    }
}
