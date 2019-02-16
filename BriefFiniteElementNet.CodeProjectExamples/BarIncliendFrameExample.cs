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

            //adding nodes
            model.Nodes.Add(new Node(-10, 0, 0) { Label = "n0" });
            model.Nodes.Add(new Node(-10, 0, 6) { Label = "n1" });
            model.Nodes.Add(new Node(0, 0, 8) { Label = "n2" });
            model.Nodes.Add(new Node(10, 0, 6) { Label = "n3" });
            model.Nodes.Add(new Node(10, 0, 0) { Label = "n4" });

            //adding elements
            model.Elements.Add(new BarElement(model.Nodes["n0"], model.Nodes["n1"]) { Label = "e0"});
            model.Elements.Add(new BarElement(model.Nodes["n1"], model.Nodes["n2"]) { Label = "e1"});
            model.Elements.Add(new BarElement(model.Nodes["n2"], model.Nodes["n3"]) { Label = "e2" });
            model.Elements.Add(new BarElement(model.Nodes["n3"], model.Nodes["n4"]) { Label = "e3" });

            //assign constraint to nodes
            model.Nodes["n0"].Constraints =
                model.Nodes["n4"].Constraints =
                    Constraints.Fixed;

            //define sections and material
            var secAA = new Sections.UniformGeometric1DSection(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));
            var secBB = new Sections.UniformGeometric1DSection(SectionGenerator.GetISetion(0.24, 0.52, 0.01, 0.006));
            var mat = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);

            //assign materials
            (model.Elements["e0"] as BarElement).Material = mat;
            (model.Elements["e1"] as BarElement).Material = mat;
            (model.Elements["e2"] as BarElement).Material = mat;
            (model.Elements["e3"] as BarElement).Material = mat;

            //assign sections
            (model.Elements["e0"] as BarElement).Section = secAA;
            (model.Elements["e1"] as BarElement).Section = secBB;
            (model.Elements["e2"] as BarElement).Section = secBB;
            (model.Elements["e3"] as BarElement).Section = secAA;

            //creating loads
            var u1 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, new Vector(0, 0, 1), -6000, CoordinationSystem.Global);
            var u2 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, new Vector(0, 0, 1), -5000, CoordinationSystem.Local);

            //assign loads
            model.Elements["e1"].Loads.Add(u1);
            model.Elements["e2"].Loads.Add(u2);

            //solve model
            model.Solve_MPC();

            //retrieve solve result
            var n0reaction = model.Nodes["N0"].GetSupportReaction();
            var n4reaction = model.Nodes["N4"].GetSupportReaction();

            Console.WriteLine("Support reaction of n0: {0}", n0reaction);
            Console.WriteLine("Support reaction of n4: {0}", n4reaction);

            var d1 = model.Nodes["N1"].GetNodalDisplacement();

            Console.WriteLine("Displacement of n1: {0}", d1);


            Controls.BarInternalForceVisualizer.VisualizeInNewWindow((model.Elements["e1"] as BarElement));

            //Controls.ModelInternalForceVisualizer.VisualizeInNewWindow(model);

        }
    }
}
