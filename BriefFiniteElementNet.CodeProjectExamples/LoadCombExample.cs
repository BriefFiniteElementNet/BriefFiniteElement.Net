using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Controls;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.CodeProjectExamples
{
    public class LoadCombExample
    {
        public void Run()
        {
            var model = new Model();

            model.Nodes.Add(new Node(0, 0, 0) { Label = "n0" });
            model.Nodes.Add(new Node(0, 2, 0) { Label = "n1" });
            model.Nodes.Add(new Node(4, 2, 0) { Label = "n2" });
            model.Nodes.Add(new Node(4, 0, 0) { Label = "n3" });

            model.Nodes.Add(new Node(0, 0, 1) { Label = "n4" });
            model.Nodes.Add(new Node(0, 2, 1) { Label = "n5" });
            model.Nodes.Add(new Node(4, 2, 1) { Label = "n6" });
            model.Nodes.Add(new Node(4, 0, 1) { Label = "n7" });


            var a = 0.1 * 0.1;//area, assume sections are 10cm*10cm rectangular
            var iy = 0.1 * 0.1 * 0.1 * 0.1 / 12.0;//Iy
            var iz = 0.1 * 0.1 * 0.1 * 0.1 / 12.0;//Iz
            var j = 0.1 * 0.1 * 0.1 * 0.1 / 12.0;//Polar
            var e = 20e9;//young modulus, 20 [GPa]
            var nu = 0.2;//poissons ratio

            var sec = new Sections.UniformParametric1DSection(a, iy, iz, j);
            var mat = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(e, nu);

            model.Elements.Add(new BarElement(model.Nodes["n0"], model.Nodes["n4"]) { Label = "e0", Section = sec, Material = mat});
            model.Elements.Add(new BarElement(model.Nodes["n1"], model.Nodes["n5"]) { Label = "e1", Section = sec, Material = mat });
            model.Elements.Add(new BarElement(model.Nodes["n2"], model.Nodes["n6"]) { Label = "e2", Section = sec, Material = mat });
            model.Elements.Add(new BarElement(model.Nodes["n3"], model.Nodes["n7"]) { Label = "e3", Section = sec, Material = mat });

            model.Elements.Add(new BarElement(model.Nodes["n4"], model.Nodes["n5"]) { Label = "e4", Section = sec, Material = mat });
            model.Elements.Add(new BarElement(model.Nodes["n5"], model.Nodes["n6"]) { Label = "e5", Section = sec, Material = mat });
            model.Elements.Add(new BarElement(model.Nodes["n6"], model.Nodes["n7"]) { Label = "e6", Section = sec, Material = mat });
            model.Elements.Add(new BarElement(model.Nodes["n7"], model.Nodes["n4"]) { Label = "e7", Section = sec, Material = mat });



            model.Nodes["n0"].Constraints =
                model.Nodes["n1"].Constraints =
                    model.Nodes["n2"].Constraints =
                        model.Nodes["n3"].Constraints =
                            Constraints.Fixed;

            var d_case = new LoadCase("d1", LoadType.Dead);
            var l_case = new LoadCase("l1", LoadType.Dead);
            var qx_case = new LoadCase("qx", LoadType.Dead);
            var qy_case = new LoadCase("qy", LoadType.Dead);

            var d1 = new Loads.UniformLoad(d_case, -1 * Vector.K, 2e3, CoordinationSystem.Global);
            var l1 = new Loads.UniformLoad(l_case, -1 * Vector.K, 1e3, CoordinationSystem.Global);



            model.Elements["e4"].Loads.Add(d1);
            model.Elements["e5"].Loads.Add(d1);
            model.Elements["e6"].Loads.Add(d1);
            model.Elements["e7"].Loads.Add(d1);

            model.Elements["e4"].Loads.Add(l1);
            model.Elements["e5"].Loads.Add(l1);
            model.Elements["e6"].Loads.Add(l1);
            model.Elements["e7"].Loads.Add(l1);

            var qx_f = new Force(5000 * Vector.I, Vector.Zero);
            var qy_f = new Force(10000 * Vector.J, Vector.Zero);

            model.Nodes["n4"].Loads.Add(new NodalLoad(qx_f, qx_case));
            model.Nodes["n4"].Loads.Add(new NodalLoad(qy_f, qy_case));

            model.Solve_MPC();




            var combination1 = new LoadCombination();// for D + 0.8 L
            combination1[d_case] = 1.0;
            combination1[l_case] = 0.8;

            var n3Force = model.Nodes["N3"].GetSupportReaction(combination1);
            Console.WriteLine(n3Force);

            var e4Force = (model.Elements["e4"] as BarElement).GetInternalForceAt(0, combination1);
            Console.WriteLine(e4Force);

        }
    }
}
