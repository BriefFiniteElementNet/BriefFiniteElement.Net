using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Mathh;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class PolynomialTest
    {
        [TestMethod]
        public void derivation_test()
        {
            var p = new Polynomial(9, 8, 7, 6, 5, 4, 3, 2, 1, 0);

            var p1_1 = p.GetDerivative(2);

            var p1_2 = p.GetDerivative(1).GetDerivative(1);

            Assert.AreEqual(p1_2, p1_1, "");
        }

    }
}
