using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation
{
    public class NodeInternalExternalEquibliriumChecker
    {
        public static void CheckForEquiblirium(Model model, LoadCase lc)
        {
            if (model.Elements.Any(i => !(i is FrameElement2Node)))
                throw new Exception();

            //if (model.Elements.Where(i=>i is FrameElement2Node).Cast<FrameElement2Node>().Any(i => i.HingedAtEnd||i.HingedAtStart))
            //throw new Exception();

            if (model.RigidElements.Any())
                throw new Exception();

            var elms = model.Elements.Cast<FrameElement2Node>().ToArray();

            var cmb = new LoadCombination();
            cmb[lc] = 1.0;


            var l = new Func<FrameElement2Node, double>(e => (e.StartNode.Location - e.EndNode.Location).Length);

            model.Solve();

            Console.WriteLine("Checking for force equilibrium on every node!");

            var maxF = new Vector();
            var maxM = new Vector();


            for (int idx = 0; idx < model.Nodes.Count; idx++)
            {
                var nde = model.Nodes[idx];
                
                var els = elms.Where(i => i.Nodes.Contains(nde)).ToArray();

                if (els.Length == 0)
                    continue;

                var eq1 = new Force();//forces from elements
                var eq2 = new Force();//forces from external nodal loads
                var eq3 = new Force();//forces from support reaction

                foreach (var elm in els)
                {
                    var fc = new Force();

                    if (elm.StartNode == nde)
                        fc = elm.GetInternalForceAt(0, cmb);
                    else
                        fc = -elm.GetInternalForceAt(l(elm), cmb);

                    var fc2 = elm.TransformLocalToGlobal(fc);

                    eq1 += fc2;

                }

                foreach (var ld in nde.Loads)
                {
                    if (ld.Case == lc)
                        eq2 += ld.Force;
                }

                var sp = nde.GetSupportReaction(cmb);

                eq3 += sp;


                var eq = eq1 + eq2 + eq3;

                /*
                Console.WriteLine("Sum of forces from elements on node # {0}: Force: {1}, Moment: {2}", idx, eq1.Forces,
                    eq1.Moments);

                Console.WriteLine("Sum of external forces on node # {0}: Force: {1}, Moment: {2}", idx, eq2.Forces,
                    eq2.Moments);

                Console.WriteLine("Sum of reaction forces on node # {0}: Force: {1}, Moment: {2}", idx, eq3.Forces,
                    eq3.Moments);
                */

                //Console.WriteLine("========================================");

                Console.WriteLine("Total force on node # {0}: Force: {1}, Moment: {2}", idx, eq.Forces,
                    eq.Moments);

                //Console.WriteLine("========================================");

                if (eq.Forces.Length > 1 || eq.Moments.Length > 1)
                    Console.WriteLine("total forces on ");

                if (eq.Forces.Length > maxF.Length)
                    maxF = eq.Forces;

                if (eq.Moments.Length > maxM.Length)
                    maxM = eq.Moments;

            }

            Console.WriteLine("");
            Console.WriteLine("======================================== Summary");

            Console.WriteLine("Maximum non equilibrated force: {0}", maxF);
            Console.WriteLine("Maximum non equilibrated moment: {0}", maxM);

        }
    }
}
