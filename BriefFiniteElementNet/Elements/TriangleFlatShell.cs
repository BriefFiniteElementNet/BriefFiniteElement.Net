using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    public class TriangleFlatShell : Element2D
    {
        private double _thickness;

        private double _poissonRatio;

        private double _elasticModulus;


        /// <summary>
        /// Gets or sets the thickness.
        /// </summary>
        /// <value>
        /// The thickness of this member, in [m] dimension.
        /// </value>
        public double Thickness
        {
            get { return _thickness; }
            set { _thickness = value; }
        }

        /// <summary>
        /// Gets or sets the Poisson ratio.
        /// </summary>
        /// <value>
        /// The Poisson ratio.
        /// </value>
        public double PoissonRatio
        {
            get { return _poissonRatio; }
            set { _poissonRatio = value; }
        }

        /// <summary>
        /// Gets or sets the elastic modulus.
        /// </summary>
        /// <value>
        /// The elastic modulus in [Pa] dimension.
        /// </value>
        public double ElasticModulus
        {
            get { return _elasticModulus; }
            set { _elasticModulus = value; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DktElement"/> class.
        /// </summary>
        public TriangleFlatShell() : base(3)
        {
            this.elementType = ElementType.Dkt;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DktElement"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected TriangleFlatShell(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this._thickness = info.GetDouble("_thickness");
            this._poissonRatio = info.GetDouble("_poissonRatio");
            this._elasticModulus = info.GetDouble("_elasticModulus");
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_thickness", this._thickness);
            info.AddValue("_poissonRatio", this._poissonRatio);
            info.AddValue("_elasticModulus", this._elasticModulus);
            base.GetObjectData(info, context);
        }


        public override Matrix GetGlobalStifnessMatrix()
        {
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
    }

    /// <summary>
    /// Represents the behavior of shell element
    /// </summary>
    public enum FlatShellBehaviour
    {
        /// <summary>
        /// The thin shell, based on discrete Kirchhoff theory, combination of Plate and Membrane
        /// </summary>
        ThinShell,

        /// <summary>
        /// The thin plate, based on discrete Kirchhoff theory, only bending behavior
        /// </summary>
        ThinPlate,

        /// <summary>
        /// The membrane, only in-plane forces, no moments
        /// </summary>
        Membrane
    }
}
