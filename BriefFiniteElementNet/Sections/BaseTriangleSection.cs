using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Sections
{
    public abstract class BaseTriangleSection
    {
        public abstract double GetThicknessAt(params double[] isoCoords);

        public abstract int GetMaxFunctionOrder();
    }
}
