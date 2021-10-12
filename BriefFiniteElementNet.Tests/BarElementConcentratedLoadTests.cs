using System;
using System.Linq;
using NUnit.Framework;
using BriefFiniteElementNet;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;


namespace BriefFiniteElementNet.Tests
{

  

    /// <summary>
    /// Tests for ConcentratedLoad applied on BarElement
    /// </summary>

    public class BarElementConcentratedLoadTests
    {
        [Test]
        public void LoadEquivalentNodalLoads_ConcentratedLod_eulerbernoullybeam_dirY_Fz()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed


            //     ^z                         /\
            //     |  /y                      ||
            //     | /                      w ||
            //      ====================================== --> x
            //

            var w = 2.0;
            var a = 4;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.ConcentratedLoad(new Force(0, 0, -w, 0, 0, 0), new IsoPoint(elm.LocalCoordsToIsoCoords(a)[0]), CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Y, elm);

            var loads = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

            var L = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            var b = L - a;


            var ma = -w * a * b * b / (L * L);
            var mb = -w * a * a * b / (L * L);

            var ra = w * (3 * a + b) * b * b / (L * L * L);//1f
            var rb = w * (a + 3 * b) * a * a / (L * L * L);//1g

            var expectedF0 = new Force(0, 0, -ra, 0, +ma, 0);
            var expectedF1 = new Force(0, 0, -rb, 0, -mb, 0);

            /*
            var m1 = -w * L * L / 12;
            var m2 = w * L * L / 12;

            var v1 = -w * L / 2;
            var v2 = -w * L / 2;
            */

            var d0 = loads[0] - expectedF0;
            var d1 = loads[1] - expectedF1;

            Assert.IsTrue(Math.Abs(d0.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d0.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(d1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d1.Moments.Length) < 1e-5, "invalid value");
        }

        [Test]
        public void LoadEquivalentNodalLoads_ConcentratedLod_eulerbernoullybeam_dirY_Fz_Start()
        {
            //concentrated load applies on location xi=-1, then equivalent nodal loads at start node should be same as concentrated load
            var w = 2.0;
            var a = 0;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.ConcentratedLoad(new Force(0, 0, -w, 0, 0, 0), new IsoPoint(elm.LocalCoordsToIsoCoords(a)[0]), CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Y, elm);

            var loads = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

            var d0 = loads[0] - u1.Force;
            var d1 = Force.Zero;

            Assert.IsTrue(Math.Abs(d0.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d0.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(d1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d1.Moments.Length) < 1e-5, "invalid value");
        }

        [Test]
        public void LoadEquivalentNodalLoads_ConcentratedLod_eulerbernoullybeam_dirY_Fz_End()
        {
            var l = 4;
            var w = 2.0;
            var a = l;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(l, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.ConcentratedLoad(new Force(0, 0, -w, 0, 0, 0), new IsoPoint(elm.LocalCoordsToIsoCoords(a)[0]), CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Y, elm);

            var loads = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

            var d0 = Force.Zero;
            var d1 = loads[1] - u1.Force;

            Assert.IsTrue(Math.Abs(d0.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d0.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(d1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d1.Moments.Length) < 1e-5, "invalid value");
        }

        [Test]
        public void LoadEquivalentNodalLoads_ConcentratedLod_eulerbernoullybeam_dirY_My()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed
            //https://www.amesweb.info/Beam/Fixed-Fixed-Beam-Bending-Moment.aspx

            //                               ^
            //     ^y                     m0 ^
            //     |                         |
            //     |                         |
            //      ====================================== --> x
            //    /z

            var w = 2.0;
            
            var l = 4.0;
            var a = l/3;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(l, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var isoLoc = elm.LocalCoordsToIsoCoords(a);

            var u1 = new Loads.ConcentratedLoad(new Force(0, 0, 0, 0, w, 0), 
                new IsoPoint(isoLoc[0]), 
                CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Y, elm);

            var loads = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

            var L = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            var b = L - a;

            var ma = w / (L * L) * (L * L - 4 * a * L + 3 * a * a);
            var mb = w / (L * L) * (3 * a * a - 2 * a * L);

            var ra = 6 * w * a / (L * L * L) * (L - a);//R1
            var rb = -6 * w * a / (L * L * L) * (L - a);//R1

            /*
            var m1 = -w * L * L / 12;
            var m2 = w * L * L / 12;

            var v1 = -w * L / 2;
            var v2 = -w * L / 2;
            */

            var expectedR1 = new Force(0, 0, -ra, 0, -ma, 0);//expected reaction 1
            var expectedR2 = new Force(0, 0, -rb, 0, -mb, 0);//expected reaction 2

            var d0 = loads[0] + expectedR1;
            var d1 = loads[1] + expectedR2;


            Assert.IsTrue(Math.Abs(d0.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d0.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(d1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d1.Moments.Length) < 1e-5, "invalid value");
        }

        [Test]
        public void LoadEquivalentNodalLoads_ConcentratedLod_eulerbernoullybeam_dirY_My_Start()
        {
            //equivalent nodal load of a concentrated load applied at start point is same as load itself!

            var w = 2.0;
            var a = 0;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.ConcentratedLoad(new Force(0, 0, 0, 0, w, 0), new IsoPoint(elm.LocalCoordsToIsoCoords(a)[0]), CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Y, elm);

            var loads = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

            var d0 = loads[0] - u1.Force;
            var d1 = Force.Zero;


            Assert.IsTrue(Math.Abs(d0.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d0.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(d1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d1.Moments.Length) < 1e-5, "invalid value");
        }

        [Test]
        public void LoadEquivalentNodalLoads_ConcentratedLod_eulerbernoullybeam_dirY_My_End()
        {
            var l = 4.0;
            var w = 2.0;
            var a = l;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(l, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var f = new Force(0, 0, 0, 0, w, 0);

            var loc = new IsoPoint(elm.LocalCoordsToIsoCoords(a)[0]);

            var u1 = new Loads.ConcentratedLoad(f, loc, CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Y, elm);

            var loads = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

            var d0 = Force.Zero;
            var d1 = loads[1] - f;


            Assert.IsTrue(Math.Abs(d0.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d0.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(d1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d1.Moments.Length) < 1e-5, "invalid value");
        }

        [Test]
        public void LoadEquivalentNodalLoads_ConcentratedLod_eulerbernoullybeam_dirZ_Mz()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed
            //https://www.amesweb.info/Beam/Fixed-Fixed-Beam-Bending-Moment.aspx

            //                               ^
            //     ^m                     m0 ^
            //     |  /z                     |
            //     | /                       |
            //      ====================================== --> x
            //

            var w = 2.0;
            var a = 1.234560;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.ConcentratedLoad(new Force(0, 0, 0, 0, 0, w), new IsoPoint(elm.LocalCoordsToIsoCoords(a)[0]), CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Z, elm);

            var loads = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

            var L = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            var b = L - a;

            var ma = -w / (L * L) * (L * L - 4 * a * L + 3 * a * a);
            var mb = w / (L * L) * (3 * a * a - 2 * a * L);

            var ra = 6 * w * a / (L * L * L) * (L - a);//R1
            var rb = -6 * w * a / (L * L * L) * (L - a);//R1


            var expectedR1 = new Force(0, ra, 0, 0, 0, ma);//expected reaction 1
            var expectedR2 = new Force(0, rb, 0, 0, 0, -mb);//expected reaction 2



            /*
            var m1 = -w * L * L / 12;
            var m2 = w * L * L / 12;

            var v1 = -w * L / 2;
            var v2 = -w * L / 2;
            */

            var d0 = loads[0] + expectedR1;
            var d1 = loads[1] + expectedR2;

            Assert.IsTrue(Math.Abs(d0.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d0.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(d1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d1.Moments.Length) < 1e-5, "invalid value");
        }

        [Test]
        public void LoadEquivalentNodalLoads_ConcentratedLod_eulerbernoullybeam_dirZ_Mz_Start()
        {
            var w = 2.0;
            var a = 0;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.ConcentratedLoad(new Force(0, 0, 0, 0, 0, w), new IsoPoint(elm.LocalCoordsToIsoCoords(a)[0]), CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Z, elm);

            var loads = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

            var d0 = loads[0] - u1.Force;
            var d1 = Force.Zero;

            Assert.IsTrue(Math.Abs(d0.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d0.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(d1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d1.Moments.Length) < 1e-5, "invalid value");
        }

        [Test]
        public void LoadEquivalentNodalLoads_ConcentratedLod_eulerbernoullybeam_dirZ_Mz_End()
        {
            var w = 2.0;
            
            var l = 4.0;
            var a = l;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(l, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.ConcentratedLoad(new Force(0, 0, 0, 0, 0, w), new IsoPoint(elm.LocalCoordsToIsoCoords(a)[0]), CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Z, elm);

            var loads = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

            var d0 = Force.Zero;
            var d1 = loads[1] - u1.Force;

            Assert.IsTrue(Math.Abs(d0.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d0.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(d1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d1.Moments.Length) < 1e-5, "invalid value");
        }

        [Test]
        public void LoadEquivalentNodalLoads_ConcentratedLod_eulerbernoullybeam_dirZ_Fy()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed


            //     ^y                       w
            //     |                        ||
            //     |                        \/
            //      ====================================== --> x
            //    / 
            //   /z

            var w = 2.0;
            var a = 2;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.ConcentratedLoad(new Force(0, -w, 0, 0, 0, 0), new IsoPoint(elm.LocalCoordsToIsoCoords(a)[0]), CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Z,elm);

            var loads = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

            var L = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            var b = L - a;


            var ma = w * a * b * b / (L * L);
            var mb = w * a * a * b / (L * L);

            var ra = w * (3 * a + b) * b * b / (L * L * L);//1f
            var rb = w * (a + 3 * b) * a * a / (L * L * L);//1g

            var expectedF0 = new Force(0, -ra, 0, 0, 0, -ma);
            var expectedF1 = new Force(0, -rb, 0, 0, 0, mb);

            /*
            var m1 = -w * L * L / 12;
            var m2 = w * L * L / 12;

            var v1 = -w * L / 2;
            var v2 = -w * L / 2;
            */

            var d0 = loads[0] - expectedF0;
            var d1 = loads[1] - expectedF1;

            Assert.IsTrue(Math.Abs(d0.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d0.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(d1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d1.Moments.Length) < 1e-5, "invalid value");
        }

        [Test]
        public void LoadEquivalentNodalLoads_ConcentratedLod_eulerbernoullybeam_dirZ_Fy_Start()
        {
            var w = 2.0;
            var a = 0;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.ConcentratedLoad(new Force(0, -w, 0, 0, 0, 0), new IsoPoint(elm.LocalCoordsToIsoCoords(a)[0]), CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Z, elm);

            var loads = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

            var d0 = loads[0] - u1.Force;
            var d1 = Force.Zero;

            Assert.IsTrue(Math.Abs(d0.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d0.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(d1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d1.Moments.Length) < 1e-5, "invalid value");
        }

        [Test]
        public void LoadEquivalentNodalLoads_ConcentratedLod_eulerbernoullybeam_dirZ_Fy_End()
        {
            var w = 2.0;
            
            var l = 4.0;
            var a = l;


            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.ConcentratedLoad(new Force(0, -w, 0, 0, 0, 0), new IsoPoint(elm.LocalCoordsToIsoCoords(a)[0]), CoordinationSystem.Global);

            var hlpr = new ElementHelpers.EulerBernoulliBeamHelper(ElementHelpers.BeamDirection.Z, elm);

            var loads = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

            var d0 = Force.Zero;
            var d1 = loads[1] - u1.Force;

            Assert.IsTrue(Math.Abs(d0.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d0.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(d1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d1.Moments.Length) < 1e-5, "invalid value");
        }





        [Test]
        public void LoadInternalForce_concentratedLLoad_eulerbernoullybeam_dirY_fz()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var w = 2.0;
            var forceLocation = 2;//[m]
            var L = 4;//[m]

            //var model = new Model();

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.ConcentratedLoad();

            u1.Case = LoadCase.DefaultLoadCase;
            u1.Force = new Force(0, 0, w, 0, 0, 0);
            u1.CoordinationSystem = CoordinationSystem.Global;

            u1.ForceIsoLocation = new IsoPoint(elm.LocalCoordsToIsoCoords(forceLocation)[0]);

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Y,elm);

            var length = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;


            foreach (var x in CalcUtil.Divide(length, 10))
            {
                //var x = 0.1;

                if (x == 0 || x == forceLocation || x == length)
                    continue;

                var xi = elm.LocalCoordsToIsoCoords(x)[0];


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
                    var rb = w * (a + 3*b) * a * a / (L * L * L);//1g

                    if (x < forceLocation)
                        vi = ra;
                    else
                        vi = -rb;
                }


                var ends = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

                var testFrc = hlpr.GetLoadInternalForceAt(elm, u1, new double[] { xi }).ToForce();

                var exactFrc = new Force(fx: 0, fy: 0, fz: vi, mx: 0, my: mi, mz: 0);


                var dm =  testFrc.My - exactFrc.My;
                var df = testFrc.Fz - exactFrc.Fz;


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

        [Test]
        public void LoadInternalForce_concentratedLLoad_eulerbernoullybeam_dirZ_fy()
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

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Z,elm);

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


                var ends = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

                var testFrc = hlpr.GetLoadInternalForceAt(elm, u1, new double[] { xi[0] * (1 - 1e-9) }).ToForce();

                var exactFrc = new Force(fx: 0, fy: -vi, fz: 0, mx: 0, my: 0, mz: +mi);

                var d = exactFrc - testFrc;

                var dm = d.Mz;
                var df = d.Fy;

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

        [Test]
        public void LoadInternalForce_concentratedLLoad_eulerbernoullybeam_dirZ_Mz()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var w = 2.0;
            var forceLocation = 2;//[m]
            var L = 4;//[m]

            //var model = new Model();

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.ConcentratedLoad();

            u1.Case = LoadCase.DefaultLoadCase;
            u1.Force = new Force(0, 0, 0, 0, 0, w);
            u1.CoordinationSystem = CoordinationSystem.Global;

            u1.ForceIsoLocation = new IsoPoint(elm.LocalCoordsToIsoCoords(forceLocation)[0]);

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Z,elm);

            var length = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;


            foreach (var x in CalcUtil.Divide(length, 10))
            {
                if (x == 0 || x == forceLocation || x == length)
                    continue;

                var xi = elm.LocalCoordsToIsoCoords(x);

                var mi = 0.0;
                var vi = 0.0;

                {
                    //https://www.amesweb.info/Beam/Fixed-Fixed-Beam-Bending-Moment.aspx

                    var a = forceLocation;
                    var b = L - a;

                    var ma = -w / (L * L) * (L * L - 4 * a * L + 3 * a * a);

                    var mb = -w / (L * L) * (3 * a * a - 2 * a * L);

                    //var m = (y1 - y0) / (x1 - x0);

                    

                    var ra = -6 * w * a / (L * L * L) * (L - a);//R1
                    var rb = -ra;//R2

                    mi = ma + ra * x + ((x > forceLocation) ? w : 0.0);

                    vi = ra;
                }

                var ends = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

                var testFrc = hlpr.GetLoadInternalForceAt(elm, u1, new double[] { xi[0] * (1 - 1e-9) }).ToForce();

                var exactFrc = new Force(fx: 0, fy: vi, fz: 0, mx: 0, my: 0, mz: -mi);

                var d = testFrc - exactFrc;

                var dm = d.Mz;
                var df = d.Fy;

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

        [Test]
        public void LoadInternalForce_concentratedLLoad_eulerbernoullybeam_dirY_My()
        {
            //internal force of 2 node beam beam with uniform load and both ends fixed

            var w = 2.0;
            var forceLocation =2.123;//[m]
            var L = 4;//[m]

            //var model = new Model();

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.ConcentratedLoad();

            u1.Case = LoadCase.DefaultLoadCase;
            u1.Force = new Force(0, 0, 0, 0, w, 0);
            u1.CoordinationSystem = CoordinationSystem.Global;

            u1.ForceIsoLocation = new IsoPoint(elm.LocalCoordsToIsoCoords(forceLocation)[0]);

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Y,elm);

            var length = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;


            foreach (var x in CalcUtil.Divide(length, 10))
            {
                //var x = 2.4;

                var xi = elm.LocalCoordsToIsoCoords(x);

                var mi = 0.0;
                var vi = 0.0;

                {
                    //https://www.amesweb.info/Beam/Fixed-Fixed-Beam-Bending-Moment.aspx

                    var a = forceLocation;
                    var b = L - a;

                    var ma = w / (L * L) * (L * L - 4 * a * L + 3 * a * a);
                    var mb = w / (L * L) * (3 * a * a - 2 * a * L);

                    var ra = 6 * w * a / (L * L * L) * (L - a);//R1
                    var rb = -6 * w * a / (L * L * L) * (L - a);//R1


                    mi = ma + ra * x - ((x > forceLocation) ? w : 0.0);

                    vi = ra;
                }


                var ends = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

                var testFrc = hlpr.GetLoadInternalForceAt(elm, u1, new double[] { xi[0] * (1 - 1e-9) }).ToForce();

                var exactFrc = new Force(fx: 0, fy: 0, fz: vi, mx: 0, my: mi, mz: 0);

                var d = testFrc - exactFrc;

                var dm = d.My;//regarding value
                var df = d.Fz;//regarding value

                Assert.IsTrue(Math.Abs(dm) < 1e-5, "invalid value");
                Assert.IsTrue(Math.Abs(df) < 1e-5, "invalid value");
            }

            {
                var end1 = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

                var f0 = hlpr.GetLoadInternalForceAt(elm, u1, new double[] { -1 + 1e-9 }).ToForce(); ;

                var sum = end1[0] - f0;

                Assert.IsTrue(Math.Abs(sum.Forces.Length) < 1e-5, "invalid value");//regarding sign
                Assert.IsTrue(Math.Abs(sum.Moments.Length) < 1e-5, "invalid value");//regarding sign
            }
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

            var u1 = new Loads.ConcentratedLoad();

            u1.Case = LoadCase.DefaultLoadCase;
            u1.Force = new Force(w, 0, 0, 0, 0, 0);
            u1.CoordinationSystem = CoordinationSystem.Global;

            u1.ForceIsoLocation = new IsoPoint(elm.LocalCoordsToIsoCoords(forceLocation)[0]);

            var hlpr = new TrussHelper(elm);

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

        [Test]
        public void LoadInternalForce_concentratedLLoad_Shaft_Mx()
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

            var hlpr = new ShaftHelper(elm);

            var length = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;


            foreach (var x in CalcUtil.Divide(length, 10))
            {
                var xi = elm.LocalCoordsToIsoCoords(x);

                var mi = 0.0;
                var vi = 0.0;

                {
                    var a = forceLocation;
                    var b = L - a;

                    var ra = (1 - (a / L)) * w;

                    var rb = (1 - (b / L)) * w;

                    mi = ra + ((x > forceLocation) ? -w : 0.0);
                }


                var ends = hlpr.GetLocalEquivalentNodalLoads(elm, u1);

                var testFrc = hlpr.GetLoadInternalForceAt(elm, u1, new double[] { xi[0] * (1 - 1e-9) }).ToForce();

                var exactFrc = new Force(fx: 0, fy: 0, fz: vi, mx: -mi, my: 0, mz: 0);

                var df = Math.Abs(testFrc.Mx) - Math.Abs(exactFrc.Mx);


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
