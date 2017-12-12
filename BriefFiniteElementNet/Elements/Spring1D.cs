using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Security.Permissions;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents an spring element with defined stiffness.
    /// </summary>
    [Serializable]
    public class Spring1D:Element1D
    {
        private double _k;

        /// <inheritdoc/>
        public override Point IsoCoordsToGlobalLocation(params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the k.
        /// </summary>
        /// <value>
        /// The stiffness of spring, in N/M.
        /// </value>
        public double K
        {
            get { return _k; }
            set { _k = value; }
        }

        /// <summary>
        /// Gets or sets the start node.
        /// </summary>
        /// <value>
        /// The start node of <see cref="TrussElement2Node"/>.
        /// </value>
        public Node StartNode
        {
            get { return nodes[0]; }
            set { nodes[0] = value; }
        }

        /// <summary>
        /// Gets or sets the end node.
        /// </summary>
        /// <value>
        /// The end node of <see cref="TrussElement2Node"/>.
        /// </value>
        public Node EndNode
        {
            get { return nodes[1]; }
            set { nodes[1] = value; }
        }


        


        public override Force GetInternalForceAt(double x, LoadCombination cmb)
        {
            throw new NotImplementedException();
        }

        public override Force GetInternalForceAt(double x)
        {
            throw new NotImplementedException();
        }


        public Spring1D() : base(2)
        {
        }

        public Spring1D(Node start, Node end)
            : base(2)
        {
            this.nodes[0] = start;
            this.nodes[1] = end;
        }

        protected Spring1D(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _k = info.GetDouble("_k");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_k", _k);
            base.GetObjectData(info, context);
        }

        public override Matrix GetGlobalStifnessMatrix()
        {
            if (nodes[0].Location.Equals(nodes[1].Location) )
            {
                var kl = new Matrix(6, 6);

                kl[0, 0] = kl[1, 1] = kl[2, 2] = kl[3, 3] =
                    kl[4, 4] = kl[5, 5] = _k;

                kl[1, 0] = kl[0, 1] =
                    kl[2, 3] = kl[3, 2] =
                        kl[4, 5] = kl[5, 4] = -_k;

                var currentOrder = new FluentElementPermuteManager.ElementLocalDof[]
                {
                    new FluentElementPermuteManager.ElementLocalDof(0, DoF.Dx),
                    new FluentElementPermuteManager.ElementLocalDof(1, DoF.Dx),

                    new FluentElementPermuteManager.ElementLocalDof(0, DoF.Dy),
                    new FluentElementPermuteManager.ElementLocalDof(1, DoF.Dy),

                    new FluentElementPermuteManager.ElementLocalDof(0, DoF.Dz),
                    new FluentElementPermuteManager.ElementLocalDof(1, DoF.Dz),
                };

                var kle = FluentElementPermuteManager.FullyExpand(kl, currentOrder, 2);

                return kle;
            }
            else
            {
                var kl = new Matrix(12, 12);//.FromRowColCoreArray(12, 12, baseArr);

                kl[0, 0] = kl[6, 6] = _k;
                kl[6, 0] = kl[0, 6] = -_k;

                var t = CalcUtil.Get2NodeElementTransformationMatrix(EndNode.Location - StartNode.Location);


                return t.Transpose()*kl*t;
            }


            throw new NotImplementedException();
        }

        public override Matrix GetGlobalMassMatrix()
        {
            throw new NotImplementedException();
        }

        public override Matrix GetGlobalDampingMatrix()
        {
            throw new NotImplementedException();
        }

        public override Force[] GetEquivalentNodalLoads(Load load)
        {
            return new Force[2];
        }


        /// <summary>
        /// Gets the transformation matrix.
        /// </summary>
        /// <returns></returns>
        private Matrix GetTransformationMatrix()
        {
            var v = this.EndNode.Location - this.StartNode.Location;
            return CalcUtil.Get2NodeElementTransformationMatrix(v);
            
        }

        ///<inheritdoc/>
        public override Matrix ComputeBMatrix(params double[] location)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public override Matrix ComputeDMatrixAt(params double[] location)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public override Matrix ComputeNMatrixAt(params double[] location)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public override Matrix ComputeJMatrixAt(params double[] location)
        {
            throw new NotImplementedException();
        }
    }
}
