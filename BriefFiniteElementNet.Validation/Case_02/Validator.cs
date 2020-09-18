using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation.Case_02
{
    public class Validator:IValidator
    {
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


        public ValidationResult Validate()
        {
            var val = new ValidationResult();

            Model model;
            #region create model

            {
                var nx = 6;
                var ny = 6;
                var nz = 1;
                var grd = Generate3DTriangleElementGridTest(nx, ny, nz);
                model = grd;
            }


            #endregion

            {
                model.Trace.Listeners.Add(new BriefFiniteElementNet.Common.ConsoleTraceListener());
                new ModelWarningChecker().CheckModel(model);
            }
           



            //Display
            //ModelVisualizer.TestShowVisualizer(model);
            model.Solve_MPC();

            var abaqusNodalDisp = "2.34E-05,1.99E-05,0.249477,0.826222,-3.52,0.000125609;4.16E-05,4.51E-05,1.05312,0.988111,-6.95463,0.000174579;5.47E-05,7.61E-05,2.2989,0.878222,-9.44415,0.000209719;6.30E-05,1.12E-04,3.84281,0.612378,-10.9571,0.000236374;6.56E-05,1.53E-04,5.54224,0.311383,-11.5553,0.000250549;0,0,0,0,0,0;1.10E-05,1.67E-05,0.312464,0.14585,-3.99132,0.000117342;2.06E-05,4.22E-05,1.15077,0.375383,-7.05487,0.000165009;2.77E-05,7.40E-05,2.39282,0.411118,-9.34073,0.000198152;3.17E-05,1.10E-04,3.91247,0.325068,-10.7541,0.000219099;3.36E-05,1.49E-04,5.57728,0.167102,-11.2656,0.000217948;0,0,0,0,0,0;3.33E-06,1.58E-05,0.324647,0.0108318,-4.10639,6.64E-05;6.22E-06,4.12E-05,1.18252,0.07555656,-7.15719,0.000143095;8.22E-06,7.34E-05,2.43115,0.115649,-9.3361,0.000175515;9.35E-06,1.10E-04,3.94409,0.102792,-10.68,0.00019205;1.03E-05,1.46E-04,5.59354,0.0521753,-11.1536,0.000217948;0,0,0,0,0,0;-3.09E-06,1.59E-05,0.326112,-0.032957,-4.10605,8.81E-05;-6.15E-06,4.15E-05,1.18395,-0.0783116,-7.16239,0.000136381;-8.65E-06,7.38E-05,2.43323,-0.098056,-9.34208,0.000170005;-1.01E-05,1.10E-04,3.94563,-0.0866577,-10.6729,0.000187627;-1.05E-05,1.46E-04,5.59322,-0.0623862,-11.142,0.00019038;0,0,0,0,0,0;-1.07E-05,1.69E-05,0.31744,-0.186162,-3.95012,9.19E-05;-2.09E-05,4.32E-05,1.15745,-0.325876,-7.08421,0.000143523;-2.82E-05,7.52E-05,2.40061,-0.366945,-9.35016,0.000180597;-3.25E-05,1.11E-04,3.91705,-0.304874,-10.7237,0.000205604;-3.46E-05,1.50E-04,5.57323,-0.219681,-11.2112,0.000223649;0,0,0,0,0,0;-2.49E-05,2.15E-05,0.264338,-0.74185,-3.64266,0.000112296;-4.27E-05,4.70E-05,1.06844,-0.948465,-6.98319,0.000158684;-5.54E-05,7.79E-05,2.31164,-0.850047,-9.439,0.00019675;-6.35E-05,1.13E-04,3.84927,-0.596331,-10.8837,0.000228331;-6.82E-05,1.56E-04,5.52876,-0.361105,-11.3,0.000253256;";
            

            writeDataTriangleElement(model, LoadCase.DefaultLoadCase);

            throw new NotImplementedException();
        }


        public ValidationResult[] DoPopularValidation()
        {
            throw new NotImplementedException();
        }

        public ValidationResult[] DoAllValidation()
        {
            return DoPopularValidation();
        }
    }
}
