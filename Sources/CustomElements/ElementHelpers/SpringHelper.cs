using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Elements.Elements;

namespace BriefFiniteElementNet.Elements.ElementHelpers
{
    public class ParametricSpringHelper:IElementHelper
    {
        public Element TargetElement { get; set; }

        public DoF SpringType;


        public Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetDMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetRhoMatrixAt(Element targetElement, params double[] isoCoords)
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

        public Matrix GetJMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix CalcLocalStiffnessMatrix(Element targetElement)
        {
            var t = targetElement as ParametricSpring;
            var buf = new Matrix(2, 2);

            var k = 0.0;

            switch (SpringType)
            {
                case DoF.Dx:
                    k = t.KDx;
                    break;
                case DoF.Dy:
                    k = t.KDy;
                    break;
                case DoF.Dz:
                    k = t.KDz;
                    break;
                case DoF.Rx:
                    k = t.KRx;
                    break;
                case DoF.Ry:
                    k = t.KRy;
                    break;
                case DoF.Rz:
                    k = t.KRz;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            buf[0, 0] = buf[1, 1] = k;
            buf[0, 1] = buf[1, 0] = -k;

            return buf;
        }

        public Matrix CalcLocalMassMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix CalcLocalDampMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public ElementPermuteHelper.ElementLocalDof[] GetDofOrder(Element targetElement)
        {
            return new ElementPermuteHelper.ElementLocalDof[]
            {
                new ElementPermuteHelper.ElementLocalDof(0, SpringType),
                new ElementPermuteHelper.ElementLocalDof(1, SpringType),
            };
        }

        public int[] GetNMaxOrder(Element targetElement)
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

        public IEnumerable<Tuple<DoF, double>> GetLocalInternalForceAt(Element targetElement, Displacement[] localDisplacements,
            params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public GeneralStressTensor GetLocalInternalStressAt(Element targetElement, Displacement[] localDisplacements,
            params double[] isoCoords)
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

        public Displacement GetLocalDisplacementAt(Element targetElement, Displacement[] localDisplacements,
            params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Displacement GetLoadDisplacementAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public Force[] GetLocalEquivalentNodalLoads(Element targetElement, ElementalLoad load)
        {
            throw new NotImplementedException();
        }

        public ILoadHandler[] GetLoadHandlers()
        {
            throw new NotImplementedException();
        }
    }
}
