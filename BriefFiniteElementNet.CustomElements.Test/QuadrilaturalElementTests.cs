using System;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Elements.ElementHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BriefFiniteElementNet.CustomElements.Test
{
    [TestClass]
    public class QuadrilaturalElementTests
    {
        [TestMethod]
        public void Q4Membrane_GetBMatrix()
        {
            var elm = new QuadrilaturalElement();

            elm.Nodes[0] = new Node(0, 0, 0);
            elm.Nodes[1] = new Node(1, 10, 0);
            elm.Nodes[2] = new Node(11, 12, 0);
            elm.Nodes[3] = new Node(9, 2, 0);

            var hlp = new Q4MembraneHelper() { TargetElement = elm };

            var test = hlp.GetBMatrixAt(elm, 0.1, 0.2);
            var exact = new Matrix(3, 8);

            //todo: fill the exact
        }

        [TestMethod]
        public void Dkq_GetBMatrix()
        {
            var elm = new QuadrilaturalElement();

            elm.Nodes[0] = new Node(0, 0, 0);
            elm.Nodes[1] = new Node(1, 10, 0);
            elm.Nodes[2] = new Node(11, 12, 0);
            elm.Nodes[3] = new Node(9, 2, 0);

            var hlp = new DkqHelper() { TargetElement = elm };

            var test = hlp.GetBMatrixAt(elm, 0.1, 0.2);
            var exact = new Matrix(3, 12);

            //todo: fill the exact
        }
    }
}
