using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation
{
    public class PosdefChecker
    {
        public static void CheckModel(Model model,LoadCase lc)
        {
            model.ReIndexNodes();


            var fullst = MatrixAssemblerUtil.AssembleFullStiffnessMatrix(model);

            var mgr = DofMappingManager.Create(model, lc);

            var dvd = CalcUtil.GetReducedZoneDividedMatrix(fullst, mgr);

            var stiffness = dvd.ReleasedReleasedPart;


            var n = stiffness.ColumnCount;

            for (int i = 0; i < n; i++)
            {
                var t = stiffness.At(i, i);

                if (t > 0)
                    continue;

                var m1 = mgr.RMap2[i];
                var m2 = mgr.RMap1[m1];

                var nodeNum = m2/6;

                if (t == 0)
                    model.Trace.Write(TraceLevel.Warning, "DoF {0} of Node {1} not properly constrained", m2%6, nodeNum);
                else//t < 0
                    model.Trace.Write(TraceLevel.Warning, "DoF {0} of Node {1} not member", m2 % 6, nodeNum);
            }
        }
    }
}
