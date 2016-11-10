using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    public class ParametricBarElementCrossSection : BaseBarElementCrossSection
    {
        public override BarCrossSectionGeometricProperties GetCrossSectionPropertiesAt(double x)
        {
            throw new NotImplementedException();
        }

        public override double[] GetIntegrationPoints()
        {
            throw new NotImplementedException();
        }
    }
}
