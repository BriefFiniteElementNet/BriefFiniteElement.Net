using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation.GithubIssues
{
    public class Issue50
    {
        public static void Run1()
        {
            double h = 5;

            Node n1 = new Node(0, 0, 0);
            Node n2 = new Node(h, 0, 0);
            n1.Constraints = Constraints.Fixed;

            var section = new UniformGeometric1DSection(SectionGenerator.GetRectangularSection(1, 0.5));
            var material = UniformIsotropicMaterial.CreateFromYoungShear(205e9, 81e9);

            BarElement e = new BarElement(n1, n2)
            {
                Section = section,
                Material = material
            };
            e.Behavior = BarElementBehaviours.FullFrame;

            var lc1 = new LoadCase("C1", LoadType.Dead);
            var lc2 = new LoadCase("C2", LoadType.Live);
            var lc3 = new LoadCase("C3", LoadType.Live);

            var load = new UniformLoad()
            {
                Direction = Vector.FromXYZ(0, 0, -1),
                CoordinationSystem = CoordinationSystem.Global,
                Magnitude = 1,
                Case = lc1
            };
            var load2 = new UniformLoad()
            {
                Direction = Vector.FromXYZ(1, 0, 0),
                CoordinationSystem = CoordinationSystem.Global,
                Magnitude = 10,
                Case = lc2
            };

            var load3 = new ConcentratedLoad()
            {
                Force = new Force(10, 10, 10, 10, 0, 0),
                ForceIsoLocation = new IsoPoint(0.50),
                CoordinationSystem = CoordinationSystem.Local,
                Case = lc3
            };


            e.Loads.Add(load);
            e.Loads.Add(load2);
            e.Loads.Add(load3);

            Model model = new Model();
            model.Nodes.Add(n1);
            model.Nodes.Add(n2);
            model.Elements.Add(e);
            model.Solve_MPC();

            //BarInternalForceVisualizer.VisualizeInNewWindow(e);
            
        }


       
    }
}
