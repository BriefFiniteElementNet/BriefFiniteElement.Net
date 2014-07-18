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
            //Note: Before executing this benchmark set the Configuration to Release and use 'DEBUG>Start Without Debbuging' or 'Ctrl+F5' to get the highest possible performance


            Log("#########################################################");
            Log("**Prformance benchmark for BriefFiniteElement.NET library available via 'brieffiniteelmentnet.codeplex.com'");
            Log("Benchmark 1: Uniform 3D Grid");


            if (Debugger.IsAttached) Log("A debugger is attached");

            Log("#########################################################");
            Log("Environment Info:");

            bool isDebug;
#if DEBUG
            isDebug = true;
#else
            isDebug = false;
#endif

            Log("\tOS: {0}", Environment.OSVersion);
            Log("\tIs OS 64bit: {0}", Environment.Is64BitOperatingSystem);
            Log("\tDebugger is Attached: {0}", Debugger.IsAttached);
            Log("\tMode: {0}", isDebug ? "Debug" : "Release");
            Log("#########################################################");
            Log("System Info:");
            var sysInfo = GetSystemInfo();
            Log("\tCPU Model: {0}", sysInfo[0]);
            Log("\tCPU Clock Speed: {0}", sysInfo[1]);
            Log("\tTotal RAM: {0:0.00} GB", double.Parse(sysInfo[2]) / (1024.0 * 1024.0));

            Log("#########################################################");
            Log("Benchmark Info:");

            var sw = System.Diagnostics.Stopwatch.StartNew();

            var nums = new int[] {10, 20, 30, 40};

            var cnt = 0;


            foreach (var nm in nums)
            {
                var st = StructureGenerator.Generate3DGrid(nm, nm, nm);



                Log("Try # {0}", cnt++);
                Log("\tGrid Size:\t{0}x{0}x{0}", nm);
                Log("\tElement Count:\t{0}", st.Elements.Count);
                Log("\tNode Count:\t{0}", st.Nodes.Count);
                Log("\tDoF Count:\t{0}", st.Nodes.Count * 6);

                foreach (var nde in st.Nodes)
                {
                    nde.Loads.Add(new NodalLoad(GetRandomForce(), LoadCase.DefaultLoadCase));
                }

                var conf = new SolverConfiguration();

                GC.Collect();

                sw.Restart();
                st.Solve(conf);
                sw.Stop();

                Log("\tGeneral solve time:\t{0}", sw.Elapsed);

                sw.Restart();
                st.LastResult.AddAnalysisResult(LoadCase.DefaultLoadCase);
                sw.Stop();

                Log("\tExtra Solve time per LoadCase:\t{0}", sw.Elapsed);

                GC.Collect();
            }

            Console.WriteLine("Done, Write result to file?[type 'Y' for yes, anything else for no]");
            var inf = Console.ReadKey();

            if (inf.KeyChar == 'y' || inf.KeyChar == 'Y')
            {
                System.IO.File.WriteAllText("BriefFemNet benchmark.txt", sb.ToString());
            }

            Environment.Exit(0);
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
