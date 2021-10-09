using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Controls;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Validation.Properties;
using HtmlTags;

namespace BriefFiniteElementNet.Validation.Case_03
{
    [ValidationCase("Console beam with tetrahedron", typeof(TetrahedronElement))]
    public class Validator : IValidationCase
    {
        public ValidationResult Validate()
        {
            //build model from Abaqus input file
            string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
            string FileName = string.Format("Case_03\\Job-10.inp", Path.GetFullPath(Path.Combine(RunningPath, @"..\..\")));
            var abaqusModel = AbaqusInputFileReader.AbaqusInputToBFE(FileName);
            //comparison
            var val = new ValidationResult();
            var span = val.Span = new HtmlTag("span");
            val.Title = "Console beam with tetrahedron elements";

            {//report

                span.Add("p").Text("Validation of a simple clamped beam, loaded on the other side.");
                span.Add("h3").Text("Validate with");
                span.Add("paragraph").Text("ABAQUS from Abaqus inc. available from www.simulia.com");
                span.Add("h3").Text("Validate objective");
                span.Add("paragraph").Text("compare nodal displacement for a model consist of tetrahedron elements");

                span.Add("h3").Text("Model Definition");
                /*

                model.MpcElements.Add(rigid);
                model.Trace.Listeners.Add(new ConsoleTraceListener()); 
                model.Solve_MPC();
                

                var delta = f * l * l * l / (3 * e * I);

                var t = cnt.FirstOrDefault().GetNodalDisplacement();
                 
                var ratio = delta / t.DX;*/

                span.Add("paragraph")
                    .Text("Look at `intro.md` file in this folder")
                    .AddClosedTag("br");

                span.Add("h3").Text("Validation Result");
//>>>>>>> 8f73e9cf4109d3eac9e87e27ab66e1661bd0a2fd
            }

            abaqusModel.Trace.Listeners.Add(new BriefFiniteElementNet.Common.ConsoleTraceListener());
            new ModelWarningChecker().CheckModel(abaqusModel);


            //Display
            //ModelVisualizer.TestShowVisualizer(model);
            abaqusModel.Solve_MPC();

            var abqlocDic = new Dictionary<int, Point>();
            var abqdispDic = new Dictionary<int, Displacement>();

            {
                string FileName2 = string.Format("Case_03\\output\\NodalDisp.txt", Path.GetFullPath(Path.Combine(RunningPath, @"..\..\")));

                var lines = System.IO.File.ReadAllLines(FileName2);

                foreach (var ln in lines)
                {
                    var patt1 = @"^(\s+)(\S+)(\s+)(\d+)(\s+)(\S+)(\s+)(\S+)(\s+)(\S+)(\s+)(\S+)(\s+)(\S+)(\s+)(\S+)$";

                    var patt2 = @"^(\s+)(\S+)(\s+)(\d+)(\s+)(\S+)(\s+)(\S+)(\s+)(\S+)(\s+)(\S+)$";

                    var mtch1 = Regex.Match(ln, patt1);

                    var mtch2 = Regex.Match(ln, patt2);

                    if (mtch1.Success)
                    {
                        var id = int.Parse(mtch1.Groups[4].Value);
                        var x = double.Parse(mtch1.Groups[6].Value);
                        var y = double.Parse(mtch1.Groups[8].Value);
                        var z = double.Parse(mtch1.Groups[10].Value);

                        var loc = new Point(x, y, z);

                        abqlocDic[id] = loc;
                    }

                    if (mtch2.Success)
                    {
                        var id = int.Parse(mtch2.Groups[4].Value);
                        var dx = double.Parse(mtch2.Groups[8].Value);
                        var dy = double.Parse(mtch2.Groups[10].Value);
                        var dz = double.Parse(mtch2.Groups[12].Value);

                        var vec = new Vector(dx, dy, dz);

                        abqdispDic[id] = new Displacement(vec, Vector.Zero);
                    }
                }
            }

            //BFE results
            var bFEResults = new Dictionary<int, Displacement>();
            var errors = new List<double>();

            for (var i = 0; i < abaqusModel.Nodes.Count; i++)
            {
                //bFEResults.Add(i, abaqusModel.Nodes[i].GetNodalDisplacement());
                var bfed = abaqusModel.Nodes[i].GetNodalDisplacement();
                var abqd = abqdispDic[i + 1];// abaqusModel.Nodes[i].GetNodalDisplacement();

                var err = Util.GetAbsError(abqd.Displacements, bfed.Displacements);
                errors.Add(err);
            }
                
            var max = errors.Max();
            var avg = errors.Average();

            var maxDisp = abqdispDic.Max(i => i.Value.Displacements.Length);
            var avgDisp = abqdispDic.Average(i => i.Value.Displacements.Length);


            span.Add("paragraph").Text(string.Format("Validation output for nodal displacements:")).AddClosedTag("br");

            span.Add("paragraph").Text(string.Format("Maximum ABS Error: {0:g2}", max)).AddClosedTag("br");
            span.Add("paragraph").Text(string.Format("Average AVG Error: {0:g2}", avg)).AddClosedTag("br");

            span.Add("paragraph").Text(string.Format("Maximum displacement: {0:g2} m", maxDisp)).AddClosedTag("br");
            span.Add("paragraph").Text(string.Format("Average displacement: {0:g2} m", avgDisp)).AddClosedTag("br");

            return val;
        }
    }
}
