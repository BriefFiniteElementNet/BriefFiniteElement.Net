using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.Validation.GithubIssues
{
    public class Issue100
    {
        public static void Run()
        {
            var model = StructureGenerator.Generate3DTetrahedralElementGrid(10, 10, 201, 1, 1, 1);

            var e = 210e9;
            var miu = 0.28;

            foreach (var elm in model.Elements)
            {
                if (elm is TetrahedronElement tet)
                {
                    tet.Material = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(e, miu);
                    tet.FixNodeOrder();
                }
            }

            var dx = model.Nodes.Max(i => i.Location.X) - model.Nodes.Min(i => i.Location.X);
            var dy = model.Nodes.Max(i => i.Location.Y) - model.Nodes.Min(i => i.Location.Y);
            var dz = model.Nodes.Max(i => i.Location.Z) - model.Nodes.Min(i => i.Location.Z);

            var l = dz;// model.Nodes.Max(i => i.Location.Z);

            var cnt = model.Nodes.Where(i => i.Location.Z == l);

            var f = 1e7;
            var I = dy * dx * dx * dx / 12;
            var rigid = new MpcElements.RigidElement_MPC() { UseForAllLoads = true };
            
            foreach (var node in cnt)
            {
                node.Loads.Add(new NodalLoad(new Force(f / cnt.Count(), 0, 0, 0, 0, 0)));
                rigid.Nodes.Add(node);
                //node.Constraints = Constraints.Released;
            }

            //model.MpcElements.Add(rigid);
            model.Trace.Listeners.Add(new ConsoleTraceListener());
            model.Solve_MPC();

            //Controls.ModelVisualizerControl.VisualizeInNewWindow(model);

            var delta = f * l * l * l / (3 * e * I);

            var t = cnt.Average(i => i.GetNodalDisplacement().DX);

            var ratio = delta / t;
        }
    }
}
