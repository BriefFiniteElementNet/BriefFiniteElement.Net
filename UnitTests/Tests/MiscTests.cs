using BriefFiniteElementNet.Mathh;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Tests
{
    public class MiscTests
    {
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
