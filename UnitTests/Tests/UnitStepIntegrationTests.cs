﻿using BriefFiniteElementNet.Integration;
using BriefFiniteElementNet.Mathh;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Tests
{
    public class UnitStepIntegrationTests
    {

        #region a1
        //rule: where x=x0 result should be zero
        [Test]
        public void test0()
        {
            var clc = new StepFunctionIntegralCalculator();

            var poly = clc.Polynomial = new Polynomial1D(5,4,3,2,1);


            var x = 1.2345;
            var x0 = x;

            var current = clc.CalculateIntegralAt(x0, x, 6);
            var expected = 0.0;

            var epsilon = 1e-10;

            var diff = current - expected;

            Assert.IsTrue(diff < epsilon);
        }

        [Test]
        public void test1()
        {
            var clc = new StepFunctionIntegralCalculator();

            var poly = clc.Polynomial = new Polynomial1D(5, 4, 3, 2, 1);


            var x = 1.2345;
            var x0 = x;

            var current = clc.CalculateIntegralAt(x0, x, 4);
            var expected = 0.0;

            var epsilon = 1e-10;

            var diff = current - expected;

            Assert.IsTrue(diff < epsilon);
        }


        [Test]
        public void test2()
        {
            var clc = new StepFunctionIntegralCalculator();

            var poly = clc.Polynomial = new Polynomial1D(5, 4, 3, 2, 1);


            var x = 1.2345;
            var x0 = x;

            var current = clc.CalculateIntegralAt(x0, x, 3);
            var expected = 0.0;

            var epsilon = 1e-10;

            var diff = current - expected;

            Assert.IsTrue(diff < epsilon);
        }

        [Test]
        public void test3()
        {
            var clc = new StepFunctionIntegralCalculator();

            var poly = clc.Polynomial = new Polynomial1D(5, 4, 3, 2, 1);


            var x = 1.2345;
            var x0 = x;

            var current = clc.CalculateIntegralAt(x0, x, 1);
            var expected = 0.0;

            var epsilon = 1e-10;

            var diff = current - expected;

            Assert.IsTrue(diff < epsilon);
        }

        [Test]
        public void test4()
        {
            var clc = new StepFunctionIntegralCalculator();

            var poly = clc.Polynomial = new Polynomial1D(5, 4, 3, 2, 1);


            var x = 1.2345;
            var x0 = x;

            var current = clc.CalculateIntegralAt(x0, x, 0);
            var expected = 0.0;

            var epsilon = 1e-10;

            var diff = current - expected;

            Assert.IsTrue(diff < epsilon);
        }

        #endregion
    }
}
