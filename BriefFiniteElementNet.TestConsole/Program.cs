using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Resolvers;
using BriefFiniteElementNet.Controls;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.MpcElements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.FemUtilies;
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
            Console.Title = "BFE tests & temporary codes";
            TestIssue20();
            //SimplySupportedBeamUDL();
            //BeamShapeFunction();
            //BarElementTester.TestEndReleaseStyiffness();

            //Test_P_Delta_matrix();
            //TestSparseRow();
            /*
            var pl = new Polynomial(1, 1, 1, 1);

            var ctrl = new FunctionVisualizer();

            ctrl.GraphColor = Colors.Black;
            //ctrl.HorizontalAxisLabel = "X";
            //ctrl.VerticalAxisLabel = "Y";
            ctrl.Min = -1;
            ctrl.Max = 1;
            ctrl.SamplingCount = 100;
            ctrl.TargetFunction = new Func<double, double>(i => pl.Evaluate(i));

            ctrl.UpdateUi();
            ctrl.InitializeComponent();

            new Window() { Content = ctrl, Title = "polynomial Visualizer!", Width = 500, Height = 300 }
                .ShowDialog();
            */
            //BarElementTester.ValidateConsoleUniformLoad();
            //BarElementTester.TestEndreleaseInternalForce();
            //SingleSpanBeamWithOverhang();
            //SingleSpanBeamWithOverhang();
            //TestTrussShapeFunction();
            //new BriefFiniteElementNet.Tests.BarElementTests().barelement_endrelease();
            //new BarElementTester.Test_Trapezoid_1
            //TestMultinodeBar1();


            //var grd = StructureGenerator.Generate3DTriangleElementGrid(5, 6, 7);
            //ModelVisualizerControl.VisualizeInNewWindow(grd);

            ////QrTest();
            //TstMtx();

            //TestCuda();
            //TestIntelMkl();
            //StiffnessCenterTest();

            //TestWithOpensees();

            //Validation.TriangleElementTester.TestSingleElement();

            //Tst();

            Console.ReadKey();
        }


        /// <summary>
        /// for issue # 20
        /// </summary>
        static void TestIssue20()
        {
            var model = new Model();

            model.Nodes.Add(new Node(0, 0, 0) { Label = "n1" });
            model.Nodes.Add(new Node(0, 0, 6) { Label = "n2" });
            model.Nodes.Add(new Node(6, 0, 6) { Label = "n3" });
            model.Nodes.Add(new Node(6, 0, 0) { Label = "n4" });

            model.Nodes["n1"].Constraints = Constraints.Fixed;
            model.Nodes["n4"].Constraints = Constraints.Fixed;

            model.Nodes["n2"].Loads.Add(new NodalLoad(new Force(10000, 0, 0, 0, 0, 0)));

            model.Elements.Add(new BarElement(model.Nodes["n1"], model.Nodes["n2"]) { Label = "r1" });
            model.Elements.Add(new BarElement(model.Nodes["n2"], model.Nodes["n3"]) { Label = "r2" });
            model.Elements.Add(new BarElement(model.Nodes["n3"], model.Nodes["n4"]) { Label = "r3" });

            (model.Elements["r1"] as BarElement).Section = new BriefFiniteElementNet.Sections.UniformGeometric1DSection(SectionGenerator.GetRectangularSection(0.01, 0.01));
            (model.Elements["r2"] as BarElement).Section = new BriefFiniteElementNet.Sections.UniformGeometric1DSection(SectionGenerator.GetRectangularSection(0.01, 0.01));
            (model.Elements["r3"] as BarElement).Section = new BriefFiniteElementNet.Sections.UniformGeometric1DSection(SectionGenerator.GetRectangularSection(0.01, 0.01));

            (model.Elements["r1"] as BarElement).Material = BriefFiniteElementNet.Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210 * Math.Pow(10, 9), 0.3);
            (model.Elements["r2"] as BarElement).Material = BriefFiniteElementNet.Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210 * Math.Pow(10, 9), 0.3);
            (model.Elements["r3"] as BarElement).Material = BriefFiniteElementNet.Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210 * Math.Pow(10, 9), 0.3);

            (model.Elements["r3"] as BarElement).StartReleaseCondition =
            new Constraint(dx: DofConstraint.Fixed, dy: DofConstraint.Fixed, dz: DofConstraint.Fixed, rx: DofConstraint.Fixed, ry: DofConstraint.Released, rz: DofConstraint.Fixed);

            model.Solve_MPC();

            Console.WriteLine((model.Elements["r2"] as BarElement).GetInternalForceAt(1, LoadCase.DefaultLoadCase).My.ToString());
            Console.WriteLine((model.Elements["r3"] as BarElement).GetInternalForceAt(-1, LoadCase.DefaultLoadCase).My.ToString());
            Console.ReadKey();
        }
        /*

                   3 kN/m                           
         ^^^^^^^^^^^^^^^^^^^^^^^^^^^              
         ----------------------------
        /\           10m             /\

        n1                         n2   

        */


        public static void BeamShapeFunction()
        {
            var fix = Constraints.Fixed;

            var beam1 = new BarElement(3);

            beam1.Nodes[0] = new Node(0, 0, 0) { Constraints = fix };
            beam1.Nodes[1] = new Node(1, 0, 0) { Constraints = fix };
            beam1.Nodes[2] = new Node(2, 0, 0) { Constraints = fix };


            var old = beam1.StartReleaseCondition;//.DZ = DofConstraint.Released;

            old.DZ = DofConstraint.Released;
            //old.RZ = DofConstraint.Released;

            beam1.StartReleaseCondition = old;

            
            var hlpr = new EulerBernoulliBeamHelper(BeamDirection.Y);

            Polynomial[] ns, ms;

            hlpr.GetShapeFunctions(beam1, out ns, out ms);

        }

        public static void SimplySupportedBeamUDL()
        {
            var model = new BriefFiniteElementNet.Model();

            var pin = new Constraint(
              dx: DofConstraint.Fixed, dy: DofConstraint.Fixed, dz: DofConstraint.Fixed,
              rx: DofConstraint.Fixed, ry: DofConstraint.Released, rz: DofConstraint.Released);

            Node n1, n2;

            model.Nodes.Add(n1 = new Node(x: 0.0, y: 0.0, z: 0.0) { Constraints = pin });
            model.Nodes.Add(n2 = new Node(x: 10.0, y: 0.0, z: 0.0) { Constraints = pin });

            var elm1 = new BarElement(n1, n2);
            model.Elements.Add(elm1);

            double height = 0.200;
            double width = 0.050;
            double E = 7900;
            var section = new UniformGeometric1DSection(SectionGenerator.GetRectangularSection(height, width));
            BaseMaterial material = UniformIsotropicMaterial.CreateFromYoungPoisson(E, 1);

            elm1.Section = section;
            elm1.Material = material;

            var u1 = new Loads.UniformLoad(LoadCase.DefaultLoadCase, new Vector(0, 1, 1), -1, CoordinationSystem.Global);
            elm1.Loads.Add(u1);

            model.Solve_MPC();

            double x;
            Force reaction1 = n1.GetSupportReaction();
            x = reaction1.Fz; //15000 = 3*10000/2 -> correct
            x = reaction1.My; // 0 -> correct

            Force f1_internal = elm1.GetExactInternalForceAt(-1+1e-10);//-1 is start
            x = f1_internal.Fz; 
            x = f1_internal.My;

            var delta = elm1.GetInternalDisplacementAt(0);

        }

        public static void SimpleBeamInternalMoment()
        {
            var model = new Model();
            model.Nodes.Add(new Node(0, 0, 0) { Label = "n1" });
            model.Nodes.Add(new Node(6, 0, 0) { Label = "n2" });
            model.Nodes["n1"].Constraints = Constraints.FixedDZ & Constraints.FixedDY & Constraints.FixedRX & Constraints.FixedRZ;
            model.Nodes["n2"].Constraints = Constraints.MovementFixed;
            model.Elements.Add(new BarElement(model.Nodes["n1"], model.Nodes["n2"]) { Label = "r1" });
            (model.Elements["r1"] as BarElement).Section = new BriefFiniteElementNet.Sections.UniformGeometric1DSection(SectionGenerator.GetRectangularSection(0.01, 0.01));
            (model.Elements["r1"] as BarElement).Material = BriefFiniteElementNet.Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210 * Math.Pow(10, 9), 0.3);
            model.Elements["r1"].Loads.Add(new BriefFiniteElementNet.Loads.UniformLoad(LoadCase.DefaultLoadCase, new Vector(0, 0, 1), -10000, CoordinationSystem.Local));
            model.Solve_MPC();

            var locs = new double[] { 1e-5, 1, 2, 3, 4, 5, 6 - 1e-5 };

            foreach(var loc in locs)
            {
                var xi = (model.Elements["r1"] as BarElement).LocalCoordsToIsoCoords(loc)[0];
                var f = (model.Elements["r1"] as BarElement).GetExactInternalForceAt(xi, LoadCase.DefaultLoadCase);
                Console.WriteLine("@X={0}, My={1}", loc, f.My);
            }

            
            Console.ReadKey();
        }

        public static void SingleSpanBeamWithOverhang()
        {
            var model = new BriefFiniteElementNet.Model();

            var pin = new Constraint(dx: DofConstraint.Fixed, dy: DofConstraint.Fixed, dz: DofConstraint.Fixed, rx: DofConstraint.Fixed, ry: DofConstraint.Fixed, rz: DofConstraint.Released);

            Node n1, n2;

            model.Nodes.Add(n1 = new Node(x: 0.0, y: 0.0, z: 0.0) { Constraints = pin });
            model.Nodes.Add(n2 = new Node(x: 10.0, y: 0.0, z: 0.0) { Constraints = pin });

            var elm1 = new BarElement(n1, n2);

            model.Elements.Add(elm1);

            elm1.Section = new BriefFiniteElementNet.Sections.UniformParametric1DSection(a: 0.01, iy: 8.3e-6, iz: 8.3e-6, j: 16.6e-6);//section's second area moments Iy and Iz = 8.3*10^-6, area = 0.01
            elm1.Material = BriefFiniteElementNet.Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.3);//Elastic mudule is 210e9 and poisson ratio is 0.3

            var frc = new Force();
            frc.Fz = 1000;// 1kN force in Z direction

            var elementLoad = new BriefFiniteElementNet.Loads.ConcentratedLoad();

            elementLoad.CoordinationSystem = CoordinationSystem.Local;
            elementLoad.Force = frc;
            elementLoad.ForceIsoLocation = new IsoPoint(0);

            //frc, 5, CoordinationSystem.Local);
            elm1.Loads.Add(elementLoad);//is this possible?

            model.Solve_MPC();//crashes here


            var val = elm1.GetExactInternalForceAt(-0.25);

            var d2 = n2.GetNodalDisplacement();
        }


        private static void TestTrussShapeFunction()
        {

            var bar = new BarElement(3);

            bar.Nodes[0] = new Node(0, 0, 0);
            bar.Nodes[1] = new Node(1, 0, 0);
            bar.Nodes[2] = new Node(2, 0, 0);

            bar.Material = UniformIsotropicMaterial.CreateFromYoungPoisson(3, 0.3);
            bar.Section = new Sections.UniformParametric1DSection(4);

            var hlp = new TrussHelper();

            var pl = hlp.GetN_i(bar, 0);
            hlp.GetJMatrixAt(bar, 0);
            var stf = hlp.CalcLocalStiffnessMatrix(bar);
        }

        private static void TestMultinodeBar1()
        {

            var n = 2;

            var bar = new BarElement(n);

            var hlp = new EulerBernoulliBeamHelper(BeamDirection.Y);

           

            for (var i = 0; i < n; i++)
            {
                bar.Nodes[i] = new Node(i * 3, 0, 0);
            }


            var testXi = -0.66;

            //var n1 = hlp.GetNMatrixBar2Node(bar, testXi);
            var n2 = hlp.GetNMatrixAt(bar, testXi);

            var b = hlp.GetBMatrixAt(bar, testXi);

            //var d = n1 - n2;

        }

        private void Test1()
        {
            var model = StructureGenerator.Generate3DFrameElementGrid(5, 5, 5);
            //StructureGenerator.AddRandomiseLoading(model, LoadCase.DefaultLoadCase);

            StructureGenerator.AddRandomiseLoading(model, true, false, LoadCase.DefaultLoadCase);


            new Frame3DDValidator(model).Validate();

        }


        public static void StiffnessCenterTest()
        {
            #region model
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

            model.Elements.Add(new BarElement(model.Nodes["n0"], model.Nodes["n4"]) { Label = "e0", Section = sec, Material = mat });
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
            #endregion

            var rgd = new RigidElement_MPC();

            rgd.Nodes.Add(model.Nodes["n4"]);
            rgd.Nodes.Add(model.Nodes["n5"]);
            rgd.Nodes.Add(model.Nodes["n6"]);
            //rgd.Nodes.Add(model.Nodes["n7"]);




            //var eqDof = new TelepathyLink

            model.MpcElements.Add(rgd);

            var loc = new StiffnessCenterFinder().GetCenters(model, rgd, LoadCase.DefaultLoadCase);

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
            tri.Behavior = TriangleElementBehaviours.Shell;
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
                .Where(i => i.Constraints != Constraints.Fixed)
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
                if (node.Constraints == Constraints.Fixed)
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
            var oldPath = Environment.GetEnvironmentVariable("PATH");

            var newPath = oldPath + @"C:\Program Files (x86)\IntelSWTools\compilers_and_libraries_2018.0.124\windows\redist\intel64_win\mkl;";

            Environment.SetEnvironmentVariable("PATH", newPath);

            //BriefFiniteElementNet.PardisoThing.test_pardiso.Main(null);

            return;
            var model = StructureGenerator.Generate3DBarElementGrid(1, 1, 2);

            
            //model.Nodes[4].Constraints = model.Nodes[5].Constraints = model.Nodes[6].Constraints = Constraints.Fixed;

            //model.Nodes[7].Constraints = Constraint.FromString("011101");

            var t = model.Nodes.Select(i => i.Constraints).ToArray();

            StructureGenerator.AddRandomiseLoading(model, true, false, LoadCase.DefaultLoadCase);

            var config = new SolverConfiguration();
            //config.SolverFactory = new IntelMklSolver.MklPardisoDirectSPDSolverFactory();
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
            //config.SolverFactory = new CudaSolver.CuSparseDirectSpdSolverFactory();
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

            var csr = new CSparse.Double.SparseMatrix(n, n, 18);
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
