using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;

namespace BriefFiniteElementNet.ElementHelpers
{
    internal static class ElementHelperExtensions
    {
        public static Matrix CalcLocalKMatrix_Bar(IElementHelper helper, Element targetElement, Matrix transformMatrix)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var trans = elm.GetTransformationMatrix();

            var nb = helper.GetBMaxOrder(targetElement, transformMatrix);
            var nd = elm.Material.GetMaxFunctionOrder() + elm.Section.GetMaxFunctionOrder();
            var nj = helper.GetDetJOrder(targetElement, transformMatrix);

            var ng = (2*nb + nd + nj)/2 + 1;
            var intg = new GaussianIntegrator();

            intg.A1 = 0;
            intg.A2 = 1;
            intg.GammaPointCount = 1;

            intg.F1 = (gama => 0);
            intg.F2 = (gama => 1);
            intg.EtaPointCount = 1;

            intg.G1 = (eta, gamma) => -1;
            intg.G2 = (eta, gamma) => +1;

            intg.XiPointCount = ng;

            intg.H = new FunctionMatrixFunction((xi, eta, gama) =>
            {
                var b = helper.GetBMatrixAt(elm, trans, xi);
                var d = helper.GetDMatrixAt(elm, trans, xi);
                var j = helper.GetJMatrixAt(elm, trans, xi);

                var buf_ = b.Transpose() * d * b;
                buf_.MultiplyByConstant(j.Determinant());

                return buf_;
            });

            var res = intg.Integrate();

            return res;
        }

        public static Matrix CalcLocalMMatrix_Bar(IElementHelper helper, Element targetElement, Matrix transformMatrix)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var trans = elm.GetTransformationMatrix();

            var nn = helper.GetNMaxOrder(targetElement, transformMatrix);
            
            var nRho = elm.Material.GetMaxFunctionOrder() + elm.Section.GetMaxFunctionOrder();

            var nj = helper.GetDetJOrder(targetElement, transformMatrix);

            var ng = (2*nn + nRho + nj)/2 + 1;

            var intg = new GaussianIntegrator();

            intg.A1 = 0;
            intg.A2 = 1;
            intg.GammaPointCount = 1;

            intg.F1 = (gama => 0);
            intg.F2 = (gama => 1);
            intg.EtaPointCount = 1;

            intg.G1 = (eta, gamma) => -1;
            intg.G2 = (eta, gamma) => +1;
            intg.XiPointCount = ng;

            intg.H = new FunctionMatrixFunction((xi, eta, gama) =>
            {
                var n = helper.GetNMatrixAt(elm, trans, xi);
                var rho = helper.GetRhoMatrixAt(elm, trans, xi);
                var j = helper.GetJMatrixAt(elm, trans, xi);

                var buf_ = n.Transpose() * rho * n;
                buf_.MultiplyByConstant(j.Determinant());

                return buf_;
            });

            var res = intg.Integrate();

            return res;
        }

        [Obsolete("Use two node approach")]
        public static void RemoveReleasedMembers_bar(IElementHelper helper, Element targetElement,Matrix bMatrix)
        {
            return;
            /*
            var bar = targetElement as BarElement;

            var order = helper.GetDofOrder(targetElement);

            var endReleases = new[]
            {
                bar.StartConnection.Dx,
                bar.StartConnection.Dy,
                bar.StartConnection.Dz,

                bar.StartConnection.Rx,
                bar.StartConnection.Ry,
                bar.StartConnection.Rz,

                bar.EndConnection.Dx,
                bar.EndConnection.Dy,
                bar.EndConnection.Dz,

                bar.EndConnection.Rx,
                bar.EndConnection.Ry,
                bar.EndConnection.Rz,
            };

            for (var i = 0; i < endReleases.Length; i++)
            {
                if (endReleases[i] == DofConstraint.Fixed)
                    continue;

                var nodeNum = i/6;
                var dof = (DoF) (i%6);

                var idx = order.FirstIndexOf(new FluentElementPermuteManager.ElementLocalDof(nodeNum, dof));

                if (idx == -1)
                    continue;

                bMatrix[0, idx] = 0;
            }
            */
        }

        public static Matrix CalcLocalCMatrix_Bar(IElementHelper helper, Element targetElement, Matrix transformMatrix)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var trans = elm.GetTransformationMatrix();

            var nn = helper.GetNMaxOrder(targetElement, transformMatrix);

            var nRho = elm.Material.GetMaxFunctionOrder() + elm.Section.GetMaxFunctionOrder();

            var nj = helper.GetDetJOrder(targetElement, transformMatrix);

            var ng = (2 * nn + nRho + nj) / 2 + 1;

            var intg = new GaussianIntegrator();

            intg.A1 = 0;
            intg.A2 = 1;
            intg.GammaPointCount = 1;

            intg.F1 = (gama => 0);
            intg.F2 = (gama => 1);
            intg.EtaPointCount = 1;

            intg.G1 = (eta, gamma) => -1;
            intg.G2 = (eta, gamma) => +1;
            intg.XiPointCount = ng;

            intg.H = new FunctionMatrixFunction((xi, eta, gama) =>
            {
                var n = helper.GetNMatrixAt(elm, trans, xi);
                var mu = helper.GetMuMatrixAt(elm, trans, xi);
                var j = helper.GetJMatrixAt(elm, trans, xi);

                var buf_ = n.Transpose() * mu * n;
                buf_.MultiplyByConstant(j.Determinant());

                return buf_;
            });

            var res = intg.Integrate();

            return res;
        }

        public static Matrix CalcLocalKMatrix_Triangle(IElementHelper helper, Element targetElement, Matrix transformMatrix)
        {
            var tri = targetElement as TriangleElement;

            if (tri == null)
                throw new Exception();

            var trans = tri.GetTransformationMatrix();

            var nb = helper.GetBMaxOrder(targetElement, transformMatrix);
            var nd = tri.Material.GetMaxFunctionOrder();
            var nt = tri.Section.GetMaxFunctionOrder();
            var nj = helper.GetDetJOrder(targetElement, transformMatrix);

            var ng = (nb + nd + nt + nj)/2 + 1;

            var intg = new GaussianIntegrator();

            intg.A2 = 1;
            intg.A1 = 0;

            intg.F2 = (gama => 1);
            intg.F1 = (gama => 0);

            intg.G2 = ((eta, gama) => 1 - eta);
            intg.G1 = ((eta, gama) => 0);

            intg.GammaPointCount = 1;
            intg.XiPointCount = intg.EtaPointCount = ng;

            intg.H = new FunctionMatrixFunction((xi, eta, gama) =>
            {
                var b = helper.GetBMatrixAt(tri, trans, xi, eta);
                var d = helper.GetDMatrixAt(tri, trans, xi, eta);
                var j = helper.GetJMatrixAt(tri, trans, xi, eta);
                var t = tri.Section.GetThicknessAt(xi, eta);

                var buf = b.Transpose() * d * b;
                buf.MultiplyByConstant(j.Determinant());
                buf.MultiplyByConstant(t);

                return buf;
            });

            var res = intg.Integrate();

            return res;
        }
    }
}
