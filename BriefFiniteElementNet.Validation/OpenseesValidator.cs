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
    public static class OpenseesValidator
    {
        private static readonly string openSeeslocation = @"C:\Opensees\Opensees.exe";


        public static DataTable[] OpenseesValidate(Model model, LoadCase loadcase, bool validateStiffness = false)
        {
            var gen = new TclGenerator();

            gen.ElementTranslators.Add(new BarElement2Tcl() { TargetGenerator = gen });
            gen.ElementTranslators.Add(new TetrahedronToTcl() { TargetGenerator = gen });
            gen.ElementLoadTranslators.Add(new UniformLoad2Tcl() { TargetGenerator = gen });

            gen.ExportElementForces = false;
            gen.ExportNodalDisplacements = true;
            gen.ExportTotalStiffness = validateStiffness;
            gen.ExportNodalReactions = true;

            var tcl = gen.Create(model, LoadCase.DefaultLoadCase);

            var nodesFile = gen.nodesOut;
            var elementsFile = gen.elementsOut;
            var stiffnessFile = gen.stiffnessOut;

            var tclFile = System.IO.Path.GetTempFileName() + ".tcl";


            Debug.WriteLine("{0}", tclFile);

            System.IO.File.WriteAllText(tclFile, tcl);

                if (!System.IO.File.Exists(openSeeslocation))
                throw new Exception("Opensees.exe not found, please put opensees.exe into 'C:\\Opensees\\Opensees.exe'");

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

            var nreact = new XmlDocument();
            nreact.Load(gen.reactionsOut);


            if (gen.ExportElementForces)
            {
                var elOut = new XmlDocument();
                elOut.Load(elementsFile);

                var elmParts = elOut.DocumentElement.LastChild.InnerText.Trim().Split('\n').Select(i => i.Trim()).Where(i => !string.IsNullOrEmpty(i)).ToArray();

            }


            var nodalDispParts = ndisp.DocumentElement.LastChild.InnerText.Trim().Split('\n').Select(i => i.Trim()).Where(i => !string.IsNullOrEmpty(i)).ToArray();
            var nodalReactParts = nreact.DocumentElement.LastChild.InnerText.Trim().Split('\n').Select(i => i.Trim()).Where(i => !string.IsNullOrEmpty(i)).ToArray();

           
            var openseesDs = nodalDispParts[0].Split(' ').Select(double.Parse).ToArray();
            var openseesReacts = nodalReactParts[0].Split(' ').Select(double.Parse).ToArray();

            var myDs = model.Nodes.SelectMany(i => Displacement.ToVector(i.GetNodalDisplacement(loadcase))).ToArray();

            var myReacts = model.Nodes.SelectMany(i => Force.ToVector(i.GetSupportReaction(loadcase))).ToArray();

            var absNodalDisp = new double[myDs.Length];
            var relNodalDisp = new double[myDs.Length];

            var absNodalReac = new double[myDs.Length];
            var relNodalReac = new double[myDs.Length];

            var nums = Enumerable.Range(0, myDs.Length).ToArray();

            FindError(openseesDs, myDs, relNodalDisp, absNodalDisp);
            FindError(openseesReacts, myReacts, relNodalReac, absNodalReac);

            //var k = absNodalDisp;
            //var key = new Func<double[]>(() => (double[])absNodalDisp.Clone());

            var nodalDispTbl = new DataTable();
            var nodalReactTbl = new DataTable();
            var elmFrcTbl = new DataTable();

            {
                nodalDispTbl.Columns.Add("Node #", typeof(int));
                nodalDispTbl.Columns.Add("DoF", typeof(string));
                nodalDispTbl.Columns.Add("DoF #", typeof(int));
                nodalDispTbl.Columns.Add("OpenSees Delta", typeof(double));
                nodalDispTbl.Columns.Add("BFE Delta", typeof(double));
                nodalDispTbl.Columns.Add("Relative Error", typeof(double));
                nodalDispTbl.Columns.Add("Absolute Error", typeof(double));
            }


            {
                nodalReactTbl.Columns.Add("Node #", typeof(int));
                nodalReactTbl.Columns.Add("DoF", typeof(string));
                nodalReactTbl.Columns.Add("DoF #", typeof(int));
                nodalReactTbl.Columns.Add("OpenSees Support Reaction", typeof(double));
                nodalReactTbl.Columns.Add("BFE Support Reaction", typeof(double));
                nodalReactTbl.Columns.Add("Relative Error", typeof(double));
                nodalReactTbl.Columns.Add("Absolute Error", typeof(double));
            }

            {
                elmFrcTbl.Columns.Add("Elm #", typeof(int));
                elmFrcTbl.Columns.Add("Relative Error", typeof(double));
                elmFrcTbl.Columns.Add("Absolute Error", typeof(double));
            }

            for (int i = 0; i < myDs.Length; i++)
            {
                nodalDispTbl.Rows.Add(
                    i / 6,
                    ((DoF)(i % 6)).ToString(),
                    nums[i],
                    openseesDs[i],
                    myDs[i],
                    relNodalDisp[i],
                    absNodalDisp[i]
                    );

                nodalReactTbl.Rows.Add(
                    i / 6,
                    ((DoF)(i % 6)).ToString().Replace("D", "F").Replace("R", "M"),
                    nums[i],
                    openseesReacts[i],
                    myReacts[i],
                    relNodalReac[i],
                    absNodalReac[i]
                    );
            }

            /*
            for (int i = 0; i < model.Elements.Count; i++)
            {
                var bar = model.Elements[i] as BarElement;

                var myFrc = bar.GetInternalForceAt(0, LoadCase.DefaultLoadCase);

                nodalDispTbl.Rows.Add(
                    i ,
                    ((DoF)(i % 6)).ToString(),
                    nums[i],
                    openseesDs[i],
                    myDs[i],
                    relNodalDisp[i],
                    absNodalDisp[i]
                    );
                
            }
            */

            return new[] { nodalDispTbl, nodalReactTbl };
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
