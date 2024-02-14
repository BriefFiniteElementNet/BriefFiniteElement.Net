﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace BriefFiniteElementNet.Mathh
{
    /// <summary>
    /// Represents a single variable polynomial (y=a_n*x^n+...+a_0*x^0)
    /// </summary>
    [Serializable]
    [Obsolete("use Polynomial1D if possible")]
    public class SingleVariablePolynomial : IPolynomial
    {
        public static SingleVariablePolynomial FromPoints(double x1, double y1)
        {
            return FromPoints(Tuple.Create(x1, y1));
        }

        public static SingleVariablePolynomial FromPoints(double x1, double y1, double x2, double y2)
        {
            return FromPoints(Tuple.Create(x1, y1), Tuple.Create(x2, y2));
        }

        public static SingleVariablePolynomial FromPoints(params Tuple<double, double>[] points)
        {
            var n = points.Length;

            var mtx = new Matrix(n, n);

            var b = new Matrix(n, 1);


            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    var xi = points[i].Item1;
                    var pow = n - j - 1;

                    mtx[i, j] = pow == 0 ? 1.0 : Math.Pow(xi, pow);
                }

                b[i, 0] = points[i].Item2;
            }

            var coefs = mtx.Solve(b.Values);

            return new SingleVariablePolynomial(coefs);
        }

        public static SingleVariablePolynomial FromPoints(double[] xs, double[] ys)
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
                    var pow = n - j - 1;

                    mtx[i, j] = pow == 0 ? 1.0 : Math.Pow(xi, pow);
                }

                b[i, 0] = ys[i];
            }

            var coefs = mtx.Solve(b.Values);

            return new SingleVariablePolynomial(coefs);
        }

        /// <summary>
        /// coefs= a4 a3 a2 a1
        /// poly = a4 * x^4 + a3 * x^3 + a2 * x^2 + a1 * x^1 + a0 * x^0
        /// </summary>
        /// <param name="coefs"></param>
        public SingleVariablePolynomial(params double[] coefs)
        {
            Coefficients = (double[])coefs.Clone();
        }

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
                var lastNonzero = 0;

                for (var i = 0; i < Coefficients.Length; i++)
                    if (Coefficients[i] != 0)
                        lastNonzero = i;

                return new int[] { lastNonzero };
            }
        }

        public SingleVariablePolynomial Clone()
        {
            var c2 = Coefficients == null ? (double[])null : (double[])Coefficients.Clone();

            var buf = new SingleVariablePolynomial(c2);

            return buf;
        }

        /// <summary>
        /// Evaluates the value o polynomial at specified <see cref="x"/>
        /// </summary>
        /// <param name="x">evaluation point</param>
        /// <returns>P(<see cref="x"/>)</returns>
        private double Evaluate_internal(double x)
        {
            var buf = 0.0;

            for (var i = 0; i < Coefficients.Length; i++)
            {
                var origPow = Coefficients.Length - 1 - i;

                //buf += Coefficients[i] * Math.Pow(x, origPow);
                buf += Coefficients[i] * MyPow(x, origPow);
            }

            return buf;
        }

        static double MyPow(double num, int exp)
        {
            //https://stackoverflow.com/a/936909

            double result = 1.0;
            while (exp > 0)
            {
                if (exp % 2 == 1)
                    result *= num;
                exp >>= 1;
                num *= num;
            }

            return result;
        }

        public Dictionary<int, double> GetCoefficientsAsDictionary()
        {
            var dic = new Dictionary<int, double>();

            for (var i = 0; i < Coefficients.Length; i++)
            {
                var origPow = Coefficients.Length - 1 - i;

                dic[origPow] = Coefficients[i]; 
            }

            return dic;
        }

        //compares the coefficiets, return false if any coef differ more than epsloin
        public bool EqualsPolynomial(SingleVariablePolynomial that,double epsilon)
        {
            var thisDic = this.GetCoefficientsAsDictionary();
            var thatDic = that.GetCoefficientsAsDictionary();

            var allKeys = thisDic.Keys.Union(thatDic.Keys).Distinct().ToList();

            foreach(var key in allKeys)
            {
                var thisValue = 0.0;
                var thatValue = 0.0;

                if (thisDic.ContainsKey(key))
                    thisValue = thisDic[key];

                if (thatDic.ContainsKey(key))
                    thatValue = thatDic[key];

                if (Math.Abs(thisValue - thatValue) > epsilon)
                    return false;
            }

            return true;
        }

        //get max difference between coeficients of two polynomials
        public static double GetMaxDiff(SingleVariablePolynomial @this , SingleVariablePolynomial that)
        {
            var thisDic = @this.GetCoefficientsAsDictionary();
            var thatDic = that.GetCoefficientsAsDictionary();

            var allKeys = thisDic.Keys.Union(thatDic.Keys).Distinct().ToList();

            var max = 0.0;

            foreach (var key in allKeys)
            {
                var thisValue = 0.0;
                var thatValue = 0.0;

                if (thisDic.ContainsKey(key))
                    thisValue = thisDic[key];

                if (thatDic.ContainsKey(key))
                    thatValue = thatDic[key];

                var err = Math.Abs(thisValue - thatValue);
                
                if (err > max)
                    max = err;
            }

            return max;
        }


        /// <summary>
        /// Evaluates the <see cref="n"/>'th integral of this polynomial at defined <see cref="x"/>. assumes the C's of integrals are all zeros.
        /// </summary>
        /// <param name="n">the degree</param>
        /// <param name="x">target point</param>
        /// <returns></returns>
        public double EvaluateNthIntegralAt(int n, double x)
        {
            //n'th integral of a*x^i is : a * x^(i+n)/((i+1)...(i+n)) : coef * x^(i+n)

            if (n == 0)
                return Evaluate_internal(x);

            var buf = 0.0;

            for (var j = 0; j < Coefficients.Length; j++)
            {
                var i = Coefficients.Length - 1 - j;
                var a = Coefficients[i];

                double coef;

                {
                    var f1 = Factorial(i + n);// (i+n)!
                    var f2 = Factorial(i);// (i+1)!

                    var t = f1 / f2;
                    coef = a /t;
                }

                buf += coef * MyPow(x, i + n);
            }

            return buf;
        }


        /// <summary>
        /// Gets the derivative of polynomial as another polynomial
        /// </summary>
        /// <param name="deg">derivation degree</param>
        /// <returns>derivation</returns>
        public SingleVariablePolynomial GetDerivative(int deg)
        {
            if (deg == 0)
                return (SingleVariablePolynomial)this.MemberwiseClone();

            if (Coefficients.Length == 0)
                return this.Clone();

            if (deg >= Coefficients.Length)
                return new SingleVariablePolynomial();

            var dic = new Dictionary<int, double>();

            var buf = new double[Coefficients.Length - deg];

            for (var i = 0; i < Coefficients.Length; i++)
            {
                var origPow = Coefficients.Length - 1 - i;

                var n = origPow;
                var m = deg;

                if (n - m >= 0)
                {
                    var newPow = n - m;

                    dic[newPow] = Factorial(n, n - m) * this.Coefficients[i];
                }
            }

            for (var i = 0; i < buf.Length; i++)
            {
                var origPow = buf.Length - 1 - i;

                buf[i] = dic[origPow];
            }

            var pBuf = new SingleVariablePolynomial(buf);
            return pBuf;
            /**/
        }


        public SingleVariablePolynomial GetIntegral(int deg)
        {
            if (deg == 0)
                return (SingleVariablePolynomial)this.MemberwiseClone();

            if (Coefficients.Length == 0)
                return this.Clone();

            var buf = new double[Coefficients.Length + deg];

            this.Coefficients.CopyTo(buf, 0);

            var n = Coefficients.Length;

            for (int i = 0; i < n; i++)
            {
                var pow = n - i - 1;
                Coefficients[i] = this.Coefficients[i] / Factorial(i + deg, deg);
            }

            return new SingleVariablePolynomial(buf);
            /*

            var dic = new Dictionary<int, double>();


            for (var i = 0; i < Coefficients.Length; i++)
            {
                var cf = this.Coefficients[i];

                if (cf == 0)
                    continue;

                var origPow = Coefficients.Length - 1 - i;

                var n = origPow;
                var m = deg;

                {
                    var newPow = n + m;

                    //dic[newPow] = Factorial(n)/Factorial(n - m) * this.Coefficients[i];
                    dic[newPow] = Factorial(n + m, n) * this.Coefficients[i];

                    //var d = Factorial(n) / Factorial(n - m) - Factorial(n, n - m);
                }
            }

            foreach (var k in dic.Keys)
            {
                var cf = k;
                var v = dic[k];

                buf[i] = dic[origPow];
            }

            for (var i = 0; i < buf.Length; i++)
            {
                var origPow = buf.Length - 1 - i;

                buf[i] = dic[origPow];
            }

            var pBuf = new SingleVariablePolynomial(buf);
            return pBuf;
            /**/
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

            var N = Coefficients.Length;

            for (var i = 0; i < N; i++)
            {
                var origPow = N - 1 - i;

                var nn = origPow;
                var mm = n;

                var nm = nn - mm;

                if (nm >= 0)
                {
                    var cff = Factorial(nn, nm);

                    var cf2 = Coefficients[i];
                    var vari = CalcUtil.Power(x, nm);
                    retVal += vari * cff * cf2;
                }
            }

            return retVal;
        }




        /*
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
        */

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

        /// <summary>
        /// return x!/y! where y < x
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        static long Factorial(int x,int y)
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

        public bool TryFindRoot(double y,out double x)
        {
            var cnt = 0;

            var loc = -1.0;

            while(cnt++<100)
            {
                var yi = this.Evaluate(loc);
                var d =  yi -y;

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

        public override string ToString()
        {
            var sb = new StringBuilder();

            var parts = new List<String>();

            for (var i = 0; i < Coefficients.Length; i++)
            {
                var origPow = Coefficients.Length - 1 - i;

                parts.Add(string.Format(CultureInfo.CurrentCulture, "{0} * x^{1}", Coefficients[i], origPow));
            }

            return string.Join(" + ", parts);
        }

        public string ToString(string doubleFormat)
        {
            var sb = new StringBuilder();

            var parts = new List<String>();

            for (var i = 0; i < Coefficients.Length; i++)
            {
                var origPow = Coefficients.Length - 1 - i;

                parts.Add(string.Format(CultureInfo.CurrentCulture, "{0} * x^{1}", Coefficients[i].ToString(doubleFormat), origPow));
            }

            return string.Join(" + ", parts);
        }

        public string ToCodeString()
        {
            var sb = new StringBuilder();

            var parts = new List<String>();

            var vals = this.Coefficients.SkipWhile(i=>i == 0).ToArray();

            var t = new StringBuilder();

            var ttt = string.Join(",", Coefficients.Select(i => i.ToString()));

            var buf = string.Format("new SingleVariablePolynomial({0});", ttt);

            return buf;
        }

        protected bool Equals(SingleVariablePolynomial other)
        {
            if (ReferenceEquals(Coefficients, null) && ReferenceEquals(other.Coefficients, null))
                return true;

            if (ReferenceEquals(Coefficients, null) || ReferenceEquals(other.Coefficients, null))
                return false;


            if (Coefficients.Length != other.Coefficients.Length)
                return false;

            for (int i = 0; i < Coefficients.Length; i++)
            {
                var d = Math.Abs(Coefficients[i] - other.Coefficients[i]);

                if (d > 1e-10*Math.Abs(Coefficients[i]))
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SingleVariablePolynomial) obj);
        }

        public override int GetHashCode()
        {
            return (Coefficients != null ? Coefficients.GetHashCode() : 0);
        }

        public double Evaluate(params double[] x)
        {
            return Evaluate_internal(x[0]);
        }

        public double[] EvaluateNthDerivative(int n, params double[] x)
        {
            
            throw new NotImplementedException();
        }

        public double[] EvaluateDerivative(params double[] x)
        {
            throw new NotImplementedException();
        }

        public void MultiplyByConstant(double constant)
        {
            for (int i = 0; i < Coefficients.Length; i++)
            {
                Coefficients[i] *= constant;
            }
        }


      
    }
}
