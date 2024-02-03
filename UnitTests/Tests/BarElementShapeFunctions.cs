using System;
using System.Collections.Generic;
using System.Text;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using NUnit.Framework;

namespace BriefFiniteElementNet.Tests
{
    public class BarElementShapeFunctions
    {
        [Test]
        public void eulerbernoullybeam_dirZ()
        {
            var w = 2.0;
            var a = 4;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var L = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            var b = L - a;

            var hlpr = new EulerBernoulliBeamHelper2Node(BeamDirection.Z, elm);

            for (var i = 0.0; i <= 1; i += 0.01)
            {
                var x = i * L;

                var iso = elm.LocalCoordsToIsoCoords(x);

                var n = hlpr.GetNMatrixAt(elm, iso);

                var n1 = n[0, 0];
                var m1 = n[0, 1];
                var n2 = n[0, 2];
                var m2 = n[0, 3];

                var n1p = n[1, 0];
                var m1p = n[1, 1];
                var n2p = n[1, 2];
                var m2p = n[1, 3];


                var xi = iso[0];

                var n1e = 0.25 * (2 - 3 * xi + xi * xi * xi);
                var m1e = 0.125 * L * (1 - xi - xi * xi + xi * xi * xi);
                var n2e = 0.25 * (2 + 3 * xi - xi * xi * xi);
                var m2e = 0.125 * L * (-1 - xi + xi * xi + xi * xi * xi);

                var epsilon = 1e-5;

                Assert.IsTrue(Math.Abs(n1 - n1e) < epsilon, "invalid value");
                Assert.IsTrue(Math.Abs(n2 - n2e) < epsilon, "invalid value");
                Assert.IsTrue(Math.Abs(m1 - m1e) < epsilon, "invalid value");
                Assert.IsTrue(Math.Abs(m2 - m2e) < epsilon, "invalid value");
            }
        }

        [Test]
        public void eulerbernoullybeam_dirZ_diff()
        {
            var w = 2.0;
            var a = 4;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(5.2355, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var L = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            var b = L - a;

            var hlpr = new EulerBernoulliBeamHelper2Node(BeamDirection.Z, elm);


            var n0 = hlpr.GetNMatrixAt(elm ,- 1 );
            var n1 = hlpr.GetNMatrixAt(elm,1 );

            var epsilon = 1e-5;

            Assert.IsTrue(Math.Abs(n0[1, 1] - L/2) < epsilon, "invalid value");
        }

        [Test]
        public void eulerbernoullybeam_dirY()
        {
            var w = 2.0;
            var a = 4;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var L = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            var b = L - a;

            var hlpr = new EulerBernoulliBeamHelper2Node(BeamDirection.Y, elm);

            for (var i = 0.0; i <= 1; i += 0.01)
            {
                var x = i * L;

                var iso = elm.LocalCoordsToIsoCoords(x);

                var n = hlpr.GetNMatrixAt(elm, iso);

                var n1 = n[0, 0];
                var m1 = n[0, 1];
                var n2 = n[0, 2];
                var m2 = n[0, 3];

                var n1p = n[1, 0];
                var m1p = n[1, 1];
                var n2p = n[1, 2];
                var m2p = n[1, 3];


                var xi = iso[0];

                var n1e = 0.25 * (2 - 3 * xi + xi * xi * xi);
                var m1e = -0.125 * L * (1 - xi - xi * xi + xi * xi * xi);
                var n2e = 0.25 * (2 + 3 * xi - xi * xi * xi);
                var m2e = -0.125 * L * (-1 - xi + xi * xi + xi * xi * xi);

                var epsilon = 1e-5;

                Assert.IsTrue(Math.Abs(n1 - n1e) < epsilon, "invalid value");
                Assert.IsTrue(Math.Abs(n2 - n2e) < epsilon, "invalid value");
                Assert.IsTrue(Math.Abs(m1 - m1e) < epsilon, "invalid value");
                Assert.IsTrue(Math.Abs(m2 - m2e) < epsilon, "invalid value");
            }
        }



        [Test]
        public void eulerbernoullybeam_dirY_diff()
        {
            var w = 2.0;
            var a = 4;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(5.236568, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var L = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            var b = L - a;

            var hlpr = new EulerBernoulliBeamHelper2Node(BeamDirection.Y, elm);


            var n1 = hlpr.GetNMatrixAt(elm, 1);

            var epsilon = 1e-5;

            Assert.IsTrue(Math.Abs(n1[1, 3] + L / 2) < epsilon, "invalid value");
        }


    }
}
