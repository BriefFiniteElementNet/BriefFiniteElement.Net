using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Mathh;
using NUnit.Framework;
using BriefFiniteElementNet;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;


namespace BriefFiniteElementNet.Tests
{

    public class PolynomialTest
    {
        [Test]
        public void derivation_test()
        {
            var p = new SingleVariablePolynomial(9, 8, 7, 6, 5, 4, 3, 2, 1, 0);

            var p1_1 = p.GetDerivative(2);

            var p1_2 = p.GetDerivative(1).GetDerivative(1);

            Assert.AreEqual(p1_2, p1_1, "");
        }

        [Test]
        public void interpolation_test()
        {
            var tpls = new Tuple<double, double>[]{ Tuple.Create(-5.0, 3.0), Tuple.Create(-4.0, 4.0), Tuple.Create(-3.0, 7.0), Tuple.Create(1.0, 0.0), Tuple.Create(5.0, 0.0) };

            var p = SingleVariablePolynomial.FromPoints(tpls);

            var epsilon = 1e-10;

            foreach (var tpl in tpls)
            {
                var diff = Math.Abs(p.Evaluate(tpl.Item1) - tpl.Item2);

                Assert.IsTrue(diff < epsilon);
            }

            
        }


        [Test]
        public void integration_test()
        {
            var p = new SingleVariablePolynomial(1, 0, 0, 0, 1);//x^4+1

            var current = p.EvaluateNthIntegralAt(1, 1.0);

            var expected = 1.2;

            var diff = Math.Abs(current - expected);

            var epsilon = 1e-10;

            Assert.IsTrue(diff < epsilon);
        }

        [Test]
        public void integration_test2()
        {
            var p = new SingleVariablePolynomial(1, 0);//y=x^1+0
            //9th integral is x^9/(9!) 

            var x = 1.23456;
            var current = p.EvaluateNthIntegralAt(9, x);

            var expected = Math.Pow(x, 9) / 362880;//9! is 362880

            var diff = Math.Abs(current - expected);

            var epsilon = 1e-10;

            Assert.IsTrue(diff < epsilon);
        }

        [Test]
        public void integration_test3()
        {
            var p = new SingleVariablePolynomial(1);//y=1
            //9th integral is x^10/(10!) 

            var x = 1.23456;
            var current = p.EvaluateNthIntegralAt(10, x);

            var expected = Math.Pow(x, 10) / 3628800;//9! is 362880

            var diff = Math.Abs(current - expected);

            var epsilon = 1e-10;

            Assert.IsTrue(diff < epsilon);
        }

    }
}
