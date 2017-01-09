using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSparse;
using CSparse.Double;

namespace BriefFiniteElementNet
{
    [Obsolete]
    public class CheckingUtil
    {
        
        public static bool IsInStaticEquilibrium(StaticLinearAnalysisResult res, LoadCase cse)
        {
            var allForces = new Force[res.Parent.Nodes.Count];


            var forceVec = res.Forces[cse];

            for (int i = 0; i < allForces.Length; i++)
            {
                var force = Force.FromVector(forceVec, 6*i);
                allForces[i] = force;
            }


            var ft = allForces.Select((i, j) => i.Move(res.Parent.Nodes[j].Location, new Point())).Sum();

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the residual of Ax-b.
        /// </summary>
        /// <param name="A">A.</param>
        /// <param name="x">The x.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static double GetResidual(CompressedColumnStorage A, double[] x, double[] b)
        {
            var buf = 0.0;

            var n = b.Length;

            var recoveredB = new double[n];

            A.Multiply(x, recoveredB);

            for (var i = 0; i < n; i++)
                recoveredB[i] -= b[i];

            //var norm = recoveredB.GetLargestAbsoluteValue();

            return
                recoveredB.Average();
                //Norm(recoveredB) / (A.Norm(MatrixNorm.OneNorm) * Norm(x) + Norm(b));

            return buf;
        }

        // infinity-norm of x
        static double Norm(double[] x)
        {
            int i;
            double normx = 0;
            var n = x.Length;
            for (i = 0; i < n; i++)
                normx = Math.Max(normx, Math.Abs(x[i]));
            return (normx);
        }
    }
}
