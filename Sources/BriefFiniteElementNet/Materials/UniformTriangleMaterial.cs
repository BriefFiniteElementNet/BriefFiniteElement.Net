using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.Materials
{
    [Obsolete("Use BaseMaterial instead")]
    public class UniformTriangleMaterial : BaseTriangleMaterial
    {
        public double E, Nu;

        public override TriangleCoordinatedMechanicalProperties GetMaterialPropertiesAt(TriangleElement targetElement, params double[] isoCoords)
        {
            var buf = new TriangleCoordinatedMechanicalProperties();
            var mat = new AnisotropicMaterialInfo();

            mat.Ex = mat.Ey = mat.Ez = this.E;

            mat.NuXy = mat.NuYx =
                mat.NuYz = mat.NuZy =
                    mat.NuXz = mat.NuZx =
                        this.Nu;

            buf.Matterial = mat;

            return buf;
        }

        public override int GetMaxFunctionOrder()
        {
            return 0;
        }
    }
}