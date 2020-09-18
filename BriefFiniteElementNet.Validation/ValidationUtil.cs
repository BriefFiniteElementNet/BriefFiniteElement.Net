using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation
{
    public static class Util
    {
        public static double GetErrorPercent(double test, double accurate)
        {
            var buf = Math.Abs(test - accurate) / Math.Abs(accurate);

            return 100 * buf;
        }

        public static double GetErrorPercent(Vector test, Vector accurate)
        {
            if (test == accurate)
                return 0;

            return 100 * Math.Abs((test - accurate).Length) / Math.Max(test.Length, accurate.Length);
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
