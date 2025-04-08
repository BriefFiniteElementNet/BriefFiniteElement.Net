using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Mathh.StiffnessPermutationMatrix.Csparsenet;
using BriefFiniteElementNet.Utils;
using CSparse.Double;
using CSparse.Factorization;
using CSparse.Ordering;
using CSparse.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Mathh.StiffnessPermutationMatrix
{

    /// <summary>
    /// used when matrix is in parts. so each part treats as a dense matrix
    /// </summary>
    public class HybridRrefFinder : IRrefFinder
    {

        
        public SparseMatrix CalculateRref(SparseMatrix mtx)
        {

            throw new Exception();
            /*
            var a = mtx.Clone();

            //need to remove last column of 'a'

            MatrixUtils.SetColumnToZero(a, a.ColumnCount - 1);//set last column to zero

            a.DropZeros();

            {
                var n2 = new CoordinateStorage<double>(mtx.RowCount, mtx.ColumnCount, 10);

                for (var i = 0; i < 5; i+=2)
                {
                    n2.At(10, i, 1);
                    n2.At(10, i + 1, 1);
                }

                var a2 = n2.ToCCs();

                a = a.Add(a2);
            }

            
            
            //var tt2 = Extensions.ToDense(mtx).ToString();

            var r = MatrixUtils.GetColumnAsDense(mtx, a.ColumnCount - 1);

            {
                var A = SymbolicColumnStorage.Create(a, true);

                var S = A.Transpose().Multiply(A);

                {
                    var vals = Enumerable.Repeat(1.0, S.NonZerosCount).ToArray();

                    var mtx2 = new CSparse.Double.SparseMatrix(S.RowCount, S.ColumnCount, vals, S.RowIndices, S.ColumnPointers);

                    var m3 = Extensions.ToDense(mtx2).ToString();
                }

                var groups = GroupNodes(S);

                {
                    //check if all 
                    var maxGroup = groups.Max();
                    var counts = new int[maxGroup + 1];

                    for (var i = 0; i < groups.Length; i++)
                    {
                        var grp = groups[i];

                        counts[grp]++;
                    }


                    for (int group = 0; group < counts.Length; group++)
                    {
                        if (counts[group] > 2)//this group has more than 1 DoFs
                        {

                        }
                    }

                }


                var ssr = StronglyConnectedComponents.Generate(S, S.ColumnCount);

                var bp = ssr.BlockPointers;
                var tt = ssr.Indices;

                var counter = 0;

                var stk = new Stack<int>();

                var tempArray = new int[mtx.ColumnCount];

                for (int i = 0; i < bp.Length - 1; i++)
                {
                    var st = bp[i];
                    var en = bp[i + 1];

                    if (en == st + 1)//group with single DoF member
                        continue;

                    stk.Clear();

                    for (int j = st; j < en; j++)
                    {
                        var nodeId = tt[j];
                        stk.Push(nodeId);
                    }

                    ToDenseRref(a, r, stk,tempArray);

                    counter++;
                }
            }

            */
            throw new NotImplementedException();
        }


        static Matrix ToDenseRref(CompressedColumnStorage<double> mtx, double[] rightSide, Stack<int> dofs, int[] tempArray/*!*/)
        {
            var dense = new Matrix(mtx.RowCount, dofs.Count + 1);//dofs.count: count of dofs, +1: right side one member

            var perm = dofs.ToArray();


            for (int i = 0; i < tempArray.Length; i++)//clear
                tempArray[i] = -1;

            var cnt = 0;

            foreach (var dof in dofs)
            {
                tempArray[dof] = cnt++;
            }

            foreach (var item in mtx.EnumerateIndexed())
            {
                dense[item.Item1, tempArray[item.Item2]] = item.Item3;
            }

            throw new NotImplementedException();
        }

        /*
        static int[] GroupNodes(SymbolicColumnStorage S)
        {
            var buf = new int[S.ColumnCount];

            var ssr = StronglyConnectedComponents.Generate(S, S.ColumnCount);

            var bp = ssr.BlockPointers;
            var tt = ssr.Indices;

            var counter = 0;

            for (int i = 0; i < bp.Length - 1; i++)
            {
                var st = bp[i];
                var en = bp[i + 1];

                for (int j = st; j < en; j++)
                {
                    var nodeId = tt[j];
                    buf[nodeId] = counter;
                }

                counter++;
            }

            return buf;
        }

        */


    }
}
