using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation
{
    public class ModelStaticEquibChecker
    {
        public static void Check(Model model, LoadCase cse)
        {
            var n = model.Nodes.Count;

            var ft = new Vector();
            var fs = new Vector[n];
            var mt = new Vector();
            
            model.Solve();

            model.LastResult.AddAnalysisResultIfNotExists(cse);


            var css = model.LastResult.ConcentratedForces[cse];//concentrated s
            var ess = model.LastResult.ElementForces[cse];//elements

            for (var i = 0; i < n; i++)
            {
                var loc = model.Nodes[i].Location;

                var cs = Force.FromVector(css, 6 * i);
                var es = Force.FromVector(ess, 6 * i);
                var ss = model.Nodes[i].GetSupportReaction(cse);

                var fi = (cs + es + ss).Move(-(Vector) loc);

                fs[i] = fi.Forces;
                
                ft += fi.Forces;
                mt += fi.Moments;
            }

            Console.WriteLine("Total Force: {0}", ft);
            Console.WriteLine("Total Moment: {0}", mt);
        }
    }
}
