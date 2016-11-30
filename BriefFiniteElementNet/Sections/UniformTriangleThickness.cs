using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Sections
{
    public class UniformTriangleThickness : BaseTriangleSection
    {
        public double T;

        public override double GetThicknessAt(params double[] isoCoords)
        {
            return T;
        }

        public override int GetMaxFunctionOrder()
        {
            return 0;
        }
    }
}
