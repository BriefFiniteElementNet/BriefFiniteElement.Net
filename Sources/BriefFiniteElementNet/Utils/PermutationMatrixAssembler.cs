using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSparse.Storage;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a matrix assembler for assembling the permutation matrix for rigid rigidElement stuff
    /// </summary>
    [Obsolete("Use PermutationGenerator class instead")]
    public class PermutationMatrixAssembler
    {
        /// <summary>
        /// Determines whether the <see cref="Model"/> Needs the permutation for defined <see cref="loadCase"/> or not. (i.e. should model consider any <see cref="RigidElement"/> while analyzing the <see cref="loadCase"/> or not.)
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="loadCase">The load case.</param>
        /// <returns>true, if need to consider any <see cref="RigidElement"/>; false, otherwise</returns>
        public static bool NeedPermutation(Model model, LoadCase loadCase)
        {
            foreach (var elm in model.RigidElements)
            {
                if (IsAppliableRigidElement(elm, loadCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the permutation matrix for defined <see cref="model" /> for applying the defined <see cref="RigidElement"/>s in <see cref="model"/> in defined <see cref="loadCase"/>.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="loadCase">The load case.</param>
        /// <returns>The permutation matrix</returns>
        public CompressedColumnStorage<double> GetPermutationMatrix(Model model, LoadCase loadCase)
        {
            for (int i = 0; i < model.Nodes.Count; i++)
                model.Nodes[i].Index = i;


            var n = model.Nodes.Count;
            var masters = new int[n];

            for (var i = 0; i < n; i++)
            {
                masters[i] = i;
            }

            var masterCount = 0;

            #region filling the masters

            var distinctElements = GetDistinctRigidElements(model, loadCase);


            foreach (var elm in distinctElements)
            {
                if (elm.Count == 0)
                    continue;

                var elmMasterIndex = GetMasterNodeIndex(model, elm);

                for (var i = 0; i < elm.Count; i++)
                {
                    masters[elm[i]] = elmMasterIndex;
                }
            }

            #endregion

            for (var i = 0; i < n; i++)
            {
                if (masters[i] == i)
                    masterCount++;
            }


            var rMaster = new int[n];

            var cnt = 0;

            for (var i = 0; i < masters.Length; i++)
            {
                if (masters[i] == i)
                    rMaster[i] = cnt++;
                else
                    rMaster[i] = -1;
            }

            var buf = new CoordinateStorage<double>(6*n, 6*masterCount, 1);

            for (var i = 0; i < n; i++)
            {
                if (masters[i] == i)
                {
                    InsertPij(Matrix.Eye(6), rMaster[i], i, buf);
                }
                else
                {
                    var j = masters[i];
                    var pij = GetPij(model, j, i);

                    InsertPij(pij, rMaster[j], i, buf);
                }
            }


            return CSparse.Converter.ToCompressedColumnStorage(buf);
        }

        /// <summary>
        /// Chooses a mater node for the rigid element with specified <see cref="nodes"/>.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="nodes">The nodes of rigid elements.</param>
        /// <returns>Master node of the rigid element with specified <see cref="nodes"/>.</returns>
        private static int GetMasterNodeIndex(Model model, List<int> nodes)
        {
            var buf = -1;

            foreach (var node in nodes)
                if (model.Nodes[node].Constraints != Constraint.Released)
                {
                    buf = node;
                    break;
                }

            foreach (var node in nodes)
                if (model.Nodes[node].Constraints != Constraint.Released)
                    if (node != buf)
                        ExceptionHelper.Throw("MA20000");

            if (buf == -1)
                buf = nodes[0];


            return buf;
        }


        /// <summary>
        /// Determines whether the specified <see cref="rigidElement"/> should be considered with defined <see cref="loadCase"/> or not.
        /// </summary>
        /// <param name="rigidElement">The rigidElement.</param>
        /// <param name="loadCase">The load case.</param>
        /// <returns></returns>
        private static bool IsAppliableRigidElement(RigidElement rigidElement, LoadCase loadCase)
        {

            return true;

            if (rigidElement.UseForAllLoads)
                return true;

            foreach (var lCase in rigidElement.AppliedLoadCases)
            {
                if (lCase.LoadType == loadCase.LoadType)
                    return true;

                if (lCase.Equals(loadCase))
                    return true;
            }

            return false;
        }


        private List<List<int>> GetDistinctRigidElements(Model model, LoadCase loadCase)
        {
            for (int i = 0; i < model.Nodes.Count; i++)
                model.Nodes[i].Index = i;

            var n = model.Nodes.Count;

            var crd = new CoordinateStorage<double>(n, n, 1);

            foreach (var elm in model.RigidElements)
            {
                if (IsAppliableRigidElement(elm, loadCase))
                {
                    for (var i = 0; i < elm.Nodes.Count; i++)
                    {
                        crd.At(elm.Nodes[i].Index, elm.Nodes[i].Index, 1.0);
                    }

                    for (var i = 0; i < elm.Nodes.Count - 1; i++)
                    {
                        crd.At(elm.Nodes[i].Index, elm.Nodes[i + 1].Index, 1.0);
                        crd.At(elm.Nodes[i + 1].Index, elm.Nodes[i].Index, 1.0);
                    }
                }
            }

            var graph = CSparse.Converter.ToCompressedColumnStorage(crd);

            var buf = CalcUtil.EnumerateGraphParts(graph);

            return buf;
        }

        public CompressedColumnStorage<double> GetInvertPermutationMatrix(Model model, LoadCase loadCase)
        {
            var n = model.Nodes.Count;
            var masters = new int[n];

            for (var i = 0; i < n; i++)
            {
                masters[i] = i;
            }

            #region filling the masters

            foreach (var elm in model.RigidElements)
            {
                var apply = false;

                if (elm.UseForAllLoads)
                    apply = true;
                else
                {
                    foreach (var cse in elm.AppliedLoadCases)
                    {
                        if (cse.Equals(loadCase))
                        {
                            apply = true;
                            break;
                        }
                    }
                }


                if (apply)
                {
                    if (elm.Nodes.Count == 0)
                        continue;

                    var elmMasterIndex = elm.Nodes[0].Index;

                    for (var i = 0; i < elm.Nodes.Count; i++)
                    {
                        masters[elm.Nodes[i].Index] = elmMasterIndex;
                    }
                }
            }

            #endregion

            var masterCount = 0;

            for (var i = 0; i < n; i++)
            {
                if (masters[i] == i)
                    masterCount++;
            }

            var buf = new CoordinateStorage<double>(6*n, masterCount, masterCount);

            for (var i = 0; i < n; i++)
            {
                if (masters[i] == i)
                {
                    InsertPij(Matrix.Eye(6), i, i, buf);
                }
            }


            return CSparse.Converter.ToCompressedColumnStorage(buf).Transpose();
        }

        /// <summary>
        /// Gets the permutation matrix for linking <see cref="master"/> and <see cref="slave"/> nodes together.
        /// </summary>
        /// <param name="model">The Model.</param>
        /// <param name="master">The master node index.</param>
        /// <param name="slave">The slave node index.</param>
        /// <returns>the permutation matrix</returns>
        public Matrix GetPij(Model model, int master, int slave)
        {
            var buf = Matrix.Eye(6);

            var d = model.Nodes[slave].Location - model.Nodes[master].Location;

            buf[0, 4] = -d.Z;
            buf[0, 5] = d.Y;

            buf[1, 3] = d.Z;
            buf[1, 5] = -d.X;

            buf[2, 3] = d.Y;
            buf[2, 4] = d.X;

            return buf;
        }

        /// <summary>
        /// Inserts the permutation matrix of linking the <see cref="master"/> and <see cref="slave"/> nodes to each other.
        /// </summary>
        /// <param name="pij">The pij.</param>
        /// <param name="master">The master node index.</param>
        /// <param name="slave">The slave node index.</param>
        /// <param name="pt">The global permutation assembly.</param>
        public void InsertPij(Matrix pij, int master, int slave, CoordinateStorage<double> pt)
        {
            var row0 = slave*6;
            var col0 = master*6;

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    pt.At(row0 + i, col0 + j, pij[i, j]);
                }
            }
        }
    }
}