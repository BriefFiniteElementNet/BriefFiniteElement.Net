using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;

namespace BriefFiniteElementNet.ElementHelpers
{
    /// <summary>
    /// Represents a helper class for Euler - Bernoulli beam element
    /// </summary>
    public class EulerBernoulliBeamHelper : IElementHelper
    {
        private BeamDirection _direction;

        /// <summary>
        /// Initializes a new instance of the <see cref="EulerBernoulliBeamHelper"/> class.
        /// </summary>
        /// <param name="direction">The direction.</param>
        public EulerBernoulliBeamHelper(BeamDirection direction)
        {
            _direction = direction;
        }

        /// <inheritdoc/>
        public Matrix GetBMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords)
        {
            //TODO: Take end supports into consideration

            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var xi = isoCoords[0];

            if (xi < -1 || xi > 1)
                throw new ArgumentOutOfRangeException(nameof(isoCoords));

            var L = (elm.EndNode.Location - elm.StartNode.Location).Length;

            var L2 = L*L;

            var buf = new Matrix(1, 4);

            double[] arr;

            if (_direction == BeamDirection.Z)
                arr = new double[] {-(6*xi)/L2, (3*xi)/L - 1/L, +(6*xi)/L2, (3*xi)/L + 1/L};
            else
                arr = new double[] {(6*xi)/L2, (3*xi)/L - 1/L, -(6*xi)/L2, (3*xi)/L + 1/L};

            buf.FillRow(0, arr);

            return buf;
        }

        /// <inheritdoc/>
        public Matrix GetDMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var xi = isoCoords[0];

            var geo = elm.Section.GetCrossSectionPropertiesAt(xi);
            var mech = elm.Material.GetMaterialPropertiesAt(xi);

            var buf = new Matrix(1, 1);

            var ei = 0.0;

            if (_direction == BeamDirection.Y)
                ei = geo.Iz*mech.E;
            else
                ei = geo.Iy * mech.E;

            buf[0, 0] = ei;

            return buf;
        }

        /// <inheritdoc/>
        public Matrix GetNMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Matrix GetJMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords)
        {
            var bar = targetElement as BarElement;

            if (bar == null)
                throw new Exception();

            var buf = new Matrix(1, 1);

            buf[0, 0] = (bar.EndNode.Location - bar.StartNode.Location).Length/2;

            return buf;
        }

        public Matrix GetInternalForceAt(Element targetElement, Matrix transformMatrix, Displacement[] globalDisplacements,
            params double[] isoCoords)
        {

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Matrix GetOverridedLocalKMatrix(Element targetElement, Matrix transformMatrix)
        {
            return null;
        }

        /// <inheritdoc/>
        public FluentElementPermuteManager.ElementLocalDof[] GetDofOrder(Element targetElement)
        {
            return new FluentElementPermuteManager.ElementLocalDof[]
            {
                new FluentElementPermuteManager.ElementLocalDof(0, _direction == BeamDirection.Y ? DoF.Dy : DoF.Dz),
                new FluentElementPermuteManager.ElementLocalDof(0, _direction == BeamDirection.Y ? DoF.Rz : DoF.Ry),
                new FluentElementPermuteManager.ElementLocalDof(1, _direction == BeamDirection.Y ? DoF.Dy : DoF.Dz),
                new FluentElementPermuteManager.ElementLocalDof(1, _direction == BeamDirection.Y ? DoF.Rz : DoF.Ry),
            };
        }

        /// <inheritdoc/>
        public bool DoesOverrideKMatrixCalculation(Element targetElement, Matrix transformMatrix)
        {
            return false;
        }

        /// <inheritdoc/>
        public int GetGaussianIntegrationPointCount(Element targetElement, Matrix transformMatrix)
        {
            return 1;
        }
    }
}
