using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
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
        public void TestEulerBernouly_diry()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var w = 2.0;
            var forceLocation = 0.5;//[m]
            var L = 4;//[m]

            //var model = new Model();

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.ConcentratedLoad();

            u1.Case = LoadCase.DefaultLoadCase;
            u1.Force = new Force(0, -w, 0, 0, 0, 0);
            u1.CoordinationSystem = CoordinationSystem.Global;

            u1.ForceIsoLocation = new IsoPoint(elm.LocalCoordsToIsoCoords(forceLocation)[0]);

            var hlpr = new EulerBernoulliBeamHelper2Node(BeamDirection.Z, elm);

            var length = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;


            foreach (var x in CalcUtil.Divide(length, 10))
            {
                var xi = elm.LocalCoordsToIsoCoords(x);


                //https://www.engineeringtoolbox.com/beams-fixed-both-ends-support-loads-deflection-d_809.html

                var mi = 0.0;
                var vi = 0.0;

                {
                    var a = forceLocation;
                    var b = L - a;

                    var ma = -w * a * b * b / (L * L);
                    var mb = -w * a * a * b / (L * L);
                    var mf = 2 * w * a * a * b * b / (L * L * L);

                    double x0, x1, y0, y1;

                    if (x < forceLocation)
                    {
                        x0 = 0;
                        x1 = forceLocation;

                        y0 = ma;
                        y1 = mf;
                    }
                    else
                    {
                        x0 = forceLocation;
                        x1 = L;

                        y0 = mf;
                        y1 = mb;
                    }


                    var m = (y1 - y0) / (x1 - x0);

                    mi = m * (x - x0) + y0;

                    var ra = w * (3 * a + b) * b * b / (L * L * L);//1f
                    var rb = w * (a + 3 * b) * a * a / (L * L * L);//1g

                    if (x < forceLocation)
                        vi = ra;
                    else
                        vi = -rb;
                }

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
