using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Integration;
using BriefFiniteElementNet.Common;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// represents a discrete Kirchoff triangular element with constant thickness.
    /// 
    /// implementations:
    /// https://github.com/eudoxos/woodem/blob/9e232d3a737cd3095a7c1eaa82e9e28829af97ab/pkg/fem/Membrane.cpp
    /// https://github.com/ahojukka5/FEMTools/blob/dfecb12d445feeac94ea2e8bcb2b6cc785fbaa72/TeeFEM/teefem/models/dkt.py (cool one, also have geometric B and stiffness matrix)
    /// https://github.com/Micket/oofem/blob/b5fc923f27ccfea47095fd62c8f3209f025224bd/src/sm/Elements/Plates/dkt.C
    /// https://github.com/IvanAssing/FEM-Shell/blob/5700101c97b90230646f82167e7528cae5d875b4/elementsdkt.cpp
    /// https://github.com/jiamingkang/bles/blob/939394f241f8e01a296d38ff52feac246793cbc9/CHak2DPlate3.cpp
    /// https://github.com/cescjf/locistream-fsi-coupling/blob/610a5b50933333bf6f43bc0b0ec19cc849dcb543/src/FSI/nlams_dkt_shape.f90
    /// 
    /// refs:
    ///     [1] "Development of Membrane, Plate and Flat Shell Elements in Java" thesis by Kaushalkumar Kansara available on the web
    ///     [2] "A STUDY OF THREE-NODE TRIANGULAR PLATE BENDING ELEMENTS" by JEAN-LOUIS BATOZ,KLAUS-JORGEN BATHE and LEE-WING HO
    ///     [3] "Membrane element" https://woodem.org/theory/membrane-element.html
    /// </summary>
    [Serializable]
    [Obsolete("Use Triangle Plate Shell with Behavior = ThinPlate")]
    public class DktElement : Element2D
    {
        #region fields and properties

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


        #endregion


        #region constructors and serialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DktElement"/> class.
        /// </summary>
        public DktElement() : base(3)
        {
            //this.elementType = ElementType.Dkt;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DktElement"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected DktElement(SerializationInfo info, StreamingContext context) : base(info, context)
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

        #endregion

        /// <inheritdoc/>
        public override double[] IsoCoordsToLocalCoords(params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets node coordinates in local coordination system.
        /// Z component of return values should be ignored.
        /// </summary>
        private Point[] GetLocalPoints()
        {
            var t = GetTransformationMatrix().Transpose(); //transpose of t

            var g0 = nodes[0].Location.ToMatrix();
            var g1 = nodes[1].Location.ToMatrix();
            var g2 = nodes[2].Location.ToMatrix();


            var p0 = (t*g0).ToPoint();
            var p1 = (t*g1).ToPoint();
            var p2 = (t*g2).ToPoint();

            return new[] {p0, p1, p2};
        }

        internal Vector TranformLocalToGlobal(Vector v)
        {
            var mtx = GetTransformationMatrix();

            var b = mtx * v.ToMatrix();

            return new Vector(b[0, 0], b[1, 0], b[2, 0]);
        }

        internal Vector TranformGlobalToLocal(Vector v)
        {
            var mtx = GetTransformationMatrix();

            var b = mtx.Transpose() * v.ToMatrix();//eq. 5.14, p. 86

            return new Vector(b[0, 0], b[1, 0], b[2, 0]);
        }

        /// <summary>
        /// Gets the 3x3 transformation matrix which converts local and global coordinates to each other.
        /// </summary>
        /// <returns>Transformation matrix.</returns>
        /// <remarks>
        /// GC = T * LC
        /// where
        ///  GC: Global Coordinates [X;Y;Z] (3x1) (same as lambda in "Development of Membrane, Plate and Flat Shell Elements in Java" thesis by Kaushalkumar Kansara available on the web).
        ///  LC: Local Coordinates [x;y;z] (3x1) (same as xyz in "Development of Membrane, Plate and Flat Shell Elements in Java" thesis by Kaushalkumar Kansara available on the web).
        ///  T: Transformation Matrix (from this method) (3x3) (same as XYZ in "Development of Membrane, Plate and Flat Shell Elements in Java" thesis by Kaushalkumar Kansara available on the web).
        /// </remarks>
        public Matrix GetTransformationMatrix()
        {
            return DktElement.GetTransformationMatrix(nodes[0].Location, nodes[1].Location, nodes[2].Location);
        }

        /// <inheritdoc />
        public override Matrix GetGlobalStifnessMatrix()
        {
            //step 1 : get points in local system
            //step 2 : get local stiffness matrix
            //step 3 : expand local stiffness matrix
            //step 4 : get global stiffness matrix

            //step 1
            var ls = GetLocalPoints();

            var xs = new double[] {ls[0].X, ls[1].X, ls[2].X};
            var ys = new double[] {ls[0].Y, ls[1].Y, ls[2].Y};

            //step 2
            var kl = DktElement.GetStiffnessMatrix(xs, ys, this._thickness, this.ElasticModulus, this.PoissonRatio);

            //step 3
            var currentOrder = new ElementPermuteHelper.ElementLocalDof[]
            {
                new ElementPermuteHelper.ElementLocalDof(0, DoF.Dz),
                new ElementPermuteHelper.ElementLocalDof(0, DoF.Rx),
                new ElementPermuteHelper.ElementLocalDof(0, DoF.Ry),

                new ElementPermuteHelper.ElementLocalDof(1, DoF.Dz),
                new ElementPermuteHelper.ElementLocalDof(1, DoF.Rx),
                new ElementPermuteHelper.ElementLocalDof(1, DoF.Ry),

                new ElementPermuteHelper.ElementLocalDof(2, DoF.Dz),
                new ElementPermuteHelper.ElementLocalDof(2, DoF.Rx),
                new ElementPermuteHelper.ElementLocalDof(2, DoF.Ry),
            };

            var kle = ElementPermuteHelper.FullyExpand(kl, currentOrder, 3);

            var lambda = GetTransformationMatrix();

            var t = lambda.Transpose().RepeatDiagonally(6); // eq. 5-16 page 78 (87 of file)

            //step 4 : get global stiffness matrix
            var buf = t.Transpose()* kle * t; //eq. 5-15 p77

            return buf;
        }

        /// <summary>
        /// Gets the transformation matrix which converts local and global coordinates to each other.
        /// </summary>
        /// <returns>Transformation matrix.</returns>
        public Matrix GetLocalStifnessMatrix()
        {
            var lpts = GetLocalPoints();

            var a = GetArea();

            var d = GetDMatrix(this.Thickness, this.ElasticModulus, this.PoissonRatio);


            var intg = new Integration.GaussianIntegrator();

            intg.A2 = 1;
            intg.A1 = 0;

            intg.F2 = (gama => 1);
            intg.F1 = (gama => 0);

            intg.G2 = ((nu, gama) => 1 - nu);
            intg.G1 = ((nu, gama) => 0);

            intg.XiPointCount = intg.EtaPointCount = 3;
            intg.GammaPointCount = 1;

            intg.H = new FunctionMatrixFunction((xi, nu, gamma) =>
            {
                var b = GetBMatrix_old(xi, nu, lpts);

                var ki = b.Transpose()*d*b;

                return ki;
            });

            /**/

            var res = intg.Integrate();

            res.Scale(2*a);

           

            //klocal DoF order is as noted in eq. 4.21 p. 44 : w1, θx1, θy1, w2, θx2, θy2, w3, θx3, θy3
            //it should convert to u1, v1, w1, θx1, θy1, θz1, ... and so on
            //will done with permutation matrix p, this formulation is not noted in any reference of this doc

            var p = new Matrix(6, 3);
            p[2, 0] = p[3, 1] = p[4, 2] = 1;

            var pt = p.RepeatDiagonally(3);

            var buf2 = (pt * res) * pt.Transpose();//local expanded stiffness matrix

            //adding drilling dof:

            var maxKii = Enumerable.Range(0, res.RowCount).Max(i => res[i, i]);
            buf2[5, 5] = buf2[11, 11] = buf2[17, 17] = maxKii/1e3;//eq 5.2, p. 71 ref [1]

            return buf2;
        }

        /// <summary>
        /// Gets the b matrix.
        /// </summary>
        /// <param name="k">The k.</param>
        /// <param name="e">The e.</param>
        /// <param name="lpts">The local points.</param>
        /// <returns></returns>
        private Matrix GetBMatrix_old(double k, double e, Point[] lpts)
        {
            #region inits

            var x = new double[] { lpts[0].X, lpts[1].X, lpts[2].X };
            var y = new double[] { lpts[0].Y, lpts[1].Y, lpts[2].Y };

            var x23 = x[1] - x[2];
            var x31 = x[2] - x[0];
            var x12 = x[0] - x[1];

            var y23 = y[1] - y[2];
            var y31 = y[2] - y[0];
            var y12 = y[0] - y[1];

            var a = GetArea();// 0.5 * Math.Abs(x31 * y12 - x12 * y31);

            var l23_2 = y23 * y23 + x23 * x23;
            var l31_2 = y31 * y31 + x31 * x31;
            var l12_2 = y12 * y12 + x12 * x12;



            var P4 = -6 * x23 / l23_2;
            var P5 = -6 * x31 / l31_2;
            var P6 = -6 * x12 / l12_2;

            var q4 = 3 * x23 * y23 / l23_2;
            var q5 = 3 * x31 * y31 / l31_2;
            var q6 = 3 * x12 * y12 / l12_2;

            var r4 = 3 * y23 * y23 / l23_2;
            var r5 = 3 * y31 * y31 / l31_2;
            var r6 = 3 * y12 * y12 / l12_2;

            var t4 = -6 * y23 / l23_2;
            var t5 = -6 * y31 / l31_2;
            var t6 = -6 * y12 / l12_2;

            #endregion

            #region h{x,y}{kesi,no}

            var hx_xi = new Func<double, double, Matrix>(
                (xi, eta) =>
                {
                    var buf = new double[]
                    {
                        P6*(1 - 2*xi) + (P5 - P6)*eta,
                        q6*(1 - 2*xi) - (q5 + q6)*eta,
                        -4 + 6*(xi + eta) + r6*(1 - 2*xi) - eta*(r5 + r6),
                        -P6*(1 - 2*xi) + eta*(P4 + P6),
                        q6*(1 - 2*xi) - eta*(q6 - q4),
                        -2 + 6*xi + r6*(1 - 2*xi) + eta*(r4 - r6),
                        -eta*(P5 + P4),
                        eta*(q4 - q5),
                        -eta*(r5 - r4)
                    };

                    return new Matrix(9, 1, buf);
                });//eq. 4.27 ref [1], also noted in several other references



            var hy_xi = new Func<double, double, Matrix>(
                (xi, eta) =>
                {
                    var buf = new double[]
                    {
                        t6*(1 - 2*xi) + eta*(t5 - t6),
                        1 + r6*(1 - 2*xi) - eta*(r5 + r6),
                        -q6*(1 - 2*xi) + eta*(q5 + q6),
                        -t6*(1 - 2*xi) + eta*(t4 + t6),
                        -1 + r6*(1 - 2*xi) + eta*(r4 - r6),
                        -q6*(1 - 2*xi) - eta*(q4 - q6),
                        -eta*(t4 + t5),
                        eta*(r4 - r5),
                        -eta*(q4 - q5)
                    };

                    return new Matrix(9, 1, buf);
                });//eq. 4.28 ref [1], also noted in several other references

            var hx_eta = new Func<double, double, Matrix>(
                (xi, eta) =>
                {
                    var buf = new double[]
                    {
                        -P5*(1 - 2*eta) - xi*(P6 - P5),
                        q5*(1 - 2*eta) - xi*(q5 + q6),
                        -4 + 6*(xi + eta) + r5*(1 - 2*eta) - xi*(r5 + r6),
                        xi*(P4 + P6),
                        xi*(q4 - q6),
                        -xi*(r6 - r4),
                        P5*(1 - 2*eta) - xi*(P4 + P5),
                        q5*(1 - 2*eta) + xi*(q4 - q5),
                        -2 + 6*eta + r5*(1 - 2*eta) + xi*(r4 - r5)
                    };

                    return new Matrix(9, 1, buf);
                });//eq. 4.29 ref [1], also noted in several other references

            var hy_eta = new Func<double, double, Matrix>(
                (xi, eta) =>
                {
                    var buf = new double[]
                    {
                        -t5*(1 - 2*eta) - xi*(t6 - t5),
                        1 + r5*(1 - 2*eta) - xi*(r5 + r6),
                        -q5*(1 - 2*eta) + xi*(q5 + q6),
                        xi*(t4 + t6),
                        xi*(r4 - r6),
                        -xi*(q4 - q6),
                        t5*(1 - 2*eta) - xi*(t4 + t5),
                        -1 + r5*(1 - 2*eta) + xi*(r4 - r5),
                        -q5*(1 - 2*eta) - xi*(q4 - q5)
                    };

                    return new Matrix(9, 1, buf);
                });//eq. 4.30 ref [1], also noted in several other references


            #endregion

            var B = new Func<double, double, Matrix>(
               (xi, eta) =>
               {
                   var b1 = (y31 * hx_xi(xi, eta) + y12 * hx_eta(xi, eta));
                   var b2 = (-x31 * hy_xi(xi, eta) - x12 * hy_eta(xi, eta));

                   var b3 =
                       (-x31 * hx_xi(xi, eta) - x12 * hx_eta(xi, eta) + y31 * hy_xi(xi, eta) + y12 * hy_eta(xi, eta));

                   var buf = Matrix.OfJaggedArray(new[] { b1.Values, b2.Values, b3.Values });

                   buf.Scale(1 / (2 * a));

                   return buf.AsMatrix();
               });//eq. 4.26 page 46 ref [1]

            var buff = B(k, e);

            return buff;
        }

        #region statics

        /// <summary>
        /// Gets the stiffness matrix of DKT element.
        /// </summary>
        /// <param name="x">The x location of points, in local coordinates.</param>
        /// <param name="y">The y location of points, in local coordinates.</param>
        /// <param name="t">The thickness.</param>
        /// <param name="e">The elastic modulus.</param>
        /// <param name="nu">The Poisson ratio.</param>
        /// <returns></returns>
        internal static Matrix GetStiffnessMatrix(double[] x, double[] y, double t, double e, double nu)
        {
            double a;//area

            {
                var x31 = x[2] - x[0];
                var x12 = x[0] - x[1];
                var y31 = y[2] - y[0];
                var y12 = y[0] - y[1];

                a = 0.5 * Math.Abs(x31 * y12 - x12 * y31);
            }

            var d = GetDMatrix(t, e, nu);



            var intg = new BriefFiniteElementNet.Integration.GaussianIntegrator();

            intg.A2 = 1;
            intg.A1 = 0;

            intg.F2 = (gama => 1);
            intg.F1 = (gama => 0);

            intg.G2 = ((eta, gama) => 1 - eta);
            intg.G1 = ((eta, gama) => 0);

            intg.XiPointCount = intg.EtaPointCount = 3;
            intg.GammaPointCount = 1;

            intg.H = new FunctionMatrixFunction((xi, eta, gamma) =>
            {
                var b = GetBMatrix(xi, eta, x, y);

                var ki = b.Transpose() * d * b;

                return ki;
            });

            var res = intg.Integrate();

            res.Scale(2 * a);

            return res;
        }

        /// <summary>
        /// Gets the B matrix of DKT element at specified location.
        /// </summary>
        /// <param name="xi">The xi.</param>
        /// <param name="eta">The eta.</param>
        /// <param name="x">The x location of points, in local coordinate system.</param>
        /// <param name="y">The y location of points, in local coordinate system.</param>
        /// <returns>B matrix</returns>
        public static Matrix GetBMatrix(double xi, double eta, double[] x, double[] y)
        {
            var x23 = x[1] - x[2];
            var x31 = x[2] - x[0];
            var x12 = x[0] - x[1];

            var y23 = y[1] - y[2];
            var y31 = y[2] - y[0];
            var y12 = y[0] - y[1];

            var a = 0.5*Math.Abs(x31*y12 - x12*y31);

            var l23_2 = y23 * y23 + x23 * x23;
            var l31_2 = y31 * y31 + x31 * x31;
            var l12_2 = y12 * y12 + x12 * x12;

            var P4 = -6 * x23 / l23_2;
            var P5 = -6 * x31 / l31_2;
            var P6 = -6 * x12 / l12_2;

            var q4 = 3 * x23 * y23 / l23_2;
            var q5 = 3 * x31 * y31 / l31_2;
            var q6 = 3 * x12 * y12 / l12_2;

            var r4 = 3 * y23 * y23 / l23_2;
            var r5 = 3 * y31 * y31 / l31_2;
            var r6 = 3 * y12 * y12 / l12_2;

            var t4 = -6 * y23 / l23_2;
            var t5 = -6 * y31 / l31_2;
            var t6 = -6 * y12 / l12_2;


            var h_x_xi = new double[]
            {
                    P6*(1 - 2*xi) + (P5 - P6)*eta,
                    q6*(1 - 2*xi) - (q5 + q6)*eta,
                    -4 + 6*(xi + eta) + r6*(1 - 2*xi) - eta*(r5 + r6),
                    -P6*(1 - 2*xi) + eta*(P4 + P6),
                    q6*(1 - 2*xi) - eta*(q6 - q4),
                    -2 + 6*xi + r6*(1 - 2*xi) + eta*(r4 - r6),
                    -eta*(P5 + P4),
                    eta*(q4 - q5),
                    -eta*(r5 - r4)
            };


            var h_y_xi = new double[]
            {
                    t6*(1 - 2*xi) + eta*(t5 - t6),
                    1 + r6*(1 - 2*xi) - eta*(r5 + r6),
                    -q6*(1 - 2*xi) + eta*(q5 + q6),
                    -t6*(1 - 2*xi) + eta*(t4 + t6),
                    -1 + r6*(1 - 2*xi) + eta*(r4 - r6),
                    -q6*(1 - 2*xi) - eta*(q4 - q6),
                    -eta*(t4 + t5),
                    eta*(r4 - r5),
                    -eta*(q4 - q5)
            };


            var h_x_eta = new double[]
            {
                    -P5*(1 - 2*eta) - xi*(P6 - P5),
                    q5*(1 - 2*eta) - xi*(q5 + q6),
                    -4 + 6*(xi + eta) + r5*(1 - 2*eta) - xi*(r5 + r6),
                    xi*(P4 + P6),
                    xi*(q4 - q6),
                    -xi*(r6 - r4),
                    P5*(1 - 2*eta) - xi*(P4 + P5),
                    q5*(1 - 2*eta) + xi*(q4 - q5),
                    -2 + 6*eta + r5*(1 - 2*eta) + xi*(r4 - r5)
            };

            var h_y_eta = new double[]
            {
                    -t5*(1 - 2*eta) - xi*(t6 - t5),
                    1 + r5*(1 - 2*eta) - xi*(r5 + r6),
                    -q5*(1 - 2*eta) + xi*(q5 + q6),
                    xi*(t4 + t6),
                    xi*(r4 - r6),
                    -xi*(q4 - q6),
                    t5*(1 - 2*eta) - xi*(t4 + t5),
                    -1 + r5*(1 - 2*eta) + xi*(r4 - r5),
                    -q5*(1 - 2*eta) - xi*(q4 - q5)
            };

            var H_x_x = new double[9];
            var H_y_y = new double[9];

            var H_xy_yx = new double[9];


            for (var i = 0; i < 9; i++)
            {
                H_x_x[i] = y31 * h_x_xi[i] + y12 * h_x_eta[i];

                H_y_y[i] = -x31 * h_y_xi[i] - x12 * h_y_eta[i];

                H_xy_yx[i] = -x31 * h_x_xi[i] - x12 * h_x_eta[i] + y31 * h_y_xi[i] + y12 * h_y_eta[i];
            } //eq 4.26 p. 46


            var buf = Matrix.OfJaggedArray(new[] { H_x_x, H_y_y, H_xy_yx });

            buf.Scale(1 / (2 * a));

            return buf.AsMatrix();
        }

        /// <summary>
        /// Gets the D matrix of DKT element.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="e">The e.</param>
        /// <param name="nu">The nu.</param>
        /// <returns></returns>
        internal static Matrix GetDMatrix(double t, double e, double nu)
        {
            var d = new Matrix(3, 3);

            {
                //page 50 of pdf

                var cf = e * t * t * t /
                         (12 * (1 - nu * nu));

                d[0, 0] = d[1, 1] = 1;
                d[1, 0] = d[0, 1] = nu;
                d[2, 2] = (1 - nu) / 2;

                d.Scale(cf);
            }

            return d;
        }

        /// <summary>
        /// Gets the 3x3 transformation matrix which converts local and global coordinates to each other.
        /// </summary>
        /// <returns>Transformation matrix.</returns>
        /// <remarks>
        /// GC = T * LC
        /// where
        ///  GC: Global Coordinates [X;Y;Z] (3x1) (same as lambda in "Development of Membrane, Plate and Flat Shell Elements in Java" thesis by Kaushalkumar Kansara available on the web).
        ///  LC: Local Coordinates [x;y;z] (3x1) (same as xyz in "Development of Membrane, Plate and Flat Shell Elements in Java" thesis by Kaushalkumar Kansara available on the web).
        ///  T: Transformation Matrix (from this method) (3x3) (same as XYZ in "Development of Membrane, Plate and Flat Shell Elements in Java" thesis by Kaushalkumar Kansara available on the web).
        /// </remarks>
        internal static Matrix GetTransformationMatrix(Point p1, Point p2, Point p3)
        {
            //these calculations are noted in page [72 of 166] (page 81 of pdf) of "Development of Membrane, Plate and Flat Shell Elements in Java" thesis by Kaushalkumar Kansara available on the web

            var v1 = p1 - Point.Origins;
            var v2 = p2 - Point.Origins;
            var v3 = p3 - Point.Origins;

            var ii = (v1 + v2) / 2;
            var jj = (v2 + v3) / 2;
            var kk = (v3 + v1) / 2;

            var vx = (jj - kk).GetUnit();//eq. 5.3
            var vr = (ii - v3).GetUnit();//eq. 5.5
            var vz = Vector.Cross(vx, vr);//eq. 5.6
            var vy = Vector.Cross(vz, vx);//eq. 5.7

            var lamX = vx.GetUnit();//Lambda_x
            var lamY = vy.GetUnit();//Lambda_x
            var lamZ = vz.GetUnit();//Lambda_x

            var lambda = Matrix.OfJaggedArray(new[]
            {
                new[] {lamX.X, lamY.X, lamZ.X},
                new[] {lamX.Y, lamY.Y, lamZ.Y},
                new[] {lamX.Z, lamY.Z, lamZ.Z}
            });//eq. 5.13

            return lambda.AsMatrix();
        }
        #endregion

        /// <inheritdoc />
        public override Matrix GetGlobalMassMatrix()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Matrix GetGlobalDampingMatrix()
        {
            throw new NotImplementedException();
        }

       

        /// <summary>
        /// Gets the area of triangular element
        /// </summary>
        /// <returns></returns>
        public double GetArea()
        {

/* Unmerged change from project 'Legacy (netstandard2.0)'
Before:
            return CalcUtil.GetTriangleArea(nodes[0].Location, nodes[1].Location, nodes[2].Location);
After:
            return GeometryUtils.GetTriangleArea(nodes[0].Location, nodes[1].Location, nodes[2].Location);
*/
            return BriefFiniteElementNet.GeometryUtils.GetTriangleArea(nodes[0].Location, nodes[1].Location, nodes[2].Location);
        }



        /// <summary>
        /// Gets the equivalent nodal load. in global coordination system.
        /// </summary>
        /// <param name="load">The load.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Force[] GetEquivalentNodalLoad(UniformLoadForPlanarElements load)
        {
            var q = 0.0; //orthogonal component of load, to be calculated
            var a = GetArea(); //area

            if (load.CoordinationSystem == CoordinationSystem.Local)
            {
                q = load.Uz;
            }
            else
            {
                var globVec = new Vector(load.Ux, load.Uy, load.Uz);
                var lVec = TranformGlobalToLocal(globVec);
                q = lVec.Z;
            }

            /*
            var N1 = new Func<double, double, double>((xi, nu) => 2*(1 - xi - nu)*(0.5 - xi - nu));
            var N2 = new Func<double, double, double>((xi, nu) => xi*(2.0*xi - 1));
            var N3 = new Func<double, double, double>((xi, nu) => nu*(2.0*nu - 1));
            var N4 = new Func<double, double, double>((xi, nu) => 4*xi*nu);
            var N5 = new Func<double, double, double>((xi, nu) => 4*nu*(1 - xi - nu));
            var N6 = new Func<double, double, double>((xi, nu) => 4*nu*(1 - xi - nu));

           

            var intg = new CustomGaussianIntegrator();

            intg.v = new[] {-Math.Sqrt(1.0/3.0), Math.Sqrt(1.0/3.0)};
            intg.w = new[] {1.0, 1.0};

            intg.a2 = 1;
            intg.a1 = 0;

            intg.f2 = (gama => 1);
            intg.f1 = (gama => 0);

            intg.g2 = ((nu, gama) => 1 - nu);
            intg.g1 = ((nu, gama) => 0);

            intg.H = 6;
            intg.W = 1;

            intg.G = (nu, gama, xi) =>
            {
                var tmpb = new double[]
                {
                    N1(xi, nu),
                    N2(xi, nu),
                    N3(xi, nu),
                    N4(xi, nu),
                    N5(xi, nu),
                    N6(xi, nu)
                };

                var mtx = new Matrix(tmpb);
                mtx.MultiplyByConstant(q);
                return mtx;
            };

            var res = intg.Integrate();
            */
            /*
            var f1 = new Force(0, 0, res[0, 0], res[1, 0], res[2, 0], 0);//for node 1, in local system
            var f2 = new Force(0, 0, res[3, 0], res[4, 0], res[5, 0], 0);//for node 2, in local system
            var f3 = new Force(0, 0, res[6, 0], res[7, 0], res[8, 0], 0);//for node 3, in local system
            */
            var f1 = new Force(0, 0, q*a/3, 0, 0, 0); //for node 1, in local system
            var f2 = new Force(0, 0, q*a/3, 0, 0, 0); //for node 2, in local system
            var f3 = new Force(0, 0, q*a/3, 0, 0, 0); //for node 3, in local system

            var f1g = TranformLocalToGlobal(f1.Forces);
            var m1g = TranformLocalToGlobal(f1.Moments);

            var f2g = TranformLocalToGlobal(f2.Forces);
            var m2g = TranformLocalToGlobal(f2.Moments);

            var f3g = TranformLocalToGlobal(f3.Forces);
            var m3g = TranformLocalToGlobal(f3.Moments);

            var buf = new Force[] {new Force(f1g, m1g), new Force(f2g, m2g), new Force(f3g, m3g)};

            return buf;
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public override Force[] GetGlobalEquivalentNodalLoads(ElementalLoad load)
        {
            if (load is UniformLoadForPlanarElements)
                return GetEquivalentNodalLoad(load as UniformLoadForPlanarElements);

            return new Force[3];
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
