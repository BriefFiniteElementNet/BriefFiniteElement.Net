using BriefFiniteElementNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.CSharp
{
    public class MklExample
    {
        public static void Run()
        {
            var model = BriefFiniteElementNet.StructureGenerator.Generate3DTetrahedralElementGrid(2, 2, 2);

            var material = BriefFiniteElementNet.Materials.UniformIsotropicMaterial.CreateFromYoungPoisson(210e9, 0.2);


            foreach (var elm in model.Elements)
                if (elm is BriefFiniteElementNet.Elements.TetrahedronElement tet)
                    tet.Material = material;


            model.Nodes.Last().Loads.Add(new NodalLoad(new Force(10, 0, 0, 0, 0, 0), LoadCase.DefaultLoadCase));

            var solverFactory = new BriefFiniteElementNet.Solvers.MklSolverFactory();

            var cfg = new BriefFiniteElementNet.SolverConfiguration(solverFactory, LoadCase.DefaultLoadCase);

            model.Solve_MPC(cfg);
        }
    }
}
