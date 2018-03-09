using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;
using BriefFiniteElementNet.Loads;
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

        public Element TargetElement { get; set; }

        /// <inheritdoc/>
        public Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
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
        public Matrix GetB_iMatrixAt(Element targetElement, int i, params double[] isoCoords)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var xi = isoCoords[0];

            if (xi < -1 || xi > 1)
                throw new ArgumentOutOfRangeException(nameof(isoCoords));

            var L = (elm.EndNode.Location - elm.StartNode.Location).Length;

            var L2 = L * L;

            double bufVal;

            switch(i)
            {
                case 0:
                    bufVal = _direction == BeamDirection.Z ? -(6 * xi) / L2 : -(6 * xi) / L2;
                    break;

                case 1:
                    bufVal = (3 * xi) / L - 1 / L;
                    break;

                case 2:
                    bufVal = _direction == BeamDirection.Z ? +(6 * xi) / L2 : -(6 * xi) / L2;
                    break;

                case 3:
                    bufVal = (3 * xi) / L + 1 / L;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            var buf = new Matrix(1, 1);

            buf.SetMember(0, 0, bufVal);

            return buf;
        }

        /// <inheritdoc/>
        public Matrix GetDMatrixAt(Element targetElement, params double[] isoCoords)
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
                ei = geo.Iy * mech.Ex;
            else
                ei = geo.Iz * mech.Ex;

            buf[0, 0] = ei;

            return buf;
        }

        /// <inheritdoc/>
        public Matrix GetRhoMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var xi = isoCoords[0];

            var geo = elm.Section.GetCrossSectionPropertiesAt(xi);
            var mech = elm.Material.GetMaterialPropertiesAt(xi);

            var buf = new Matrix(1, 1);

            buf[0, 0] = geo.A*mech.Rho;

            return buf;
        }

        /// <inheritdoc/>
        public Matrix GetMuMatrixAt(Element targetElement, params double[] isoCoords)
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
        public Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords)
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

            return buf;
        }

        /// <inheritdoc/>
        public Matrix GetJMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var bar = targetElement as BarElement;

            if (bar == null)
                throw new Exception();

            var buf = new Matrix(1, 1);

            buf[0, 0] = (bar.EndNode.Location - bar.StartNode.Location).Length/2;

            return buf;
        }



        /// <inheritdoc/>
        public Matrix CalcLocalKMatrix(Element targetElement)
        {
            var buf = ElementHelperExtensions.CalcLocalKMatrix_Bar(this, targetElement);

            return buf;
        }

        /// <inheritdoc/>
        public Matrix CalcLocalMMatrix(Element targetElement)
        {
            var buf = ElementHelperExtensions.CalcLocalMMatrix_Bar(this, targetElement);

            return buf;
        }

        /// <inheritdoc/>
        public Matrix CalcLocalCMatrix(Element targetElement)
        {
            return ElementHelperExtensions.CalcLocalCMatrix_Bar(this, targetElement);
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
        public int[] GetNMaxOrder(Element targetElement)
        {
            return new int[] {3, 0, 0};
        }

        public int[] GetBMaxOrder(Element targetElement)
        {
            return new[] {1,0,0};
        }

        public int[] GetDetJOrder(Element targetElement)
        {
            return new int[] {0, 0, 0};
        }


        /// <inheritdoc/>
        public FlatShellStressTensor GetLoadInternalForceAt(Element targetElement, Load load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public FlatShellStressTensor GetLoadDisplacementAt(Element targetElement, Load load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        
        /// <inheritdoc/>
        public Displacement GetLocalDisplacementAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            var nodalDisps = new Matrix(1, 4);

            var ld = localDisplacements;

            if (_direction == BeamDirection.Y)
                nodalDisps.FillColumn(0, ld[0].DY, ld[0].RZ, ld[1].DY, ld[1].RZ);
            else
                nodalDisps.FillColumn(0, ld[0].DZ, ld[0].RY, ld[1].DZ, ld[1].RY);
            
            var shp = GetNMatrixAt(targetElement, isoCoords);

            var d = Matrix.DotProduct(nodalDisps, shp);

            var buf = new Displacement();

            if (_direction == BeamDirection.Y)
                buf.DY = d;
            else
                buf.DZ = d;

            return buf;
        }

        /// <inheritdoc/>
        public Matrix GetLocalInternalForceAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            var ld = localDisplacements;

            var b = GetBMatrixAt(targetElement, isoCoords);
            var d = GetDMatrixAt(targetElement, isoCoords);
            var u = new Matrix(1, 4);

            if (_direction == BeamDirection.Y)
                u.FillColumn(0, ld[0].DY, ld[0].RZ, ld[1].DY, ld[1].RZ);
            else
                u.FillColumn(0, ld[0].DZ, ld[0].RY, ld[1].DZ, ld[1].RY);

            var frc = d * b * u;

            return frc;
        }


        public Force[] GetLocalEquivalentNodalLoads(Element targetElement, Load load)
        {
            //https://www.quora.com/How-should-I-perform-element-forces-or-distributed-forces-to-node-forces-translation-in-the-beam-element

            var tr = targetElement.GetTransformationManager();

            #region uniform

            if (load is UniformLoad)
            {
                var ul = load as UniformLoad;

                var localDir = ul.Direction.GetUnit();

                if (ul.CoordinationSystem == CoordinationSystem.Global)
                {
                    
                    localDir = tr.TransformGlobalToLocal(localDir);
                }

                var ux = localDir.X * ul.Magnitude;
                var uy = localDir.Y * ul.Magnitude;
                var uz = localDir.Z * ul.Magnitude;

                var intg = GaussianIntegrator.CreateFor1DProblem(xi =>
                {
                    var shp = GetNMatrixAt(targetElement, xi, 0, 0);
                    var j = GetJMatrixAt(targetElement, xi, 0, 0);
                    shp.MultiplyByConstant(j.Determinant());

                    return shp;
                }, -1, 1, 2);

                var res = intg.Integrate();

                var localForces = new Force[2];

                if (this._direction == BeamDirection.Y)
                {
                    var fz0 = res[0, 0] * uz;
                    var my0 = res[0, 1] * uz;
                    var fz1 = res[0, 2] * uz;
                    var my1 = res[0, 3] * uz;

                    localForces[0] = new Force(0, 0, fz0, 0, my0, 0);
                    localForces[1] = new Force(0, 0, fz1, 0, my1, 0);
                }
                else
                {

                    var fy0 = res[0, 0] * uy;
                    var mz0 = res[0, 1] * uy;
                    var fy1 = res[0, 2] * uy;
                    var mz1 = res[0, 3] * uy;

                    localForces[0] = new Force(0, fy0, 0, 0, 0, mz0);
                    localForces[1] = new Force(0, fy1, 0, 0, 0, mz1);
                }

                var globalForces = localForces.Select(i => tr.TransformLocalToGlobal(i)).ToArray();

                return globalForces;
            }

            #endregion

            #region trapezoid

            if (load is TrapezoidalLoad)
            {
                var ul = load as TrapezoidalLoad;

                var localDir = ul.Direction;

                var startOffset = ul.StartOffsets[0];
                var endOffset = ul.EndOffsets[0];
                var startMag = ul.StartMagnitudes[0];
                var endMag = ul.EndMagnitudes[0];


                if (ul.CoordinationSystem == CoordinationSystem.Global)
                {
                    localDir = tr.TransformGlobalToLocal(localDir);
                }


                var xi0 = -1 + startOffset;
                var xi1 = 1 - endOffset;

                var intg = GaussianIntegrator.CreateFor1DProblem(xi =>
                {
                    var shp = GetNMatrixAt(targetElement, xi, 0, 0);
                    var q__ = ul.GetMagnitudesAt(xi)[0];
                    var j = GetJMatrixAt(targetElement, xi, 0, 0);
                    shp.MultiplyByConstant(j.Determinant());

                    var q_ = ul.Direction.GetUnit() * q__;

                    if (this._direction == BeamDirection.Y)
                        shp.MultiplyByConstant(q_.Z);
                    else
                        shp.MultiplyByConstant(q_.Y);

                    return shp;
                }, xi0, xi1, 3);

                var res = intg.Integrate();

                var localForces = new Force[2];

                if (this._direction == BeamDirection.Y)
                {
                    var fz0 = res[0, 0];
                    var my0 = res[0, 1];
                    var fz1 = res[0, 2];
                    var my1 = res[0, 3];

                    localForces[0] = new Force(0, 0, fz0, 0, my0, 0);
                    localForces[1] = new Force(0, 0, fz1, 0, my1, 0);
                }
                else
                {

                    var fy0 = res[0, 0];
                    var mz0 = res[0, 1];
                    var fy1 = res[0, 2];
                    var mz1 = res[0, 3];

                    localForces[0] = new Force(0, fy0, 0, 0, 0, mz0);
                    localForces[1] = new Force(0, fy1, 0, 0, 0, mz1);
                }

                var globalForces = localForces.Select(i => tr.TransformLocalToGlobal(i)).ToArray();

                return globalForces;
            }

            #endregion

            #region concentrated

            if (load is ConcentratedLoad)
            {
                var ul = load as ConcentratedLoad;

                var localforce = ul.Force;

                if (ul.CoordinationSystem == CoordinationSystem.Global)
                {
                    localforce = tr.TransformGlobalToLocal(ul.Force);
                }

                var shp = GetNMatrixAt(targetElement, ul.ForceIsoLocation);
                throw new NotImplementedException();
            }

            #endregion

            throw new NotImplementedException();
        }
    }
}
