using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Solver;

namespace BriefFiniteElementNet.Validation
{
    /// <summary>
    /// Validates result from this library with result from Frame3DD application
    /// </summary>
    public class Frame3DDValidator
    {
        public Frame3DDValidator(Model model)
        {
            this.model = model;
            this.ndes = model.Nodes.ToArray();
            this.elements = model.Elements.Where(i => i is FrameElement2Node).Cast<FrameElement2Node>().ToArray();
        }

        private Model model;
        private Node[] ndes;
        private FrameElement2Node[] elements;
        public string Frame3ddOutputMessage;
        public string Frame3ddErrorMessage;
        public string ValidationResult;

        public Model Model
        {
            get { return model; }
            set { model = value; }
        }

        public void Validate()
        {
            var code = Createf3DdInputFile();

            var input = System.IO.Path.GetTempFileName();
            var output = System.IO.Path.GetTempFileName();

            System.IO.File.WriteAllText(input, code);

            var start = new System.Diagnostics.ProcessStartInfo();
            start.Arguments = string.Format(@"""{0}"" ""{1}"" ", input, output);
            start.FileName = "frame3dd.exe";
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.UseShellExecute = false;


            if (!System.IO.File.Exists(start.FileName))
            {
                Console.WriteLine(@"frame3dd.exe not found, please download it from 
http://master.dl.sourceforge.net/project/frame3dd/frame3dd/0.20091203/Frame3DD_20091203_win32.zip
and place frame3dd.exe beside this application.");
                return;

            }


            using (var process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    Frame3ddOutputMessage = reader.ReadToEnd();
                }

                using (StreamReader reader = process.StandardError)
                {
                    Frame3ddErrorMessage = reader.ReadToEnd();
                }
            }

            if (!string.IsNullOrEmpty(Frame3ddErrorMessage))
            {
                Console.WriteLine("Error message from Frame3dd.exe stderr: ");
                Console.WriteLine(Frame3ddErrorMessage);
            }

            var nodeLoadCases = model.Nodes.SelectMany(i => i.Loads).Select(i => i.Case).Distinct().ToList();
            var elementLoadCases = model.Elements.SelectMany(i => i.Loads).Select(i => i.Case).Distinct().ToList();

            var allCases = nodeLoadCases;
            allCases.AddRange(elementLoadCases);

            var cmb = new LoadCombination();
            allCases.ForEach(i => cmb[i] = 1.0);

            //Process.Start(@"C:\Program Files (x86)\Notepad++\notepad++.exe", output);
            var cfg = new SolverConfiguration();
            cfg.LoadCases = allCases;
            //cfg.SolverGenerator = i => new CholeskySolver(i);
            cfg.SolverFactory = new CholeskySolverFactory();
            model.Solve(cfg);

            var disps = model.Nodes.Select(i => i.GetNodalDisplacement(cmb)).ToArray();

            var f3ds = ReadF3dDisplacementVector(output);
            var myds = ReadMyDisplacementVector(cmb);

            var d = GetAbsDifference(f3ds, myds);
            //var dm = GetRelativeDifference(f3ds, myds).Max();


            Console.WriteLine("");
            Console.WriteLine("========== Absolute error in result from BFE against results from FRAME3DD application:");

            var minError = d.Min(i => Math.Abs(i));
            var avgnError = d.Average(i => Math.Abs(i));
            var maxError = d.Max(i => Math.Abs(i));


            Console.WriteLine("");
            Console.WriteLine("--ABSOLUTE ERRORS ");
            Console.WriteLine("Min : {0} ({1:E})", minError, minError);
            Console.WriteLine("Average : {0} ({1:E})", avgnError, avgnError);
            Console.WriteLine("Max : {0} ({1:E})", maxError, maxError);

            var err =
                Enumerable.Range(0, f3ds.Length).Select(i => d[i]*Math.Max(Math.Abs(f3ds[i]), Math.Abs(myds[i]))).Sum()
                /Enumerable.Range(0, f3ds.Length).Select(i => Math.Max(Math.Abs(f3ds[i]), Math.Abs(myds[i]))).Sum();

            Console.WriteLine("");
            Console.WriteLine("--RELATIVE ERRORS ");
            Console.WriteLine("Average (regarding weight of values) : {0:0.00}% ({1:E})", err*100, err);
        }

        private static double[] GetAbsDifference(double[] d1, double[] d2)
        {
            if (d1.Length != d2.Length)
                throw new Exception();

            var buf = new double[d1.Length];

            for (var i = 0; i < d1.Length; i++)
            {
                var v1 = d1[i];
                var v2 = d2[i];

                buf[i] = Math.Abs(v2 - v1);
            }

            return buf;
        }

        #region frame3dd input file stuff

        /// <summary>
        /// Gets the index of specified <see cref="node"/>.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private int Index(Node node)
        {
            return ndes.IndexOfReference(node);
        }

        private int Index(FrameElement2Node elm)
        {
            return elements.IndexOfReference(elm);
        }

        public string Createf3DdInputFile()
        {
            if (model.Elements.Any(i => !(i is FrameElement2Node)))
                throw new Exception("Only frame element supported by f3dd");

            if (
                model.Elements.Cast<FrameElement2Node>()
                    .Any(i => i.Loads.Any(j => !(j is UniformLoad1D) && !(j is ConcentratedLoad1D))))
                throw new Exception("Only uniform load supported by f3dd");

            var sb = new StringBuilder();

            sb.AppendLine("# Generated by BriefFiniteElementNet.Validation, Units: M, Kg, N");
            sb.AppendLine();

            sb.AppendLine("# node data ...");
            WriteNodeData(sb);
            sb.AppendLine();

            sb.AppendLine("# constrained node data ...");
            WriteConstraintedNodeData(sb);
            sb.AppendLine();

            sb.AppendLine("# element data ...");
            WriteElementData(sb);
            sb.AppendLine();

            sb.AppendLine("# analysis options ...");
            WriteAnalysisOptions(sb);
            sb.AppendLine();

            /*var distinctLoadCases = GetDistinctLoadCases();

            foreach (var lcase in distinctLoadCases)
            {
                WriteLoadCaseData(sb, lcase);
            }*/

            sb.AppendLine("1\t# number of static load cases");
            sb.AppendLine("# gravitational acceleration for self-weight loading (global)");
            sb.AppendLine("#.gX\tgY\tgZ");
            sb.AppendLine("#.mm/s^2	mm/s^2\tmm/s^2");
            sb.AppendLine("0\t0\t0");




            sb.AppendLine("# node load data ...");
            WriteNodesWithAllLoads(sb);

            sb.AppendLine("# element load data ...");
            WriteElementsWithAllUniformLoads(sb);

            sb.AppendLine("0\t# number of trapezoidally-distributed element loads (local)");
            //sb.AppendLine("0\t# number of concentrated interior point loads (local)");
            WriteElementsWithAllConcentratedLoads(sb);

            sb.AppendLine("0\t# number of frame elements with temperature changes (local)");
            sb.AppendLine("0\t# number of desired dynamic modes ");
            sb.AppendLine("0\t# 1= Subspace-Jacobi iteration, 2= Stodola (matrix iteration) method");
            sb.AppendLine("0\t# 0= consistent mass matrix, 1= lumped mass matrix");
            sb.AppendLine("0\t# frequency convergence tolerance  approx 1e-4");
            sb.AppendLine("0\t# frequency shift-factor for rigid body modes, make 0 for pos.def. [K]");
            sb.AppendLine("0\t# exaggerate modal mesh deformations");

            sb.AppendLine("0\t# number of nodes with extra node mass or rotatory inertia");
            sb.AppendLine("0\t# number of frame elements with extra mass ");
            sb.AppendLine("0\t# number of modes to be animated, nA ");
            sb.AppendLine("\t# list of modes to animate - omit if nA == 0");
            sb.AppendLine("0\t# pan rate during animation");


            return sb.ToString();
        }

        private void WriteNodeData(StringBuilder sb)
        {
            sb.AppendLine("{0}\t# number of nodes ", ndes.Length);
            sb.AppendLine("#.node\tx\ty\tz\tr");
            sb.AppendLine("#\t\tm\tm\tm\tm");


            for (var i = 0; i < ndes.Length; i++)
            {
                var loc = ndes[i].Location;

                sb.AppendLineArray(
                    (i + 1).Union(loc.X, loc.Y, loc.Z, 0.0)
                    );
            }
        }

        private void WriteConstraintedNodeData(StringBuilder sb)
        {
            var notReleaseNodes = ndes.Where(i => i.Constraints != Constraint.Released).ToArray();

            sb.AppendLine();
            sb.AppendLine(string.Format("{0}\t# number of nodes with reactions", notReleaseNodes.Length));
            sb.AppendLine("#.n\tx\ty\tz\txx\tyy\tzz\t1=fixed, 0=free");

            foreach (var nde in notReleaseNodes)
            {
                var ctx = nde.Constraints;

                sb.AppendLineArray(
                    (Index(nde) + 1).Union((int) ctx.DX, (int) ctx.DY, (int) ctx.DZ, (int) ctx.RX, (int) ctx.RY,
                        (int) ctx.RZ)
                    );
            }
        }

        private void WriteElementData(StringBuilder sb)
        {
            sb.AppendLine(string.Format("{0}\t# number of frame elements", model.Elements.Count));
            sb.AppendLine("#.e	n1	n2	Ax	Asy	Asz	Jxx	Iyy	Izz	E	G	roll	density");
            sb.AppendLine("#	.	.	m^2	m^2	m^2	m^4	m^4	m^4	MPa	MPa	deg	T/mm^3");

            var cnt = 0;
            foreach (var elm in model.Elements)
            {
                if (!(elm is FrameElement2Node))
                    throw new NotSupportedException();

                var fr = elm as FrameElement2Node;

                sb.AppendFormat("{0}\t", 1 + cnt++);
                sb.AppendFormat("{0}\t", 1 + Index(fr.StartNode));
                sb.AppendFormat("{0}\t", 1 + Index(fr.EndNode));

                sb.AppendFormat("{0}\t", fr.A);
                sb.AppendFormat("{0}\t", fr.Ay);
                sb.AppendFormat("{0}\t", fr.Az);
                sb.AppendFormat("{0}\t", fr.J);
                sb.AppendFormat("{0}\t", fr.Iy);
                sb.AppendFormat("{0}\t", fr.Iz);
                sb.AppendFormat("{0}\t", fr.E);
                sb.AppendFormat("{0}\t", fr.G);
                sb.AppendFormat("{0}\t", fr.WebRotation);
                sb.AppendFormat("{0}", fr.MassDensity);

                sb.AppendLine();
            }

        }

        private void WriteAnalysisOptions(StringBuilder sb)
        {
            sb.AppendLine("0\t# 1: include shear deformation");
            sb.AppendLine("0\t# 1: include geometric stiffness");
            sb.AppendLine("50.0\t# exaggerate mesh deformations");
            sb.AppendLine("1.0\t# zoom scale for 3D plotting");
            sb.AppendLine("-1.0\t# x-axis increment for internal forces");
        }

        private void WriteNodesWithAllLoads(StringBuilder sb)
        {
            var nodesWithThisCase = ndes.Where(i => i.Loads.Any()).ToList();

            sb.AppendLine("{0}\t# number of loaded nodes", nodesWithThisCase.Count);
            sb.AppendLine("#.n	Fx	Fy	Fz	Mxx	Myy	Mzz");
            sb.AppendLine("#.	N	N	N	N.m	N.m	N.m");

            foreach (var node in nodesWithThisCase)
            {
                var force = node.Loads.Select(i => i.Force).Aggregate((a, b) => a + b);
                sb.AppendLineArray(Index(node) + 1, force.Fx, force.Fy, force.Fz, force.Mx, force.My, force.Mz);
            }

            sb.AppendLine();
        }

        private void WriteElementsWithAllUniformLoads(StringBuilder sb)
        {
            var elementsWithThisLoad =
                elements.Where(
                    i => i.Loads.Where(j => j is UniformLoad1D).Cast<UniformLoad1D>().Any())
                    .ToList();

            sb.AppendLine("{0}\t# number of uniformly-distributed element loads (local)", elementsWithThisLoad.Count);
            sb.AppendLine("#.elmnt\tX-load\tY-load\tZ-load\tuniform member loads in member coordinates");
            sb.AppendLine("#\tN/m\tN/m\tN/m");

            foreach (var elm in elementsWithThisLoad)
            {
                if (elm.Loads.Where(i => i is UniformLoad1D)
                        .Cast<UniformLoad1D>()
                        .Any(i => i.CoordinationSystem != CoordinationSystem.Local))
                    throw new Exception();


                var lds = elm.Loads.Where(i => i is UniformLoad1D).Cast<UniformLoad1D>().ToList();

                var ux = lds.Where(i => i.Direction == LoadDirection.X).Sum(i => i.Magnitude);
                var uy = lds.Where(i => i.Direction == LoadDirection.Y).Sum(i => i.Magnitude);
                var uz = lds.Where(i => i.Direction == LoadDirection.Z).Sum(i => i.Magnitude);

                /*foreach (var lde in elm.Loads)
                {
                    if (!(lde is UniformLoad1D))
                        continue;

                    var l = lde as UniformLoad1D;

                    if (l.CoordinationSystem == CoordinationSystem.Global)
                        throw new NotSupportedException("Global coordination system in UniformLoad1D is not supported");

                    switch (l.Direction)
                    {
                        case LoadDirection.X:
                            ux += l.Magnitude;
                            break;
                        case LoadDirection.Y:
                            uy += l.Magnitude;
                            break;
                        case LoadDirection.Z:
                            uz += l.Magnitude;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }*/


                sb.AppendLineArray(Index(elm) + 1, ux, uy, uz);
            }

            sb.AppendLine();
        }

        private void WriteElementsWithAllConcentratedLoads(StringBuilder sb)
        {
            var allLoads = elements.Select(j => j.Loads.Count(k => k is ConcentratedLoad1D)).Sum();

            sb.AppendLine("{0}\t# number of concentrated interior point loads (local)", allLoads);

            sb.AppendLine("#.elmnt\tX-load\tY-load\tZ-load\tx-loc'n\tpoint loads in member coordinates ");
            sb.AppendLine("#\tN\tN\tN\tm");

            foreach (var elm in model.Elements)
            {
                var felm = elm as FrameElement2Node;

                if (felm == null)
                    continue;

                foreach (var ld in felm.Loads)
                {
                    var cld = ld as ConcentratedLoad1D;

                    if (cld == null)
                        continue;

                    if (cld.Force.Moments != Vector.Zero)
                        throw new Exception();

                    sb.AppendLine("{0}\t{1}\t{2}\t{3}\t{4}", model.Elements.IndexOfReference(elm) + 1, cld.Force.Fx,
                        cld.Force.Fy, cld.Force.Fz, cld.DistanseFromStartNode);
                }
            }

            sb.AppendLine();
        }
        /*
        private List<LoadCase> GetDistinctLoadCases()
        {
           var allLoadCases =
                model.Nodes.SelectMany(i => i.Loads).Select(i => i.Case).Distinct().ToArray()
                    .Concat(model.Elements.SelectMany(i => i.Loads).Select(i => i.Case).Distinct().ToArray())
                    .Distinct()
                    .Concat(new LoadCase[] { model.SettlementLoadCase }).Cast<LoadCase>().ToList();

            //throw new NotImplementedException();
            return allLoadCases;
        }

        private void WriteLoadCasesData(StringBuilder sb, LoadCase[] lcs)
        {
            sb.AppendLine("{0}\t# number of static load cases", lcs.Length);

            for (var i = 0; i < lcs.Length; i++)
            {
                sb.AppendLine("# Start Static Load Case {0} of {1} (CaseName: {2}, Type: {3})", i + 1, lcs.Length,
                    lcs[i].CaseName, lcs[i].LoadType);
                WriteLoadCaseData(sb, lcs[i]);
                sb.AppendLine("# End Static Load Case {0} of {1} (CaseName: {2}, Type: {3})", i + 1, lcs.Length,
                    lcs[i].CaseName, lcs[i].LoadType);
            }
        }

        private void WriteLoadCaseData(StringBuilder sb, LoadCase lcase)
        {
            sb.AppendLine(@"# gravitational acceleration for self-weight loading (global)");
            sb.AppendLine(@"#.gX	gY	gZ");
            sb.AppendLine(@"#.m./s^2	m./s^2	m./s^2");
            sb.AppendLineArray(0.0.Union(0.0, 10));
            sb.AppendLine();

            WriteLoadedNodes(sb, lcase);
            WriteUniformLoadedElements(sb, lcase);
            sb.AppendLine();
            WriteTrapezoidLoadedElements(sb, lcase);
            sb.AppendLine();
            WriteConcentratedLoadedElements(sb, lcase);
            sb.AppendLine();
            WriteTermalLoadedElements(sb, lcase);
            sb.AppendLine();
            WriteTermalLoadedElements(sb, lcase);
            sb.AppendLine();
            WriteSettlementedNodes(sb, lcase);
            sb.AppendLine();
        }

        private void WriteLoadedNodes(StringBuilder sb, LoadCase lcase)
        {
            var nodesWithThisCase = ndes.Where(i => i.Loads.Any(j => j.Case == lcase)).ToList();
            sb.AppendLine("{0}\t# number of loaded nodes", nodesWithThisCase.Count);
            sb.AppendLine("#.n	Fx	Fy	Fz	Mxx	Myy	Mzz");
            sb.AppendLine("#.	N	N	N	N.m	N.m	N.m");

            foreach (var node in nodesWithThisCase)
            {
                var force = node.Loads.Where(i => i.Case == lcase).Select(i => i.Force).Aggregate((a, b) => a + b);
                sb.AppendLineArray(Index(node) + 1, force.Fx, force.Fy, force.Fz, force.Mx, force.My, force.Mz);
            }

            sb.AppendLine();
        }


        private void WriteUniformLoadedElements(StringBuilder sb, LoadCase lcase)
        {
            var elementsWithThisLoad =
                elements.Where(
                    i => i.Loads.Where(j => j is UniformLoad1D).Cast<UniformLoad1D>().Any(j => j.Case == lcase))
                    .ToList();

            sb.AppendLine("{0}\t# number of uniformly-distributed element loads (local)", elementsWithThisLoad.Count);
            sb.AppendLine("#.elmnt\tX-load\tY-load\tZ-load\tuniform member loads in member coordinates");
            sb.AppendLine("#\tN/m\tN/m\tN/m");

            foreach (var elm in elementsWithThisLoad)
            {
                var ux = 0.0;
                var uy = 0.0;
                var uz = 0.0;

                foreach (var lde in elm.Loads)
                {
                    if (!(lde is UniformLoad1D))
                        continue;

                    var l = lde as UniformLoad1D;

                    if (l.CoordinationSystem == CoordinationSystem.Global)
                        throw new NotSupportedException("Global coordination system in UniformLoad1D is not supported");

                    switch (l.Direction)
                    {
                        case LoadDirection.X:
                            ux += l.Magnitude;
                            break;
                        case LoadDirection.Y:
                            uy += l.Magnitude;
                            break;
                        case LoadDirection.Z:
                            uz += l.Magnitude;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }


                sb.AppendLineArray(Index(elm) + 1, ux, uy, uz);
            }

            sb.AppendLine();
        }

        private void WriteTrapezoidLoadedElements(StringBuilder sb, LoadCase lcase)
        {
            sb.AppendLine("{0}\t# number of uniformly-distributed element loads (local)", 0);
        }

        private void WriteConcentratedLoadedElements(StringBuilder sb, LoadCase lcase)
        {
            //sb.AppendLine("{0}\t# number of uniformly-distributed element loads (local)", 0);
            //return;
            var elementsWithThisLoad =
                elements.Where(
                    i =>
                        i.Loads.Where(j => j is ConcentratedLoad1D).Cast<ConcentratedLoad1D>().Any(j => j.Case == lcase))
                    .ToList();

            var countOfAllConcentratedLoads = elements.SelectMany(i => i.Loads)
                .Where(i => i.Case == lcase)
                .Where(i => i is ConcentratedLoad1D)
                .Cast<ConcentratedLoad1D>()
                .ToList();


            sb.AppendLine("{0}\t# number of concentrated interior point loads (local)", countOfAllConcentratedLoads.Count);
            sb.AppendLine("#.elmnt\tX-load\tY-load\tZ-load\tx-loc'n  point loads in member coordinates ");
            sb.AppendLine("#\tN\tN\tN\tm");

            foreach (var elm in elementsWithThisLoad)
            {
                var concentratedForces =
                    elm.Loads.Where(i => i.Case == lcase)
                        .Where(i => i is ConcentratedLoad1D)
                        .Cast<ConcentratedLoad1D>()
                        .ToList();

                foreach (var lde in concentratedForces)
                {
                    if (lde.CoordinationSystem == CoordinationSystem.Global)
                        throw new NotSupportedException(
                            "Global coordination system in ConcentratedLoad1D is not supported");

                    var force = lde.Force;

                    if (!force.Moments.Equals(Vector.Zero))
                        throw new NotSupportedException("Moments in ConcentratedLoad1D is not supported");

                    sb.AppendLineArray(Index(elm) + 1, force.Fx, force.Fy, force.Fz);
                }
            }

            sb.AppendLine();
        }

        private void WriteTermalLoadedElements(StringBuilder sb, LoadCase lcase)
        {
            sb.AppendLine("{0}\t# number of frame elements with temperature changes (local)", 0);
        }
        /**/

        private void WriteSettlementedNodes(StringBuilder sb, LoadCase lc)
        {
            var settledNodes = new List<Node>();

            if (model.SettlementLoadCase == lc)
            {
                settledNodes = ndes.Where(i => !i.Settlements.Equals(Displacement.Zero)).ToList();
            }

            sb.AppendLine("{0}\t# number of prescribed displacements nD<=nR (global)", settledNodes.Count);
            sb.AppendLine("#.node\tX-displ\tY-displ\tZ-displ\tX-rot'n\tY-rot'n\tZ-rot'n");
            sb.AppendLine("#\tmm\tmm\tmm\tradian\tradian\tradian");


            foreach (var nde in settledNodes)
            {
                sb.AppendLineArray(nde.Settlements.DX, nde.Settlements.DY, nde.Settlements.DZ, nde.Settlements.RX,
                    nde.Settlements.RY, nde.Settlements.RZ);
            }


        }

        #endregion

        private double[] ReadF3dDisplacementVector(string f3dOutData)
        {
            var t = new F3ddFileReader(f3dOutData).GetNodalDisplacements();

            var ds = new double[6*model.Nodes.Count];

            for (var i = 0; i < model.Nodes.Count; i++)
            {
                if (t.ContainsKey(i))
                {
                    var d = t[i];

                    ds[6 * i + 0] = d.DX;
                    ds[6 * i + 1] = d.DY;
                    ds[6 * i + 2] = d.DZ;

                    ds[6 * i + 3] = d.RX;
                    ds[6 * i + 4] = d.RY;
                    ds[6 * i + 5] = d.RZ;
                }
            }

            return ds;
        }

        private double[] ReadMyDisplacementVector(LoadCombination allCmb)
        {
            var ds = new double[6 * model.Nodes.Count];

            for (var i = 0; i < model.Nodes.Count; i++)
            {
                    var d = model.Nodes[i].GetNodalDisplacement(allCmb);

                    ds[6 * i + 0] = d.DX;
                    ds[6 * i + 1] = d.DY;
                    ds[6 * i + 2] = d.DZ;

                    ds[6 * i + 3] = d.RX;
                    ds[6 * i + 4] = d.RY;
                    ds[6 * i + 5] = d.RZ;
            }

            return ds;
        }


    }
}