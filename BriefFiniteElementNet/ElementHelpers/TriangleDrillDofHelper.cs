using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.Common;
using CSparse.Storage;

namespace BriefFiniteElementNet.ElementHelpers
{

    /// <summary>
    /// uses fictious stiffness for drilling dofs
    /// uses values from membrane and plate bending stifness parameters
    /// </summary>
    public class TriangleDrillDofHelper: IElementHelper
    {
        public double Scale = 2e-3;

        public Element TargetElement { get; set; }

        public Matrix CalcLocalDampMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        private static double Max(params double[] items)
        {
            return items.Max();
        }


        public Matrix CalcLocalStiffnessMatrix(Element targetElement)
        {
            var mmb = new CstHelper().CalcLocalStiffnessMatrix(targetElement);

            var bnd = new DktHelper().CalcLocalStiffnessMatrix(targetElement);

            var buf = new Matrix(3, 3);


            if(false)
            {
                //eq 5.2, p71 (80/175), thesis pdf

                buf[0, 0] = Max(mmb[0, 0], mmb[1, 1], bnd[0, 0], bnd[1, 1], bnd[2, 2]);
                buf[1, 1] = Max(mmb[2, 2], mmb[3, 3], bnd[3, 3], bnd[4, 4], bnd[5, 5]);
                buf[2, 2] = Max(mmb[4, 4], mmb[5, 5], bnd[6, 6], bnd[7, 7], bnd[8, 8]);

                return buf;

            }
            else
            {
                //this formulation give much closer value to abaqus
                var max = double.MinValue;//maximum on diagonal

                for (var i = 0; i < mmb.RowCount; i++)
                    if (mmb[i, i] > max)
                        max = mmb[i, i];

                for (var i = 0; i < bnd.RowCount; i++)
                    if (bnd[i, i] > max)
                        max = bnd[i, i];


                var buf2 = Matrix.Eye(3);

                buf2.Scale(max * Scale);

                return buf2;
            }

            

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

        public GeneralStressTensor GetLocalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public GeneralStressTensor GetLoadStressAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public GeneralStressTensor GetLocalInternalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            return new GeneralStressTensor();
        }

        GeneralStressTensor IElementHelper.GetLocalInternalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            return new GeneralStressTensor();
            throw new NotImplementedException();
        }

        GeneralStressTensor IElementHelper.GetLoadStressAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }
    }
}
