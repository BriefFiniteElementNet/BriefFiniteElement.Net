using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// represents a discrete Kirchoff triangular element with constant thickness
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DktElement"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected DktElement(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets node coordinates in local coordination system.
        /// Z component of return values should be ignored.
        /// </summary>
        private Point[] GetLocalPoints()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Matrix GetGlobalStifnessMatrix()
        {
            var lpts = GetLocalPoints();

            #region inits

            var x = new double[] {lpts[0].X, lpts[1].X, lpts[2].X};
            var y = new double[] {lpts[0].Y, lpts[1].Y, lpts[2].Y};

            var x23 = x[1] - x[2];
            var x31 = x[2] - x[0];
            var x12 = x[0] - x[1];

            var y23 = y[1] - y[2];
            var y31 = y[2] - y[0];
            var y12 = y[0] - y[1];

            var a = 0.5*Math.Abs(x31*y12 - x12*y31);

            var l23_2 = y23*y23 + x23*x23;
            var l31_2 = y31*y31 + x31*x31;
            var l12_2 = y12*y12 + x12*x12;



            var p4 = -6*x23/l23_2;
            var p5 = -6*x31/l31_2;
            var p6 = -6*x12/l12_2;

            var q4 = 3*x23*y23/l23_2;
            var q5 = 3*x31*y31/l31_2;
            var q6 = 3*x12*y12/l12_2;

            var r4 = 3*y23*y23/l23_2;
            var r5 = 3*y31*y31/l31_2;
            var r6 = 3*y12*y12/l12_2;

            var t4 = -6*y23/l23_2;
            var t5 = -6*y31/l31_2;
            var t6 = -6*y12/l12_2;

            #endregion

            #region 

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

                    return Matrix.FromRowColCoreArray(6, 1, buf);
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

                    return Matrix.FromRowColCoreArray(6, 1, buf);
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

                    return Matrix.FromRowColCoreArray(6, 1, buf);
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

                    return Matrix.FromRowColCoreArray(6, 1, buf);
                });


            #endregion


            var B = new Func<double, double, Matrix>(
                (ξ, η) =>
                {
                    var b1 = y31 * hxkesi(ξ, η) + y12 * hxno(ξ, η);
                    var b2 = -x31 * hykesi(ξ, η) - x12 * hyno(ξ, η);
                    var b3 = -x31*hxkesi(ξ, η) - x12*hxno(ξ, η) + y31*hykesi(ξ, η) + y12*hyno(ξ, η);

                    var buf = Matrix.VerticalConcat(b1, b2);
                    buf = Matrix.VerticalConcat(buf, b3);

                    buf.MultiplyByConstant(1/(2*a));

                    return new Matrix(5, 5);
                });


            var d = new Matrix(3, 3);

            var eval_points = new List<double[]>();//eval_points[i][0] : kesi, eval_points[i][1] : no, eval_points[i][2] : weight

            eval_points.Add(new[] { 0.5, 0, 1 / 3.0 });
            eval_points.Add(new[] { 0.5, 0, 1 / 3.0 });
            eval_points.Add(new[] { 0.5, 0, 1 / 3.0 });

            var klocal = new Matrix(9, 9);

            for (var i = 0; i < eval_points.Count; i++)
            {
                for (var j = 0; j < eval_points.Count; j++)
                {
                    var w = eval_points[i][2]*eval_points[j][2];
                    var kesi = eval_points[i][0];
                    var no = eval_points[i][1];

                    klocal += w*B(kesi, no).Transpose()*d*B(kesi, no);
                }
            }

            throw new NotImplementedException();
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