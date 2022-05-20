using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Integration
{
    /// <summary>
    /// Utility class
    /// </summary>
    public class FunctionMatrixFunction:IMatrixFunction
    {
        public Func<double, double, double, Matrix> Function;

        public FunctionMatrixFunction(Func<double, double, double, Matrix> function)
        {
            Function = function;
        }

        public Matrix GetMatrix(double xi, double nu, double gamma)
        {
            return Function(xi, nu, gamma);
        }
    }
}
