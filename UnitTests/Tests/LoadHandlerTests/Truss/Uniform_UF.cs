using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Tests.LoadHandlerTests.Truss
{
    public class Uniform_UF
    {
        [Test]
        public void EqNodalLoads()
        {
            var w = 2.0;
            var L = 4;//[m]

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(L, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.UniformLoad();

            u1.Case = LoadCase.DefaultLoadCase;
            u1.CoordinationSystem = CoordinationSystem.Global;
            u1.Magnitude = w;
            u1.Direction = Vector.I;

            elm.Loads.Add(u1);

            var hlpr = new TrussHelper2Node(elm);

            var handler = new ElementHelpers.LoadHandlers.Truss2Node.Uniform_UF_Handler();

            var end1 = handler.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

            var f0 = new Force(w * L / 2, 0, 0, 0, 0, 0);
            var f1 = new Force(w * L / 2, 0, 0, 0, 0, 0);


            var d1 = f0 - end1[0];
            var d2 = f1 - end1[1];


            Assert.IsTrue(Math.Abs(d1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d1.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(d2.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d2.Moments.Length) < 1e-5, "invalid value");
        }


        [Test]
        public void InternalForce()
        {
            var w = 2.0;
            var L = 4;//[m]

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(L, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.UniformLoad();

            u1.Case = LoadCase.DefaultLoadCase;
            u1.CoordinationSystem = CoordinationSystem.Global;
            u1.Magnitude = w;
            u1.Direction = Vector.I;

            elm.Loads.Add(u1);

            var hlpr = new TrussHelper2Node(elm);

            var handler = new ElementHelpers.LoadHandlers.Truss2Node.Uniform_UF_Handler();

            var end1 = handler.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

            var f0 = new Force(w * L / 2, 0, 0, 0, 0, 0);
            var f1 = new Force(w * L / 2, 0, 0, 0, 0, 0);


            var d1 = f0 - end1[0];
            var d2 = f1 - end1[1];


            Assert.IsTrue(Math.Abs(d1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d1.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(d2.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d2.Moments.Length) < 1e-5, "invalid value");
        }



        [Test]
        public void LoadInternalForce_concentratedLLoad_truss_Fx()
        {
            //internal force of 2 node truss with concentrated load and both ends fixed

            var w = 2.0;
            var forceLocation = 0.5;//[m]
            var L = 4;//[m]

            //var model = new Model();

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };


            var mat = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.25);
            var sec = new Sections.UniformParametric1DSection(0.01);

            elm.Material = mat;
            elm.Section = sec;


            var u1 = new Loads.ConcentratedLoad();

            u1.Case = LoadCase.DefaultLoadCase;
            u1.Force = new Force(w, 0, 0, 0, 0, 0);
            u1.CoordinationSystem = CoordinationSystem.Global;

            u1.ForceIsoLocation = new IsoPoint(elm.LocalCoordsToIsoCoords(forceLocation)[0]);

            var hlpr = new TrussHelper2Node(elm);

            var length = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;


            foreach (var x in CalcUtil.Divide(length, 10))
            {
                var xi = elm.LocalCoordsToIsoCoords(x);

                var mi = 0.0;
                var vi = 0.0;

                {
                    //https://www.amesweb.info/Beam/Fixed-Fixed-Beam-Bending-Moment.aspx

                    var a = forceLocation;
                    var b = L - a;

                    var ra = (1 - (a / L)) * w;

                    var rb = (1 - (b / L)) * w;

                    mi = ra + ((x > forceLocation) ? -w : 0.0);
                }


                var ends = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

                var testFrc = hlpr.GetLoadInternalForceAt(elm, u1, new double[] { xi[0] * (1 - 1e-9) });

                var exactFrc = new Force(fx: mi, fy: 0, fz: vi, mx: 0, my: 0, mz: 0);

                var df = testFrc.FirstOrDefault(i => i.Item1 == DoF.Dx).Item2 - exactFrc.Fx;


                Assert.IsTrue(Math.Abs(df) < 1e-5, "invalid value");
            }

            {
                var end1 = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

                var f0 = hlpr.GetLoadInternalForceAt(elm, u1, new double[] { -1 + 1e-9 }).ToForce(); ;

                var sum = end1[0] - f0;

                Assert.IsTrue(Math.Abs(sum.Forces.Length) < 1e-5, "invalid value");
                Assert.IsTrue(Math.Abs(sum.Moments.Length) < 1e-5, "invalid value");


            }
        }
    }
}
