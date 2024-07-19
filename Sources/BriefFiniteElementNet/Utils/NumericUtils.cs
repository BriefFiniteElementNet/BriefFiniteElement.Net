using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Utils
{
    public static class NumericUtils
    {


        public static double LinearInterpolate(double x1, double y1, double x2, double y2, double x)
        {
            var m = (y2 - y1) / (x2 - x1);

            var y = y1 + m * (x - x1);

            return y;
        }




        /// <summary>
        /// divides the specified length into <see cref="pcs"/> parts.
        /// </summary>
        /// <param name="length"></param>
        /// <param name="pcs"></param>
        /// <returns>list of joints of parts</returns>
        public static double[] Divide(double length, int pcs)
        {
            if (pcs < 1)
                throw new Exception();


            var delta = length / (pcs);

            var buf = new double[pcs + 1];
            var n = pcs + 1.0;

            for (var i = 0; i < pcs + 1; i++)
            {
                buf[i] = length * i / (pcs * 1.0);
            }

            return buf;
        }

        /// <summary>
        /// divides the specified span into <see cref="pcs"/> parts.
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="x1"></param>
        /// <param name="pcs"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static double[] DivideSpan(double x0, double x1, int pcs)
        {
            if (pcs < 1)
                throw new Exception();

            var length = x1 - x0;

            var buf = Divide(length, pcs);

            for (int i = 0; i < buf.Length; i++)
                buf[i] += x0;

            return buf;
        }


        /// <summary>
        /// computes Math.Pow(num,exp) in much faster way
        /// </summary>
        /// <param name="num"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        /// <remarks>
        /// based on this implementation: https://stackoverflow.com/a/101613/1106889
        /// </remarks>
        public static double Power(double num, int exp)
        {
            //exp can be negative, zero or positive
            double result = 1.0;

            var isNegative = exp < 0;

            if (isNegative)
                exp = -exp;

            while (exp > 0)
            {
                if (exp % 2 == 1)
                    result *= num;
                exp >>= 1;
                num *= num;
            }

            if (isNegative)
                result = 1 / result;

            return result;
        }

        /// <summary>
        /// Interpolates samples to calculate value at x
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double PolynomialInterpolate(Tuple<double, double>[] samples, double x)
        {
            //TODO: use Newton method to increase speed by removing inverse matrix calculation
            //https://en.wikipedia.org/wiki/Polynomial_interpolation

            var deg = samples.Length;

            var a = new Matrix(deg, deg);

            var b = new Matrix(deg, 1);

            for (var i = 0; i < deg; i++)
            {
                for (var j = 0; j < deg; j++)
                {
                    a[i, j] = Power(samples[i].Item1, deg - j - 1);
                }

                b[i, 0] = samples[i].Item2;
            }

            var buf = 0.0;

            var t = a.Solve(b.CoreArray);

            for (var i = 0; i < deg; i++)
                buf += t[i] * Math.Pow(x, deg - i - 1);


            return buf;
        }


        public static long Factorial(int x)
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
                var buf = 1L;

                for (var i = 1; i <= x; i++)
                    buf = buf * i;

                return buf;
                //return x * Factorial(x - 1);
            }
        }


        /// <summary>
        /// return x!/y! where y < x, i.e. returns x*(x-1)*(x-2)*...*(y+2)*(y+1)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static long Factorial(int x, int y)
        {
            if (x < y)
                throw new NotImplementedException();

            if (x == y)
                return 1;

            if (y > 1)// also x > 0 and x > y
            {
                var buf = 1L;

                for (var i = y + 1; i <= x; i++)
                    buf *= i;

                return buf;
            }

            if (y == 0 || y == 1)
            {
                var buf = 1L;

                for (var i = x; i > 1; i--)
                {
                    buf *= i;
                }

                return buf;
            }

            if (y < 0)

                throw new Exception();

            throw new Exception();
            return Factorial(x) / Factorial(y);
        }


        public static double Min(params double[] arr)
        {
            return arr.Min();
        }

    }
}
