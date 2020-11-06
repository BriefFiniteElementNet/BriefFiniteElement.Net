using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet;
using CCS = CSparse.Double.SparseMatrix;
using BriefFiniteElementNet.Solver;

namespace BriefFiniteElementNet.Legacy
{
    public static class SolveExtensions
    {
        /// <summary>
        /// Adds the analysis result.
        /// </summary>
        /// <param name="loadCase">The load case.</param>
        /// <remarks>if model is analyzed against specific load case, then displacements are available through <see cref="Displacements"/> property.
        /// If system is not analyses against a specific load case, then this method will analyses structure against <see cref="LoadCase"/>.
        /// While this method is using pre computed Cholesky Decomposition , its have a high performance in solving the system.
        /// </remarks>
        public static void AddAnalysisResult(this StaticLinearAnalysisResult thiis, LoadCase loadCase)
        {
            ISolver solver;

            var map = DofMappingManager.Create(thiis.Parent, loadCase);

            var n = thiis.Parent.Nodes.Count;//node count
            var m = map.M;//master node count

            var pu = PermutationGenerator.GetDisplacementPermute(thiis.Parent, map);//permutation of U
            var pf = PermutationGenerator.GetForcePermute(thiis.Parent, map);//permutation of F

            var fe = thiis.ElementForces[loadCase] = thiis.GetTotalElementsForceVector(loadCase);
            var fc = thiis.ConcentratedForces[loadCase] = thiis.GetTotalConcentratedForceVector(loadCase);


            var ft = fe.Add(fc);


            var fr = pf.Multiply(ft);
            var ffr = thiis.GetFreePartOfReducedVector(fr, map);
            var fsr = thiis.GetFixedPartOfReducedVector(fr, map);

            var kt = MatrixAssemblerUtil.AssembleFullStiffnessMatrix(thiis.Parent);
            var kr = (CCS)((CCS)pf.Multiply(kt)).Multiply(pu);

            #region  U_s,r
            var usr = new double[map.RMap3.Length];

            {
                //should fill usr
                var ut_temp = thiis.GetTotalDispVector(loadCase, map);

                for (int i = 0; i < usr.Length; i++)
                {
                    var t1 = map.RMap3[i];
                    var t2 = map.RMap1[t1];

                    usr[i] = ut_temp[t2];
                }

            }

            #endregion

            var krd = CalcUtil.GetReducedZoneDividedMatrix(kr, map);
            thiis.AnalyseStiffnessMatrixForWarnings(krd, map, loadCase);

            {//TODO: remove

                var minAbsDiag = double.MaxValue;

                foreach (var tpl in krd.ReleasedReleasedPart.EnumerateIndexed())
                {
                    if (tpl.Item1 == tpl.Item2)
                        minAbsDiag = Math.Min(minAbsDiag, Math.Abs(tpl.Item3));
                }

                if (krd.ReleasedReleasedPart.RowCount != 0)
                {
                    //var kk = krd.ReleasedReleasedPart.ToDenseMatrix();
                }
            }

            #region  solver

            if (thiis.Solvers.ContainsKey(map.MasterMap))
            {
                solver = thiis.Solvers[map.MasterMap];
            }
            else
            {
                solver =
                    //SolverGenerator(krd.ReleasedReleasedPart);
                    thiis.SolverFactory.CreateSolver(krd.ReleasedReleasedPart);

                thiis.Solvers[map.MasterMap] = solver;
            }


            if (!solver.IsInitialized)
                solver.Initialize();

            #endregion

            double[] ufr = new double[map.RMap2.Length];
            //string message;

            var input = ffr.Subtract(krd.ReleasedFixedPart.Multiply(usr));


            solver.Solve(input, ufr);

            //if (res2 != SolverResult.Success)
            //    throw new BriefFiniteElementNetException(message);

            var fpsr = krd.FixedReleasedPart.Multiply(ufr).Add(krd.FixedFixedPart.Multiply(usr));

            var fsrt = fpsr.Subtract(fsr);// no needed

            var fx = thiis.SupportReactions[loadCase] = new double[6 * n];

            #region forming ft


            for (var i = 0; i < map.Fixity.Length; i++)
                if (map.Fixity[i] == DofConstraint.Fixed)
                    ft[i] = 0;

            for (var i = 0; i < fpsr.Length; i++)
            {
                var totDofNum = map.RMap1[map.RMap3[i]];

                ft[totDofNum] = fx[totDofNum] = fpsr[i];
            }


            #endregion

            #region forming ur

            var ur = new double[map.M * 6];


            for (var i = 0; i < usr.Length; i++)
            {
                ur[map.RMap3[i]] = usr[i];
            }

            for (var i = 0; i < ufr.Length; i++)
            {
                ur[map.RMap2[i]] = ufr[i];
            }

            #endregion

            var ut = pu.Multiply(ur);

            thiis.Forces[loadCase] = ft;
            thiis.Displacements[loadCase] = ut;
        }
    }
}
