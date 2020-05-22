using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CSparse;
using CSparse.Double;
using CSparse.Double.Factorization;
using CSparse.Factorization;
using CSparse.Storage;
using CCS = CSparse.Double.SparseMatrix;
using BriefFiniteElementNet.Common;

namespace BriefFiniteElementNet.Mathh
{
    public class CsparsenetQrDisplacementPermutationCalculator : IDisplacementPermutationCalculator
    {
        public Tuple<CCS, double[]> CalculateDisplacementPermutation(CCS a)
        {

            

            //based on this solution : https://github.com/wo80/CSparse.NET/issues/7#issuecomment-317268696

            if (a.RowCount < a.ColumnCount)
            {
                //For a matrix A with rows < columns, SparseQR does a factorization of the transpose A'
                //so we add zero rows to A and make it rectangular!

                var a2 = new CoordinateStorage<double>(a.ColumnCount , a.ColumnCount, a.NonZerosCount);

                foreach (var t in a.EnumerateIndexed())
                    a2.At(t.Item1, t.Item2, t.Item3);

                a = a2.ToCCs();
            }

            //var adense = a.ToDenseMatrix();

            SparseMatrix buf;

            var n = a.RowCount;
            var m = a.ColumnCount;

            var order = ColumnOrdering.Natural;

            var sp = System.Diagnostics.Stopwatch.StartNew();

            var qr = SparseQR.Create(a, order);

            var t2 = sp.ElapsedMilliseconds;


            var r = GetFactorR(qr);
            var q = GetFactorQ(qr);
            var s = GetSymbolicFactorization(qr);

            var leads = new int[a.RowCount];
            leads.FillWith(-1);
            //leads[i] = columnIndex of first nonzero in i'th row of r fasctor
            //if leads[i] == -1, then there is no nonzero in row[i] then i'th equation in original a matrix in non usable
            //this will be used for rdep and rindep

            var epsilon = 1e-8;

            
            foreach (var r_item in r.EnumerateIndexed())
            {
                var rowNum = r_item.Item1;
                var colNum = r_item.Item2;
                var val = r_item.Item3;


                if (Math.Abs(val) <= epsilon)
                    ;//then row at 

                if (Math.Abs(val) > epsilon) //Then, if abs(R[i, i]) > tol, you found an independent row
                    //Make sure to take row permutations into account (again, you can get the SymbolicFactorization containing row and column permutations using reflection)
                {
                    if (leads[rowNum] == -1)
                        leads[rowNum] = colNum;
                    else
                        leads[rowNum] = Math.Min(leads[rowNum], colNum);
                }
            }
            
            
           
            //throw new NotImplementedException("above section not right");

            var leadCount = leads.Count(i => i != -1);

            {
                

                var nnzApprox = leadCount;
                var p1Crd = new CoordinateStorage<double>(leadCount, r.RowCount, nnzApprox);
                var p2Crd = new CoordinateStorage<double>(r.ColumnCount, leadCount, nnzApprox);
                var p2pCrd = new CoordinateStorage<double>(r.ColumnCount, r.ColumnCount - leadCount, nnzApprox);

                var cnt1 = 0;
                var cnt2 = 0;

                for (var i = 0; i < leads.Length; i++)
                {
                    if (leads[i] != -1)
                    {
                        var j = leads[i];

                        p1Crd.At(cnt1, i, 1);
                        p2Crd.At(j, cnt1, 1);

                        cnt1++;
                    }

                    if (leads[i] == -1)
                    {
                        var j = i;// leads[i];

                        p2pCrd.At(j, cnt2, 1);

                        cnt2++;
                    }
                }

                var p1 = p1Crd.ToCCs();
                var p2 = p2Crd.ToCCs();
                var p2p = p2pCrd.ToCCs();

                //var rp = r.Transpose();

                var rdep = p1.Multiply(r).Multiply(p2);

                var rinDep = p1.Multiply(r).Multiply(p2p);

                var qr2 = SparseQR.Create(rdep, order);

                var t = sp.ElapsedMilliseconds;

                var right = new double[rinDep.RowCount];
                var sol = new double[rinDep.RowCount];

                var bufCrd = new CoordinateStorage<double>(leadCount, a.ColumnCount-leadCount, leadCount);

                for (var j = 0; j < rinDep.ColumnCount; j++)
                {
                    //put the j'th column of r into the rightSide
                    
                    right.FillWith(0);

                    {
                        var col = j;

                        var st = rinDep.ColumnPointers[j];
                        var end = rinDep.ColumnPointers[j + 1];

                        for (var ii = st; ii < end; ii++)
                        {
                            var row = rinDep.RowIndices[ii];

                            var val = rinDep.Values[ii];

                            right[row] = val;
                        }
                    }

                    qr2.Solve(right, sol);

                    for (var i = 0; i < rdep.RowCount; i++) bufCrd.At(i, j, sol[i]);
                }

                buf = bufCrd.ToCCs();
            }

            //var top = buf.ToDenseMatrix();

            var buft = buf.Transpose();


            var masterCount = a.ColumnCount - 1 - leadCount;

            //var p_d_dense = new Matrix(a.ColumnCount - 1, masterCount);

            var p_d = new CoordinateStorage<double>(a.ColumnCount - 1, masterCount, 1);

            var rightSideDense = new double[a.ColumnCount - 1];
            var rightSide= new double[a.ColumnCount - 1];

            var cnt = 0;
            var cnt3 = 0;

            for (var i = 0; i < a.ColumnCount - 1; i++)
            {
                if (leads[i] != -1)
                {
                    /*{//dense
                        var rw = top.ExtractRow(cnt).CoreArray;

                        var right = rw.Last();
                        Array.Resize(ref rw, rw.Length - 1);

                        for (var j = 0; j < rw.Length; j++)
                        {
                            p_d_dense[i, j] = -rw[j];
                        }

                        rightSideDense[i] = -right;
                    }*/

                    {//sparse

                        var right = 0.0;

                        //var rowNum = s.pinv[i];

                        foreach (var t in buft.EnumerateColumnMembers(cnt))//enumerate column members of transposed matrix
                        {
                            var rowNum = i;
                            var colNum = t.Item1;

                            
                            //var colNum = s.q[t.Item1];

                            var value = t.Item2;

                            if (t.Item1 != p_d.ColumnCount )
                                p_d.At(rowNum, colNum, -value);
                            else
                                right = value;
                        }

                        rightSide[i] = -right;
                        //rightSide[rowNum] = -right;
                    }

                   
                    cnt++;
                }
                else
                {
                    var t = cnt3++;

                    var rowNum = i;// s.pinv[ i];
                    var colNum = t;// s.q[ t];

                    p_d.At(rowNum, colNum, 1);
                }
            }

            //var top2 = p_d_dense;//.ToDenseMatrix();
            //var top3 = p_d.ToCCs().ToDenseMatrix();

            //var d = (top2 - top3);
            //var d2 = rightSide.Plus(rightSideDense, -1);


            var pdd = (CCS)p_d.ToCCs();

            return Tuple.Create(pdd, rightSide);

            var tol = 1e-6;

            throw new NotImplementedException();
        }

        CompressedColumnStorage<double> GetFactorR(SparseQR qr)
        {
            var info = typeof(SparseQR).GetField("R", BindingFlags.Instance | BindingFlags.NonPublic);

            return info.GetValue(qr) as CompressedColumnStorage<double>;
        }

        CompressedColumnStorage<double> GetFactorQ(SparseQR qr)
        {
            var info = typeof(SparseQR).GetField("Q", BindingFlags.Instance | BindingFlags.NonPublic);

            return info.GetValue(qr) as CompressedColumnStorage<double>;
        }

        SymbolicFactorization GetSymbolicFactorization(SparseQR qr)
        {
            var info = typeof(SparseQR).GetField("S", BindingFlags.Instance | BindingFlags.NonPublic);

            return info.GetValue(qr) as SymbolicFactorization;
        }
    }
}
