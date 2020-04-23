using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Mathh;
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

        [TestMethod]
        public void interpolation_test()
        {
            var tpls = new Tuple<double, double>[]{ Tuple.Create(-5.0, 3.0), Tuple.Create(-4.0, 4.0), Tuple.Create(-3.0, 7.0), Tuple.Create(1.0, 0.0), Tuple.Create(5.0, 0.0) };

            var p = Polynomial.FromPoints(tpls);

            var epsilon = 1e-10;

            foreach (var tpl in tpls)
            {
                var diff = Math.Abs(p.Evaluate(tpl.Item1) - tpl.Item2);

                Assert.IsTrue(diff < epsilon);
            }

            
        }

    }
}
