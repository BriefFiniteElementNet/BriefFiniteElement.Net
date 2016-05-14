using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.Validation
{
    public class DktElementChecker
    {
        public static void Test1()
        {
            //Example 7, page 120 of "Kaushalkumar Kansara" thesis

            #region creating model

            var model = new Model();

            var l = 144*0.0254; //in [m]
            var t = 6*0.0254;//

            var e = 24821126255.44; //3600 ksi
            var no = 0.2; //Poisson ratio

            var n = 9;

            var span = l/(n - 1);

            var nodes = new Node[n][];

            for (var i = 0; i < n; i++)
            {
                nodes[i] = new Node[n];

                for (var j = 0; j < n; j++)
                {
                    model.Nodes.Add(nodes[i][j] = new Node(i*span, j*span, 0));

                    if (i == 0 || i == n - 1 || j == 0 || j == n - 1)
                        nodes[i][j].Constraints = Constraint.Fixed;

                    var newCns = new Constraint(DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Released,
                        DofConstraint.Released, DofConstraint.Released, DofConstraint.Fixed);

                    nodes[i][j].Constraints = nodes[i][j].Constraints & newCns;
                }
            }

            for (var i = 0; i < n - 1; i++)
            {
                for (var j = 0; j < n - 1; j++)
                {
                    //first elements
                    var elm1 = new DktElement() {ElasticModulus = e, PoissonRatio = no, Thickness = t};

                    elm1.Nodes[0] = nodes[i][j];
                    elm1.Nodes[1] = nodes[i][j + 1];
                    elm1.Nodes[2] = nodes[i + 1][j];

                    model.Elements.Add(elm1);

                    //second elements
                    var elm2 = new DktElement() { ElasticModulus = e, PoissonRatio = no, Thickness = t };

                    elm2.Nodes[0] = nodes[i + 1][j + 1];
                    elm2.Nodes[1] = nodes[i][j + 1];
                    elm2.Nodes[2] = nodes[i + 1][j];

                    model.Elements.Add(elm2);
                }
            }

            //loading, 0.1 kips/sq. in. on all elements
            foreach (var elm in model.Elements)
            {
                elm.Loads.Add(new UniformLoadForPlanarElements()
                {
                    CoordinationSystem = CoordinationSystem.Global,
                    Uz = 689475.728 //0.1 kips/sq. in.
                });
            }

            //test loading, not one who is defined at pdf
            /*
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    nodes[i][j].Loads.Add(new NodalLoad(new Force(0, 0, 100, 0, 0, 0)));
                }
            }
            */


            #endregion

            model.Trace.Listeners.Add(new ConsoleTraceListener());
            new ModelWarningChecker().CheckModel(model);

            model.Solve();

            var d = nodes[n/2][n/2].GetNodalDisplacement();
        }

        public static void Test2()
        {
            //a console beam

            #region creating model

            var model = new Model();

            var l = 10; //in [m]
            var w = 1;

            var t = 0.1;//

            var e = 210e9; //3600 ksi
            var no = 0.2; //Poisson ratio

            var n = 10;
            var m = 100;

            var span = 0.1;

            var nodes = new Node[n+1][];

            for (var i = 0; i <= n; i++)
            {
                nodes[i] = new Node[m+1];

                for (var j = 0; j <= m; j++)
                {
                    model.Nodes.Add(nodes[i][j] = new Node(i * span, j * span, 0));

                    if (j == 0)
                        nodes[i][j].Constraints = Constraint.Fixed;

                    var newCns = new Constraint(DofConstraint.Fixed, DofConstraint.Fixed, DofConstraint.Released,
                        DofConstraint.Released, DofConstraint.Released, DofConstraint.Fixed);

                    nodes[i][j].Constraints = nodes[i][j].Constraints & newCns;
                }
            }

            for (var i = 0; i < n ; i++)
            {
                for (var j = 0; j < m ; j++)
                {
                    //first elements
                    var elm1 = new DktElement() { ElasticModulus = e, PoissonRatio = no, Thickness = t };

                    elm1.Nodes[0] = nodes[i][j];
                    elm1.Nodes[1] = nodes[i][j + 1];
                    elm1.Nodes[2] = nodes[i + 1][j];

                    model.Elements.Add(elm1);

                    //second elements
                    var elm2 = new DktElement() { ElasticModulus = e, PoissonRatio = no, Thickness = t };

                    elm2.Nodes[0] = nodes[i + 1][j + 1];
                    elm2.Nodes[1] = nodes[i][j + 1];
                    elm2.Nodes[2] = nodes[i + 1][j];

                    model.Elements.Add(elm2);
                }
            }

            //loading, 0.1 kips/sq. in. on all elements
            foreach (var elm in model.Elements)
            {
                elm.Loads.Add(new UniformLoadForPlanarElements()
                {
                    CoordinationSystem = CoordinationSystem.Global,
                    Uz = -1e3 
                });
            }

            //test loading, not one who is defined at pdf
            /*
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    nodes[i][j].Loads.Add(new NodalLoad(new Force(0, 0, 100, 0, 0, 0)));
                }
            }
            */


            #endregion

            model.Trace.Listeners.Add(new ConsoleTraceListener());
            new ModelWarningChecker().CheckModel(model);

            model.Solve();

            var d = nodes[n/2][m].GetNodalDisplacement();
        }

        public static void Test6()
        {
            Console.WriteLine("Example 14: I Beam With Flat Shell");

            var magic = 0;

            //example #13 p175

            #region creating model

            var model = new Model();

            var l = UnitConverter.In2M(40);
            var w = UnitConverter.In2M(10);
            var h = UnitConverter.In2M(5);
            var t = UnitConverter.In2M(0.25);

            var e = UnitConverter.Ksi2Pas(10000); //10'000 ksi
            var no = 0.3;

            var n = 9;

            var xSpan = l/(n - 1);

            var nodes = new Node[n][];

            for (var i = 0; i < n; i++)
            {
                var x = i*xSpan;

                nodes[i] = new Node[7];

                nodes[i][0] = new Node(x, 0, 0);
                nodes[i][1] = new Node(x, w/2, 0);
                nodes[i][2] = new Node(x, w, 0);

                nodes[i][3] = new Node(x, w/2, h/2);

                nodes[i][4] = new Node(x, 0, h);
                nodes[i][5] = new Node(x, w/2, h);
                nodes[i][6] = new Node(x, w, h);

                model.Nodes.AddRange(nodes[i]);
            }

            var pairs = new int[6][];

            pairs[0] = new int[] {0, 1};
            pairs[1] = new int[] {1, 2};
            pairs[2] = new int[] {1, 3};
            pairs[3] = new int[] {3, 5};
            pairs[4] = new int[] {4, 5};
            pairs[5] = new int[] {5, 6};

            for (var i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    var n11 = nodes[i][pairs[j][0]];
                    var n12 = nodes[i][pairs[j][1]];

                    var n21 = nodes[i + 1][pairs[j][0]];
                    var n22 = nodes[i + 1][pairs[j][1]];

                    {
                        var elm1 = new TriangleFlatShell() {Thickness = t, PoissonRatio = no, ElasticModulus = e};

                        elm1.Nodes[0] = n11;
                        elm1.Nodes[1] = n12;
                        elm1.Nodes[2] = n21;

                        model.Elements.Add(elm1);

                        var elm2 = new TriangleFlatShell() {Thickness = t, PoissonRatio = no, ElasticModulus = e};

                        elm2.Nodes[0] = n21;
                        elm2.Nodes[1] = n22;
                        elm2.Nodes[2] = n12;

                        model.Elements.Add(elm2);
                    }
                }
            }

            //loading
            nodes.Last()[0].Loads.Add(new NodalLoad(new Force(0, UnitConverter.Kip2N(1.6), 0, 0, 0, 0)));
            nodes.Last()[6].Loads.Add(new NodalLoad(new Force(0, -UnitConverter.Kip2N(1.6), 0, 0, 0, 0)));

            nodes[0].ToList().ForEach(i => i.Constraints = Constraint.Fixed);

            #endregion

            model.Trace.Listeners.Add(new BriefFiniteElementNet.ConsoleTraceListener());
            new ModelWarningChecker().CheckModel(model);


            //ModelVisualizerControl.VisualizeInNewWindow(model);

            model.Solve();

            var A = nodes.Last()[2];
            var B = nodes.Last()[4];

            var da = 1/0.0254*A.GetNodalDisplacement().Displacements; // [inch]
            var db = 1/0.0254*B.GetNodalDisplacement().Displacements; // [inch]

            var sap2000Da = new Vector(-0.014921, 0.085471, 0.146070); //tbl 7.14
            var sap2000Db = new Vector(-0.014834, -0.085475, -0.144533); //tbl 7.14

            Console.WriteLine("Err at A against Sap2000: {0:0.0}%", GetError(da, sap2000Da));
            Console.WriteLine("Err at B against Sap2000: {0:0.0}%", GetError(db, sap2000Db));

        }

        private static double GetError(Vector test, Vector accurate)
        {
            return 100 * Math.Abs((test - accurate).Length) / Math.Max(test.Length, accurate.Length);
        }
    }
}