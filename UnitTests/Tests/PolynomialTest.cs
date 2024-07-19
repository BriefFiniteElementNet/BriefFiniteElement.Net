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
        public void construction_test()
        {
            var p = new Polynomial1D(5.6789, 4.567, 3.456, 2.345);//y=2.345*x^3+3.456*x^2+4.567*x+5.6789

            var xs = new double[] { 2, 4, 6, 8 };
            var ys = new double[] { 47.3969, 229.3229, 664.0169, 1464.0389 };

            var current = Polynomial1D.FromPoints(xs, ys);
            var expected = p;

            var diff = Utils.AlgebraUtils.GetDiffNorm2(current.Coefficients, expected.Coefficients);

            var epsilon = 1e-10;

            Assert.IsTrue(diff < epsilon);
        }


        [Test]
        public void integral_test()
        {
            var p = new Polynomial1D(0, 0, 0, 1.234);//y=1.234*x^3


            var x = 2.3456;

            var current = p.EvaluateNthIntegral(1, x)[0];
            var expected = 1.234 * Utils.NumericUtils.Power(x, 4) / 4;

            var diff = Math.Abs(current - expected);

            var epsilon = 1e-10;

            Assert.IsTrue(diff < epsilon);
        }

        [Test]
        public void integral_test2()
        {
            var p = new Polynomial1D(0, 0, 0, 1.234);//y=1.234*x^3


            var x = 2.3456;

            var current = p.EvaluateNthIntegral(4, x)[0];
            var expected = 1.234 * Utils.NumericUtils.Power(x, 7) / (7 * 6 * 5 * 4);

            var diff = Math.Abs(current - expected);

            var epsilon = 1e-10;

            Assert.IsTrue(diff < epsilon);
        }


        [Test]
        public void derivation_test()
        {
            var p = new Polynomial1D(0, 0, 0, 1.234);//y=1.234*x^3


            var x = 2.3456;

            var current = p.EvaluateNthDerivative(2, x)[0];
            var expected = 1.234 * 3 * 2 * x;

            var diff = Math.Abs(current - expected);

            var epsilon = 1e-10;

            Assert.IsTrue(diff < epsilon);
        }
    


        [Test]
        public void derivation_test2()
        {
            var p = new SingleVariablePolynomial(9, 8, 7, 6, 5, 4, 3, 2, 1, 0);

            var p1_1 = p.GetDerivative(2);

            var p1_2 = p.GetDerivative(1).GetDerivative(1);

            Assert.AreEqual(p1_2, p1_1, "");
        }


        [Test]
        public void interpolation_test4()
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
        public void integration_test2()
        {
            var p = new SingleVariablePolynomial(1, 0, 0, 0, 1);//x^4+1

            var current = p.EvaluateNthIntegralAt(1, 1.0);

            var expected = 1.2;

            var diff = Math.Abs(current - expected);

            var epsilon = 1e-10;

            Assert.IsTrue(diff < epsilon);
        }

        [Test]
        public void integration_test44()
        {
            var p = new SingleVariablePolynomial(1.2);//y=1.2
            //9th integral is 1.2*x^9/(9!) 

            var n = 5;

            var fact = Utils.NumericUtils.Factorial(n);

            var x = 1.789;
            var current = p.EvaluateNthIntegralAt(9, x);

            var expected = 1.2* Utils.NumericUtils.Power(x, 9) / 362880;//9! is 362880

            var diff = Math.Abs(current - expected);

            var epsilon = 1e-10;

            Assert.IsTrue(diff < epsilon);
        }

    }
}
