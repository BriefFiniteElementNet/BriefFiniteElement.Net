using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSparse.Double;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents a virtual support for nodes.
    /// </summary>
    /// <remarks>
    /// This class is a vitual support for nodes in it. this also supports settlement.
    /// </remarks>
    /// <seealso cref="BriefFiniteElementNet.Elements.MpcElement" />
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

        public Constraint _constraint;

        /// <summary>
        /// Gets or sets the constraint.
        /// </summary>
        /// <value>
        /// The constraint of virtual support
        /// </value>
        public Constraint Constraint
        {
            get { return _constraint; }
            set { _constraint = value; }
        }


        public override CompressedColumnStorage GetExtraEquations()
        {
            var buf = new CSparse.Storage.CoordinateStorage<double>(GetExtraEquationsCount(), parent.Nodes.Count * 6 + 1, 1);

            for (var i = 0; i < Nodes.Count; i++)
            {
                var nde = Nodes[i];

                if (_constraint.DX == DofConstraint.Fixed)
                    buf.At(6 * i + 0, nde.Index * 6 + 0, _settlement.DX);

                if (_constraint.DY == DofConstraint.Fixed)
                    buf.At(6 * i + 1, nde.Index * 6 + 1, _settlement.DY);

                if (_constraint.DZ == DofConstraint.Fixed)
                    buf.At(6 * i + 2, nde.Index * 6 + 2, _settlement.DZ);


                if (_constraint.RX == DofConstraint.Fixed)
                    buf.At(6 * i + 3, nde.Index * 6 + 3, _settlement.RX);

                if (_constraint.RY == DofConstraint.Fixed)
                    buf.At(6 * i + 4, nde.Index * 6 + 4, _settlement.RY);

                if (_constraint.RZ == DofConstraint.Fixed)
                    buf.At(6 * i + 5, nde.Index * 6 + 5, _settlement.RZ);
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
    }
}
