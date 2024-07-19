using BriefFiniteElementNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Mathh
{
    [Obsolete("work in progress")]
    //difference with SingleVariablePolynomial is only the coefficient order
    public class Polynomial1D : IPolynomial
    {
        

        public static Polynomial1D FromIPolynomial(IPolynomial pl)
        {
            if(pl is SingleVariablePolynomial svp)
            {
                var t = (double[])svp.Coefficients.Clone();
                Array.Reverse(t);
                return new Polynomial1D(t);
            }

            throw new Exception();
        }

        public static Polynomial1D Zero
        {
            get
            {
                return new Polynomial1D(0);
            }
        }

        #region from points


        public static Polynomial1D FromPoints(double x1, double y1)
        {
            return FromPoints(Tuple.Create(x1, y1));
        }

        public static Polynomial1D FromPoints(double x1, double y1, double x2, double y2)
        {
            return FromPoints(Tuple.Create(x1, y1), Tuple.Create(x2, y2));
        }

        public static Polynomial1D FromPoints(params Tuple<double, double>[] points)
        {
            return FromPoints(points.Select(i => i.Item1).ToArray(), points.Select(i => i.Item2).ToArray());
        }

        public static Polynomial1D FromPoints(double[] xs, double[] ys)
        {
            if (xs.Length != ys.Length)
                throw new Exception();

            var n = xs.Length;

            var mtx = new Matrix(n, n);

            var b = new Matrix(n, 1);


            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    var xi = xs[i];
                    var pow = j;// n - j - 1;

                    var v = pow == 0 ? 1.0 : Math.Pow(xi, pow);
                    mtx[i, j] = v;
                }

                b[i, 0] = ys[i];
            }

            var tt = mtx.ToString();
            var coefs = mtx.Solve(b.Values);

            return new Polynomial1D(coefs);
        }

        #endregion


        #region contructor
        /// <summary>
        /// coefs order = a0 a1 a2 a3
        /// poly = a4 * x^4 + a3 * x^3 + a2 * x^2 + a1 * x^1 + a0 * x^0
        /// </summary>
        /// <param name="coefs"></param>
        public Polynomial1D(params double[] coefs)
        {
            this.Coefficients = (double[])coefs.Clone();

        }
        #endregion

        #region properties and field

        public double[] Coefficients;

        public int VariableCount
        {
            get
            {
                return 1;
            }
        }

        public int[] Degree
        {
            get
            {
                var firstNonzero = 0;

                for (var i = 0; i < Coefficients.Length; i++)
                    if (Coefficients[i] != 0)
                    {
                        firstNonzero = i;
                        break;
                    }
                        

                return new int[] { firstNonzero };
            }
        }

        #endregion


        public double Evaluate(params double[] v)
        {
            var buf = 0.0;
            var x = v[0];

            for (var i = 0; i < Coefficients.Length; i++)
            {
                var pow = i;
                var a = Coefficients[i];
                var y = NumericUtils.Power(x, pow);
                buf += a * y;//buf += a*x^i
            }

            return buf;
        }

        public double[] EvaluateDerivative(params double[] v)
        {
            return EvaluateNthDerivative(1, v);
        }

        public double[] EvaluateNthDerivative(int m, params double[] v)
        {
            var buf = 0.0;
            var x = v[0];

            for (var i = 0; i < Coefficients.Length; i++)
            {
                var n = i;

                var newPow = n - m;

                if (newPow < 0)
                    continue;

                var coef = Coefficients[i];

                var newCoef = coef * NumericUtils.Factorial(n, n - m);

                var val = NumericUtils.Power(x, newPow);
                buf += newCoef * val;
            }

            return new double[] { buf };
        }

        public double[] EvaluateNthIntegral(int m, params double[] v)
        {
            var buf = 0.0;
            var x = v[0];

            for (var i = 0; i < Coefficients.Length; i++)
            {
                var n = i;

                var newPow = n + m;

                if (newPow < 0)
                    continue;

                var coef = Coefficients[i];

                var newCoef = coef / NumericUtils.Factorial(n + m, n);

                var val = NumericUtils.Power(x, newPow);
                buf += newCoef * val;
            }

            return new double[] { buf };
        }
    }
}
