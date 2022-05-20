using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.Validation.GithubIssues
{
    public class Issue96
    {
        public static void Run1()
        {
            var m1 = new Model();

            m1.Nodes.Add(new Node(0, 0, 0) { Label = "n1" });

            m1.Nodes.Add(new Node(0, 0, 6) { Label = "n2" });

            m1.Nodes["n1"].Constraints = new Constraint(dx: DofConstraint.Fixed, dy: DofConstraint.Fixed, dz: DofConstraint.Fixed, rx: DofConstraint.Fixed, ry: DofConstraint.Fixed, rz: DofConstraint.Fixed);

            m1.Nodes["n2"].Loads.Add(new NodalLoad(new Force(0, 0, -4000, 0, 0, 0)));

            m1.Elements.Add(new BarElement(m1.Nodes["n1"], m1.Nodes["n2"]) { Label = "r1" });

            (m1.Elements["r1"] as BarElement).Behavior = BarElementBehaviours.FullFrame;

            (m1.Elements["r1"] as BarElement).Section = new BriefFiniteElementNet.Sections.UniformParametric1DSection(16.43 / 10000, 541.22 / 100000000, 44.92 / 100000000, 541.22 / 100000000 + 44.92 / 100000000);

            (m1.Elements["r1"] as BarElement).Material = BriefFiniteElementNet.Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210 * Math.Pow(10, 9), 0.3);

            m1.Solve_MPC();

            var r11 = m1.Nodes[0].GetSupportReaction();

            var r12 = m1.Nodes[1].GetSupportReaction();

            Console.WriteLine("n0 reaction: {0}", r11);

            Console.WriteLine("n1 reaction: {0}", r12);

            Console.ReadKey();
        }
    }
}
