﻿using BriefFiniteElementNet.ElementHelpers.BarHelpers;
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
    [TestFixture, Description("Bar Element - Uniform Section - exact internal displacement")]
    public class BarElementExactInternalDisplacementUniformSection
    {
        [Test, Description("Euler Bernoully - Uniform Load - Uniform Section - dir Z")]
        [Category("BarElement")]
        [TestOf(typeof(EulerBernoulliBeamHelper2Node))]
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
        [Description("Euler Bernoully - Uniform Load - Uniform Section - dir Y")]
        [Category("BarElement")]
        [TestOf(typeof(EulerBernoulliBeamHelper2Node))]

        public void TestEulerBernouly_Distributed_diry()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var dir = BeamDirection.Y;
            var w = 2;
            var L = 3;//[m]
            var I = 5;
            var E = 7;


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

                var currentD = hlpr.GetLoadDisplacementAt(elm, u1, xi);

                var current = currentD.DZ;

                //https://mechanicalc.com/reference/beam-deflection-tables
                var expected = (w * x * x) / (24 * E * I) * (L - x) * (L - x);

                Assert.IsTrue(Math.Abs(current - expected) < epsilon, "invalid value");

                if (x != 0 && x != L)
                {
                    Assert.IsTrue(Math.Sign(current) == Math.Sign(w), "invalid sign");
                }
            }

        }

        [Test, Description("Truss - Uniform Load")]
        [Category("BarElement")]
        [TestOf(typeof(EulerBernoulliBeamHelper2Node))]
        public void TestTruss_Distributed()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed
            var w = 2.0;
            var L = 4;//[m]
            var A = 1;
            var E = 1;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0));
            nodes[1] = (new Node(L, 0, 0));

            var elm = new BarElement(nodes[0], nodes[1]);
            elm.Section = new UniformParametric1DSection(A, 0, 0);
            elm.Material = UniformIsotropicMaterial.CreateFromYoungPoisson(E, 0.25);

            var u1 = new Loads.UniformLoad();

            u1.Case = LoadCase.DefaultLoadCase;
            u1.Direction = Vector.I;
            u1.CoordinationSystem = CoordinationSystem.Local;
            u1.Magnitude = w;

            //u1.ForceIsoLocation = new IsoPoint(elm.LocalCoordsToIsoCoords(forceLocation)[0]);

            var hlpr = new TrussHelper2Node(elm);

            var epsilon = 1e-6;


            foreach (var x in CalcUtil.Divide(L, 10))
            {
                var xi = elm.LocalCoordsToIsoCoords(x);

                var current = hlpr.GetLoadDisplacementAt(elm, u1, xi).DX;

                var expected = w * x / (2 * E * A) * (L - x);

                Assert.IsTrue(Math.Abs(current - expected) < epsilon, "invalid value");

                if (x != 0 && x != L)
                {
                    Assert.IsTrue(Math.Sign(current) == Math.Sign(w), "invalid sign");
                }
            }
        }

        [Test, Description("Truss - concentrate Load")]
        [Category("BarElement")]
        [TestOf(typeof(TrussHelper2Node))]
        public void TestTruss_Concentrated()
        {
            

        }


        [Test]
        [Description("Shaft - Uniform Load - Uniform Section")]
        [Category("BarElement")]
        [TestOf(typeof(EulerBernoulliBeamHelper2Node))]
        public void TestShaft_Concentrated()
        {
            var T = 2.0;
            var L = 4;//[m]
            var I = 1;
            var J = 1;
            var E = 1;
            var G = 1;
            var xt = 2;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0));
            nodes[1] = (new Node(L, 0, 0));

            var elm = new BarElement(nodes[0], nodes[1]);
            elm.Section = new UniformParametric1DSection(0, 0, 0, J);
            elm.Material = UniformIsotropicMaterial.CreateFromShearPoisson(G, 0.25);

            var u1 = new Loads.ConcentratedLoad();

            u1.Case = LoadCase.DefaultLoadCase;
            u1.Force = new Force(0, 0, 0, T, 0, 0);
            u1.CoordinationSystem = CoordinationSystem.Local;
            u1.ForceIsoLocation = new IsoPoint(elm.LocalCoordsToIsoCoords(xt));

            //u1.ForceIsoLocation = new IsoPoint(elm.LocalCoordsToIsoCoords(forceLocation)[0]);

            var hlpr = new ShaftHelper2Node(elm);

            var epsilon = 1e-6;

            double t0, t1;

            {
                var ends = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

                t0 = -ends[0].Mx;
                t1 = -ends[1].Mx;
            }

            foreach (var x in CalcUtil.Divide(L, 10))
            {
                var xi = elm.LocalCoordsToIsoCoords(x);

                var current = hlpr.GetLoadDisplacementAt(elm, u1, xi).RX;
                
                var expected = double.NaN;//TODO: fix the formula

                if (x <= xt)
                    expected = double.NaN;//todo
                else
                    expected = double.NaN;//todo

                if (x <= xt)
                    expected = -t0 * x / (G * J);
                else
                    expected = -t0 * xt / (G * J) + (t0 + T) * (x - xt) / (G * J);

                Assert.IsTrue(Math.Abs(current - expected) < epsilon, "invalid value");

                if (x != 0 && x != L)
                {
                    Assert.IsTrue(Math.Sign(current) == Math.Sign(T), "invalid sign");
                }
            }
        }
    }
}
