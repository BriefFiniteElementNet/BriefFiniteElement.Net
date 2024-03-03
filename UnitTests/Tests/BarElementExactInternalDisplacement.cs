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
        public void TestEulerBernouly_Distributed_dirz()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var dir = BeamDirection.Z;
            var w = 2.0;
            var L = 4;//[m]
            var I = 1;
            var E = 1;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0));
            nodes[1] = (new Node(L, 0, 0));

            var elm = new BarElement(nodes[0], nodes[1]);
            elm.Section = new UniformParametric1DSection(0, 0, I);
            elm.Material = UniformIsotropicMaterial.CreateFromYoungPoisson(E, 0.25);

            var u1 = new Loads.UniformLoad();

            u1.Case = LoadCase.DefaultLoadCase;
            u1.Direction = Vector.J;
            u1.CoordinationSystem = CoordinationSystem.Local;
            u1.Magnitude = w;

            //u1.ForceIsoLocation = new IsoPoint(elm.LocalCoordsToIsoCoords(forceLocation)[0]);

            var hlpr = new EulerBernoulliBeamHelper2Node(dir, elm);

            var epsilon = 1e-6;


            foreach (var x in CalcUtil.Divide(L, 10))
            {
                var xi = elm.LocalCoordsToIsoCoords(x);

                var current = hlpr.GetLoadDisplacementAt(elm, u1, xi).DY;

                //https://mechanicalc.com/reference/beam-deflection-tables
                var expected = (w * x * x) / (24 * E * I) * (L - x) * (L - x);

                Assert.IsTrue(Math.Abs(current - expected) < epsilon, "invalid value");

                if (x != 0 && x != L)
                {
                    Assert.IsTrue(Math.Sign(current) == Math.Sign(w), "invalid sign");
                }
            }

        }


        [Test]
        [Description("Euler Bernoully - Uniform Load - Uniform Section")]
        [Category("BarElement")]
        [TestOf(typeof(EulerBernoulliBeamHelper2Node))]

        public void TestEulerBernouly_Distributed_diry()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var dir = BeamDirection.Y;
            var w = 2.0;
            var L = 4;//[m]
            var I = 1;
            var E = 1;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0));
            nodes[1] = (new Node(L, 0, 0));

            var elm = new BarElement(nodes[0], nodes[1]);
            elm.Section = new UniformParametric1DSection(0, I, 0);
            elm.Material = UniformIsotropicMaterial.CreateFromYoungPoisson(E, 0.25);

            var u1 = new Loads.UniformLoad();

            u1.Case = LoadCase.DefaultLoadCase;
            u1.Direction = Vector.K;
            u1.CoordinationSystem = CoordinationSystem.Local;
            u1.Magnitude = w;

            //u1.ForceIsoLocation = new IsoPoint(elm.LocalCoordsToIsoCoords(forceLocation)[0]);

            var hlpr = new EulerBernoulliBeamHelper2Node(dir, elm);

            var epsilon = 1e-6;


            foreach (var x in CalcUtil.Divide(L, 10))
            {
                var xi = elm.LocalCoordsToIsoCoords(x);

                var current = hlpr.GetLoadDisplacementAt(elm, u1, xi).DZ;

                //https://mechanicalc.com/reference/beam-deflection-tables
                var expected = (w * x * x) / (24 * E * I) * (L - x) * (L - x);

                Assert.IsTrue(Math.Abs(current - expected) < epsilon, "invalid value");

                if (x != 0 && x != L)
                {
                    Assert.IsTrue(Math.Sign(current) == Math.Sign(w), "invalid sign");
                }
            }

        }
    }
}
