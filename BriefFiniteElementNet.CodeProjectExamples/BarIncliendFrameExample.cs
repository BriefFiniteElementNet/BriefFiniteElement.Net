using BriefFiniteElementNet.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.CodeProjectExamples
{
    class BarIncliendFrameExample
    {
        public void Run()
        {
            var model = new Model();

            model.Nodes.Add(new Node(-10, 0, 0) { Label = "n0" });
            model.Nodes.Add(new Node(-10, 0, 6) { Label = "n1" });
            model.Nodes.Add(new Node(0, 0, 8) { Label = "n2" });
            model.Nodes.Add(new Node(10, 0, 6) { Label = "n3" });
            model.Nodes.Add(new Node(10, 0, 0) { Label = "n4" });

            model.Nodes["n0"].Constraints =
                model.Nodes["n4"].Constraints =
                    Constraints.Fixed;

            var secAA = new Sections.UniformGeometric1DSection(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));
            var secBB = new Sections.UniformGeometric1DSection(SectionGenerator.GetISetion(0.24, 0.52, 0.01, 0.006));
            var mat = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);

            model.Elements.Add(new BarElement(model.Nodes["n0"], model.Nodes["n1"]) { Label = "e0", Section = secAA, Material = mat });
            model.Elements.Add(new BarElement(model.Nodes["n1"], model.Nodes["n2"]) { Label = "e1", Section = secBB, Material = mat });
            model.Elements.Add(new BarElement(model.Nodes["n2"], model.Nodes["n3"]) { Label = "e2", Section = secBB, Material = mat });
            model.Elements.Add(new BarElement(model.Nodes["n3"], model.Nodes["n4"]) { Label = "e3", Section = secAA, Material = mat });

            var u1 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, -1 * Vector.K, 10000, CoordinationSystem.Global);
            var u2 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, -1 * Vector.K, 10000, CoordinationSystem.Local);

            model.Elements["e1"].Loads.Add(u1);
            model.Elements["e2"].Loads.Add(u2);

            model.Solve_MPC();

            var n3Force = model.Nodes["N3"].GetSupportReaction();
            Console.WriteLine("support reaction of n3: {0}", n3Force);

            {
                var x = 1.0;

                var iso = (model.Elements["e4"] as BarElement).LocalCoordsToIsoCoords(x);

                var e4Force = (model.Elements["e4"] as BarElement).GetInternalForceAt(iso[0]);
                Console.WriteLine("internal force at x={0} is {1}", x, e4Force);
            }
            
            
        }
    }
}
