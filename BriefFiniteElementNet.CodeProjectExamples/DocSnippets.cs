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
    }
}
