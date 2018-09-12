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
    /// Represents a helper for shaft (torsion element).
    /// </summary>
    public class ShaftHelper : IElementHelper
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

            ThrowUtil.ThrowIf(!CalcUtil.IsIsotropicMaterial(mech), "anistropic material not impolemented yet");

            var g = mech.Ex / (2 * (1 + mech.NuXy));

            buf.FillRow(0, geo.J * g);

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


            var c1 = bar.StartReleaseCondition;
            var c2 = bar.EndReleaseCondition;

            if (c1.RX == DofConstraint.Released)
                n1 = 0;

            if (c2.RX == DofConstraint.Released)
                n2 = 0;


            var buf = new Matrix(1, 2);

            double[] arr;

            arr = new double[] {n1, n2};

            buf.FillRow(0, arr);

            return buf;
        }


        public Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var xi = isoCoords[0];

            Polynomial[] ns = null;

            {//retrieve or generate shapefunctions
                var nsKey = "{4EA77E8B-F44A-4524-9F1F-848A807106E6}";//a random unified key for store truss shape functions for bar element

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

        public Polynomial GetN_i(Element targetElement, int ith)
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
                if (bar.NodalReleaseConditions[i].RX == DofConstraint.Fixed)
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
            return ElementHelperExtensions.CalcLocalKMatrix_Bar(this, targetElement);
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
                new FluentElementPermuteManager.ElementLocalDof(0, DoF.Rx),
                new FluentElementPermuteManager.ElementLocalDof(1, DoF.Rx),
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

            u.FillColumn(0, ld[0].RX, ld[1].RX);

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

        public IEnumerable<Tuple<DoF, double>> GetLoadInternalForceAt(Element targetElement, Load load,
            double[] isoLocation)
        {
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

            for (var i = 0; i < targetElement.Nodes.Length; i++)
                u[i, 0] = localDisplacements[i].RX;

            var buf = n * u;

            return new Displacement(0, 0, 0, buf[0, 0], 0, 0);
        }

        public Force[] GetLocalEquivalentNodalLoads(Element targetElement, Load load)
        {
            if (load is UniformLoad)
            {
                return new Force[2];
            }

            if (load is PartialTrapezoidalLoad)
            {
                return new Force[2];
            }

            throw new NotImplementedException();
        }
    }
}