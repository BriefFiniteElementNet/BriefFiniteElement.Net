using CSparse.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Mathh;
using BriefFiniteElementNet.Solver;
using CSparse.Double;
using CSparse.Storage;
using System.Globalization;


namespace BriefFiniteElementNet.Utils
{
    public static class SolverUtils
    {

        public static Tuple<SparseMatrix, double[]> GenerateP_Delta_Mpc(Model target, LoadCase loadCase, IRrefFinder rrefFinder)
        {
            var totDofCount = target.Nodes.Count * 6;

            target.ReIndexNodes();

            var n = target.Nodes.Count;

            var boundaryConditions = GetModelBoundaryConditions(target, loadCase);

            var lastRow = 0;

            #region step 1
            var extraEqCount = 0;

            foreach (var mpcElm in target.MpcElements)
                if (mpcElm.AppliesForLoadCase(loadCase))
                    extraEqCount += mpcElm.GetExtraEquationsCount();

            //extraEqCount += boundaryConditions.RowCount;


            var totalEqCount = extraEqCount + boundaryConditions.RowCount;


            var allEqsCrd = new CoordinateStorage<double>(totalEqCount, n * 6 + 1, 1);//rows: extra eqs, cols: 6*n+1 (+1 is for right hand side)

            foreach (var mpcElm in target.MpcElements)
            {
                if (mpcElm.AppliesForLoadCase(loadCase))
                    if (mpcElm.GetExtraEquationsCount() != 0)
                    {
                        var extras = mpcElm.GetExtraEquations();

                        if (extras.ColumnCount != totDofCount + 1)
                            throw new Exception();

                        foreach (var tuple in extras.EnumerateIndexed())
                        {
                            var row = tuple.Item1;
                            var col = tuple.Item2;
                            var val = tuple.Item3;


                            allEqsCrd.At(row + lastRow, col, val);
                        }

                        lastRow += extras.RowCount;
                    }
            }

            {
                if (boundaryConditions.ColumnCount != totDofCount + 1)
                    throw new Exception();


                foreach (var tuple in boundaryConditions.EnumerateIndexed())
                {
                    var row = tuple.Item1;
                    var col = tuple.Item2;
                    var val = tuple.Item3;


                    allEqsCrd.At(row + lastRow, col, val);
                }

                /*
                boundaryConditions.EnumerateMembers((row, col, val) =>
                {
                    allEqsCrd.At(row + lastRow, col, val);
                });
                */

                lastRow += boundaryConditions.RowCount;
            }

            var allEqs = allEqsCrd.ToCCs();


            var empties = allEqs.EmptyRowCount();

            //var dns = allEqs.ToDenseMatrix();

            #endregion

            #region comment
            /*
            #region step 2
            //step 2: create adjacency matrix of variables

            //step 2-1: find nonzero pattern
            var allEqsNonzeroPattern = allEqs.Clone();

            for (var i = 0; i < allEqsNonzeroPattern.Values.Length; i++)
                allEqsNonzeroPattern.Values[i] = 1;

            //https://math.stackexchange.com/questions/2340450/extract-independent-sub-systems-from-a-bigger-linear-eq-system
            var tmp = allEqsNonzeroPattern.Transpose();

            var variableAdj = tmp.Multiply(allEqsNonzeroPattern);
            #endregion

            #region step 3
            //extract parts
            var parts = EnumerateGraphParts(variableAdj);

            #endregion

            #region step 4
            {

                allEqs.EnumerateColumns((colNum, vals) =>
                {
                    if (vals.Count == 0)
                    Console.WriteLine("Col {0} have {1} nonzeros", colNum, vals.Count);
                });

                var order = ColumnOrdering.MinimumDegreeAtPlusA;

                // Partial pivoting tolerance (0.0 to 1.0)
                double tolerance = 1.0;

                var lu = CSparse.Double.Factorization.SparseLU.Create(allEqs, order, tolerance);

            }

            #endregion
            */

            #endregion


            var rref = rrefFinder.CalculateRref(allEqs);

            var rrefSys = SparseEqSystem.Generate(rref);

            #region generate P_Delta

            var pRows = new int[totDofCount]; // pRows[i] = index of equation that its right side is Di (ai*Di = a1*D1+...+an*Dn)

            pRows.FillWith(-1);

            for (var i = 0; i < rrefSys.RowCount; i++)
            {
                foreach (var tpl in rrefSys.Equations[i].EnumerateIndexed())
                {
                    if (rrefSys.ColumnNonzeros[tpl.Item1] == 1)
                    {
                        if (pRows[tpl.Item1] != -1)
                            throw new Exception();

                        pRows[tpl.Item1] = i;
                    }
                }
            }

            int cnt = 0;

            var lastColIndex = rrefSys.ColumnCount - 1;

            var p2Coord = new CoordinateStorage<double>(totDofCount, totDofCount, 1);

            var rightSide = new double[totDofCount];





            for (var i = 0; i < totDofCount; i++)
            {
                if (pRows[i] == -1)
                {
                    p2Coord.At(i, i, 1);
                    continue;
                }

                var eq = rrefSys.Equations[pRows[i]];
                eq.Multiply(-1 / eq.GetMember(i));

                var minus1 = eq.GetMember(i);

                if (!minus1.FEquals(-1, 1e-9))
                    throw new Exception();

                //eq.SetMember(i, 0);

                foreach (var tpl in eq.EnumerateIndexed())
                {
                    if (tpl.Item1 != lastColIndex)
                        p2Coord.At(i, tpl.Item1, tpl.Item2);
                    else
                        rightSide[i] = tpl.Item2;
                }
            }




            cnt = 0;

            foreach (var eq in rrefSys.Equations)
            {
                if (eq.IsZeroRow(1e-9))
                {
                    cnt++;
                }
            }

            #endregion

            var p2 = p2Coord.ToCCs();

            var colsToRemove = new bool[totDofCount];

            var colNumPerm = new int[totDofCount];

            colNumPerm.FillWith(-1);

            colsToRemove.FillWith(false);

            var tmp = 0;

            for (var i = 0; i < rrefSys.ColumnNonzeros.Length; i++)
                if (i != lastColIndex)
                {
                    if (rrefSys.ColumnNonzeros[i] == 1)
                        colsToRemove[i] = true;
                    else
                        colNumPerm[i] = tmp++;
                }


            var p3Crd = new CoordinateStorage<double>(totDofCount, totDofCount - colsToRemove.Count(i => i), 1);

            foreach (var tpl in p2.EnumerateIndexed())
            {
                if (!colsToRemove[tpl.Item2])
                {
                    p3Crd.At(tpl.Item1, colNumPerm[tpl.Item2], tpl.Item3);
                }
            }

            var p3 = p3Crd.ToCCs();

            return Tuple.Create(p3, rightSide);

        }

        public static Tuple<CSparse.Double.SparseMatrix, double[]> GenerateP_Delta_Mpc(Model target, LoadCase loadCase, IDisplacementPermutationCalculator permFinder)
        {
            var totDofCount = target.Nodes.Count * 6;

            target.ReIndexNodes();

            var n = target.Nodes.Count;

            var boundaryConditions = GetModelBoundaryConditions(target, loadCase);

            var lastRow = 0;

            #region mpc elements
            var extraEqCount = 0;

            foreach (var mpcElm in target.MpcElements)
                if (mpcElm.AppliesForLoadCase(loadCase))
                    extraEqCount += mpcElm.GetExtraEquationsCount();

            var totalEqCount = extraEqCount + boundaryConditions.RowCount;

            var allEqsCrd = new CoordinateStorage<double>(totalEqCount, n * 6 + 1, 1);//rows: extra eqs, cols: 6*n+1 (+1 is for right hand side)

            foreach (var mpcElm in target.MpcElements)
            {
                if (mpcElm.AppliesForLoadCase(loadCase))
                    if (mpcElm.GetExtraEquationsCount() != 0)
                    {
                        var extras = mpcElm.GetExtraEquations();

                        if (extras.ColumnCount != totDofCount + 1)
                            throw new Exception();

                        foreach (var tuple in extras.EnumerateIndexed())
                        {
                            var row = tuple.Item1;
                            var col = tuple.Item2;
                            var val = tuple.Item3;


                            allEqsCrd.At(row + lastRow, col, val);
                        }

                        lastRow += extras.RowCount;
                    }
            }

            {
                if (boundaryConditions.ColumnCount != totDofCount + 1)
                    throw new Exception();

                foreach (var tuple in boundaryConditions.EnumerateIndexed())
                {
                    var row = tuple.Item1;
                    var col = tuple.Item2;
                    var val = tuple.Item3;


                    allEqsCrd.At(row + lastRow, col, val);
                }

                lastRow += boundaryConditions.RowCount;
            }

            var allEqs = allEqsCrd.ToCCs();

            //var dns = allEqs.ToDenseMatrix();

            #endregion

            {
                var rowNnzs = new int[allEqs.RowCount];//nnz count of each row disregard last column which is right side

                foreach (var tpl in allEqs.EnumerateIndexed())
                {
                    if (tpl.Item3 != 0)
                        if (tpl.Item2 != allEqs.ColumnCount - 1)
                            rowNnzs[tpl.Item1]++;
                }

                if (rowNnzs.Max() < 2)//each row maximum have 1 nonzero
                {
                    //then no need to calculate rref by elimition etc. just a single permutation is needed

                    //var mults
                }
            }


            var res = permFinder.CalculateDisplacementPermutation(allEqs);

            return res;
            var buf = new CoordinateStorage<double>(res.Item1.RowCount, res.Item1.ColumnCount - 1, 1);

            // TODO: NOT IMPLEMENTED?
            throw new NotImplementedException();
            /*
            foreach (var tpl in res.EnumerateIndexed2())
            {
                {
                    buf.At(tpl.Item1, colNumPerm[tpl.Item2], tpl.Item3);
                }
            }

            var p3 = p3Crd.ToCCs();

            return Tuple.Create(p3, rightSide);
            */
        }



        /// <summary>
        /// Gets the boundary conditions of model (support conditions) as a extra eq system for using in master slave model.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="loadCase"></param>
        /// <returns></returns>
        internal static SparseMatrix GetModelBoundaryConditions(Model model, LoadCase loadCase)
        {
            var fixedDofsCount = model.Nodes.Sum(ii => CalcUtil.FixedCount(ii.Constraints));

            var n = model.Nodes.Count * 6;

            var crd = new CoordinateStorage<double>(fixedDofsCount, 6 * model.Nodes.Count + 1, 1);

            var cnt = 0;

            foreach (var node in model.Nodes)
            {
                var stDof = 6 * node.Index;

                var stm = Displacement.Zero;

                foreach (var stlm in node.Settlements)
                    if (stlm.LoadCase == loadCase)
                        stm += stlm.Displacement;


                #region 

                if (node.Constraints.DX == DofConstraint.Fixed)
                {
                    crd.At(cnt, stDof + 0, 1);
                    crd.At(cnt, n, stm.DX);
                    cnt++;
                }

                if (node.Constraints.DY == DofConstraint.Fixed)
                {
                    crd.At(cnt, stDof + 1, 1);
                    crd.At(cnt, n, stm.DY);
                    cnt++;
                }

                if (node.Constraints.DZ == DofConstraint.Fixed)
                {
                    crd.At(cnt, stDof + 2, 1);
                    crd.At(cnt, n, stm.DZ);
                    cnt++;
                }


                if (node.Constraints.RX == DofConstraint.Fixed)
                {
                    crd.At(cnt, stDof + 3, 1);
                    crd.At(cnt, n, stm.RX);
                    cnt++;
                }

                if (node.Constraints.RY == DofConstraint.Fixed)
                {
                    crd.At(cnt, stDof + 4, 1);
                    crd.At(cnt, n, stm.RY);
                    cnt++;
                }

                if (node.Constraints.RZ == DofConstraint.Fixed)
                {
                    crd.At(cnt, stDof + 5, 1);
                    crd.At(cnt, n, stm.RZ);
                    cnt++;
                }

                #endregion
            }

            return crd.ToCCs();
        }

    }
}
