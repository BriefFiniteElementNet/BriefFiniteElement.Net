using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CSparse.Double.Factorization;
using CSparse.Storage;

namespace BriefFiniteElementNet
{
    public static class ReflectionUtils
    {
       public static object GetFactorR(SparseQR qr, string field)
        {
            var info = typeof(SparseQR).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic);

            return info.GetValue(qr);// as CompressedColumnStorage<double>;
        }
    }
}
