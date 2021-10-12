using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;

namespace BriefFiniteElementNet.Validation.GithubIssues
{
    public class Issue111
    {
       

        public static void Run1()
        {
            ///https://user-images.githubusercontent.com/10573467/111787314-781d7d00-88c7-11eb-869d-d8d7d40db005.png
            
            var model = new Model();

            //adding nodes
            model.Nodes.Add(new Node(0, 0, 0) { Label = "n0" });
            model.Nodes.Add(new Node(5, 0, 5) { Label = "n1" });

            //adding elements

            BarElement elm;

            model.Elements.Add(elm = new BarElement(model.Nodes["n0"], model.Nodes["n1"]) {Label = "e0"});

            //assign constraint to nodes
            model.Nodes["n0"].Constraints = Constraints.MovementFixed ;
            model.Nodes["n1"].Constraints = Constraints.MovementFixed;

            //define sections and material
            var secAA = new Sections.UniformGeometric1DSection(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));
            var mat = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);

            //assign materials
            (model.Elements["e0"] as BarElement).Material = mat;

            //assign sections
            (model.Elements["e0"] as BarElement).Section = secAA;

            //creating loads
            var u1 = new Loads.PartialNonUniformLoad() { Direction = Vector.K, CoordinationSystem = CoordinationSystem.Local };
            u1.SeverityFunction = Mathh.SingleVariablePolynomial.FromPoints(-1, -6, 1, -4);
            u1.StartLocation = new IsoPoint(-1);      //set locations of trapezoidal load
            u1.EndLocation = new IsoPoint(1);         //set locations of trapezoidal load

            //assign loads
            model.Elements["e0"].Loads.Add(u1);

            var cse = new LoadCase("T1", LoadType.Other);

            Util.AreaLoad2ConcentratedLoads(elm, u1, cse);

            //solve model
            model.Solve_MPC(LoadCase.DefaultLoadCase, cse);

            //retrieve solve result
            var n0reaction = model.Nodes["n0"].GetSupportReaction();
            var n0reaction2 = model.Nodes["n0"].GetSupportReaction(cse);

            var n1reaction = model.Nodes["n1"].GetSupportReaction();
            var n1reaction2 = model.Nodes["n1"].GetSupportReaction(cse);

            Console.WriteLine("Support reaction of n0: {0}", n0reaction);
            Console.WriteLine("Support reaction of n1: {0}", n1reaction);

            var x = 1.0; //need to find internal force at x = 1.0 m
            var iso = (model.Elements["e0"] as BarElement).LocalCoordsToIsoCoords(x);//find the location of 1m in iso coordination system
            var e4Force = (model.Elements["e0"] as BarElement).GetInternalForceAt(iso[0]);//find internal force
            Console.WriteLine("internal force at x={0} is {1}", x, e4Force);


            var r1 = new Force(fx: -13.33333333, fy: 0, fz: 13.333333333, mx:0, my:0, mz:0);
            var r2 = new Force(fx: -11.666666666, fy: 0, fz: 11.66666666, mx:0, my:0, mz:0);

            var d1 = n0reaction - r1;
            var d2 = n1reaction - r2;

            var epsilon = 1e-3;


            Debug.Assert(d1.Forces.Length < epsilon && d1.Moments.Length < epsilon);
            Debug.Assert(d2.Forces.Length < epsilon && d2.Moments.Length < epsilon);
        }


    }
}
