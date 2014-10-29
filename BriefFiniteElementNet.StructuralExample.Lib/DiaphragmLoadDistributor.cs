using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Solver;

namespace BriefFiniteElementNet.StructuralExample.Lib
{
    public class DiaphragmLoadDistributor
    {
        /// <summary>
        /// Distributes the concentrated forces which applied to diaphragm center into the connected nodes.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="diaphragms">list of diaphragms.</param>
        /// <param name="diaphragmLoads">The concentrated forces on each rigid diaphragm.</param>
        /// <returns>Loads on each node, with same order of <see cref="Model.Nodes"/> collection.</returns>
        public Force[] DistributeDiaphragmLoads(Model model, List<RigidDiaphragm> diaphragms,
            Dictionary<string, Force> diaphragmLoads)
        {
            var cloneModel = model.Clone();

            var diaphragmElements = diaphragms.Select(i => GetRigidDiaphragmElements(model, i)).ToList();

            foreach (var nde in cloneModel.Nodes)
            {
                nde.Loads.Clear();
                nde.Settlements = Displacement.Zero;
            }

            foreach (var elm in cloneModel.Elements)
            {
                elm.Loads.Clear();
            }

            foreach (var dp in diaphragms)
            {
                var closestNode = dp.Nodes.OrderBy(i => (i.Location - dp.MassCenter).Length).First();
                var transferedLoad = diaphragmLoads[dp.Label].Move(dp.MassCenter, closestNode.Location);

                closestNode.Loads.Add(new NodalLoad(transferedLoad));
            }

            cloneModel.Solve(SolverType.CholeskyDecomposition);

            var uf = cloneModel.LastResult.Displacements[LoadCase.DefaultLoadCase];

            var kff = (cloneModel.LastResult.Solver as CholeskySolver).A;

            var ff = new double[uf.Length];

            kff.Multiply(uf, ff);

            //cloneModel.LastResult.Displacements[new LoadCase("DistributedLoads", LoadType.Other)]=;

            var n = model.Nodes.Count;
            
            var fullDisplacement = new double[6*n];

            var cnt = 0;

            var f = model.LastResult.ReleasedMap;



            for (var i = 0; i < uf.Length; i++)
            {
                var nde = model.Nodes[i];

                var cst = nde.Constraints;

                var disp = new Displacement();



                if (cst.DX == DofConstraint.Released)
                    disp.DX = uf[cnt++];

                if (cst.DY == DofConstraint.Released)
                    disp.DY = uf[cnt++];

                if (cst.DZ == DofConstraint.Released)
                    disp.DZ = uf[cnt++];



                if (cst.RX == DofConstraint.Released)
                    disp.RX = uf[cnt++];

                if (cst.RY == DofConstraint.Released)
                    disp.RY = uf[cnt++];

                if (cst.RZ == DofConstraint.Released)
                    disp.RZ = uf[cnt++];

                var arr = ToDoubleArray(disp);

                for (var j = 0; j < 6; j++)
                    fullDisplacement[6*i + j] = arr[j];
            }

            throw new NotImplementedException();
        }


        private double[] ToDoubleArray(Displacement disp)
        {
            return new double[] {disp.DX, disp.DY, disp.DZ, disp.RX, disp.RY, disp.RZ};
        }

        /// <summary>
        /// Gets a set of elements which can be assumed as a rigid diaphragm that contains specified <see cref="RigidDiaphragm.Nodes"/>.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="diaphragm">The diaphragm.</param>
        /// <returns>the list of elements</returns>
        private Element[] GetRigidDiaphragmElements(Model model, RigidDiaphragm diaphragm)
        {
            var elements = new List<FrameElement2Node>();

            if (diaphragm.Nodes.Count < 2)
                return elements.Cast<Element>().ToArray();

            var n0 = diaphragm.Nodes[0];

            for (var i = 1; i < diaphragm.Nodes.Count; i++)
                elements.Add(new FrameElement2Node(n0, diaphragm.Nodes[i]));


            return elements.Cast<Element>().ToArray();
        }

    }
}
