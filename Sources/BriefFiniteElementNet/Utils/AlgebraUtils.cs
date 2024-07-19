using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Utils
{
    public static class AlgebraUtils
    {
        // TODO: EXTENSION - move to Extensions class?
        public static void AddToSelf(this double[] vector, double[] addition, double coef = 1)
        {
            if (vector.Length != addition.Length)
                throw new Exception();

            for (var i = 0; i < vector.Length; i++)
            {
                vector[i] += addition[i] * coef;
            }
        }

        public static double[] Add(double[] a, double[] b)
        {
            return a.Add(b);
        }

        public static double[] Subtract(double[] a, double[] b)
        {
            return a.Subtract(b);
        }

        /// <summary>
        /// a*x+b*y=f
        /// c*x+d*y=g
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        /// <param name="i"></param>
        public static void Solve2x2(double a, double b, double f, double c, double d, double g, out double h, out double i)
        {
            var det = a * d - b * c;
            CalcUtil.Swap(ref a, ref d);
            b = -b;
            c = -c;


            h = a * f + b * g;
            i = c * f + d * g;

            h = h / det;
            i = i / det;
        }



        /// <summary>
        /// gets the norm(2) of difference of two vectors
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public static double GetDiffNorm2(double[] a1, double[] a2)
        {
            var l = Math.Max(a1.Length, a2.Length);
            var diff = new double[l];

            for (var i = 0; i < l; i++)
            {
                if (i >= a1.Length || i >= a2.Length)
                    break;

                diff[i] = a1[i] - a2[i];
            }

            for (var i = 0; i < l; i++)
            {
                diff[i] *= diff[i];
            }

            return Norm2(diff);
        }

        public static double Norm2(params double[] array)
        {
            var t = array.Select(i => i * i).OrderBy(i => i).Sum();
            return Math.Sqrt(t);
        }

        public static double NormInf(params double[] array)
        {
            var t = array.Max(i => Math.Abs(i));
            return t;
        }

        /// <summary>
        /// gets the norm(infinity) of difference of two vectors
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public static double GetDiffNormInf(double[] a1, double[] a2)
        {
            var l = Math.Max(a1.Length, a2.Length);
            var diff = new double[l];

            for (var i = 0; i < l; i++)
            {
                if (i >= a1.Length || i >= a2.Length)
                    break;

                diff[i] = a1[i] - a2[i];
            }

            return NormInf(diff);
        }



        /// <summary>
        /// calculates the dot product of two same length vectors
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double DotProduct(double[] v1, double[] v2)
        {
            ThrowUtil.ThrowIf(v1.Length != v2.Length, "inconsistent sizes");

            var buf = 0.0;

            for (var i = 0; i < v1.Length; i++)
                buf += v1[i] * v2[i];

            return buf;
        }
    }
}
