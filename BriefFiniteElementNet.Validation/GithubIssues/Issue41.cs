using BriefFiniteElementNet.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation.GithubIssues
{
    public class Issue41
    {
        public static void Run()
        {
            // Two span beam of 20m overall length with elements of 1m length
            // Vertical 'springs' are used at supports

            List<Node> nodeList = new List<Node>();
            List<BarElement> elementList = new List<BarElement>();
            LoadCase loadCase1 = new LoadCase("test", LoadType.Other);// LoadCase.DefaultLoadCase;  // new LoadCase("L1", LoadType.Dead);

            var model = new BriefFiniteElementNet.Model();

            for (double n = 0; n <= 20; n = n + 1)
            {
                nodeList.Add(new Node(x: n, y: 0.0, z: 0.0) { Label = "N" + n });
            }

            nodeList.Add(new Node(x: 0, y: 0.0, z: -2.0) { Label = "N21" });
            nodeList.Add(new Node(x: 10, y: 0.0, z: -2.0) { Label = "N22" });
            nodeList.Add(new Node(x: 20, y: 0.0, z: -2.0) { Label = "N23" });

            //var load1 = new BriefFiniteElementNet.Loads.UniformLoad(loadCase1, new Vector(0, 0, 1), 0, CoordinationSystem.Global);  // Load in N/m (UDL = 0 N/m)

            var a = 0.1;                            // m²
            var iy = 0.008333
                ;                      // m4
            var iz = 8.333e-5;                      // m4
            var j = 0.1 * 0.1 * 0.1 * 1 / 12.0;     // m4
            var e = 205e9;                          // N/m²
            var nu = 0.3;                           // Poisson's ratio
            var secAA = new BriefFiniteElementNet.Sections.UniformParametric1DSection(a, iy, iz, j);

            var mat = BriefFiniteElementNet.Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(e, nu);

            for (int m = 0; m <= 19; m++)
            {
                BarElement el = new BarElement(nodeList[m], nodeList[m + 1]);
                el.Section = secAA;
                el.Material = mat;
                //el.Loads.Add(load1);
                elementList.Add(el);
            }


            BarElement el2 = new BarElement(nodeList[0], nodeList[21]); el2.Section = secAA; el2.Material = mat; elementList.Add(el2);
            BarElement el3 = new BarElement(nodeList[10], nodeList[22]); el3.Section = secAA; el3.Material = mat; elementList.Add(el3);
            BarElement el4 = new BarElement(nodeList[20], nodeList[23]); el4.Section = secAA; el4.Material = mat; elementList.Add(el4);

            nodeList[21].Constraints = Constraints.MovementFixed & Constraints.FixedRX; // Constraints.FixedDX & Constraints.FixedDY & Constraints.FixedDZ & Constraints.FixedRY & Constraints.FixedRZ;
            nodeList[22].Constraints = Constraints.FixedDZ & Constraints.FixedDY & Constraints.FixedRX;  // z = vertical
            nodeList[23].Constraints = Constraints.FixedDZ & Constraints.FixedDY & Constraints.FixedRX;  // z = vertical

            nodeList[22].Settlements.Add(new Settlement(loadCase1, new Displacement(0, 0, -0.010, 0, 0, 0)));     // -10mm settlement

            model.Elements.Add(elementList.ToArray());
            model.Nodes.AddRange(nodeList);

            model.Solve_MPC(loadCase1);//or model.Solve();

            
            var disp = nodeList[10].GetNodalDisplacement(loadCase1);
            Console.WriteLine("Node 10 displacement in Z direction is {0:0.000} m", disp.DZ);
            Console.WriteLine("Node 10 rotation in YY direction is {0:0.000} rads\n", disp.RY);

            foreach (BarElement elem in elementList)
            {
                Force f1 = elem.GetExactInternalForceAt(-0.999999, loadCase1);  // -1 = start, 0 = mid, 1 = end, exact solver takes UDL on member into account, doesn't then accept -1 or 1
                Console.WriteLine("Element BMyy is {0:0.000} kNm at start,    SFz is {1:0.000} kN at start", f1.My / 1000, f1.Fz / 1000);
                Force f2 = elem.GetExactInternalForceAt(0.999999, loadCase1);
                Console.WriteLine("Element BMyy is {0:0.000} kNm at end,    SFz is {1:0.000} kN at start", f2.My / 1000, f2.Fz / 1000);
            }

            Console.WriteLine("Node 21 vertical reaction {0:0.000} kN", model.Nodes[21].GetSupportReaction(loadCase1).Fz / 1000);  // gives      51.171 kN  CORRECT
            Console.WriteLine("Node 22 vertical reaction {0:0.000} kN", model.Nodes[22].GetSupportReaction(loadCase1).Fz / 1000);  // gives  102397.658 kN  INCORRECT   (EXPECT -102.342 kN)
            Console.WriteLine("Node 23 vertical reaction {0:0.000} kN", model.Nodes[23].GetSupportReaction(loadCase1).Fz / 1000);  // gives      51.171 kN  CORRECT


             Console.ReadKey();
        }

        public static void Run2()
        {
            // Two span beam of 20m overall length with elements of 1m length
            // Vertical 'springs' are used at supports

            var model = new Model();

            var n0 = new Node(0, 0, 0);
            var n1 = new Node(5, 0, 0);
            var n2 = new Node(10, 0, 0);

            var e1 = new BarElement(n0, n1);
            var e2 = new BarElement(n1, n2);


            model.Nodes.Add(n0, n1, n2);
            model.Elements.Add(e1, e2);

            n0.Constraints = Constraints.MovementFixed;
            n1.Constraints = Constraints.Fixed;
            n2.Constraints = Constraints.MovementFixed;

            n1.Settlements.Add(new Settlement(new Displacement(0, 0, -0.01, 0, 0, 0)));

            var a = 0.1;                            // m²
            var iy = 0.008333;                      // m4
            var iz = 8.333e-5;                      // m4
            var j = 0.1 * 0.1 * 0.1 * 1 / 12.0;     // m4
            var e = 205e9;                          // N/m²
            var nu = 0.3;                           // Poisson's ratio
            var secAA = new BriefFiniteElementNet.Sections.UniformParametric1DSection(a, iy, iz, j);

            var mat = BriefFiniteElementNet.Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(e, nu);

            e1.Section = e2.Section = secAA;
            e1.Material = e2.Material = mat;


            model.Solve_MPC();//or model.Solve();

            var disp = n0.GetNodalDisplacement();
            Console.WriteLine("Node 1 displacement in Z direction is {0:0.000} m", disp.DZ);
            Console.WriteLine("Node 1 rotation in YY direction is {0:0.000} rads\n", disp.RY);

            Console.WriteLine("Node 0 vertical reaction {0:0.000} kN", n0.GetSupportReaction().Fz / 1000);  // gives      51.171 kN  CORRECT
            Console.WriteLine("Node 1 vertical reaction {0:0.000} kN", n1.GetSupportReaction().Fz / 1000);  // gives  102397.658 kN  INCORRECT   (EXPECT -102.342 kN)
            Console.WriteLine("Node 2 vertical reaction {0:0.000} kN", n2.GetSupportReaction().Fz / 1000);  // gives      51.171 kN  CORRECT


            Console.ReadKey();
        }
    }
}
