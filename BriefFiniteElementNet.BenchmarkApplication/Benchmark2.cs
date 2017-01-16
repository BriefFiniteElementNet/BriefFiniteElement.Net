using System;
using System.Diagnostics;
using BriefFiniteElementNet.Common;


namespace BriefFiniteElementNet.BenchmarkApplication
{
    public class Benchmark2 : IBenchmarkCase
    {
        public Model GetCaseModel()
        {
            return StructureGenerator.Generate3DTet4Grid(Dimension, Dimension, Dimension);
        }

        public string GetBenchmarkInfo()
        {
            return "3D Grid with Tetrahedron Element";
        }

        public int Dimension { get; set; }

        public BuiltInSolverType SolverType { get; set; }

        public TextLogger Logger { get; set; }


        public void DoTheBenchmark()
        {
            var sw = Stopwatch.StartNew();

            var st = GetCaseModel();


            var case1 = new LoadCase("c1", LoadType.Other);
            var case2 = new LoadCase("c2", LoadType.Other);


            foreach (var nde in st.Nodes)
            {
                nde.Loads.Add(new NodalLoad(Util.GetRandomForce(), case1));
                nde.Loads.Add(new NodalLoad(Util.GetRandomForce(), case2));
            }

            var type = SolverType;

            var conf = new SolverConfiguration
            {
                //SolverGenerator = i => CalcUtil.CreateBuiltInSolver(type, i),
                SolverFactory = CalcUtil.CreateBuiltInSolverFactory(type)
            };

            GC.Collect();

            Logger.Log("");
            Logger.Log("\tSolver type: {0}", Util.GetEnumDescription(SolverType));


            try
            {
                st.Solve(conf);

                sw.Restart();
                st.LastResult.AddAnalysisResultIfNotExists(case1);
                sw.Stop();

                Logger.Log("\t\tgeneral solve time: {0}", sw.Elapsed);

                sw.Restart();
                st.LastResult.AddAnalysisResultIfNotExists(case2);
                sw.Stop();

                Logger.Log("\t\textra solve time per LoadCase: {0}", sw.Elapsed);
            }
            catch (Exception ex)
            {
                Logger.Log("\t\tFailed, err = {0}", ex.Message);
            }

            sw.Stop();

            GC.Collect();
        }
    }
}