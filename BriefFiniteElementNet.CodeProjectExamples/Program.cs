using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Controls;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.MpcElements;


namespace BriefFiniteElementNet.CodeProjectExamples
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            PartialTrapezoidalLoadExamples.test1();
            //InternalForceExample.Run();
            //new BarIncliendFrameExample().Run();
            //new UniformLoadCoordSystem().run();
            //DocSnippets.Test2();
            //new UniformLoadCoordSystem().run();
            //Example1();
            //Example2();
            //DocSnippets.Test1();
            //TestMatrixMult();
        }

        private static void Example1()
        {
            Console.WriteLine("Example 1: Simple 3D truss with four members");


            // Initiating Model, Nodes and Members
            var model = new Model();

            var n1 = new Node(1, 1, 0);
            n1.Label = "n1";//Set a unique label for node
            var n2 = new Node(-1, 1, 0) {Label = "n2"};//using object initializer for assigning Label
            var n3 = new Node(1, -1, 0) {Label = "n3"};
            var n4 = new Node(-1, -1, 0) {Label = "n4"};
            var n5 = new Node(0, 0, 1) {Label = "n5"};

            var e1 = new BarElement(n1, n5) { Label = "e1", Behavior = BarElementBehaviours.Truss };
            var e2 = new BarElement(n2, n5) {Label = "e2", Behavior = BarElementBehaviours.Truss };
            var e3 = new BarElement(n3, n5) {Label = "e3", Behavior = BarElementBehaviours.Truss };
            var e4 = new BarElement(n4, n5) { Label = "e4", Behavior = BarElementBehaviours.Truss };
            //Note: labels for all members should be unique, else you will receive InvalidLabelException when adding it to model

            e1.Section = new Sections.UniformParametric1DSection() { A = 9e-4 };
            e2.Section = new Sections.UniformParametric1DSection() { A = 9e-4 };
            e3.Section = new Sections.UniformParametric1DSection() { A = 9e-4 };
            e4.Section = new Sections.UniformParametric1DSection() { A = 9e-4 };

            e1.Material = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);
            e2.Material = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);
            e3.Material = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);
            e4.Material = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);


            model.Nodes.Add(n1, n2, n3, n4, n5);
            model.Elements.Add(e1, e2, e3, e4);

            //Applying restrains


            n1.Constraints = n2.Constraints = n3.Constraints = n4.Constraints = Constraints.Fixed;
            n5.Constraints = Constraints.RotationFixed;


            //Applying load
            var force = new Force(0, 0, -1000, 0, 0, 0);
            n5.Loads.Add(new NodalLoad(force));//adds a load with LoadCase of DefaultLoadCase to node loads
            
            //Adds a NodalLoad with Default LoadCase

            model.Solve();

            var r1 = n1.GetSupportReaction();
            var r2 = n2.GetSupportReaction();
            var r3 = n3.GetSupportReaction();
            var r4 = n4.GetSupportReaction();

            var rt = r1 + r2 + r3 + r4;//shows the Fz=1000 and Fx=Fy=Mx=My=Mz=0.0

            Console.WriteLine("Total reactions SUM :" + rt.ToString());
        }

        private static void Example2()
        {
            Console.WriteLine("Example 1: Simple 3D Frame with distributed loads");

            var model = new Model();
            
            var n1 = new Node(-10, 0, 0);
            var n2 = new Node(-10, 0, 6);
            var n3 = new Node(0, 0, 8);
            var n4 = new Node(10, 0, 6);
            var n5 = new Node(10, 0, 0);

            model.Nodes.Add(n1, n2, n3, n4, n5);

            var secAA = new PolygonYz(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));
            var secBB = new PolygonYz(SectionGenerator.GetISetion(0.24, 0.52, 0.01, 0.006));

            var e1 = new FrameElement2Node(n1, n2);
            e1.Label = "e1";
            var e2 = new FrameElement2Node(n2, n3);
            e2.Label = "e2";
            var e3 = new FrameElement2Node(n3, n4);
            e3.Label = "e3";
            var e4 = new FrameElement2Node(n4, n5);
            e4.Label = "e4";


            e1.Geometry = e4.Geometry = secAA;
            e2.Geometry = e3.Geometry = secBB;

            e1.E = e2.E = e3.E = e4.E = 210e9;
            e1.G = e2.G = e3.G = e4.G = 210e9/(2*(1 + 0.3));//G = E / (2*(1+no))

            e1.UseOverridedProperties = 
                e2.UseOverridedProperties = e3.UseOverridedProperties = e4.UseOverridedProperties = false;

            model.Elements.Add(e1, e2, e3, e4);


            n1.Constraints =
                n2.Constraints =
                    n3.Constraints =
                        n4.Constraints =
                            n5.Constraints =
                                Constraints.FixedDY & Constraints.FixedRX & Constraints.FixedRZ;//DY fixed and RX fixed and RZ fixed


            n1.Constraints = n1.Constraints & Constraints.MovementFixed;
            n5.Constraints = n5.Constraints & Constraints.MovementFixed;


            var ll = new UniformLoad1D(-10000, LoadDirection.Z, CoordinationSystem.Global);
            var lr = new UniformLoad1D(-10000, LoadDirection.Z, CoordinationSystem.Local);

            e2.Loads.Add(ll);
            e3.Loads.Add(lr);

            var wnd = WpfTraceListener.CreateModelTrace(model);
            new ModelWarningChecker().CheckModel(model);
            wnd.ShowDialog();

            model.Solve();
        }

        private static void LoadComb()
        {
            new BarIncliendFrameExample().Run();
        }

        private static void TestMatrixMult()
        {
            var n = 1500;
            var m1 = new Matrix(n, n);
            var m2 = new Matrix(n, n);

            var sp = System.Diagnostics.Stopwatch.StartNew();

            var m3 = m1.Multiply(m2);

            Console.WriteLine("{1}x{1}*{1}x{1} took {0} milisecs", sp.ElapsedMilliseconds,n);
        }

       

        private static void TestConcLoad()
        {
            var model = new Model();
            
        }
    }
}
