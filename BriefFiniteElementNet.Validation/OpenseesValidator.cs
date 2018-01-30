using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Validation.OpenseesTclGenerator;
using System.Diagnostics;
using System.Xml;
using System.Data;

namespace BriefFiniteElementNet.Validation
{
    public class OpenseesValidator
    {
        private static readonly string openSeeslocation = @"C:\temp\Opensees\Opensees.exe";


        public static void OpenseesValidate(Model model, LoadCase loadcase, bool validateStiffness = false)
        {
            var gen = new TclGenerator();

            gen.ElementTranslators.Add(new BarElement2Tcl() { TargetGenerator = gen});

            gen.ExportElementForces = true;
            gen.ExportNodalDisplacements = true;
            gen.ExportTotalStiffness = validateStiffness;

            var tcl = gen.Create(model, LoadCase.DefaultLoadCase);

            var nodesFile = gen.nodesOut;
            var elementsFile = gen.elementsOut;
            var stiffnessFile = gen.stiffnessOut;

            var tclFile = System.IO.Path.GetTempFileName() + ".tcl";

            System.IO.File.WriteAllText(tclFile, tcl);

            var stInf = new ProcessStartInfo(openSeeslocation);

            stInf.Arguments = tclFile;
            stInf.RedirectStandardOutput = true;
            stInf.UseShellExecute = false;

            var prc = Process.Start(stInf);

            prc.WaitForExit();
            var code = prc.ExitCode;

            var stiffness = stiffnessFile == null ? null : System.IO.File.ReadAllText(stiffnessFile);

            var ndisp = new XmlDocument();
            ndisp.Load(nodesFile);

            var elOut = new XmlDocument();
            elOut.Load(elementsFile);

            var dParts = ndisp.DocumentElement.LastChild.InnerText.Trim().Split('\n').Select(i => i.Trim()).Where(i => !string.IsNullOrEmpty(i)).ToArray();

            var elmParts = elOut.DocumentElement.LastChild.InnerText.Trim().Split('\n').Select(i => i.Trim()).Where(i => !string.IsNullOrEmpty(i)).ToArray();

            var openseesDs = dParts[0].Split(' ').Select(double.Parse).ToArray();

            var myDs = model.Nodes.SelectMany(i => Displacement.ToVector(i.GetNodalDisplacement(loadcase))).ToArray();

            var abss = new double[myDs.Length];
            var rels = new double[myDs.Length];

            var nums = Enumerable.Range(0, myDs.Length).ToArray();

            FindError(openseesDs, myDs, rels, abss);

            var k = abss;
            var key = new Func<double[]>(() => (double[])k.Clone());

            var tbl = new DataTable();

            tbl.Columns.Add("Node #", typeof(int));
            tbl.Columns.Add("DoF", typeof(string));
            tbl.Columns.Add("DoF #", typeof(int));
            tbl.Columns.Add("OpenSees Delta", typeof(double));
            tbl.Columns.Add("BFE Delta", typeof(double));
            tbl.Columns.Add("RelativeErr", typeof(double));
            tbl.Columns.Add("AbsErr", typeof(double));

            for (int i = 0; i < myDs.Length; i++)
            {
                tbl.Rows.Add(
                    i / 6,
                    ((DoF)(i % 6)).ToString(),
                    nums[i],
                    openseesDs[i],
                    myDs[i],
                    rels[i],
                    abss[i]
                    );
            }

            for (var i = 0; i < model.Nodes.Count; i++)
                model.Nodes[i].Label = i.ToString();
        }

        public static void FindError(double[] exact, double[] test, double[] relativeError, double[] absError)
        {
            if (exact.Length != test.Length)
            {
                throw new Exception();
            }

            var n = exact.Length;

            for (var i = 0; i < n; i++)
            {
                var absErr = Math.Abs(exact[i] - test[i]);

                var relErr = absErr / Math.Abs(exact[i]);

                if (exact[i] == 0)
                {
                    if (absErr == 0)
                        relErr = 0;
                    else
                        relErr = 1;
                }

                {
                    if (relativeError != null)
                        relativeError[i] = relErr;

                    if (absError != null)
                        absError[i] = absErr;
                }

            }
        }

        public static Matrix GetTotalStiffnessMatrix(Model model, LoadCase loadcase)
        {
            var gen = new TclGenerator();

            gen.ExportTotalStiffness = true;

            var tcl = gen.Create(model, LoadCase.DefaultLoadCase);



            var tclFile = System.IO.Path.GetTempFileName() + ".tcl";

            System.IO.File.WriteAllText(tclFile, tcl);



            var stInf = new ProcessStartInfo(openSeeslocation);

            stInf.Arguments = tclFile;
            stInf.RedirectStandardOutput = true;
            stInf.UseShellExecute = false;


            var prc = Process.Start(stInf);


            prc.WaitForExit();

            var openseesStfArr =
            System.IO.File.ReadAllLines(gen.stiffnessOut)
                .Select(i => i.Trim())
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Select(
                    i =>
                        i.Split(' ')
                            .Where(j => !string.IsNullOrWhiteSpace(j))
                            .Select(j => double.Parse(j))
                            .ToArray())
                .ToArray();


            return new Matrix(openseesStfArr);
        }

    }
}
