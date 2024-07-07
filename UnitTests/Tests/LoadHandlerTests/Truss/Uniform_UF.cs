using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Tests.LoadHandlerTests.Truss
{
    public class Uniform_UF
    {
        [Test]
        public void EqNodalLoads()
        {
            var w = 2.0;
            var L = 4;//[m]

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(L, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.UniformLoad();

            u1.Case = LoadCase.DefaultLoadCase;
            u1.CoordinationSystem = CoordinationSystem.Global;
            u1.Magnitude = w;
            u1.Direction = Vector.I;

            elm.Loads.Add(u1);

            var hlpr = new TrussHelper2Node(elm);

            var handler = new ElementHelpers.LoadHandlers.Truss2Node.Uniform_UF_Handler();

            var end1 = handler.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

            var f0 = new Force(w * L / 2, 0, 0, 0, 0, 0);
            var f1 = new Force(w * L / 2, 0, 0, 0, 0, 0);


            var d1 = f0 - end1[0];
            var d2 = f1 - end1[1];


            Assert.IsTrue(Math.Abs(d1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d1.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(d2.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d2.Moments.Length) < 1e-5, "invalid value");
        }


        [Test]
        public void InternalForce()
        {
            var w = 2.0;
            var L = 4;//[m]

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0" });
            nodes[1] = (new Node(L, 0, 0) { Label = "n1" });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var u1 = new Loads.UniformLoad();

            u1.Case = LoadCase.DefaultLoadCase;
            u1.CoordinationSystem = CoordinationSystem.Global;
            u1.Magnitude = w;
            u1.Direction = Vector.I;

            elm.Loads.Add(u1);

            var hlpr = new TrussHelper2Node(elm);

            var handler = new ElementHelpers.LoadHandlers.Truss2Node.Uniform_UF_Handler();

            var end1 = handler.GetLocalEquivalentNodalLoads(elm, hlpr, u1);

            var f0 = new Force(w * L / 2, 0, 0, 0, 0, 0);
            var f1 = new Force(w * L / 2, 0, 0, 0, 0, 0);


            var d1 = f0 - end1[0];
            var d2 = f1 - end1[1];


            Assert.IsTrue(Math.Abs(d1.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d1.Moments.Length) < 1e-5, "invalid value");

            Assert.IsTrue(Math.Abs(d2.Forces.Length) < 1e-5, "invalid value");
            Assert.IsTrue(Math.Abs(d2.Moments.Length) < 1e-5, "invalid value");
        }
    }
}
