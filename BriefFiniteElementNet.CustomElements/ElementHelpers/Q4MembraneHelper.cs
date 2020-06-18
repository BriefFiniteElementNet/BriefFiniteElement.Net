using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.ElementHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements.ElementHelpers
{
    class Q4MembraneHelper : IElementHelper
    {
        public Element TargetElement { get; set; }

        public Matrix CalcLocalDampMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix CalcLocalMassMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix CalcLocalStiffnessMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public int[] GetBMaxOrder(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public int[] GetDetJOrder(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix GetDMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var tri = targetElement as TriangleElement;

            if (tri == null)
                throw new Exception();

            var d = new Matrix(3, 3);

            var mat = tri.Material.GetMaterialPropertiesAt(isoCoords);


            if (tri.MembraneFormulation == MembraneFormulation.PlaneStress)
            {
                //http://help.solidworks.com/2013/english/SolidWorks/cworks/c_linear_elastic_orthotropic.htm
                //orthotropic material
                d[0, 0] = mat.Ex / (1 - mat.NuXy * mat.NuYx);
                d[1, 1] = mat.Ey / (1 - mat.NuXy * mat.NuYx);
                d[1, 0] = d[0, 1] = mat.NuXy * mat.Ey / (1 - mat.NuXy * mat.NuYx);

                d[2, 2] = mat.Ex * mat.Ey / (mat.Ex + mat.Ey + 2 * mat.Ey * mat.NuXy);
            }
            else
            {
                var delta = 1 - mat.NuXy * mat.NuYx - mat.NuZy * mat.NuYz - mat.NuZx * mat.NuXz - 2 * mat.NuXy * mat.NuYz * mat.NuZx;

                delta /= mat.Ex * mat.Ey * mat.Ez;

                //http://www.efunda.com/formulae/solid_mechanics/mat_mechanics/hooke_orthotropic.cfm

                d[0, 0] = (1 - mat.NuYz * mat.NuZy) / (mat.Ey * mat.Ez * delta);
                d[0, 1] = (mat.NuYx + mat.NuZx * mat.NuYz) / (mat.Ey * mat.Ez * delta);
                d[1, 0] = (mat.NuXy + mat.NuXz * mat.NuZy) / (mat.Ez * mat.Ex * delta);
                d[1, 1] = (1 - mat.NuZx * mat.NuXz) / (mat.Ez * mat.Ex * delta);
            }

            return d;

        }

        public ElementPermuteHelper.ElementLocalDof[] GetDofOrder(Element targetElement)
        {
            var buf = new ElementPermuteHelper.ElementLocalDof[]
            {
                new ElementPermuteHelper.ElementLocalDof(0, DoF.Dx),
                new ElementPermuteHelper.ElementLocalDof(0, DoF.Dy),

                new ElementPermuteHelper.ElementLocalDof(1, DoF.Dx),
                new ElementPermuteHelper.ElementLocalDof(1, DoF.Dy),

                new ElementPermuteHelper.ElementLocalDof(2, DoF.Dx),
                new ElementPermuteHelper.ElementLocalDof(2, DoF.Dy),

                new ElementPermuteHelper.ElementLocalDof(3, DoF.Dx),
                new ElementPermuteHelper.ElementLocalDof(3, DoF.Dy),
            };

            return buf;
        }

        public Matrix GetJMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Displacement GetLoadDisplacementAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<DoF, double>> GetLoadInternalForceAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public GeneralStressTensor GetLoadStressAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public Displacement GetLocalDisplacementAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Force[] GetLocalEquivalentNodalLoads(Element targetElement, ElementalLoad load)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<DoF, double>> GetLocalInternalForceAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public GeneralStressTensor GetLocalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetMuMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public int[] GetNMaxOrder(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix GetRhoMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }
    }
}
