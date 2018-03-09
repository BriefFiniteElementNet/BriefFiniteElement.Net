using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using HtmlTags;

namespace BriefFiniteElementNet.Validation
{
    public class TestValidator:IValidator
    {
        public ValidationResult[] DoAllValidation()
        {
            throw new NotImplementedException();
        }

        public ValidationResult[] DoPopularValidation()
        {
            var res = new DataTable();// OpenseesValidator.OpenseesValidate(grd, LoadCase.DefaultLoadCase, false);

            res.Columns.Add("Col1");
            res.Columns.Add("Col2", typeof(double));

            var rnd = new Random();

            for (int i = 0; i < 10; i++)
            {
                res.Rows.Add(i, 100 * rnd.Next());
            }
            //var buf = new ValidationResult();

            var span = new HtmlTag("span");
            span.Add("h1").Text("Test Validation");
            span.Add("h2").Text("Validate with");
            span.Add("paragraph").Text("Nothing!");
            span.Add("h2").Text("Validate objective");
            span.Add("paragraph").Text("Temporary output text");

            span.Add("h2").Text("Model Definition");

            span.Add("paragraph")
                .Text($"-");

            span.Add("h2").Text("Validation Result");
            span.Add("paragraph")
                .Text("This is a temporary table");

            var id = "tbl_" + Guid.NewGuid().ToString("N").Substring(0, 5);

            span.Add("button").Attr("type","button").Text("Toggle")
                .Attr("onclick", $"$('#{id}').collapse('toggle');");

            var div = span.Add("div").AddClasses("panel-collapse", "collapse", "out").Id(id);

            var tbl = div.Add("table").AddClass("table table-striped table-inverse table-bordered table-hover");

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


            //div.Add("script").Attr("type", "text/javascript").Text(@"$('.spoiler-trigger').click(function() {$(this).parent().next().collapse('toggle');});");
            var buf = new ValidationResult();
            buf.Span = span;
            buf.Title = "Fake Validator";
            return new ValidationResult[] { buf , buf , buf , buf};
        }


    }
}
