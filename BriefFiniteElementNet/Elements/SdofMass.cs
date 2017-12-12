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
    /// Represents a single DoF mass element
    /// </summary>
    [Serializable]
    public class SdofMass : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SdofMass"/> class.
        /// </summary>
        public SdofMass() : base(1)
        {
        }

        /// <inheritdoc/>
        public override Point IsoCoordsToGlobalLocation(params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SdofMass"/> class.
        /// </summary>
        /// <param name="targetNode">The target node.</param>
        public SdofMass(Node targetNode)
            : base(1)
        {
            nodes[0] = targetNode;
        }

        /// <inheritdoc />
        protected SdofMass(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _massAmount = info.GetDouble("_massAmount");
            _appliedDof = (DoF) info.GetInt32("_appliedDof");
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


            buf[(int) AppliedDof, (int) AppliedDof] = MassAmount;

            return buf;
        }

        private double _massAmount;

        private DoF _appliedDof;

        /// <summary>
        /// Gets or sets the mass amount.
        /// </summary>
        /// <value>
        /// The mass amount in SI unit (Kg,m,sec).
        /// </value>
        public double MassAmount
        {
            get { return _massAmount; }
            set { _massAmount = value; }
        }

        /// <summary>
        /// Gets or sets the applied DoF.
        /// </summary>
        /// <value>
        /// The applied DoF.
        /// </value>
        public DoF AppliedDof
        {
            get { return _appliedDof; }
            set { _appliedDof = value; }
        }


        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_massAmount", _massAmount);
            info.AddValue("_appliedDof", (int)_appliedDof);

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
