using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a tetrahedron with isotropic material.
    /// </summary>
    /// <remarks>
    /// Not fully implemented yet!</remarks>
    [Serializable]
    public class TetrahedralIso : Element3D
    {
        private double _e;
        private double _nu;
        private double _massDensity;

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

        /// <summary>
        /// The combined hash code of positions of last points whom a,b,c and d matrix are calculated based on them
        /// look at UpdateGeoMatrix()
        /// </summary>
        private int hash;

        /// <summary>
        /// Updates the a, b, c and d matrix if there is need.
        /// </summary>
        public void UpdateGeoMatrix()
        {
            var p1 = nodes[0].Location;
            var p2 = nodes[1].Location;
            var p3 = nodes[2].Location;
            var p4 = nodes[3].Location;

            var newHash = CalcUtil.GetHashCode(p1, p2, p3, p4);

            if (newHash == hash)
                return;

            var x1 = p1.X;
            var x2 = p2.X;
            var x3 = p3.X;
            var x4 = p4.X;

            var y1 = p1.Y;
            var y2 = p2.Y;
            var y3 = p3.Y;
            var y4 = p4.Y;

            var z1 = p1.Z;
            var z2 = p2.Z;
            var z3 = p3.Z;
            var z4 = p4.Z;

            if (a == null) a = new double[4];
            if (b == null) b = new double[4];
            if (c == null) c = new double[4];
            if (d == null) d = new double[4];

            {
                d[0] = x2 * y3 * z4 - x2 * y4 * z3 - x3 * y2 * z4 + x3 * y4 * z2 + x4 * y2 * z3 - x4 * y3 * z2;
                d[1] = x1 * y4 * z3 - x1 * y3 * z4 + x3 * y1 * z4 - x3 * y4 * z1 - x4 * y1 * z3 + x4 * y3 * z1;
                d[2] = x1 * y2 * z4 - x1 * y4 * z2 - x2 * y1 * z4 + x2 * y4 * z1 + x4 * y1 * z2 - x4 * y2 * z1;
                d[3] = x1 * y3 * z2 - x1 * y2 * z3 + x2 * y1 * z3 - x2 * y3 * z1 - x3 * y1 * z2 + x3 * y2 * z1;

                a[0] = y3 * z2 - y2 * z3 + y2 * z4 - y4 * z2 - y3 * z4 + y4 * z3;
                a[1] = y1 * z3 - y3 * z1 - y1 * z4 + y4 * z1 + y3 * z4 - y4 * z3;
                a[2] = y2 * z1 - y1 * z2 + y1 * z4 - y4 * z1 - y2 * z4 + y4 * z2;
                a[3] = y1 * z2 - y2 * z1 - y1 * z3 + y3 * z1 + y2 * z3 - y3 * z2;

                b[0] = x2 * z3 - x3 * z2 - x2 * z4 + x4 * z2 + x3 * z4 - x4 * z3;
                b[1] = x3 * z1 - x1 * z3 + x1 * z4 - x4 * z1 - x3 * z4 + x4 * z3;
                b[2] = x1 * z2 - x2 * z1 - x1 * z4 + x4 * z1 + x2 * z4 - x4 * z2;
                b[3] = x2 * z1 - x1 * z2 + x1 * z3 - x3 * z1 - x2 * z3 + x3 * z2;

                c[0] = x3 * y2 - x2 * y3 + x2 * y4 - x4 * y2 - x3 * y4 + x4 * y3;
                c[1] = x1 * y3 - x3 * y1 - x1 * y4 + x4 * y1 + x3 * y4 - x4 * y3;
                c[2] = x2 * y1 - x1 * y2 + x1 * y4 - x4 * y1 - x2 * y4 + x4 * y2;
                c[3] = x1 * y2 - x2 * y1 - x1 * y3 + x3 * y1 + x2 * y3 - x3 * y2;
            }

            det = x1 * y3 * z2 - x1 * y2 * z3 + x2 * y1 * z3 - x2 * y3 * z1 - x3 * y1 * z2 + x3 * y2 * z1 + x1 * y2 * z4 - x1 * y4 * z2 - x2 * y1 * z4 +
                      x2 * y4 * z1 + x4 * y1 * z2 - x4 * y2 * z1 - x1 * y3 * z4 + x1 * y4 * z3 + x3 * y1 * z4 - x3 * y4 * z1 - x4 * y1 * z3 + x4 * y3 * z1 +
                      x2 * y3 * z4 - x2 * y4 * z3 - x3 * y2 * z4 + x3 * y4 * z2 + x4 * y2 * z3 - x4 * y3 * z2;

            hash = newHash;
        }

        /// <summary>
        /// Gets or sets the elastic modulus.
        /// </summary>
        /// <value>
        /// The elastic modulus in Pa (N/M) units.
        /// </value>
        public double E
        {
            get { return _e; }
            set { _e = value; }
        }

        /// <summary>
        /// Gets or sets the Poisson ratio of material.
        /// </summary>
        /// <value>
        /// The Poisson ratio in unknown units.
        /// </value>
        public double Nu
        {
            get { return _nu; }
            set { _nu = value; }
        }

        /// <summary>
        /// Gets or sets the mass density.
        /// </summary>
        /// <value>
        /// The mass density of member in kg/m^3.
        /// </value>
        public double MassDensity
        {
            get { return _massDensity; }
            set { _massDensity = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TetrahedralIso"/> class.
        /// </summary>
        public TetrahedralIso()
            : base(4)
        {
            this.elementType = ElementType.TetrahedralIso;
        }

        /// <inheritdoc />
        public override Matrix GetGlobalStifnessMatrix()
        {
            //Code ported from D3_TETRAH.m from fem_toolbox

            UpdateGeoMatrix();

            var v = det/6.0;

            var buf = new Matrix(12, 12);
            
            var d1 = _e*(1 - _nu)/((1 + _nu)*(1 - 2*_nu)); //this is D[0,0] equals to D[1,1] equals to D[2,2]
            var d2 = _e*_nu/((1 + _nu)*(1 - 2*_nu)); //this is D[0,1] equals to D[0,2] equals to D[1,2]
            var d3 = _e/(2*(1 + _nu));

            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    buf[3 * i + 0, 3 * j + 0] = a[i] * a[j] * d1 + b[i] * b[j] * d3 + c[i] * c[j] * d3;
                    buf[3 * i + 0, 3 * j + 1] = a[i] * b[j] * d2 + a[j] * b[i] * d3;
                    buf[3 * i + 0, 3 * j + 2] = a[i] * c[j] * d2 + a[j] * c[i] * d3;

                    buf[3 * i + 1, 3 * j + 0] = a[j] * b[i] * d2 + a[i] * b[j] * d3;
                    buf[3 * i + 1, 3 * j + 1] = a[i] * a[j] * d3 + b[i] * b[j] * d1 + c[i] * c[j] * d3;
                    buf[3 * i + 1, 3 * j + 2] = b[i] * c[j] * d2 + b[j] * c[i] * d3;

                    buf[3 * i + 2, 3 * j + 0] = a[j] * c[i] * d2 + a[i] * c[j] * d3;
                    buf[3 * i + 2, 3 * j + 1] = b[j] * c[i] * d2 + b[i] * c[j] * d3;
                    buf[3 * i + 2, 3 * j + 2] = a[i] * a[j] * d3 + b[i] * b[j] * d3 + c[i] * c[j] * d1;
                }
            }

            for (var i = buf.CoreArray.Length - 1; i >= 0; i--)
            {
                buf.CoreArray[i] /= (36.0*v);
            }
            

            return buf;
        }

        /// <inheritdoc />
        public override Matrix GetGlobalMassMatrix()
        {
            //Code ported from D3_TETRAH.m from fem_toolbox

            UpdateGeoMatrix();

            var v = det / 6.0;

            var buf = new Matrix(12, 12);

            for (var i = 0; i < 12; i++)
            {
                buf[i, i] = 2.0;

                if (i < 9)
                    buf[i, i + 3] = buf[i + 3, i] = 1.0;

                if (i < 6)
                    buf[i, i + 6] = buf[i + 6, i] = 1.0;

                if (i < 3)
                    buf[i, i + 9] = buf[i + 9, i] = 1.0;
            }


            for (var i = buf.CoreArray.Length - 1; i >= 0; i--)
            {
                buf.CoreArray[i] *= _massDensity*v/20.0;
            }


            return buf;
        }

        /// <inheritdoc />
        public override Matrix GetGlobalDampingMatrix()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the stress tensor at specified location.
        /// </summary>
        /// <param name="location">The location in global coordination system.</param>
        /// <returns>The stress tensor at specified global coordination system location</returns>
        public StressTensor3D GetStressAt(Point location)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the strain tensor at specified location.
        /// </summary>
        /// <param name="location">The location in global coordination system.</param>
        /// <returns>The stress tensor at specified global coordination system location</returns>
        public StressTensor3D GetStrainAt(Point location)
        {
            throw new NotImplementedException();
        }

        #region Deserialization Constructor

        protected TetrahedralIso(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _e = (double)info.GetValue("_e", typeof(double));
            _nu = (double)info.GetValue("_nu", typeof(double));
            _massDensity = (double)info.GetValue("_massDensity", typeof(double));
            hash = (int)info.GetValue("hash", typeof(int));
        }

        #endregion

        #region ISerialization Implementation

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_e", _e);
            info.AddValue("_nu", _nu);
            info.AddValue("_massDensity", _massDensity);
            info.AddValue("hash", hash);
        }

        #endregion
    }
}