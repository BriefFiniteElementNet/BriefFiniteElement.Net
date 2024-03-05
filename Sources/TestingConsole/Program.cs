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

            var nodes = new Node[2];

            nodes[0] = (new Node(0, 0, 0) { Label = "n0", Constraints = Constraints.Fixed });
            nodes[1] = (new Node(4, 0, 0) { Label = "n1", Constraints = Constraints.Fixed });

            var elm = new BarElement(nodes[0], nodes[1]) { Label = "e0" };

            var mat = new UniformIsotropicMaterial(210e9, 0.25);
            var sec = new UniformParametric1DSection(1, 1, 1);

            elm.Section = sec;
            elm.Material = mat;
            var u1 = new BriefFiniteElementNet.Loads.UniformLoad(LoadCase.DefaultLoadCase, -Vector.K, w, CoordinationSystem.Global);

            elm.Loads.Add(u1);
            var model = new Model();
            model.Nodes.Add(nodes[0], nodes[1]);
            model.Elements.Add(elm);
            model.Solve_MPC();

            var d = elm.GetExactInternalDisplacementAt(0.0);


        }
    }
}
