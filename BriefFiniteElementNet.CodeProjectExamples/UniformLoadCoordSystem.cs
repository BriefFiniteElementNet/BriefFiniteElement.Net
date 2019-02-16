using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;

namespace BriefFiniteElementNet.CodeProjectExamples
{
    public class UniformLoadCoordSystem
    {
        public void run()
        {
            Run1();
            Run2();
        }

        public void Run1()
        {
            var m1 = new Model();

            var el1 = new BarElement();

            el1.Nodes[0] = new Node(0, 0, 0) { Constraints = Constraints.MovementFixed & Constraints.FixedRX, Label = "n0" };
            el1.Nodes[1] = new Node(3, 0, 4) { Constraints = Constraints.MovementFixed, Label = "n1" };

            el1.Section = new Sections.UniformGeometric1DSection(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));
            el1.Material = UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);


            var l1 = new Loads.UniformLoad();

            l1.Direction = Vector.K;
            l1.CoordinationSystem = CoordinationSystem.Global;
            l1.Magnitude = 1e3;


            el1.Loads.Add(l1);

            m1.Elements.Add(el1);
            m1.Nodes.Add(el1.Nodes);

            m1.Solve_MPC();

            Console.WriteLine("n0 reaction: {0}", m1.Nodes[0].GetSupportReaction());
            Console.WriteLine("n1 reaction: {0}", m1.Nodes[0].GetSupportReaction());

        }

        public void Run2()
        {
            var m1 = new Model();

            var el1 = new BarElement();

            el1.Nodes[0] = new Node(0, 0, 0) { Constraints = Constraints.MovementFixed & Constraints.FixedRX, Label = "n0" };
            el1.Nodes[1] = new Node(3, 0, 4) { Constraints = Constraints.MovementFixed, Label = "n1" };

            el1.Section = new Sections.UniformGeometric1DSection(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));
            el1.Material = UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);


            var l1 = new Loads.UniformLoad();

            l1.Direction = Vector.K;
            l1.CoordinationSystem = CoordinationSystem.Local;
            l1.Magnitude = 1e3;


            el1.Loads.Add(l1);

            m1.Elements.Add(el1);
            m1.Nodes.Add(el1.Nodes);

            m1.Solve_MPC();

            Console.WriteLine("n0 reaction: {0}", m1.Nodes[0].GetSupportReaction());
            Console.WriteLine("n1 reaction: {0}", m1.Nodes[0].GetSupportReaction());

        }
    }
}
