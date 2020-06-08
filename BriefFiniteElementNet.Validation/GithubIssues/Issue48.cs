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
    public class Issue48
    {
        public static void Run1()
        {
            var l = 10;
            var model = new Model();
            model.Nodes.Add(new Node(0, 0, 0) { Label = "n0" });
            model.Nodes.Add(new Node(l, 0, 0) { Label = "n1" });
            //Fixed nodes
            model.Nodes["n0"].Constraints = Constraints.Fixed;
            model.Nodes["n1"].Constraints = Constraints.Fixed;

            var bar = new BarElement(model.Nodes["n0"], model.Nodes["n1"]) { Label = "e0" };
            //Pinned bar releases
            
            bar.StartReleaseCondition = Constraints.MovementFixed;
            
            bar.EndReleaseCondition = Constraints.MovementFixed & Constraints.FixedRX;

            model.Elements.Add(bar);

            var sec = new UniformGeometric1DSection(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));
            var mat = UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);

            (model.Elements["e0"] as BarElement).Material = mat;
            (model.Elements["e0"] as BarElement).Section = sec;

            var u1 = new UniformLoad(LoadCase.DefaultLoadCase, -Vector.K, 1, CoordinationSystem.Global);

            model.Elements["e0"].Loads.Add(u1);

            model.Solve_MPC();

            var n0Force = model.Nodes["n0"].GetSupportReaction();
            var n1Force = model.Nodes["n1"].GetSupportReaction();
            Console.WriteLine("support reaction of n0: {0}, n1: {1}", n0Force, n1Force);

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

            FunctionVisualizer.VisualizeInNewWindow(fnc, 1E-6, l - 1E-6, 11);
        }
    }
}
