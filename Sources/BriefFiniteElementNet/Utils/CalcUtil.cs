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

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a utility for graphs!
    /// </summary>
    public static class CalcUtil
    {

        


        public static void Swap<T>(ref T a, ref T b) where T : struct
        {
            var t = a;
            a = b;
            b = t;
        }

        /// <summary>
        /// Creates a built in solver appropriated with <see cref="tp"/>.
        /// </summary>
        /// <param name="type">The solver type.</param>
        /// <returns></returns>
        public static ISolver CreateBuiltInSolver(BuiltInSolverType type, SparseMatrix A)
        {
            switch (type)
            {
                case BuiltInSolverType.CholeskyDecomposition:
                    return new CholeskySolverFactory().CreateSolver(A);
                case BuiltInSolverType.ConjugateGradient:
                    return new ConjugateGradientFactory().CreateSolver(A);// PCG(new SSOR());
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }


        public static ISolverFactory CreateBuiltInSolverFactory(BuiltInSolverType type)
        {
            switch (type)
            {
                case BuiltInSolverType.CholeskyDecomposition:
                    return new CholeskySolverFactory();
                /**/
                case BuiltInSolverType.ConjugateGradient:
                    return new ConjugateGradientFactory();// PCG(new SSOR());
                case BuiltInSolverType.Lu:
                    return new ConjugateGradientFactory();// PCG(new SSOR());
                /**/
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        // TODO: LEGACY CODE - GetHashCode (Point), move out of main assembly?

        public static int GetHashCode(params Point[] objects)
        {
            if (objects == null)
                throw new ArgumentNullException("objects");

            if (objects.Length == 0)
                return 0;

            var buf = objects[0].GetHashCode();

            for (var i = 1; i < objects.Length; i++)
            {
                buf = (buf * 390) ^ (objects[i].GetHashCode());
            }

            buf = (buf * 390) ^ objects.Length;

            return buf;
        }

        public static int[] GetMasterMapping(Model model, LoadCase cse)
        {
            for (int i = 0; i < model.Nodes.Count; i++)
                model.Nodes[i].Index = i;


            var n = model.Nodes.Count;
            var masters = new int[n];

            for (var i = 0; i < n; i++)
            {
                masters[i] = i;
            }

            //var masterCount = 0;

            #region filling the masters

            var distinctElements = GetDistinctRigidElements(model, cse);

            var centralNodePrefreation = new bool[n];


            foreach (var elm in model.RigidElements)
            {
                if (elm.CentralNode != null)
                    centralNodePrefreation[elm.CentralNode.Index] = true;
            }


            foreach (var elm in distinctElements)
            {
                if (elm.Count == 0)
                    continue;

                var elmMasterIndex = GetMasterNodeIndex(model, elm, centralNodePrefreation);

                for (var i = 0; i < elm.Count; i++)
                {
                    masters[elm[i]] = elmMasterIndex;
                }
            }

            #endregion

            return masters;
        }

        private static List<List<int>> GetDistinctRigidElements(Model model, LoadCase loadCase)
        {
            for (int i = 0; i < model.Nodes.Count; i++)
                model.Nodes[i].Index = i;

            var n = model.Nodes.Count;

            var ecrd = new CoordinateStorage<double>(n, n, 1);//for storing existence of rigid elements
            var crd = new CoordinateStorage<double>(n, n, 1);//for storing hinged connection of rigid elements

            for (int ii = 0; ii < model.RigidElements.Count; ii++)
            {
                var elm = model.RigidElements[ii];

                if (IsAppliableRigidElement(elm, loadCase))
                {
                    for (var i = 0; i < elm.Nodes.Count; i++)
                    {
                        ecrd.At(elm.Nodes[i].Index, elm.Nodes[i].Index, 1.0);
                    }

                    for (var i = 0; i < elm.Nodes.Count - 1; i++)
                    {
                        ecrd.At(elm.Nodes[i].Index, elm.Nodes[i + 1].Index, 1.0);
                        ecrd.At(elm.Nodes[i + 1].Index, elm.Nodes[i].Index, 1.0);
                    }
                }
            }

            var graph = SparseMatrix.OfIndexed(ecrd);

            var buf = BriefFiniteElementNet.Utils.GraphUtils.EnumerateGraphParts(graph);

            return buf;
        }


        private static bool IsAppliableRigidElement(RigidElement elm, LoadCase loadCase)
        {
            if (elm.UseForAllLoads)
                return true;

            if (elm.AppliedLoadTypes.Contains(loadCase.LoadType))
                return true;

            if (elm.AppliedLoadCases.Contains(loadCase))
                return true;

            return false;
        }

        private static int GetMasterNodeIndex(Model model, List<int> nodes, bool[] preferation)
        {
            var buf = -1;

            foreach (var node in nodes)
                if (preferation[node])
                {
                    buf = node;
                    break;
                }

            if (buf == -1)
                foreach (var node in nodes)
                    if (model.Nodes[node].Constraints != Constraints.Released)
                    {
                        buf = node;
                        break;
                    }

            foreach (var node in nodes)
                if (model.Nodes[node].Constraints != Constraints.Released)
                    if (node != buf)
                        ExceptionHelper.Throw("MA20000");

            if (buf == -1)
                buf = nodes[0];


            return buf;
        }

        public static int Sum(this Constraint ctx)
        {
            return (int)ctx.DX + (int)ctx.DY + (int)ctx.DZ +
                   (int)ctx.RX + (int)ctx.RY + (int)ctx.RZ;
        }

        public static DofConstraint[] ToArray(this Constraint ctx)
        {
            return new DofConstraint[] { ctx.DX, ctx.DY, ctx.DZ, ctx.RX, ctx.RY, ctx.RZ };
        }

        /// <summary>
        /// cacluates the A = B' D B.
        /// </summary>
        /// <param name="B">The B.</param>
        /// <param name="D">The D.</param>
        /// <param name="bi_ColCount">The column count of B[i].</param>
        /// <param name="result">The result.</param>
        public static void Bt_D_B(Matrix B, Matrix D, Matrix result)
        {
            //method 1 is divide B into smallers
            //method 2 is: B' D B = (D' B)' B
            // first find D' B which is possible high performance and call it R1
            // find R1' B which is possible high performance too!

            var dd = B.RowCount;//dim of b
            var dOut = B.ColumnCount;// dim of out

            if (!D.IsSquare() || D.RowCount != dd)
                throw new Exception();

            var buf = result;

            if (buf.RowCount != dOut || buf.ColumnCount != dOut)
                throw new Exception();

            var buf1 =
                new Matrix(D.ColumnCount, B.ColumnCount);
            //MatrixPool.Allocate(D.ColumnCount, B.ColumnCount);

            D.TransposeMultiply(B, buf1);

            buf1.TransposeMultiply(B, buf);
        }

        public static bool IsIsotropicMaterial(AnisotropicMaterialInfo inf)
        {
            var arr1 = new double[] { inf.Ex, inf.Ey, inf.Ez };
            var arr2 = new double[]
            {
                inf.NuXy, inf.NuYx,
                inf.NuXz, inf.NuZx,
                inf.NuZy, inf.NuYz
            };

            return arr1.Distinct().Count() == 1 && arr2.Distinct().Count() == 1;
        }

       


        internal static int FixedCount(Constraint cns)
        {
            var buf = 0;

            if (cns.DX == DofConstraint.Fixed)
                buf++;

            if (cns.DY == DofConstraint.Fixed)
                buf++;

            if (cns.DZ == DofConstraint.Fixed)
                buf++;

            if (cns.RX == DofConstraint.Fixed)
                buf++;

            if (cns.RY == DofConstraint.Fixed)
                buf++;

            if (cns.RZ == DofConstraint.Fixed)
                buf++;

            return buf;
        }




    }
}