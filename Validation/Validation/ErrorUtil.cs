using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation
{
    public static class ErrorUtil
    {
        public static double GetRelativeError<T>(T refrence,T approximate,Func<T,double[]> selector)
        {
            var exact = selector(refrence);
            var approx = selector(approximate);

            var dn2 = Utils.AlgebraUtils.GetDiffNorm2(exact, approx);
            var en2 = Utils.AlgebraUtils.Norm2(exact);

            return dn2 / en2;
        }
    }
}
