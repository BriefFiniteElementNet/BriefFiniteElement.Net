using BriefFiniteElementNet.Mathh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Integration
{
    /// <summary>
    /// Stuff about calculating the integral of f.u where f is polynomial and u is heaviside (unit step function)
    /// refer to docs.
    /// </summary>
    public class StepFunctionIntegralCalculator
    {

        //public SingleVariablePolynomial Polynomial;
        public Polynomial1D Polynomial;


        /// <summary>
        /// Calculates the n'th integral of <see cref="Polynomial"/> at point<see cref="x0"/>
        /// refer to docs
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public double CalculateIntegralAt(double x0,double x, int n)
        {
            if (n < 0 || n >6)
                throw new ArgumentException("0<n<5");


            var u = 0.0;

            if (x > x0)
                u = 1.0;

            double buf;


            var x2 = x * x;
            var x3 = x2 * x;
            var x4 = x3 * x;
            var x5 = x4 * x;

            var x02 = x0 * x0;
            var x03 = x02 * x0;
            var x04 = x03 * x0;
            var x05 = x04 * x0;

            switch (n)
            {
                case 0:
                    buf = u * F0(x);
                    break;

                case 1:
                    buf = u * F1(x) - u * F1(x0);
                    break;

                case 2:
                    buf = -u * x * F1(x0) + u * x0 * F1(x0) + u * F2(x) - u * F2(x0);
                    break;

                case 3:
                    buf = -u * x2 * F1(x0) / 2 + u * x * x0 * F1(x0) - u * x * F2(x0) - u * x02 * F1(x0) / 2 + u * x0 * F2(x0) + u * F3(x) - u * F3(x0);
                    break;

                case 4:
                    buf = -u * x3 * F1(x0) / 6 + u * x2 * x0 * F1(x0) / 2 - u * x2 * F2(x0) / 2 - u * x * x02 * F1(x0) / 2 + u * x * x0 * F2(x0) - u * x * F3(x0) + u * x03 * F1(x0) / 6 - u * x02 * F2(x0) / 2 + u * x0 * F3(x0) + u * F4(x) - u * F4(x0);
                    break;

                case 5:
                    buf = -u * x4 * F1(x0) / 24 + u * x3 * x0 * F1(x0) / 6 - u * x3 * F2(x0) / 6 - u * x2 * x02 * F1(x0) / 4 + u * x2 * x0 * F2(x0) / 2 - u * x2 * F3(x0) / 2 + u * x * x03 * F1(x0) / 6 - u * x * x02 * F2(x0) / 2 + u * x * x0 * F3(x0) - u * x * F4(x0) - u * x04 * F1(x0) / 24 + u * x03 * F2(x0) / 6 - u * x02 * F3(x0) / 2 + u * x0 * F4(x0) + u * F5(x) - u * F5(x0);
                    break;

                case 6:
                    buf = -u * x5 * F1(x0) / 120 + u * x4 * x0 * F1(x0) / 24 - u * x4 * F2(x0) / 24 - u * x3 * x02 * F1(x0) / 12 + u * x3 * x0 * F2(x0) / 6 - u * x3 * F3(x0) / 6 + u * x2 * x03 * F1(x0) / 12 - u * x2 * x02 * F2(x0) / 4 + u * x2 * x0 * F3(x0) / 2 - u * x2 * F4(x0) / 2 - u * x * x04 * F1(x0) / 24 + u * x * x03 * F2(x0) / 6 - u * x * x02 * F3(x0) / 2 + u * x * x0 * F4(x0) - u * x * F5(x0) + u * x + u * x05 * F1(x0) / 120 - u * x04 * F2(x0) / 24 + u * x03 * F3(x0) / 6 - u * x02 * F4(x0) / 2 + u * x0 * F5(x0) - u * x0;
                    break;

                default:
                    throw new Exception("n");
                    break;
            }

            return buf;
        }

        private double F0(double x)
        {
            return Polynomial.EvaluateNthIntegral(0, x)[0];
        }

        private double F1(double x)
        {
            return Polynomial.EvaluateNthIntegral(1, x)[0];
        }

        private double F2(double x)
        {
            return Polynomial.EvaluateNthIntegral(2, x)[0];
        }

        private double F3(double x)
        {
            return Polynomial.EvaluateNthIntegral(3, x)[0];
        }

        private double F4(double x)
        {
            return Polynomial.EvaluateNthIntegral(4, x)[0];
        }

        private double F5(double x)
        {
            return Polynomial.EvaluateNthIntegral(5, x)[0];
        }

    }
}
