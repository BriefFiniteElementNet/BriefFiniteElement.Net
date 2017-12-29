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

        public static Matrix CalcLocalKMatrix_Bar(IElementHelper helper, Element targetElement)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var nb = helper.GetBMaxOrder(targetElement);
            var nd = elm.Material.GetMaxFunctionOrder();
            var nt = elm.Section.GetMaxFunctionOrder();
            var nj = helper.GetDetJOrder(targetElement);


            var sum = new int[3];

            foreach (var i in new int[][] { nb, nd, nb, nt, nj })
                for (int j = 0; j < 3; j++)
                    sum[j] += i[j];

            var nXi = sum[0] / 2 + 1;
            var nEta = sum[1] / 2 + 1;
            var nGama = sum[2] / 2 + 1;


            //var ng = (2*nb + nd + nj)/2 + 1;
            var intg = new GaussianIntegrator();

            intg.A1 = 0;
            intg.A2 = 1;

            intg.F1 = (gama => 0);
            intg.F2 = (gama => 1);
            intg.EtaPointCount = 1;

            intg.G1 = (eta, gamma) => -1;
            intg.G2 = (eta, gamma) => +1;

            intg.GammaPointCount = nGama;
            intg.XiPointCount = nXi;
            intg.EtaPointCount = nEta;

            intg.H = new FunctionMatrixFunction((xi, eta, gama) =>
            {
                var b = helper.GetBMatrixAt(elm, xi);
                var d = helper.GetDMatrixAt(elm, xi);
                var j = helper.GetJMatrixAt(elm, xi);

                var buf_ = b.Transpose() * d * b;
                buf_.MultiplyByConstant(Math.Abs(j.Determinant()));

                return buf_;
            });

            var res = intg.Integrate();

            return res;
        }

        public static Matrix CalcLocalMMatrix_Bar(IElementHelper helper, Element targetElement)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var nn = helper.GetNMaxOrder(targetElement);
            var nd = elm.Material.GetMaxFunctionOrder();
            var nt = elm.Section.GetMaxFunctionOrder();
            var nj = helper.GetDetJOrder(targetElement);

            var sum = new int[3];

            foreach (var i in new int[][] { nn, nd,  nt, nn, nj })
                for (int j = 0; j < 3; j++)
                    sum[j] += i[j];

            var nXi = sum[0] / 2 + 1;
            var nEta = sum[1] / 2 + 1;
            var nGama = sum[2] / 2 + 1;


            //var nRho = elm.Material.GetMaxFunctionOrder().Max() + elm.Section.GetMaxFunctionOrder();

            //var ng = (2*nn + nRho + nj)/2 + 1;

            var intg = new GaussianIntegrator();

            intg.GammaPointCount = nGama;
            intg.XiPointCount = nXi;
            intg.EtaPointCount = nEta;

            intg.A1 = 0;
            intg.A2 = 1;

            intg.F1 = (gama => 0);
            intg.F2 = (gama => 1);

            intg.G1 = (eta, gamma) => -1;
            intg.G2 = (eta, gamma) => +1;
            //intg.XiPointCount = ng;

            intg.H = new FunctionMatrixFunction((xi, eta, gama) =>
            {
                var n = helper.GetNMatrixAt(elm, xi);
                var rho = helper.GetRhoMatrixAt(elm, xi);
                var j = helper.GetJMatrixAt(elm, xi);

                var buf_ = n.Transpose() * rho * n;
                buf_.MultiplyByConstant(j.Determinant());

                return buf_;
            });

            var res = intg.Integrate();

            return res;
        }

        public static Matrix CalcLocalCMatrix_Bar(IElementHelper helper, Element targetElement)
        {
            var elm = targetElement as BarElement;

            if (elm == null)
                throw new Exception();

            var nn = helper.GetNMaxOrder(targetElement);
            var nd = elm.Material.GetMaxFunctionOrder();
            var nt = elm.Section.GetMaxFunctionOrder();
            var nj = helper.GetDetJOrder(targetElement);

            var sum = new int[3];

            foreach (var i in new int[][] { nn, nd, nt, nn, nj })
                for (int j = 0; j < 3; j++)
                    sum[j] += i[j];

            var nXi = sum[0] / 2 + 1;
            var nEta = sum[1] / 2 + 1;
            var nGama = sum[2] / 2 + 1;

            //var nRho = elm.Material.GetMaxFunctionOrder().Max() + elm.Section.GetMaxFunctionOrder();
            //var ng = (2 * nn + nRho + nj) / 2 + 1;

            var intg = new GaussianIntegrator();

            intg.GammaPointCount = nGama;
            intg.XiPointCount = nXi;
            intg.EtaPointCount = nEta;

            intg.A1 = 0;
            intg.A2 = 1;

            intg.F1 = (gama => 0);
            intg.F2 = (gama => 1);
            intg.EtaPointCount = 1;

            intg.G1 = (eta, gamma) => -1;
            intg.G2 = (eta, gamma) => +1;
            //intg.XiPointCount = ng;

            intg.H = new FunctionMatrixFunction((xi, eta, gama) =>
            {
                var n = helper.GetNMatrixAt(elm, xi);
                var mu = helper.GetMuMatrixAt(elm, xi);
                var j = helper.GetJMatrixAt(elm, xi);

                var buf_ = n.Transpose() * mu * n;
                buf_.MultiplyByConstant(j.Determinant());

                return buf_;
            });

            var res = intg.Integrate();

            return res;
        }

        public static Matrix CalcLocalKMatrix_Triangle(IElementHelper helper, Element targetElement)
        {
            var qq = targetElement as TriangleElement;

            if (qq == null)
                throw new Exception();

            var trans = qq.GetLambdaMatrix().Transpose();

            var nb = helper.GetBMaxOrder(targetElement);
            var nd = qq.Material.GetMaxFunctionOrder();
            var nt = qq.Section.GetMaxFunctionOrder();
            var nj = helper.GetDetJOrder(targetElement);

            var sum = new int[3];

            foreach (var i in new int[][] { nb, nd, nb, nt, nj })
                for (int j = 0; j < 3; j++)
                    sum[j] += i[j];

            var nXi = sum[0] / 2 + 1;
            var nEta = sum[1] / 2 + 1;
            var nGama = sum[2] / 2 + 1;

            var intg = new GaussianIntegrator();

            intg.GammaPointCount = nGama;
            intg.XiPointCount = nXi;
            intg.EtaPointCount = nEta;

            intg.A2 = 1;
            intg.A1 = 0;

            intg.F2 = (gama => 1);
            intg.F1 = (gama => 0);

            intg.G2 = ((eta, gama) => 1 - eta);
            intg.G1 = ((eta, gama) => 0);

            intg.H = new FunctionMatrixFunction((xi, eta, gama) =>
            {
                var b = helper.GetBMatrixAt(qq, xi, eta);
                var d = helper.GetDMatrixAt(qq, xi, eta);
                var j = helper.GetJMatrixAt(qq, xi, eta);

                var buf = MatrixPool.Allocate(b.ColumnCount, b.ColumnCount);

                CalcUtil.Bt_D_B(b, d, buf);

                buf.MultiplyByConstant((j.Determinant()));

                return buf;
            });

            var res = intg.Integrate();

            return res;
        }

        
    }
}
