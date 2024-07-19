using CSparse.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCS = CSparse.Double.SparseMatrix;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Utils;

namespace BriefFiniteElementNet.Mathh
{
    public class BuiltinGaussDisplacementPermutationFinder : IDisplacementPermutationCalculator
    {
        public Tuple<CCS, double[]> CalculateDisplacementPermutation(CCS a)
        {
            var totDofCount = a.ColumnCount - 1;


            var allEqs = a;


            //var empties = allEqs.EmptyRowCount();

            //var dns = allEqs.ToDenseMatrix();


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

            var rrefFinder = new GaussRrefFinder();

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


    }
}
