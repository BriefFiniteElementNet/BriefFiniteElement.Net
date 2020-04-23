using BriefFiniteElementNet.Elements;
using CSparse.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Common;

namespace BriefFiniteElementNet.ElementHelpers
{
    public class TriangleBasicDrillingDofHelper:IElementHelper
    {
        public Element TargetElement { get; set; }

        public Matrix CalcLocalDampMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix CalcLocalStiffnessMatrix(Element targetElement)
        {
            var buf = Matrix.Eye(4);

            buf.MultiplyByConstant(1e3);

            return buf;

        }

        public Matrix CalcLocalMassMatrix(Element targetElement)
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

        public Matrix GetB_iMatrixAt(Element targetElement, int i, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public int[] GetDetJOrder(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix GetDMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public ElementPermuteHelper.ElementLocalDof[] GetDofOrder(Element targetElement)
        {
            return new ElementPermuteHelper.ElementLocalDof[]
            {
                new ElementPermuteHelper.ElementLocalDof(0, DoF.Rz),

                new ElementPermuteHelper.ElementLocalDof(1, DoF.Rz),

                new ElementPermuteHelper.ElementLocalDof(2, DoF.Rz),
            };
        }

        public Force[] GetLocalEquivalentNodalLoads(Element targetElement, ElementalLoad load)
        {
            throw new NotImplementedException();
        }

        public Matrix GetJMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Displacement GetLoadDisplacementAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<DoF, double>> GetLoadInternalForceAt(Element targetElement, ElementalLoad load,
            double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public Displacement GetLocalDisplacementAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<DoF, double>> GetLocalInternalForceAt(Element targetElement,
            Displacement[] globalDisplacements, params double[] isoCoords)
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

        public void AddStiffnessComponents(CoordinateStorage<double> global)
        {
            throw new NotImplementedException();
        }
    }
}
