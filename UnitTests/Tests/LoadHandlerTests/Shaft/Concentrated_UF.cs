using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using NUnit.Framework;


namespace BriefFiniteElementNet.Tests.LoadHandlerTests.Shaft
{

    internal class Concentrated_UF
    {
        [Test]
        public void EqNodalForce()
        {
            var w = 2.0;
            var forceLocation = 0.5;//[m]
            var L = 4;//[m]

            //var model = new Model();

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(L, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.ConcentratedLoad();

            u1.Case = LoadCase.DefaultLoadCase;
            u1.Force = new Force(0, 0, 0, w, 0, 0);
            u1.CoordinationSystem = CoordinationSystem.Global;

            var xi = elm.LocalCoordsToIsoCoords(forceLocation);

            u1.ForceIsoLocation = new IsoPoint(xi);

            var hlpr = new TrussHelper2Node(elm);


            var handler = new ElementHelpers.LoadHandlers.ShaftHelper.Concentrated_UF_Handler();

            var end1 = handler.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

            var expected0 = w * (L - forceLocation )/ L;
            var expected1 = w * ((forceLocation) / L);


            var f0 = new Force(0, 0, 0, expected0, 0, 0);
            var f1 = new Force(0, 0, 0, expected1, 0, 0);


            var err0 = f0 - end1[0];
            var err1 = f1 - end1[1];

            Assert.IsTrue(Math.Abs(err0.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(err0.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(err1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(err1.Moments.Length) < 1e-5, "invalid value");
        }

        [Test]
        public void InternalForce()
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

            var u1 = new Loads.ConcentratedLoad();

            u1.Case = LoadCase.DefaultLoadCase;
            u1.Force = new Force(0, 0, 0, w, 0, 0);
            u1.CoordinationSystem = CoordinationSystem.Global;

            u1.ForceIsoLocation = new IsoPoint(elm.LocalCoordsToIsoCoords(forceLocation)[0]);

            var hlpr = new ShaftHelper2Node(elm);


            var handler = new ElementHelpers.LoadHandlers.ShaftHelper.Concentrated_UF_Handler();


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

                var testTensor = handler.GetLocalLoadInternalForceAt(elm, hlpr, u1, new IsoPoint(xi[0] * (1 - 1e-9)));
                var exactFx = mi;

                var testFx = testTensor.S11;

                var err = testFx - exactFx;

                Assert.IsTrue(Math.Abs(err) < 1e-5, "invalid value");
            }

            /*{
                
            }*/
        }

        [Test]
        public void InternalDisplacement()
        {
            double ft = 2.0;
            double L = 4;//[m]
            double A = 1;
            double E = 1;
            var G = 1;
            double xt = 2;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0));
            nodes[1] = (new Node(L, 0, 0));

            var elm = new BarElement(nodes[0], nodes[1]);
            elm.Section = new UniformParametric1DSection(A, 0, 0);
            elm.Material = UniformIsotropicMaterial.CreateFromYoungPoisson(E, 0.25);

            var u1 = new Loads.ConcentratedLoad();

            u1.Case = LoadCase.DefaultLoadCase;
            u1.Force = new Force(0, 0, 0, ft, 0, 0);
            u1.CoordinationSystem = CoordinationSystem.Local;
            u1.ForceIsoLocation = new IsoPoint(elm.LocalCoordsToIsoCoords(xt));

            //u1.ForceIsoLocation = new IsoPoint(elm.LocalCoordsToIsoCoords(forceLocation)[0]);

            var hlpr = new TrussHelper2Node(elm);

            var handler = new ElementHelpers.LoadHandlers.ShaftHelper.Concentrated_UF_Handler();

            var epsilon = 1e-6;


            var f0 = -xt / L * ft;


            foreach (var x in CalcUtil.Divide(L, 10))
            {
                var xi = elm.LocalCoordsToIsoCoords(x);

                var currentTensor = handler.GetLocalLoadDisplacementAt(elm, hlpr, u1, new IsoPoint(xi));
                var currentDx = currentTensor.RX;

                double expected;

                if (x <= xt)
                    expected = -f0 * x / (E * A);
                else
                    expected = -f0 * xt / (E * A) + (f0 + ft) * (x - xt) / (E * A);

                Assert.IsTrue(Math.Abs(currentDx - expected) < epsilon, "invalid value");

                if (x != 0 && x != L)
                {
                    Assert.IsTrue(Math.Sign(currentDx) == Math.Sign(ft), "invalid sign");
                }

            }
        }
    }
}
