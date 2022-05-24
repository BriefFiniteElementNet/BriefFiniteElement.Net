using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.CodeProjectExamples
{
    public class PartiNonUniformLoadExamples
    {
        public static void test1()
        {
            var load = new PartialNonUniformLoad();            //creating new instance of load
            load.SeverityFunction = Mathh.SingleVariablePolynomial.FromPoints(-1, 5, 1, 7);

            load.StartLocation = new IsoPoint(-0.5);      //set locations of trapezoidal load
            load.EndLocation = new IsoPoint(0.5);         //set locations of trapezoidal load

            load.Direction = Vector.K;                      //set direction
            load.CoordinationSystem =
                CoordinationSystem.Global;                  //set coordination system

            var model = new Model();

            model.Nodes.Add(new Node(0, 0, 0) { Label = "n0" });
            model.Nodes.Add(new Node(4, 0, 0) { Label = "n1" });

            model.Elements.Add(new BarElement(model.Nodes["n0"], model.Nodes["n1"]) { Label = "e0" });

            model.Nodes["n0"].Constraints =
                model.Nodes["n1"].Constraints =
                    Constraints.Fixed;

            var sec = new Sections.UniformGeometric1DSection(SectionGenerator.GetISetion(0.24, 0.67, 0.01, 0.006));
            var mat = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);

            (model.Elements["e0"] as BarElement).Material = mat;

            (model.Elements["e0"] as BarElement).Section = sec;

            var u1 = load;

            model.Elements["e0"].Loads.Add(u1);

            model.Solve_MPC();

            var n1Force = model.Nodes["n1"].GetSupportReaction();
            Console.WriteLine("support reaction of n1: {0}", n1Force);
            var elm = model.Elements[0] as BarElement;

            var frc = elm.GetGlobalEquivalentNodalLoads(u1);

        }


    }
}
