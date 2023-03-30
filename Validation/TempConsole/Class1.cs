using BriefFiniteElementNet;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Validation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace TempConsole
{
    public class Class1
    {

        public static void T()
        {
            var model = new Model();

            var l = 4;

            var node1 = new Node(1, 2, 3);
            var node2 = new Node(1 + l, 2, 3);

            var elm2 = new BarElement(node1, node2);

            var sec = new BriefFiniteElementNet.Sections.UniformParametric1DSection(3, 5, 7, 11);
            var mat = BriefFiniteElementNet.Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(17, 0.25);

            elm2.Section = sec;
            elm2.Material = mat;


            model.Elements.Add(elm2);

            model.Nodes.Add(node1, node2);

            node1.Constraints = Constraints.MovementFixed;
            node2.Constraints = Constraints.MovementFixed;

            node1.Loads.Add(new NodalLoad(new Force(0, 0, 0, 0, 10, 0)));
            node2.Loads.Add(new NodalLoad(new Force(0, 0, 0, 0, -10, 0)));

            model.Solve();

            var d1 = node1.GetNodalDisplacement();
            var dd1 = elm2.GetInternalDisplacementAt(-1);

            var d2 = node2.GetNodalDisplacement();
            var dd2 = elm2.GetInternalDisplacementAt(+1);

            var res = OpenseesValidator.OpenseesValidate(model, LoadCase.DefaultLoadCase, false);

        }
    }
}
