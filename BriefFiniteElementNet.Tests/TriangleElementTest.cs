using System;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Elements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BriefFiniteElementNet.Tests
{
    [TestClass]
    public class TriangleElementTest
    {
        [TestMethod]
        public void Cst_GetBMatrix()
        {
            var element = new TriangleElement();

            element.Nodes[0] = new Node(1, 2, 3);
            element.Nodes[1] = new Node(4, 5, 6);
            element.Nodes[2] = new Node(7, 8, 9);

            var helper = new CstHelper();

            var b = helper.GetBMatrixAt(element, 0, 0, 0);

            var exact = new Matrix(3, 6);
        }

        [TestMethod]
        public void Dkt_GetBMatrix()
        {
            var element = new TriangleElement();

            element.Nodes[0] = new Node(1, 2, 3);
            element.Nodes[1] = new Node(4, 5, 6);
            element.Nodes[2] = new Node(7, 8, 9);

            var helper = new DktHelper();

            var b = helper.GetBMatrixAt(element, 0, 0, 0);

            var exact = new Matrix(3, 6);
        }


    }
}
