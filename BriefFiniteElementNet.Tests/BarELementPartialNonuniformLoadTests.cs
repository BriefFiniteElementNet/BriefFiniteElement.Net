using BriefFiniteElementNet.Elements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Tests
{
    [TestClass]
    public class BarELementPartialNonuniformLoadTests
    {
        [TestMethod]
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


            u1.CoordinationSystem = CoordinationSystem.Global;
            u1.Direction = -Vector.K;

            // u1.StartLocation = new double[] { };
            // u1.EndLocation = new double[] { };

            //u1.StartMagnitude

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Y, elm);

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
    }
}
