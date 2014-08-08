using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace BriefFiniteElementNet.BenchmarkApplication
{
    class Program
    {

        static void Main(string[] args)
        {
            //Note: Before executing this benchmark set the Configuration to Release and use 'DEBUG>Start Without Debugging' or 'Ctrl+F5' to get the highest possible performance


            Log("###############################################################################");
            Log("**Performance benchmark for BriefFiniteElement.NET library \navailable via http://brieffiniteelmentnet.codeplex.com");
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
            Log("\tTotal RAM: {0:0.00} GB", double.Parse(sysInfo[2]) / (1024.0 * 1024.0));

            Log("###############################################################################");
            Log("");
            Log("Benchmark Info:");

            var solvers = Enum.GetValues(typeof (SolverType));

            var sw = System.Diagnostics.Stopwatch.StartNew();

            //10, 11, 12, 13, 14, 15
            var nums = new int[] { 10, 11, 12, 13, 14, 15 };

            var cnt = 0;

            foreach (var nm in nums)
            {
                

                var paramerts = new string[]
                    {
                        String.Format("grid size: {0}x{0}x{0}", nm),
                        String.Format("{0} elements", 3*nm*nm*(nm-1)),
                        String.Format("{0} nodes", nm*nm*nm),
                        String.Format("{0} free DoFs", 6*nm*nm*(nm-1))
                    };

                Log("Try # {0}", cnt++);
                Log(string.Join(", ", paramerts));
                

                foreach (SolverType solverType in solvers)
                {
                    var st = StructureGenerator.Generate3DGrid(nm, nm, nm);


                    foreach (var nde in st.Nodes)
                        nde.Loads.Add(new NodalLoad(GetRandomForce(), LoadCase.DefaultLoadCase));

                    var conf = new SolverConfiguration(new LoadCase()) {SolverType = solverType};

                    GC.Collect();

                    sw.Restart();


                    Log("");
                    Log("\tSolver type: {0}", solverType);


                    try
                    {
                        st.Solve(conf);
                        

                        Log("\t\tgeneral solve time: {0}", sw.Elapsed);

                        sw.Restart();
                        st.LastResult.AddAnalysisResult(LoadCase.DefaultLoadCase);
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

                buf += 6 - ((int) cns.Dx + (int) cns.Dy + (int) cns.Dz + (int) cns.Rx + (int) cns.Ry + (int) cns.Rz);
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
                100 * (1 - rnd.NextDouble()), 100 * (1 - rnd.NextDouble()), 100 * (1 - rnd.NextDouble()),
                100 * (1 - rnd.NextDouble()), 100 * (1 - rnd.NextDouble()), 100 * (1 - rnd.NextDouble()));

            return buf;
        }
    }
}
