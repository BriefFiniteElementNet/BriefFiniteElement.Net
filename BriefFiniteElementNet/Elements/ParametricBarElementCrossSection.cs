using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    public class ParametricBarElementCrossSection : BaseBarElemenetCrossSection
    {
        public override void GetCrossSectionPropertiesAt(double x, out double A, out double Iy)
        {
            throw new NotImplementedException();
        }

        public override double[] GetIntegrationPoints()
        {
            throw new NotImplementedException();
        }
    }
}
