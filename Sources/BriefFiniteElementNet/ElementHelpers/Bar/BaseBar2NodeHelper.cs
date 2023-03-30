using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Elements;
using CSparse.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers.Bar
{
    //bar element with two nodes, no partial connections
    public abstract class BaseBar2NodeHelper : IElementHelper
    {
        public BaseBar2NodeHelper(Element targetElement)
        {
            TargetElement = targetElement;
        }


        #region statics
        private static void EnsureBarTwoNode(Element elm)
        {
            var b2 = elm as BarElement;

            var flag = false;

            if (b2 == null)
                flag = true;
            else
                if (b2.NodeCount == 2) flag = true;

            if (flag)
                throw new Exception("BarElement with More than 2 node is not supported");
        }


        public static double GetLength(Element elm)
        {
            var bar = elm as BarElement;
            var l = (bar.Nodes[1].Location - bar.Nodes[0].Location).Length;
            return l;
        }

        public static double GetJ(Element elm)
        {
            //we need J = ∂X / ∂ξ

            return GetLength(elm) / 2;
        }

        public static double Iso2Local(Element targetElement, double xi)
        {
            //x = l/2 * (xi+1) =  j * (xi + 1)
            var bar = targetElement as BarElement;
            var l = GetLength(bar);
            var j = GetJ(bar);

            var b1 = l * (xi + 1) / 2;
            var b2 = j * (xi + 1);

            return b2;
        }

        public static double Local2Iso(Element targetElement, double x)
        {
            //x*2/l-1 = x / j - 1 = (xi+1)

            var bar = targetElement as BarElement;
            var l = GetLength(bar);
            var j = GetJ(bar);

            var b1 = 2 * x / l - 1;
            var b2 = x / j - 1;

            return b2;

        }

        #endregion


        public Element TargetElement { get; set; }

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
        public double[] Iso2Local(Element targetElement, params double[] isoCoords)
        {
            var buf = Iso2Local(targetElement, isoCoords[0]);
            return new double[] { buf };
        }

        /// <inheritdoc/>
        public double[] Local2Iso(Element targetElement, params double[] localCoords)
        {
            var buf = Local2Iso(targetElement, localCoords[0]);
            return new double[] { buf };
        }

        /// <inheritdoc/>
        public Matrix GetJMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var j = GetJ(targetElement);

            var buf = new Matrix(1, 1);

            buf[0, 0] = j;

            return buf;
        }

        /// <inheritdoc/>
        public int[] GetDetJOrder(Element targetElement)
        {
            return new int[] { 0, 0, 0 };
        }

        /// <inheritdoc/>
        public int[] GetNMaxOrder(Element targetElement)
        {
            return new int[] { GetNOrder(), 0, 0 };
        }

        /// <inheritdoc/>
        public int[] GetBMaxOrder(Element targetElement)
        {
            return new int[] { GetBOrder(), 0, 0 };
        }

        public ElementPermuteHelper.ElementLocalDof[] GetDofOrder(Element targetElement)
        {
            var n = targetElement.Nodes.Length;
            var dpn = GetDofsPerNode();
            var m = dpn.Length;

            var buf = new ElementPermuteHelper.ElementLocalDof[n * m];

            for (var i = 0; i < n; i++)
                for (var j = 0; j < dpn.Length; j++)
                {
                    buf[m * i + j] = new ElementPermuteHelper.ElementLocalDof(i, dpn[j]);
                }

            return buf;
        }


        //what dofs for each node, order matters, same as B or N matrix order
        public abstract DoF[] GetDofsPerNode();

        protected abstract int GetBOrder();


        protected abstract int GetNOrder();


        public abstract Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords);

        public abstract Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords);




        public abstract Displacement GetLoadDisplacementAt(Element targetElement, ElementalLoad load, double[] isoLocation);

        public abstract IEnumerable<Tuple<DoF, double>> GetLoadInternalForceAt(Element targetElement, ElementalLoad load, double[] isoLocation);

        public abstract GeneralStressTensor GetLoadStressAt(Element targetElement, ElementalLoad load, double[] isoLocation);

        public abstract Force[] GetLocalEquivalentNodalLoads(Element targetElement, ElementalLoad load);


        public abstract Displacement GetLocalDisplacementAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords);

        public abstract GeneralStressTensor GetLocalInternalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords);

        public abstract IEnumerable<Tuple<DoF, double>> GetLocalInternalForceAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords);



        public Matrix GetMuMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var xi = isoCoords[0];

            var rho = GetRho(elm, xi);

            var buf = Matrix.OfVector(rho);//1x1 matrix

            return buf;
        }

        public Matrix GetDMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var xi = isoCoords[0];

            var d = GetD(elm, xi);

            var buf = Matrix.OfVector(d);//1x1 matrix

            return buf;
        }

        public Matrix GetRhoMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var xi = isoCoords[0];

            var d = GetRho(elm, xi);

            var buf = Matrix.OfVector(d);//1x1 matrix

            return buf;
        }



        ///geometric (A,J,Iy or Iz)
        //public abstract double GetGeo(BarElement targetElement, double xi);

        //damp
        public abstract double GetMu(BarElement targetElement, double xi);

        //mass
        public abstract double GetRho(BarElement targetElement, double xi);

        //elastic modulus
        public abstract double GetD(BarElement targetElement, double xi);


    }
}
