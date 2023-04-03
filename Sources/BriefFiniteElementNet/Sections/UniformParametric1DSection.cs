using BriefFiniteElementNet.Elements;
using System.Runtime.Serialization;
using System;
using System.Security.Permissions;


namespace BriefFiniteElementNet.Sections
{
    /// <summary>
    /// Represents a uniform section for <see cref="BarElement"/> which defines section properties such 
    /// as area or area moments using <see cref="double"/> properties.
    /// </summary>
    /// <remarks>
    /// </remarks>
    [Serializable]
    public class UniformParametric1DSection : Base1DSection
    {
        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_a", _a);
            info.AddValue("_iy", _iy);
            info.AddValue("_iz", _iz);
            info.AddValue("_ay", _ay);
            info.AddValue("_az", _az);
            info.AddValue("_j", _j);

            info.AddValue("_kz", _kz);
            info.AddValue("_ky", _ky);

            base.GetObjectData(info, context);
    
        }

        protected UniformParametric1DSection(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _a = info.GetDouble("_a");
            _iy = info.GetDouble("_iy");
            _iz = info.GetDouble("_iz");
            _ay = info.GetDouble("_ay");
            _az = info.GetDouble("_az");
            _j = info.GetDouble("_j");

            _ky = info.GetDoubleOrDefault("_ky");
            _kz = info.GetDoubleOrDefault("_kz");
        }

        public UniformParametric1DSection()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a">The area of section in m^2</param>
        /// <param name="iy">The Second Moment of Area of section regard to Z axis.</param>
        /// <param name="iz">The Second Moment of Area of section regard to Y axis.</param>
        /// <param name="j">The polar moment of inertial.</param>\
        public UniformParametric1DSection(double a, double iy, double iz, double j)
        {
            _a = a;
            _iy = iy;
            _iz = iz;
            _j = j;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a">The area of section in m^2</param>
        /// <param name="iy">The Second Moment of Area of section regard to Z axis.</param>
        /// <param name="iz">The Second Moment of Area of section regard to Y axis.</param>
        public UniformParametric1DSection(double a, double iy, double iz)
        {
            _a = a;
            _iy = iy;
            _iz = iz;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a">The area of section in m^2</param>
        public UniformParametric1DSection(double a)
        {
            _a = a;
        }

        private double _a;
        private double _ay;
        private double _az;
        private double _iy;
        private double _iz;
        private double _j;

        private double _ky;
        private double _kz;

        /// <summary>
        /// Gets or sets a.
        /// </summary>
        /// <value>
        /// The area of section in m^2
        /// </value>
        public double A
        {
            get { return _a; }
            set { _a = value; }
        }

        /// <summary>
        /// Gets or sets the ay.
        /// </summary>
        /// <value>
        /// shear area of element, in local y direction, only used when shear deformation should be considered
        /// </value>
        public double Ay
        {
            get { return _ay; }
            set { _ay = value; }
        }

        /// <summary>
        /// Gets or sets the az.
        /// </summary>
        /// <value>
        /// shear area of element, in local z direction, only used when shear deformation should be considered
        /// </value>
        public double Az
        {
            get { return _az; }
            set { _az = value; }
        }

        /// <summary>
        /// Gets or sets the iy.
        /// </summary>
        /// <value>
        /// The Second Moment of Area of section regard to Z axis.
        /// </value>
        /// <remarks>
        ///     /
        /// Iy= | Z^2 . dA
        ///    /A
        /// </remarks>
        public double Iy
        {
            get { return _iy; }
            set { _iy = value; }
        }

        /// <summary>
        /// Gets or sets the _iz.
        /// </summary>
        /// <value>
        /// The Second Moment of Area of section regard to Y axis
        /// </value>
        /// <remarks>
        ///     /
        /// Iz= | Y^2 . dA
        ///    /A
        /// </remarks>
        public double Iz
        {
            get { return _iz; }
            set { _iz = value; }
        }

        /// <summary>
        /// Gets the torsional constant.
        /// </summary>
        /// <value>
        /// Torsion Constant which is involved in the relationship between angle of twist and applied torque
        /// </value>
        /// <remarks>
        /// Note that polar moment of area is not same as torsional constant J. have a look at issue #38 in github or https://en.wikipedia.org/wiki/Torsion_constant#cite_note-1
        /// </remarks>
        public double J
        {
            get { return _j; }
            set { _j = value; }
        }


        /// <summary>
        /// Gets or sets the κy 
        /// </summary>
        /// <value>
        /// Shear correction factor in Y direction (used in timoshenko beam with BeamDirection.Y, e.g. rotation about Y)
        /// </value>
        public double Ky
        {
            get { return _ky; }
            set { _ky = value; }
        }

        /// <summary>
        /// Gets or sets the κz 
        /// </summary>
        /// <value>
        /// Shear correction factor in Z direction (used in timoshenko beam with BeamDirection.Z, e.g. rotation about Z)
        /// </value>
        public double Kz
        {
            get { return _kz; }
            set { _kz = value; }
        }


        public override _1DCrossSectionGeometricProperties GetCrossSectionPropertiesAt(double xi)
        {
            return GetCrossSectionPropertiesAt(xi, null);
        }

        public override _1DCrossSectionGeometricProperties GetCrossSectionPropertiesAt(double xi,Element targetElement)
        {
            var buf = new _1DCrossSectionGeometricProperties();

            buf.A = this._a;
            buf.Ay = this._ay;
            buf.Az = this._az;
            buf.Iy = this._iy;
            buf.Iz = this._iz;
            buf.J = this.J;

            buf.Ky= this._ky;
            buf.Kz = this._kz;

            return buf;
        }

        public override int[] GetMaxFunctionOrder()
        {
            return new int[] {0, 0, 0};
        }
    }
}
