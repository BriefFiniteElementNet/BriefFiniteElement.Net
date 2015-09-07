using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation
{
    /// <summary>
    /// Checks for every element to be statically stable
    /// </summary>
    public class ElementEquibiliriumChecker
    {
        public static void Check(Model model, LoadCase lc)
        {
            if (model.Elements.Any(i => !(i is FrameElement2Node)))
                throw new Exception();

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

            for(var idx=0;idx<elms.Length;idx++)
            {
                var elm = elms[idx];

                var f1 = elm.GetInternalForceAt(0, cmb);
                var f2 = elm.GetInternalForceAt(l(elm), cmb);

                
                var efs = new Force[2];

                foreach (var ld in elm.Loads)
                {
                    if (ld.Case != lc)
                        continue;

                    var tg = ld.GetGlobalEquivalentNodalLoads(elm);

                    var f1l = elm.TransformGlobalToLocal(tg[0]);
                    var f2l = elm.TransformGlobalToLocal(tg[1]);

                    efs[0] += f1l;
                    efs[1] += f2l;
                }

                var ft1 = efs[0] - f1;
                var ft2 = efs[1] - f2;

                var eq = ft1 - ft2.Move(new Vector(-l(elm), 0, 0));

                Console.WriteLine("Total loads on element # {0}: Force: {1}, Moment: {2}", idx, eq.Forces,
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
