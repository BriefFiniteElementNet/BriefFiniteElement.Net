using BriefFiniteElementNet.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.CodeProjectExamples
{
    public class DocSnippets
    {
        public static void Test1()
        {
            var model = new Model();

            Node n1, n2;

            model.Nodes.Add(n1 = new Node(0, 0, 0) { Constraints = Constraints.Fixed });
            model.Nodes.Add(n2 = new Node(1, 0, 0) { Constraints = Constraints.Fixed });

            var elm = new BarElement(n1, n2);

            elm.Section = new BriefFiniteElementNet.Sections.UniformParametric1DSection(a: 0.01, iy: 0.01, iz: 0.01, j: 0.01);
            elm.Material = BriefFiniteElementNet.Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);

            var load = new Loads.UniformLoad();

            load.Case = LoadCase.DefaultLoadCase;
            load.CoordinationSystem = CoordinationSystem.Global;
            load.Direction = Vector.K;
            load.Magnitude = 10;

            elm.Loads.Add(load);
            model.Elements.Add(elm);

            model.Solve_MPC();

            var f1 = elm.GetInternalForceAt(0);
            var f2 = elm.GetExactInternalForceAt(0);
        }

        public static void Test2()
        {
            Console.WriteLine("Simple 2D truss with 3 members");

            var model = new Model();

            var n1 = new Node(0, 0, 0);
            n1.Label = "n1";//Set a unique label for node
            var n2 = new Node(1, 0, 1.732) { Label = "n2" };//using object initializer for assigning Label
            var n3 = new Node(1, 0, 0) { Label = "n3" };

            var e1 = new TrussElement2Node(n1, n2) { Label = "e1" };
            var e2 = new TrussElement2Node(n2, n3) { Label = "e2" };
            var e3 = new TrussElement2Node(n1, n3) { Label = "e3" };

            e1.A = e2.A = e3.A = 0.001;
            e1.E = e2.E = e3.E = 200e9;

            model.Nodes.Add(n1, n2, n3);
            model.Elements.Add(e1, e2, e3);

            n1.Constraints = Constraints.Fixed;
            n2.Constraints = Constraints.RotationFixed & Constraints.FixedDY; 
            n3.Constraints = Constraints.Fixed;

            var force = new Force(10000, 0, 0, 0, 0, 0);
            n2.Loads.Add(new NodalLoad(force));//adds a load with LoadCase of DefaultLoadCase to node loads

            model.Solve();

            var d = n2.GetNodalDisplacement();

            Console.WriteLine("displacement on node 2: " + d.DX.ToString());

            Console.ReadKey();
        }
    }
}
