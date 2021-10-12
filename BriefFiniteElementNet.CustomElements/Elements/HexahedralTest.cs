using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Elo = BriefFiniteElementNet.ElementPermuteHelper.ElementLocalDof;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Integration;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents a hexahedral with isotropic material.
    /// </summary>
    [Serializable]
    [Obsolete("not fully implemented yet")]
    public class HexahedralElement : Element
    {
        public HexahedralElement() : base(8)
        {
        }
        #region mechanical props
        private BaseMaterial _material;
        public BaseMaterial Material
        {
            get { return _material; }
            set { _material = value; }
        }
        private double _e;
        private double _nu;
        private double _massDensity;

        /// <summary>
        /// Gets or sets the elastic modulus.
        /// </summary>
        /// <value>
        /// The elastic modulus 
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
        /// The Poisson ratio 
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
        /// The mass density
        /// </value>
        public double MassDensity
        {
            get { return _massDensity; }
            set { _massDensity = value; }
        }
        /// <summary>
        /// Gets the consitutive matrix. Only for isotropic materials!!! If orthotropic is needed: check http://web.mit.edu/16.20/homepage/3_Constitutive/Constitutive_files/module_3_with_solutions.pdf
        /// </summary>
        /// <returns></returns>
        private Matrix GetConstitutive()
        {
            var miu = this.Nu;
            var s = (1 - miu);
            var E = new Matrix(6, 6);

            E.FillMatrixRowise(1.0, miu / s, miu / s, 0, 0, 0, miu / s, 1.0, miu / s, 0, 0, 0, miu / s, miu / s, 1.0, 0, 0, 0, 0, 0, 0,
                (1.0 - 2.0 * miu) / (2.0 * s), 0, 0, 0, 0, 0, 0, (1.0 - 2.0 * miu) / (2.0 * s), 0, 0, 0, 0, 0, 0, (1.0 - 2.0 * miu) / (2.0 * s));

            E.MultiplyByConstant(this.E * (1.0 - miu) / ((1.0 + miu) * (1.0 - 2.0 * miu)));

            return E;
        }

        #endregion
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
            var p4 = nodes[4].Location;
            var p5 = nodes[5].Location;
            var p6 = nodes[6].Location;
            var p7 = nodes[7].Location;

            var newHash = CalcUtil.GetHashCode(p0, p1, p2, p3, p4, p5, p6, p7);

            if (newHash == hash)
                return;

            var x1 = p0.X;
            var x2 = p1.X;
            var x3 = p2.X;
            var x4 = p3.X;
            var x5 = p4.X;
            var x6 = p5.X;
            var x7 = p6.X;
            var x8 = p7.X;

            var y1 = p0.Y;
            var y2 = p1.Y;
            var y3 = p2.Y;
            var y4 = p3.Y;
            var y5 = p4.Y;
            var y6 = p5.Y;
            var y7 = p6.Y;
            var y8 = p7.Y;

            var z1 = p0.Z;
            var z2 = p1.Z;
            var z3 = p2.Z;
            var z4 = p3.Z;
            var z5 = p4.Z;
            var z6 = p5.Z;
            var z7 = p6.Z;
            var z8 = p7.Z;

            //if (a == null) a = new double[4];
            //if (b == null) b = new double[4];
            //if (c == null) c = new double[4];
            //if (d == null) d = new double[4];

            //{
            //    d[0] = x2 * y3 * z4 - x2 * y4 * z3 - x3 * y2 * z4 + x3 * y4 * z2 + x4 * y2 * z3 - x4 * y3 * z2;
            //    d[1] = x1 * y4 * z3 - x1 * y3 * z4 + x3 * y1 * z4 - x3 * y4 * z1 - x4 * y1 * z3 + x4 * y3 * z1;
            //    d[2] = x1 * y2 * z4 - x1 * y4 * z2 - x2 * y1 * z4 + x2 * y4 * z1 + x4 * y1 * z2 - x4 * y2 * z1;
            //    d[3] = x1 * y3 * z2 - x1 * y2 * z3 + x2 * y1 * z3 - x2 * y3 * z1 - x3 * y1 * z2 + x3 * y2 * z1;

            //    a[0] = y3 * z2 - y2 * z3 + y2 * z4 - y4 * z2 - y3 * z4 + y4 * z3;
            //    a[1] = y1 * z3 - y3 * z1 - y1 * z4 + y4 * z1 + y3 * z4 - y4 * z3;
            //    a[2] = y2 * z1 - y1 * z2 + y1 * z4 - y4 * z1 - y2 * z4 + y4 * z2;
            //    a[3] = y1 * z2 - y2 * z1 - y1 * z3 + y3 * z1 + y2 * z3 - y3 * z2;

            //    b[0] = x2 * z3 - x3 * z2 - x2 * z4 + x4 * z2 + x3 * z4 - x4 * z3;
            //    b[1] = x3 * z1 - x1 * z3 + x1 * z4 - x4 * z1 - x3 * z4 + x4 * z3;
            //    b[2] = x1 * z2 - x2 * z1 - x1 * z4 + x4 * z1 + x2 * z4 - x4 * z2;
            //    b[3] = x2 * z1 - x1 * z2 + x1 * z3 - x3 * z1 - x2 * z3 + x3 * z2;

            //    c[0] = x3 * y2 - x2 * y3 + x2 * y4 - x4 * y2 - x3 * y4 + x4 * y3;
            //    c[1] = x1 * y3 - x3 * y1 - x1 * y4 + x4 * y1 + x3 * y4 - x4 * y3;
            //    c[2] = x2 * y1 - x1 * y2 + x1 * y4 - x4 * y1 - x2 * y4 + x4 * y2;
            //    c[3] = x1 * y2 - x2 * y1 - x1 * y3 + x3 * y1 + x2 * y3 - x3 * y2;
            //}

            //det = x1 * y3 * z2 - x1 * y2 * z3 + x2 * y1 * z3 - x2 * y3 * z1 - x3 * y1 * z2 + x3 * y2 * z1 + x1 * y2 * z4 - x1 * y4 * z2 - x2 * y1 * z4 +
            //          x2 * y4 * z1 + x4 * y1 * z2 - x4 * y2 * z1 - x1 * y3 * z4 + x1 * y4 * z3 + x3 * y1 * z4 - x3 * y4 * z1 - x4 * y1 * z3 + x4 * y3 * z1 +
            //          x2 * y3 * z4 - x2 * y4 * z3 - x3 * y2 * z4 + x3 * y4 * z2 + x4 * y2 * z3 - x4 * y3 * z2;

            //hash = newHash;
        }
        public virtual Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var J = GetJMatrixAt(targetElement, isoCoords);
            var detJ = J.Determinant();

            var V = detJ;
            if (V < 0)
                throw new Exception();

            var xi = isoCoords[0];
            var eta = isoCoords[1];
            var zeta = isoCoords[2];

            var n1 = targetElement.Nodes[0].Location;
            var n2 = targetElement.Nodes[1].Location;
            var n3 = targetElement.Nodes[2].Location;
            var n4 = targetElement.Nodes[3].Location;
            var n5 = targetElement.Nodes[4].Location;
            var n6 = targetElement.Nodes[5].Location;
            var n7 = targetElement.Nodes[6].Location;
            var n8 = targetElement.Nodes[7].Location;

            var x1 = n1.X;
            var x2 = n2.X;
            var x3 = n3.X;
            var x4 = n4.X;
            var x5 = n5.X;
            var x6 = n6.X;
            var x7 = n7.X;
            var x8 = n8.X;

            var y1 = n1.Y;
            var y2 = n2.Y;
            var y3 = n3.Y;
            var y4 = n4.Y;
            var y5 = n5.Y;
            var y6 = n6.Y;
            var y7 = n7.Y;
            var y8 = n8.Y;

            var z1 = n1.Z;
            var z2 = n2.Z;
            var z3 = n3.Z;
            var z4 = n4.Z;
            var z5 = n5.Z;
            var z6 = n6.Z;
            var z7 = n7.Z;
            var z8 = n8.Z;

            //d(xi)
            var J11 = 1.0 / 8.0 * (-1.0 - eta + zeta + zeta * eta);
            var J12 = 1.0 / 8.0 * (-1.0 - eta - zeta - zeta * eta);
            var J13 = 1.0 / 8.0 * (1.0 - eta - zeta + zeta * eta);
            var J14 = 1.0 / 8.0 * (1.0 + eta + zeta + zeta * eta);
            var J15 = 1.0 / 8.0 * (-1.0 - eta + zeta + eta * zeta);
            var J16 = 1.0 / 8.0 * (-1.0 + eta - zeta + eta * zeta);
            var J17 = 1.0 / 8.0 * (1.0 + eta - zeta - eta * zeta);
            var J18 = 1.0 / 8.0 * (1.0 - eta - zeta + eta * zeta);


            //d(eta)
            var J21 = 1.0 / 8.0 * (-1.0 - xi + zeta + zeta * xi);
            var J23 = 1.0 / 8.0 * (-1.0 - xi + zeta + zeta * xi);
            var J24 = 1.0 / 8.0 * (1.0 + xi + zeta + zeta * xi);
            var J25 = 1.0 / 8.0 * (1.0 - xi - zeta + zeta * xi);
            var J22 = 1.0 / 8.0 * (1.0 - xi + zeta - zeta * xi);
            var J26 = 1.0 / 8.0 * (-1.0 + xi - zeta + zeta * xi);
            var J27 = 1.0 / 8.0 * (-1.0 + xi + zeta - zeta * xi);
            var J28 = 1.0 / 8.0 * (-1.0 - xi + zeta + zeta * xi);


            //d(zeta)
            var J31 = 1.0 / 8.0 * (-1.0 + xi + eta + eta * xi);
            var J33 = 1.0 / 8.0 * (1.0 - xi + eta - eta * xi);
            var J34 = 1.0 / 8.0 * (-1.0 - xi + eta + eta * xi);
            var J35 = 1.0 / 8.0 * (1.0 + xi + eta + eta * xi);
            var J32 = 1.0 / 8.0 * (-1.0 + xi - eta + eta * xi);
            var J36 = 1.0 / 8.0 * (1.0 - xi - eta + eta * xi);
            var J37 = 1.0 / 8.0 * (-1.0 - xi + eta - eta * xi);
            var J38 = 1.0 / 8.0 * (-1.0 - xi + eta + eta * xi);

            var B10 = new Matrix(3, 1);
            B10.FillMatrixRowise(J11, J21, J31);
            var B1 = J.Inverse() * B10;

            var B20 = new Matrix(3, 1);
            B20.FillMatrixRowise(J12, J22, J32);
            var B2 = J.Inverse() * B20;

            var B30 = new Matrix(3, 1);
            B30.FillMatrixRowise(J13, J23, J33);
            var B3 = J.Inverse() * B30;

            var B40 = new Matrix(3, 1);
            B40.FillMatrixRowise(J14, J24, J34);
            var B4 = J.Inverse() * B40;

            var B50 = new Matrix(3, 1);
            B50.FillMatrixRowise(J15, J25, J35);
            var B5 = J.Inverse() * B50;

            var B60 = new Matrix(3, 1);
            B60.FillMatrixRowise(J16, J26, J36);
            var B6 = J.Inverse() * B60;

            var B70 = new Matrix(3, 1);
            B70.FillMatrixRowise(J17, J27, J37);
            var B7 = J.Inverse() * B70;

            var B80 = new Matrix(3, 1);
            B80.FillMatrixRowise(J18, J28, J38);
            var B8 = J.Inverse() * B80;

            double a1 = B1[0,0]; double b1 = B1[1, 0]; double c1 = B1[2, 0];
            double a2 = B1[0, 0]; double b2 = B1[1, 0]; double c2 = B1[2, 0];
            double a3 = B1[0, 0]; double b3 = B1[1, 0]; double c3 = B1[2, 0];
            double a4 = B1[0, 0]; double b4 = B1[1, 0]; double c4 = B1[2, 0];
            double a5 = B1[0, 0]; double b5 = B1[1, 0]; double c5 = B1[2, 0];
            double a6 = B1[0, 0]; double b6 = B1[1, 0]; double c6 = B1[2, 0];
            double a7 = B1[0, 0]; double b7 = B1[1, 0]; double c7 = B1[2, 0];
            double a8 = B1[0, 0]; double b8 = B1[1, 0]; double c8 = B1[2, 0];
           
            var b = new Matrix(6, 24);//transpose of b

            b.FillMatrixRowise(
                a1, 0, 0, a2, 0, 0, a3, 0, 0, a4, 0, 0, a5, 0, 0, a6, 0, 0, a7, 0, 0, a8, 0, 0,
                0, b1, 0, 0, b2, 0, 0, b3, 0, 0, b4, 0, 0, b5, 0, 0, b6, 0, 0, b7, 0, 0, b8, 0,
                0, 0, c1, 0, 0, c2, 0, 0, c3, 0, 0, c4, 0, 0, c5, 0, 0, c6, 0, 0, c7, 0, 0, c8,
                0, c1, b1, 0, c2, b2, 0, c3, b3, 0, c4, b4, 0, c5, b5, 0, c6, b6, 0, c7, b7, 0, c8, b8,
                c1, 0, a1, c2, 0, a2, c3, 0, a3, c4, 0, a4, c5, 0, a5, c6, 0, a6, c7, 0, a7, c8, 0, a8,
                b1, a1, 0, b2, a2, 0, b3, a3, 0, b4, a4, 0, b5, a5, 0, b6, a6, 0, b7, a7, 0, b8, a8, 0);

            return b;
        }
        public virtual Matrix GetJMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var xi = isoCoords[0];
            var eta = isoCoords[1];
            var zeta = isoCoords[2];

            var n1 = targetElement.Nodes[0].Location;
            var n2 = targetElement.Nodes[1].Location;
            var n3 = targetElement.Nodes[2].Location;
            var n4 = targetElement.Nodes[3].Location;
            var n5 = targetElement.Nodes[4].Location;
            var n6 = targetElement.Nodes[5].Location;
            var n7 = targetElement.Nodes[6].Location;
            var n8 = targetElement.Nodes[7].Location;

            var x1 = n1.X;
            var x2 = n2.X;
            var x3 = n3.X;
            var x4 = n4.X;
            var x5 = n5.X;
            var x6 = n6.X;
            var x7 = n7.X;
            var x8 = n8.X;

            var y1 = n1.Y;
            var y2 = n2.Y;
            var y3 = n3.Y;
            var y4 = n4.Y;
            var y5 = n5.Y;
            var y6 = n6.Y;
            var y7 = n7.Y;
            var y8 = n8.Y;

            var z1 = n1.Z;
            var z2 = n2.Z;
            var z3 = n3.Z;
            var z4 = n4.Z;
            var z5 = n5.Z;
            var z6 = n6.Z;
            var z7 = n7.Z;
            var z8 = n8.Z;

            //d(xi)
            var J11 = 1.0 / 8.0 * (-1.0 - eta + zeta + zeta * eta);
            var J12 = 1.0 / 8.0 * (-1.0 - eta - zeta - zeta * eta);
            var J13 = 1.0 / 8.0 * (1.0 - eta - zeta + zeta * eta);
            var J14 = 1.0 / 8.0 * (1.0 + eta + zeta + zeta * eta);
            var J15 = 1.0 / 8.0 * (-1.0 - eta + zeta + eta * zeta);
            var J16 = 1.0 / 8.0 * (-1.0 + eta - zeta + eta * zeta);
            var J17 = 1.0 / 8.0 * (1.0 + eta - zeta - eta * zeta);
            var J18 = 1.0 / 8.0 * (1.0 - eta - zeta + eta * zeta);


            //d(eta)
            var J21 = 1.0 / 8.0 * (-1.0 - xi + zeta + zeta * xi);
            var J23 = 1.0 / 8.0 * (-1.0 - xi + zeta + zeta * xi);
            var J24 = 1.0 / 8.0 * (1.0 + xi + zeta + zeta * xi);
            var J25 = 1.0 / 8.0 * (1.0 - xi - zeta + zeta * xi);
            var J22 = 1.0 / 8.0 * (1.0 - xi + zeta - zeta * xi);
            var J26 = 1.0 / 8.0 * (-1.0 + xi - zeta + zeta * xi);
            var J27 = 1.0 / 8.0 * (-1.0 + xi + zeta - zeta * xi);
            var J28 = 1.0 / 8.0 * (-1.0 - xi + zeta + zeta * xi);


            //d(zeta)
            var J31 = 1.0 / 8.0 * (-1.0 + xi + eta + eta * xi);
            var J33 = 1.0 / 8.0 * (1.0 - xi + eta - eta * xi);
            var J34 = 1.0 / 8.0 * (-1.0 - xi + eta + eta * xi);
            var J35 = 1.0 / 8.0 * (1.0 + xi + eta + eta * xi);
            var J32 = 1.0 / 8.0 * (-1.0 + xi - eta + eta * xi);
            var J36 = 1.0 / 8.0 * (1.0 - xi - eta + eta * xi);
            var J37 = 1.0 / 8.0 * (-1.0 - xi + eta - eta * xi);
            var J38 = 1.0 / 8.0 * (-1.0 - xi + eta + eta * xi);

            var buf = new Matrix(3, 3);

            buf.FillRow(0, (J11 * x1 + J12 * x2 + J13 * x3 + J14 * x4 + J15 * x5 + J16 * x6 + J17 * x7 + J18 * x8),
                           (J11 * y1 + J12 * y2 + J13 * y3 + J14 * y4 + J15 * y5 + J16 * y6 + J17 * y7 + J18 * y8),
                           (J11 * z1 + J12 * z2 + J13 * z3 + J14 * z4 + J15 * z5 + J16 * z6 + J17 * z7 + J18 * z8));
            buf.FillRow(1, (J21 * x1 + J22 * x2 + J23 * x3 + J24 * x4 + J25 * x5 + J26 * x6 + J27 * x7 + J28 * x8),
                           (J21 * y1 + J22 * y2 + J23 * y3 + J24 * y4 + J25 * y5 + J26 * y6 + J27 * y7 + J28 * y8),
                           (J21 * z1 + J22 * z2 + J23 * z3 + J24 * z4 + J25 * z5 + J26 * z6 + J27 * z7 + J28 * z8));
            buf.FillRow(2, (J31 * x1 + J32 * x2 + J33 * x3 + J34 * x4 + J35 * x5 + J36 * x6 + J37 * x7 + J38 * x8),
                           (J31 * y1 + J32 * y2 + J33 * y3 + J34 * y4 + J35 * y5 + J36 * y6 + J37 * y7 + J38 * y8),
                           (J31 * z1 + J32 * z2 + J33 * z3 + J34 * z4 + J35 * z5 + J36 * z6 + J37 * z7 + J38 * z8));
            //buf.FillRow(1, J21, J22, J23, J24, J25, J26, J27, J28);
            //buf.FillRow(2, J31, J32, J33, J34, J35, J36, J37, J38);

            return buf;
        }

        /// <inheritdoc/>
        public override double[] IsoCoordsToLocalCoords(params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public override Matrix GetGlobalStifnessMatrix()
        {
            return CalcLocalKMatrix(this);
        }
        public Matrix GetGlobalMassMatrix(Element targetElement)
        {
            //http://what-when-how.com/the-finite-element-method/fem-for-3d-solids-finite-element-method-part-2/
            var j = GetJMatrixAt(targetElement, new double[] {1,1,1});
            var v = Math.Abs(j.Determinant());

            var buf = new Matrix(8, 8);

            for (var i = 0; i < 8; i++)
            {
                buf[i, i] = 8.0;
            }
            buf[1, 0] = buf[0, 1] = 4.0;
            buf[2, 0] = buf[0, 2] = 2.0;
            buf[3, 0] = buf[0, 3] = 4.0;
            buf[4, 0] = buf[0, 4] = 4.0;
            buf[5, 0] = buf[0, 5] = 2.0;
            buf[6, 0] = buf[0, 6] = 1.0;
            buf[7, 0] = buf[0, 7] = 2.0;

            buf[2, 1] = buf[1, 2] = 4.0;
            buf[3, 1] = buf[1, 3] = 2.0;
            buf[4, 1] = buf[1, 4] = 2.0;
            buf[5, 1] = buf[1, 5] = 4.0;
            buf[6, 1] = buf[1, 6] = 2.0;
            buf[7, 1] = buf[1, 7] = 1.0;

            buf[3, 2] = buf[2, 3] = 4.0;
            buf[4, 2] = buf[2, 4] = 1.0;
            buf[5, 2] = buf[2, 5] = 2.0;
            buf[6, 2] = buf[2, 6] = 4.0;
            buf[7, 2] = buf[2, 7] = 2.0;

            buf[4, 3] = buf[3, 4] = 2.0;
            buf[5, 3] = buf[3, 5] = 1.0;
            buf[6, 3] = buf[3, 6] = 2.0;
            buf[7, 3] = buf[3, 7] = 4.0;

            buf[5, 4] = buf[4, 5] = 4.0;
            buf[6, 4] = buf[4, 6] = 2.0;
            buf[7, 4] = buf[4, 7] = 4.0;

            buf[6, 5] = buf[5, 6] = 4.0;
            buf[7, 5] = buf[5, 7] = 2.0;

            buf[7, 6] = buf[6, 7] = 4.0;

            for (var i = buf.CoreArray.Length - 1; i >= 0; i--)
            {
                buf.CoreArray[i] *= _massDensity * v / 216.0;
            }

            return buf;
        }
        public Matrix CalcLocalKMatrix(Element targetElement)
        {
            var intg = new GaussianIntegrator();

            intg.GammaPointCount = 2;
            intg.XiPointCount = 2;
            intg.EtaPointCount = 2;

            intg.A2 = 1;
            intg.A1 = -1;

            intg.F2 = (gama => +1);
            intg.F1 = (gama => -1);

            intg.G2 = (eta, gama) => +1;
            intg.G1 = (eta, gama) => -1;

            intg.H = new FunctionMatrixFunction((xi, eta, gamma) =>
            {
                var b = this.GetBMatrixAt(targetElement, new double[] { xi, eta, gamma });
                var d = GetConstitutive();
                var j = this.GetJMatrixAt(targetElement, new double[] { xi, eta, gamma });

                var buf = new Matrix(b.ColumnCount, b.ColumnCount);

                CalcUtil.Bt_D_B(b, d, buf);

                //var detj = Math.Abs(j.Determinant());

                //buf.MultiplyByConstant(detj);

                return buf;
            });

            var res = intg.Integrate();

            return res;
        }
        public override Matrix GetGlobalDampingMatrix()
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
        public override Matrix GetGlobalMassMatrix()
        {
            throw new NotImplementedException();
        }
        public override Force[] GetGlobalEquivalentNodalLoads(ElementalLoad load)
        {
            //if (load is UniformBodyLoad3D)
            //{
            //    //p.263 of Structural Analysis with the Finite Element Method Linear Statics, ISBN: 978-1-4020-8733-2
            //    //formula (8.37c) : the total body force is distributed in equal parts between the four nodes, as expected!

            //    //this.UpdateGeoMatrix();
            //    var j = GetJMatrixAt(targetElement, new double[] { 0, 0, 0 });
            //    var v = Math.Abs(j.Determinant());

            //    var f = new Force();

            //    f.Fx = v / 8 * ((UniformBodyLoad3D)load).Vx;
            //    f.Fy = v / 8 * ((UniformBodyLoad3D)load).Vy;
            //    f.Fz = v / 8 * ((UniformBodyLoad3D)load).Vz;

            //    return new[] { f, f, f, f, f, f, f, f };
            //}


            throw new NotImplementedException();
        }
        public Force[] GetGlobalEquivalentNodalLoads(Element targetElement, ElementalLoad load)
        {
            if (load is UniformBodyLoad3D)
            {
                //p.263 of Structural Analysis with the Finite Element Method Linear Statics, ISBN: 978-1-4020-8733-2
                //formula (8.37c) : the total body force is distributed in equal parts between the four nodes, as expected!

                //this.UpdateGeoMatrix();
                var j = GetJMatrixAt(targetElement, new double[] {1,1,1});
                var v = Math.Abs(j.Determinant());

                var f = new Force();

                f.Fx = v / 8 * ((UniformBodyLoad3D)load).Vx;
                f.Fy = v / 8 * ((UniformBodyLoad3D)load).Vy;
                f.Fz = v / 8 * ((UniformBodyLoad3D)load).Vz;

                return new[] { f, f, f, f, f, f, f, f };
            }


            throw new NotImplementedException();
        }

        #region Deserialization Constructor

        protected HexahedralElement(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _e = (double)info.GetValue("_e", typeof(double));
            _nu = (double)info.GetValue("_nu", typeof(double));
            _massDensity = (double)info.GetValue("_massDensity", typeof(double));
            _material = (BaseMaterial)info.GetValue("_material", typeof(int));
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
            info.AddValue("_material", _material);
        }

        #endregion
    }
}
