using BriefFiniteElementNet;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.MpcElements;
using BriefFiniteElementNet.Sections;
using BriefFiniteElementNet.Utils;
using BriefFiniteElementNet.Validation.OpenseesTclGenerator;
using CSparse;
using CSparse.Double;
using CSparse.Double.Factorization;
using CSparse.Storage;
using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TestingConsole
{
    internal class NewSolverEfforts
    {

        private static void DeterminePrimarySecondary(SparseMatrix j, out int[] perm, out int rj)
        {
            //perm:
            //first n-rj members which are only nnz in their row and col are secondary
            //then rj primary

            //J should be full rank (no row duplicates)

            rj = j.RowCount;
            var n = j.ColumnCount;

            var rowNnzCount = new int[rj];


            var colNnzCount = new int[n];
            var colNnzMember = new int[n];//row index of last nnz of column[i]

            {
                foreach (var item in j.EnumerateIndexed())
                {
                    rowNnzCount[item.Item1]++;
                    colNnzCount[item.Item2]++;
                    colNnzMember[item.Item2] = item.Item1;
                }
            }

            var rowFlags = new bool[rj];
            //var lst1 = new List<int>();
            //var lst2 = new List<int>();


            perm = new int[n];

            var c1 = 0;
            var c2 = 0;

            for (var col = 0; col < n; col++)
            {

                var flag = false;

                if (colNnzCount[col] != 1)
                {
                    flag = true;
                }

                var nnzRow = colNnzMember[col];

                if (rowFlags[nnzRow])
                {
                    flag = true;
                }

                if (!flag)
                {
                    perm[col] = c1++;
                    rowFlags[nnzRow] = true;
                }
                else
                {
                    perm[col] = rj + c2++;
                }
            }


            return;

            /*
            var newJ = j.PermuteColumns(perm);

            var J = BriefFiniteElementNet.Mathh.Extensions.ToDense(j);
            var nJ = BriefFiniteElementNet.Mathh.Extensions.ToDense(newJ);

            var tmp = nJ.ToString();

            throw new NotImplementedException();
            */

        }


        public static SparseMatrix PermuteColumns(int[] permuta, SparseMatrix mtx)
        {
            var l = permuta.Count(i => i != -1);

            var kt = new CoordinateStorage<double>(mtx.RowCount, l, mtx.NonZerosCount);

            {
                foreach (var item in mtx.EnumerateIndexed())
                {
                    var oldCol = item.Item2;

                    if (oldCol >= permuta.Length)
                        continue;

                    var newCol = permuta[oldCol];

                    if (newCol == -1)
                        continue;

                    kt.At(item.Item1, newCol, item.Item3);
                }
            }

            var buf = kt.ToCCs();

            return buf;
        }

        private static T[] RentArray<T>(int n)
        {
            return new T[n];
        }

        private static void ReturnArray<T>(T[] array)
        {
        }


        public static void Do()
        {
            var dim = 50;
            var useMpc = false;

            //for (int i = 0; i < 1; (i)++)
            {


            
                var model = GetModel2(dim, useMpc);

                model.Trace.Listeners.Add(new BriefFiniteElementNet.Common.ConsoleTraceListener());

                var loadCase = LoadCase.DefaultLoadCase;

                var tmr = System.Diagnostics.Stopwatch.StartNew();

                var solver = new NewSolver();

                solver.Init(model);

                //tmr.Restart();

                //solver.Solve();

                if (false)
                {
                    var e1 = tmr.ElapsedMilliseconds;

                    Console.WriteLine("New Solve: " + e1);
                    tmr.Restart();

                    model.Solve_MPC();

                    var e2 = tmr.ElapsedMilliseconds;

                    Console.WriteLine("Solve_MPC(): " + e2);

                    var e3 = 0l;

                    if (!model.MpcElements.Any())
                    {
                        tmr.Restart();

                        model.Solve();

                        e3 = tmr.ElapsedMilliseconds;
                        Console.WriteLine("Solve(): " + e3);
                    }
                }

                var openSees = true;



                var numberers = new string[] { "Plain", "RCM" };

                var systems = new string[] {"SparseSYM", "ProfileSPD" , "UmfPack" };

                /*

                if (openSees)
                    foreach (var ste in systems)
                        foreach (var nmb in numberers)
                            RunOpenSees(model, ste, nmb, true);
                */

            }


        }


        private static void RunOpenSees(Model model,string system,string numberer,bool shellOutput=true)
        {

            Console.WriteLine(string.Format("{0}, {1}", system, numberer));

            var sp = System.Diagnostics.Stopwatch.StartNew();

            var gen = new TclGenerator();

            gen.ElementTranslators.Add(new BarElement2Tcl() { TargetGenerator = gen });
            gen.ElementTranslators.Add(new TetrahedronToTcl() { TargetGenerator = gen });
            gen.ElementTranslators.Add(new TriangleElementToTri3Tcl() { TargetGenerator = gen });
            gen.ElementTranslators.Add(new TetrahedralElementToTcl() { TargetGenerator = gen });

            gen.ElementLoadTranslators.Add(new UniformLoad2Tcl() { TargetGenerator = gen });

            gen.ExportElementForces = false;
            gen.ExportNodalDisplacements = false;
            gen.ExportTotalStiffness = false;
            gen.ExportNodalReactions = false;
            //gen.CustomNumberer = "RCM";
            gen.CustomSystem = system;
            gen.CustomNumberer = numberer;

            var tcl = gen.Create(model, LoadCase.DefaultLoadCase);

            var nodesFile = gen.nodesOut;
            var elementsFile = gen.elementsOut;
            var stiffnessFile = gen.stiffnessOut;

            var tclFile = System.IO.Path.GetTempFileName() + ".tcl";

            Debug.WriteLine("{0}", tclFile);

            System.IO.File.WriteAllText(tclFile, tcl);

            var openSeeslocation = @"C:\opensees\bin\opensees.exe";

            if (!System.IO.File.Exists(openSeeslocation))
                throw new Exception("Opensees.exe not found, please put opensees.exe in this path: " + openSeeslocation);

            var stInf = new ProcessStartInfo(openSeeslocation);

            stInf.Arguments = tclFile;
            stInf.RedirectStandardOutput = shellOutput;
            stInf.RedirectStandardError = shellOutput;
            stInf.UseShellExecute = false;

            
            var prc = Process.Start(stInf);

            prc.WaitForExit();
            var code = prc.ExitCode;

            sp.Stop();

            Console.WriteLine($"Opensees took {sp.ElapsedMilliseconds} ms");
        }

        private static SparseMatrix PermuteK(SparseMatrix k, int[] Qt, int[] Q)
        {
            //calculates Qt.K.Q

            var ccs = new CoordinateStorage<double>(k.RowCount, k.ColumnCount, k.NonZerosCount);

            foreach (var item in k.EnumerateIndexed())
            {
                var oldRow = item.Item1;
                var oldCol = item.Item2;
                var val = item.Item3;

                var newRow = Q[oldRow];
                var newCol = Qt[oldCol];

                ccs.At(newRow, newCol, val);
            }

            return (SparseMatrix)ccs.ToCCs();
        }

        private static void PermuteArray(double[] input, double[] output, int[] perm)
        {
            
            for (int i = 0; i < perm.Length; i++)
            {
                output[perm[i]] = input[i];
            }
        }

        private static SparseMatrix ToPermutationMatrix(int[] permutation)
        {
            var max = permutation.Max();

            var a11 = new CoordinateStorage<double>(permutation.Length, max+1, permutation.Length);

            for (int i = 0; i < permutation.Length; i++)
            {
                a11.At(i, permutation[i], 1);
            }

            return a11.ToCCs();
        }

        private static SparseMatrix VertConcat(SparseMatrix a, SparseMatrix b)
        {
            if (a.ColumnCount != b.ColumnCount)
                throw new Exception();

            var ar = a.RowCount;
            var ac = a.ColumnCount;

            var br = b.RowCount;

            var cc = new CoordinateStorage<double>(a.RowCount+b.RowCount, a.ColumnCount, a.NonZerosCount + b.NonZerosCount);

            foreach (var item in a.EnumerateIndexed())
            {
                cc.At(item.Item1, item.Item2, item.Item3);
            }

            foreach (var item in b.EnumerateIndexed())
            {
                cc.At(item.Item1 + ar, item.Item2, item.Item3);
            }

            return cc.ToCCs();
        }

        private static void Split(double[] a, int row1,
            out double[] a1,
            out double[] a2)
        {
            a1 = new double[row1];
            a2 = new double[a.Length - row1];

            Array.Copy(a, 0, a1, 0, a1.Length);
            Array.Copy(a, a1.Length, a2, 0, a1.Length);
        }

        private static void Split(SparseMatrix a, int row1,int col1,
            out SparseMatrix A11, 
            out SparseMatrix A12, 
            out SparseMatrix A21, 
            out SparseMatrix A22)
        {
            var row2 = a.RowCount - row1;
            var col2 = a.ColumnCount - col1;

            var a11 = new CoordinateStorage<double>(row1, col1, 1);
            var a12 = new CoordinateStorage<double>(row1, col2, 1);
            var a21 = new CoordinateStorage<double>(row2, col1, 1);
            var a22 = new CoordinateStorage<double>(row2, col2, 1);


            foreach (var item in a.EnumerateIndexed())
            {
                var row = item.Item1;
                var col = item.Item2;
                var val = item.Item3;

                if (row < row1)
                {
                    if (col < col1)
                        a11.At(row, col, val);
                    else
                        a12.At(row, col - col1, val);
                }
                else
                {
                    if (col < col1)
                        a21.At(row-row1, col, val);
                    else
                        a22.At(row - row1, col - col1, val);
                }
            }


            A11 = a11.ToCCs();
            A12 = a12.ToCCs();
            A21 = a21.ToCCs();
            A22 = a22.ToCCs();
        }


        private static SparseMatrix SpecialInvert(SparseMatrix matrix)
        {
            var buf = (SparseMatrix)matrix.Transpose();

            var rs = new int[matrix.RowCount];
            var cs = new int[matrix.ColumnCount];


            foreach (var item in matrix.EnumerateIndexed())
            {
                rs[item.Item1]++;
                cs[item.Item2]++;
            }


            if (rs.Any(i => i != 1))
                throw new Exception();

            if (cs.Any(i => i != 1))
                throw new Exception();

            for (int i = 0; i < matrix.Values.Length; i++)
            {
                buf.Values[i] = 1 / buf.Values[i];
            }

            return buf;
        }

        private static int[] InvertPermutation(int[] perm,int max=-1)
        {
            if (max == -1)
                max = perm.Max()+1;

            var buf = new int[max];

            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = -1;
            }


            for (int i = 0; i < perm.Length; i++)
            {
                buf[perm[i]] = i;
            }

            return buf;
        }

        public static string ToString(MathNet.Numerics.LinearAlgebra.Double.DenseMatrix mtx)
        {
            var bldr = new StringBuilder();

            for (int i = 0; i < mtx.RowCount; i++)
            {
                for (int j = 0; j < mtx.ColumnCount; j++)
                {
                    var val = mtx[i, j];

                    bldr.AppendFormat("{0}\t", val == 0.0 ? "-" : val.ToString());
                }

                bldr.AppendLine();
            }

            return bldr.ToString();
        }

        public static SparseMatrix GetJ(Model target)
        {
            var loadCase = LoadCase.DefaultLoadCase;

            var n = target.Nodes.Count * 6;
            var totDofCount = n;


            #region step 1
            var extraEqCount = 0;

            foreach (var mpcElm in target.MpcElements)
                if (mpcElm.AppliesForLoadCase(loadCase))
                    extraEqCount += mpcElm.GetExtraEquationsCount();

            //extraEqCount += boundaryConditions.RowCount;


            var totalEqCount = extraEqCount ;


            var allEqsCrd = new CoordinateStorage<double>(totalEqCount, n , 1);//rows: extra eqs, cols: 6*n+1 (+1 is for right hand side)

            var lastRow = 0;

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

            var allEqs = (SparseMatrix)SparseMatrix.OfIndexed(allEqsCrd, true);


            return allEqs;

            #endregion
        }


        public static Model GetModel2(int dim, bool addMpc)
        {
            var model = StructureGenerator.Generate3DBarElementGrid(dim, dim, dim);

            StructureGenerator.AddRandomiseLoading(model, true, false, LoadCase.DefaultLoadCase);

            model.ReIndexNodes();

            var nc = model.Nodes.Count;

            var ns1 = Enumerable.Range(nc / 10, 3 * nc / 4);

            if (addMpc)
            {
                var m1 = new RigidElement_MPC();
                m1.Nodes.AddRange(ns1.Select(i => model.Nodes[i]));

                m1.UseForAllLoads = true;

                model.MpcElements.Add(m1);


                m1.Nodes[m1.Nodes.Count / 2].Constraints = Constraints.Fixed;
            }
         
            return model;
        }

        public static Model GetModel()
        {
            var model = new Model();

            var l0 = 1;
            var l1 = 1.5;

            var l2 = 2;
            var l3 = 3;

            var n0 = new Node(0, 0, 0);
            var n1 = new Node(l0, 0, 0);
            var n2 = new Node(l1 + l0, 0, 0);
            var n3 = new Node(l2 + l1 + l0, 0, 0);
            var n4 = new Node(l3 + l2 + l1 + l0, 0, 0);

            var sec = new UniformParametric1DSection();

            sec.A = 1;
            sec.Iy = 2;
            sec.Iz = 3;
            sec.J = 4;
            var e = 210e9;

            var mat = UniformIsotropicMaterial.CreateFromYoungPoisson(e, 0.25);

            n0.Constraints = Constraints.Fixed;

            {
                var c3 = Constraints.Fixed;
                c3.RZ = DofConstraint.Released;
                n3.Constraints = c3;
            }

            {
                var c4 = Constraints.Fixed;
                c4.DY = DofConstraint.Released;
                n4.Constraints = c4;
            }

            model.Nodes.Add(n0, n1, n2, n3, n4);

            var e1 = new BarElement(n0, n1);
            var e2 = new BarElement(n2, n3);


            e1.Material = e2.Material = mat;
            e1.Section = e2.Section = sec;

            var me1 = new RigidElement_MPC();
            var me2 = new RigidElement_MPC();

            me1.Nodes.Add(n1);
            me1.Nodes.Add(n2);

            me2.Nodes.Add(n3);
            me2.Nodes.Add(n4);

            me1.UseForAllLoads = true;
            me2.UseForAllLoads = true;

            

            model.Elements.Add(e1);
            model.Elements.Add(e2);


            model.MpcElements.Add(me1);
            model.MpcElements.Add(me2);


            for (var i = 0; i < model.Nodes.Count; i++)
            {
                var f = RandomHelper.GetRandomForce(-100, 100);

                model.Nodes[i].Loads.Add(new NodalLoad(f));


                var disp = RandomHelper.GetRandomDisplacement(-0.0100, 0.0100);

                model.Nodes[i].Settlements.Add(new NodalSettlement(disp));
            }

            model.ReIndexNodes();

            return model;
        }
    }
}
