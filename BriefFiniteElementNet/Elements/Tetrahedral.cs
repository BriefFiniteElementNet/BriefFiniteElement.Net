using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Elo = BriefFiniteElementNet.FluentElementPermuteManager.ElementLocalDof;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents a tetrahedron with isotropic material.
    /// </summary>
    /// <remarks>
    /// Not fully implemented yet!
    /// </remarks>
    [Serializable]
    public class Tetrahedral : Element3D
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
            var p0 = nodes[0].Location;
            var p1 = nodes[1].Location;
            var p2 = nodes[2].Location;
            var p3 = nodes[3].Location;

            var newHash = CalcUtil.GetHashCode(p0, p1, p2, p3);

            if (newHash == hash)
                return;

            var x1 = p0.X;
            var x2 = p1.X;
            var x3 = p2.X;
            var x4 = p3.X;

            var y1 = p0.Y;
            var y2 = p1.Y;
            var y3 = p2.Y;
            var y4 = p3.Y;

            var z1 = p0.Z;
            var z2 = p1.Z;
            var z3 = p2.Z;
            var z4 = p3.Z;

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
        /// Initializes a new instance of the <see cref="Tetrahedral"/> class.
        /// </summary>
        public Tetrahedral()
            : base(4)
        {
            this.elementType = ElementType.TetrahedralIso;
        }

        /// <inheritdoc />
        public override Matrix GetGlobalStifnessMatrix()
        {
            //Code ported from D3_TETRAH.m from fem_toolbox

            var newNodeOrder = new int[4];

           


            {
                //based on "9 - The Linear Tetrahedron, file: AFEM.Ch09, page 9-4
                //http://www.colorado.edu/engineering/CAS/courses.d/AFEM.d/AFEM.Ch09.d/AFEM.Ch09.pdf
                //we do select points 1 and 4
                //then will change 2-3 to 3-2 if invalid :)

                newNodeOrder[0] = 0;
                newNodeOrder[1] = 1;
                newNodeOrder[2] = 2;
                newNodeOrder[3] = 3;

                var Jp = new Matrix(4, 4);

                for (var i = 0; i < 4; i++)
                {
                    Jp[0, i] = 1;
                    Jp[1, i] = nodes[newNodeOrder[i]].Location.X;
                    Jp[2, i] = nodes[newNodeOrder[i]].Location.Y;
                    Jp[3, i] = nodes[newNodeOrder[i]].Location.Z;
                }

                if (Jp.Determinant() < 0)
                {
                    newNodeOrder[1] = 2;
                    newNodeOrder[2] = 1;
                }

                for (var i = 0; i < 4; i++)
                {
                    Jp[0, i] = 1;
                    Jp[1, i] = nodes[newNodeOrder[i]].Location.X;
                    Jp[2, i] = nodes[newNodeOrder[i]].Location.Y;
                    Jp[3, i] = nodes[newNodeOrder[i]].Location.Z;
                }

                if (Jp.Determinant() < 0)
                    throw new Exception("!");
            }




            var J = new Matrix(4, 4);

            for (var i = 0; i < 4; i++)
            {
                J[0, i] = 1;
                J[1, i] = nodes[newNodeOrder[i]].Location.X;
                J[2, i] = nodes[newNodeOrder[i]].Location.Y;
                J[3, i] = nodes[newNodeOrder[i]].Location.Z;
            }


            var detJ = J.Determinant();
            var V = detJ / 6;


            if (V < 0)
                throw new Exception();


            var Q = detJ * J.Inverse();

            double a1 = Q[0, 1]; double b1 = Q[0, 2]; double c1 = Q[0, 3];
            double a2 = Q[1, 1]; double b2 = Q[1, 2]; double c2 = Q[1, 3];
            double a3 = Q[2, 1]; double b3 = Q[2, 2]; double c3 = Q[2, 3];
            double a4 = Q[3, 1]; double b4 = Q[3, 2]; double c4 = Q[3, 3];

            var b = new Matrix(6, 12);//transpose of b

            b.FillMatrixRowise(
                a1, 0, 0, a2, 0, 0, a3, 0, 0, a4, 0, 0,
                0, b1, 0, 0, b2, 0, 0, b3, 0, 0, b4, 0,
                0, 0, c1, 0, 0, c2, 0, 0, c3, 0, 0, c4,
                0, c1, b1, 0, c2, b2, 0, c3, b3, 0, c4, b4,
                c1, 0, a1, c2, 0, a2, c3, 0, a3, c4, 0, a4,
                b1, a1, 0, b2, a2, 0, b3, a3, 0, b4, a4, 0);



            b.MultiplyByConstant(1 / (6 * V));

            var miu = this.Nu;
            var s = (1 - miu);
            var E = new Matrix(6, 6);

            E.FillMatrixRowise(1, miu / s, miu / s, 0, 0, 0, miu / s, 1, miu / s, 0, 0, 0, miu / s, miu / s, 1, 0, 0, 0, 0, 0, 0,
                (1 - 2 * miu) / (2 * s), 0, 0, 0, 0, 0, 0, (1 - 2 * miu) / (2 * s), 0, 0, 0, 0, 0, 0, (1 - 2 * miu) / (2 * s));

            E.MultiplyByConstant(this.E * (1 - miu) / ((1 + miu) * (1 - 2 * miu)));



            var buf = b.Transpose() * E * b;

            buf.MultiplyByConstant(V);

            var currentOrder = new Elo[12];

            for (var i = 0; i < 4; i++)
            {
                currentOrder[3 * i + 0] = new Elo(newNodeOrder[i], DoF.Dx);
                currentOrder[3 * i + 1] = new Elo(newNodeOrder[i], DoF.Dy);
                currentOrder[3 * i + 2] = new Elo(newNodeOrder[i], DoF.Dz);
            }

            var bufEx = FluentElementPermuteManager.FullyExpand(buf, currentOrder, 4);

            return bufEx;
        }

        public Matrix GetGlobalStifnessMatrix_old()
        {
            //Code ported from D3_TETRAH.m from fem_toolbox

            UpdateGeoMatrix();


            var J = new Matrix(4, 4);

            for (var i = 0; i < 4; i++)
            {
                J[0, i] = 1;
                J[1, i] = nodes[i].Location.X;
                J[2, i] = nodes[i].Location.Y;
                J[3, i] = nodes[i].Location.Z;
            }

            var detJ = J.Determinant();
            var V = detJ / 6;

            var Q = detJ * J.Inverse();




            double a1 = a[0]; double b1 = b[0]; double c1 = c[0]; double d1 = d[0];
            double a2 = a[1]; double b2 = b[1]; double c2 = c[1]; double d2 = d[1];
            double a3 = a[2]; double b3 = b[2]; double c3 = c[2]; double d3 = d[2];
            double a4 = a[3]; double b4 = b[3]; double c4 = c[3]; double d4 = d[3];

            var B = new Matrix(6, 12);//transpose of b

            B.FillMatrixRowise(
                b1, 0, 0, b2, 0, 0, b3, 0, 0, b4, 0, 0,
                0, c1, 0, 0, c2, 0, 0, c3, 0, 0, c4, 0,
                0, 0, d1, 0, 0, d2, 0, 0, d3, 0, 0, d4,
                c1, b1, 0, c2, b2, 0, c3, b3, 0, c4, b4, 0,
                0, d1, c1, 0, d2, c2, 0, d3, c3, 0, d4, c4,
                d1, 0, b1, d2, 0, b2, d3, 0, b3, d4, 0, b4);



            B.MultiplyByConstant(1 / (2 * V));

            var miu = this.Nu;
            var s = (1 - miu);
            var E = new Matrix(6, 6);

            E.FillMatrixRowise(1, miu / s, miu / s, 0, 0, 0, miu / s, 1, miu / s, 0, 0, 0, miu / s, miu / s, 1, 0, 0, 0, 0, 0, 0,
                (1 - 2 * miu) / (2 * s), 0, 0, 0, 0, 0, 0, (1 - 2 * miu) / (2 * s), 0, 0, 0, 0, 0, 0, (1 - 2 * miu) / (2 * s));

            E.MultiplyByConstant(this.E * (1 - miu) / ((1 + miu) * (1 - 2 * miu)));

            var buf = B.Transpose() * E * B;

            buf.MultiplyByConstant(V);

            var currentOrder = new Elo[12];

            for (var i = 0; i < 4; i++)
            {
                currentOrder[3 * i + 0] = new Elo(i, DoF.Dx);
                currentOrder[3 * i + 1] = new Elo(i, DoF.Dy);
                currentOrder[3 * i + 2] = new Elo(i, DoF.Dz);
            }


            var bufEx = FluentElementPermuteManager.FullyExpand(buf, currentOrder, 4);

            var tmp2 = GetGlobalStifnessMatrix_old();

            return tmp2;
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

        protected Tetrahedral(SerializationInfo info, StreamingContext context)
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

        ///<inheritdoc/>
        public override Force[] GetEquivalentNodalLoads(Load load)
        {
            if (load is UniformBodyLoad3D)
            {
                //p.263 of Structural Analysis with the Finite Element Method Linear Statics, ISBN: 978-1-4020-8733-2
                //formula (8.37c) : the total body force is distributed in equal parts between the four nodes, as expected!

                this.UpdateGeoMatrix();

                var v = Math.Abs(this.det) / 6;

                var f = new Force();

                f.Fx = v / 4 * ((UniformBodyLoad3D)load).Vx;
                f.Fy = v / 4 * ((UniformBodyLoad3D)load).Vy;
                f.Fz = v / 4 * ((UniformBodyLoad3D)load).Vz;

                return new[] { f, f, f, f };
            }
           

            throw new NotImplementedException();
        }
    }
}