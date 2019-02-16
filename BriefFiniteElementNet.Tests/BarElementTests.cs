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
    public class BarElementTests
    {
        [TestMethod]
        public void LoadInternalForce_uniformload_eulerbernoullybeam_dirY()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var w = 2.0;

            //var model = new Model();

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };


            var u1 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, -Vector.K, w, CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Y);

            var length = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;


            foreach (var x in CalcUtil.Divide(length, 10))
            {
                var xi = elm.LocalCoordsToIsoCoords(x);

                var mi = w / 12 * (6 * length * x - 6 * x * x - length * length);
                var vi = w * (length / 2 - x);

                var testFrc = hlpr.GetLoadInternalForceAt(elm, u1, new double[] { xi[0] * (1 - 1e-9) });

                var exactFrc = new Force(fx: 0, fy: 0, fz: vi, mx: 0, my: mi, mz: 0);

                var d = testFrc.FirstOrDefault(i => i.Item1 == DoF.Ry).Item2 + exactFrc.My;

                Assert.IsTrue(d < 1e-5, "invalid value");

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

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Z);

            var length = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;


            foreach (var x in CalcUtil.Divide(length, 10))
            {
                var xi = elm.LocalCoordsToIsoCoords(x);

                var mi = w / 12 * (6 * length * x - 6 * x * x - length * length);
                var vi = w * (length / 2 - x);

                var testFrc = hlpr.GetLoadInternalForceAt(elm, u1, new double[] { xi[0] * (1 - 1e-9) });

                var exactFrc = new Force(fx: 0, fy: vi, fz: 0, mx: 0, my: 0, mz: mi);

                var d = testFrc.FirstOrDefault(i => i.Item1 == DoF.Rz).Item2 + exactFrc.Mz;

                Assert.IsTrue(d < 1e-5, "invalid value");

            }
        }

        //[TestMethod]
        public void LoadInternalForce_trapezoidload_eulerbernoullybeam_dirY()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var w = 2.0;

            //var model = new Model();

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };


            var u1 =
                //new Loads.(LoadCase.DefaultLoadCase, -Vector.K, w, CoordinationSystem.Global);
                new Loads.PartialNonUniformLoad();


            u1.CoordinationSystem= CoordinationSystem.Global;
            u1.Direction = -Vector.K;

           // u1.StartLocation = new double[] { };
           // u1.EndLocation = new double[] { };

            //u1.StartMagnitude

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Y);

            var length = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;


            foreach (var x in CalcUtil.Divide(length, 10))
            {
                var xi = elm.LocalCoordsToIsoCoords(x);

                var mi = w / 12 * (6 * length * x - 6 * x * x - length * length);
                var vi = w * (length / 2 - x);

                var testFrc = hlpr.GetLoadInternalForceAt(elm, u1, new double[] { xi[0] * (1 - 1e-9) });

                var exactFrc = new Force(fx: 0, fy: 0, fz: vi, mx: 0, my: mi, mz: 0);

                var d = testFrc.FirstOrDefault(i => i.Item1 == DoF.Ry).Item2 + exactFrc.My;

                Assert.IsTrue(d < 1e-5, "invalid value");

            }
        }

        [TestMethod]
        public void barelement_endrelease()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var eI = 210e9*(0.1*0.1*0.1*0.1)/12;
            var l = 2.0;
            
            var n1 = new Node(0, 0, 0);
            var n2 = new Node(l, 0, 0);

            var e1 = new BarElement(2);

            e1.Nodes[0] =  n1;
            e1.Nodes[1] = n2;

            e1.NodalReleaseConditions[0] = Constraints.Fixed;
            e1.NodalReleaseConditions[1] = Constraints.MovementFixed;

            e1.Material = new UniformIsotropicMaterial(210e9, 0.3);
            e1.Section = new UniformParametric1DSection(0.1, 0.01, 0.01, 0.01);

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Y);

            var s1 = hlpr.CalcLocalStiffnessMatrix(e1);

            //var d1 = 1.0/s1[3, 3];

            var theoricalK = 12*eI/(l*l*l);
            var calculatedK = s1[2, 2];

            var ratio = theoricalK / calculatedK;
        }

        [TestMethod]
        public void LoadInternalForce_concentratedLLoad_eulerbernoullybeam_dirY_fz()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var w = 2.0;

            var pz = 7.0;


            //https://en.wikipedia.org/wiki/Fixed_end_moment

            var nodes = new Node[2];

            var l = 4.0;
            var a = 1.5;
            var b = l - a;


            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(l, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.ConcentratedLoad();

            u1.CoordinationSystem = CoordinationSystem.Global;
            u1.Force = new Force(0, 0, pz, 0, 0, 0);

            var xi = elm.LocalCoordsToIsoCoords(a)[0];

            u1.ForceIsoLocation = new IsoPoint(xi);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Y);

            var t = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

            var my1_pz = pz * a * b * b / (l * l);//for pz
            var my2_pz = -pz * a * a * b / (l * l);//for pz

            var tol = 1e-10;

            Assert.IsTrue(Math.Abs(my1_pz - t[0].My) < tol, "Invalid value");
            Assert.IsTrue(Math.Abs(my2_pz - t[1].My) < tol, "Invalid value");

        }

        [TestMethod]
        public void LoadInternalForce_concentratedLLoad_eulerbernoullybeam_dirY_my()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var m0 = 7.0;


            //https://en.wikipedia.org/wiki/Fixed_end_moment

            var nodes = new Node[2];

            var l = 4.0;
            var a = 1.25866;
            var b = l - a;


            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(l, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.ConcentratedLoad();

            u1.CoordinationSystem = CoordinationSystem.Global;
            u1.Force = new Force(0, 0, 0, 0, m0, 0);

            var xi = elm.LocalCoordsToIsoCoords(a)[0];

            u1.ForceIsoLocation = new IsoPoint(xi);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Y);

            var t = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

            var my1 = m0 * b * (2*a-b) / (l * l);//for pz
            var my2 = m0 * a * (2 * b - a) / (l * l);//for pz

            var tol = 1e-10;

            Assert.IsTrue(Math.Abs(my1 - t[0].My) < tol, "Invalid value");
            Assert.IsTrue(Math.Abs(my2 - t[1].My) < tol, "Invalid value");

        }
    }
}
