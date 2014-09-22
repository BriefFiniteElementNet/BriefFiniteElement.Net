using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    internal static class TraceUtil
    {
        public static void WritePerformanceTrace(string format, params object[] pars)
        {
            var st = string.Format(format, pars);
            System.Diagnostics.Trace.WriteLine(st,"PerformanceStats");
        }
    }
}
