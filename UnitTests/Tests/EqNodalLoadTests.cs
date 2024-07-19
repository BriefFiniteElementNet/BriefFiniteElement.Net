using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Mathh;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EulerBernoulliBeamHelper = BriefFiniteElementNet.ElementHelpers.BarHelpers.EulerBernoulliBeamHelper2Node;


namespace BriefFiniteElementNet.Tests
{
    internal class EqNodalLoadTests_eulerbernoullybeam
    {

        [Test]
        public void LoadEquivalentNodalLoads_uniformload_eulerbernoullybeam_dirY()
        {
            //uniform load, section and material

            var w = 2.0;

            var nodes = new Node[2];

            nodes[0] = new Node(0, 0, 0) { Label = "n0" };
            nodes[1] = new Node(4, 0, 0) { Label = "n1" };

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, Vector.K, w, CoordinationSystem.Global);

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Y, elm);

            var hnd = new BriefFiniteElementNet.ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node.Uniform_UF_Handler();

            var loads = hnd.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

            var L = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            var m1 = -w * L * L / 12;
            var m2 = w * L * L / 12;

            var v1 = w * L / 2;
            var v2 = w * L / 2;

            Assert.IsTrue(Math.Abs(loads[0].Fz - v1) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(loads[0].My - m1) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(loads[1].Fz - v2) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(loads[1].My - m2) < 1e-5, "invalid value");
        }

        [Test]
        public void LoadEquivalentNodalLoads_uniformload_eulerbernoullybeam_dirZ()
        {
            //uniform load, section and material

            var w = 2.0;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, Vector.J, w, CoordinationSystem.Global);

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Z, elm);

            var hnd = new ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node.Uniform_UF_Handler();

            var loads = hnd.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

            var L = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            var m1 = w * L * L / 12;
            var m2 = -w * L * L / 12;

            var v1 = w * L / 2;
            var v2 = w * L / 2;

            Assert.IsTrue(Math.Abs(loads[0].Fy - v1) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(loads[0].Mz - m1) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(loads[1].Fy - v2) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(loads[1].Mz - m2) < 1e-5, "invalid value");
        }
        
        [Test]
        public void LoadEquivalentNodalLoads_uniformload_eulerbernoullybeam_dirY1()
        {
            //uniform load, section and material
            //using non uniform formulation

            var w = 2.0;

            var nodes = new Node[2];

            nodes[0] = new Node(0, 0, 0) { Label = "n0" };
            nodes[1] = new Node(4, 0, 0) { Label = "n1" };

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var mat = new UniformIsotropicMaterial(1, 0.25);
            var sec = new Sections.UniformParametric1DSection(1, 1, 1);

            elm.Section = sec;
            elm.Material = mat;


            var u1 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, Vector.K, w, CoordinationSystem.Global);

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Y, elm);

            var hnd = new ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node.Uniform_NUF_Handler();

            var loads = hnd.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

            //var loads = hlpr.GetLocalEquivalentNodalLoads_uniformLoad_nonUniformMatSection(elm, u1);

            var L = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            var m1 = -w * L * L / 12;
            var m2 = w * L * L / 12;

            var v1 = w * L / 2;
            var v2 = w * L / 2;

            Assert.IsTrue(Math.Abs(loads[0].Fz - v1) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(loads[0].My - m1) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(loads[1].Fz - v2) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(loads[1].My - m2) < 1e-5, "invalid value");
        }

        [Test]
        public void LoadEquivalentNodalLoads_uniformload_eulerbernoullybeam_dirZ1()
        {
            //uniform load, section and material
            //using non uniform formulation

            var w = 2.0;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };
            var mat = new UniformIsotropicMaterial(1, 0.25);
            var sec = new Sections.UniformParametric1DSection(1, 1, 1);

            elm.Section = sec;
            elm.Material = mat;



            var u1 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, Vector.J, w, CoordinationSystem.Global);

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Z, elm);

            var hnd = new ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node.Uniform_NUF_Handler();

            var loads = hnd.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

            //var loads = hlpr.GetLocalEquivalentNodalLoads_uniformLoad_nonUniformMatSection(elm, u1);

            var L = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            var m1 = w * L * L / 12;
            var m2 = -w * L * L / 12;

            var v1 = w * L / 2;
            var v2 = w * L / 2;

            Assert.IsTrue(Math.Abs(loads[0].Fy - v1) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(loads[0].Mz - m1) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(loads[1].Fy - v2) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(loads[1].Mz - m2) < 1e-5, "invalid value");
        }

        [Test]
        public void LoadEquivalentNodalLoads_uniformload_nonuniformGeo_eulerbernoullybeam_dirY1()
        {
            //uniform load, section and material
            //using non uniform formulation

            var w = 2.0;

            var nodes = new Node[2];

            var L = 4.0;

            nodes[0] = new Node(0, 0, 0) { Label = "n0" };
            nodes[1] = new Node(L, 0, 0) { Label = "n1" };

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var mat = new UniformIsotropicMaterial(1, 0.25);

            var sec = new Sections.NonUniformSamplingParametric1DSection();
            sec.Samples = new List<Tuple<IsoPoint, _1DCrossSectionGeometricProperties>>();

            {
                var h0 = 1.0;
                var h1 = 0.8;
                var h2 = 0.5;

                var pl = Polynomial1D.FromPoints(new double[] { -1, 0, 1 }, new double[] { h0, h1, h2 });

                var sp = Utils.NumericUtils.DivideSpan(-1, 1, 7);

                var B = 1.0;

                foreach (var ksi in sp)
                {
                    var h = pl.Evaluate(ksi);
                    var i = B * Utils.NumericUtils.Power(h, 3) / 12.0;
                    
                    sec.Samples.Add(Tuple.Create(new IsoPoint(ksi), new _1DCrossSectionGeometricProperties() { Iy = i }));
                }
            }


            elm.Section = sec;
            elm.Material = mat;

            var u1 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, Vector.K, w, CoordinationSystem.Global);

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Y, elm);

            //var loads = hlpr.GetLocalEquivalentNodalLoads_uniformLoad_nonUniformMatSection(elm, u1);
            var hnd = new ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node.Uniform_NUF_Handler();

            var loads = hnd.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

            var v1 = 281344 * w / 123397.0;//from octave output

            var m1 = -6457048 * w / 3331719;//from octave output


            var r1 = loads[0].My / m1;
            var r2 = loads[0].Fz / v1;

            Assert.IsTrue(r1 > 0, "invalid sign");
            Assert.IsTrue(r2 > 0, "invalid sing");

            var err = 0.03;

            var v2 = L * w - v1;

            var m2 = L * L * w / 2 + m1 - v2 * L;

            Assert.IsTrue(Math.Abs(1 - r1) < err, "too much error");
            Assert.IsTrue(Math.Abs(1 - r2) < err, "too much error");
        }


        [Test]
        public void LoadEquivalentNodalLoads_uniformload_nonuniformGeo_eulerbernoullybeam_dirZ1()
        {
            //uniform load, section and material
            //using non uniform formulation

            var w = 2.0;

            var nodes = new Node[2];

            var L = 4.0;

            nodes[0] = new Node(0, 0, 0) { Label = "n0" };
            nodes[1] = new Node(L, 0, 0) { Label = "n1" };

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var mat = new UniformIsotropicMaterial(1, 0.25);

            var sec = new Sections.NonUniformSamplingParametric1DSection();
            sec.Samples = new List<Tuple<IsoPoint, _1DCrossSectionGeometricProperties>>();

            {
                var h0 = 1.0;
                var h1 = 0.8;
                var h2 = 0.5;

                var pl = Polynomial1D.FromPoints(new double[] { -1, 0, 1 }, new double[] { h0, h1, h2 });

                var sp = Utils.NumericUtils.DivideSpan(-1, 1, 7);

                var B = 1.0;

                foreach (var ksi in sp)
                {
                    var h = pl.Evaluate(ksi);
                    var i = B * Utils.NumericUtils.Power(h, 3) / 12.0;

                    sec.Samples.Add(Tuple.Create(new IsoPoint(ksi), new _1DCrossSectionGeometricProperties() { Iz = i }));
                }
            }


            elm.Section = sec;
            elm.Material = mat;

            var u1 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, Vector.J, w, CoordinationSystem.Global);

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Z, elm);

            var hnd = new ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node.Uniform_NUF_Handler();

            var loads = hnd.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

            //var loads = hlpr.GetLocalEquivalentNodalLoads_uniformLoad_nonUniformMatSection(elm, u1);

            var v1 = 281344 * w / 123397.0;//from octave output

            var m1 = 6457048 * w / 3331719;//from octave output

            var r1 = loads[0].Mz / m1;
            var r2 = loads[0].Fy / v1;

            Assert.IsTrue(r1 > 0, "invalid sign");
            Assert.IsTrue(r2 > 0, "invalid sing");

            var err = 0.03;

            var v2 = L * w - v1;

            var m2 = L * L * w / 2 + m1 - v2 * L;

            Assert.IsTrue(Math.Abs(1 - r1) < err, "too much error");
            Assert.IsTrue(Math.Abs(1 - r2) < err, "too much error");
        }
    }
}
