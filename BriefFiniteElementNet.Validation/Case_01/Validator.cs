using System.Linq;
using BriefFiniteElementNet.Elements;
using HtmlTags;

namespace BriefFiniteElementNet.Validation.Case_01
{
    [ValidationCase("I Beam Torsion with TriangleElement", typeof(TriangleElement))]
    public class Validator : IValidationCase
    {
        public ValidationResult Validate()
        {
            var val = new ValidationResult();

            val.Title = "I Beam torsion with triangle element";

            var span = val.Span = new HtmlTag("span");

            {//report
                
                span.Add("p").Text("Validate an I Beam nodal displacement and reactions and internal forces");
                span.Add("h3").Text("Validate with");
                span.Add("paragraph").Text("ABAQUS from Abaqus inc. available from www.simulia.com");
                span.Add("h3").Text("Validate objective");


                span.Add("paragraph").Text("compare nodal displacement for a model consist of Triangle Elements");

                span.Add("h3").Text("Model Definition");

                span.Add("paragraph")
                    .Text("An I shaped beam, totally fixed on one side and under a couple force on other side")
                    .AddClosedTag("br");

                span.Add("h3").Text("Validation Result");
            }
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

            model.Solve_MPC();


           

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


            var n50 = model.Nodes[50];
            var n56 = model.Nodes[56];
            var n57 = model.Nodes[57];


            {//nodal displacements

                span.Add("h4").Text("Nodal Displacements");
                span.Add("paragraph").Text(string.Format("Validation output for nodal displacements:"));

                var da = 1 / 0.0254 * A.GetNodalDisplacement().Displacements; // [inch]
                var db = 1 / 0.0254 * B.GetNodalDisplacement().Displacements; // [inch]

                var d50 = 1 / 0.0254 * n50.GetNodalDisplacement().Displacements; // [inch]
                var d56 = 1 / 0.0254 * n56.GetNodalDisplacement().Displacements; // [inch]
                var d57 = 1 / 0.0254 * n57.GetNodalDisplacement().Displacements; // [inch]

                var sap2000Da = new Vector(-0.014921, 0.085471, 0.146070); //tbl 7.14
                var sap2000Db = new Vector(-0.014834, -0.085475, -0.144533); //tbl 7.14

                var abaqusDa = new Vector(-15.4207E-03, 88.2587E-03, 150.910E-03); //node 9
                var abaqusDb = new Vector(-15.3246E-03, -88.2629E-03, -148.940E-03); //node 5

                var abaqus8 = new Vector(-120.875E-06, 88.3894E-03, -1.01662E-03); //node 8
                var abaqus12 = new Vector(15.3931E-03, 89.1206E-03, -149.515E-03); //node 12
                var abaqus41 = new Vector(-189.084E-06, 72.3778E-03, -734.918E-06); //node 41


                span.Add("paragraph").Text(string.Format("Validation output for nodal displacements:"));

                span.Add("paragraph").Text(string.Format("Err at node A (displacement): {0:0.00}%", Util.GetErrorPercent(da, abaqusDa))).AddClosedTag("br"); ;
                span.Add("paragraph").Text(string.Format("Err at node B (displacement): {0:0.00}%", Util.GetErrorPercent(db, abaqusDb))).AddClosedTag("br"); ;

                span.Add("paragraph").Text(string.Format("Err at node 41 (57) (displacement): {0:0.00}%", Util.GetErrorPercent(d50, abaqus41))).AddClosedTag("br"); ;
                span.Add("paragraph").Text(string.Format("Err at node 12 (56) (displacement): {0:0.00}%", Util.GetErrorPercent(d56, abaqus12))).AddClosedTag("br"); ;
                span.Add("paragraph").Text(string.Format("Err at node 08 (50) (displacement): {0:0.00}%", Util.GetErrorPercent(d57, abaqus8))).AddClosedTag("br"); ;
            }

            return val;
        }


    }
}
