using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents the mechanical properties of triangle element in specific point
    /// </summary>
    [Obsolete]
    public class TriangleCoordinatedMechanicalProperties
    {
        private AnisotropicMaterialInfo mat;

        public AnisotropicMaterialInfo Matterial
        {
            get { return mat; }
            set { mat = value; }
        }
    }
}
