using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Elo = BriefFiniteElementNet.ElementPermuteHelper.ElementLocalDof;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Integration;
using System.Collections.Generic;
using BriefFiniteElementNet.Elements.ElementHelpers;
using System.Linq;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents a hexahedral with isotropic material.
    /// </summary>
    [Serializable]
    [Obsolete("not fully implemented yet")]
    public class HexahedralElement : Element
    {
        public HexahedralElement() : base(8)
        {
        }

        private BaseMaterial _material;
        public BaseMaterial Material
        {
            get { return _material; }
            set { _material = value; }
        }

        /// <summary>
        /// the a, look at UpdateGeoMatrix()
        /// </summary>
        [NonSerialized]
        internal double[] a;

        /// <summary>
        /// The b, look at UpdateGeoMatrix()
        /// </summary>
        [NonSerialized]
        internal double[] b;

        /// <summary>
        /// The c, look at UpdateGeoMatrix()
        /// </summary>
        [NonSerialized]
        internal double[] c;

        /// <summary>
        /// The d, look at UpdateGeoMatrix()
        /// </summary>
        [NonSerialized]
        internal double[] d;

        /// <summary>
        /// The determinant, look at UpdateGeoMatrix()
        /// </summary>
        [NonSerialized]
        internal double det;

        /// <inheritdoc/>
        public override double[] IsoCoordsToLocalCoords(params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks the node order, throws exception if order is invalid
        /// </summary>
        private void CheckNodeOrder()
        {
            throw new NotImplementedException();
        }

        public override Matrix GetLambdaMatrix()
        {
            //local coor system is same as global one
            var buf = MatrixPool.Allocate(3, 3);

            buf[0, 0] = buf[1, 1] = buf[2, 2] = 1;

            return buf;
        }

        #region element methods
        public override IElementHelper[] GetHelpers()
        {
            var helpers = new List<IElementHelper>();

            {
                helpers.Add(new HexaHedralHelper());
            }

            return helpers.ToArray();
        }

        public Matrix GetLocalDampMatrix()
        {
            var helpers = this.GetHelpers();

            var buf = MatrixPool.Allocate(24, 24);

            for (var i = 0; i < helpers.Count(); i++)
            {
                var helper = helpers[i];

                var ki = helper.CalcLocalDampMatrix(this);// ComputeK(helper, transMatrix);

                var dofs = helper.GetDofOrder(this);

                for (var ii = 0; ii < dofs.Length; ii++)
                {
                    var bi = dofs[ii].NodeIndex * 6 + (int)dofs[ii].Dof;

                    for (var jj = 0; jj < dofs.Length; jj++)
                    {
                        var bj = dofs[jj].NodeIndex * 6 + (int)dofs[jj].Dof;

                        buf[bi, bj] += ki[ii, jj];
                    }
                }
            }
            return buf;
        }
        
        public override Matrix GetGlobalDampingMatrix()
        {
            CheckNodeOrder();
            return GetLocalDampMatrix();
        }

        public Matrix GetLocalMassMatrix()
        {
            var helpers = GetHelpers();

            var buf = MatrixPool.Allocate(24, 24);

            for (var i = 0; i < helpers.Count(); i++)
            {
                var helper = helpers[i];

                var ki = helper.CalcLocalMassMatrix(this);// ComputeK(helper, transMatrix);

                var dofs = helper.GetDofOrder(this);

                for (var ii = 0; ii < dofs.Length; ii++)
                {
                    var bi = dofs[ii].NodeIndex * 6 + (int)dofs[ii].Dof;

                    for (var jj = 0; jj < dofs.Length; jj++)
                    {
                        var bj = dofs[jj].NodeIndex * 6 + (int)dofs[jj].Dof;

                        buf[bi, bj] += ki[ii, jj];
                    }
                }
            }
            return buf;
        }

        public override Matrix GetGlobalMassMatrix()
        {
            CheckNodeOrder();

            return GetLocalMassMatrix();
        }

        public Matrix GetLocalStifnessMatrix()
        {
            var helpers = GetHelpers();

            var buf = MatrixPool.Allocate(24, 24); // 3*nodecount x 3*nodecount

            for (var i = 0; i < helpers.Length; i++)
            {
                var helper = helpers[i];

                var ki = helper.CalcLocalStiffnessMatrix(this);

                var dofs = helper.GetDofOrder(this);

                for (var ii = 0; ii < dofs.Length; ii++)
                {
                    var bi = dofs[ii].NodeIndex * 6 + (int)dofs[ii].Dof;

                    for (var jj = 0; jj < dofs.Length; jj++)
                    {
                        var bj = dofs[jj].NodeIndex * 6 + (int)dofs[jj].Dof;

                        buf[bi, bj] += ki[ii, jj];
                    }
                }
            }

            return buf;
        }

        public override Matrix GetGlobalStifnessMatrix()
        {
            CheckNodeOrder();
            return GetLocalStifnessMatrix();//local coords is same as global coords in hexahedral
        }

        #endregion

      
        public override Force[] GetGlobalEquivalentNodalLoads(ElementalLoad load)
        {
            var helpers = GetHelpers();

            var buf = new Force[nodes.Length];

            var t = GetTransformationManager();

            foreach (var helper in helpers)
            {
                var forces = helper.GetLocalEquivalentNodalLoads(this, load);

                for (var i = 0; i < buf.Length; i++)
                {
                    buf[i] = buf[i] + forces[i];
                }
            }

            for (var i = 0; i < buf.Length; i++)
                buf[i] = t.TransformLocalToGlobal(buf[i]);

            return buf;
        }

        #region Deserialization Constructor

        protected HexahedralElement(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _material = (BaseMaterial)info.GetValue("_material", typeof(int));
        }

        #endregion

        #region ISerialization Implementation

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_material", _material);
        }

        public override void GetGlobalStifnessMatrix(Matrix stiffness)
        {
            throw new NotImplementedException();
        }

        public override int GetGlobalStifnessMatrixDimensions()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
