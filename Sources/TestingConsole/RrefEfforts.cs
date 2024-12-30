using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet;
using BriefFiniteElementNet.MpcElements;
using BriefFiniteElementNet.Utils;
using CSparse;
using CSparse.Factorization;
using CSparse.Ordering;

namespace TestingConsole
{
    public class RrefEfforts
    {
        public void Effort(CSparse.Storage.CompressedColumnStorage<double> mtx)
        {
            var model = StructureGenerator.Generate3DBarElementGrid(5, 5, 5);


            var m1 = new int[] { 12, 17, 54, 22, 35 };

            var m2 = new int[] { 11, 17, 45, 32, 34 };

            {
                var elm = new RigidElement_MPC();

                foreach (var i in m1)
                    elm.Nodes.Add(model.Nodes[i]);

                elm.AppliedLoadCases.Add(LoadCase.DefaultLoadCase);

                model.MpcElements.Add(elm);
            }

            {
                var elm = new RigidElement_MPC();

                foreach (var i in m2)
                    elm.Nodes.Add(model.Nodes[i]);

                elm.AppliedLoadCases.Add(LoadCase.DefaultLoadCase);

                model.MpcElements.Add(elm);
            }

            var cse = SolverUtils.GetModelMpcEquations(model, LoadCase.DefaultLoadCase);


            //var t = new CSparse.Double.Factorization.SparseQR(cse);



            var perm = AMD.Generate(cse, ColumnOrdering.MinimumDegreeAtA);

            var tree = CSparse.GraphHelper.EliminationTree(cse.RowCount, cse.ColumnCount, cse.ColumnPointers, cse.RowIndices, false);


            new SymbolicFactorization();


            model.Solve_MPC();
        }
    }
}
