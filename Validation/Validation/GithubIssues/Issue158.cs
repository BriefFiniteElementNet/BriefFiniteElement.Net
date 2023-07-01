using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation.GithubIssues
{
    public class Issue158
    {
        public static void Run()
        {
            var sec = new UniformParametric1DSection(6400, 1706670, 1706670);
            var mat = new UniformIsotropicMaterial(9350, 0.3);

            var node1 = new BriefFiniteElementNet.Node(0, 0, 0) { Label = "n1" };
            var node2 = new BriefFiniteElementNet.Node(1000, 0, 0) { Label = "n2" };
            var node3 = new BriefFiniteElementNet.Node(2000, 0, 0) { Label = "n3" };

            node1.Constraints = Constraints.Fixed;
            node3.Constraints = Constraints.Fixed;

            var element1 = new BarElement(node1, node2) { Section = sec, Material = mat, };
            var element2 = new BarElement(node2, node3) { Section = sec, Material = mat, };

            element1.Loads.Add(new UniformLoad(LoadCase.DefaultLoadCase, new BriefFiniteElementNet.Vector(0, 1, 0), -0.0208, CoordinationSystem.Global));
            element2.Loads.Add(new UniformLoad(LoadCase.DefaultLoadCase, new BriefFiniteElementNet.Vector(0, 1, 0), -0.0208, CoordinationSystem.Global));

            var model = new Model();

            model.Nodes.Add(node1, node2, node3);
            model.Elements.Add(element1, element2);

            //model.Solve_MPC();
            model.Trace.Listeners.Add(new ConsoleTraceListener());

            PosdefChecker.CheckModel_mpc(model, LoadCase.DefaultLoadCase);
        }
    }
}
