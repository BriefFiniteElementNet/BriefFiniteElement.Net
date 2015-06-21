using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using BriefFiniteElementNet.CSparse.Double;
using BriefFiniteElementNet.Solver;

namespace BriefFiniteElementNet.BenchmarkApplication
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Note: Before executing this benchmark set the Configuration to Release and use 'DEBUG>Start Without Debugging' or 'Ctrl+F5' to get the highest possible performance


            Log("###############################################################################");
            Log(
                "**Performance benchmark for BriefFiniteElement.NET library \navailable via http://brieffiniteelmentnet.codeplex.com");
            Log("Benchmark 1: Uniform 3D Grid");

            Log("-------------------------------------------------------------------------------");
            Log("Environment Info:");

            bool isDebug;
#if DEBUG
            isDebug = true;
#else
            isDebug = false;
#endif

            Log("\tOS: {0}", Environment.OSVersion);
            //Log("\tIs OS 64bit: {0}", Environment.Is64BitOperatingSystem);
            Log("\tDebugger is Attached: {0}", Debugger.IsAttached);
            Log("\tMode: {0}", isDebug ? "Debug" : "Release");
            Log("-------------------------------------------------------------------------------");
            Log("System Info:");
            var sysInfo = GetSystemInfo();
            Log("\tCPU Model: {0}", sysInfo[0]);
            Log("\tCPU Clock Speed: {0}", sysInfo[1]);
            Log("\tTotal RAM: {0:0.00} GB", double.Parse(sysInfo[2])/(1024.0*1024.0));

            Log("###############################################################################");
            Log("");
            Log("Benchmark Info:");

            var solvers = Enum.GetValues(typeof (BuiltInSolverType));

            var sw = System.Diagnostics.Stopwatch.StartNew();

            var nums = new int[] {10, 11, 12, 13, 14, 15};

            var cnt = 0;

            var case1 = new LoadCase("c1", LoadType.Other);
            var case2 = new LoadCase("c2", LoadType.Other);


            foreach (var nm in nums)
            {
                var paramerts = new string[]
                {
                    String.Format("grid size: {0}x{0}x{0}", nm),
                    String.Format("{0} elements", 3*nm*nm*(nm - 1)),
                    String.Format("{0} nodes", nm*nm*nm),
                    String.Format("{0} free DoFs", 6*nm*nm*(nm - 1))
                };

                Log("Try # {0}", cnt++);
                Log(string.Join(", ", paramerts));


                foreach (BuiltInSolverType solverType in solvers)
                {
                    var st = StructureGenerator.Generate3DGrid(nm, nm, nm);


                    foreach (var nde in st.Nodes)
                    {
                        nde.Loads.Add(new NodalLoad(GetRandomForce(), case1));
                        nde.Loads.Add(new NodalLoad(GetRandomForce(), case2));
                    }

                    var type = solverType;

                    var conf = new SolverConfiguration()
                    {
                        SolverGenerator = i => CreateInternalSolver(type, i)
                    };

                    GC.Collect();

                    Log("");
                    Log("\tSolver type: {0}", GetEnumDescription(solverType));


                    try
                    {
                        st.Solve(conf);

                        sw.Restart();
                        st.LastResult.AddAnalysisResultIfNotExists(case1);
                        sw.Stop();

                        Log("\t\tgeneral solve time: {0}", sw.Elapsed);

                        sw.Restart();
                        st.LastResult.AddAnalysisResultIfNotExists(case2);
                        sw.Stop();

                        Log("\t\textra solve time per LoadCase: {0}", sw.Elapsed);
                    }
                    catch (Exception ex)
                    {
                        Log("\t\tFailed, err = {0}", ex.Message);
                    }

                    sw.Stop();

                    GC.Collect();
                }
            }

            Console.WriteLine();
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("Done, Write result to file?[type 'Y' for yes, anything else for no]");
            var inf = Console.ReadKey();

            if (inf.KeyChar == 'y' || inf.KeyChar == 'Y')
            {
                var fileName = "BriefFemNet benchmark.txt";
                System.IO.File.WriteAllText(fileName, sb.ToString());
                System.Diagnostics.Process.Start(fileName);
            }

            Environment.Exit(0);
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[]) fi.GetCustomAttributes(
                    typeof (DescriptionAttribute),
                    false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        private static ISolver CreateInternalSolver(BuiltInSolverType type, CompressedColumnStorage ccs)
        {
            switch (type)
            {
                case BuiltInSolverType.CholeskyDecomposition:
                    return new CholeskySolver(ccs);
                    break;
                case BuiltInSolverType.ConjugateGradient:
                    return new PCG(new SSOR()) {A = ccs};
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        /// <summary>
        /// Gets the count of free DoFs of <see cref="model"/>.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The count of free DoFs of model</returns>
        private static int GetFreeDofs(Model model)
        {
            var buf = 0;

            var n = model.Nodes.Count;

            for (int i = 0; i < n; i++)
            {
                var cns = model.Nodes[i].Constraints;

                buf += 6 - ((int) cns.DX + (int) cns.DY + (int) cns.DZ + (int) cns.RX + (int) cns.RY + (int) cns.RZ);
            }

            return buf;
        }

        private static string[] GetSystemInfo()
        {
            var buf = new string[5];

            try
            {
                var Mo = new ManagementObject("Win32_Processor.DeviceID='CPU0'");
                buf[0] = (Mo["Name"]).ToString();
                buf[1] = (Mo["CurrentClockSpeed"]).ToString();
                Mo.Dispose();

                Mo = new ManagementObject("Win32_OperatingSystem=@");
                buf[2] = (Mo["TotalVisibleMemorySize"]).ToString();
                Mo.Dispose();
            }
            catch (Exception ex)
            {
                for (int i = 0; i < buf.Length; i++)
                {
                    if (buf[i] == null)
                        buf[i] = "Error getting value";
                }
            }


            return buf;
        }

        private static void Log(string format, params object[] parameters)
        {
            var str = string.Format(format, parameters);

            Console.WriteLine(str);
            sb.AppendLine(str);
        }

        private static StringBuilder sb = new StringBuilder();


        private static Random rnd = new Random();

        private static Force GetRandomForce()
        {
            var buf = new Force(
                100*(1 - rnd.NextDouble()), 100*(1 - rnd.NextDouble()), 100*(1 - rnd.NextDouble()),
                100*(1 - rnd.NextDouble()), 100*(1 - rnd.NextDouble()), 100*(1 - rnd.NextDouble()));

            return buf;
        }
    }
}