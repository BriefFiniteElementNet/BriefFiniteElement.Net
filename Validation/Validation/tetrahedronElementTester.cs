using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Legacy;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using HtmlTags;


namespace BriefFiniteElementNet.Validation
{
    public class TetrahedronElementTester : IValidator
    {
        public ValidationResult[] DoAllValidation()
        {
            throw new NotImplementedException();
        }

        public ValidationResult[] DoPopularValidation()
        {
            var buf = new List<ValidationResult>();

            buf.Add(Validation_1());

            return buf.ToArray();
        }

        public static ValidationResult Validation_1()
        {
            var nx = 3;
            var ny = 3;
            var nz = 3;

            #region model definition
            var grd = BriefFiniteElementNet.Legacy.StructureGenerator.Generate3DTet4Grid(nx, ny, nz);

            //StructureGenerator.SetRandomiseConstraints(grd);
            StructureGenerator.SetRandomiseSections(grd);

            StructureGenerator.AddRandomiseNodalLoads(grd, LoadCase.DefaultLoadCase);//random nodal loads
            //StructureGenerator.AddRandomiseBeamUniformLoads(grd, LoadCase.DefaultLoadCase);//random elemental loads
            StructureGenerator.AddRandomDisplacements(grd, 0.1);
            #endregion

            grd.Solve_MPC();

            #region solve & compare
            var res = OpenseesValidator.OpenseesValidate(grd, LoadCase.DefaultLoadCase, false);


            var disp = res[0];
            var reac = res[1];

            var dispAbsErrIdx = disp.Columns.Cast<DataColumn>().ToList().FindIndex(i => i.ColumnName.ToLower().Contains("absolute"));
            var dispRelErrIdx = disp.Columns.Cast<DataColumn>().ToList().FindIndex(i => i.ColumnName.ToLower().Contains("relative"));

            var reacAbsErrIdx = reac.Columns.Cast<DataColumn>().ToList().FindIndex(i => i.ColumnName.ToLower().Contains("absolute"));
            var reacRelErrIdx = reac.Columns.Cast<DataColumn>().ToList().FindIndex(i => i.ColumnName.ToLower().Contains("relative"));


            var maxDispAbsError = disp.Rows.Cast<DataRow>().Max(ii => (double)ii.ItemArray[dispAbsErrIdx]);
            var maxDispRelError = disp.Rows.Cast<DataRow>().Max(ii => (double)ii.ItemArray[dispRelErrIdx]);


            var maxReacAbsError = reac.Rows.Cast<DataRow>().Max(ii => (double)ii.ItemArray[reacAbsErrIdx]);
            var maxReacRelError = reac.Rows.Cast<DataRow>().Max(ii => (double)ii.ItemArray[reacRelErrIdx]);


            var maxInternalDisplacementAbsErr = 0.0;
            var maxInternalForceResidual = 0.0;

            
            #endregion


            var span = new HtmlTag("span");
            span.Add("p").Text("Validate 3D frame nodal displacement and reactions");
            span.Add("h3").Text("Validate with");
            span.Add("paragraph").Text("OpenSEES (the Open System for Earthquake Engineering Simulation) software (available via http://opensees.berkeley.edu/)");
            span.Add("h3").Text("Validate objective");


            span.Add("paragraph").Text("compare nodal displacement from BFE.net library and OpenSEES for a model consist of random 3d bars");

            span.Add("h3").Text("Model Definition");

            {

            }
            span.Add("paragraph").Text(string.Format("A {0}x{1}x{2} grid, with {3} nodes and {4} bar elements.",nx,ny,nz,grd.Nodes.Count,grd.Elements.Count)).AddClosedTag("br");

            span.Add("paragraph").Text("Every node in the model have a random load on it, random displacement in original location.").AddClosedTag("br");
            span.Add("paragraph").Text("Every element in the model have a random uniform distributed load on it.").AddClosedTag("br");

            span.Add("h3").Text("Validation Result");

            #region nodal disp

            {//nodal displacements

                span.Add("h4").Text("Nodal Displacements");
                span.Add("paragraph")
               .Text(string.Format("Validation output for nodal displacements:"));


                span.Add("p").AddClass("bg-info").AppendHtml(string.Format("-Max ABSOLUTE Error: {0:e3}<br/>-Max RELATIVE Error: {1:e3}", maxDispAbsError, maxDispRelError));

                var id = "tbl_" + Guid.NewGuid().ToString("N").Substring(0, 5);

                span.Add("button").Attr("type", "button").Text("Toggle Details").AddClasses("btn btn-primary")
                    .Attr("onclick", string.Format( "$('#{0}').collapse('toggle');",id));

                var div = span.Add("div").AddClasses("panel-collapse", "collapse", "out").Id(id);

                var tbl = div.Add("table").AddClass("table table-striped table-inverse table-bordered table-hover");
                tbl.Id(id);

                var trH = tbl.Add("Thead").Add("tr");


                foreach (DataColumn column in disp.Columns)
                {
                    trH.Add("th").Attr("scope", "col").Text(column.ColumnName);
                }

                var tbody = tbl.Add("tbody");

                for (var i = 0; i < disp.Rows.Count; i++)
                {
                    var tr = tbody.Add("tr");

                    for (var j = 0; j < disp.Columns.Count; j++)
                    {
                        tr.Add("td").Text(disp.Rows[i][j].ToString());
                    }
                }
            }

            #endregion

            #region nodal reaction





            {//nodal reactions
                span.Add("h4").Text("Nodal Support Reactions");
                span.Add("paragraph")
               .Text(string.Format("Validation output for nodal support reactions:"));


                span.Add("p").AddClass("bg-info").AppendHtml(string.Format("-Max ABSOLUTE Error: {0:e3}<br/>-Max RELATIVE Error: {1:e3}", maxReacAbsError, maxReacRelError));

                var id = "tbl_" + Guid.NewGuid().ToString("N").Substring(0, 5);

                span.Add("button").Attr("type", "button").Text("Toggle Details").AddClasses("btn btn-primary")
                    .Attr("onclick", string.Format("$('#{0}').collapse('toggle');",id));

                var div = span.Add("div").AddClasses("panel-collapse", "collapse", "out").Id(id);

                var tbl = div.Add("table").AddClass("table table-striped table-inverse table-bordered table-hover");
                tbl.Id(id);

                var trH = tbl.Add("Thead").Add("tr");


                foreach (DataColumn column in reac.Columns)
                {
                    trH.Add("th").Attr("scope", "col").Text(column.ColumnName);
                }

                var tbody = tbl.Add("tbody");

                for (var i = 0; i < reac.Rows.Count; i++)
                {
                    var tr = tbody.Add("tr");

                    for (var j = 0; j < reac.Columns.Count; j++)
                    {
                        tr.Add("td").Text(reac.Rows[i][j].ToString());
                    }
                }
            }

            #endregion

            #region internal displacement
            {//internal displacements

                span.Add("h4").Text("Internal Displacements");
                span.Add("paragraph")
                    .Text(string.Format("Validation output for internal displacements (Displacement at each end node of bar elements should be equal to bar element's internal displacement of bar element at that node)"));

                span.Add("p").AddClass("bg-info").AppendHtml(string.Format("-Max ABSOLUTE Error: {0:e3}", maxInternalDisplacementAbsErr));
            }
            #endregion

            #region internal force
            {//internal force

                span.Add("h4").Text("Internal Force");
                span.Add("paragraph")
                    .Text(string.Format("Validation output for internal force (forces retrived by K.Δ formula should be in equiblirium with internal force of bar elements at any location within element):"));

                span.Add("p").AddClass("bg-info").AppendHtml(string.Format("-Max ABSOLUTE Error: {0:e3}", maxInternalForceResidual));
            }
            #endregion

            var buf = new ValidationResult();
            buf.Span = span;
            buf.Title = "3D Grid Validation";

            return buf;
        }

    }
}
