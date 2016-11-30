using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.Materials
{
    public class UniformTriangleMaterial : BaseTriangleMaterial
    {
        public double E, Nu;

        public override TriangleCoordinatedMechanicalProperties GetMaterialPropertiesAt(TriangleElement targetElement, params double[] isoCoords)
        {
            var buf = new TriangleCoordinatedMechanicalProperties();
            var mat = new OrthotropicMaterial();

            mat.Ex = mat.Ey = mat.Ez = this.E;

            mat.nu_xy = mat.nu_yx =
                mat.nu_yz = mat.nu_zy =
                    mat.nu_xz = mat.nu_zx = this.Nu;

            buf.Matterial = mat;

            return buf;
        }

        public override int GetMaxFunctionOrder()
        {
            return 0;
        }
    }
}