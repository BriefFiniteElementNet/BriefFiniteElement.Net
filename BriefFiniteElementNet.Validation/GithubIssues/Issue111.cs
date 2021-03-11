using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.Validation.GithubIssues
{
    public class Issue111
    {
        public static void Run1()
        {
            var model = new Model();

            //adding nodes
            model.Nodes.Add(new Node(0, 0, 0) { Label = "n0" });
            model.Nodes.Add(new Node(5, 5, 5) { Label = "n1" });

            //adding elements
            model.Elements.Add(new BarElement(model.Nodes["n0"], model.Nodes["n1"]) { Label = "e0" });

            //assign constraint to nodes
            model.Nodes["n0"].Constraints = Constraints.MovementFixed & Constraints.FixedRX;
            model.Nodes["n1"].Constraints = Constraints.MovementFixed;

            //define sections and material
            var secAA = new Sections.UniformGeometric1DSection(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));
            var mat = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);

            //assign materials
            (model.Elements["e0"] as BarElement).Material = mat;

            //assign sections
            (model.Elements["e0"] as BarElement).Section = secAA;

            //creating loads
            var u1 = new Loads.PartialNonUniformLoad() { Direction = Vector.K, CoordinationSystem = CoordinationSystem.Global };
            u1.SeverityFunction = Mathh.SingleVariablePolynomial.FromPoints(-1, -6, 1, -4);
            u1.StartLocation = new IsoPoint(-0.5);      //set locations of trapezoidal load
            u1.EndLocation = new IsoPoint(0.5);         //set locations of trapezoidal load

            //assign loads
            model.Elements["e0"].Loads.Add(u1);

            //solve model
            model.Solve_MPC();

            //retrieve solve result
            var n0reaction = model.Nodes["n0"].GetSupportReaction();
            var n1reaction = model.Nodes["n1"].GetSupportReaction();

            Console.WriteLine("Support reaction of n0: {0}", n0reaction);
            Console.WriteLine("Support reaction of n1: {0}", n1reaction);

            var x = 1.0; //need to find internal force at x = 1.0 m
            var iso = (model.Elements["e0"] as BarElement).LocalCoordsToIsoCoords(x);//find the location of 1m in iso coordination system
            var e4Force = (model.Elements["e0"] as BarElement).GetInternalForceAt(iso[0]);//find internal force
            Console.WriteLine("internal force at x={0} is {1}", x, e4Force);
        }
    }
}
