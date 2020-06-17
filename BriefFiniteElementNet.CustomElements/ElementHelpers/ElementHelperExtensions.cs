//---------------------------------------------------------------------------------------
//
// Project: VIT-V
//
// Program: BriefFiniteElement.Net - ElementHelpersExtension.cs
//
// Revision History
//
// Date          Author          	            Description
// 17.06.2020    T.Thaler, M.Mischke     	    v1.0  
// 
//---------------------------------------------------------------------------------------
// Copyleft 2017-2020 by Brandenburg University of Technology. Intellectual proprerty 
// rights reserved in terms of LGPL license. This work is a research work of Brandenburg
// University of Technology. Use or copying requires an indication of the authors reference.
//---------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.ElementHelpers;

namespace BriefFiniteElementNet.Elements.ElementHelpers
{
    // not working so far
    public static class ElementHelperExtensions
    {
        public static Matrix CalcLocalKMatrix_Quad(IElementHelper helper, Element targetElement)    // function is not recognized 
        {
            var qq = targetElement as QuadrilaturalElement; 

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

            intg.F2 = (gama => +1);
            intg.F1 = (gama => -1);

            intg.G2 = (eta, gama) => +1;
            intg.G1 = (eta, gama) => -1;   // formula 4.53 (Development of Membrane, Plate and Flat Shell Elements in Java) --> see GaussianIntegrator.cs for correct variable assignment

            intg.H = new FunctionMatrixFunction((xi, eta, gama) =>
            {
                var b = helper.GetBMatrixAt(qq, xi, eta);
                var d = helper.GetDMatrixAt(qq, xi, eta);
                var j = helper.GetJMatrixAt(qq, xi, eta);
                var detj = Math.Abs(j.Determinant());

                var buf = new Matrix(b.ColumnCount, b.ColumnCount); // result matrix

                CalcUtil.Bt_D_B(b, d, buf);                         // multiplicates three matrices

                buf.MultiplyByConstant(detj);

                return buf;
            });


            var res = intg.Integrate();

            return res;
        }
    }
}

