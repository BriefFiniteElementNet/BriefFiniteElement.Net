using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Integration;

namespace BriefFiniteElementNet.Elements
{

    /// <summary>
    /// represents a constant strain triangle element with constant thickness.
    /// Whole formulation is taken from "Development of Membrane, Plate and Flat Shell Elements in Java" thesis by Kaushalkumar Kansara available on the web
    /// </summary>
    [Serializable]
    [Obsolete("Use Triangle Plate Shell with Behavior = Membrane")]
    public class CstElement : Element2D
    {
        private double _thickness;

        private double _poissonRatio;

        private double _elasticModulus;

        private MembraneFormulation _formulationType;

        /// <inheritdoc/>
        public override Point IsoCoordsToGlobalLocation(params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

       

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
        /// Gets or sets the type of the formulation.
        /// </summary>
        /// <value>
        /// The type of the formulation.
        /// </value>
        public MembraneFormulation FormulationType
        {
            get { return _formulationType; }
            set { _formulationType = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CstElement"/> class.
        /// </summary>
        public CstElement()
            : base(3)
        {
            this.elementType = ElementType.Cst;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CstElement"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected CstElement(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this._thickness = info.GetDouble("_thickness");
            this._poissonRatio = info.GetDouble("_poissonRatio");
            this._elasticModulus = info.GetDouble("_elasticModulus");
            this._formulationType = (MembraneFormulation)info.GetInt32("_formulationType");
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_thickness", this._thickness);
            info.AddValue("_poissonRatio", this._poissonRatio);
            info.AddValue("_elasticModulus", this._elasticModulus);
            info.AddValue("_formulationType", (int)this._formulationType);

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


            var p0 = (t * g0).ToPoint();
            var p1 = (t * g1).ToPoint();
            var p2 = (t * g2).ToPoint();

            return new[] { p0, p1, p2 };
        }

        internal Vector TranformLocalToGlobal(Vector v)
        {
            var mtx = GetTransformationMatrix();

            var b = mtx * v.ToMatrix();

            return new Vector(b[0, 0], b[1, 0], b[2, 0]);
        }

        internal Vector TranformGlobalToLocal(Vector v)
        {
            var mtx = GetTransformationMatrix().Transpose();

            var b = mtx * v.ToMatrix();

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
            var lamY = vy.GetUnit();//Lambda_y
            var lamZ = vz.GetUnit();//Lambda_z

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
            //step 1 : get points in local system
            //step 2 : get local stiffness matrix
            //step 3 : expand local stiffness matrix
            //step 4 : get global stiffness matrix

            //step 1
            var ls = GetLocalPoints();

            var xs = new [] { ls[0].X, ls[1].X, ls[2].X };
            var ys = new [] { ls[0].Y, ls[1].Y, ls[2].Y };

            //step 2
            var kl = CstElement.GetStiffnessMatrix(xs, ys, this._thickness, this.ElasticModulus, this.PoissonRatio,
                this._formulationType);

            //step 3
            var currentOrder = new []
            {
                new FluentElementPermuteManager.ElementLocalDof(0, DoF.Dx),
                new FluentElementPermuteManager.ElementLocalDof(0, DoF.Dy),

                new FluentElementPermuteManager.ElementLocalDof(1, DoF.Dx),
                new FluentElementPermuteManager.ElementLocalDof(1, DoF.Dy),

                new FluentElementPermuteManager.ElementLocalDof(2, DoF.Dx),
                new FluentElementPermuteManager.ElementLocalDof(2, DoF.Dy),
            };

            var kle = FluentElementPermuteManager.FullyExpand(kl, currentOrder, 3);

            var lambda = GetTransformationMatrix();

            var tr = Matrix.DiagonallyRepeat(lambda.Transpose(), 6); // eq. 5-16 page 78 (87 of file)

            //step 4 : get global stiffness matrix
            var buf = tr.Transpose() * kle * tr; //eq. 5-15 p77

            return buf;
        }

        /// <summary>
        /// Gets the transformation matrix which converts local and global coordinates to each other.
        /// </summary>
        /// <returns>Transformation matrix.</returns>
        public Matrix GetLocalStifnessMatrix()
        {
            throw new NotImplementedException();
            /*
            var lpts = GetLocalPoints();

            var d = GetDMatrix(_elasticModulus, _poissonRatio, _formulationType);

            var b = GetBMatrix(0, 0, lpts);//only one Gaussian point

            var klocal = b.Transpose()*d*b;

            klocal.MultiplyByConstant(_thickness*GetArea());

            var p = new Matrix(6, 2);
            p[0, 0] = p[1, 1] = 1;

            var pt = Matrix.DiagonallyRepeat(p, 3);

            var buf2 = (pt * klocal) * pt.Transpose();//local expanded stiffness matrix

            return buf2;
            */
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
        internal static Matrix GetStiffnessMatrix(double[] x, double[] y,double t, double e, double nu,MembraneFormulation f)
        {
            double a;//area

            {
                var x31 = x[2] - x[0];
                var x12 = x[0] - x[1];
                var y31 = y[2] - y[0];
                var y12 = y[0] - y[1];

                a = 0.5 * Math.Abs(x31 * y12 - x12 * y31);
            }

            var d = GetDMatrix(e, nu, f);

            var b = GetBMatrix(0, 0, x,y);//only one Gaussian point

            var klocal = b.Transpose() * d * b;

            klocal.MultiplyByConstant(t * a);

            return klocal;
        }

        internal static Matrix GetDMatrix(double e, double nu, MembraneFormulation f)
        {
            var d = new Matrix(3, 3);

            if (f == MembraneFormulation.PlaneStress)
            {
                //page 23 of JAVA Thesis pdf

                var cf = e/(1 - nu*nu);

                d[0, 0] = d[1, 1] = 1;
                d[1, 0] = d[0, 1] = nu;
                d[2, 2] = (1 - nu)/2;

                d.MultiplyByConstant(cf);
            }
            else
            {
                //page 24 of JAVA Thesis pdf

                var cf = e/((1 + nu)*(1 - 2*nu));

                d[0, 0] = d[1, 1] = 1 - nu;
                d[1, 0] = d[0, 1] = nu;
                d[2, 2] = (1 - 2*nu)/2;

                d.MultiplyByConstant(cf);
            }

            return d;
        }

        /// <summary>
        /// Gets the b matrix.
        /// </summary>
        /// <param name="xi">The k.</param>
        /// <param name="eta">The e.</param>
        /// <param name="lpts">The local points.</param>
        /// <returns></returns>
        internal static Matrix GetBMatrix(double xi, double eta, double[] x, double[] y)
        {
            var x32 = x[2] - x[1];
            var x13 = x[0] - x[2];
            var x31 = -x13;
            var x21 = x[1] - x[0];
            var x12 = -x21;

            var y23 = y[1] - y[2];
            var y31 = y[2] - y[0];
            var y12 = y[0] - y[1];

            var a = 0.5 * Math.Abs(x31 * y12 - x12 * y31);

            //eq 3.24 page 29 of thesis pdf

            var buf2 = new Matrix(new double[][]
            {
                new [] {y23, 0, y31, 0, y12, 0},
                new [] {0, x32, 0, x13, 0, x21},
                new [] {x32, y23, x13, y31, x21, y12},
            });


            buf2.MultiplyByConstant(1/(2*a));

            return buf2;
        }

        /// <summary>
        /// Gets the b matrix.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        internal static Matrix GetBMatrix(double[] x, double[] y)
        {
            var x32 = x[2] - x[1];
            var x13 = x[0] - x[2];
            var x31 = -x13;
            var x21 = x[1] - x[0];
            var x12 = -x21;

            var y23 = y[1] - y[2];
            var y31 = y[2] - y[0];
            var y12 = y[0] - y[1];

            var a = 0.5 * Math.Abs(x31 * y12 - x12 * y31);

            //eq 3.24 page 29 of thesis pdf

            var buf2 = new Matrix(new double[][]
            {
                new [] {y23, 0, y31, 0, y12, 0},
                new [] {0, x32, 0, x13, 0, x21},
                new [] {x32, y23, x13, y31, x21, y12},
            });


            buf2.MultiplyByConstant(1 / (2 * a));

            return buf2;
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
            var v1 = nodes[1].Location - nodes[0].Location;
            var v2 = nodes[2].Location - nodes[0].Location;

            var cross = Vector.Cross(v1, v2);
            return cross.Length / 2;
        }

        ///<inheritdoc/>
        public override Force[] GetEquivalentNodalLoads(Load load)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public override Matrix ComputeBMatrix(params double[] location)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public override Matrix ComputeDMatrixAt(params double[] location)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public override Matrix ComputeNMatrixAt(params double[] location)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public override Matrix ComputeJMatrixAt(params double[] location)
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
    }
}
