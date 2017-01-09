using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCS = CSparse.Double.CompressedColumnStorage;

namespace BriefFiniteElementNet.Solver
{
    public static class Extensions
    {
        public static double[] Multiply(this CCS matrix, double[] vector)
        {
            var buf = new double[matrix.RowCount];

            matrix.Multiply(vector, buf);

            return buf;
        }
    }
}
