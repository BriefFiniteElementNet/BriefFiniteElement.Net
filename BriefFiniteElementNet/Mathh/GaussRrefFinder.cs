using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCS = CSparse.Double.CompressedColumnStorage;//<double>;
using BriefFiniteElementNet;

namespace BriefFiniteElementNet.Mathh
{
    public class GaussRrefFinder : IRrefFinder
    {
        public CCS CalculateRref(CCS a)
        {
            var sys = SparseEqSystem.Generate((CSparse.Double.CompressedColumnStorage)a);

            var dns = a.ToDenseMatrix();

            sys.RowsPool = new SortedSet<SparseRow>();

            var eqs = sys.Equations;

            for (var i = 0; i < sys.Equations.Length; i++)
                sys.Equations[i].Tag = i;

            var lastColIndex = a.ColumnCount-1;

            //should have n columns with one nonzero

            //var colPivotHistory = new HashSet<int>();
            //var rowPivotHistory = new HashSet<int>();
            var dependentRows = new bool[sys.RowCount];// HashSet<int>();
            var leadingMember = new bool[sys.RowCount];


            //var pivotedYet = new bool[a.RowCount];

            for (var i = 0; i < sys.Equations.Length; i++)
            {
                foreach(var tuple in sys.Equations[i].EnumerateIndexed())
                {
                    if (sys.ColumnNonzeros[tuple.Item1] == 1)
                        leadingMember[i] = true;
                }
            }


            var maxItter = Math.Max(a.ColumnCount, a.RowCount);

            var itter = 0;

            while (itter++ < maxItter)
            {
                //find count
                var oneNnzCols = sys.ColumnNonzeros.Count(i => i == 1);

                var eqNeeded = Math.Min(a.ColumnCount - 1, a.RowCount - dependentRows.Count(i => i == true));


                if (oneNnzCols == eqNeeded)
                    break;

                //select pivot
                //minimum row nnz x col nnz
                //select row strategy: row with minimum nnz, where nnz is nonzero count except last column (right side) and nnz > 1


                var minRowNnz = int.MaxValue;// sys.RowNonzeros.Where(i => i > 1).Min();
                var minRowNnzIndex = -1;// sys.RowNonzeros.FirstIndexOf(minRowNnz);

                {
                    for (var j = 0; j < sys.RowCount; j++)
                    {
                        if (leadingMember[j])
                            continue;

                        if (dependentRows[j])
                            continue;


                        //if (sys.RowNonzeros[j] <= 1)
                        //    continue;

                        if (sys.RowNonzeros[j] < minRowNnz)
                        {
                            minRowNnzIndex = j;
                            minRowNnz = sys.RowNonzeros[j];
                        }
                    }
                }

                if (minRowNnzIndex == -1)
                    throw new Exception();


                //var minColNnz = sys.ColumnNonzeros.Where(i => i > 1).Min();
                var minColNnz = int.MaxValue;// sys.ColumnNonzeros.Where(i => i > 1).Min();
                var minColNnzIndex = -1;


                {
                    for (var jj = 0; jj < sys.Equations[minRowNnzIndex].Size; jj++)
                    {
                        var j = sys.Equations[minRowNnzIndex].Indexes[jj];

                        //if (colPivotHistory.Contains(j))
                        //    continue;

                        if (sys.ColumnNonzeros[j] <= 1)
                            continue;

                        if (sys.ColumnNonzeros[j] < minColNnz)
                        {
                            minColNnzIndex = j;
                            minColNnz = sys.ColumnNonzeros[j];
                        }
                    }
                }

                //var minColNnzIndex = sys.ColumnNonzeros.FirstIndexOf(minColNnz);



                if (minColNnzIndex == -1)
                {
                    //var t = sys.ToCcs().ToDenseMatrix();

                    throw new Exception();
                }

                var col = minColNnzIndex;


                var c1 = sys.Equations.Where(i => i.ContainsIndex(col) && !leadingMember[i.Tag]);

                var rw =
                c1.MinBy(j => j.Size, null);
                //sys.Equations[minRowNnzIndex];

                if (dependentRows[sys.Equations.IndexOfReference(rw)])
                    throw new Exception();

                Console.WriteLine("Pivot: {0},{1}", sys.Equations.IndexOfReference(rw), col);

                //Console.ReadKey();
                // eqs.Equations.Where(i => i.Size > 1).MinBy(new SparseRowLengthComparer());

                var eliminator = rw;// sys.Equations[rw];

                //eliminate all rows with pivot
                for (var i = 0; i < sys.Equations.Length; i++)
                {
                    var canditate = eqs[i];

                    if (canditate.ContainsIndex(col) && !ReferenceEquals(eliminator, canditate))
                    {
                        var oldOne = eqs[i];//to be removed

                        foreach (var enm in oldOne.EnumerateIndexed())
                        {
                            sys.ColumnNonzeros[enm.Item1]--;
                        }

                        var newOne = SparseRow.Eliminate(eliminator, oldOne, col);

                        if (newOne.CalcNnz(lastColIndex) == 0 && newOne.GetRightSideValue(lastColIndex) != 0)
                        {
                            throw new Exception("Inconsistent system");
                        }

                        if (newOne.CalcNnz(lastColIndex) == 0 && newOne.GetRightSideValue(lastColIndex) == 0)
                        {
                            //fully zero equation
                            dependentRows[i] = true;// newOne.Tag);
                        }



                        newOne.Tag = oldOne.Tag;

                        foreach (var enm in newOne.EnumerateIndexed())
                        {
                            sys.ColumnNonzeros[enm.Item1]++;

                            if (sys.ColumnNonzeros[enm.Item1] == 1)
                                leadingMember[i] = true;
                        }


                        if (sys.ColumnNonzeros.Contains(0))
                            Guid.NewGuid();

                        eqs[i] = newOne;

                        sys.RowNonzeros[i] = newOne.CalcNnz(lastColIndex);
                    }

                }

                leadingMember[sys.Equations.IndexOfReference(rw)] = true;

                Console.WriteLine("elimination done for all eqs, Col[{0}] : {1} nnzs", col, sys.ColumnNonzeros[col]);

                if (sys.ColumnNonzeros[col] != 1)
                    Guid.NewGuid();
            }

            SparseEqSystem newSys;// = new SparseEqSystem();

            {
                ////////start final check

                var tol = 1e-9;

                var dependentEquationCount = sys.Equations.Count(i => i.CalcNnz(lastColIndex) == 0 && i.GetRightSideValue(lastColIndex).FEquals(0, tol));
                var incosistentEquationCount = sys.Equations.Count(i => i.CalcNnz(lastColIndex) == 0 && !i.GetRightSideValue(lastColIndex).FEquals(0, tol));
                var NnzMemberInCol = new int[sys.ColumnCount - 1];

                foreach (var eq in sys.Equations)
                {
                    foreach (var idx in eq.EnumerateIndexed())
                        if (idx.Item1 != lastColIndex)
                            if (!idx.Item2.FEquals(0, tol))
                                NnzMemberInCol[idx.Item1]++;
                }

                var leadingMembers = NnzMemberInCol.Count(i => i == 1);

                if (leadingMembers != Math.Min(sys.ColumnCount-1, sys.RowCount - dependentEquationCount) )
                    throw new Exception();

                if (incosistentEquationCount != 0)
                    throw new Exception();

                var allExceptDependentEqs = sys.Equations.Where(i => !(i.CalcNnz(lastColIndex) == 0 && i.GetRightSideValue(lastColIndex).FEquals(0, tol))).ToArray();

                newSys = SparseEqSystem.Generate(allExceptDependentEqs, a.ColumnCount);
            }

            return newSys.ToCcs();
        }

        

        public class SparseRowLengthComparer : IComparer<SparseRow>
        {
            public int Compare(SparseRow x, SparseRow y)
            {
                return x.Size.CompareTo(y.Size);
            }
        }
    }
}
