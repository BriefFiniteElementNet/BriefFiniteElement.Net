using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// represents a discrete Kirchoff triangular element with constant thickness.
    /// Whole formulation is taken from "Development of Membrane, Plate and Flat Shell Elements in Java" thesis by Kaushalkumar Kansara available on the web
    /// </summary>
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
            this.elementType = ElementType.Dkt;
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets node coordinates in local coordination system.
        /// Z component of return values should be ignored.
        /// </summary>
        private Point[] GetLocalPoints()
        {
            var t = GetTransformationMatrix().Transpose(); //transpose of t
            var p0 = (t*nodes[0].Location.ToMatrix()).ToPoint();
            var p1 = (t*nodes[1].Location.ToMatrix()).ToPoint();
            var p2 = (t*nodes[2].Location.ToMatrix()).ToPoint();

            return new[] {p0, p1, p2};

            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets the 3x3 transformation matrix which converts local and global coordinates to each other.
        /// </summary>
        /// <returns>Transformation matrix.</returns>
        /// <remarks>
        /// GC = T * LC
        /// where
        ///  GC: Global Coordinates [X;Y;Z] (3x1)
        ///  LC: Local Coordinates [x;y;z] (3x1)
        ///  T: Transformation Matrix (from this method) (3x3)
        /// </remarks>
        private Matrix GetTransformationMatrix()
        {
            //these calculations are a noted in page [72 of 166] (page 81 of pdf) of "Development of Membrane, Plate and Flat Shell Elements in Java" thesis by Kaushalkumar Kansara available on the web

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

            });//3x3 matrix

            return lambda;
        }

        /// <inheritdoc />
        public override Matrix GetGlobalStifnessMatrix()
        {
            var local = GetLocalStifnessMatrix();

            var t1 = GetTransformationMatrix();

            var t = Matrix.DiagonallyRepeat(t1, 6);

            var buf = t.Transpose()*local*t;

            return buf;
        }

        /// <summary>
        /// Gets the transformation matrix which converts local and global coordinates to each other.
        /// </summary>
        /// <returns>Transformation matrix.</returns>
        public Matrix GetLocalStifnessMatrix()
        {
            var lpts = GetLocalPoints();

            #region inits

            var x = new double[] { lpts[0].X, lpts[1].X, lpts[2].X };
            var y = new double[] { lpts[0].Y, lpts[1].Y, lpts[2].Y };

            var x23 = x[1] - x[2];
            var x31 = x[2] - x[0];
            var x12 = x[0] - x[1];

            var y23 = y[1] - y[2];
            var y31 = y[2] - y[0];
            var y12 = y[0] - y[1];

            var a = 0.5 * Math.Abs(x31 * y12 - x12 * y31);

            var l23_2 = y23 * y23 + x23 * x23;
            var l31_2 = y31 * y31 + x31 * x31;
            var l12_2 = y12 * y12 + x12 * x12;



            var p4 = -6 * x23 / l23_2;
            var p5 = -6 * x31 / l31_2;
            var p6 = -6 * x12 / l12_2;

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

            var hxkesi = new Func<double, double, Matrix>(
                (ξ, η) =>
                {
                    var buf = new double[]
                    {
                        p6*(1 - 2*ξ) + (p5 - p6)*η,
                        q6*(1 - 2*ξ) - (q5 + q6)*η,
                        -4 + 6*(ξ + η) + r6*(1 - 2*ξ) - η*(r5 + r6),
                        -p6*(1 - 2*ξ) + η*(p4 + p6),
                        q6*(1 - 2*ξ) - η*(q6 - q4),
                        -2 + 6*ξ + r6*(1 - 2*ξ) + η*(r4 - r6),
                        -η*(p5 + p4),
                        η*(q4 - q5),
                        -η*(r5 - r4)
                    };

                    return Matrix.FromRowColCoreArray(9, 1, buf);
                });



            var hykesi = new Func<double, double, Matrix>(
                (ξ, η) =>
                {
                    var buf = new double[]
                    {
                        t6*(1 - 2*ξ) + η*(t5 - t6),
                        1 + r6*(1 - 2*ξ) - η*(r5 + r6),
                        -q6*(1 - 2*ξ) + η*(q5 + q6),
                        -t6*(1 - 2*ξ) + η*(t4 + t6),
                        -1 + r6*(1 - 2*ξ) + η*(r4 - r6),
                        -q6*(1 - 2*ξ) - η*(q4 - q6),
                        -η*(t4 + t5),
                        η*(r4 - r5),
                        -η*(q4 - q5)
                    };

                    return Matrix.FromRowColCoreArray(9, 1, buf);
                });

            var hxno = new Func<double, double, Matrix>(
                (ξ, η) =>
                {
                    var buf = new double[]
                    {
                        -p5*(1 - 2*η) - (p6 - p5)*ξ,
                        q5*(1 - 2*η) - (q5 + q6)*ξ,
                        -4 + 6*(ξ + η) + r5*(1 - 2*η) - ξ*(r5 + r6),
                        ξ*(p4 + p6),
                        ξ*(q4 - q6),
                        -ξ*(r6 - r4),
                        p5*(1 - 2*η) - ξ*(p4 + p5),
                        q5*(1 - 2*η) + ξ*(q4 - q5),
                        -2 + 6*η + r5*(1 - 2*η) + ξ*(r4 - r5)
                    };

                    return Matrix.FromRowColCoreArray(9, 1, buf);
                });

            var hyno = new Func<double, double, Matrix>(
                (ξ, η) =>
                {
                    var buf = new double[]
                    {
                        -t5*(1 - 2*η) - ξ*(t6 - t5),
                        1 + r5*(1 - 2*η) - ξ*(r5 + r6),
                        -q5*(1 - 2*η) + ξ*(q5 + q6),
                        ξ*(t4 + t6),
                        ξ*(r4 - r6),
                        -ξ*(q4 - q6),
                        t5*(1 - 2*η) - ξ*(t4 + t5),
                        -1 + r5*(1 - 2*η) + ξ*(r4 - r5),
                        -q5*(1 - 2*η) - ξ*(q4 - q5)
                    };

                    return Matrix.FromRowColCoreArray(9, 1, buf);
                });


            #endregion


            var B = new Func<double, double, Matrix>(
                (ξ, η) =>
                {
                    var b1 = y31 * hxkesi(ξ, η).Transpose() + y12 * hxno(ξ, η).Transpose();
                    var b2 = -x31 * hykesi(ξ, η).Transpose() - x12 * hyno(ξ, η).Transpose();
                    var b3 = -x31*hxkesi(ξ, η).Transpose() - x12*hxno(ξ, η).Transpose() + y31*hykesi(ξ, η).Transpose() +
                             y12*hyno(ξ, η).Transpose();

                    var buf = Matrix.VerticalConcat(b1, b2);
                    buf = Matrix.VerticalConcat(buf, b3);

                    buf.MultiplyByConstant(1 / (2 * a));

                    return buf;
                });


            var d = new Matrix(3, 3);

            {
                //page 50 of pdf
                
                var cf = this._elasticModulus*this._thickness*this._thickness*this._thickness/
                         (12*(1 - _poissonRatio*_poissonRatio));

                d[0, 0] = d[1, 1] = 1;
                d[1, 0] = d[0, 1] = _poissonRatio;
                d[2, 2] = (1 - _poissonRatio)/2;

                d.MultiplyByConstant(cf);
            }

            var eval_points = new List<double[]>();//eval_points[i][0] : kesi, eval_points[i][1] : no, eval_points[i][2] : weight

            eval_points.Add(new[] { 0.5, 0.0, 1 / 3.0 });
            eval_points.Add(new[] { 0.5, 0.5, 1 / 3.0 });
            eval_points.Add(new[] { 0.0, 0.5, 1 / 3.0 });

            var klocal = new Matrix(9, 9);

            for (var i = 0; i < eval_points.Count; i++)
            {
                for (var j = 0; j < eval_points.Count; j++)
                {
                    var w = eval_points[i][2] * eval_points[j][2];
                    var kesi = eval_points[i][0];
                    var no = eval_points[i][1];

                    var b = B(kesi, no);
                    //var b = GetBMatrix(no, kesi);

                    //var dd = b2 - b;

                    var bt = b.Transpose();

                    var ki = bt * d * b;


                    klocal += w * ki;
                }
            }


            var p = new Matrix(6, 3);
            p[2, 0] = p[3, 1] = p[4, 2] = 1;

            var pt = Matrix.DiagonallyRepeat(p, 3);

            var buf2 = (pt*klocal)*pt.Transpose();//local expanded stiffness matrix

            return buf2;
        }


        private Matrix GetBMatrix(double ksi,double eta)
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

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}