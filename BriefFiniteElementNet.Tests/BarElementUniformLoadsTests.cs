using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BriefFiniteElementNet;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;


namespace BriefFiniteElementNet.Tests
{
    [TestClass]
    public class BarElementUniformLoadsTests
    {
        [TestMethod]
        public void LoadEquivalentNodalLoads_uniformload_eulerbernoullybeam_dirY()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var w = 2.0;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };


            var u1 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, -Vector.K, w, CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Y, elm);

            var loads = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

            var L = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            var m1 = w * L * L / 12;
            var m2 = -w * L * L / 12;

            var v1 = -w * L / 2;
            var v2 = -w * L / 2;

            Assert.IsTrue(Math.Abs(loads[0].Fz - v1) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(loads[0].My - m1) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(loads[1].Fz - v2) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(loads[1].My - m2) < 1e-5, "invalid value");



        }

        [TestMethod]
        public void LoadEquivalentNodalLoads_uniformload_eulerbernoullybeam_dirZ()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var w = 2.0;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };


            var u1 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, -Vector.J, w, CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Z, elm);

            var loads = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

            var L = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            var m1 = -w * L * L / 12;
            var m2 = w * L * L / 12;

            var v1 = -w * L / 2;
            var v2 = -w * L / 2;



            Assert.IsTrue(Math.Abs(loads[0].Fy - v1) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(loads[0].Mz - m1) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(loads[1].Fy - v2) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(loads[1].Mz - m2) < 1e-5, "invalid value");
        }

        [TestMethod]
        public void LoadEquivalentNodalLoads_uniformload_truss()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var w = 2.0;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };


            var u1 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, -Vector.I, w, CoordinationSystem.Global);

            var hlpr = new ElementHelpers.TrussHelper(elm);

            var loads = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

            var L = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            var f1 = -w * L / 2;
            var f2 = -w * L / 2;



            Assert.IsTrue(Math.Abs(loads[0].Fx - f1) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(loads[1].Fx - f2) < 1e-5, "invalid value");
        }

        [TestMethod]
        public void LoadInternalForce_uniformload_eulerbernoullybeam_dirY()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var w = 2.0;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };


            var u1 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, -Vector.K, w, CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Y, elm);

            var length = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;


            foreach (var x in CalcUtil.Divide(length, 10))
            {
                var xi = elm.LocalCoordsToIsoCoords(x);

                var mi = w / 12 * (6 * length * x - 6 * x * x - length * length);
                var vi = w * (length / 2 - x);

                var testFrc = hlpr.GetLoadInternalForceAt(elm, u1, new double[] { xi[0] * (1 - 1e-9) }).ToForce();

                var exactFrc = new Force(fx: 0, fy: 0, fz: vi, mx: 0, my: mi, mz: 0);

                var d = testFrc + exactFrc;

                var dm = d.My;
                var df = d.Fz;


                Assert.IsTrue(Math.Abs(dm) < 1e-5, "invalid value");
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

        [TestMethod]
        public void LoadInternalForce_uniformload_eulerbernoullybeam_dirZ()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var w = 2.0;

            //var model = new Model();

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };


            var u1 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, -Vector.J, w, CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Z, elm);

            var length = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;


            foreach (var x in CalcUtil.Divide(length, 10))
            {
                var xi = elm.LocalCoordsToIsoCoords(x);

                var mi = w / 12 * (6 * length * x - 6 * x * x - length * length);
                var vi = -w * (length / 2 - x);

                var testFrc = hlpr.GetLoadInternalForceAt(elm, u1, new double[] { xi[0] * (1 - 1e-9) }).ToForce();

                var exactFrc = new Force(fx: 0, fy: vi, fz: 0, mx: 0, my: 0, mz: mi);

                var dm = Math.Abs(testFrc.Mz) - Math.Abs(exactFrc.Mz);
                var df = Math.Abs(testFrc.Fy) - Math.Abs(exactFrc.Fy);


                Assert.IsTrue(Math.Abs(dm) < 1e-5, "invalid value");
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

        [TestMethod]
        public void barelement_endrelease()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var eI = 210e9 * (0.1 * 0.1 * 0.1 * 0.1) / 12;
            var l = 2.0;

            var n1 = new Node(0, 0, 0);
            var n2 = new Node(l, 0, 0);

            var e1 = new BarElement(2);

            e1.Nodes[0] = n1;
            e1.Nodes[1] = n2;

            //e1.NodalReleaseConditions[0] = Constraints.Fixed;
            //e1.NodalReleaseConditions[1] = Constraints.MovementFixed;

            e1.Material = new UniformIsotropicMaterial(210e9, 0.3);
            e1.Section = new UniformParametric1DSection(0.1, 0.01, 0.01, 0.01);

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Y, e1);

            var s1 = hlpr.CalcLocalStiffnessMatrix(e1);

            //var d1 = 1.0/s1[3, 3];

            var theoricalK = 12 * eI / (l * l * l);
            var calculatedK = s1[2, 2];

            var ratio = theoricalK / calculatedK;
        }

    }
}
