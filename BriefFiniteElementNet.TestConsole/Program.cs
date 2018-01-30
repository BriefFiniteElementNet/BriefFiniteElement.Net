using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Resolvers;
using BriefFiniteElementNet.Controls;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Mathh;
using BriefFiniteElementNet.Sections;
using BriefFiniteElementNet.Validation;
using CSparse;
using CSparse.Double.Factorization;
using CSparse.Storage;
using BriefFiniteElementNet.Common;
using CSparse.Double;

namespace BriefFiniteElementNet.TestConsole
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Test_P_Delta_matrix();
            //TestSparseRow();

            //BarElementTester.TestBarStiffness();

            ////QrTest();
            //TstMtx();

            //TestCuda();
            //TestIntelMkl();

            TestWithOpensees();

            //Validation.TriangleElementTester.TestSingleElement();

            //Tst();

            Console.ReadKey();
        }


        private void Test1()
        {
            var model = StructureGenerator.Generate3DFrameElementGrid(5, 5, 5);
            //StructureGenerator.AddRandomiseLoading(model, LoadCase.DefaultLoadCase);

            StructureGenerator.AddRandomiseLoading(model, true, false, LoadCase.DefaultLoadCase);


            new Frame3DDValidator(model).Validate();

        }



        private static void TestSerialization()
        {
            Validation.SerializationTest.Test1();
        }

        private static void TestXmlSerialization()
        {
            //BriefFiniteElementNet.XmlSerialization.Tester.Test();
        }

        private static void Test2()
        {
            var model = StructureGenerator.Generate3DFrameElementGrid(5, 5, 5);
            //StructureGenerator.AddRandomiseLoading(model, LoadCase.DefaultLoadCase);

            //var wrapped = SerializationObsolete.ObjectWrapper.Wrap(model);

            var data = DataContractSerializerHelper.SerializeXml(model);

        }

        private static void TestBar()
        {
            var iy = 0.03;
            var iz = 0.02;
            var a = 0.01;
            var j = 0.05;

            var e = 7;
            var g = 11;
            var rho = 13;

            var model = new Model();

            model.Nodes.Add(new Node(0, 0, 0));
            model.Nodes.Add(new Node(1, 0, 0));

            var barElement = new BarElement(model.Nodes[0], model.Nodes[1]);

            barElement.Behavior = BarElementBehaviours.FullFrame;

            var frameElement = new FrameElement2Node(model.Nodes[0], model.Nodes[1])
            {
                Iy = iy,
                Iz = iz,
                A = a,
                J = j,
                E = e,
                G = g,
                MassDensity = rho
            };


            //barElement.Material = new UniformBarMaterial(e, g, rho);
            barElement.Section = new UniformParametric1DSection() {Iy = iy, Iz = iz, A = a,J=j};

            frameElement.MassFormulationType = MassFormulation.Consistent;

            //barElement.EndConnection = BarElementEndConnection.TotallyHinged;
            //barElement.StartConnection = BarElementEndConnection.TotallyHinged;

            frameElement.HingedAtStart = true;
            //frameElement.HingedAtEnd = true;


            var frameM = frameElement.GetLocalStiffnessMatrix();
            //MathUtil.FillLowerTriangleFromUpperTriangle(frameM);

            var barM = barElement.GetLocalStifnessMatrix();

            var t = 1;//- 1e-10;

            var d = (frameM - t* barM);//
            var dMax = d.CoreArray.Max(i => Math.Abs(i));

            model.Nodes[0].Constraints = Constraint.Fixed;

            model.Solve();
        }

        private static void TestTriangle()
        {
            var t = 0.01;
            var e = 210e9;
            var nu = 0.2;

            var n1 = new Node(new Point(0, 0, 0));
            var n2 = new Node(new Point(3, 5, 7));
            var n3 = new Node(new Point(1, -5, 4));

            var dkt = new TriangleFlatShell()
            {
                Behavior = FlatShellBehaviour.ThinPlate,
                PoissonRatio = nu,
                ElasticModulus = e,
                Thickness=t
            };


            dkt.Nodes[0] = n1;
            dkt.Nodes[1] = n2;
            dkt.Nodes[2] = n3;

            var tri = new TriangleElement();
            tri.Behavior = FlatShellBehaviours.FullThinShell;
            tri.Section = new UniformParametric2DSection() { T = t };
            tri.Material = new UniformIsotropicMaterial(e, nu);// {E = e, Nu = nu};

            tri.Nodes[0] = n1;
            tri.Nodes[1] = n2;
            tri.Nodes[2] = n3;


            var kTri = tri.GetLocalStifnessMatrix();
            var kDkt = dkt.GetLocalPlateBendingStiffnessMatrix();

            var d = kTri - kDkt;

            var xi = 0.162598494;
            var eta = 0.284984989;

            var b1 = new DktHelper().GetBMatrixAt(tri, xi, eta);
            var lpts = dkt.GetLocalPoints();

            var b2 = DktElement.GetBMatrix(xi, eta,
                new[] {lpts[0].X, lpts[1].X, lpts[2].X},
                new[] {lpts[0].Y, lpts[1].Y, lpts[2].Y});
            // new DktHelper().GetBMatrixAt(tri, tri.GetTransformationMatrix(), xi, eta);


            tri.GetLocalStifnessMatrix();
            //GC.Collect();
            
            var db = b1 - b2;
        }

        private static void TestVisualize()
        {
            var model = StructureGenerator.Generate3DFrameElementGrid(2, 2, 2);
            StructureGenerator.AddRandomiseLoading(model, true, false, LoadCase.DefaultLoadCase);

            ModelVisualizerControl.VisualizeInNewWindow(model);
        }

        private static void TestTransformation()
        {
            MatrixTransformValidator.Validate();
        }

        private static void TestTransposeMultiply()
        {
            var d1 = 200;
            var d2 = 200;

            var m1 = Matrix.RandomMatrix(d1, d2);
            var m2 = Matrix.RandomMatrix(d1, d1);

            var sp = System.Diagnostics.Stopwatch.StartNew();

            var res1 = m1.Transpose() * m2;

            Console.WriteLine("Usual took {0} Ms", sp.ElapsedMilliseconds);

            var res2 = new Matrix(res1.RowCount, res1.ColumnCount);

            sp.Restart();

            Matrix.TransposeMultiply(m1, m2, res2);

            Console.WriteLine("Optimal took {0} Ms", sp.ElapsedMilliseconds);

            var d = (res1 - res2).Max(ii => Math.Abs(ii));

        }

        private static void QrTest()
        {
            var coord = new CoordinateStorage<double>(7, 7, 1);

            coord.At(0, 2, 1);
            coord.At(0, 3, 1);
            coord.At(0, 4, 3);
            coord.At(0, 6, 2);

            coord.At(1, 2, 2);
            coord.At(1, 3, 6);
            coord.At(1, 4, 1);
            coord.At(1, 6, 5);

            coord.At(2, 2, 3);
            coord.At(2, 3, 7);
            coord.At(2, 4, 4);
            coord.At(2, 6, 7);

            var ccs = coord.ToCCs();

            var qr = SparseQR.Create(ccs, ColumnOrdering.Natural);

            //var r = GetFactorR(qr, "R").ToDenseMatrix();
            //var q = GetFactorR(qr, "Q").ToDenseMatrix();

            
            //var t = (q * r.Transpose());
            
        }

        

        private static void Test_P_Delta_matrix()
        {
            var model = StructureGenerator.Generate3DFrameElementGrid(2, 2, 2);


            var zs = model.Nodes
                .Where(i => i.Constraints != Constraint.Fixed)
                .Select(i => i.Location.Z).Distinct().ToList();

            
            foreach(var z in zs)
            {
                var relm = new RigidElement_MPC();
                var relm2 = new RigidElement();

                relm.Nodes.AddRange(model.Nodes.Where(i => i.Location.Z == z));
                relm2.Nodes.AddRange(model.Nodes.Where(i => i.Location.Z == z));

                model.MpcElements.Add(relm);
                model.RigidElements.Add(relm2);

                relm.UseForAllLoads = true;
                relm2.UseForAllLoads = true;
            }

            //StructureGenerator.AddRandomDisplacements(model, 0.1);

            /**/
            foreach (var node in model.Nodes)
            {
                if (node.Constraints == Constraint.Fixed)
                {
                    node.Settlements = new Displacement(1, 0, 0, 0, 0, 0);
                    node.Loads.Clear();
                }
                    
            }
            /**/


            StructureGenerator.AddRandomiseLoading(model, true, false, LoadCase.DefaultLoadCase);

            //model.Clone();

            #region

            #endregion

            model.Solve();
            //CalcUtil.GenerateP_Delta_Mpc(model, LoadCase.DefaultLoadCase,new GaussRrefFinder());
            model.LastResult.AddAnalysisResult(LoadCase.DefaultLoadCase);
            model.LastResult.AddAnalysisResult_MPC(LoadCase.DefaultLoadCase);

        }

        private static void TestBtDB()
        {
            var d1 = 36;
            var d2 = 27*6;

            var B = Matrix.RandomMatrix(d1, d2);
            var D = Matrix.RandomMatrix(d1, d1);

            var cnt = 1000;

            var sp = System.Diagnostics.Stopwatch.StartNew();

            Matrix res1 = null;

            for(var i = 0;i < cnt;i++)
            {
                res1 = B.Transpose() * D * B;
            }
            
            Console.WriteLine("Usual took {0} Ms", sp.ElapsedMilliseconds);


            var res2 = new Matrix(res1.RowCount, res1.ColumnCount);
            sp.Restart();

            for (var i = 0; i < cnt; i++)
                CalcUtil.Bt_D_B(B, D, res2);
            Console.WriteLine("Optimal took {0} Ms", sp.ElapsedMilliseconds);

            var d = (res1 - res2).Max(ii => Math.Abs(ii));
            Console.WriteLine("Err: {0:g}", d);
        }

        private static void TestIntelMkl()
        {
            var model = StructureGenerator.Generate3DBarElementGrid(1, 1, 2);

            
            //model.Nodes[4].Constraints = model.Nodes[5].Constraints = model.Nodes[6].Constraints = Constraints.Fixed;

            //model.Nodes[7].Constraints = Constraint.FromString("011101");

            var t = model.Nodes.Select(i => i.Constraints).ToArray();

            StructureGenerator.AddRandomiseLoading(model, true, false, LoadCase.DefaultLoadCase);

            var config = new SolverConfiguration();
            config.SolverFactory = new IntelMklSolver.MklPardisoDirectSPDSolverFactory();
            config.LoadCases = new List<LoadCase>() { LoadCase.DefaultLoadCase };

            model.Solve_MPC(config);
            //model.Solve(config);

            //model.Solve();

            var tmp = model.LastResult.Displacements.First().Value;

        }

        private static void TestWithOpensees()
        {
            var model = StructureGenerator.Generate3DBarElementGrid(5, 5, 5);

            StructureGenerator.AddRandomiseLoading(model, true, false, LoadCase.DefaultLoadCase);
            StructureGenerator.AddRandomDisplacements(model, 0.3);


            model.Solve_MPC();

            OpenseesValidator.OpenseesValidate(model, LoadCase.DefaultLoadCase, false);

        }

        private static void TestCuda()
        {
            var model = StructureGenerator.Generate3DFrameElementGrid(2, 2, 2);


            //model.Nodes[4].Constraints = model.Nodes[5].Constraints = model.Nodes[6].Constraints = Constraints.Fixed;

            //model.Nodes[7].Constraints = Constraint.FromString("011101");

            var t = model.Nodes.Select(i => i.Constraints).ToArray();

            StructureGenerator.AddRandomiseLoading(model, true, false, LoadCase.DefaultLoadCase);

            var config = new SolverConfiguration();
            config.SolverFactory = new CudaSolver.CuSparseDirectSpdSolverFactory();
            config.LoadCases = new List<LoadCase>() { LoadCase.DefaultLoadCase };

            model.Solve_MPC(config);
            //model.Solve(config);

            //model.Solve();

            var tmp = model.LastResult.Displacements.First().Value;

        }


        private static void Tst()
        {
            int n = 8;
            int[] ia/*[9]*/ = new int[] { 1, 5, 8, 10, 12, 15, 17, 18, 19 };

            int[] ja/*[18]*/ = new int[] { 1, 3, 6, 7,
                                         2, 3, 5,
                                         3, 8,
                                         4, 7,
                                         5, 6, 7,
                                         6, 8,
                                         7,
                                         8 };

            double[] a/*[18]*/ = new double[] { 7.0, 1.0, 2.0, 7.0,
                                              -4.0, 8.0, 2.0,
                                              1.0, 5.0,
                                              7.0, 9.0,
                                              5.0, 1.0, 5.0,
                                              -1.0, 5.0,
                                              11.0,
                                              5.0 };

            var csr = new CSparse.Double.CompressedColumnStorage(n, n, 18);
            csr.ColumnPointers = ia;
            csr.RowIndices = ja;
            csr.Values = a;

            
        }

        private static void TstMtx()
        {
            var crd = new CoordinateStorage<double>(5, 5, 1);

            crd.At(0, 0, 1);
            crd.At(0, 1, -1);
            crd.At(0, 3, -3);

            crd.At(1, 0, -2);
            crd.At(1, 1, 5);

            crd.At(2, 2, 4);
            crd.At(2, 3, 6);
            crd.At(2, 4, 4);

            crd.At(3, 0, -4);
            crd.At(3, 2, 2);
            crd.At(3, 3, 7);

            crd.At(4, 1, 8);
            crd.At(4, 4, -5);

            var sp = crd.ToCCs().Transpose();
            var dns = sp.ToDenseMatrix();

        }

        private static void TestSparseRow()
        {
            var r1 = new SparseRow();
            var r2 = new SparseRow();

            var rnd = new Random();

            r1.Add(24, 1);
            r1.Add(28, -0.064);
            r1.Add(29, -1.03);
            r1.Add(30, -1);


            r2.Add(24, 1);
            r2.Add(28, -0.07);
            r2.Add(29, -0.017);
            r2.Add(36, -1);


            var r3 = SparseRow.Eliminate(r1, r2, 24);

        }
    }
}
