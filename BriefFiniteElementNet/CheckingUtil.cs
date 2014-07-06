using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
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
    }
}
