using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Controls;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.MpcElements;


namespace BriefFiniteElementNet.CodeProjectExamples
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            StiffnessCenterTest();

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

            var e1 = new TrussElement2Node(n1, n5) {Label = "e1"};
            var e2 = new TrussElement2Node(n2, n5) {Label = "e2"};
            var e3 = new TrussElement2Node(n3, n5) {Label = "e3"};
            var e4 = new TrussElement2Node(n4, n5) {Label = "e4"};
            //Note: labels for all members should be unique, else you will receive InvalidLabelException when adding it to model

            e1.A = e2.A = e3.A = e4.A = 9e-4;
            e1.E = e2.E = e3.E = e4.E = 210e9;

            model.Nodes.Add(n1, n2, n3, n4, n5);
            model.Elements.Add(e1, e2, e3, e4);

            //Applying restrains


            n1.Constraints = n2.Constraints = n3.Constraints = n4.Constraints = Constraint.Fixed;
            n5.Constraints = Constraint.RotationFixed;


            //Applying load
            var force = new Force(0, 1000, -1000, 0, 0, 0);
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

            var secAA = SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006);
            var secBB = SectionGenerator.GetISetion(0.24, 0.52, 0.01, 0.006);

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
                                Constraint.FixedDY & Constraint.FixedRX & Constraint.FixedRZ;//DY fixed and RX fixed and RZ fixed


            n1.Constraints = n1.Constraints & Constraint.MovementFixed;
            n5.Constraints = n5.Constraints & Constraint.MovementFixed;


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
            new LoadCombExample().Run();
        }

        public static void StiffnessCenterTest()
        {
            #region model
            var model = new Model();

            model.Nodes.Add(new Node(0, 0, 0) { Label = "n0" });
            model.Nodes.Add(new Node(0, 2, 0) { Label = "n1" });
            model.Nodes.Add(new Node(4, 2, 0) { Label = "n2" });
            model.Nodes.Add(new Node(4, 0, 0) { Label = "n3" });

            model.Nodes.Add(new Node(0, 0, 1) { Label = "n4" });
            model.Nodes.Add(new Node(0, 2, 1) { Label = "n5" });
            model.Nodes.Add(new Node(4, 2, 1) { Label = "n6" });
            model.Nodes.Add(new Node(4, 0, 1) { Label = "n7" });


            var a = 0.1 * 0.1;//area, assume sections are 10cm*10cm rectangular
            var iy = 0.1 * 0.1 * 0.1 * 0.1 / 12.0;//Iy
            var iz = 0.1 * 0.1 * 0.1 * 0.1 / 12.0;//Iz
            var j = 0.1 * 0.1 * 0.1 * 0.1 / 12.0;//Polar
            var e = 20e9;//young modulus, 20 [GPa]
            var nu = 0.2;//poissons ratio

            var sec = new Sections.UniformParametric1DSection(a, iy, iz, j);
            var mat = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(e, nu);

            model.Elements.Add(new BarElement(model.Nodes["n0"], model.Nodes["n4"]) { Label = "e0", Section = sec, Material = mat });
            model.Elements.Add(new BarElement(model.Nodes["n1"], model.Nodes["n5"]) { Label = "e1", Section = sec, Material = mat });
            model.Elements.Add(new BarElement(model.Nodes["n2"], model.Nodes["n6"]) { Label = "e2", Section = sec, Material = mat });
            model.Elements.Add(new BarElement(model.Nodes["n3"], model.Nodes["n7"]) { Label = "e3", Section = sec, Material = mat });

            model.Elements.Add(new BarElement(model.Nodes["n4"], model.Nodes["n5"]) { Label = "e4", Section = sec, Material = mat });
            model.Elements.Add(new BarElement(model.Nodes["n5"], model.Nodes["n6"]) { Label = "e5", Section = sec, Material = mat });
            model.Elements.Add(new BarElement(model.Nodes["n6"], model.Nodes["n7"]) { Label = "e6", Section = sec, Material = mat });
            model.Elements.Add(new BarElement(model.Nodes["n7"], model.Nodes["n4"]) { Label = "e7", Section = sec, Material = mat });



            model.Nodes["n0"].Constraints =
                model.Nodes["n1"].Constraints =
                    model.Nodes["n2"].Constraints =
                        model.Nodes["n3"].Constraints =
                            Constraints.Fixed;
            #endregion

            var rgd = new RigidElement_MPC();

            rgd.Nodes.Add(model.Nodes["n4"]);
            rgd.Nodes.Add(model.Nodes["n5"]);
            rgd.Nodes.Add(model.Nodes["n6"]);
            //rgd.Nodes.Add(model.Nodes["n7"]);




            //var eqDof = new TelepathyLink

            model.MpcElements.Add(rgd);

            var loc = new StiffnessCenterFinder().GetCenters(model, rgd, LoadCase.DefaultLoadCase);

        }
    }
}
