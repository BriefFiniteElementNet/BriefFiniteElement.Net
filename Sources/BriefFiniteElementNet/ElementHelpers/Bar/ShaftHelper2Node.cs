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
using BriefFiniteElementNet.ElementHelpers.Bar;
using BriefFiniteElementNet.Materials;

namespace BriefFiniteElementNet.ElementHelpers.BarHelpers
{
    public class ShaftHelper2Node : BaseBar2NodeHelper
    {
        public Element TargetElement { get; set; }


        public static Matrix GetNMatrixAt(double xi, double l, DofConstraint r1, DofConstraint r2)
        {
            double n1, n2, n1p, n2p;

            var b1 = r1 == DofConstraint.Fixed;
            var b2 = r2 == DofConstraint.Fixed;

            var num = (b1 ? 1 : 0) * 2 + (b2 ? 1 : 0);


            switch (num)
            {
                case 0://both released
                    n1 = n2 = n1p = n2p = 0;
                    break;
                case 1://b2: fix, b1: release
                    n1 = n1p = n2p = 0;
                    n2 = 1;
                    break;
                case 2://b2: release, b1: fix
                    n2 = n1p = n2p = 0;
                    n1 = 1;
                    break;
                case 3://both fixed
                    n1 = -0.5 * xi + 0.5;
                    n2 = 0.5 * xi + 0.5;
                    n1p = -0.5;
                    n2p = +0.5;
                    break;
                default:
                    throw new NotImplementedException();

            }

            var buf = new Matrix(2, 2);

            buf.SetRow(0, n1, n2);
            buf.SetRow(1, n1p, n2p);

            return buf;
        }


        public ShaftHelper2Node(Element targetElement) :
            base(targetElement)
        {
        }

        public override Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var xi = isoCoords[0];
            var bar = targetElement as BarElement;
            var l = (bar.Nodes[1].Location - bar.Nodes[0].Location).Length;

            var r0 = bar.StartReleaseCondition.RX;
            var r1 = bar.EndReleaseCondition.RX;

            return GetNMatrixAt(xi, l, r0, r1);
        }

        /// <inheritdoc/>
        public override Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
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
            buf.ScaleRow(0, 1 / detJ);

            return buf;


        }





        /// <inheritdoc/>
        public override IEnumerable<Tuple<DoF, double>> GetLocalInternalForceAt(Element targetElement,
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


        public IEnumerable<Tuple<DoF, double>> GetLoadInternalForceAt_old(Element targetElement, ElementalLoad load,
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

        public Displacement GetLoadDisplacementAt_old(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            var bar = targetElement as BarElement;

            var n = targetElement.Nodes.Length;

            if (n != 2)
                throw new Exception("more than two nodes not supported");


            var eIorder = bar.Section.GetMaxFunctionOrder()[0] + bar.Material.GetMaxFunctionOrder()[0];

            var isuniformSection = eIorder == 0 && bar.StartReleaseCondition == Constraints.Fixed && bar.EndReleaseCondition == Constraints.Fixed;//uniform and both ends fixed

            if (isuniformSection)//constant/uniform section along the length of beam
            {
                //EI is constant through the length of beam
                //end releases are fixed

                if (load is UniformLoad ul)
                    return Displacement.Zero;

                if (load is ConcentratedLoad cl)
                    return GetLoadDisplacementAt_ConcentratedLoad_uniformSection(bar, cl, isoLocation[0]);

                //if (load is PartialNonUniformLoad pnl)
                //    return GetLoadDisplacementAt_PartialNonUniformLoad_uniformSection(bar, pnl, isoLocation[0]);
            }

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Displacement GetLocalDisplacementAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            var n = GetNMatrixAt(targetElement, isoCoords).Row(0);

            var u = new double[targetElement.Nodes.Length];

            for (var i = 0; i < targetElement.Nodes.Length; i++)
                u[i] = localDisplacements[i].RX;

            return new Displacement(0, 0, 0, CalcUtil.DotProduct(n, u), 0, 0);
        }

        private Displacement GetLoadDisplacementAt_ConcentratedLoad_uniformSection(BarElement bar, ConcentratedLoad load, double xi)
        {
            double L;
            double tt;//tprsion concentrated
            double t0;//inverse of eq nodal loads
            double xt;//applied location

            if (bar.NodeCount != 2)
                throw new Exception();

            {//step 0
                L = (bar.Nodes[1].Location - bar.Nodes[0].Location).Length;

                xt = bar.IsoCoordsToLocalCoords(load.ForceIsoLocation.Xi)[0];
            }

            #region step 1

            {
                var p0 = GetLocalEquivalentNodalLoads(bar, load)[0];

                p0 = -p0;

                t0 = p0.Mx;
            }

            {
                var localForce = load.Force;

                var tr = bar.GetTransformationManager();

                if (load.CoordinationSystem == CoordinationSystem.Global)
                    localForce = tr.TransformGlobalToLocal(localForce);

                tt = localForce.Mx;
            }

            #endregion

            {
                var eiOrder = bar.Section.GetMaxFunctionOrder()[0] + bar.Material.GetMaxFunctionOrder()[0];

                if (eiOrder != 0) throw new BriefFiniteElementNetException("Nonuniform EI");
            }

            {
                var sec = bar.Section.GetCrossSectionPropertiesAt(xi, bar);
                var mat = bar.Material.GetMaterialPropertiesAt(new IsoPoint(xi), bar);

                var g = mat.Ex / (2 * (1 + mat.NuYz));

                var j = sec.J;
                var x = bar.IsoCoordsToLocalCoords(xi)[0];
                
                var d = 0.0;//TODO

                if (x <= xt)
                    d = -t0 * x / (g * j);
                else
                    d = -t0 * xt / (g * j) + (t0 + tt) * (x - xt) / (g * j);

                var buf = new Displacement();

                buf.RX = d;

                return buf;
            }
        }


        public Force[] GetLocalEquivalentNodalLoads_old(Element targetElement, ElementalLoad load)
        {
            var tr = targetElement.GetTransformationManager();

            if (load is UniformLoad || load is PartialNonUniformLoad)
            {
                return new Force[2];
            }

            if (load is ConcentratedLoad)
            {
                var cns = load as ConcentratedLoad;

                var shapes = GetNMatrixAt(targetElement, cns.ForceIsoLocation.Xi);

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

        public override GeneralStressTensor GetLoadStressAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public override GeneralStressTensor GetLocalInternalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public override DoF[] GetDofsPerNode()
        {
            return new DoF[] { DoF.Rx };
        }

        protected override int GetBOrder()
        {
            return 0;
        }

        protected override int GetNOrder()
        {
            return 1;
        }


        public override double GetMu(BarElement targetElement, double xi)
        {
            var geo = targetElement.Section.GetCrossSectionPropertiesAt(xi, targetElement);
            var mat = targetElement.Material.GetMaterialPropertiesAt(xi);

            return mat.Mu * geo.A;
        }

        public override double GetRho(BarElement targetElement, double xi)
        {
            var geo = targetElement.Section.GetCrossSectionPropertiesAt(xi, targetElement);
            var mat = targetElement.Material.GetMaterialPropertiesAt(xi);

            return mat.Rho * geo.A;//TODO? not sure. this way mass is calculated 3 times same in truss, shaft, beam
        }

        public override double GetD(BarElement targetElement, double xi)
        {
            var geo = targetElement.Section.GetCrossSectionPropertiesAt(xi, targetElement);
            var mech = targetElement.Material.GetMaterialPropertiesAt(xi);

            var g = mech.Ex / (2 * (1 + mech.NuXy));

            return g * geo.J;
        }

        public override ILoadHandler[] GetLoadHandlers()
        {
            return new ILoadHandler[] {

                new LoadHandlers.ShaftHelper.Concentrated_UF_Handler(),
                new LoadHandlers.ShaftHelper.GeneralHandler(),
            };
        }
    }
}
