using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Solver;
using BriefFiniteElementNet.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Benchmarks
{
    //3d grid with bar element
    public class BarGrid
    {

        public void Run(int dim)
        {

            ISolverFactory[] solvers;

            {
                solvers = new ISolverFactory[] { new CholeskySolverFactory(), new ConjugateGradientFactory() };
            }

            Model origModel;

            {
                origModel = StructureGenerator.Generate3DBarElementGrid(dim, dim, dim);

                StructureGenerator.AddRandomiseLoading(origModel, true, true, LoadCase.DefaultLoadCase);

                StructureGenerator.AddRandomDisplacements(origModel, 0.1);
            }


            foreach(var solver in solvers)
            {
                Model clone;

                using (var str = new MemoryStream())
                {
                    Model.Save(str, origModel);
                    str.Position = 0;
                    clone = Model.Load(str);
                }

                clone.Trace.Listeners.Add(new ConsoleTraceListener());

                {
                    clone.Trace.Write(TraceLevel.Info, "--------------");
                    clone.Trace.Write(TraceLevel.Info, "Dimension: {0}", dim);
                    clone.Trace.Write(TraceLevel.Info, "Solver: {0}", solver.GetType().Name);

                    var dofs = clone.Nodes.SelectMany(i => i.Constraints.ToArray());
                    clone.Trace.Write(TraceLevel.Info, "Fixed DoFs: {0}", dofs.Count(i => i.IsFixed()));
                    clone.Trace.Write(TraceLevel.Info, "Freed DoFs: {0}", dofs.Count(i => i.IsReleased()));

                }

                var cfg = new SolverConfiguration();

                cfg.SolverFactory = solver;
                clone.Solve_MPC();
            }



            /* result:
             * 
dim	fixed dof	free dof	permutatio ms	stiffness ms	boundary ms	eq ms
2	24	24	17	14	0	4
3	54	108	0	2	0	0
4	96	288	0	6	0	1
5	150	600	1	12	0	2
6	216	1080	4	21	1	7
7	294	1764	7	35	2	18
8	384	2688	16	53	3	41
9	486	3888	25	75	5	85
10	600	5400	51	104	7	188
11	726	7260	81	138	9	376
12	864	9504	122	177	14	732
13	1014	12168	181	234	17	1215
14	1176	15288	272	297	23	2047
15	1350	18900	368	370	28	3465
16	1536	23040	500	438	31	5474
17	1734	27744	682	519	37	9289
18	1944	33048	898	639	42	12984
19	2166	38988	1181	829	54	22823
             
             */
        }
    }
}
