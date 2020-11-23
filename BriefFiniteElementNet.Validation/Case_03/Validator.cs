using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
            string FileName = string.Format("{0}Resources\\Job-10.inp", Path.GetFullPath(Path.Combine(RunningPath, @"..\..\")));
            var abaqusModel = AbaqusInputFileReader.AbaqusInputToBFE(FileName);
            //comparison
            var val = new ValidationResult();
            var span = val.Span = new HtmlTag("span");
            val.Title = "Clamped beam with tetrahedron elements";

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

            //List of result displacements from Abaqus
            Dictionary<int, Displacement> abaqusResults = new Dictionary<int, Displacement>();
            abaqusResults.Add(67, new Displacement(-795.863E-09, -288.503E-06, 131.482E-09));
            abaqusResults.Add(169, new Displacement(-434.254E-09, -246.688E-06, -431.229E-09));
            abaqusResults.Add(172, new Displacement(-549.082E-09, -204.906E-06, -491.084E-09));
            abaqusResults.Add(175, new Displacement(-622.334E-09, -164.843E-06, -380.641E-09));
            abaqusResults.Add(178, new Displacement(-602.786E-09, -127.49E-06, -333.347E-09));
            abaqusResults.Add(181, new Displacement(-655.981E-09, -93.343E-06, -441.491E-09));
            abaqusResults.Add(184, new Displacement(-899.89E-09, -63.0811E-06, -65.9539E-09));
            abaqusResults.Add(187, new Displacement(-244.578E-09, -37.7898E-06, -108.451E-09));
            abaqusResults.Add(190, new Displacement(85.1779E-09, -17.7659E-06, -252.828E-09));
            abaqusResults.Add(193, new Displacement(-518.751E-09, -4.53149E-06, -291.286E-09));
            abaqusResults.Add(49, new Displacement(-12.2049E-33, 108.099E-33, 147.403E-33));

            //BFE results
            Dictionary<int, Displacement> bFEResults = new Dictionary<int, Displacement>();
            bFEResults.Add(67, abaqusModel.Nodes[67].GetNodalDisplacement());
            bFEResults.Add(169, abaqusModel.Nodes[169].GetNodalDisplacement());
            bFEResults.Add(172, abaqusModel.Nodes[172].GetNodalDisplacement());
            bFEResults.Add(175, abaqusModel.Nodes[175].GetNodalDisplacement());
            bFEResults.Add(178, abaqusModel.Nodes[178].GetNodalDisplacement());
            bFEResults.Add(181, abaqusModel.Nodes[181].GetNodalDisplacement());
            bFEResults.Add(184, abaqusModel.Nodes[184].GetNodalDisplacement());
            bFEResults.Add(187, abaqusModel.Nodes[187].GetNodalDisplacement());
            bFEResults.Add(190, abaqusModel.Nodes[190].GetNodalDisplacement());
            bFEResults.Add(193, abaqusModel.Nodes[193].GetNodalDisplacement());
            bFEResults.Add(49, abaqusModel.Nodes[49].GetNodalDisplacement());

            //Errors
            List<double> errors = new List<double>();
            foreach (var item in abaqusResults)
            {
                errors.Add(Util.GetErrorPercent(bFEResults[item.Key], item.Value));
            }
            var max = errors.Max();
            var avg = errors.Average();

            span.Add("paragraph").Text(string.Format("Validation output for nodal displacements:(36 nodes)")).AddClosedTag("br");

            span.Add("paragraph").Text(string.Format("Maximum Error: {0:g2}%", max)).AddClosedTag("br");
            span.Add("paragraph").Text(string.Format("Average Error: {0:g2}%", avg)).AddClosedTag("br");
           
            return val;
        }
    }
}
