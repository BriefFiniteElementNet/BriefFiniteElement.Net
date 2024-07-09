using System;
using System.Linq;
using NUnit.Framework;
using BriefFiniteElementNet;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using EulerBernoulliBeamHelper = BriefFiniteElementNet.ElementHelpers.BarHelpers.EulerBernoulliBeamHelper2Node;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Tests.LoadHandlerTests.EulerBernullyBeam
{
    public class EqNodal_Concentrated_UF
    {
        [Test]
        public void EqLoads_DirZ_Fy()
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

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Y, elm);

            var hnd = new ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node.Concentrated_UF_Handler();

            var loads = hnd.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

            var L = (elm.Nodes[1].Location - elm.Nodes[0].Location).Length;

            var b = L - a;

            var ma = -w * a * b * b / (L * L);
            var mb = -w * a * a * b / (L * L);

            var ra = w * (3 * a + b) * b * b / (L * L * L);//1f
            var rb = w * (a + 3 * b) * a * a / (L * L * L);//1g

            var expectedF0 = new Force(0, 0, -ra, 0, +ma, 0);
            var expectedF1 = new Force(0, 0, -rb, 0, -mb, 0);

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

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Y, elm);

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

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Y, elm);

            var hnd = new ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node.Concentrated_UF_Handler();

            var loads = hnd.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

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
            var a = l / 3;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(l, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var isoLoc = elm.LocalCoordsToIsoCoords(a);

            var u1 = new Loads.ConcentratedLoad(new Force(0, 0, 0, 0, w, 0),
                new IsoPoint(isoLoc[0]),
                CoordinationSystem.Global);

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Y, elm);

            var hnd = new ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node.Concentrated_UF_Handler();

            var loads = hnd.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

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

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Y, elm);

            var hnd = new ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node.Concentrated_UF_Handler();

            var loads = hnd.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

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

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Y, elm);

            var hnd = new ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node.Concentrated_UF_Handler();

            var loads = hnd.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

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

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Z, elm);

            var hnd = new ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node.Concentrated_UF_Handler();

            var loads = hnd.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

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

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Z, elm);

            var hnd = new ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node.Concentrated_UF_Handler();

            var loads = hnd.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

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

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Z, elm);

            var hnd = new ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node.Concentrated_UF_Handler();

            var loads = hnd.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

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

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Z, elm);

            var hnd = new ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node.Concentrated_UF_Handler();

            var loads = hnd.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

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

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Z, elm);

            var hnd = new ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node.Concentrated_UF_Handler();

            var loads = hnd.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

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

            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Z, elm);

            var hnd = new ElementHelpers.LoadHandlers.EulerBernoulliBeamHelper2Node.Concentrated_UF_Handler();

            var loads = hnd.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

            var d0 = Force.Zero;
            var d1 = loads[1] - u1.Force;

            Assert.IsTrue(Math.Abs(d0.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d0.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(d1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d1.Moments.Length) < 1e-5, "invalid value");
        }




    }
}
