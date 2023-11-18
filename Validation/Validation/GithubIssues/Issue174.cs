using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation.GithubIssues
{
    public static class Issue174
    {
        public static void Run()
        {
            var load = new PartialNonUniformLoad(); //creating new instance of load

            var l = 6.5;

            var model = new Model();

            Node n1 = new Node(0, 0, 0) { Label = "n0" };
            Node n2 = new Node(l, 0, 0) { Label = "n1" };

            var bar = new BarElement(n1, n2) { Label = "e0" };

            model.Nodes.Add(n1);
            model.Nodes.Add(n2);
            model.Elements.Add(bar);

            var x1 = 2.0;
            var x2 = 5.0;
            var x3 = 6.5;

            var xi1 = bar.LocalCoordsToIsoCoords(x1)[0];
            var xi2 = bar.LocalCoordsToIsoCoords(x2)[0];

            var p1 = 1.8 * 9806.65;//1.8 ton/m = 1.8 * 9806.65 N/m
            var p2 = 2.3 * 9806.65;//2.3 ton/m = 1.8 * 9806.65 N/m

            // For the case of a triangular load on a beam
            load.SeverityFunction = BriefFiniteElementNet.Mathh.SingleVariablePolynomial.FromPoints(xi1, p1, xi2, p2);
            load.StartLocation = new IsoPoint(xi1);      //set locations of trapezoidal load
            load.EndLocation = new IsoPoint(xi2);

            load.Direction = -Vector.K;
            load.CoordinationSystem = CoordinationSystem.Global;

            n1.Constraints = Constraints.MovementFixed;
            n2.Constraints = Constraints.MovementFixed & Constraints.FixedRX;

            var sec = new BriefFiniteElementNet.Sections.UniformGeometric1DSection(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));
            var mat = BriefFiniteElementNet.Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);

            bar.Material = mat;
            bar.Section = sec;

            var u1 = load;

            bar.Loads.Add(u1);

            model.Solve_MPC();

            var r1 = n1.GetSupportReaction();
            var r2 = n2.GetSupportReaction();

            Console.WriteLine("support reaction of n1: {0}", r1);
            Console.WriteLine("support reaction of n2: {0}", r2);

            //var frc = bar.GetGlobalEquivalentNodalLoads(u1);
        }
    }
}
