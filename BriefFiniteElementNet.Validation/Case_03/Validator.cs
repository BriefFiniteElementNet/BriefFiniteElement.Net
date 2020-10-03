using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.Validation.Case_03
{
    [ValidationCase("Console beam with tetrahedron", typeof(TetrahedronElement))]
    public class Validator : IValidationCase
    {
        public ValidationResult Validate()
        {

            /**/
            {
                var model = StructureGenerator.Generate3DTetrahedralElementGrid(3, 3, 10);

                var l = model.Nodes.Max(i => i.Location.Z);

                var cnt = model.Nodes.Where(i => i.Location.Z == l);

                var f = 1e7;

                foreach (var node in cnt)
                {
                    node.Loads.Add(new NodalLoad(new Force(f / cnt.Count(), 0, 0, 0, 0, 0)));
                }

                model.Solve_MPC();

                var e = 210e9;
                var I = 2 * 2 * 2 * 2 / 12;

                var delta = f * l * l * l / (3 * e * I);

                var t = cnt.FirstOrDefault().GetNodalDisplacement();
            }
            /**/

            {
                //code from here: https://github.com/BriefFiniteElementNet/BriefFiniteElement.Net/issues/13

                var model = new Model();

                var p = new Point[20];
                var ns = new Node[20];

                p[0] = new Point(x: 0, y: 1, z: 0);
                p[1] = new Point(x: 0, y: 0, z: 0);
                p[2] = new Point(x: 0.20, y: 0, z: 0);
                p[3] = new Point(x: 0.20, y: 0, z: 0.20);
                p[4] = new Point(x: 0.20, y: 1, z: 0);
                p[5] = new Point(x: 0.20, y: 1, z: 0.20);
                p[6] = new Point(x: 0, y: 1, z: 0.20);
                p[7] = new Point(x: 0, y: 0, z: 0.20);
                p[8] = new Point(x: 0, y: 0.50, z: 0);
                p[9] = new Point(x: 0.20, y: 0.50, z: 0);
                p[10] = new Point(x: 0, y: 0.50, z: 0.20);
                p[11] = new Point(x: 0.20, y: 0.50, z: 0.20);
                p[12] = new Point(x: 0.20, y: 0.25, z: 0.20);
                p[13] = new Point(x: 0, y: 0.25, z: 0.20);
                p[14] = new Point(x: 0, y: 0.25, z: 0);
                p[15] = new Point(x: 0.20, y: 0.25, z: 0);
                p[16] = new Point(x: 0.20, y: 0.75, z: 0.20);
                p[17] = new Point(x: 0.20, y: 0.75, z: 0);
                p[18] = new Point(x: 0, y: 0.75, z: 0);
                p[19] = new Point(x: 0, y: 0.75, z: 0.20);

                for (var i = 0; i < 20; i++)
                {
                    model.Nodes.Add(ns[i] = new Node(p[i]));
                    ns[i].Label = "n" + i.ToString(CultureInfo.CurrentCulture);
                    ns[i].Constraints = Constraints.RotationFixed;
                }
                
                var mesh = new int[24][];

                mesh[0] = new int[] { 0, 4, 16, 17 };
                mesh[1] = new int[] { 8, 15, 12, 14 };
                mesh[2] = new int[] { 8, 16, 17, 18 };
                mesh[3] = new int[] { 10, 8, 11, 12 };
                //mesh[4] = new int[] { 5, 19, 0, 16 };
                mesh[5] = new int[] { 1, 15, 14, 12 };
                mesh[6] = new int[] { 8, 10, 11, 16 };
                mesh[7] = new int[] { 3, 13, 1, 7 };
                mesh[8] = new int[] { 3, 13, 12, 1 };
                mesh[9] = new int[] { 8, 12, 13, 14 };
                mesh[10] = new int[] { 1, 15, 12, 2 };
                mesh[11] = new int[] { 9, 8, 11, 16 };
                mesh[12] = new int[] { 10, 8, 12, 13 };
                mesh[13] = new int[] { 5, 0, 4, 16 };
                //mesh[14] = new int[] { 5, 19, 6, 0 };
                mesh[15] = new int[] { 8, 19, 16, 18 };
                mesh[16] = new int[] { 8, 19, 10, 16 };
                mesh[17] = new int[] { 0, 19, 18, 16 };
                mesh[18] = new int[] { 1, 3, 2, 12 };
                mesh[19] = new int[] { 8, 15, 9, 12 };
                mesh[20] = new int[] { 13, 12, 1, 14 };
                mesh[21] = new int[] { 8, 9, 11, 12 };
                mesh[22] = new int[] { 9, 8, 16, 17 };
                mesh[23] = new int[] { 16, 0, 17, 18 };


                var mat = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);

                foreach (var elm in mesh)
                {
                    if (elm == null)
                        continue;

                    var felm = new TetrahedronElement();

                    felm.Nodes[0] = ns[elm[0]];
                    felm.Nodes[1] = ns[elm[1]];
                    felm.Nodes[2] = ns[elm[2]];
                    felm.Nodes[3] = ns[elm[3]];

                    felm.Material = mat;

                    model.Elements.Add(felm);
                }

                ns[1].Constraints = ns[2].Constraints = ns[3].Constraints = ns[7].Constraints = Constraints.Fixed;



                var load = new BriefFiniteElementNet.NodalLoad();
                var frc = new Force();
                frc.Fz = 1000;// 1kN force in Z direction
                load.Force = frc;

                ns[5].Loads.Add(load);
                ns[6].Loads.Add(load);

                model.Solve_MPC();

                var d5 = ns[5].GetNodalDisplacement();
                var d6 = ns[6].GetNodalDisplacement();

                Console.WriteLine("Nodal displacement in Z direction is {0} meters (thus {1} mm)", d5.DZ, d5.DZ * 1000);
                Console.WriteLine("Nodal displacement in Z direction is {0} meters (thus {1} mm)", d6.DZ, d6.DZ * 1000);

                var tetra = model.Elements[0] as TetrahedronElement;

                var res = OpenseesValidator.OpenseesValidate(model, LoadCase.DefaultLoadCase, false);

                throw new NotImplementedException();
            }
        }
    }
}
