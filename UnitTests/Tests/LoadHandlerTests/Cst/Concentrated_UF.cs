using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BriefFiniteElementNet.Tests.LoadHandlerTests.Cst
{
    internal class Concentrated_UF
    {
        [Test]
        public void EqNodalForce()
        {
            var n0 = new Node(0, 0, 0);
            var n1 = new Node(0, 1, 0);
            var n2 = new Node(1, 0, 0);

            var elm = new TriangleElement();
            elm.Nodes[0] = n0;
            elm.Nodes[1] = n1;
            elm.Nodes[2] = n2;

            var f = new Force();

            f.Fx = 1;

            var load = new ConcentratedLoad();
            load.Force = f;
            load.ForceIsoLocation = new IsoPoint(1.0/3, 1.0/3);//center of the triangle

            var hnd = new BriefFiniteElementNet.ElementHelpers.LoadHandlers.CstHelper.Concentrated_UF_Handler();
            var hlpr = new ElementHelpers.CstHelper();

            var frcs = hnd.GetLocalEquivalentNodalLoads(elm, hlpr, load);

            var fe = new Force(f.Fx / 3, 0, 0, 0, 0, 0);

            var expected = new Force[] { fe, fe, fe };

            var e0 = expected[0] - frcs[0];
            var e1 = expected[1] - frcs[1];
            var e2 = expected[2] - frcs[2];


            Assert.IsTrue(Math.Abs(e0.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(e0.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(e1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(e1.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(e2.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(e2.Moments.Length) < 1e-5, "invalid value");
        }

        [Test]
        public void InternalForce()
        {
            
        }

        [Test]
        public void InternalDisplacement()
        {
            
        }
    }
}
