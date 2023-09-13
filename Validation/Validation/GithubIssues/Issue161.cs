using System.Diagnostics;
using System;
using BriefFiniteElementNet;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Loads;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;

namespace BriefFiniteElementNet.Validation.GithubIssues
{
    public class Issue161
    {
        public static void Run()
        {
            {
                var h = 0.3;//200mm
                var w = 0.3;//100mm
                var h1 = 0.4;//200mm
                var w1 = 0.25;//100mm

                var e = 2.189e9;
                var m = 1000.0;
                var n = 800.0;


                Node node1, node2;
                BarElement element1;
                Model model;
                string dl = "", lqx = "", lqy = "";
                LoadCase lc_dl;
                LoadCase lc_eqx;
                LoadCase lc_eqy;

                var geo = SectionGenerator.GetRectangularSection(h, w);
                var sec = new UniformGeometric1DSection(geo);

                var mat = new UniformIsotropicMaterial(e, 0.3);

                node1 = new BriefFiniteElementNet.Node(0, 0, 0) { Label = "n1" };
                node2 = new BriefFiniteElementNet.Node(0, 0, 3.5) { Label = "n2" };

                node1.Constraints = Constraints.Fixed;

                element1 = new BarElement(node1, node2) { Section = sec, Material = mat, };

                lc_eqx = new LoadCase(lqx, LoadType.Quake);
                lc_eqy = new LoadCase(lqy, LoadType.Quake);

                var fuerzax = new Force(m * Vector.I, Vector.Zero);
                var fuerzay = new Force(m * Vector.J, Vector.Zero);

                node2.Loads.Add(new NodalLoad(fuerzax, lc_eqx));
                node2.Loads.Add(new NodalLoad(fuerzay, lc_eqy));

                model = new Model();

                model.Nodes.Add(node1, node2);
                model.Elements.Add(element1);

                model.Solve_MPC();

                var f1 = element1.GetInternalForceAt(-0.99999999999, lc_eqx); // 
                var f9 = element1.GetInternalForceAt(0.999999999999, lc_eqx);

                var f1a = element1.GetInternalForceAt(-0.99999999999, lc_eqy); // 
                var f9a = element1.GetInternalForceAt(0.999999999999, lc_eqy);

                var mgr = element1.GetTransformationManager();

                var buf = mgr.TransformLocalToGlobal(f1);
                var buf1 = mgr.TransformLocalToGlobal(f9);

                var buf2 = mgr.TransformLocalToGlobal(f1a);
                var buf3 = mgr.TransformLocalToGlobal(f9a);

                Console.WriteLine("My 1 = " + Math.Round(buf.My, 3) + " : " + "Mz 1 = " + Math.Round(buf2.Mz, 3));
                Console.WriteLine("My 2 = " + Math.Round(buf1.My, 3) + " : " + "Mz 2 = " + Math.Round(buf3.Mz, 3));

                //My 1 = 3500 : Mz 1 = 0
                //My 2 = 0 : Mz 2 = 0
            }
        }
    }
}
