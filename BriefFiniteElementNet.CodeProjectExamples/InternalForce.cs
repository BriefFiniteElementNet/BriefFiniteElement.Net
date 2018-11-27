using BriefFiniteElementNet.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.CodeProjectExamples
{
    public class InternalForceExample
    {
        public static void Run()
        {
            var model = new Model();

            model.Nodes.Add(new Node(0, 0, 0) { Label = "n0" });
            model.Nodes.Add(new Node(4, 0, 0) { Label = "n1" });

            model.Elements.Add(new BarElement(model.Nodes["n0"], model.Nodes["n1"]) { Label = "e0" });

            model.Nodes["n0"].Constraints =
                model.Nodes["n1"].Constraints =
                    Constraints.Fixed;

            var sec = new Sections.UniformGeometric1DSection(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));
            var mat = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);

            (model.Elements["e0"] as BarElement).Material = mat;

            (model.Elements["e0"] as BarElement).Section = sec;

            var u1 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, -Vector.K, 1, CoordinationSystem.Global);

            model.Elements["e0"].Loads.Add(u1);

            model.Solve_MPC();

            var n1Force = model.Nodes["n1"].GetSupportReaction();
            Console.WriteLine("support reaction of n1: {0}", n1Force);
            var elm = model.Elements[0] as BarElement;

            var frc = elm.GetExactInternalForceAt(0.00);
        }
    }
}
