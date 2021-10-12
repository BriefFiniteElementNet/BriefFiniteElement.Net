using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;

namespace BriefFiniteElementNet.Validation
{
    public static class Util
    {
        /// <summary>
        /// Converts one area load applied on a BarElement with a series of equivalent ConcentratedLoads
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="load">The load.</param>
        /// <param name="cse">The cse.</param>
        public static void AreaLoad2ConcentratedLoads(BarElement element, PartialNonUniformLoad load, LoadCase cse)
        {
            var n = 1000;

            var d = element.EndNode.Location - element.StartNode.Location;

            var l = d.Length;

            for (var i = 0; i < n; i++)
            {
                var dx = l / n;

                var x = i * dx + 0.5 * dx;

                var cn = new ConcentratedLoad();

                var xi = (2 * x - l) / l;

                cn.ForceIsoLocation = new IsoPoint(xi, 0, 0);

                var mag = load.GetMagnitudeAt(element, cn.ForceIsoLocation);

                var f = load.Direction * mag * dx;

                cn.Force = new Force(f, Vector.Zero);
                cn.Case = cse;

                cn.CoordinationSystem = load.CoordinationSystem;

                if (mag != 0.0)
                    element.Loads.Add(cn);
            }
        }

        public static string HtmlEncode(this string text)
        {
            char[] chars = System.Web.HttpUtility.HtmlEncode(text).ToCharArray();

            StringBuilder result = new StringBuilder(text.Length + (int)(text.Length * 0.1));

            foreach (char c in chars)
            {
                int value = Convert.ToInt32(c);
                if (value > 127)
                    result.AppendFormat("&#{0};", value);
                else
                    result.Append(c);
            }

            return result.ToString();
        }

        public static double GetErrorPercent(double test, double accurate)
        {
            var buf = Math.Abs(test - accurate) / Math.Abs(accurate);

            return 100 * buf;
        }

        public static double GetAbsError(double test, double accurate)
        {
            var buf = Math.Abs(test - accurate);

            return buf;
        }

        public static double GetErrorPercent(Vector test, Vector accurate)
        {
            if (test == accurate)
                return 0;

            return 100 * Math.Abs((test - accurate).Length) / Math.Max(test.Length, accurate.Length);
        }

        public static double GetAbsError(Vector test, Vector accurate)
        {
            return (test - accurate).Length;
        }


        public static double GetErrorPercent(Displacement test, Displacement accurate)
        {
            if (test == accurate)
                return 0;

            return GetErrorPercent(test.Displacements, accurate.Displacements) +
                   GetErrorPercent(test.Rotations, accurate.Rotations);
        }


        public static double GetErrorPercent(CauchyStressTensor test, CauchyStressTensor accurate)
        {
            var f1t = new Vector(test.S11, test.S12, test.S13);
            var f1a = new Vector(accurate.S11, accurate.S12, accurate.S13);

            var f2t = new Vector(test.S21, test.S22, test.S23);
            var f2a = new Vector(accurate.S21, accurate.S22, accurate.S23);

            var f3t = new Vector(test.S31, test.S32, test.S33);
            var f3a = new Vector(accurate.S31, accurate.S32, accurate.S33);

            var e1 = GetErrorPercent(f1t, f1a);
            var e2 = GetErrorPercent(f2t, f2a);
            var e3 = GetErrorPercent(f3t, f3a);


            return e1 + e2 + e3;
        }

    }
}
