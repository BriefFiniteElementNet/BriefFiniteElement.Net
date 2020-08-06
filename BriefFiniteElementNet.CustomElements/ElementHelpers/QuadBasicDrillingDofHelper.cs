//---------------------------------------------------------------------------------------
//
// Project: VIT-V
//
// Program: BriefFiniteElement.Net - QuadBasicDrillingDofHelper.cs
//
// Revision History
//
// Date          Author          	            Description
// 24.06.2020    T.Thaler, M.Mischke     	    v1.0  
// 
//---------------------------------------------------------------------------------------
// Copyleft 2017-2020 by Brandenburg University of Technology. Intellectual proprerty 
// rights reserved in terms of LGPL license. This work is a research work of Brandenburg
// University of Technology. Use or copying requires an indication of the authors reference.
//---------------------------------------------------------------------------------------

using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.ElementHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements.ElementHelpers 
{
    public class QuadBasicDrillingDofHelper : IElementHelper    // based on the BFE.Net TriangleBasicDrillingDofHelper-Class
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
            var buf = Matrix.Eye(4);
            buf.MultiplyByConstant(1e3);
            return buf;
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
            throw new NotImplementedException();
        }

        public ElementPermuteHelper.ElementLocalDof[] GetDofOrder(Element targetElement)
        {
            return new ElementPermuteHelper.ElementLocalDof[]
            {
                new ElementPermuteHelper.ElementLocalDof(0, DoF.Rz),

                new ElementPermuteHelper.ElementLocalDof(1, DoF.Rz),

                new ElementPermuteHelper.ElementLocalDof(2, DoF.Rz),

                new ElementPermuteHelper.ElementLocalDof(3, DoF.Rz),
            };
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

        public GeneralStressTensor GetLocalInternalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            return new GeneralStressTensor();
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
