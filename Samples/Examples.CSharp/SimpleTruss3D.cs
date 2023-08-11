using BriefFiniteElementNet;
using BriefFiniteElementNet.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Examples.CSharp
{
    public class SimpleTruss3D
    {
        public static void Run()
        {
            // Initiating Model, Nodes and Members
            var model = new Model();

            var n1 = new Node(1, 1, 0);
            n1.Label = "n1";//Set a unique label for node
            var n2 = new Node(-1, 1, 0) { Label = "n2" };//using object initializer for assigning Label
            var n3 = new Node(1, -1, 0) { Label = "n3" };
            var n4 = new Node(-1, -1, 0) { Label = "n4" };
            var n5 = new Node(0, 0, 1) { Label = "n5" };



            var e1 = new BarElement(n1, n5) { Label = "e1", Behavior = BarElementBehaviours.Truss };
            var e2 = new BarElement(n2, n5) { Label = "e2", Behavior = BarElementBehaviours.Truss };
            var e3 = new BarElement(n3, n5) { Label = "e3", Behavior = BarElementBehaviours.Truss };
            var e4 = new BarElement(n4, n5) { Label = "e4", Behavior = BarElementBehaviours.Truss };


            model.Nodes.Add(n1, n2, n3, n4, n5);
            model.Elements.Add(e1, e2, e3, e4);

            var section = new BriefFiniteElementNet.Sections.UniformParametric1DSection() { A = 9e-4 };

            e1.Section = e2.Section = e3.Section = e4.Section = section;

            var material = BriefFiniteElementNet.Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);

            e1.Material = e2.Material = e3.Material = e4.Material = material;

            n1.Constraints = n2.Constraints = n3.Constraints = n4.Constraints = Constraints.Fixed;
            n5.Constraints = Constraints.RotationFixed;

            var force = new Force(0, 0, -1000, 0, 0, 0);
            n5.Loads.Add(new NodalLoad(force));//adds a load with LoadCase of DefaultLoadCase to node loads


            model.Solve_MPC();

            var r1 = n1.GetSupportReaction();
            var r2 = n2.GetSupportReaction();
            var r3 = n3.GetSupportReaction();
            var r4 = n4.GetSupportReaction();


            var rt = r1 + r2 + r3 + r4;//shows the Fz=1000 and Fx=Fy=Mx=My=Mz=0.0

            Console.WriteLine("Total reactions SUM :" + rt.ToString());
        }

    }
}
