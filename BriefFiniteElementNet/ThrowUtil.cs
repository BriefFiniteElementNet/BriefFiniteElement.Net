using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    public static class ThrowUtil
    {
        public static void ThrowIf(bool val, string format, params object[] param)
        {
            if (!val)
                return;

            var msg = string.Format(format, param);

            throw new BriefFiniteElementNetException(msg);
        }

    }
}
