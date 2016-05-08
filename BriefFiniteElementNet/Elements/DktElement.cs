using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using BriefFiniteElementNet.Integration;

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
    public class DktElement : Element2D
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
        public DktElement() : base(3)
        {
            this.elementType = ElementType.Dkt;
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
        private Matrix GetTransformationMatrix()
        {
            //these calculations are noted in page [72 of 166] (page 81 of pdf) of "Development of Membrane, Plate and Flat Shell Elements in Java" thesis by Kaushalkumar Kansara available on the web

            var cp = new Point[3];// clockwise points

            cp = nodes.Select(i => i.Location).ToArray();

            var p1 = cp[0] - Point.Origins;
            var p2 = cp[1] - Point.Origins;
            var p3 = cp[2] - Point.Origins;

            var ii = (p1 + p2) / 2;
            var jj = (p2 + p3) / 2;
            var kk = (p3 + p1) / 2;

            var vx = (jj - kk).GetUnit();//eq. 5.3
            var vr = (ii - p3).GetUnit();//eq. 5.5
            var vz = Vector.Cross(vx, vr);//eq. 5.6
            var vy = Vector.Cross(vz, vx);//eq. 5.7

            var lamX = vx.GetUnit();//Lambda_x
            var lamY = vy.GetUnit();//Lambda_x
            var lamZ = vz.GetUnit();//Lambda_x

            var lambda = new Matrix(new[]
            {
                new[] {lamX.X, lamY.X, lamZ.X},
                new[] {lamX.Y, lamY.Y, lamZ.Y},
                new[] {lamX.Z, lamY.Z, lamZ.Z}
            });//eq. 5.13

            return lambda;
        }

        /// <inheritdoc />
        public override Matrix GetGlobalStifnessMatrix()
        {
            //throw new NotImplementedException();
            //code not tested yet!

            var local = GetLocalStifnessMatrix();
            //var local2 = GetLocalStifnessMatrix2();

            //var d = local - local2;


            var lambda = GetTransformationMatrix();

            var t = Matrix.DiagonallyRepeat(lambda.Transpose(), 6);// eq. 5-16 page 78 (87 of file)

            var buf = t.Transpose() * local * t;//eq. 5-15 p77

            //var tmp = (buf - buf.Transpose()).Max(i=>Math.Abs(i));

            return buf;
        }

        /// <summary>
        /// Gets the transformation matrix which converts local and global coordinates to each other.
        /// </summary>
        /// <returns>Transformation matrix.</returns>
        public Matrix GetLocalStifnessMatrix_old()
        {
            //TODO: check with this one: https://github.com/ahojukka5/FEMTools/blob/dfecb12d445feeac94ea2e8bcb2b6cc785fbaa72/TeeFEM/examples/platemodels.py

            var lpts = GetLocalPoints();

            var a = GetArea();

            var d = new Matrix(3, 3);

            {
                //page 50 of pdf

                var cf = this._elasticModulus * this._thickness * this._thickness * this._thickness /
                         (12 * (1 - _poissonRatio * _poissonRatio));

                d[0, 0] = d[1, 1] = 1;
                d[1, 0] = d[0, 1] = _poissonRatio;
                d[2, 2] = (1 - _poissonRatio) / 2;

                d.MultiplyByConstant(cf);
            }

            var eval_points = new List<double[]>();//eval_points[i][0] : kesi, eval_points[i][1] : no, eval_points[i][2] : weight

            eval_points.Add(new[] { 0.5, 0.0, 1 / 3.0 });
            eval_points.Add(new[] { 0.5, 0.5, 1 / 3.0 });
            eval_points.Add(new[] { 0.0, 0.5, 1 / 3.0 });

            var klocal = new Matrix(9, 9);

            //start of eq. 4.32 page 49 ref [1]
            for (var i = 0; i < eval_points.Count; i++)
            {
                for (var j = 0; j < eval_points.Count; j++)
                {
                    var wi = eval_points[i][2];
                    var wj = eval_points[j][2];

                    var xi_i = eval_points[i][0];
                    var eta_j = eval_points[j][1];

                    var vi = eval_points[i][0];
                    var vj = eval_points[j][1];



                    var b = GetBMatrix(xi_i, eta_j,lpts);

                    var ki = b.Transpose() * d * b;

                    ki.MultiplyByConstant( wi * wj);

                    klocal += ki;
                }
            }

            klocal.MultiplyByConstant(2*a);
            //end of eq. 4.32 page 49 ref [1]



            //klocal DoF order is as noted in eq. 4.21 p. 44 : w1, θx1, θy1, w2, θx2, θy2, w3, θx3, θy3
            //it should convert to u1, v1, w1, θx1, θy1, θz1, ... and so on
            //will done with permutation matrix p, this formulation is not noted in any reference of this doc
            var p = new Matrix(6, 3);
            p[2, 0] = p[3, 1] = p[4, 2] = 1;

            var pt = Matrix.DiagonallyRepeat(p, 3);

            var buf2 = (pt * klocal) * pt.Transpose();//local expanded stiffness matrix

            return buf2;
        }

        /// <summary>
        /// Gets the transformation matrix which converts local and global coordinates to each other.
        /// </summary>
        /// <returns>Transformation matrix.</returns>
        public Matrix GetLocalStifnessMatrix()
        {
            var lpts = GetLocalPoints();

            var a = GetArea();

            var d = new Matrix(3, 3);

            {
                //page 50 of pdf

                var cf = this._elasticModulus * this._thickness * this._thickness * this._thickness /
                         (12 * (1 - _poissonRatio * _poissonRatio));

                d[0, 0] = d[1, 1] = 1;
                d[1, 0] = d[0, 1] = _poissonRatio;
                d[2, 2] = (1 - _poissonRatio) / 2;

                d.MultiplyByConstant(cf);
            }

            /*
            var eval_points = new List<double[]>();//eval_points[i][0] : kesi, eval_points[i][1] : no, eval_points[i][2] : weight

            eval_points.Add(new[] { 0.5, 0.0, 1 / 3.0 });
            eval_points.Add(new[] { 0.5, 0.5, 1 / 3.0 });
            eval_points.Add(new[] { 0.0, 0.5, 1 / 3.0 });
            */


            var intg = new Integration.GaussianIntegrator();

            intg.A2 = 1;
            intg.A1 = 0;

            intg.F2 = (gama => 1);
            intg.F1 = (gama => 0);

            intg.G2 = ((nu, gama) => 1 - nu);
            intg.G1 = ((nu, gama) => 0);

            intg.XiPointCount = intg.NuPointCount = 3;
            intg.GammaPointCount = 1;

            intg.H = new FunctionMatrixFunction((xi, nu, gamma) =>
            {
                var b = GetBMatrix(xi, nu, lpts);

                var ki = b.Transpose()*d*b;

                return ki;
            });

            /**/

            var res = intg.Integrate();

            res.MultiplyByConstant(2*a);

           

            //klocal DoF order is as noted in eq. 4.21 p. 44 : w1, θx1, θy1, w2, θx2, θy2, w3, θx3, θy3
            //it should convert to u1, v1, w1, θx1, θy1, θz1, ... and so on
            //will done with permutation matrix p, this formulation is not noted in any reference of this doc

            var p = new Matrix(6, 3);
            p[2, 0] = p[3, 1] = p[4, 2] = 1;

            var pt = Matrix.DiagonallyRepeat(p, 3);

            var buf2 = (pt * res) * pt.Transpose();//local expanded stiffness matrix

            //adding drilling dof:

            var maxKii = Enumerable.Range(0, res.RowCount).Max(i => res[i, i]);
            buf2[5, 5] = buf2[11, 11] = buf2[17, 17] = maxKii/1e3;//eq 5.2, p. 71 ref [1]

            return buf2;
        }


        public Matrix GetLocalStifnessMatrix2()
        {
            //Based on this article: Structural Analysis with the Finite Element Method Linear Statics by Eugenio Oñate p. 352

            /*
            var lpts = GetLocalPoints();

            var a = GetArea();


            var d = new Matrix(3, 3);

            {
                //page 50 of pdf

                var cf = this._elasticModulus * this._thickness * this._thickness * this._thickness /
                         (12 * (1 - _poissonRatio * _poissonRatio));

                d[0, 0] = d[1, 1] = 1;
                d[1, 0] = d[0, 1] = _poissonRatio;
                d[2, 2] = (1 - _poissonRatio) / 2;

                d.MultiplyByConstant(cf);
            }

            var R = Matrix.Ones(3) + Matrix.Identity(3);
            var x = new double[] { lpts[0].X, lpts[1].X, lpts[2].X };
            var y = new double[] { lpts[0].Y, lpts[1].Y, lpts[2].Y };

            var x1 = x[0] ;
            var x2 = x[1] ;
            var x3 = x[2] ;

            var y1 = y[0];
            var y2 = y[1];
            var y3 = y[2];

            var x23 = x[1] - x[2];
            var x31 = x[2] - x[0];
            var x12 = x[0] - x[1];

            var y23 = y[1] - y[2];
            var y31 = y[2] - y[0];
            var y12 = y[0] - y[1];


            var l31_2 = x31*x31 + y31*y31;
            var l12_2 = x12*x12 + y12*y12;
            var l23_2 = x23*x23 + y23*y23;

            var p5 = -6*x3/l31_2;
            var t5 = -6*y3/l31_2;
            var q5 = 3*x3*y3/l31_2;
            var r4 = 3 * y23 * y23 / l23_2;
            var r5 = 3 * y31 * y31 / l31_2;

            var a4 = -x23 / l23_2;
            var a5 = -x31 / l31_2;
            var a6 = -x12 / l12_2;

            var b4 = 3.0 / 4.0 * x23 * y23 / l23_2;
            var b5 = 3.0 / 4.0 * x31 * y31 / l31_2;
            var b6 = 3.0 / 4.0 * x12 * y12 / l12_2;

            var c4 = (0.25 * x23 * x23 - 0.5 * y23 * y23) / l23_2;
            var c5 = (0.25 * x31 * x31 - 0.5 * y31 * y31) / l31_2;
            var c6 = (0.25 * x12 * x12 - 0.5 * y12 * y12) / l12_2;

            var d4 = -y23 / l23_2;
            var d5 = -y31 / l31_2;
            var d6 = -y12 / l12_2;

            var l4 = (0.25 * y23 * y23 - 0.5 * x23 * x23) / l23_2;
            var l5 = (0.25 * y31 * y31 - 0.5 * x31 * x31) / l31_2;
            var l6 = (0.25 * y12 * y12 - 0.5 * x12 * x12) / l12_2;

            var s1 = new double[] { 6 * y3 * a6, 0, -4 * y3, -6 * y3 * a6, 0, -2 * y3, 0, 0, 0 };//first line of s
            var s2 = new double[] {-6*y3*a6, 0, 2*y3, 6*y3*a6, 0, 4*y3, 0, 0, 0};
            var s3 = new double[]
            {y3*p5, -y3*q5, y3*(2 - r5), 6*y3*a4, 4*y3*b4, y3*(r4 - 2), -y3*(6*a4 + p5), y3*(4*b4 - q5), y3*(r4 - r5)};

            var s4 = new double[] { -x2 * t5, x23 + x2 * r5, -x2 * q5, 0, x3, 0, x2 * t5, x2 * (r5 - 1), x2 * q5 };
            var s5 = new double[] { 0, x23, 0, 6 * x2 * d4, x3 + x2 * r4, -4 * x2 * b4, 6 * x2 * d4, x2 * (r4 - 1), -4 * x2 * b4 };
            var s6 = new double[] { x23 * t5, x23 * (1 - r5), x23 * q5, -6 * x3 * d4, x3 * (1 - r4), 4 * x3 * b4, -x23 * t5 + 6 * x3 * d4, -x23 * r5 - x3 * r4 - x2, 4 * x3 * b4 + x23 * q5 };


            var s7 = new double[]
            {-6*x3*a6 - x2*p5, (x2*q5 + y3), -4*x23 + x2*r5, 6*x3*a6, -y3, 2*x3, x2*p5, x2*q5, (r5 - 2)*x2};

            var s8 = new double[]
            {-6*x23*a6, y3, 2*x23, 6*x23*a6 + 6*x2*a4, -(y3 + 4*x2*b4), (-4*x3 + x2*r4) - 6*x2*a4, 4*x2*b4, (r4 - 2)*x2};

            var q4 = 0.0;//TODO: it is not defined in book formulation

            var s9 = new double[] {x23*p5+y3*t5,-x23*p5+(1 - r5)*y3,(2 - r5)*x23+y3*q5,6*x3*q4-6*y3*d4,(r4 - 1)*y3 -4*x3*b4,(2 - r4)*x3-4*y3*b4,-x23*p5+6*x3*a4-(6*d4 + t5)*y3,-x23*q5-4*x3*b4+(r4 - r5)*y3,x23*r5-x3*r4+4*x2+(q5 - 4b4)*y3};


            var p = new Matrix(6, 3);
            p[2, 0] = p[3, 1] = p[4, 2] = 1;

            var pt = Matrix.DiagonallyRepeat(p, 3);

            var buf2 = (pt * klocal) * pt.Transpose();//local expanded stiffness matrix

            return buf2;
            */
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the b matrix.
        /// </summary>
        /// <param name="k">The k.</param>
        /// <param name="e">The e.</param>
        /// <param name="lpts">The local points.</param>
        /// <returns></returns>
        private Matrix GetBMatrix(double k, double e, Point[] lpts)
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

                    return Matrix.FromRowColCoreArray(9, 1, buf);
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

                    return Matrix.FromRowColCoreArray(9, 1, buf);
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

                    return Matrix.FromRowColCoreArray(9, 1, buf);
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

                    return Matrix.FromRowColCoreArray(9, 1, buf);
                });//eq. 4.30 ref [1], also noted in several other references


            #endregion

            var B = new Func<double, double, Matrix>(
               (xi, eta) =>
               {
                   var b1 = (y31 * hx_xi(xi, eta) + y12 * hx_eta(xi, eta));
                   var b2 = (-x31 * hy_xi(xi, eta) - x12 * hy_eta(xi, eta));
                   var b3 =
                       (-x31 * hx_xi(xi, eta) - x12 * hx_eta(xi, eta) + y31 * hy_xi(xi, eta) + y12 * hy_eta(xi, eta));

                   var buf = new Matrix(new[] { b1.CoreArray, b2.CoreArray, b3.CoreArray });

                   buf.MultiplyByConstant(1 / (2 * a));

                   return buf;
               });//eq. 4.26 page 46 ref [1]

            return B(k, e);
        }

        private Matrix GetBMatrixPy(double k, double e)
        {
            // Ported from teefem/models/dkt.py

            double x1, x2, x3, y1, y2, y3;
            var locs = GetLocalPoints();

            x1 = locs[0].X;
            y1 = locs[0].Y;

            x2 = locs[1].X;
            y2 = locs[1].Y;

            x3 = locs[2].X;
            y3 = locs[2].Y;

            var x12 = x1 - x2;
            var x31 = x3 - x1;
            var x23 = x2 - x3;
            var y12 = y1 - y2;
            var y31 = y3 - y1;
            var y23 = y2 - y3;
            var A2 = x31*y12 - x12*y31;
            var A = A2/2;

            var l12_2 = (x12*x12 + y12*y12);
            var l23_2 = (x23*x23 + y23*y23);
            var l31_2 = (x31*x31 + y31*y31);


            var P4 = -6*x23/l23_2;
            var P5 = -6*x31/l31_2;
            var P6 = -6*x12/l12_2;
            var t4 = -6*y23/l23_2;
            var t5 = -6*y31/l31_2;
            var t6 = -6*y12/l12_2;
            var q4 = 3*x23*y23/l23_2;
            var q5 = 3*x31*y31/l31_2;
            var q6 = 3*x12*y12/l12_2;
            var r4 = 3*y23*y23/l23_2;
            var r5 = 3*y31*y31/l31_2;
            var r6 = 3*y12*y12/l12_2;


            var Hxk = new Matrix(new[]
            {
                P6*(1 - 2*k) + (P5 - P6)*e,
                q6*(1 - 2*k) - (q5 + q6)*e,
                -4 + 6*(k + e) + r6*(1 - 2*k) - e*(r5 + r6),
                -P6*(1 - 2*k) + e*(P4 + P6),
                q6*(1 - 2*k) - e*(q6 - q4),
                -2 + 6*k + r6*(1 - 2*k) + e*(r4 - r6),
                -e*(P5 + P4),
                e*(q4 - q5),
                -e*(r5 - r4)
            });

            var Hyk = new Matrix(new[]
            {
                t6*(1 - 2*k) + e*(t5 - t6),
                1 + r6*(1 - 2*k) - e*(r5 + r6),
                -q6*(1 - 2*k) + e*(q5 + q6),
                -t6*(1 - 2*k) + e*(t4 + t6),
                -1 + r6*(1 - 2*k) + e*(r4 - r6),
                -q6*(1 - 2*k) - e*(q4 - q6),
                -e*(t4 + t5),
                e*(r4 - r5),
                -e*(q4 - q5)
            });

            var Hxe = new Matrix(new[]
            {
                -P5*(1 - 2*e) - k*(P6 - P5),
                q5*(1 - 2*e) - k*(q5 + q6),
                -4 + 6*(k + e) + r5*(1 - 2*e) - k*(r5 + r6),
                k*(P4 + P6),
                k*(q4 - q6),
                -k*(r6 - r4),
                P5*(1 - 2*e) - k*(P4 + P5),
                q5*(1 - 2*e) + k*(q4 - q5),
                -2 + 6*e + r5*(1 - 2*e) + k*(r4 - r5)
            });

            var Hye = new Matrix(new[]
            {
                - t5*(1 - 2*e) - k*(t6 - t5),
                1 + r5*(1 - 2*e) - k*(r5 + r6),
                -q5*(1 - 2*e) + k*(q5 + q6),
                k*(t4 + t6),
                k*(r4 - r6),
                -k*(q4 - q6),
                t5*(1 - 2*e) - k*(t4 + t5),
                -1 + r5*(1 - 2*e) + k*(r4 - r5),
                -q5*(1 - 2*e) - k*(q4 - q5)
            });


            var b1 = y31*Hxk + y12*Hxe;
            var b2 = -x31*Hyk - x12*Hye;
            var b3 = -x31*Hxk - x12*Hxe + y31*Hyk + y12*Hye;

            var buf = new Matrix(new []
            {
                b1.CoreArray, b2.CoreArray, b3.CoreArray
            });

            buf.MultiplyByConstant(1/A2);

            return buf;
        }


        private Matrix GetBMatrixOofem(double ksi, double eta)
        {
            // get node coordinates
            //Ported from oofem library

            double x1, x2, x3, y1, y2, y3;
            var locs = GetLocalPoints();

            x1 = locs[0].X;
            y1 = locs[0].Y;

            x2 = locs[1].X;
            y2 = locs[1].Y;

            x3 = locs[2].X;
            y3 = locs[2].Y;

            double N1dk = 4.0 * ksi - 1.0;
            double N2dk = 0.0;
            double N3dk = -3.0 + 4.0 * ksi + 4.0 * eta;
            double N4dk = 4.0 * eta;
            double N5dk = -4.0 * eta;
            double N6dk = 4.0 * (1.0 - 2.0 * ksi - eta);

            double N1de = 0.0;
            double N2de = 4.0 * eta - 1.0;
            double N3de = -3.0 + 4.0 * eta + 4.0 * ksi;
            double N4de = 4.0 * ksi;
            double N5de = 4.0 * (1.0 - 2.0 * eta - ksi);
            double N6de = -4.0 * ksi;

            double A = N1dk * x1 + N2dk * x2 + N3dk * x3 + N4dk * (x1 + x2) / 2 + N5dk * (x2 + x3) / 2 + N6dk * (x1 + x3) / 2;
            double B = N1dk * y1 + N2dk * y2 + N3dk * y3 + N4dk * (y1 + y2) / 2 + N5dk * (y2 + y3) / 2 + N6dk * (y1 + y3) / 2;
            double C = N1de * x1 + N2de * x2 + N3de * x3 + N4de * (x1 + x2) / 2 + N5de * (x2 + x3) / 2 + N6de * (x1 + x3) / 2;
            double D = N1de * y1 + N2de * y2 + N3de * y3 + N4de * (y1 + y2) / 2 + N5de * (y2 + y3) / 2 + N6de * (y1 + y3) / 2;

            double dxdk = D / (A * D - B * C);
            double dydk = -C / (A * D - B * C);
            double dxde = -B / (A * D - B * C);
            double dyde = A / (A * D - B * C);

            double dN102 = N1dk * dxdk + N1de * dxde;
            double dN104 = N2dk * dxdk + N2de * dxde;
            double dN106 = N3dk * dxdk + N3de * dxde;
            double dN108 = N4dk * dxdk + N4de * dxde;
            double dN110 = N5dk * dxdk + N5de * dxde;
            double dN112 = N6dk * dxdk + N6de * dxde;

            double dN201 = -N1dk * dydk - N1de * dyde;
            double dN203 = -N2dk * dydk - N2de * dyde;
            double dN205 = -N3dk * dydk - N3de * dyde;
            double dN207 = -N4dk * dydk - N4de * dyde;
            double dN209 = -N5dk * dydk - N5de * dyde;
            double dN211 = -N6dk * dydk - N6de * dyde;

            // normals on element sides
            double dx4 = x2 - x1;
            double dy4 = y2 - y1;
            double l4 = Math.Sqrt(dx4 * dx4 + dy4 * dy4);

            double dx5 = x3 - x2;
            double dy5 = y3 - y2;
            double l5 = Math.Sqrt(dx5 * dx5 + dy5 * dy5);

            double dx6 = x1 - x3;
            double dy6 = y1 - y3;
            double l6 = Math.Sqrt(dx6 * dx6 + dy6 * dy6);

            double c4 = dy4 / l4;
            double s4 = -dx4 / l4;

            double c5 = dy5 / l5;
            double s5 = -dx5 / l5;

            double c6 = dy6 / l6;
            double s6 = -dx6 / l6;

            // transformation matrix between element DOFs (w, fi_x, fi_y)  and DOFs of element with qudratic rotations
            double T11 = -1.5 / l4 * c4;
            double T12 = -0.25 * c4 * c4 + 0.5 * s4 * s4;
            double T13 = -0.25 * c4 * s4 - 0.5 * c4 * s4;
            double T14 = 1.5 / l4 * c4;
            double T15 = -0.25 * c4 * c4 + 0.5 * s4 * s4;
            double T16 = -0.25 * c4 * s4 - 0.5 * c4 * s4;

            double T21 = -1.5 / l4 * s4;
            double T22 = -0.25 * c4 * s4 - 0.5 * c4 * s4;
            double T23 = -0.25 * s4 * s4 + 0.5 * c4 * c4;
            double T24 = 1.5 / l4 * s4;
            double T25 = -0.25 * c4 * s4 - 0.5 * c4 * s4;
            double T26 = -0.25 * s4 * s4 + 0.5 * c4 * c4;

            double T34 = -1.5 / l5 * c5;
            double T35 = -0.25 * c5 * c5 + 0.5 * s5 * s5;
            double T36 = -0.25 * c5 * s5 - 0.5 * c5 * s5;
            double T37 = 1.5 / l5 * c5;
            double T38 = -0.25 * c5 * c5 + 0.5 * s5 * s5;
            double T39 = -0.25 * c5 * s5 - 0.5 * c5 * s5;

            double T44 = -1.5 / l5 * s5;
            double T45 = -0.25 * c5 * s5 - 0.5 * c5 * s5;
            double T46 = -0.25 * s5 * s5 + 0.5 * c5 * c5;
            double T47 = 1.5 / l5 * s5;
            double T48 = -0.25 * c5 * s5 - 0.5 * c5 * s5;
            double T49 = -0.25 * s5 * s5 + 0.5 * c5 * c5;

            double T51 = 1.5 / l6 * c6;
            double T52 = -0.25 * c6 * c6 + 0.5 * s6 * s6;
            double T53 = -0.25 * c6 * s6 - 0.5 * c6 * s6;
            double T57 = -1.5 / l6 * c6;
            double T58 = -0.25 * c6 * c6 + 0.5 * s6 * s6;
            double T59 = -0.25 * c6 * s6 - 0.5 * c6 * s6;

            double T61 = 1.5 / l6 * s6;
            double T62 = -0.25 * c6 * s6 - 0.5 * c6 * s6;
            double T63 = -0.25 * s6 * s6 + 0.5 * c6 * c6;
            double T67 = -1.5 / l6 * s6;
            double T68 = -0.25 * c6 * s6 - 0.5 * c6 * s6;
            double T69 = -0.25 * s6 * s6 + 0.5 * c6 * c6;

            var buf = new Matrix(3, 9);

            buf[0, 1 - 1] = T21 * dN108 + T61 * dN112;
            buf[0, 2 - 1] = T22 * dN108 + T62 * dN112;
            buf[0, 3 - 1] = dN102 + T23 * dN108 + T63 * dN112;
            buf[0, 4 - 1] = T24 * dN108 + T44 * dN110;
            buf[0, 5 - 1] = T25 * dN108 + T45 * dN110;
            buf[0, 6 - 1] = dN104 + T26 * dN108 + T46 * dN110;
            buf[0, 7 - 1] = T47 * dN110 + T67 * dN112;
            buf[0, 8 - 1] = T48 * dN110 + T68 * dN112;
            buf[0, 9 - 1] = dN106 + T49 * dN110 + T69 * dN112;

            buf[1, 1 - 1] = T11 * dN207 + T51 * dN211;
            buf[1, 2 - 1] = dN201 + T12 * dN207 + T52 * dN211;
            buf[1, 3 - 1] = T13 * dN207 + T53 * dN211;
            buf[1, 4 - 1] = T14 * dN207 + T34 * dN209;
            buf[1, 5 - 1] = dN203 + T15 * dN207 + T35 * dN209;
            buf[1, 6 - 1] = T16 * dN207 + T36 * dN209;
            buf[1, 7 - 1] = T37 * dN209 + T57 * dN211;
            buf[1, 8 - 1] = dN205 + T38 * dN209 + T58 * dN211;
            buf[1, 9 - 1] = T39 * dN209 + T59 * dN211;

            buf[2, 1 - 1] = -T11 * dN108 - T51 * dN112 - T21 * dN207 - T61 * dN211;
            buf[2, 2 - 1] = -dN102 - T12 * dN108 - T52 * dN112 - T22 * dN207 - T62 * dN211;
            buf[2, 3 - 1] = -dN201 - T13 * dN108 - T53 * dN112 - T23 * dN207 - T63 * dN211;
            buf[2, 4 - 1] = -T14 * dN108 - T34 * dN110 - T24 * dN207 - T44 * dN209;
            buf[2, 5 - 1] = -dN104 - T15 * dN108 - T35 * dN110 - T25 * dN207 - T45 * dN209;
            buf[2, 6 - 1] = -dN203 - T16 * dN108 - T36 * dN110 - T26 * dN207 - T46 * dN209;
            buf[2, 7 - 1] = -T37 * dN110 - T57 * dN112 - T47 * dN209 - T67 * dN211;
            buf[2, 8 - 1] = -dN106 - T38 * dN110 - T58 * dN112 - T48 * dN209 - T68 * dN211;
            buf[2, 9 - 1] = -dN205 - T39 * dN110 - T59 * dN112 - T49 * dN209 - T69 * dN211;

            // Note: no shear strains, no shear forces => the 4th and 5th rows are zero

            return buf;
        }

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
            var v1 = nodes[1].Location - nodes[0].Location;
            var v2 = nodes[2].Location - nodes[0].Location;

            var cross = Vector.Cross(v1, v2);
            return cross.Length/2;
        }



        /// <summary>
        /// Gets the equivalent nodal load. in global coordination system.
        /// </summary>
        /// <param name="load">The load.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Force[] GetEquivalentNodalLoad(UniformLoadForPlanarElements load)
        {
            var N1 = new Func<double, double, double>((xi, nu) => 2*(1 - xi - nu)*(0.5 - xi - nu));
            var N2 = new Func<double, double, double>((xi, nu) => xi*(2.0*xi - 1));
            var N3 = new Func<double, double, double>((xi, nu) => nu*(2.0*nu - 1));
            var N4 = new Func<double, double, double>((xi, nu) => 4*xi*nu);
            var N5 = new Func<double, double, double>((xi, nu) => 4*nu*(1 - xi - nu));
            var N6 = new Func<double, double, double>((xi, nu) => 4*nu*(1 - xi - nu));

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
    }
}