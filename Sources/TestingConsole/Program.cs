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
using CSparse;

namespace TestingConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Etc.SampleTest();
            //return;
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

            //test();
            //tmp();
            //BriefFiniteElementNet.Validation.GithubIssues.Issue183.Run();
            //BriefFiniteElementNet.Validation.GithubIssues.Issue181.Run();
            //BriefFiniteElementNet.Validation.GithubIssues.Issue41.Run();

            //TestPerm();
            //HollowPermutationTests.TestPaQ();

            //HollowPermutationTests.TestTranspose();
            //HollowPermutationTests.TestArrMtx();
            //HollowPermutationTests.TestMultArr();
            //HollowPermutationTests.TestMultArr_Transpose();
            //HollowPermutationTests.TestMultMtx();
            //HollowPermutationTests.TestMult2Mtx();
            //HollowPermutationTests.TestMultMtx2();

            
            Console.Write("Done");

            //Console.ReadKey();
        }


        static void TestPerm()
        {
            var a = new int[] { 1, 5, 9, 7, 5, 3 };
            var p = new int[] { 0, 2, 3, 1, 4, 5 };

            var ap = new int[a.Length];

            Permutation.ApplyInverse(p, a, ap, 6);
        }

        static void tmp2()
        {
            var n1 = new Node(0, 0, 0);
            var n2 = new Node(1, 0, 0);

            var elm = new BarElement(n1, n2);

            elm.Section = new UniformGeometric1DSection(SectionGenerator.GetISetion(1, 1, 0.01, 0.01));
            elm.Material = UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.25);

            var model = new Model();
            model.Nodes.Add(n1, n2);
            model.Elements.Add(elm);

            model.Solve_MPC();
        }

        static void tmp()
        {
            var e = 210e9;

            #region generate model
            var sec = new UniformParametric1DSection();
            var sec2 = new UniformParametric1DSection();

            sec.A = 1;
            sec.Iy = 2;
            sec.Iz = 3;
            sec.J = 4;

            sec2.A = 5;

            var mat = UniformIsotropicMaterial.CreateFromYoungPoisson(e, 0.25);
            var mat2 = UniformIsotropicMaterial.CreateFromYoungPoisson(100, 0.25);

            var grid = StructureGenerator.Generate3DBarElementGrid(4, 4, 4,
                () => new BriefFiniteElementNet.Elements.BarElement() { Section = sec, Material = mat });

            //var lowest = grid.Nodes.Select(i => i.Location.Z).Min();

            var cns = Constraints.Fixed;
            cns.DZ = DofConstraint.Released;

            var lowestNodes = grid.Nodes.Where(n => n.Constraints == Constraints.Fixed).ToArray();

            foreach (var n in lowestNodes)
            {
                if (n.Constraints == Constraints.Fixed)//lowest nodes
                {
                    n.Constraints = cns;
                    var n2 = new Node(n.Location);
                    n2.Constraints = Constraints.Fixed;
                    var elm = new BarElement(n, n2);
                    elm.Material = mat2;
                    elm.Section = sec2;
                    //elm.KDz = 1;
                    grid.Elements.Add(elm);
                    grid.Nodes.Add(n2);
                }
            }

            #endregion

            var gen = new TclGenerator();

            gen.ElementTranslators.Add(new BarElement2Tcl() { TargetGenerator = gen });

            gen.ElementLoadTranslators.Add(new UniformLoad2Tcl() { TargetGenerator = gen });

            gen.ExportElementForces = false;
            gen.ExportNodalDisplacements = true;
            gen.ExportTotalStiffness = false;
            gen.ExportNodalReactions = true;


            StructureGenerator.AddRandomiseNodalLoads(grid, -1000, 1000, LoadCase.DefaultLoadCase);

            var tcl = gen.Create(grid, LoadCase.DefaultLoadCase);


            Model.Save(@"c:\temp\tt.bin", grid);
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

            var xs = BriefFiniteElementNet.Utils.NumericUtils.DivideSpan(0, L, 10);

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
