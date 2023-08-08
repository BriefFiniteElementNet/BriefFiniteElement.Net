using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using System.Xml.Linq;

namespace BriefFiniteElementNet.Validation.GithubIssues
{
    public class Issue160
    {
        public static void Run()
        {
            var h = 0.2;//200mm
            var w = 0.1;//100mm
            var e = 210e9;
            var m = 1000.0;

            Node node1, node2;
            BarElement element1;
            Model model;

            {
                var geo = SectionGenerator.GetRectangularSection(h, w);
                var sec = new UniformGeometric1DSection(geo);

                var mat = new UniformIsotropicMaterial(e, 0.3);

                node1 = new BriefFiniteElementNet.Node(0, 0, 0) { Label = "n1" };
                node2 = new BriefFiniteElementNet.Node(10, 0, 0) { Label = "n2" };

                node1.Constraints = Constraints.Fixed;

                element1 = new BarElement(node1, node2) { Section = sec, Material = mat, };

                element1.Loads.Add(new UniformLoad(LoadCase.DefaultLoadCase, -Vector.K, m, CoordinationSystem.Global));

                model = new Model();

                model.Nodes.Add(node1, node2);
                model.Elements.Add(element1);

                model.Trace.Listeners.Add(new ConsoleTraceListener());

                model.Solve_MPC();
            }
           

            var d1 = node2.GetNodalDisplacement();

            

            {
                model.LastResult.Clear();

                element1.WebRotation = 30;

                model.Solve_MPC();
            }
            

            var d2 = node2.GetNodalDisplacement();



        }
    }
}
