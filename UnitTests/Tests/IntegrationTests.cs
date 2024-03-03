using BriefFiniteElementNet.Integration;
using BriefFiniteElementNet.Mathh;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Tests
{
    public class IntegrationTests
    {
        [Test]
        public void integration_test()
        {
            var p = new Polynomial1D(1.2);//y=1.2

            var x0 = 1.7;

            var x = 2.3;


            for (var i = 1; i < 6; i++)
            {
                var deg = i;

                var itg = new StepFunctionIntegralCalculator() { Polynomial = p };

                var current = itg.CalculateIntegralAt(x0, x, deg);

                var xp = x - x0;
                
                var expected = 1.2 * Math.Pow(xp, deg ) / CalcUtil.Factorial(i);

                var diff = Math.Abs(current - expected);

                var epsilon = 1e-10;

                Assert.IsTrue(diff < epsilon);
            }
        }
    }
}
