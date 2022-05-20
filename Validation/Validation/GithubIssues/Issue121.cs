using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation.GithubIssues
{
    public class Issue121
    {
        public static void Run()
        {
            var model = new Model();
            var n1 = new Node(0, 0, 0) { Label = "n1", Constraints = Constraints.Fixed };
            var n2 = new Node(10, 0, 0) { Label = "n2", Constraints = Constraints.RotationFixed & Constraints.FixedDX & Constraints.FixedDY };

            var beam = new BarElement(n1, n2) { Label = "Beam", Behavior = BarElementBehaviour.BeamYEulerBernoulli };
            //beam.Section = new UniformGeometric1DSection(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));
            beam.Section = new UniformParametric1DSection() { A = 1e-4, Iy = 1e-6, Iz = 1e-6, J = 1e-6 };
            beam.Material = UniformIsotropicMaterial.CreateFromYoungPoisson(1e10, 0.2);

            model.Nodes.Add(n1, n2);
            model.Elements.Add(beam);

            var force = new Force(0, 0, -1000, 0, 0, 0);
            n2.Loads.Add(new NodalLoad(force));

            model.Solve_MPC();
        }
    }
}
