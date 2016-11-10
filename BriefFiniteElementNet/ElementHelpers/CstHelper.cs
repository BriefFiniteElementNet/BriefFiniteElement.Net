using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.ElementHelpers
{
    public class CstHelper : IElementHelper
    {
        public Matrix GetBMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetDMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetNMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetJMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetKMatrix(Element targetElement, Matrix transformMatrix)
        {
            throw new NotImplementedException();
        }

        public FluentElementPermuteManager.ElementLocalDof[] GetDofOrder(Element targetElement)
        {
            throw new NotImplementedException();
        }
    }
}
