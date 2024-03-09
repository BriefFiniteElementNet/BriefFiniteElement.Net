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

namespace TestingConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //BriefFiniteElementNet.Validation.GithubIssues.Issue152.Run();
            //EulerBernouly2nodeChecker.Check2NodeShapeFunctionYDir();
            //EulerBernouly2nodeChecker.Check2NodeShapeFunctionZDir();
            //EulerBernouly2nodeChecker.CheckTrussShapeFunction();

            //TimoshenkoBeamChecker.Test2();
            //BriefFiniteElementNet.Validation.GithubIssues.Issue158.Run();
            //BriefFiniteElementNet.Validation.GithubIssues.Issue161.Run();
            //BriefFiniteElementNet.Validation.GithubIssues.Issue174.Run();
            //EulerBernouly2nodeChecker.TestConsoleBeam();
            //new BarElementExactInternalDisplacement().TestEulerBernouly_diry();

            test();
            Console.Write("Done");

            Console.ReadKey();
        }


        static void TestExactDisp()
        {

        }

        static void test()
        {
            var w = 2.0;
            var I = 1;
            var E = 1;
            var L = 4.0;

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0", Constraints = Constraints.MovementFixed & Constraints.FixedRX });
            nodes[1] = (new Node(L, 0, 0) { Label = "n1", Constraints = Constraints.MovementFixed & Constraints.FixedRX });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var sec = new UniformParametric1DSection(0, I, I);
            var mat = UniformIsotropicMaterial.CreateFromYoungPoisson(E, 0.25);

            elm.Section = sec;
            elm.Material = mat;
            var u1 = new BriefFiniteElementNet.Loads.UniformLoad(LoadCase.DefaultLoadCase, -Vector.K, w, CoordinationSystem.Global);

            elm.Loads.Add(u1);
            var model = new Model();
            model.Nodes.Add(nodes[0], nodes[1]);
            model.Elements.Add(elm);
            model.Solve_MPC();

            //https://mechanicalc.com/reference/beam-deflection-tables
            var dx = new Func<double, double>(x => -w * x / (24 * E * I) * (L * L * L - 2 * L * x * x + x * x * x));

            var xs = CalcUtil.DivideSpan(0, L, 10);

            var epsilon = 1e-10;

            foreach (var x in xs)
            {
                if (x == 0 || x == L) continue;

                var ksi = elm.LocalCoordsToIsoCoords(x)[0];
                var d1 = elm.GetExactInternalDisplacementAt(ksi).DZ;
                var d2 = dx(x);

                var diff = Math.Abs(d1 - d2);

                if(diff > epsilon)
                {
                    throw new Exception();
                }
            }
            
        }
    }
}
