using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Validation
{
    public class TriangleElementTester
    {
        public static void TestSingleElement()
        {
            var model = new Model();

            var elm = new TriangleElement();

            var arr = new[]
            {
                new Node(1, 1, 0),
                new Node(3.25, 0.5, 0),
                new Node(3.733, 1.92, 0),
            };

            arr.CopyTo(elm.Nodes, 0);

            model.Nodes.AddRange(arr);
            model.Elements.Add(elm);

            model.Nodes[2].Loads.Add(new NodalLoad(new Force(1, 1, 1, 1, 1, 1) * -5000));

            model.Nodes[0].Constraints =
                model.Nodes[1].Constraints =
                Constraint.Fixed;

            elm.Behavior = FlatShellBehaviours.FullThinShell;
            elm.MembraneFormulation = MembraneFormulation.PlaneStress;
            elm.Material = new UniformIsotropicMaterial(2100, 0.3);
            elm.Section = new UniformParametric2DSection(0.2);

            model.Solve();

            Console.WriteLine(model.Nodes[2].GetNodalDisplacement().ToString(5));

            //var force = elm.GetLocalInternalForceAt(LoadCase.DefaultLoadCase, -0.57735, -0.57735);

        }
    }
}
