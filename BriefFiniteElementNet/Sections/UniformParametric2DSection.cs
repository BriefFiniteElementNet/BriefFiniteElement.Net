using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Sections
{
    public class UniformParametric2DSection : Base2DSection
    {
        public double T;

        public override double GetThicknessAt(params double[] isoCoords)
        {
            return T;
        }

        public override int[] GetMaxFunctionOrder()
        {
            return new[] {0, 0, 0};
        }
    }
}
