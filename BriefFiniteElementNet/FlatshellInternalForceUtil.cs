using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Geometry;

namespace BriefFiniteElementNet
{
    public static class FlatshellInternalForceUtil
    {
        public static double GetTotalShearForceAlongIntersectionWithPlane(IEnumerable<TriangleFlatShell> elements,
            Plane plane)
        {

            var sampling = 5;

            foreach (var element in elements)
            {
                var points = element.GetIntersectionPoints(plane, sampling);

                
            }
            throw new NotImplementedException();
        }
    }
}
