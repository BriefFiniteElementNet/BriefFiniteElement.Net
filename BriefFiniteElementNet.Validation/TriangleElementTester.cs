﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using BriefFiniteElementNet.Validation.Case_01;
using HtmlTags;

namespace BriefFiniteElementNet.Validation
{
    public class TriangleElementTester: IValidator
    {
        public static void TestSingleElement()
        {
            var model = new Model();

            var elm = new TriangleElement();

            var arr = new[]
            {
                new Node(1, 1, 0),
                new Node(3.25, 0.5, 0),
                new Node(3.733, 1.92, 0),
            };

            arr.CopyTo(elm.Nodes, 0);

            model.Nodes.AddRange(arr);
            model.Elements.Add(elm);

            model.Nodes[2].Loads.Add(new NodalLoad(new Force(1, 1, 1, 1, 1, 1) * -5000));

            model.Nodes[0].Constraints =
                model.Nodes[1].Constraints =
                Constraints.Fixed;

            elm.Behavior = PlateElementBehaviours.Shell;
            elm.MembraneFormulation = MembraneFormulation.PlaneStress;
            elm.Material = new UniformIsotropicMaterial(2100, 0.3);
            elm.Section = new UniformParametric2DSection(0.2);

            model.Solve();

            Console.WriteLine(model.Nodes[2].GetNodalDisplacement().ToString(5));

            //var force = elm.GetLocalInternalForceAt(LoadCase.DefaultLoadCase, -0.57735, -0.57735);

        }

        public ValidationResult[] DoAllValidation()
        {
            var buf = new List<ValidationResult>();

            buf.Add(Validation_1());

            return buf.ToArray();
        }

        public ValidationResult[] DoPopularValidation()
        {
            var buf = new List<ValidationResult>();

            buf.Add(new Validator().Validate());

            buf.Add(Validation_2());



            return buf.ToArray();
        }

        public static ValidationResult Validation_1()
        {
            var nx = 5;
            var ny = 5;
            var nz = 5;

            #region model definition
            var grd = StructureGenerator.Generate3DTriangleElementGrid(nx, 1, nz);

            StructureGenerator.SetRandomiseSections(grd);
            StructureGenerator.SetRandomiseMaterial(grd);

            StructureGenerator.AddRandomiseNodalLoads(grd, LoadCase.DefaultLoadCase);//random nodal loads
            //StructureGenerator.AddRandomiseBeamUniformLoads(grd, LoadCase.DefaultLoadCase);//random elemental loads
            StructureGenerator.AddRandomDisplacements(grd, 0.3);
            #endregion




            StructureGenerator.AddRandomiseNodalLoads(grd, LoadCase.DefaultLoadCase);//random nodal loads

            grd.Solve_MPC();


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


            //var buf = new ValidationResult();

            var span = new HtmlTag("span");
            span.Add("p").Text("Validate a flat sheet");
            span.Add("h3").Text("Validate with");
            span.Add("paragraph").Text("OpenSEES (the Open System for Earthquake Engineering Simulation) software (available via http://opensees.berkeley.edu/)");
            span.Add("h3").Text("Validate objective");
            span.Add("paragraph").Text("compare nodal displacement from BFE.net library and OpenSEES for a model consist of 3d bar elements with random material and section for each one, that forms a grid"
                + " with a randomized nodal loading and narrow erratic on location of joint of grid elements.");

            span.Add("h3").Text("Model Definition");

            span.Add("paragraph")
                .Text(string.Format("A {0}x{1}x{2} grid, with {3} nodes and {4} bar elements." ,nx,ny,nz,grd.Nodes.Count,grd.Elements.Count)+
                      " Every node in the model have a random load on it.");

            span.Add("h3").Text("Validation Result");

            span.Add("paragraph")
                .Text(string.Format("Validation output for nodal displacements:"));


            span.Add("p").AddClass("bg-info").AppendHtml(string.Format("-Max ABSOLUTE Error: {0:e3}<br/>-Max RELATIVE Error: {1:e3}", maxDispAbsError, maxDispRelError));



            //span.Add("").Text(string.Format("Max ABSOLUTE Error: {0:e3}", maxAbsError));


            var id = "tbl_" + Guid.NewGuid().ToString("N").Substring(0, 5);

            span.Add("button").Attr("type", "button").Text("Toggle Details").AddClasses("btn btn-primary")
                .Attr("onclick", string.Format("$('#{0}').collapse('toggle');", id));

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

            var buf = new ValidationResult();
            buf.Span = span;
            buf.Title = "3D Grid Validation";

            return buf;
        }

        public static ValidationResult Validation_2()
        {
            var nx = 2;
            var ny = 1;
            var nz = 2;

            #region model definition
            var grd = StructureGenerator.Generate3DTriangleElementGrid(nx, ny, nz);

            //StructureGenerator.SetRandomiseSections(grd);
            //StructureGenerator.SetRandomiseMaterial(grd);

            //StructureGenerator.AddRandomiseNodalLoads(grd, LoadCase.DefaultLoadCase);//random nodal loads
            //StructureGenerator.AddRandomiseBeamUniformLoads(grd, LoadCase.DefaultLoadCase);//random elemental loads
            //StructureGenerator.AddRandomDisplacements(grd, 0.3);

            var zMax = grd.Nodes.Max(i => i.Location.Z);

            foreach (var node in grd.Nodes)
            {
                if (node.Location.Z > 0.99 * zMax)
                    node.Loads.Add(new NodalLoad(new Force(new Vector(000, 0000, 10000), Vector.Zero)));

            }

            #endregion




            //StructureGenerator.AddRandomiseNodalLoads(grd, LoadCase.DefaultLoadCase);//random nodal loads

            grd.Solve_MPC();


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


            //var buf = new ValidationResult();

            var span = new HtmlTag("span");
            span.Add("p").Text("Validate a flat vertical sheet");
            span.Add("h3").Text("Validate with");
            span.Add("paragraph").Text("OpenSEES (the Open System for Earthquake Engineering Simulation) software (available via http://opensees.berkeley.edu/)");
            span.Add("h3").Text("Validate objective");
            span.Add("paragraph").Text("compare nodal displacement from BFE.net library and OpenSEES for a model consist of 3d triangle elements with same material and section for all, that forms a straight square plate"
                + " with a loads on top nodes");

            span.Add("h3").Text("Model Definition");

            span.Add("paragraph")
                .Text(string.Format("A {0}x{1}x{2} grid, with {3} nodes and {4} bar elements.", nx, 1, nz, grd.Nodes.Count, grd.Elements.Count) );

            span.Add("h3").Text("Validation Result");

            span.Add("paragraph")
                .Text(string.Format("Validation output for nodal displacements:"));


            span.Add("p").AddClass("bg-info").AppendHtml(string.Format("-Max ABSOLUTE Error: {0:e3}<br/>-Max RELATIVE Error: {1:e3}", maxDispAbsError, maxDispRelError));



            //span.Add("").Text(string.Format("Max ABSOLUTE Error: {0:e3}", maxAbsError));


            var id = "tbl_" + Guid.NewGuid().ToString("N").Substring(0, 5);

            span.Add("button").Attr("type", "button").Text("Toggle Details").AddClasses("btn btn-primary")
                .Attr("onclick", string.Format("$('#{0}').collapse('toggle');", id));

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

            var buf = new ValidationResult();
            buf.Span = span;
            buf.Title = "3D Grid Validation";

            return buf;
        }
    }
}
