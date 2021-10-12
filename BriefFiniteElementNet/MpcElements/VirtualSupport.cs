using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSparse.Double;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace BriefFiniteElementNet.MpcElements
{
    /// <summary>
    /// Represents a virtual support for nodes.
    /// </summary>
    /// <remarks>
    /// This class is a virtual support for nodes in it. this also supports settlement.
    /// </remarks>
    /// <seealso cref="BriefFiniteElementNet.Elements.MpcElement" />
    [Serializable]
    public class VirtualConstraint : MpcElement
    {
        private Displacement _settlement;

        /// <summary>
        /// Gets or sets the settlement.
        /// </summary>
        /// <value>
        /// The settlement of support (if any).
        /// </value>
        public Displacement Settlement
        {
            get { return _settlement; }
            set { _settlement = value; }
        }

        private Constraint _constraint;

        /// <summary>
        /// Gets or sets the constraint.
        /// </summary>
        /// <value>
        /// The constraint of virtual support, applies to all nodes
        /// </value>
        public Constraint Constraint
        {
            get { return _constraint; }
            set { _constraint = value; }
        }


        public override SparseMatrix GetExtraEquations()
        {
            var n = parent.Nodes.Count;

            var buf = new CSparse.Storage.CoordinateStorage<double>(GetExtraEquationsCount(), parent.Nodes.Count * 6 + 1, 1);

            var cnt = 0;

            for (var i = 0; i < Nodes.Count; i++)
            {
                var nde = Nodes[i];

                var stIdx = nde.Index * 6;


                if (_constraint.DX == DofConstraint.Fixed)
                {
                    buf.At(cnt, stIdx + 0, 1);
                    buf.At(cnt, 6*n, _settlement.DX);
                    cnt++;
                }

                if (_constraint.DY == DofConstraint.Fixed)
                {
                    buf.At(cnt, stIdx + 1, 1);
                    buf.At(cnt, 6 * n, _settlement.DY);
                    cnt++;
                }

                if (_constraint.DZ == DofConstraint.Fixed)
                {
                    buf.At(cnt, stIdx + 2, 1);
                    buf.At(cnt, 6 * n, _settlement.DZ);
                    cnt++;
                }


                if (_constraint.RX == DofConstraint.Fixed)
                {
                    buf.At(cnt, stIdx + 3, 1);
                    buf.At(cnt, 6 * n, _settlement.DX);
                    cnt++;
                }

                if (_constraint.RY == DofConstraint.Fixed)
                {
                    buf.At(cnt, stIdx + 4, 1);
                    buf.At(cnt, 6 * n, _settlement.DX);
                    cnt++;
                }

                if (_constraint.RZ == DofConstraint.Fixed)
                {
                    buf.At(cnt, stIdx + 5, 1);
                    buf.At(cnt, 6 * n, _settlement.DX);
                    cnt++;
                }
            }

            return buf.ToCCs();
        }

        public override int GetExtraEquationsCount()
        {
            var arr = new DofConstraint[] {
            Constraint.DX,Constraint.DY,Constraint.DZ,
            Constraint.RX,Constraint.RY,Constraint.RZ};

            var restCount = arr.Count(i => i == DofConstraint.Fixed);

            return restCount * Nodes.Count;
        }

        #region ISerialization Implementation

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_settlement", _settlement);
            info.AddValue("_constraint", _constraint);
        }

        #endregion


        #region Constructor

        public VirtualConstraint()
        {
        }

        #endregion


        #region Deserialization Constructor

        protected VirtualConstraint(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _settlement = (Displacement)info.GetValue("_settlement", typeof(Displacement));
            _constraint = (Constraint)info.GetValue("_constraint", typeof(Constraint));
        }

        #endregion
    }
}
