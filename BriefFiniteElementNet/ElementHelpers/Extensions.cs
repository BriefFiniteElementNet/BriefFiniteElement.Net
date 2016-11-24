using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;

namespace BriefFiniteElementNet.ElementHelpers
{
    internal static class Extensions
    {
        public static Matrix CalcLocalKMatrix_Bar(this IElementHelper helper, Element targetElement, Matrix transformMatrix)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var trans = elm.GetTransformationMatrix();

            var bar = elm;

            var n1 = bar.Material.GetGaussianIntegrationPoints();
            var n2 = bar.Section.GetGaussianIntegrationPoints();
            var n3 = helper.GetGaussianIntegrationPointCount(elm, trans);

            var intg = new GaussianIntegrator();

            intg.A1 = 0;
            intg.A2 = 1;
            intg.GammaPointCount = 1;

            intg.F1 = (gama => 0);
            intg.F2 = (gama => 1);
            intg.EtaPointCount = 1;

            intg.G1 = (eta, gamma) => -1;
            intg.G2 = (eta, gamma) => +1;
            intg.XiPointCount = (new int[] { n1, n2, n3 }).Max() + 1;

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
    }
}
