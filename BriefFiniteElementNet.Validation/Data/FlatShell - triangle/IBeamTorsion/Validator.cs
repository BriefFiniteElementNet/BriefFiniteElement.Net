using BriefFiniteElementNet.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation.Data.FlatShell___triangle.IBeamTorsion
{
    public class Validator : IValidator
    {
        public ValidationResult[] DoAllValidation()
        {
            throw new NotImplementedException();
        }


        public ValidationResult Validate()
        {

            var val = new ValidationResult();

            //var magic = 0;

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

            var xSpan = l / (n - 1);

            var nodes = new Node[n][];

            for (var i = 0; i < n; i++)
            {
                var x = i * xSpan;

                nodes[i] = new Node[7];

                nodes[i][0] = new Node(x, 0, 0);
                nodes[i][1] = new Node(x, w / 2, 0);
                nodes[i][2] = new Node(x, w, 0);

                nodes[i][3] = new Node(x, w / 2, h / 2);

                nodes[i][4] = new Node(x, 0, h);
                nodes[i][5] = new Node(x, w / 2, h);
                nodes[i][6] = new Node(x, w, h);

                model.Nodes.AddRange(nodes[i]);
            }

            var pairs = new int[6][];

            pairs[0] = new int[] { 0, 1 };
            pairs[1] = new int[] { 1, 2 };
            pairs[2] = new int[] { 1, 3 };
            pairs[3] = new int[] { 3, 5 };
            pairs[4] = new int[] { 4, 5 };
            pairs[5] = new int[] { 5, 6 };


            var mat = new Materials.UniformIsotropicMaterial( e,no);
            var sec = new Sections.UniformParametric2DSection(t);


            for (var i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    var n11 = nodes[i][pairs[j][0]];
                    var n12 = nodes[i][pairs[j][1]];

                    var n21 = nodes[i + 1][pairs[j][0]];
                    var n22 = nodes[i + 1][pairs[j][1]];

                    {
                        var elm1 = new TriangleElement() { Material = mat, Section = sec };

                        elm1.Nodes[0] = n11;
                        elm1.Nodes[1] = n12;
                        elm1.Nodes[2] = n21;

                        model.Elements.Add(elm1);

                        var elm2 = new TriangleElement() { Material = mat, Section = sec };

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

            nodes[0].ToList().ForEach(i => i.Constraints = Constraints.Fixed);

            #endregion

            model.Trace.Listeners.Add(new BriefFiniteElementNet.Common.ConsoleTraceListener());
            new ModelWarningChecker().CheckModel(model);


            //ModelVisualizerControl.VisualizeInNewWindow(model);

            model.Solve();

            var A = nodes.Last()[2];
            var B = nodes.Last()[4];
            var C = nodes.First()[1];
            var D = nodes.First()[0];
            var E = nodes.Last()[6];

            /*
            for (int i = 0; i < nodes.Last().Length; i++)
            {
                nodes.Last()[i].Label = i.ToString();
            }
            */

            /**/
            A.Label = "A";
            B.Label = "B";
            C.Label = "C";
            D.Label = "D";
            E.Label = "E";
            /**/


            val.Title = "I Beam torsion with triangle element";

            var da = 1 / 0.0254 * A.GetNodalDisplacement().Displacements; // [inch]
            var db = 1 / 0.0254 * B.GetNodalDisplacement().Displacements; // [inch]

            var sap2000Da = new Vector(-0.014921, 0.085471, 0.146070); //tbl 7.14
            var sap2000Db = new Vector(-0.014834, -0.085475, -0.144533); //tbl 7.14

            var abaqusDa = new Vector(-15.4207E-03  ,   88.2587E-03   ,  150.910E-03); //node 9
            var abaqusDb = new Vector(-15.3246E-03, -88.2629E-03, -148.940E-03); //node 5

            Console.WriteLine("Err at A against abaqus (displacement): {0:0.00}%", GetError(da, abaqusDa));
            Console.WriteLine("Err at B against abaqus (displacement): {0:0.00}%", GetError(db, abaqusDb));

            
            {
                var e81 = model.Elements[85] as TriangleElement;

                var stress = e81.GetLocalInternalStress(LoadCase.DefaultLoadCase, 1 / 6.0, 1 / 6.0, 0);
            }

            for (int i = 0; i < model.Elements.Count; i++)
                model.Elements[i].Label = i.ToString();

            throw new NotImplementedException();
        }

        private static double GetError(double test, double accurate)
        {
            var buf = Math.Abs(test - accurate) / Math.Abs(accurate);

            return 100 * buf;
        }

        private static double GetError(Vector test, Vector accurate)
        {
            return 100 * Math.Abs((test - accurate).Length) / Math.Max(test.Length, accurate.Length);
        }

        public ValidationResult[] DoPopularValidation()
        {
            return new ValidationResult[] { Validate() };
        }
    }
}
