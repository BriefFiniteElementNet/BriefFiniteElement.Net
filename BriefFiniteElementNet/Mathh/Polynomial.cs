using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Mathh
{
    public class Polynomial
    {

        public Polynomial(params double[] coefs)
        {
            Coefficients = (double[])coefs.Clone();
        }


        double[] Coefficients;

        /// <summary>
        /// Evaluates the value o polynomial at specified <see cref="x"/>
        /// </summary>
        /// <param name="x">evaluation point</param>
        /// <returns>P(<see cref="x"/>)</returns>
        public double Evaluate(double x)
        {
            var buf = 0.0;

            for (var i = 0; i < Coefficients.Length; i++)
            {
                var origPow = Coefficients.Length - 1 - i;

                buf += Coefficients[i] * Math.Pow(x, origPow);
            }

            return buf;
        }


        

        /// <summary>
        /// Gets the derivative of polynomial as another polynomial
        /// </summary>
        /// <param name="deg">derivation degree</param>
        /// <returns>derivation</returns>
        public Polynomial GetDerivative(int deg)
        {
            throw new NotImplementedException();
            /*
            var buf = new double[Coefficients.Length];

            for (var i = 0; i < Coefficients.Length; i++)
            {
                var origPow = Coefficients.Length - 1 - i;

                var n = origPow;
                var m = deg;

                if (n - m >= 0)
                    buf[i] = Factorial(n) / Factorial(n - m);
            }


            return new Polynomial(buf);
            */
        }



        /// <summary>
        /// Evaluates the <see cref="n"/>'th derivation of current polynomial at specified <see cref="x"/>.
        /// </summary>
        /// <param name="n">derivation degree</param>
        /// <param name="x">evaluation location</param>
        /// <returns>evaluation of derivation</returns>
        public double EvaluateDerivative(double x, int n)
        {
            var retVal = 0.0;

            for (var i = 0; i < Coefficients.Length; i++)
            {
                var origPow = Coefficients.Length - 1 - i;

                var nn = origPow;
                var mm = n;

                if (nn - mm >= 0)
                {
                    var cf = Factorial(nn) / Factorial(nn - mm);
                    var cf2 = Coefficients[i];
                    var vari = Math.Pow(x, nn - mm);
                    retVal += vari * cf * cf2;
                }
            }

            return retVal;
        }

        /// <summary>
        /// returns value of n'th derivative of F where F(x) = x ^ n
        /// </summary>
        /// <param name="x"></param>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        static double NthDer(double x, int n, int m)
        {
            var pval = Math.Pow(x, n - m);

            if (m > n) return 0;

            return Factorial(n) / Factorial(n - m) * pval;
        }

        static int Factorial(int x)
        {
            if (x < 0)
            {
                return -1;
            }
            else if (x == 1 || x == 0)
            {
                return 1;
            }
            else
            {
                return x * Factorial(x - 1);
            }
        }

        public bool TryFindRoot(double y,out double x)
        {
            var cnt = 0;

            var loc = -1.0;

            while(cnt++<100)
            {
                var yi = this.Evaluate(loc);
                var d = y - yi;

                if (Math.Abs(d) < 1e-6)
                {
                    x = loc;
                    return true;
                }

                loc = loc - d / this.EvaluateDerivative(loc, 1);
            }

            x = default(double);

            return false;
        }
    }
}
