using BriefFiniteElementNet.Common;
using CSparse.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;
using BriefFiniteElementNet.Loads;
using BriefFiniteElementNet.Mathh;
using CSparse.Double;
using BriefFiniteElementNet.ElementHelpers.Bar;

namespace BriefFiniteElementNet.ElementHelpers.BarHelpers
{
    public class TrussHelper2Node : BaseBar2NodeHelper
    {
        public TrussHelper2Node(Element targetElement) : base(targetElement)
        {
        }

        public static Matrix GetNMatrixAt(double xi, double l, DofConstraint d1, DofConstraint d2)
        {
            double n1, n2, n1p, n2p;

            var b1 = d1 == DofConstraint.Fixed;
            var b2 = d2 == DofConstraint.Fixed;

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

        public override Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var xi = isoCoords[0];
            var bar = targetElement as BarElement;
            var l = (bar.Nodes[1].Location - bar.Nodes[0].Location).Length;


            var d0 = bar.StartReleaseCondition.DX;
            var d1 = bar.EndReleaseCondition.DX;

            return GetNMatrixAt(xi, l, d0, d1);

        }

        /// <inheritdoc/>
        public override Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var bar = targetElement as BarElement;
            var l = (bar.Nodes[1].Location - bar.Nodes[0].Location).Length;

            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var n = GetNMatrixAt(targetElement, isoCoords);

            var buf = n.ExtractRow(1);

            //buff is dN/dξ
            //but B is dN/dx
            //so B will be arr * dξ/dx = arr * 1/ j.det

            var detJ = GetJ(bar);
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


            var u =
                //new Matrix(nc, 1);
                targetElement.MatrixPool.Allocate(nc, 1);

            for (var i = 0; i < nc; i++)
                u[i, 0] = ld[i].DX;
            //u.FillColumn(0, ld[0].DX, ld[1].DX);


            //var frc = d * b * u;
            var frc = d[0, 0] * Utils.AlgebraUtils.DotProduct(b.Values, u.Values);//performance tip, equals to d * b * u
            d.ReturnToPool();
            u.ReturnToPool();
            b.ReturnToPool();

            var buf = new List<Tuple<DoF, double>>();

            buf.Add(Tuple.Create(DoF.Dx, frc));

            return buf;
        }

        [TodoDelete]
        public /*override*/ Force[] GetLocalEquivalentNodalLoads_Old(Element targetElement, ElementalLoad load)
        {
            var tr = targetElement.GetTransformationManager();

            #region uniform & trapezoid

            if (load is UniformLoad || load is PartialNonUniformLoad)
            {

                Func<double, double> magnitude;
                Vector localDir;

                double xi0, xi1;
                int degree;//polynomial degree of magnitude function

                #region inits
                if (load is UniformLoad)
                {
                    var uld = load as UniformLoad;

                    magnitude = xi => uld.Magnitude;
                    localDir = uld.Direction;

                    if (uld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    localDir = localDir.GetUnit();

                    xi0 = -1;
                    xi1 = 1;
                    degree = 0;
                }
                else if (load is PartialNonUniformLoad)
                {
                    var uld = load as PartialNonUniformLoad;

                    magnitude = xi => uld.GetMagnitudeAt(targetElement, new IsoPoint(xi));
                    localDir = uld.Direction;

                    if (uld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    localDir = localDir.GetUnit();

                    xi0 = uld.StartLocation.Xi;
                    xi1 = uld.EndLocation.Xi;

                    degree = uld.SeverityFunction.Degree[0];// Coefficients.Length; 
                }
                else
                    throw new NotImplementedException();

                localDir = localDir.GetUnit();
                #endregion

                {

                    var nOrd = GetNMaxOrder(targetElement).Max();

                    var gpt = (nOrd + degree) / 2 + 1;//gauss point count

                    var intg = GaussianIntegrator.CreateFor1DProblem(xi =>
                    {
                        var shp = GetNMatrixAt(targetElement, xi, 0, 0);
                        var q__ = magnitude(xi);
                        var j = GetJMatrixAt(targetElement, xi, 0, 0);
                        shp.Scale(j.Determinant());

                        var q_ = localDir * q__;

                        shp.Scale(q_.X);

                        return shp;
                    }, xi0, xi1, gpt);

                    var res = intg.Integrate();

                    var localForces = new Force[2];

                    var fx0 = res[0, 0];
                    var fx1 = res[0, 1];

                    localForces[0] = new Force(fx0, 0, 0, 0, 0, 0);
                    localForces[1] = new Force(fx1, 0, 0, 0, 0, 0);

                    return localForces;
                }
            }



            #endregion

            if (load is ConcentratedLoad)
            {
                var cns = load as ConcentratedLoad;

                var shapes = GetNMatrixAt(targetElement, cns.ForceIsoLocation.Xi);

                var localForce = cns.Force;

                if (cns.CoordinationSystem == CoordinationSystem.Global)
                    localForce = tr.TransformGlobalToLocal(localForce);

                shapes.Scale(localForce.Fx);

                var fxs = shapes.Row(0);

                var n = targetElement.Nodes.Length;

                var buf = new Force[n];

                for (var i = 0; i < n; i++)
                    buf[i] = new Force(fxs[i], 0, 0, 0, 0, 0);

                return buf;
            }

            throw new NotImplementedException();

        }

        [TodoDelete]
        /// <inheritdoc/>
        public /*override*/ IEnumerable<Tuple<DoF, double>> GetLoadInternalForceAt_old(Element targetElement, ElementalLoad load,
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
                    if (xi_s[i] <= isoLocation[0])
                    {
                        var frc_i = endForces[i];// new Force();
                        ends += frc_i.Move(new Point(x_s[i], 0, 0), Point.Origins);
                    }

                }
            }


            #endregion


            var to = Iso2Local(targetElement, isoLocation)[0];

            //var xi = isoLocation[0];

            #region uniform & trapezoid

            if (load is UniformLoad || load is PartialNonUniformLoad)
            {

                Func<double, double> magnitude;
                Vector localDir;

                double xi0;
                int degree;//polynomial degree of magnitude function

                #region inits

                if (load is UniformLoad)
                {
                    var uld = load as UniformLoad;

                    magnitude = xi => uld.Magnitude;
                    localDir = uld.Direction;

                    if (uld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    localDir = localDir.GetUnit();

                    xi0 = -1;
                    //xi1 = to;
                    degree = 0;
                }
                else if (load is PartialNonUniformLoad)
                {
                    var uld = load as PartialNonUniformLoad;

                    magnitude = xi => uld.GetMagnitudeAt(targetElement, new IsoPoint(xi));
                    localDir = uld.Direction;

                    if (uld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    localDir = localDir.GetUnit();

                    xi0 = uld.StartLocation.Xi;

                    to = Math.Min(to, uld.EndLocation.Xi);

                    degree = uld.SeverityFunction.Degree[0];
                }
                else
                    throw new NotImplementedException();

                localDir = localDir.GetUnit();
                #endregion

                {

                    var nOrd = 0;// GetNMaxOrder(targetElement).Max();

                    var gpt = (nOrd + degree) / 2 + 1;//gauss point count

                    Matrix integral;


                    if (isoLocation[0] < xi0)
                    {
                        integral = new Matrix(2, 1);
                    }
                    else
                    {
                        var intgV = GaussianIntegrator.CreateFor1DProblem(x =>
                        {
                            var xi = Local2Iso(targetElement, x);
                            var q__ = magnitude(xi);
                            var q_ = localDir * q__;

                            var df = q_.X;

                            var buf_ = Matrix.OfVector(new double[] { df });

                            return buf_;
                        }, 0, to, gpt);

                        integral = intgV.Integrate();
                    }

                    var X = Iso2Local(targetElement, isoLocation)[0];

                    var f_i = integral[0, 0];

                    var f = new Force();

                    f.Fx = f_i;


                    //this block is commented to fix the issue #48 on github
                    //when this block is commented out, then issue 48 is fixed
                    {

                        //if (br.StartReleaseCondition.DX == DofConstraint.Released)
                        //    f_i = 0;
                    }

                    var f2 = f + ends;

                    f2 = f2.Move(new Point(0, 0, 0), new Point(X, 0, 0));

                    f2 *= -1;

                    /*
                    var movedEnds = ends.Move(new Point(), new Point());//no need to move as it is truss without moments
                    var fMoved = new Force(f_i, 00, 00, 0, 0, 0);

                    var ft = movedEnds + fMoved;
                    */

                    //ft *= -1;

                    buff.Add(Tuple.Create(DoF.Dx, f2.Fx));

                }

                return buff;
            }

            #endregion

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
                frc = frc.Move(new Point(0, 0, 0), new Point(targetX, 0, 0));

                var movedEnds = ends.Move(new Point(0, 0, 0), new Point(targetX, 0, 0));

                var f2 = frc + movedEnds;
                f2 *= -1;

                buff.Add(Tuple.Create(DoF.Dx, f2.Fx));

                return buff;
            }

            #endregion


            throw new NotImplementedException();
        }

        [TodoDelete]
        public /*override*/ Displacement GetLoadDisplacementAt_old(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            var bar = targetElement as BarElement;

            var n = targetElement.Nodes.Length;

            if (n != 2)
                throw new Exception("more than two nodes not supported");


            var eIorder = bar.Section.GetMaxFunctionOrder()[0] + bar.Material.GetMaxFunctionOrder()[0];

            if (eIorder == 0 && bar.StartReleaseCondition == Constraints.Fixed && bar.EndReleaseCondition == Constraints.Fixed)//constant/uniform section along the length of beam
            {
                //EI is constant through the length of beam
                //end releases are fixed

                if (load is UniformLoad ul)
                    return GetLoadDisplacementAt_UniformLoad_uniformSection(bar, ul, isoLocation[0]);

                if (load is ConcentratedLoad cl)
                    return GetLoadDisplacementAt_ConcentratedLoad_uniformSection(bar, cl, isoLocation[0]);

                //if (load is PartialNonUniformLoad pnl)
                //    return GetLoadDisplacementAt_PartialNonUniformLoad_uniformSection(bar, pnl, isoLocation[0]);
            }

            throw new NotImplementedException();
        }

        [TodoDelete]
        //copied to LoadHandler
        private Displacement GetLoadDisplacementAt_UniformLoad_uniformSection(BarElement bar, UniformLoad load, double xi)
        {
            //https://byjusexamprep.com/gate-ce/fixed-beams
            double w0;
            double L;
            double f0, m0;

            if (bar.NodeCount != 2)
                throw new Exception();

            {//step 0
                L = (bar.Nodes[1].Location - bar.Nodes[0].Location).Length;
            }

            #region step 1
            {
                var p0 = GetLocalEquivalentNodalLoads(bar, load)[0];

                p0 = -p0;

                var localDir = load.Direction;

                var tr = bar.GetTransformationManager();

                if (load.CoordinationSystem == CoordinationSystem.Global)
                    localDir = tr.TransformGlobalToLocal(localDir);

                localDir = localDir.GetUnit();

                w0 = localDir.X * load.Magnitude;
            }
            #endregion

            {
                var eiOrder = bar.Section.GetMaxFunctionOrder()[0] + bar.Material.GetMaxFunctionOrder()[0];

                if (eiOrder != 0) throw new BriefFiniteElementNetException("Nonuniform EI");
            }




            {
                var sec = bar.Section.GetCrossSectionPropertiesAt(xi, bar);
                var mat = bar.Material.GetMaterialPropertiesAt(new IsoPoint(xi), bar);

                var e = mat.Ex;
                var A = sec.A;
                var x = bar.IsoCoordsToLocalCoords(xi)[0];

                
                var d = w0 * x / (2 * e * A) * (L - x);

                var buf = new Displacement();

                buf.DX = d;

                return buf;
            }
        }

        [TodoDelete]
        //copied to load handler
        public Force[] GetLocalEquivalentNodalLoads_imposedStrainLoad_UniformMatSection(BarElement bar, ImposedStrainLoad load)
        {
            var E = bar.Material.GetMaterialPropertiesAt(0).Ex;
            var A = bar.Material.GetMaterialPropertiesAt(0).Ex;
            var L = bar.GetLength();
            var strain = load.ImposedStrainMagnitude;

            //F = K . D, K = E.A/L, strain = D/L, F=strain*E*A

            var f = strain * E * A;


            throw new NotImplementedException();
            
        }

        [TodoDelete]
        private Displacement GetLoadDisplacementAt_ConcentratedLoad_uniformSection(BarElement bar, ConcentratedLoad load, double xi)
        {
            
            double L;
            double ft;//force concentrated
            double f0;//inverse of eq nodal loads
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

                f0 = p0.Fx;
            }

            {
                var localForce = load.Force;

                var tr = bar.GetTransformationManager();

                if (load.CoordinationSystem == CoordinationSystem.Global)
                    localForce = tr.TransformGlobalToLocal(localForce);

                ft = localForce.Fx;
            }

            #endregion

            {
                var eiOrder = bar.Section.GetMaxFunctionOrder()[0] + bar.Material.GetMaxFunctionOrder()[0];

                if (eiOrder != 0) throw new BriefFiniteElementNetException("Nonuniform EI");
            }

            {
                var sec = bar.Section.GetCrossSectionPropertiesAt(xi, bar);
                var mat = bar.Material.GetMaterialPropertiesAt(new IsoPoint(xi), bar);

                var e = mat.Ex;

                var a = sec.A;

                var x = bar.IsoCoordsToLocalCoords(xi)[0];

                var d = 0.0;

                if (x <= xt)
                    d = -f0 * x / (e * a);
                else
                    d = -f0 * xt / (e * a) + (f0 + ft) * (x - xt) / (e * a);

                var buf = new Displacement();

                buf.DX = d;

                return buf;
            }
        }

        /// <inheritdoc/>
        public override Displacement GetLocalDisplacementAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            var n = GetNMatrixAt(targetElement, isoCoords).Row(0);

            var u = new double[targetElement.Nodes.Length];

            for (var i = 0; i < targetElement.Nodes.Length; i++)
                u[i] = localDisplacements[i].DX;

            return new Displacement(Utils.AlgebraUtils.DotProduct(n, u), 0, 0, 0, 0, 0);
        }


        public void AddStiffnessComponents(CoordinateStorage<double> global)
        {
            throw new NotImplementedException();
        }

        [TodoDelete]
        public GeneralStressTensor GetLocalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        [TodoDelete]
        public override GeneralStressTensor GetLoadStressAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        [TodoDelete]
        public override GeneralStressTensor GetLocalInternalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public override DoF[] GetDofsPerNode()
        {
            return new DoF[] { DoF.Dx };
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

            return mat.Rho * geo.A;
        }

        public override double GetD(BarElement targetElement, double xi)
        {
            var geo = targetElement.Section.GetCrossSectionPropertiesAt(xi, targetElement);
            var mech = targetElement.Material.GetMaterialPropertiesAt(xi);

            return mech.Ex * geo.A;
        }

        public override ILoadHandler[] GetLoadHandlers()
        {
            return new ILoadHandler[] {

                new LoadHandlers.Truss2Node.Concentrated_UF_Handler(),
                new LoadHandlers.Truss2Node.ImposedStrain_UF_Handler(),
                new LoadHandlers.Truss2Node.PartialNonuniform_UF_Handler(),
                new LoadHandlers.Truss2Node.Uniform_UF_Handler(),
            };
        }

        public override int CalcLocalStiffnessMatrixSize(Element targetElement)
        {
            return 2;
        }
    }
}
