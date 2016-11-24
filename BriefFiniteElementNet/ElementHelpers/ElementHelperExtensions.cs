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

            var ng = (nb + nd + nj)/2 + 1;

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
    }
}
