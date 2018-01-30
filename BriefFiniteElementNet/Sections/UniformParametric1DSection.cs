using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet.Sections
{
    /// <summary>
    /// Represents a uniform section for <see cref="BarElement"/> which defines section properties such 
    /// as area or area moments using <see cref="double"/> properties.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class UniformParametric1DSection : Base1DSection
    {
        public UniformParametric1DSection()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a">The area of section in m^2</param>
        /// <param name="iy">The Second Moment of Area of section regard to Z axis.</param>
        /// <param name="iz">The Second Moment of Area of section regard to Y axis.</param>
        /// <param name="j">The polar moment of inertial.</param>
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
        /// Gets or sets the j.
        /// </summary>
        /// <value>
        /// The polar moment of inertial.
        /// </value>
        /// <remarks>
        ///     /          /
        /// J= | ρ². dA = | (y²+z²).dA = <see cref="Iy"/> + <see cref="Iz"/> 
        ///    /A         /A
        /// </remarks>
        public double J
        {
            get { return _j; }
            set { _j = value; }
        }

        public override _1DCrossSectionGeometricProperties GetCrossSectionPropertiesAt(double x)
        {
            var buf = new _1DCrossSectionGeometricProperties();

            buf.A = this._a;
            buf.Ay = this._ay;
            buf.Az = this._az;
            buf.Iy = this._iy;
            buf.Iz = this._iz;
            buf.J = this._j;

            return buf;
        }

        public override int[] GetMaxFunctionOrder()
        {
            return new int[] {0, 0, 0};
        }
    }
}
