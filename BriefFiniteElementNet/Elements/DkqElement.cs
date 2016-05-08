using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using BriefFiniteElementNet.Integration;

namespace BriefFiniteElementNet.Elements
{
    [Obsolete("This class give wrong results")]
    [Serializable]
    public class DkqElement : Element2D
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

        public DkqElement() : base(4)
        {
            this.elementType = ElementType.Dkq;
        }

        public DkqElement(int nodes) : base(nodes)
        {
            this.elementType = ElementType.Dkq;
        }

        public DkqElement(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this._thickness = info.GetDouble("_thickness");
            this._poissonRatio = info.GetDouble("_poissonRatio");
            this._elasticModulus = info.GetDouble("_elasticModulus");
        }

        public override Matrix GetGlobalStifnessMatrix()
        {
            var local = GetLocalStifnessMatrix();

            var lambda = GetTransformationMatrix();

            var t = Matrix.DiagonallyRepeat(lambda.Transpose(), 8);// eq. 5-17 page 78 (87 of file)

            var buf = t.Transpose() * local * t;//eq. 5-15 p77

            return buf;
        }

        public override Matrix GetGlobalMassMatrix()
        {
            throw new NotImplementedException();
        }

        public override Matrix GetGlobalDampingMatrix()
        {
            throw new NotImplementedException();
        }

        private Matrix GetBMatrix(double xi, double nu, Point[] lpts,out double detJ)
        {
            //formulation based on p. 58 of thesis.

            #region inits

            var x = new double[] {lpts[0].X, lpts[1].X, lpts[2].X, lpts[3].X};
            var y = new double[] {lpts[0].Y, lpts[1].Y, lpts[2].Y, lpts[3].Y};

            var x12 = x[0] - x[1];
            var x23 = x[1] - x[2];
            var x34 = x[2] - x[3];
            var x41 = x[3] - x[0];

            var y12 = y[0] - y[1];
            var y23 = y[1] - y[2];
            var y34 = y[2] - y[3];
            var y41 = y[3] - y[0];

            var l12_2 = y12*y12 + x12*x12;
            var l23_2 = y23*y23 + x23*x23;
            var l34_2 = y34*y34 + x34*x34;
            var l41_2 = y41*y41 + x41*x41;

            //k = 5, 6, 7, 8 ij = 12, 23, 34, 41

            //a_k = - x_ij / l_ij ^ 2
            var a5 = -x12/l12_2;
            var a6 = -x23/l23_2;
            var a7 = -x34/l34_2;
            var a8 = -x41/l41_2;

            //b_k = 3 / 4 * x_ij * y_ij / l_ij ^ 2
            var b5 = 3.0/4.0*x12*y12/l12_2;
            var b6 = 3.0/4.0*x23*y23/l23_2;
            var b7 = 3.0/4.0*x34*y34/l34_2;
            var b8 = 3.0/4.0*x41*y41/l41_2;

            //c_k = (0.25 * x_ij ^ 2 -0.5 y_ij ^ 2) / l_ij ^ 2
            var c5 = (0.25*x12*x12 - 0.5*y12*y12)/l12_2;
            var c6 = (0.25*x23*x23 - 0.5*y23*y23)/l23_2;
            var c7 = (0.25*x34*x34 - 0.5*y34*y34)/l34_2;
            var c8 = (0.25*x41*x41 - 0.5*y41*y41)/l41_2;

            //d_k = - y_ij ^ 2 / l_ij ^ 2
            var d5 = y12*y12/l12_2;
            var d6 = y23*y23/l23_2;
            var d7 = y34*y34/l34_2;
            var d8 = y41*y41/l41_2;

            //e_k = (0.25 * x_ij ^ 2 -0.5 y_ij ^ 2) / l_ij ^ 2
            var e5 = (0.25*y12*y12 - 0.5*x12*x12)/l12_2;
            var e6 = (0.25*y23*y23 - 0.5*x23*x23)/l23_2;
            var e7 = (0.25*y34*y34 - 0.5*x34*x34)/l34_2;
            var e8 = (0.25*y41*y41 - 0.5*x41*x41)/l41_2;


            var n_xi = new double[]
            {
                0,//this is for indexing problem :)
                0.25*(2*xi + nu)*(1 - nu),
                0.25*(2*xi - nu)*(1 - nu),
                0.25*(2*xi + nu)*(1 + nu),
                0.25*(2*xi - nu)*(1 + nu),
                -xi*(1 - nu),
                0.5*(1 - nu*nu),
                -xi*(1 + nu),
                -0.5*(1 - nu*nu),
            };

            var n_nu = new double[]
            {
                0,//this is for indexing problem :)
                0.25*(2*nu + xi)*(1 - xi),
                0.25*(2*nu - xi)*(1 + xi),
                0.25*(2*nu + xi)*(1 + xi),
                0.25*(2*nu - xi)*(1 - xi),
                -0.5*(1 - xi*xi),
                -nu*(1 + xi),
                0.5*(1 - xi*xi),
                -nu*(1 - xi),
            };


            var x21 = x[1] - x[0];
            var y21 = y[1] - y[0];

            var x32 = x[2] - x[1];
            var y32 = y[2] - y[1];


            var J11 = x21 + x34 + nu*(x12 + x34);
            J11 *= 0.25;

            var J12 = y21 + y34 + nu*(y12 + y34);
            J12 *= 0.25;

            var J21 = x32 + x41 + xi*(x12 + x34);
            J21 *= 0.25;

            var J22 = y32 + y41 + xi*(y12 + y34);
            J22 *= 0.25;


            var x42 = x[3] - x[1];
            var y42 = y[3] - y[1];

            var x31 = x[2] - x[0];
            var y31 = y[2] - y[0];

            var det = detJ = 1/8.0*(y42*x31 - y31*x42) + xi/8*(y34*x21 - y21*x34) + nu/8*(y41*x32 - y32*x41);

            var j11 = +J22/det;
            var j12 = -J12/det;
            var j21 = -J21/det;
            var j22 = +J11/det;

            var H_x_xi = new double[]
            {
                3.0/2.0*(a5*n_xi[5] - a8*n_xi[8]),
                b5*n_xi[5] + b8*n_xi[8],
                n_xi[1] - c5*n_xi[5] - c8*n_xi[8],

                3.0/2.0*(a6*n_xi[6] - a5*n_xi[5]),
                b6*n_xi[6] + b5*n_xi[5],
                n_xi[2] - c6*n_xi[6] - c5*n_xi[5],

                3.0/2.0*(a7*n_xi[7] - a6*n_xi[6]),
                b7*n_xi[7] + b6*n_xi[6],
                n_xi[3] - c7*n_xi[7] - c6*n_xi[6],

                3.0/2.0*(a8*n_xi[8] - a7*n_xi[7]),
                b8*n_xi[8] + b7*n_xi[7],
                n_xi[4] - c8*n_xi[8] - c7*n_xi[7],
            };

            var H_y_xi = new double[]
            {
                3.0/2.0*(d5*n_xi[5] - d8*n_xi[8]),
                -n_xi[1] - e5*n_xi[5] - e8*n_xi[8],
                -b5*n_xi[5] - b8*n_xi[8],

                3.0/2.0*(d6*n_xi[6] - d5*n_xi[5]),
                -n_xi[2] - e6*n_xi[6] - e5*n_xi[5],
                -b6*n_xi[6] - b5*n_xi[5],

                3.0/2.0*(d7*n_xi[7] - d6*n_xi[6]),
                -n_xi[3] - e7*n_xi[7] - e6*n_xi[6],
                -b7*n_xi[7] - b6*n_xi[6],

                3.0/2.0*(d8*n_xi[8] - d7*n_xi[7]),
                -n_xi[4] - e8*n_xi[8] - e7*n_xi[7],
                -b8*n_xi[8] - b7*n_xi[7],
            };




            var H_x_nu = new double[]
            {
                3.0/2.0*(a5*n_nu[5] - a8*n_nu[8]),
                b5*n_nu[5] + b8*n_nu[8],
                n_nu[1] - c5*n_nu[5] - c8*n_nu[8],

                3.0/2.0*(a6*n_nu[6] - a5*n_nu[5]),
                b6*n_nu[6] + b5*n_nu[5],
                n_nu[2] - c6*n_nu[6] - c5*n_nu[5],

                3.0/2.0*(a7*n_nu[7] - a6*n_nu[6]),
                b7*n_nu[7] + b6*n_nu[6],
                n_nu[3] - c7*n_nu[7] - c6*n_nu[6],

                3.0/2.0*(a8*n_nu[8] - a7*n_nu[7]),
                b8*n_nu[8] + b7*n_nu[7],
                n_nu[4] - c8*n_nu[8] - c7*n_nu[7],
            };

            var H_y_nu = new double[]
            {
                3.0/2.0*(d5*n_nu[5] - d8*n_nu[8]),
                -n_nu[1] - e5*n_nu[5] - e8*n_nu[8],
                -b5*n_nu[5] - b8*n_nu[8],

                3.0/2.0*(d6*n_nu[6] - d5*n_nu[5]),
                -n_nu[2] - e6*n_nu[6] - e5*n_nu[5],
                -b6*n_nu[6] - b5*n_nu[5],

                3.0/2.0*(d7*n_nu[7] - d6*n_nu[6]),
                -n_nu[3] - e7*n_nu[7] - e6*n_nu[6],
                -b7*n_nu[7] - b6*n_nu[6],

                3.0/2.0*(d8*n_nu[8] - d7*n_nu[7]),
                -n_nu[4] - e8*n_nu[8] - e7*n_nu[7],
                -b8*n_nu[8] - b7*n_nu[7],
            };

            #endregion


            var H_x_x = new double[12];
            var H_y_y = new double[12];

            var H_xy_yx = new double[12];

            for (var i = 0; i < 12; i++)
            {
                H_x_x[i] = j11 * H_x_xi[i] + j12 * H_x_nu[i];

                H_y_y[i] = j21 * H_y_xi[i] + j22 * H_y_nu[i];

                H_xy_yx[i] = j11*H_y_xi[i] + j12*H_y_nu[i] + j21*H_x_xi[i] + j22*H_x_nu[i];
            }//eq 4.48 p. 68

            return new Matrix(new[] {H_x_x, H_y_y, H_xy_yx});
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

            var cp = new Point[4];

            cp = nodes.Select(i => i.Location).ToArray();
            
            var p1 = cp[0] - Point.Origins;
            var p2 = cp[1] - Point.Origins;
            var p3 = cp[2] - Point.Origins;
            var p4 = cp[3] - Point.Origins;

            var ii = (p1 + p2) / 2;
            var jj = (p2 + p3) / 2;
            var kk = (p3 + p4) / 2;
            var ll = (p4 + p1) / 2;

            var vx = (jj - ll);//eq. 5.8

            var vr = (kk - ii);//eq. 5.10
            var vz = Vector.Cross(vx, vr);//eq. 5.11
            var vy = Vector.Cross(vz, vx);//eq. 5.12

            var lamX = vx.GetUnit();
            var lamY = vy.GetUnit();
            var lamZ = vz.GetUnit();

            var lambda = new Matrix(new[]
            {
                new[] {lamX.X, lamY.X, lamZ.X},
                new[] {lamX.Y, lamY.Y, lamZ.Y},
                new[] {lamX.Z, lamY.Z, lamZ.Z}
            });//eq. 5.13

            return lambda;
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

        public Matrix GetLocalStifnessMatrix()
        {
            var lpts = GetLocalPoints();

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

            var intg = new Integration.GaussianIntegrator();


            {
                intg.A2 = 1;
                intg.A1 = 0;

                intg.F2 = (gama => 1);
                intg.F1 = (gama => -1);

                intg.G2 = ((nu, gama) => 1);
                intg.G1 = ((nu, gama) => -1);

                intg.XiPointCount = intg.NuPointCount = 3;
                intg.GammaPointCount = 1;

                intg.H = new FunctionMatrixFunction((xi, nu, gamma) =>
                {
                    double detJ;

                    var b = GetBMatrix(xi, nu, lpts, out detJ);

                    var ki = b.Transpose() * d * b;

                    ki.MultiplyByConstant(detJ);

                    return ki;
                });
            }
           

            var res = intg.Integrate();

            //klocal DoF order is as noted in eq. 4.43 p. 57 : w1, θx1, θy1, w2, θx2, θy2, w3, θx3, θy3, w4, θx4, θy4
            //it should convert to u1, v1, w1, θx1, θy1, θz1, ... and so on
            //will done with permutation matrix p, this formulation is not noted in any reference of this doc

            var p = new Matrix(6, 3);
            p[2, 0] = p[3, 1] = p[4, 2] = 1;

            var pt = Matrix.DiagonallyRepeat(p, 4);

            var buf2 = (pt * res) * pt.Transpose();//local expanded stiffness matrix

            //adding drilling dof:

            var maxKii = Enumerable.Range(0, res.RowCount).Max(i => res[i, i]);
            buf2[5, 5] = buf2[11, 11] = buf2[17, 17] = buf2[23, 23] = maxKii / 1e3;//eq 5.2, p. 71 ref [1]

            return buf2;
        }

        private Point[] GetLocalPoints()
        {
            var t = GetTransformationMatrix().Transpose(); //transpose of t

            var g0 = nodes[0].Location.ToMatrix();
            var g1 = nodes[1].Location.ToMatrix();
            var g2 = nodes[2].Location.ToMatrix();
            var g3 = nodes[3].Location.ToMatrix();


            var p0 = (t * g0).ToPoint();
            var p1 = (t * g1).ToPoint();
            var p2 = (t * g2).ToPoint();
            var p3 = (t * g3).ToPoint();

            return new[] { p0, p1, p2, p3 };
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
        /// Gets the equivalent nodal load. in global coordination system.
        /// </summary>
        /// <param name="load">The load.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Force[] GetEquivalentNodalLoad(UniformLoadForPlanarElements load)
        {
            var q = 0.0; //orthogonal component of load, to be calculated

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

            var localPoints = GetLocalPoints();

            var area = 0.0;

            area += localPoints[0].X * localPoints[1].Y - localPoints[0].Y * localPoints[1].X;
            area += localPoints[1].X * localPoints[2].Y - localPoints[1].Y * localPoints[2].X;
            area += localPoints[2].X * localPoints[3].Y - localPoints[2].Y * localPoints[3].X;
            area += localPoints[3].X * localPoints[0].Y - localPoints[3].Y * localPoints[0].X;

            area *= 0.5;

            if (area < 0)
                area *= -1;

            var f1 = new Force(0, 0, q * area / 3, 0, 0, 0); //for node 1, in local system
            var f2 = new Force(0, 0, q * area / 3, 0, 0, 0); //for node 2, in local system
            var f3 = new Force(0, 0, q * area / 3, 0, 0, 0); //for node 3, in local system
            var f4 = new Force(0, 0, q * area / 3, 0, 0, 0); //for node 3, in local system

            var f1g = TranformLocalToGlobal(f1.Forces);
            var m1g = TranformLocalToGlobal(f1.Moments);

            var f2g = TranformLocalToGlobal(f2.Forces);
            var m2g = TranformLocalToGlobal(f2.Moments);

            var f3g = TranformLocalToGlobal(f3.Forces);
            var m3g = TranformLocalToGlobal(f3.Moments);

            var f4g = TranformLocalToGlobal(f4.Forces);
            var m4g = TranformLocalToGlobal(f4.Moments);

            var buf = new Force[] {new Force(f1g, m1g), new Force(f2g, m2g), new Force(f3g, m3g), new Force(f4g, m4g)};

            return buf;
        }
    }
}