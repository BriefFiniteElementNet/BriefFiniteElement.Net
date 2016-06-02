using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.BenchmarkApplication
{
    public class Benchmark1:IBenchmarkCase
    {
        public int Dimension { get; set; }

        public BuiltInSolverType SolverType { get; set; }

        public void DoTheBenchmark()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            var st = StructureGenerator.Generate3DGrid(Dimension, Dimension, Dimension);


            var case1 = new LoadCase("c1", LoadType.Other);
            var case2 = new LoadCase("c2", LoadType.Other);


            foreach (var nde in st.Nodes)
            {
                nde.Loads.Add(new NodalLoad(Util.GetRandomForce(), case1));
                nde.Loads.Add(new NodalLoad(Util.GetRandomForce(), case2));
            }

            var type = SolverType;

            var conf = new SolverConfiguration()
            {
                SolverGenerator = i => Util.CreateInternalSolver(type, i)
            };

            GC.Collect();

            Util.Log("");
            Util.Log("\tSolver type: {0}", Util.GetEnumDescription(SolverType));


            try
            {
                st.Solve(conf);

                sw.Restart();
                st.LastResult.AddAnalysisResultIfNotExists(case1);
                sw.Stop();

                Util.Log("\t\tgeneral solve time: {0}", sw.Elapsed);

                sw.Restart();
                st.LastResult.AddAnalysisResultIfNotExists(case2);
                sw.Stop();

                Util.Log("\t\textra solve time per LoadCase: {0}", sw.Elapsed);
            }
            catch (Exception ex)
            {
                Util.Log("\t\tFailed, err = {0}", ex.Message);
            }

            sw.Stop();

            GC.Collect();
        }

        
    }
}
