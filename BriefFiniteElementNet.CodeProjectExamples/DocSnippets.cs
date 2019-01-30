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

        public static void Test3()
        {
            var model = new BriefFiniteElementNet.Model();

            var p = new Point[20];
            var ns = new Node[20];


            p[0] = new Point(x: 0, y: 1, z: 0);
            p[1] = new Point(x: 0, y: 0, z: 0);
            p[2] = new Point(x: 0.20, y: 0, z: 0);
            p[3] = new Point(x: 0.20, y: 0, z: 0.20);
            p[4] = new Point(x: 0.20, y: 1, z: 0);
            p[5] = new Point(x: 0.20, y: 1, z: 0.20);
            p[6] = new Point(x: 0, y: 1, z: 0.20);
            p[7] = new Point(x: 0, y: 0, z: 0.20);
            p[8] = new Point(x: 0, y: 0.50, z: 0);
            p[9] = new Point(x: 0.20, y: 0.50, z: 0);
            p[10] = new Point(x: 0, y: 0.50, z: 0.20);
            p[11] = new Point(x: 0.20, y: 0.50, z: 0.20);
            p[12] = new Point(x: 0.20, y: 0.25, z: 0.20);
            p[13] = new Point(x: 0, y: 0.25, z: 0.20);
            p[14] = new Point(x: 0, y: 0.25, z: 0);
            p[15] = new Point(x: 0.20, y: 0.25, z: 0);
            p[16] = new Point(x: 0.20, y: 0.75, z: 0.20);
            p[17] = new Point(x: 0.20, y: 0.75, z: 0);
            p[18] = new Point(x: 0, y: 0.75, z: 0);
            p[19] = new Point(x: 0, y: 0.75, z: 0.20);

            for (var i = 0; i < 20; i++)
            {
                model.Nodes.Add(ns[i] = new Node(p[i]));
                ns[i].Label = "n" + i.ToString();
                ns[i].Constraints = Constraints.RotationFixed;
            }




            var mesh = new int[24][];

            mesh[0] = new int[] { 0, 4, 16, 17 };
            mesh[1] = new int[] { 8, 15, 12, 14 };
            mesh[2] = new int[] { 8, 16, 17, 18 };
            mesh[3] = new int[] { 10, 8, 11, 12 };
            mesh[4] = new int[] { 5, 19, 0, 16 };
            mesh[5] = new int[] { 1, 15, 14, 12 };
            mesh[6] = new int[] { 8, 10, 11, 16 };
            mesh[7] = new int[] { 3, 13, 1, 7 };
            mesh[8] = new int[] { 3, 13, 12, 1 };
            mesh[9] = new int[] { 8, 12, 13, 14 };
            mesh[10] = new int[] { 1, 15, 12, 2 };
            mesh[11] = new int[] { 9, 8, 11, 16 };
            mesh[12] = new int[] { 10, 8, 12, 13 };
            mesh[13] = new int[] { 5, 0, 4, 16 };
            mesh[14] = new int[] { 5, 19, 6, 0 };
            mesh[15] = new int[] { 8, 19, 16, 18 };
            mesh[16] = new int[] { 8, 19, 10, 16 };
            mesh[17] = new int[] { 0, 19, 18, 16 };
            mesh[18] = new int[] { 1, 3, 2, 12 };
            mesh[19] = new int[] { 8, 15, 9, 12 };
            mesh[20] = new int[] { 13, 12, 1, 14 };
            mesh[21] = new int[] { 8, 9, 11, 12 };
            mesh[22] = new int[] { 9, 8, 16, 17 };
            mesh[23] = new int[] { 16, 0, 17, 18 };

            foreach (var elm in mesh)
            {
                var felm = new Tetrahedral();

                felm.Nodes[0] = ns[elm[0]];
                felm.Nodes[1] = ns[elm[1]];
                felm.Nodes[2] = ns[elm[2]];
                felm.Nodes[3] = ns[elm[3]];

                felm.E = 210e9;
                felm.Nu = 0.33;
                model.Elements.Add(felm);
            }


            var relm = new BriefFiniteElementNet.MpcElements.RigidElement_MPC();
            relm.Nodes = new NodeList() { ns[0], ns[4], ns[5], ns[6] };
            relm.UseForAllLoads = true;
            //model.MpcElements.Add(relm);

            ns[1].Constraints = ns[2].Constraints = ns[3].Constraints = ns[7].Constraints = Constraints.Fixed;



            var load = new BriefFiniteElementNet.NodalLoad();
            var frc = new Force();
            frc.Fz = 1000;// 1kN force in Z direction
            load.Force = frc;

            ns[5].Loads.Add(load);
            ns[6].Loads.Add(load);

            model.Solve_MPC();

            var d5 = ns[5].GetNodalDisplacement();
            var d6 = ns[6].GetNodalDisplacement();

            Console.WriteLine("Nodal displacement in Z direction is {0} meters (thus {1} mm)", d5.DZ, d5.DZ * 1000);
            Console.WriteLine("Nodal displacement in Z direction is {0} meters (thus {1} mm)", d6.DZ, d6.DZ * 1000);

            var tetra = model.Elements[0] as Tetrahedral;
            var stress = tetra.GetInternalForce(LoadCombination.DefaultLoadCombination);

        }
    }
}
