using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.ElementHelpers
{
    public class TetrahedronHelper : IElementHelper
    {
        /// <inheritdoc/>
        public Matrix GetBMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Matrix GetDMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetRhoMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetMuMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Matrix GetNMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Matrix GetJMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Matrix CalcLocalKMatrix(Element targetElement, Matrix transformMatrix)
        {
            throw new NotImplementedException();
        }

        public Matrix CalcLocalMMatrix(Element targetElement, Matrix transformMatrix)
        {
            throw new NotImplementedException();
        }

        public Matrix CalcLocalCMatrix(Element targetElement, Matrix transformMatrix)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public FluentElementPermuteManager.ElementLocalDof[] GetDofOrder(Element targetElement)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Matrix GetLocalInternalForceAt(Element targetElement, Matrix transformMatrix, Displacement[] globalDisplacements,
            params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool DoesOverrideKMatrixCalculation(Element targetElement, Matrix transformMatrix)
        {
            return false;
        }

        /// <inheritdoc/>
        public int GetNMaxOrder(Element targetElement, Matrix transformMatrix)
        {
            throw new NotImplementedException();
        }

        public int GetBMaxOrder(Element targetElement, Matrix transformMatrix)
        {
            throw new NotImplementedException();
        }

        public int GetDetJOrder(Element targetElement, Matrix transformMatrix)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Displacement GetLocalDisplacementAt(Element targetElement, Matrix transformMatrix, Displacement[] localDisplacements,
            params double[] isoCoords)
        {
            throw new NotImplementedException();
        }
    }
}
