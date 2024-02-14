using BriefFiniteElementNet.Mathh;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet;


namespace BriefFiniteElementNet.Tests
{
    public class MiscTests
    {

        [Test]
        public void DivideTest()
        {
            var x0 = 1;
            var x1 = 3;

            var res = CalcUtil.DivideSpan(x0, x1, 2);

            var epsilon = 1e-9;

            Assert.AreEqual(res.Length, 3);
            Assert.True(res[0].FEquals(1, epsilon));
            Assert.True(res[1].FEquals(2, epsilon));
            Assert.True(res[2].FEquals(3, epsilon));
        }

        [Test]
        public void DivideTest2()
        {
            var x0 = -1;
            var x1 = 1;

            var res = CalcUtil.DivideSpan(x0, x1, 2);

            var epsilon = 1e-9;

            Assert.AreEqual(res.Length, 3);
            Assert.True(res[0].FEquals(x0, epsilon));
            Assert.True(res[1].FEquals(0, epsilon));
            Assert.True(res[2].FEquals(x1, epsilon));
        }


        [Test]
        public void PowTest()
        {
            var @base = 1.234567;

            var pows = new int[] { -5, -3, -1, 0, 1, 3, 5 };

            foreach (var pow in pows)
            {
                var expected = Math.Pow(@base, pow);

                var current = CalcUtil.Power(@base, pow);

                var diff = Math.Abs(current - expected);

                var epsilon = 1e-10;

                Assert.IsTrue(diff < epsilon);
            }
        }


        [Test]
        public void PlyInterolateTest()
        {
            var coefs = new double[] { 6.25e-2, -6.875e-1, 2.9375, -3.1250e-1 };
            var poly = new SingleVariablePolynomial(coefs);

            var xs = new double[] { 1, 3, 5, 7 };
            var ys = new double[] { 2, 4, 5, 8 };

            var pts = xs.Zip(ys).Select(i => Tuple.Create(i.First, i.Second)).ToArray();

            var tests = new double[] { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var vals = new double[] { -4, -0.3125, 2, 3.3125, 4, 4.4375, 5, 6.0625, 8, 11.1875, 16 };

            for (int i = 0; i < tests.Length; i++)
            {
                double x = tests[i];

                var current = CalcUtil.PolynomialInterpolate(pts, x);

                var expected = vals[i];

                var diff = Math.Abs(current - expected);

                var epsilon = 1e-10;

                Assert.IsTrue(diff < epsilon);
            }
        }
    }
}
