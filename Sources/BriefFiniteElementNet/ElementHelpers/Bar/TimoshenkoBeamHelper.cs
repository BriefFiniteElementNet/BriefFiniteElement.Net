using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Mathh;
using CSparse.Storage;

namespace BriefFiniteElementNet.ElementHelpers
{
    /// <summary>
    /// Represents a helper class for Timoshenko beam element.
    /// </summary>
    public class TimoshenkoBeamHelper : IElementHelper
    {
        #region statics

        /// <summary>
        /// get the shape functions that defines the Y (shape of beam)
        /// </summary>
        /// <param name="xi"></param>
        /// <param name="l"></param>
        /// <param name="phi"></param>
        /// <returns></returns>
        public static void GetYShapeFunctions(double xi, double l, double phi, BeamDirection dir,out SingleVariablePolynomial[] nss,out SingleVariablePolynomial[] mss)
        {
            var phi_h = 1 / (1 + phi);

            var n1 = new SingleVariablePolynomial(phi_h / 4, 0, -phi * phi_h / 2 - 3 * phi_h / 4, phi * phi_h / 2 + phi_h / 2);
            var n2 = new SingleVariablePolynomial(-phi_h / 4, 0, phi * phi_h / 2 + 3 * phi_h / 4, phi * phi_h / 2 + phi_h / 2);

            var m1 = new SingleVariablePolynomial(l * phi_h / 8, -l * phi * phi_h / 8 - l * phi_h / 8, -l * phi_h / 8, l * phi * phi_h / 8 + l * phi_h / 8);
            var m2 = new SingleVariablePolynomial(l * phi_h / 8, l * phi * phi_h / 8 + l * phi_h / 8, -l * phi_h / 8, -l * phi * phi_h / 8 - l * phi_h / 8);

            m1.MultiplyByConstant(l);
            m2.MultiplyByConstant(l);

            nss = new[] { n1, n2 };
            mss = new[] { m1, m2 };
        }

        /// <summary>
        /// get the shape functions that defines the Y' (rotation of beam)
        /// </summary>
        /// <param name="xi"></param>
        /// <param name="l"></param>
        /// <param name="phi"></param>
        /// <returns></returns>
        public static void GetThetaShapeFunctions(double xi, double l, double phi, BeamDirection dir, out SingleVariablePolynomial[] nss, out SingleVariablePolynomial[] mss)
        {
            var phi_h = 1 / (1 + phi);

            var n1p = new SingleVariablePolynomial(3 * phi_h / (2 * l), 0, -3 * phi_h / (2 * l));
            var n2p = new SingleVariablePolynomial(-3 * phi_h / (2 * l), 0, 3 * phi_h / (2 * l));

            var m1p = new SingleVariablePolynomial(3 * phi_h / 4, -phi * phi_h / 2 - phi_h / 2, phi * phi_h / 2 - phi_h / 4);
            var m2p = new SingleVariablePolynomial(3 * phi_h / 4, +phi * phi_h / 2 + phi_h / 2, phi * phi_h / 2 - phi_h / 4);

            nss = new[] { n1p, n2p };
            mss = new[] { m1p, m2p };
        }

        #endregion

        private void EnsureNoPartialEndRelease(Element elm) 
        {
            var bar = elm as BarElement;

            var cnss = bar.NodalReleaseConditions;

            var dofs = cnss.SelectMany(i => new[] { i.DY, i.DZ, i.RY, i.RZ }).ToArray();

            if (dofs.Any(i => i != DofConstraint.Fixed))
                throw new Exception("TimoshenkoBeam not support partial end release yet");
        }

        public static Matrix GetNMatrixAt(double xi, double l,double phi, DofConstraint D0, DofConstraint R0, DofConstraint D1, DofConstraint R1, BeamDirection dir)
        {
            var n1 = new SingleVariablePolynomial();


            SingleVariablePolynomial[] nss, mss;

            EulerBernouly2NodeShapeFunction.GetShapeFunctions(l, D0, R0, D1, R1, dir, out nss, out mss);

            var buf = new Matrix(4, 4);

            for (var i = 0; i < 2; i++)
            {
                if (nss[i] == null) nss[i] = new SingleVariablePolynomial();
                if (mss[i] == null) mss[i] = new SingleVariablePolynomial();
            }

            for (var i = 0; i < 2; i++)
            {
                var ni = nss[i];
                var mi = mss[i];

                for (var ii = 0; ii < 4; ii++)
                {
                    buf[ii, 2 * i + 0] = ni.EvaluateDerivative(xi, ii);
                    buf[ii, 2 * i + 1] = mi.EvaluateDerivative(xi, ii);
                }
            }


            return buf;
        }

        private BeamDirection _direction;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimoshenkoBeamHelper"/> class.
        /// </summary>
        /// <param name="direction">The direction.</param>
        public TimoshenkoBeamHelper(BeamDirection direction)
        {
            _direction = direction;
        }


        public Element TargetElement { get; set; }

        /// <inheritdoc/>
        public Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Matrix GetJMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Matrix CalcLocalStiffnessMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix CalcLocalMassMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix CalcLocalDampMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ElementPermuteHelper.ElementLocalDof[] GetDofOrder(Element targetElement)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerable<Tuple<DoF, double>> GetLocalInternalForceAt(Element targetElement,
            Displacement[] globalDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool DoesOverrideKMatrixCalculation(Element targetElement, Matrix transformMatrix)
        {
            return false;
        }

        /// <inheritdoc/>
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

        public IEnumerable<Tuple<DoF, double>> GetLoadInternalForceAt(Element targetElement, ElementalLoad load,
            double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public Displacement GetLoadDisplacementAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Displacement GetLocalDisplacementAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Force[] GetLocalEquivalentNodalLoads(Element targetElement, ElementalLoad load)
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
            throw new NotImplementedException();
        }
    }
}
