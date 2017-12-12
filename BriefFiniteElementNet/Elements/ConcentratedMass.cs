using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Security.Permissions;
using BriefFiniteElementNet.ElementHelpers;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents a concentrated mass (with no moment inertia)
    /// </summary>
    [Serializable]
    public class ConcentratedMass : Element
    {
         /// <summary>
        /// Initializes a new instance of the <see cref="ConcentratedMass"/> class.
        /// </summary>
        public ConcentratedMass():base(1)
         {
             this.elementType = ElementType.ConcentratedMass;
         }

        /// <inheritdoc/>
        public override Point IsoCoordsToGlobalLocation(params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcentratedMass"/> class.
        /// </summary>
        /// <param name="targetNode">The node which this mass applied at.</param>
        public ConcentratedMass(Node targetNode)
            : base(1)
        {
            nodes[0] = targetNode;
            this.elementType = ElementType.ConcentratedMass;
        }

        /// <inheritdoc />
        protected ConcentratedMass(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _massAmount = info.GetDouble("_massAmount");
        }

        /// <inheritdoc />
        public override Matrix GetGlobalStifnessMatrix()
        {
            return new Matrix(6, 6);
        }

        /// <inheritdoc />
        public override Matrix GetGlobalDampingMatrix()
        {
            return new Matrix(6, 6);
        }

        /// <inheritdoc />
        public override Matrix GetGlobalMassMatrix()
        {
            var buf = new Matrix(6, 6);

            for (var i = 0; i < 3; i++)
                buf[i, i] = _massAmount;

            return buf;
        }

        private double _massAmount;

        /// <summary>
        /// Gets or sets the mass amount.
        /// </summary>
        /// <value>
        /// The mass amount in Kg.
        /// </value>
        public double MassAmount
        {
            get { return _massAmount; }
            set { _massAmount = value; }
        }



        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_massAmount", _massAmount);

            base.GetObjectData(info, context);
        }

        ///<inheritdoc/>
        public override Force[] GetEquivalentNodalLoads(Load load)
        {
            throw new NotImplementedException();
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

        public override Matrix GetLambdaMatrix()
        {
            throw new NotImplementedException();
        }

        public override IElementHelper[] GetHelpers()
        {
            throw new NotImplementedException();
        }
    }
}

