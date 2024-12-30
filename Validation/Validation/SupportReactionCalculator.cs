using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.ElementHelpers.Bar;
using BriefFiniteElementNet.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using static System.Net.Mime.MediaTypeNames;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using BriefFiniteElementNet.Validation.OpenseesTclGenerator;
using System.Reflection;


namespace BriefFiniteElementNet.Validation
{
    public class SupportReactionCalculator
    {
        public static void Run()
        {
            var n1 = new Node(0, 0, 0);
            var n2 = new Node(2, 3, 5);

            n1.Constraints = Constraints.Fixed;

            var elm = new BarElement(n1, n2);

            elm.Section = new UniformGeometric1DSection(SectionGenerator.GetISetion(1, 1, 0.01, 0.01));
            elm.Material = UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.25);

            var model = new Model();
            model.Nodes.Add(n1, n2);
            model.Elements.Add(elm);

            var frc = new Force(7, 11, 13, 17, 19, 23);

            n2.Loads.Add(new NodalLoad(frc));



            model.Solve_MPC();

            var s0 = n1.GetSupportReaction();

            var zero = s0 + frc.Move(n2.Location, n1.Location);

        }
    }
}
