using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using HtmlTags;

namespace BriefFiniteElementNet.Validation
{
    public class BarElementTester:IValidator
    {

        public ValidationResult[] DoAllValidation()
        {
            var buf = new List<ValidationResult>();

            buf.Add(TestFixedEndMoment_uniformLoad());
            buf.Add(Test_Trapezoid_1());

            return buf.ToArray();
        }

        public ValidationResult[]  DoPopularValidation()
        {
            var buf = new List<ValidationResult>();

            buf.Add(Validation_1());

            return buf.ToArray();
        }

        public static void TestBarStiffness()
        {
            var iy = 0.02;
            var iz = 0.02;
            var a = 0.01;
            var j = 0.05;

            var e = 210e9;
            var g = 70e9;
            var rho = 13;

            var model = new Model();

            model.Nodes.Add(new Node(0, 0, 0));
            model.Nodes.Add(new Node(3, 5, 7));

            var barElement = new BarElement(model.Nodes[0], model.Nodes[1]);

            barElement.Behavior = BarElementBehaviours.FullFrame;
            barElement.Material = UniformIsotropicMaterial.CreateFromYoungShear(e, g);

            var frameElement = new FrameElement2Node(model.Nodes[0], model.Nodes[1])
            {
                Iy = iy,
                Iz = iz,
                A = a,
                J = j,
                E = e,
                G = g,
                //MassDensity = rho
            };

            frameElement.ConsiderShearDeformation = false;

            //barElement.Material = new UniformBarMaterial(e, g, rho);
            barElement.Section = new UniformParametric1DSection() { Iy = iy, Iz = iz, A = a, J = j };

            var frK = frameElement.GetGlobalStifnessMatrix();
            var barK = barElement.GetGlobalStifnessMatrix();

            var d = (frK - barK).Max(i => Math.Abs(i));


        }

        public static ValidationResult Validation_1()
        {
            var nx = 5;
            var ny = 5;
            var nz = 5;

            var grd = StructureGenerator.Generate3DBarElementGrid(nx, ny, nz);

            //StructureGenerator.SetRandomiseConstraints(grd);
            StructureGenerator.SetRandomiseSections(grd);

            StructureGenerator.AddRandomiseNodalLoads(grd, LoadCase.DefaultLoadCase);//random nodal loads
            //StructureGenerator.AddRandomiseBeamUniformLoads(grd, LoadCase.DefaultLoadCase);//random elemental loads

            //StructureGenerator.AddRandomDisplacements(grd, 0.1);

            grd.Solve_MPC();


            var res = OpenseesValidator.OpenseesValidate(grd, LoadCase.DefaultLoadCase, false);

            var absErrIdx = res.Columns.Cast<DataColumn>().ToList().FindIndex(i => i.ColumnName.ToLower().Contains("absolute"));
            var relErrIdx = res.Columns.Cast<DataColumn>().ToList().FindIndex(i => i.ColumnName.ToLower().Contains("relative"));

            var maxAbsError = res.Rows.Cast<DataRow>().Max(ii => (double)ii.ItemArray[absErrIdx]);
            var maxRelError = res.Rows.Cast<DataRow>().Max(ii => (double)ii.ItemArray[relErrIdx]);

            //var buf = new ValidationResult();

            var span = new HtmlTag("span");
            span.Add("h1").Text("Validate a 3D frame");
            span.Add("h2").Text("Validate with");
            span.Add("paragraph").Text("OpenSEES (the Open System for Earthquake Engineering Simulation) software (available via http://opensees.berkeley.edu/)");
            span.Add("h2").Text("Validate objective");
            span.Add("paragraph").Text("compare nodal displacement from BFE.net library and OpenSEES for a model consist of 3d bar elements with random material and section for each one, that forms a grid"
                +" with a randomized nodal loading and narrow erratic on location of joint of grid elements.");

            span.Add("h2").Text("Model Definition");

            span.Add("paragraph")
                .Text($"A {nx}x{ny}x{nz} grid, with {grd.Nodes.Count} nodes and {grd.Elements.Count} bar elements." +
                      " Every node in the model have a random load on it.");

            span.Add("h2").Text("Validation Result");

            span.Add("paragraph")
                .Text(string.Format("Validation output for nodal displacements:"));


            span.Add("p").AddClass("bg-info").AppendHtml(string.Format("-Max ABSOLUTE Error: {0:e3}<br/>-Max RELATIVE Error: {1:e3}", maxAbsError, maxRelError));



            //span.Add("").Text(string.Format("Max ABSOLUTE Error: {0:e3}", maxAbsError));


            var id = "tbl_" + Guid.NewGuid().ToString("N").Substring(0, 5);

            span.Add("button").Attr("type", "button").Text("Toggle Details").AddClasses("btn btn-primary")
                .Attr("onclick", $"$('#{id}').collapse('toggle');");

            var div = span.Add("div").AddClasses("panel-collapse", "collapse", "out").Id(id);

            var tbl = div.Add("table").AddClass("table table-striped table-inverse table-bordered table-hover");
            tbl.Id(id);

            var trH = tbl.Add("Thead").Add("tr");


            foreach (DataColumn column in res.Columns)
            {
                trH.Add("th").Attr("scope", "col").Text(column.ColumnName);
            }

            var tbody = tbl.Add("tbody");

            for (var i = 0; i < res.Rows.Count; i++)
            {
                var tr = tbody.Add("tr");

                for (var j = 0; j < res.Columns.Count; j++)
                {
                    tr.Add("td").Text(res.Rows[i][j].ToString());
                }
            }

            var buf = new ValidationResult();
            buf.Span = span;
            buf.Title = "3D Grid Validation";

            return buf;
        }


        public static void ValidateSingleInclinedFrame()
        {
            var model = new Model();
            var ndes = new Node[] { new Node(0, 0, 0), new Node(2, 3, 5) };

            var h = 0.1;
            var w = 0.05;

            var a = h * w;
            var iy = h * h * h * w / 12;
            var iz = w * w * w * h / 12;
            var j = iy + iz;

            var sec = new Sections.UniformParametric1DSection(a, iy, iz, j);
            var mat = UniformIsotropicMaterial.CreateFromYoungPoisson(1, 0.25);

            var elm = new BarElement(ndes[0], ndes[1]) { Material = mat, Section = sec, Behavior = BarElementBehaviours.FullFrame };
            //var elm2 = new BarElement(ndes[1], ndes[2]) { Material = mat, Section = sec, Behavior = BarElementBehaviours.FullFrame };

            model.Elements.Add(elm);
            model.Nodes.Add(ndes);

            ndes[0].Constraints =  Constraints.Fixed;

            ndes[1].Loads.Add(new NodalLoad(new Force(0, 1, 0, 0, 0, 0)));

            model.Solve_MPC();

            var res = OpenseesValidator.OpenseesValidate(model, LoadCase.DefaultLoadCase, false);



        }

        public static double epsilon = 1e-9;

        public static ValidationResult TestFixedEndMoment_uniformLoad()
        {
            var buff = new ValidationResult();

            buff.Title = "Test #1 for UniformLoad on BarElement";
            buff.Span.Add("p").Text("endforce from uniformload should be statically in equiblirium with uniform load");


            var elm = new BarElement(new Node(0,0,0), new Node(8.66, 0, 5));
        
            elm.Behavior = BarElementBehaviours.FullFrame;

            var ld = new Loads.UniformLoad();
            ld.Magnitude = 1;//*Math.Sqrt(2);
            ld.Direction = Vector.K;
            ld.CoordinationSystem = CoordinationSystem.Global;
            elm.Loads.Add(ld);

            var loads = elm.GetEquivalentNodalLoads(ld);

            {//test 1 : static balance

                var l = (elm.Nodes[1].Location - elm.Nodes[0].Location);

                var totEndForces = new Force();

                for (int i = 0; i < loads.Length; i++)
                {
                    totEndForces += loads[i].Move(elm.Nodes[i].Location, elm.Nodes[0].Location);
                }

                var d = l / 2;

                var gDir = ld.Direction;

                if (ld.CoordinationSystem == CoordinationSystem.Local)
                    gDir = elm.GetTransformationManager().TransformLocalToGlobal(ld.Direction);

                var cos = (1 / (d.Length * gDir.Length)) * Vector.Dot(d, gDir);

                var f_mid =  gDir * ld.Magnitude* (l.Length);//uniform load as concentrated load at middle
                var m = Vector.Cross(d, f_mid);

                var expectedForce = new Force(f_mid, m);
                var zero = totEndForces - expectedForce;

                buff.ValidationFailed = !zero.Forces.Length.FEquals(0, epsilon) || !zero.Moments.Length.FEquals(0, epsilon);
            }


            return buff;
        }

        public static ValidationResult Test_Trapezoid_1()
        {
            var buff = new ValidationResult();

            buff.Title = "Test #2 for Trapezoid Load on BarElement";

            buff.Span = new HtmlTag("span");

            buff.Span.Add("p").Text("endforces from Trapezoidal load with 0 offset and same start and end should be same as uniform load");

            var elm = new BarElement(new Node(0, 0, 0), new Node(1, 0, 0));

            elm.Behavior = BarElementBehaviours.BeamZ;

            var direction = Vector.K + Vector.I + Vector.J;
            var ld_u = new Loads.UniformLoad();
            ld_u.Magnitude = 1;//*Math.Sqrt(2);
            ld_u.Direction = direction;
            ld_u.CoordinationSystem = CoordinationSystem.Global;

            var ld_t = new Loads.TrapezoidalLoad();
            ld_t.EndOffsets = ld_t.StartOffsets = new double[] { 0 };
            ld_t.StartMagnitudes = ld_t.EndMagnitudes = new double[] { 1 };
            ld_t.Direction = direction;
            ld_t.CoordinationSystem = CoordinationSystem.Global;

            var loads = elm.GetEquivalentNodalLoads(ld_u);
            var loads2 = elm.GetEquivalentNodalLoads(ld_t);

            var epsilon = 1e-9;

            {//test 1 : equality betweeb above

                var resid = new Force[loads.Length];

                for(var i = 0;i<loads.Length;i++)
                {
                    var f = resid[i] = loads[i] - loads2[i];

                    buff.ValidationFailed = Math.Abs(f.Forces.Length) > epsilon || Math.Abs(f.Moments.Length) > epsilon;
                }
            }


            return buff;
        }
    }
}
