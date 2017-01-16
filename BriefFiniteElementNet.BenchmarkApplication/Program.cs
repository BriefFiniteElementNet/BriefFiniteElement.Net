using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using BriefFiniteElementNet.Common;
using CSparse.Double;
using BriefFiniteElementNet.Solver;

namespace BriefFiniteElementNet.BenchmarkApplication
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Note: Before executing this benchmark set the Configuration to Release and use 'DEBUG>Start Without Debugging' or 'Ctrl+F5' to get the highest possible performance

            var logger = new TextLogger();



            logger.Log("###############################################################################");
            logger.Log(
                "**Performance benchmark for BriefFiniteElement.NET library \navailable via http://brieffiniteelmentnet.codeplex.com");
            logger.Log("-------------------------------------------------------------------------------");
            logger.Log("Environment Info:");

            bool isDebug;
#if DEBUG
            isDebug = true;
#else
            isDebug = false;
#endif

            logger.Log("\tOS: {0}", Environment.OSVersion);
            //Log("\tIs OS 64bit: {0}", Environment.Is64BitOperatingSystem);
            logger.Log("\tDebugger is Attached: {0}", Debugger.IsAttached);
            logger.Log("\tMode: {0}", isDebug ? "Debug" : "Release");
            logger.Log("-------------------------------------------------------------------------------");
            logger.Log("System Info:");
            var sysInfo = GetSystemInfo();
            logger.Log("\tCPU Model: {0}", sysInfo[0]);
            logger.Log("\tCPU clock: {0} MHz", sysInfo[1]);
            logger.Log("\tTotal RAM: {0:0.00} GB", double.Parse(sysInfo[2])/(1024.0*1024.0));

            logger.Log("###############################################################################");
            logger.Log("");
            //Log("Benchmarks:");

            var solvers = Enum.GetValues(typeof (BuiltInSolverType));

            var sw = System.Diagnostics.Stopwatch.StartNew();

            var nums = new int[] {10, 11, 12, 13, 14, 15};

            var cnt = 0;

            var case1 = new LoadCase("c1", LoadType.Other);
            var case2 = new LoadCase("c2", LoadType.Other);

            var benchs = new IBenchmarkCase[] {new Benchmark1() {Logger = logger}, new Benchmark2() {Logger = logger}};

            //var bnchTypes = new Type[] {typeof(Benchmark1), typeof(Benchmark2)};// IBenchmarkCase[] { new Benchmark1() { Logger = logger }, new Benchmark2() { Logger = logger } };


            var cnt1 = 1;

            //foreach (var bnchTp in bnchTypes)
            foreach (var bnch in benchs)
            {

                //var bnch = (IBenchmarkCase)Activator.CreateInstance(bnchTp);
                bnch.Logger = logger;

                logger.Log("");
                logger.Log("=========");
                logger.Log("Benchmark #{0}: {1}", cnt1++, bnch.GetBenchmarkInfo());
                
                logger.Log("");

                cnt = 0;

                foreach (var nm in nums)
                {
                    bnch.Dimension = nm;

                    var model = bnch.GetCaseModel();


                    var paramerts = new string[]
                    {
                        String.Format("Benchamrk Dimension: {0}", nm),
                        String.Format("{0} elements", model.Elements.Count),
                        String.Format("{0} nodes", model.Nodes.Count),
                        String.Format("{0} free DoFs", Util.GetFreeDofsCount(model))
                    };


                    logger.Log("Try # {0}", cnt++);
                    logger.Log(string.Join(", ", paramerts));




                    foreach (BuiltInSolverType solverType in solvers)
                    {
                        //Benchmark1.DoIt(nm, solverType);
                        bnch.SolverType = solverType;
                        
                        bnch.DoTheBenchmark();
                        
                        GC.Collect();
                    }
                }
            }

            

            Console.WriteLine();
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("Done, Write result to file? [y/n]");

            var inf = Console.ReadKey();

            if (inf.KeyChar == 'y' || inf.KeyChar == 'Y')
            {
                var fileName = "BFEbenchmark.txt";
                System.IO.File.WriteAllText(fileName, logger.GetAllLog());
                System.Diagnostics.Process.Start(fileName);
            }
            

            Console.WriteLine("Done, press any key to exit...");


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