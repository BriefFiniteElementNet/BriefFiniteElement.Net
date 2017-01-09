using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSparse;
using CSparse.Storage;
using CCS = CSparse.Double.CompressedColumnStorage;
using Coord = CSparse.Storage.CoordinateStorage<double>;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a class for generating permutation matrix for rigi element stuff
    /// </summary>
    public class PermutationGenerator
    {
        public static CCS GetDisplacementPermute(Model model, DofMappingManager mgr)
        {
            var buf = new PermutationGenerator();
            buf._target = model;
            buf.DofMap = mgr;
            buf._m = mgr.M;
            buf._n = mgr.N;

            return buf.GetDisplacemenetPermutation();
        }

        public static CCS GetForcePermute(Model model,  DofMappingManager mgr)
        {
            var buf = new PermutationGenerator();
            buf._target = model;
            buf.DofMap = mgr;
            buf._m = mgr.M;
            buf._n = mgr.N;

            return buf.GetForcePermutation();
        }

        private PermutationGenerator()
        {
        }

        public DofMappingManager DofMap;

        /// <summary>
        /// Gets the force permutation (Pf).
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>force permutation matrix</returns>
        public CCS GetForcePermutation()
        {
            var buf = GetDisplacemenetPermutation();
            buf = (CCS)buf.Transpose();
            
            return buf;
        }

        /// <summary>
        /// Gets the displacement permutation (Pd).
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>displacement permutation matrix</returns>
        public CCS GetDisplacemenetPermutation()
        {
            var buf = new Coord(6*_n, 6*_m, 0);

            for (var i = 0; i < _n; i++)
            {
                var master = DofMap.MasterMap[i];
                var pd = master == i ? Matrix.Eye(6) : GetPdij(master, i);

                var j = DofMap.RMasterMap[master];

                InsertSubmatrix(pd, i, j, buf);
            }

            var buff = (CCS) Converter.ToCompressedColumnStorage(buf);

            return buff;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PermutationGenerator"/> class.
        /// </summary>
        /// <param name="targetModel">The target model.</param>
        public PermutationGenerator(Model targetModel)
        {
            _target = targetModel;

            //SetMasterMapping();
        }

        private static void InsertSubmatrix(int row, int column, Coord coord, Matrix subMatrix)
        {
            throw new NotImplementedException();
        }


        private int _n;

        /// <summary>
        /// The master count
        /// </summary>
        private int _m;
        //private int[] _masterMap;
        //private int[] _rMasterMap;
        private Model _target;


        private List<List<int>> GetDistinctRigidElements(Model model)
        {
            for (int i = 0; i < model.Nodes.Count; i++)
                model.Nodes[i].Index = i;

            var n = model.Nodes.Count;

            var crd = new CoordinateStorage<double>(n, n, 1);

            foreach (var elm in model.RigidElements)
            {
                //if (IsAppliableRigidElement(elm, loadCase))
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

            var graph = Converter.ToCompressedColumnStorage(crd);

            var buf = CalcUtil.EnumerateGraphParts(graph);

            return buf;
        }

        private void SetMasterMapping()
        {
            var model = _target;

            for (int i = 0; i < model.Nodes.Count; i++)
                model.Nodes[i].Index = i;


            var n = _n = model.Nodes.Count;
            var masters =  new int[n];

            for (var i = 0; i < n; i++)
            {
                masters[i] = i;
            }

            var masterCount = 0;

            #region filling the masters

            var distinctElements = GetDistinctRigidElements(model);


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

            _m = masterCount;
        }


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
        /// Gets the force permutation matrix.
        /// </summary>
        /// <returns>force permutation matrix</returns>
        public CCS GetForcePermutationMatrix()
        {
            var _masterMap = DofMap.MasterMap;

            for (var i = 0; i < _n; i++)
            {
                if (_masterMap[i] == i)
                    continue;
                
                var pfij = GetPfij(_masterMap[i], i);


            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the Pf (force permutation) for the slave node (transfers the forces and momnets on slave node into forces and moments on the master node).
        /// </summary>
        /// <param name="master">The master index.</param>
        /// <param name="slave">The slave index.</param>
        /// <returns></returns>
        private Matrix GetPfij(int master, int slave)
        {
            var _masterMap = DofMap.MasterMap;

            if (_masterMap[slave] != master)
                throw new InvalidOperationException();

            var d = _target.Nodes[master].Location - _target.Nodes[slave].Location;

            var buf = Matrix.Eye(6);

            buf[3, 1] = -(buf[4, 0] = d.Z);
            buf[5, 0] = -(buf[3, 2] = d.Y);
            buf[4, 2] = -(buf[5, 1] = d.X);

            return buf;
        }

        /// <summary>
        /// Gets the Pd (displacement permutation) for the slave node (moves the master node displacement and rotations into the slave node).
        /// </summary>
        /// <param name="master">The master index.</param>
        /// <param name="slave">The slave index.</param>
        /// <returns></returns>
        private Matrix GetPdij(int master, int slave)
        {
            return GetPdij(master, slave, false);
        }

        private Matrix GetPdij(int master, int slave, bool hinged)
        {
            //Note: this is transpose of Pfij
            var _masterMap = DofMap.MasterMap;

            if (_masterMap[slave] != master)
                throw new InvalidOperationException();

            var d = _target.Nodes[master].Location - _target.Nodes[slave].Location;

            var buf = Matrix.Eye(6);

            buf[0, 4] = -(buf[1, 3] = d.Z);
            buf[2, 3] = -(buf[0, 5] = d.Y);
            buf[1, 5] = -(buf[2, 4] = d.X);

            return buf;
        }

        private static void InsertSubmatrix(Matrix submatrix, int row, int column, Coord coordinatedStorage)
        {
            if (submatrix.RowCount != 6 || submatrix.ColumnCount != 6)
                throw new InvalidOperationException();

            for (var i = 0; i < 6; i++)
            {
                for (var j = 0; j < 6; j++)
                {
                    coordinatedStorage.At(i + 6*row, j + 6*column, submatrix[i, j]);
                }
            }
        }
    }
}