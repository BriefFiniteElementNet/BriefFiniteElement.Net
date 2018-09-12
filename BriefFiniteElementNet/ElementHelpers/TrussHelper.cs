using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;
using BriefFiniteElementNet.Loads;
using BriefFiniteElementNet.Mathh;

namespace BriefFiniteElementNet.ElementHelpers
{
    /// <summary>
    /// Represents an element helper for truss element.
    /// </summary>
    public class TrussHelper : IElementHelper
    {
        public Element TargetElement { get; set; }

        /// <inheritdoc/>
        public Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            double[] v1 = null;

            {//new
                var n = GetNMatrixAt(targetElement, isoCoords);

                var buf = n.ExtractRow(1);
                var l = (targetElement.Nodes.First().Location - targetElement.Nodes.Last().Location).Length;

                buf.MultiplyByConstant(2 / l);//http://www.solid.iei.liu.se/Education/TMHL08/Lectures/Lecture__8.pdf

                v1 = buf.CoreArray;

                return buf;
            }


            double[] v2 = null;

            {//old
               

                var l = (elm.EndNode.Location - elm.StartNode.Location).Length;

                var buf = new Matrix(1, 2);

                var b1 = -1 / l;
                var b2 = 1 / l;

                var c1 = elm.StartReleaseCondition;
                var c2 = elm.EndReleaseCondition;

                if (c1.DX == DofConstraint.Released)
                    b1 = 0;

                if (c2.DX == DofConstraint.Released)
                    b2 = 0;

                buf.FillRow(0, b1, b2);

                return buf;
                v2 = buf.CoreArray;
            }

            return null;
            
        }

        /// <inheritdoc/>
        public Matrix GetB_iMatrixAt(Element targetElement, int i, params double[] isoCoords)
        {
            if (i != 0)
                throw new Exception();

            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var l = (elm.EndNode.Location - elm.StartNode.Location).Length;

            var buf = new Matrix(1, 2);

            buf.FillRow(0, -1 / l, 1 / l);


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

            buf.FillRow(0, geo.A*mech.Ex);

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

            var geo = elm.Section.GetCrossSectionPropertiesAt(xi);
            var mech = elm.Material.GetMaterialPropertiesAt(xi);

            var buf = new Matrix(1, 1);

            buf[0, 0] = geo.A * mech.Mu;

            return buf;
        }

        /// <inheritdoc/>
        public Matrix GetNMatrixAt_old(Element targetElement, params double[] isoCoords)
        {
            var xi = isoCoords[0];

            if (xi < -1 || xi > 1)
                throw new ArgumentOutOfRangeException(nameof(isoCoords));

            var bar = targetElement as BarElement;

            if (bar == null)
                throw new Exception();

            var n1 = 1 / 2.0 - xi / 2;
            var n2 = 1 / 2.0 + xi / 2;

            var buf = new Matrix(1, 2);

            double[] arr;

            var c1 = bar.StartReleaseCondition;
            var c2 = bar.EndReleaseCondition;

            if (c1.DX == DofConstraint.Released)
                n1 = 0;

            if (c2.DX == DofConstraint.Released)
                n2 = 0;

            arr = new double[] { n1, n2 };
            
            buf.FillRow(0, arr);

            return buf;
        }


        public Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var xi = isoCoords[0];

            Polynomial[] ns = null;

            {//retrieve or generate shapefunctions
                var nsKey = "{F2133247-149A-429C-857F-9E0159227170}";//a random unified key for store truss shape functions for bar element

                object obj;

                if (targetElement.Cache.TryGetValue(nsKey, out obj))
                {
                    ns = obj as Polynomial[];
                }

                if (ns == null)
                {
                    ns = new Polynomial[targetElement.Nodes.Length];

                    for (var i = 0; i < ns.Length; i++)
                        ns[i] = GetN_i(targetElement, i);
                }
            }

            var buf = new Matrix(2, ns.Length);

            {//fill buff
                for (var i = 0; i < ns.Length; i++)
                {
                    buf[0, i] = ns[i].EvaluateDerivative(xi, 0);
                    buf[1, i] = ns[i].EvaluateDerivative(xi, 1);
                }
            }

            return buf;
        }

        public Polynomial GetN_i(Element targetElement,int ith)
        {
            var bar = targetElement as BarElement;

            if (bar == null)
                return null;

            var n = bar.NodeCount;

            var xis = new Func<int, double>(i =>
            {
                var delta = 2.0 / (n - 1);

                return -1 + delta * i;
            });

            var conditions = new List<Tuple<double, double>>();

            for (var i = 0; i < n; i++)
            {
                if (bar.NodalReleaseConditions[i].DX == DofConstraint.Fixed)
                    conditions.Add(Tuple.Create(xis(i), ith == i ? 1.0 : 0.0));
            }

            var condCount = conditions.Count;

            var condMtx = new Matrix(condCount, condCount);
            var rMtx = new Matrix(condCount, 1);

            for (var i = 0; i < condCount; i++)
            {
                var rw = new double[condCount];
                var cond = conditions[i];

                for (var j = 0; j < condCount; j++)
                {
                    var origPow = condCount - 1 - j;

                    rw[j] = Math.Pow(cond.Item1, origPow);
                }

                condMtx.SetRow(i, rw);
                rMtx.SetRow(i, cond.Item2);
            }

            var res = condMtx.Inverse() * rMtx;
            var buf = new Polynomial(res.CoreArray);

            { //test
                var epsilon = 0.0;

                for (var i = 0; i < condCount; i++)
                {
                    var cond = conditions[i];

                    var d = buf.Evaluate(cond.Item1) - cond.Item2;

                    epsilon = Math.Max(epsilon, Math.Abs(d));
                }

                if (epsilon > 1e-7)
                    throw new Exception();
            }

            return buf;
        }

        /// <inheritdoc/>
        public Matrix GetJMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var bar = targetElement as BarElement;

            if (bar == null)
                throw new Exception();

            var x_xi = bar.GetIsoToLocalConverter();
            
            var buf = new Matrix(1, 1);
            //we need J = ∂X / ∂ξ = dX / dξ

            buf[0, 0] = x_xi.EvaluateDerivative(isoCoords[0], 1);
            //var old = l / 2;
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
                new FluentElementPermuteManager.ElementLocalDof(0, DoF.Dx),
                new FluentElementPermuteManager.ElementLocalDof(1, DoF.Dx),
            };
        }

        /// <inheritdoc/>
        public IEnumerable<Tuple<DoF, double>> GetLocalInternalForceAt(Element targetElement,
            Displacement[] localDisplacements, params double[] isoCoords)
        {
            var ld = localDisplacements;

            var b = GetBMatrixAt(targetElement, isoCoords);
            var d = GetDMatrixAt(targetElement, isoCoords);
            var u = new Matrix(2, 1);

            u.FillColumn(0, ld[0].DX, ld[1].DX);

            var frc = d * b * u;

            var buf = new List<Tuple<DoF, double>>();

            buf.Add(Tuple.Create(DoF.Dx, frc[0, 0]));

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
            return new int[] { targetElement.Nodes.Length - 1, 0, 0 };
        }

        public int[] GetBMaxOrder(Element targetElement)
        {
            return new int[] { targetElement.Nodes.Length - 2, 0, 0};
        }

        public int[] GetDetJOrder(Element targetElement)
        {
            return new int[] { targetElement.Nodes.Length - 2, 0, 0 };
        }

        public double[] Iso2Local(Element targetElement, params double[] isoCoords)
        {
            var tg = targetElement as BarElement;


            if (tg != null)
            {
                var xi = isoCoords[0];

                if (tg.Nodes.Length == 2)
                {
                    var l = (tg.Nodes[1].Location - tg.Nodes[0].Location).Length;
                    return new[] { l * (xi + 1) / 2 };
                }
            }

            throw new NotImplementedException();
        }

        public double[] Local2Iso(Element targetElement, params double[] localCoords)
        {
            var tg = targetElement as BarElement;


            if (tg != null)
            {
                var x = localCoords[0];

                if (tg.Nodes.Length == 2)
                {
                    var l = (tg.Nodes[1].Location - tg.Nodes[0].Location).Length;
                    return new[] { 2 * x / l - 1 };
                }
            }

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerable<Tuple<DoF, double>> GetLoadInternalForceAt(Element targetElement, Load load,
            double[] isoLocation)
        {
            var buf = new FlatShellStressTensor();

            var tr = targetElement.GetTransformationManager();

            var br = targetElement as BarElement;

            var endForces = GetLocalEquivalentNodalLoads(targetElement, load);


            var v0 =
                endForces[0].Fx;

            v0 = -v0;

            var to = Iso2Local(targetElement, isoLocation)[0];

            //var xi = isoLocation[0];

            #region uniform & trapezoid

            if (load is UniformLoad || load is PartialTrapezoidalLoad)
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

                    xi0 = -1;
                    xi1 = 1;
                    degree = 0;
                }
                else
                {
                    var tld = (load as PartialTrapezoidalLoad);

                    magnitude = (xi => (load as PartialTrapezoidalLoad).GetMagnitudesAt(xi, 0, 0)[0]);
                    localDir = tld.Direction;

                    if (tld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    xi0 = tld.StarIsoLocations[0];
                    xi1 = tld.EndIsoLocations[0];
                    degree = 1;
                }

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

                            var buf_ = new Matrix(new double[] { df});

                            return buf_;
                        }, 0, to, gpt);

                        integral = intgV.Integrate();
                    }

                    var f_i = integral[0, 0];

                    var memb = buf.MembraneTensor;
                    var bnd = buf.BendingTensor;

                    var v = memb.S11 = -(f_i + v0);

                    buf.MembraneTensor = memb;
                    buf.BendingTensor = bnd;

                    //return buf;
                }
            }



            #endregion

            throw new NotImplementedException();
        }

        public Displacement GetLoadDisplacementAt(Element targetElement, Load load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Displacement GetLocalDisplacementAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            var n = GetNMatrixAt(targetElement, isoCoords).ExtractRow(0);

            var u = new Matrix(targetElement.Nodes.Length, 1);

            for(var i=0;i< targetElement.Nodes.Length;i++)
                u[i, 0] = localDisplacements[i].DX;

            var buf = n * u;

            return new Displacement(buf[0, 0], 0, 0, 0, 0, 0);
        }

        public Force[] GetLocalEquivalentNodalLoads(Element targetElement, Load load)
        {
            var tr = targetElement.GetTransformationManager();

            #region uniform & trapezoid

            if (load is UniformLoad || load is PartialTrapezoidalLoad)
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

                    xi0 = -1;
                    xi1 = 1;
                    degree = 0;
                }
                else
                {
                    var tld = (load as PartialTrapezoidalLoad);

                    magnitude = (xi => (load as PartialTrapezoidalLoad).GetMagnitudesAt(xi, 0, 0)[0]);
                    localDir = tld.Direction;

                    if (tld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    xi0 = tld.StarIsoLocations[0];
                    xi1 = tld.EndIsoLocations[0];
                    degree = 1;
                }

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
                        shp.MultiplyByConstant(j.Determinant());

                        var q_ = localDir * q__;

                        shp.MultiplyByConstant(q_.X);

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

            throw new NotImplementedException();

            

            
            
        }
    }
}