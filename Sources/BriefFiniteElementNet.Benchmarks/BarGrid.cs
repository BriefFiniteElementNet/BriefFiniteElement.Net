using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Solver;
using BriefFiniteElementNet.Solvers;
using BriefFiniteElementNet.Validation;
using CSparse.Factorization;
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
                solvers = new ISolverFactory[] {
                    new CholeskySolverFactory(),
                    new ConjugateGradientFactory()
                    //new CholModSolverFactory()
                 };
            }

            Model origModel;

            {
                origModel = StructureGenerator.Generate3DBarElementGrid(dim, dim, dim);

                StructureGenerator.AddRandomiseLoading(origModel, true, true, LoadCase.DefaultLoadCase);

                StructureGenerator.AddRandomDisplacements(origModel, 0.1);
            }

            {
                origModel.Trace.Listeners.Add(new ConsoleTraceListener());

                origModel.Trace.Write(TraceLevel.Info, "-----------------------");
                origModel.Trace.Write(TraceLevel.Info, "Dimension: {0}", dim);
                var dofs = origModel.Nodes.SelectMany(i => i.Constraints.ToArray());
                origModel.Trace.Write(TraceLevel.Info, "Fixed DoFs: {0}", dofs.Count(i => i.IsFixed()));
                origModel.Trace.Write(TraceLevel.Info, "Freed DoFs: {0}", dofs.Count(i => i.IsReleased()));
                origModel.Trace.Write(TraceLevel.Info, "------");
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
                    clone.Trace.Write(TraceLevel.Info, "---");
                    clone.Trace.Write(TraceLevel.Info, "Solver: {0}", solver.GetType().Name);
                }

                var cfg = new SolverConfiguration();

                cfg.SolverFactory = solver;

                cfg.LoadCases.Add(LoadCase.DefaultLoadCase);

                clone.Solve_MPC(cfg);
            }

        }
    }
}
