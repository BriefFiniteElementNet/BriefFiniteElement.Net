using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Sections
{
    public abstract class Base2DSection
    {
        public abstract double GetThicknessAt(params double[] isoCoords);

        public abstract int[] GetMaxFunctionOrder();
    }
}
