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

        public ValidationResult[]  DoValidation()
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
            StructureGenerator.AddRandomiseLoading(grd, true, false, LoadCase.DefaultLoadCase);
            StructureGenerator.AddRandomDisplacements(grd, 0.1);

            grd.Solve_MPC();


            var res = OpenseesValidator.OpenseesValidate(grd, LoadCase.DefaultLoadCase, false);

            //var buf = new ValidationResult();

            var span = new HtmlTag("span");
            span.Add("h1").Text("Validate a 3D frame");
            span.Add("h2").Text("Validate with");
            span.Add("paragraph").Text("OpenSEES (the Open System for Earthquake Engineering Simulation) software (available via http://opensees.berkeley.edu/)");
            span.Add("h2").Text("Validate objective");
            span.Add("paragraph").Text("compare nodal displacement from BFE.net library and OpenSEES for a model consist of 3d bar elements that forms a grid"
                +"with a randomized nodal loading and narrow erratic on location of joint of grid elements");

            span.Add("h2").Text("Model Definition");

            span.Add("paragraph")
                .Text($"A {nx}x{ny}x{nz} grid, with {grd.Nodes.Count} nodes and {grd.Elements.Count} bar elements." +
                      "Every node in the model have a random load on it.");

            span.Add("h2").Text("Validation Result");
            span.Add("paragraph")
                .Text("This is out put of validation");

            var id = "tbl_" + Guid.NewGuid().ToString("N").Substring(0, 5);

            span.Add("button").Attr("type", "button").Text("Toggle")
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
    }
}
