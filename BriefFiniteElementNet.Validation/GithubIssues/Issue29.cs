using BriefFiniteElementNet.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation.GithubIssues
{
    public class Issue29
    {
        public static void Run()
        {
            var nx = 6;
            var ny = 6;
            var nz = 1;
            var grd = Generate3DTriangleElementGridTest(nx, ny, nz);
            var model = grd;
            grd.Solve_MPC();

            //METHOD FOR MODEL:
            

        }

        public static Model Generate3DTriangleElementGridTest(int m, int n, int l)
        {
            var buf = new Model();

            var dx = 0.15;
            var dy = 0.15;
            var dz = 0.15;

            var nodes = new Node[m, n, l];

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < l; k++)
                    {
                        var pos = new Point(j * dx, i * dy, k * dz);
                        var nde = new Node() { Location = pos };
                        buf.Nodes.Add(nde);
                        nodes[i, j, k] = nde;
                    }
                }
            }

            //elements parallel to XZ
            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < m - 1; i++)
                {
                    for (int k = 0; k < l - 1; k++)
                    {
                        {
                            var elm2 = new TriangleElement();
                            elm2.Nodes[0] = nodes[i, j, k];
                            elm2.Nodes[1] = nodes[i, j, k + 1];
                            elm2.Nodes[2] = nodes[i + 1, j, k];

                            buf.Elements.Add(elm2);
                        }

                        {
                            var elm2 = new TriangleElement();
                            elm2.Nodes[0] = nodes[i + 1, j, k + 1];
                            elm2.Nodes[1] = nodes[i, j, k + 1];
                            elm2.Nodes[2] = nodes[i + 1, j, k];

                            buf.Elements.Add(elm2);
                        }

                    }
                }
            }


            //elements parallel to YZ
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    for (int k = 0; k < l - 1; k++)
                    {
                        {
                            var elm2 = new TriangleElement();
                            elm2.Nodes[0] = nodes[i, j, k];
                            elm2.Nodes[1] = nodes[i, j, k + 1];
                            elm2.Nodes[2] = nodes[i, j + 1, k];

                            buf.Elements.Add(elm2);
                        }

                        {
                            var elm2 = new TriangleElement();
                            elm2.Nodes[0] = nodes[i, j + 1, k + 1];
                            elm2.Nodes[1] = nodes[i, j, k + 1];
                            elm2.Nodes[2] = nodes[i, j + 1, k];

                            buf.Elements.Add(elm2);
                        }

                    }
                }
            }

            //elements parallel to XY
            for (int k = 0; k < l; k++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    for (int i = 0; i < m - 1; i++)
                    {
                        {
                            var elm2 = new TriangleElement();
                            elm2.Nodes[0] = nodes[i, j, k];
                            elm2.Nodes[1] = nodes[i + 1, j, k];
                            elm2.Nodes[2] = nodes[i, j + 1, k];

                            buf.Elements.Add(elm2);
                        }

                        {
                            var elm2 = new TriangleElement();
                            elm2.Nodes[0] = nodes[i + 1, j + 1, k];
                            elm2.Nodes[1] = nodes[i + 1, j, k];
                            elm2.Nodes[2] = nodes[i, j + 1, k];

                            buf.Elements.Add(elm2);
                        }

                    }
                }
            }


            foreach (var elm in buf.Elements)
            {
                var triElm = elm as TriangleElement;


                if (triElm == null)
                    continue;

                triElm.Behavior = PlaneElementBehaviours.FullThinShell;



                var h = 0.03;
                var w = 0.003;

                var e = 210e9;
                var nu = 0.3;

                var sec = (Sections.UniformParametric2DSection)(triElm.Section = new Sections.UniformParametric2DSection());

                var mat = triElm.Material = Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(e, nu);

                sec.T = 0.003;

            }


            for (int i = 0; i < m; i++)
            {

                nodes[i, 0, 0].Constraints = Constraint.Fixed;

            }
            for (int i = 0; i < m; i++)
            {
                nodes[i, n - 1, 0].Loads.Add(new NodalLoad(new Force(1500, 0, 0, 0, 0, 0), LoadCase.DefaultLoadCase));
            }
            return buf;
        }
    }
}
