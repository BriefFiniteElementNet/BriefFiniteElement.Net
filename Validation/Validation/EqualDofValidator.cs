using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation
{
    public class EqualDofValidator
    {
        public static void Test1()
        {
            var model = StructureGenerator.Generate3DBarElementGrid(1, 1, 3);

            var mpc = new MpcElements.TelepathyLink(model.Nodes[1], model.Nodes[2]);

            mpc.ConnectDx = true;
            mpc.ConnectDy = true;

            model.Nodes[2].Loads.Add(new NodalLoad(new Force(1, 2, 3, 4, 5, 6)));

            mpc.UseForAllLoads = true;

            model.MpcElements.Add(mpc);

            model.Solve_MPC();


        }
    }
}
