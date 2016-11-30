using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;
using ElementLocalDof = BriefFiniteElementNet.FluentElementPermuteManager.ElementLocalDof;

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

            ElementHelperExtensions.RemoveReleasedMembers_bar(this, targetElement, buf);

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
        public Matrix GetRhoMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var xi = isoCoords[0];

            var geo = elm.Section.GetCrossSectionPropertiesAt(xi);
            var mech = elm.Material.GetMaterialPropertiesAt(xi);

            var buf = new Matrix(1, 1);

            buf[0, 0] = geo.A*mech.Density;

            return buf;
        }

        /// <inheritdoc/>
        public Matrix GetMuMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var xi = isoCoords[0];

            var geo = elm.Section.GetCrossSectionPropertiesAt(xi);
            var mech = elm.Material.GetMaterialPropertiesAt(xi);

            var buf = new Matrix(1, 1);

            buf[0, 0] = geo.A * mech.Mu;

            return buf;
        }

        /// <inheritdoc/>
        public Matrix GetNMatrixAt(Element targetElement, Matrix transformMatrix, params double[] isoCoords)
        {
            var xi = isoCoords[0];

            if (xi < -1 || xi > 1)
                throw new ArgumentOutOfRangeException(nameof(isoCoords));

            var bar = targetElement as BarElement;

            if (bar == null)
                throw new Exception();

            var L = (bar.EndNode.Location - bar.StartNode.Location).Length;

            var n1 = 1/4.0*(1 - xi)*(1 - xi)*(2 + xi); //[slope(-1) = slope(1) = val(1) = 0, val(-1) = 1]
            var m1 = L/8.0*(1 - xi)*(1 - xi)*(xi + 1); //[slope(-1) = 1, slope(1) = val(1) = val(-1) = 0]

            var n2 = 1/4.0*(1 + xi)*(1 + xi)*(2 - xi); //[val(1) = 1, slope(-1) = slope(1) = val(-1) = 0]
            var m2 = L/8.0*(1 + xi)*(1 + xi)*(xi - 1); //[slope(1) = 1, slope(-1) = val(1) = val(-1) = 0]


            var buf = new Matrix(1, 4);

            double[] arr;

            if (_direction == BeamDirection.Z)
                arr = new double[] {n1, -m1, n2, -m2};
            else
                arr = new double[] {n1, m1, n2, m2};

            buf.FillRow(0, arr);


            ElementHelperExtensions.RemoveReleasedMembers_bar(this, targetElement, buf);

            return buf;
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

        public Matrix GetLocalInternalForceAt(Element targetElement, Matrix transformMatrix, Displacement[] globalDisplacements,
            params double[] isoCoords)
        {

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Matrix CalcLocalKMatrix(Element targetElement, Matrix transformMatrix)
        {
            var buf = ElementHelperExtensions.CalcLocalKMatrix_Bar(this, targetElement,
                transformMatrix);

            return buf;
        }

        /// <inheritdoc/>
        public Matrix CalcLocalMMatrix(Element targetElement, Matrix transformMatrix)
        {
            var buf = ElementHelperExtensions.CalcLocalMMatrix_Bar(this, targetElement,
                transformMatrix);

            return buf;
        }

        /// <inheritdoc/>
        public Matrix CalcLocalCMatrix(Element targetElement, Matrix transformMatrix)
        {
            return ElementHelperExtensions.CalcLocalCMatrix_Bar(this, targetElement,
                 transformMatrix);
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
        public int GetNMaxOrder(Element targetElement, Matrix transformMatrix)
        {
            return 3;
        }

        public int GetBMaxOrder(Element targetElement, Matrix transformMatrix)
        {
            return 1;
        }

        public int GetDetJOrder(Element targetElement, Matrix transformMatrix)
        {
            return 0;
        }

        /// <inheritdoc/>
        public Displacement GetLocalDisplacementAt(Element targetElement, Matrix transformMatrix, Displacement[] localDisplacements,
            params double[] isoCoords)
        {
            throw new NotImplementedException();
        }
    }
}
