using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;
using BriefFiniteElementNet.Loads;
using BriefFiniteElementNet.Mathh;
using CSparse.Storage;
using BriefFiniteElementNet.Common;

namespace BriefFiniteElementNet.ElementHelpers
{
    public class ShaftHelper2Node:IElementHelper
    {
        public Element TargetElement { get; set; }

        public static Matrix GetNMatrixAt(double xi, double l)
        {
            var buf = new Matrix(2, 2);

            {
                var n1 = -0.5 * xi + 0.5;
                var n2 = 0.5 * xi + 0.5;

                buf.SetRow(0, n1, n2);

                buf.SetRow(1, -0.5, 0.5);
            }

            return buf;
        }

        public static double GetJ(double l)
        {
            return l / 2;
        }

        
        public ShaftHelper2Node(Element targetElement)
        {
            TargetElement = targetElement;
        }

        public Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var xi = isoCoords[0];
            var bar = targetElement as BarElement;
            var l = (bar.Nodes[1].Location - bar.Nodes[0].Location).Length;

            return GetNMatrixAt(xi, l);
        }

        /// <inheritdoc/>
        public Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var n = GetNMatrixAt(targetElement, isoCoords);

            var buf = targetElement.MatrixPool.Allocate(1, n.ColumnCount);

            for (var i = 0; i < buf.ColumnCount; i++)
                buf[0, i] = n[1, i];

            //n.ExtractRow(1);

            n.ReturnToPool();

            //buff is dN/dξ
            //but B is dN/dx
            //so B will be arr * dξ/dx = arr * 1/ j.det

            var J = GetJMatrixAt(targetElement, isoCoords);
            var detJ = J.Determinant();
            J.ReturnToPool();
            buf.ScaleRow(0, 1 / (detJ));

            return buf;


        }



        public Matrix GetJMatrixAt(Element targetElement, params double[] isoCoords)
        {
            //we need J = ∂X / ∂ξ
            var bar = targetElement as BarElement;
            var l = (bar.Nodes[1].Location - bar.Nodes[0].Location).Length;

            var j = GetJ(l);

            var buf = new Matrix(1, 1);

            buf[0, 0] = j;

            return buf;
        }


        /// <inheritdoc/>
        public Matrix GetDMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var xi = isoCoords[0];

            var geo = elm.Section.GetCrossSectionPropertiesAt(xi, targetElement);
            var mech = elm.Material.GetMaterialPropertiesAt(xi);

            var buf =
            //new Matrix(1, 1);
            targetElement.MatrixPool.Allocate(1, 1);

            ThrowUtil.ThrowIf(!CalcUtil.IsIsotropicMaterial(mech), "anistropic material not impolemented yet");

            var g = mech.Ex / (2 * (1 + mech.NuXy));

            buf.At(0, 0, geo.J * g);

            return buf;
        }

        /// <inheritdoc/>
        public Matrix GetRhoMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var xi = isoCoords[0];

            var geo = elm.Section.GetCrossSectionPropertiesAt(xi, targetElement);
            var mech = elm.Material.GetMaterialPropertiesAt(xi);

            var buf = new Matrix(1, 1);

            buf[0, 0] = geo.J * mech.Rho;

            return buf;
        }

        /// <inheritdoc/>
        public Matrix GetMuMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var xi = isoCoords[0];

            var geo = elm.Section.GetCrossSectionPropertiesAt(xi, targetElement);
            var mech = elm.Material.GetMaterialPropertiesAt(xi);

            var buf = new Matrix(1, 1);

            buf[0, 0] = geo.A * mech.Mu;

            return buf;
        }

        

        /// <inheritdoc/>
        public Matrix CalcLocalStiffnessMatrix(Element targetElement)
        {
            return ElementHelperExtensions.CalcLocalKMatrix_Bar(this, targetElement);
        }

        /// <inheritdoc/>
        public Matrix CalcLocalMassMatrix(Element targetElement)
        {
            var buf = ElementHelperExtensions.CalcLocalMMatrix_Bar(this, targetElement);

            return buf;
        }

        /// <inheritdoc/>
        public Matrix CalcLocalDampMatrix(Element targetElement)
        {
            return ElementHelperExtensions.CalcLocalCMatrix_Bar(this, targetElement);
        }

        /// <inheritdoc/>
        public ElementPermuteHelper.ElementLocalDof[] GetDofOrder(Element targetElement)
        {
            var n = targetElement.Nodes.Length;

            var buf = new ElementPermuteHelper.ElementLocalDof[n];

            for (int i = 0; i < n; i++)
            {
                buf[i] = new ElementPermuteHelper.ElementLocalDof(i, DoF.Rx);
            }

            return buf;

        }

        /// <inheritdoc/>
        public IEnumerable<Tuple<DoF, double>> GetLocalInternalForceAt(Element targetElement,
            Displacement[] localDisplacements, params double[] isoCoords)
        {
            var ld = localDisplacements;

            var b = GetBMatrixAt(targetElement, isoCoords);
            var d = GetDMatrixAt(targetElement, isoCoords);
            var nc = targetElement.Nodes.Length;


            var u = new Matrix(nc, 1);

            for (var i = 0; i < nc; i++)
                u[i, 0] = ld[i].RX;

            var frc = d * b * u;

            var buf = new List<Tuple<DoF, double>>();

            buf.Add(Tuple.Create(DoF.Rx, frc[0, 0]));

            return buf;
        }

        /// <inheritdoc/>
        public bool DoesOverrideKMatrixCalculation(Element targetElement, Matrix transformMatrix)
        {
            return false;
        }

        /// <inheritdoc/>
        public int[] GetNMaxOrder(Element targetElement)
        {
            return new int[] { 1, 0, 0 };
        }

        public int[] GetBMaxOrder(Element targetElement)
        {
            return new int[] { 0, 0, 0 };
        }

        public int[] GetDetJOrder(Element targetElement)
        {
            return new int[] { 0, 0, 0 };
        }

        public IEnumerable<Tuple<DoF, double>> GetLoadInternalForceAt(Element targetElement, ElementalLoad load,
            double[] isoLocation)
        {

            var buff = new List<Tuple<DoF, double>>();

            //var buf = new FlatShellStressTensor();

            var tr = targetElement.GetTransformationManager();

            var br = targetElement as BarElement;

            var endForces = GetLocalEquivalentNodalLoads(targetElement, load);

            var n = targetElement.Nodes.Length;

            for (var i = 0; i < n; i++)
                endForces[i] = -endForces[i];

            #region 2,1 (due to inverse of equivalent nodal loads)

            Force ends;//internal force in x=0 due to inverse of equivalent nodal loads will store in this variable, 

            {
                var xi_s = new double[br.Nodes.Length];//xi loc of each force
                var x_s = new double[br.Nodes.Length];//x loc of each force

                for (var i = 0; i < xi_s.Length; i++)
                {
                    var x_i = targetElement.Nodes[i].Location - targetElement.Nodes[0].Location;
                    var xi_i = br.LocalCoordsToIsoCoords(x_i.Length)[0];

                    xi_s[i] = xi_i;
                    x_s[i] = x_i.X;
                }

                ends = new Force();//sum of moved end forces to destination

                for (var i = 0; i < n; i++)
                {
                    if (xi_s[i] < isoLocation[0])
                    {
                        var frc_i = endForces[i];// new Force();
                        ends += frc_i.Move(new Point(x_s[i], 0, 0), Point.Origins);
                    }

                }
            }


            #endregion


            if (load is UniformLoad || load is PartialNonUniformLoad)
            {
                return new List<Tuple<DoF, double>>();
            }

            #region concentrated

            if (load is ConcentratedLoad)
            {
                var cns = load as ConcentratedLoad;

                var xi = isoLocation[0];
                var targetX = br.IsoCoordsToLocalCoords(xi)[0];

                var frc = Force.Zero;

                if (cns.ForceIsoLocation.Xi < xi)
                    frc = cns.Force;

                if (cns.CoordinationSystem == CoordinationSystem.Global)
                    frc = tr.TransformGlobalToLocal(frc);


                var frcX = br.IsoCoordsToLocalCoords(cns.ForceIsoLocation.Xi)[0];

                frc = frc.Move(new Point(frcX, 0, 0), new Point(0, 0, 0));

                if (br.StartReleaseCondition.RX == DofConstraint.Released)
                    frc.Mx = 0;

                frc = frc.Move(new Point(0, 0, 0), new Point(targetX, 0, 0));

                var movedEnds = ends.Move(new Point(0, 0, 0), new Point(targetX, 0, 0));

                var f2 = frc + movedEnds;
                f2 *= -1;

                buff.Add(Tuple.Create(DoF.Rx, f2.Mx));

                return buff;
            }

            #endregion

            throw new NotImplementedException();

        }

        public Displacement GetLoadDisplacementAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Displacement GetLocalDisplacementAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            var n = GetNMatrixAt(targetElement, isoCoords).Row(0);

            var u = new double[targetElement.Nodes.Length];

            for (var i = 0; i < targetElement.Nodes.Length; i++)
                u[i] = localDisplacements[i].RX;

            return new Displacement(0, 0, 0, CalcUtil.DotProduct(n, u), 0, 0);
        }

        public Force[] GetLocalEquivalentNodalLoads(Element targetElement, ElementalLoad load)
        {

            var tr = targetElement.GetTransformationManager();


            if (load is UniformLoad)
            {
                return new Force[2];
            }

            if (load is PartialNonUniformLoad)
            {
                return new Force[2];
            }

            if (load is ConcentratedLoad)
            {
                var cns = load as ConcentratedLoad;

                var shapes = this.GetNMatrixAt(targetElement, cns.ForceIsoLocation.Xi);

                var localForce = cns.Force;

                if (cns.CoordinationSystem == CoordinationSystem.Global)
                    localForce = tr.TransformGlobalToLocal(localForce);


                shapes.Scale(localForce.Mx);

                var fxs = shapes.Row(0);

                var n = targetElement.Nodes.Length;

                var buf = new Force[n];

                for (var i = 0; i < n; i++)
                    buf[i] = new Force(0, 0, 0, fxs[i], 0, 0);

                return buf;
            }

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
