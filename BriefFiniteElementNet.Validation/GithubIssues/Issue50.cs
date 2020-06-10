using BriefFiniteElementNet.Controls;
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
    public class Issue50
    {
        public static void Run1()
        {
            double h = 5;

            Node n1 = new Node(0, 0, 0);
            Node n2 = new Node(0, 0, h);
            n1.Constraints = Constraints.Fixed;

            var section = new UniformGeometric1DSection(SectionGenerator.GetRectangularSection(1, 0.5));
            var material = UniformIsotropicMaterial.CreateFromYoungShear(205e9, 81e9);

            BarElement e = new BarElement(n1, n2)
            {
                Section = section,
                Material = material
            };
            e.Behavior = BarElementBehaviours.FullFrame;


            var load = new UniformLoad()
            {
                Direction = Vector.FromXYZ(0, 0, -1),
                CoordinationSystem = CoordinationSystem.Global,
                Magnitude = 1
            };
            var load2 = new UniformLoad()
            {
                Direction = Vector.FromXYZ(1, 0, 0),
                CoordinationSystem = CoordinationSystem.Global,
                Magnitude = 10,
            };
            e.Loads.Add(load);
            e.Loads.Add(load2);

            Model model = new Model();
            model.Nodes.Add(n1);
            model.Nodes.Add(n2);
            model.Elements.Add(e);
            model.Solve_MPC();


            BarInternalForceVisualizer.VisualizeInNewWindow(e);
            {
                var elm = model.Elements[0] as BarElement;
                var fnc = new Func<double, double>(x =>
                {
                    try
                    {
                        var xi = elm.LocalCoordsToIsoCoords(x);
                        var frc = elm.GetExactInternalForceAt(xi[0]);
                        return frc.My;
                    }
                    catch
                    {

                        return 0;
                    }

                });

                FunctionVisualizer.VisualizeInNewWindow(fnc, 0, elm.GetElementLength(), 11);
            }
        }
    }
}
