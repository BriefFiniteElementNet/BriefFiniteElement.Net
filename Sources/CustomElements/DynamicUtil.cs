using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Mathh;
using BriefFiniteElementNet.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Elements
{
    public class DynamicUtil
    {
        public CSparse.Storage.CompressedColumnStorage<double> K, M, C, Kr, Mr, Cr;

        public void GetMatrices(Model parent, LoadCase loadCase)
        {
            var n = parent.Nodes.Count * 6;
            var dt = new double[n];//total delta 

            ISolver solver;

            var sp = Stopwatch.StartNew();


            var permCalculator = new CsparsenetQrDisplacementPermutationCalculator();

            var perm =
                SolverUtils.GenerateP_Delta_Mpc(parent, loadCase, permCalculator);


            parent.Trace.Write(Common.TraceLevel.Info, "Calculating Displacement Permutation Matrix took {0} ms", sp.ElapsedMilliseconds);

            //var rd = perm.Item2;

            var pd = perm.Item1;

            //var todo = pd.ToDenseMatrix();

            sp.Restart();

            var kt = K = MatrixAssemblerUtil.AssembleFullStiffnessMatrix(parent);//total stiffness matrix, same for all load cases
            var mt = M = MatrixAssemblerUtil.AssembleFullMassMatrix(parent);//total mass matrix, same for all load cases
            var ct = C = MatrixAssemblerUtil.AssembleFullDampingMatrix(parent);//total damp matrix, same for all load cases


            parent.Trace.Write(Common.TraceLevel.Info, "Assemble Full Stiffness Matrix took {0} ms", sp.ElapsedMilliseconds);


            if (perm.Item1.RowCount > 0 && perm.Item1.RowCount > 0)
            {
                var pf = pd.Transpose();

                Kr = pf.Multiply(kt).Multiply(pd);
                Mr = pf.Multiply(mt).Multiply(pd);
                Cr = pf.Multiply(ct).Multiply(pd);
            }

        }
    }
}
