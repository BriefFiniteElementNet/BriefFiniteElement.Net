using BriefFiniteElementNet.DebuggerVisualizers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation.Data.FlatShell___triangle.FlatPlate
{
    public class Validator
    {
        public static void Model1()
        {
            var nx = 6;
            var ny = 6;
            var nz = 1;
            var grd = Generate3DTriangleElementGridTest(nx, ny, nz);
            var model = grd;
            //Display
            ModelVisualizer.TestShowVisualizer(model);
            grd.Solve_MPC();

            writeDataTriangleElement(model, LoadCase.DefaultLoadCase);
        }
        #region meshing
        static Model Generate3DTriangleElementGridTest(int m, int n, int l)
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
            double t = 0.003;
            double E = 210000000000;
            double nu = 0.3;
            //elm1.Behavior = FlatShellBehaviour.ThinPlate;


            foreach (var elm in buf.Elements)
            {
                var triElm = elm as TriangleElement;


                if (triElm == null)
                    continue;

                //triElm.ElasticModulus = E;
                //triElm.PoissonRatio = nu;
                //triElm.Thickness = t;
                triElm.Material = new UniformIsotropicMaterial(E, nu);
                triElm.Section = new UniformParametric2DSection(t);
                triElm.Behavior = PlateElementBehaviours.Shell;
                triElm.MembraneFormulation = MembraneFormulation.PlaneStress;
            }


            for (int i = 0; i < m; i++)
            {

                nodes[i, 0, 0].Constraints = Constraint.Fixed;

            }
            for (int i = 0; i < m; i++)
            {
                nodes[i, n - 1, 0].Loads.Add(new NodalLoad(new Force(0, 2500, 2500, 0, 0, 0), LoadCase.DefaultLoadCase));
            }
            return buf;
        }
        #endregion
        #region output
        public static void writeDataTriangleElement(Model model, LoadCase testCase)
        {
            //Nodal displacements
            List<string> results = new List<string>();
            results.Add("Displacements");
            foreach (var Node in model.Nodes)
            {
                results.Add(Node.GetNodalDisplacement(testCase).DX + ";" + Node.GetNodalDisplacement(testCase).DY + ";" + Node.GetNodalDisplacement().DZ + ";" + Node.GetNodalDisplacement().RX + ";" + Node.GetNodalDisplacement().RY + ";" + Node.GetNodalDisplacement().RZ);
            }
            results.Add("Total stress at integration points");
            foreach (var element in model.Elements)
            {
                var el = (TriangleElement)element;
                var cauchy = el.GetInternalStress(new double[] { 0.166666666, 0.1666666, 1.0 }, LoadCombination.DefaultLoadCombination, SectionPoints.Envelope);
                results.Add(cauchy.S11 + ";" + cauchy.S12 + ";" + cauchy.S13 + ";" + cauchy.S21 + ";" + cauchy.S22 + ";" + cauchy.S23
                    + ";" + cauchy.S31 + ";" + cauchy.S32 + ";" + cauchy.S32 + ";" + CauchyStressTensor.GetVonMisesStress(cauchy));

                cauchy = el.GetInternalStress(new double[] { 0.6666666, 0.1666666, 1.0 }, LoadCombination.DefaultLoadCombination, SectionPoints.Envelope);
                results.Add(cauchy.S11 + ";" + cauchy.S12 + ";" + cauchy.S13 + ";" + cauchy.S21 + ";" + cauchy.S22 + ";" + cauchy.S23
                   + ";" + cauchy.S31 + ";" + cauchy.S32 + ";" + cauchy.S32 + ";" + CauchyStressTensor.GetVonMisesStress(cauchy));

                cauchy = el.GetInternalStress(new double[] { 0.166666666, 0.6666666, 1.0 }, LoadCombination.DefaultLoadCombination, SectionPoints.Envelope);
                results.Add(cauchy.S11 + ";" + cauchy.S12 + ";" + cauchy.S13 + ";" + cauchy.S21 + ";" + cauchy.S22 + ";" + cauchy.S23
                   + ";" + cauchy.S31 + ";" + cauchy.S32 + ";" + cauchy.S32 + ";" + CauchyStressTensor.GetVonMisesStress(cauchy));

            }
            File.WriteAllLines(@"D:\Solver\Validation\Output\Model1Displ.csv", results.ToArray());
          
        }
        #endregion
    }
}
