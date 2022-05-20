using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Common;

namespace BriefFiniteElementNet.Integration
{
    /// <summary>
    /// gets the helper and element, then gives Bt*d*B*|J| as result
    /// </summary>
    public class MultiplierMatrixFunction : IMatrixFunction
    {
        IElementHelper helper;

        Element targetElement;

        public MultiplierMatrixFunction(Element targetElement, IElementHelper helper)
        {
            this.helper = helper;
            this.targetElement = targetElement;
        }

        public Matrix GetMatrix(double xi, double eta, double gamma)
        {
            var elm = targetElement;

            var b = helper.GetBMatrixAt(elm, xi);
            var d = helper.GetDMatrixAt(elm, xi);
            var j = helper.GetJMatrixAt(elm, xi);

            var buf_ =
                targetElement.MatrixPool.Allocate(b.ColumnCount, b.ColumnCount);

            b.TransposeMultiply(b, buf_);

            //var buf_2 = b.Transpose() * d * b;
            buf_.Scale(d[0, 0] * Math.Abs(j.Determinant()));

            b.ReturnToPool();
            d.ReturnToPool();
            j.ReturnToPool();

            return buf_;
        }
    }
}
