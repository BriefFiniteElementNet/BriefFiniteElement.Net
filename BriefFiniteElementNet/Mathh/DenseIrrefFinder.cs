using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSparse.Double;
using CSparse.Ordering;
using CSparse.Storage;
using CCS = CSparse.Double.CompressedColumnStorage;//<double>;
namespace BriefFiniteElementNet.Mathh
{
    public class DenseIrrefFinder:IRrefFinder
    {
        public CCS CalculateRref(CCS a)
        {
            var singleMemberColumns = 00;

            var aTran = a.Transpose();

            var n = a.ColumnCount;

            for (var i = 0; i < n; i++)
                if (a.GetNnzcForColumn(i) == 1)
                    singleMemberColumns++;

            if (singleMemberColumns == a.RowCount)
                return a.Clonee();

            var pat = new CoordinateStorage<double>(a.RowCount, a.ColumnCount - 1, 1, true);
            var bCrd = new CoordinateStorage<double>(a.RowCount, a.ColumnCount - 1, 1, true);//a matrix without last column

            a.EnumerateMembers((row, col, val) =>
            {
                if (col != a.ColumnCount - 1)
                {
                    pat.At(row, col, 1);
                    bCrd.At(row, col, val);
                }
            });
            //nonzero pattern of a, except last column

            var b = bCrd.ToCCs();
            var bTran = b.Transpose();

            var patt = pat.ToCCs();
            var pattTr = patt.Transpose();


            var varGraph = pattTr.Multiply(patt);
            varGraph.Values.SetAllMembers(1);

            var eqnGraph = patt.Multiply(pattTr);
            eqnGraph.Values.SetAllMembers(1);



            if (!varGraph.IsSymmetric() || !eqnGraph.IsSymmetric())
                throw new Exception();

            var eqnParts = CalcUtil.EnumerateGraphParts(eqnGraph);

            var varParts = CalcUtil.EnumerateGraphParts(varGraph);

            var varVisited = new bool[varGraph.ColumnCount];

            foreach (var eqnPart in eqnParts)
            {
                //i'th independent system.
                var firstEq = eqnPart.First();

                int firstVarOfFirstEq = -1;

                /*
                patt.EnumerateRowMembers(firstEq, (row, col, val) => {
                if (firstValOfFirstEq != -1)
                    firstValOfFirstEq = col;
                );
                */


                pattTr.EnumerateColumnMembers(firstEq, (row, col, val) =>
                {
                    if (firstVarOfFirstEq == -1)
                        firstVarOfFirstEq = row;
                });

                //var firstVar = patt.EnumerateColumnMembers()

                var connectedVariables = CalcUtil.DepthFirstSearch(varGraph, varVisited, firstVarOfFirstEq);

                connectedVariables.Sort();

                var vp = new Dictionary<int,int>();//variablePermute

                //vp[a] = b, a: number of variable in original system
                // b: index of variable in connectedVariables

                for (var i = 0; i < connectedVariables.Count; i++)
                    vp[connectedVariables[i]] = i;


                var denseSubSytem = new Matrix(eqnPart.Count, connectedVariables.Count+1);////i'th independent system as dense

                var dns = aTran.ToDenseMatrix();

                for (var i = 0; i < eqnPart.Count; i++)
                {
                    //insert i'th eqn into denseSubSytem

                    var row = i;

                    var st = aTran.ColumnPointers[i];
                    var en = aTran.ColumnPointers[i + 1];

                    for (int j = st; j < en; j++)
                    {
                        var col = aTran.RowIndices[j];

                        if (col == a.ColumnCount - 1)
                            continue;

                        var val = aTran.Values[j];

                        
                        denseSubSytem[i, vp[col]] = val;

                    }
                }

            }

            throw new NotImplementedException();
        }

       

       
    }
}
