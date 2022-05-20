using BriefFiniteElementNet.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation.GithubIssues
{
    public class Issue23
    {
        public static void Run()
        {
            var model = new Model();

            var l = 5.0;

            var n1 = new Node() { Constraints = Constraints.Fixed };
            var n2 = new Node(l,0,0) { Constraints = Constraints.Fixed };

            var elm = new BarElement();

            elm.Material = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.25);
            elm.Section = new Sections.UniformGeometric1DSection(SectionGenerator.GetRectangularSection(0.05, 0.05));

            elm.Nodes[0] = n1;
            elm.Nodes[1] = n2;

           // elm.EndReleaseCondition = Constraints.RotationFixed;

            var load = new Loads.UniformLoad(LoadCase.DefaultLoadCase, Vector.K, 100, CoordinationSystem.Global);
            //var load2 = new Loads.ConcentratedLoad();// LoadCase.DefaultLoadCase, Vector.K, -100, CoordinationSystem.Global);
            //l/oad2.Force = new Force(0, 100, 100, 0, 0, 0);
            //load2.ForceIsoLocation = new IsoPoint(0.5, 0, 0);

            elm.Loads.Add(load);

            model.Elements.Add(elm);
            model.Nodes.Add(n1, n2);

            model.Solve_MPC();

            var fnc = new Func<double, double>(x =>
            {
                try
                {
                    var xi = elm.LocalCoordsToIsoCoords(x);
                    var frc = elm.GetExactInternalForceAt(xi[0]);
                    return frc.Fz;
                }
                catch
                {
                    return 0;
                }
            });

            //Controls.FunctionVisualizer.VisualizeInNewWindow(fnc, 1E-6, l-1E-6, 500);
        }
    }
}
