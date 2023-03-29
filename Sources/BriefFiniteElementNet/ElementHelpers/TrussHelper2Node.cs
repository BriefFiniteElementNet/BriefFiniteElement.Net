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
using BriefFiniteElementNet;
using CSparse.Double;



namespace BriefFiniteElementNet.ElementHelpers
{
    public class TrussHelper2Node : IElementHelper
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

            var detJ = BaseBar2NodeHelper.GetJ(bar);
            buf.ScaleRow(0, 1 / (detJ));

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

            buf.At(0, 0, geo.A * mech.Ex);

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

            buf[0, 0] = geo.A * mech.Rho;

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
            var buf = ElementHelperExtensions.CalcLocalKMatrix_Bar(this, targetElement);

            return buf;
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
                buf[i] = new ElementPermuteHelper.ElementLocalDof(i, DoF.Dx);
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


            var u =
                //new Matrix(nc, 1);
                targetElement.MatrixPool.Allocate(nc, 1);

            for (var i = 0; i < nc; i++)
                u[i, 0] = ld[i].DX;
            //u.FillColumn(0, ld[0].DX, ld[1].DX);


            //var frc = d * b * u;
            var frc = d[0, 0] * CalcUtil.DotProduct(b.Values, u.Values);//performance tip, equals to d * b * u
            d.ReturnToPool();
            u.ReturnToPool();
            b.ReturnToPool();

            var buf = new List<Tuple<DoF, double>>();

            buf.Add(Tuple.Create(DoF.Dx, frc));

            return buf;
        }


        /// <inheritdoc/>
        public int[] GetNMaxOrder(Element targetElement)
        {
            return new int[] { targetElement.Nodes.Length - 1, 0, 0 };
        }

        public int[] GetBMaxOrder(Element targetElement)
        {
            return new int[] { targetElement.Nodes.Length - 2, 0, 0 };
        }

        public int[] GetDetJOrder(Element targetElement)
        {
            return new int[] { targetElement.Nodes.Length - 2, 0, 0 };
        }


        public double[] Iso2Local(Element targetElement, params double[] isoCoords)
        {
            var buf = BaseBar2NodeHelper.Iso2Local(targetElement, isoCoords[0]);
            return new double[] { buf };
        }

        public double[] Local2Iso(Element targetElement, params double[] localCoords)
        {
            var buf = BaseBar2NodeHelper.Local2Iso(targetElement, localCoords[0]);
            return new double[] { buf };
        }

        public Matrix GetJMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var j = BaseBar2NodeHelper.GetJ(targetElement);

            var buf = new Matrix(1, 1);

            buf[0, 0] = j;

            return buf;
        }


        /// <inheritdoc/>
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
                    var uld = (load as UniformLoad);

                    magnitude = (xi => uld.Magnitude);
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
                    var uld = (load as PartialNonUniformLoad);

                    magnitude = (xi => uld.GetMagnitudeAt(targetElement, new IsoPoint(xi)));
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
                            var xi = Local2Iso(targetElement, x)[0];
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
                u[i] = localDisplacements[i].DX;

            return new Displacement(CalcUtil.DotProduct(n, u), 0, 0, 0, 0, 0);
        }

        public Force[] GetLocalEquivalentNodalLoads(Element targetElement, ElementalLoad load)
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
                    var uld = (load as UniformLoad);

                    magnitude = (xi => uld.Magnitude);
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
                    var uld = (load as PartialNonUniformLoad);

                    magnitude = (xi => uld.GetMagnitudeAt(targetElement, new IsoPoint(xi)));
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

                var shapes = this.GetNMatrixAt(targetElement, cns.ForceIsoLocation.Xi);

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
