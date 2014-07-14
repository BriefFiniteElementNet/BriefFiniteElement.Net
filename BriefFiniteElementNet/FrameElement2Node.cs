using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    public sealed class FrameElement2Node : Element1D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameElement2Node"/> class.
        /// </summary>
        public FrameElement2Node() : base(2)
        {
        }

        public FrameElement2Node(Node start,Node end)
            : base(2)
        {
            this.nodes[0] = start;
            this.nodes[1] = end;
        }

        #region Members

        private double _a;
        private double _ay;
        private double _az;
        private double _iy;
        private double _iz;
        private double _j;
        private bool useOverridedProperties = true;
        private PolygonYz geometry;
        private bool considerShearDeformation;
        private bool hingedAtStart;
        private bool hingedAtEnd;

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
        /// To be documented
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
        /// To be documented
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
        /// The j.
        /// </value>
        /// <remarks>
        ///     /
        /// J= | Y.Z . dA
        ///    /A
        /// </remarks>
        public double J
        {
            get { return _j; }
            set { _j = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use overrided properties].
        /// </summary>
        /// <value>
        /// <c>true</c> if overrided properties (<see cref="A"/>,<see cref="Ay"/>,<see cref="Az"/>,<see cref="Iy"/>,<see cref="Iz"/>,<see cref="J"/>) should be taken into account for calculations such as stiffness matrix calculation ];
        /// <c>false</c> if geomtriv properties of section should be calculated from <see cref="Geometry"/> Property of <see cref="FrameElement2Node"/>.
        /// </value>
        public bool UseOverridedProperties
        {
            get { return useOverridedProperties; }
            set { useOverridedProperties = value; }
        }

        /// <summary>
        /// Gets or sets the geometry.
        /// </summary>
        /// <value>
        /// The geometry of section.
        /// </value>
        /// <remarks>
        /// When <see cref="UseOverridedProperties"/> is setted to <c>false</c>, geometric properties of section of <see cref="FrameElement2Node"/> such as Area and Second moments of area etc. will be calculated with regard to this geometry.
        /// </remarks>
        public PolygonYz Geometry
        {
            get { return geometry; }
            set { geometry = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [consider shear deformation].
        /// </summary>
        /// <value>
        /// <c>true</c> if [consider shear deformation]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// There are some cases that shear deformation should be considered in analysis. In those cases <see cref="Ay"/> and <see cref="Az"/> are used for calculating stiffness matrix.
        /// also in those cases, if <see cref="UseOverridedProperties"/> == true then Ay and Az will be calculated automatically regarding <see cref="Geometry"/> property.
        /// </remarks>
        public bool ConsiderShearDeformation
        {
            get { return considerShearDeformation; }
            set { considerShearDeformation = value; }
        }

        /// <summary>
        /// Gets or sets the start node.
        /// </summary>
        /// <value>
        /// The start node of <see cref="FrameElement2Node"/>.
        /// </value>
        public Node StartNode
        {
            get { return nodes[0]; }
            set { nodes[0] = value; }
        }

        /// <summary>
        /// Gets or sets the end node.
        /// </summary>
        /// <value>
        /// The end node of <see cref="FrameElement2Node"/>.
        /// </value>
        public Node EndNode
        {
            get { return nodes[1]; }
            set { nodes[1] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether member is hinged at start.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [hinged at start]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// If member is connected with a hing at its start (like simply supported beam) then <see cref="HingedAtStart"/> is set to true, otherwise false
        /// </remarks>
        public bool HingedAtStart
        {
            get { return hingedAtStart; }
            set { hingedAtStart = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [hinged at end].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [hinged at end]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// If member is connected with a hing at its end (like simply supported beam) then <see cref="HingedAtStart"/> is set to true, otherwise false
        /// </remarks>
        public bool HingedAtEnd
        {
            get { return hingedAtEnd; }
            set { hingedAtEnd = value; }
        }

        #endregion

        /// <summary>
        /// Gets the stifness matrix of member in global coordination system.
        /// </summary>
        /// <returns>
        /// The stiffnes matrix
        /// </returns>
        /// <exception cref="System.NotImplementedException">
        /// </exception>
        /// <remarks>
        /// The number of DoFs is in element local regrading order in <see cref="Nodes" />!
        /// </remarks>
        public override Matrix GetGlobalStifnessMatrix()
        {
            var k = GetLocalStiffnessMatrix();
            var kArr = k.CoreArray;

            var r = GetTransformationParameters();

            for (int i = 0; i < 4; i++)
            {
                for (int j = i; j < 4; j++)
                {
                    MultSubMatrix(kArr, r, i, j);
                }
            }

            MathUtil.FillLowerTriangleFromUpperTriangle(k);

            /** /
            #region Tests
            
            var rMatrix = new Matrix(3, 3);
            var tMatrix = new Matrix(12, 12);

            Array.Copy(r, rMatrix.CoreArray, 9);

            for (var ct = 0; ct < 4; ct++)
                for (var i = 0; i < 3; i++)
                    for (var j = 0; j < 3; j++)
                        tMatrix[i + ct*3, j + ct*3] = rMatrix[i, j];

           // var k2 = GetLocalStiffnessMatrix();

            //var kg = tMatrix.Transpose() * k2 * tMatrix;

            //var dif = kg - k;

            //var max = dif.Select(i => Math.Abs(i)).Max();
            #endregion
            /**/
            return k;

        }

        private static void MultSubMatrix(double[] k, double[] r, int i, int j)
        {
            var st = j*36 + 3*i;

            var tmp = new double[9];
            var tmp2 = new double[9];

            tmp[0] = r[00]*k[st + 00] + r[01]*k[st + 01] + r[02]*k[st + 02];
            tmp[3] = r[00]*k[st + 12] + r[01]*k[st + 13] + r[02]*k[st + 14];
            tmp[6] = r[00]*k[st + 24] + r[01]*k[st + 25] + r[02]*k[st + 26];
            tmp[1] = r[03]*k[st + 00] + r[04]*k[st + 01] + r[05]*k[st + 02];
            tmp[4] = r[03]*k[st + 12] + r[04]*k[st + 13] + r[05]*k[st + 14];
            tmp[7] = r[03]*k[st + 24] + r[04]*k[st + 25] + r[05]*k[st + 26];
            tmp[2] = r[06]*k[st + 00] + r[07]*k[st + 01] + r[08]*k[st + 02];
            tmp[5] = r[06]*k[st + 12] + r[07]*k[st + 13] + r[08]*k[st + 14];
            tmp[8] = r[06]*k[st + 24] + r[07]*k[st + 25] + r[08]*k[st + 26];

            tmp2[0] = tmp[00] * r[00] + tmp[03] * r[01] + tmp[06] * r[02];
            tmp2[3] = tmp[00] * r[03] + tmp[03] * r[04] + tmp[06] * r[05];
            tmp2[6] = tmp[00] * r[06] + tmp[03] * r[07] + tmp[06] * r[08];
            tmp2[1] = tmp[01] * r[00] + tmp[04] * r[01] + tmp[07] * r[02];
            tmp2[4] = tmp[01] * r[03] + tmp[04] * r[04] + tmp[07] * r[05];
            tmp2[7] = tmp[01] * r[06] + tmp[04] * r[07] + tmp[07] * r[08];
            tmp2[2] = tmp[02] * r[00] + tmp[05] * r[01] + tmp[08] * r[02];
            tmp2[5] = tmp[02] * r[03] + tmp[05] * r[04] + tmp[08] * r[05];
            tmp2[8] = tmp[02] * r[06] + tmp[05] * r[07] + tmp[08] * r[08];


            //var tmp3 = Matrix.FromRowColCoreArray(3, 3, tmp2);
            //var dif = (buf - tmp3).Select(l=>Math.Abs(l)).Max();

            k[st + 00] = tmp2[0];
            k[st + 12] = tmp2[3];
            k[st + 24] = tmp2[6];
            k[st + 01] = tmp2[1];
            k[st + 13] = tmp2[4];
            k[st + 25] = tmp2[7];
            k[st + 02] = tmp2[2];
            k[st + 14] = tmp2[5];
            k[st + 26] = tmp2[8];
        }

        public Matrix GetLocalStiffnessMatrix()
        {
            double a = _a, iz = _iz, iy = _iy, j = _j, ay = _ay, az = _az;

            if (!useOverridedProperties)
                throw new Exception();


            var l = (this.EndNode.Location - this.StartNode.Location).Length;
            var l2 = l*l;


            var baseArr = new double[144];
            var buf = Matrix.FromRowColCoreArray(12, 12, baseArr);

            if (!this.considerShearDeformation)
            {
                #region With no shear

                buf[0, 0] = a*l2;
                buf[0, 6] = -a*l2;
                buf[1, 1] = iz*12;
                buf[1, 5] = iz*6*l;
                buf[1, 7] = -iz*12;
                buf[1, 11] = iz*6*l;
                buf[2, 2] = iy*12;
                buf[2, 4] = -iy*6*l;
                buf[2, 8] = -iy*12;
                buf[2, 10] = -iy*6*l;
                buf[3, 3] = g*j*l2/e;
                buf[3, 9] = -g*j*l2/e;
                buf[4, 4] = iy*4*l2;
                buf[4, 8] = iy*6*l;
                buf[4, 10] = iy*2*l2;
                buf[5, 5] = iz*4*l2;
                buf[5, 7] = -iz*6*l;
                buf[5, 11] = iz*2*l2;
                buf[6, 6] = a*l2;
                buf[7, 7] = iz*12;
                buf[7, 11] = -iz*6*l;
                buf[8, 8] = iy*12;
                buf[8, 10] = iy*6*l;
                buf[9, 9] = g*j*l2/e;
                buf[10, 10] = iy*4*l2;
                buf[11, 11] = iz*4*l2;



                #endregion

                var alfa = E/(l*l*l);
                for (var i = 0; i < 144; i++)
                    baseArr[i] *= alfa;
            }
            else
            {
                #region With Shear


                if (ay == 0 || az == 0 || g == 0)
                    throw new InvalidOperationException("When considering shar ddefoemation none of the parameters Ay, Az and G should be zero");

                var ez = e*iz/(ay*g);
                var ey = e*iy/(az*g);

                buf[0, 0] = a/l;
                buf[0, 6] = -(a/l);
                buf[1, 1] = (iz/(l*(l2/12 + ez)))*(1);
                buf[1, 5] = (iz/(l*(l2/12 + ez)))*(l/2);
                buf[1, 7] = -((iz/(l*(l2/12 + ez)))*(1));
                buf[1, 11] = (iz/(l*(l2/12 + ez)))*(l/2);
                buf[2, 2] = (iy/(l*(l2/12 + ey)))*(1);
                buf[2, 4] = -(iy/(l*(l2/12 + ey)))*(l/2);
                buf[2, 8] = -((iy/(l*(l2/12 + ey)))*(1));
                buf[2, 10] = -(iy/(l*(l2/12 + ey)))*(l/2);
                buf[3, 3] = g*j/e*l;
                buf[3, 9] = -g*j/e*l;
                buf[4, 4] = (iy/(l*(l2/12 + ey)))*((l2/3 + ey));
                buf[4, 8] = -(-(iy/(l*(l2/12 + ey)))*(l/2));
                buf[4, 10] = (iy/(l*(l2/12 + ey)))*((l2/6 - ey));
                buf[5, 5] = (iz/(l*(l2/12 + ez)))*((l2/3 + ez));
                buf[5, 7] = -((iz/(l*(l2/12 + ez)))*(l/2));
                buf[5, 11] = (iz/(l*(l2/12 + ez)))*((l2/6 - ez));
                buf[6, 6] = a/l;
                buf[7, 7] = (iz/(l*(l2/12 + ez)))*(1);
                buf[7, 11] = -((iz/(l*(l2/12 + ez)))*(l/2));
                buf[8, 8] = (iy/(l*(l2/12 + ey)))*(1);
                buf[8, 10] = -(-(iy/(l*(l2/12 + ey)))*(l/2));
                buf[9, 9] = g*j/e*l;
                buf[10, 10] = (iy/(l*(l2/12 + ey)))*((l2/3 + ey));
                buf[11, 11] = (iz/(l*(l2/12 + ez)))*((l2/3 + ez));



                #endregion

                var alfa = E;
                for (var i = 0; i < 144; i++)
                    baseArr[i] *= alfa;
            }

            MathUtil.FillLowerTriangleFromUpperTriangle(buf);



            if (this.hingedAtStart || this.hingedAtEnd)
            {
                var k = new Matrix(12, 12);

                //Array.Copy(buf.CoreArray, k.CoreArray, 144);

                if (this.hingedAtStart && !this.hingedAtEnd)
                {
                    #region begin released and end fixed

                    k[00, 00] = buf[00, 00];
                    k[00, 01] = buf[00, 01];
                    k[00, 02] = buf[00, 02];
                    k[00, 03] = buf[00, 03];
                    k[00, 04] = buf[00, 04];
                    k[00, 05] = buf[00, 05];
                    k[00, 06] = buf[00, 06];
                    k[00, 07] = buf[00, 07];
                    k[00, 08] = buf[00, 08];
                    k[00, 09] = buf[00, 09];
                    k[00, 10] = buf[00, 10];
                    k[00, 11] = buf[00, 11];
                    k[01, 01] = buf[01, 01] + -(1.5 / l) * buf[05, 01];
                    k[01, 02] = buf[01, 02] + -(1.5 / l) * buf[05, 02];
                    k[01, 03] = buf[01, 03] + -(1.5 / l) * buf[05, 03];
                    k[01, 04] = buf[01, 04] + -(1.5 / l) * buf[05, 04];
                    k[01, 05] = buf[01, 05] + -(1.5 / l) * buf[05, 05];
                    k[01, 06] = buf[01, 06] + -(1.5 / l) * buf[05, 06];
                    k[01, 07] = buf[01, 07] + -(1.5 / l) * buf[05, 07];
                    k[01, 08] = buf[01, 08] + -(1.5 / l) * buf[05, 08];
                    k[01, 09] = buf[01, 09] + -(1.5 / l) * buf[05, 09];
                    k[01, 10] = buf[01, 10] + -(1.5 / l) * buf[05, 10];
                    k[01, 11] = buf[01, 11] + -(1.5 / l) * buf[05, 11];
                    k[02, 02] = buf[02, 02] + -(-1.5 / l) * buf[04, 02];
                    k[02, 03] = buf[02, 03] + -(-1.5 / l) * buf[04, 03];
                    k[02, 04] = buf[02, 04] + -(-1.5 / l) * buf[04, 04];
                    k[02, 05] = buf[02, 05] + -(-1.5 / l) * buf[04, 05];
                    k[02, 06] = buf[02, 06] + -(-1.5 / l) * buf[04, 06];
                    k[02, 07] = buf[02, 07] + -(-1.5 / l) * buf[04, 07];
                    k[02, 08] = buf[02, 08] + -(-1.5 / l) * buf[04, 08];
                    k[02, 09] = buf[02, 09] + -(-1.5 / l) * buf[04, 09];
                    k[02, 10] = buf[02, 10] + -(-1.5 / l) * buf[04, 10];
                    k[02, 11] = buf[02, 11] + -(-1.5 / l) * buf[04, 11];
                    k[03, 03] = buf[03, 03];
                    k[03, 04] = buf[03, 04];
                    k[03, 05] = buf[03, 05];
                    k[03, 06] = buf[03, 06];
                    k[03, 07] = buf[03, 07];
                    k[03, 08] = buf[03, 08];
                    k[03, 09] = buf[03, 09];
                    k[03, 10] = buf[03, 10];
                    k[03, 11] = buf[03, 11];
                    k[06, 06] = buf[06, 06];
                    k[06, 07] = buf[06, 07];
                    k[06, 08] = buf[06, 08];
                    k[06, 09] = buf[06, 09];
                    k[06, 10] = buf[06, 10];
                    k[06, 11] = buf[06, 11];
                    k[07, 07] = 1.5 / l * buf[05, 07] + buf[07, 07];
                    k[07, 08] = 1.5 / l * buf[05, 08] + buf[07, 08];
                    k[07, 09] = 1.5 / l * buf[05, 09] + buf[07, 09];
                    k[07, 10] = 1.5 / l * buf[05, 10] + buf[07, 10];
                    k[07, 11] = 1.5 / l * buf[05, 11] + buf[07, 11];
                    k[08, 08] = -1.5 / l * buf[04, 08] + buf[08, 08];
                    k[08, 09] = -1.5 / l * buf[04, 09] + buf[08, 09];
                    k[08, 10] = -1.5 / l * buf[04, 10] + buf[08, 10];
                    k[08, 11] = -1.5 / l * buf[04, 11] + buf[08, 11];
                    k[09, 09] = buf[09, 09];
                    k[09, 10] = buf[09, 10];
                    k[09, 11] = buf[09, 11];
                    k[10, 10] = -0.5 * buf[04, 10] + buf[10, 10];
                    k[10, 11] = -0.5 * buf[04, 11] + buf[10, 11];
                    k[11, 11] = -0.5 * buf[05, 11] + buf[11, 11];


                    #endregion
                }

                if (!this.hingedAtStart && this.hingedAtEnd)
                {
                    #region begin fixed and end releases

                    k[00, 00] = buf[00, 00];
                    k[00, 01] = buf[00, 01];
                    k[00, 02] = buf[00, 02];
                    k[00, 03] = buf[00, 03];
                    k[00, 04] = buf[00, 04];
                    k[00, 05] = buf[00, 05];
                    k[00, 06] = buf[00, 06];
                    k[00, 07] = buf[00, 07];
                    k[00, 08] = buf[00, 08];
                    k[00, 09] = buf[00, 09];
                    k[00, 10] = buf[00, 10];
                    k[00, 11] = buf[00, 11];
                    k[01, 01] = buf[01, 01] + -(1.5 / l) * buf[11, 01];
                    k[01, 02] = buf[01, 02] + -(1.5 / l) * buf[11, 02];
                    k[01, 03] = buf[01, 03] + -(1.5 / l) * buf[11, 03];
                    k[01, 04] = buf[01, 04] + -(1.5 / l) * buf[11, 04];
                    k[01, 05] = buf[01, 05] + -(1.5 / l) * buf[11, 05];
                    k[01, 06] = buf[01, 06] + -(1.5 / l) * buf[11, 06];
                    k[01, 07] = buf[01, 07] + -(1.5 / l) * buf[11, 07];
                    k[01, 08] = buf[01, 08] + -(1.5 / l) * buf[11, 08];
                    k[01, 09] = buf[01, 09] + -(1.5 / l) * buf[11, 09];
                    k[01, 10] = buf[01, 10] + -(1.5 / l) * buf[11, 10];
                    k[01, 11] = buf[01, 11] + -(1.5 / l) * buf[11, 11];
                    k[02, 02] = buf[02, 02] + -(-1.5 / l) * buf[10, 02];
                    k[02, 03] = buf[02, 03] + -(-1.5 / l) * buf[10, 03];
                    k[02, 04] = buf[02, 04] + -(-1.5 / l) * buf[10, 04];
                    k[02, 05] = buf[02, 05] + -(-1.5 / l) * buf[10, 05];
                    k[02, 06] = buf[02, 06] + -(-1.5 / l) * buf[10, 06];
                    k[02, 07] = buf[02, 07] + -(-1.5 / l) * buf[10, 07];
                    k[02, 08] = buf[02, 08] + -(-1.5 / l) * buf[10, 08];
                    k[02, 09] = buf[02, 09] + -(-1.5 / l) * buf[10, 09];
                    k[02, 10] = buf[02, 10] + -(-1.5 / l) * buf[10, 10];
                    k[02, 11] = buf[02, 11] + -(-1.5 / l) * buf[10, 11];
                    k[03, 03] = buf[03, 03];
                    k[03, 04] = buf[03, 04];
                    k[03, 05] = buf[03, 05];
                    k[03, 06] = buf[03, 06];
                    k[03, 07] = buf[03, 07];
                    k[03, 08] = buf[03, 08];
                    k[03, 09] = buf[03, 09];
                    k[03, 10] = buf[03, 10];
                    k[03, 11] = buf[03, 11];
                    k[04, 04] = buf[04, 04] + -0.5 * buf[10, 04];
                    k[04, 05] = buf[04, 05] + -0.5 * buf[10, 05];
                    k[04, 06] = buf[04, 06] + -0.5 * buf[10, 06];
                    k[04, 07] = buf[04, 07] + -0.5 * buf[10, 07];
                    k[04, 08] = buf[04, 08] + -0.5 * buf[10, 08];
                    k[04, 09] = buf[04, 09] + -0.5 * buf[10, 09];
                    k[04, 10] = buf[04, 10] + -0.5 * buf[10, 10];
                    k[04, 11] = buf[04, 11] + -0.5 * buf[10, 11];
                    k[05, 05] = buf[05, 05] + -0.5 * buf[11, 05];
                    k[05, 06] = buf[05, 06] + -0.5 * buf[11, 06];
                    k[05, 07] = buf[05, 07] + -0.5 * buf[11, 07];
                    k[05, 08] = buf[05, 08] + -0.5 * buf[11, 08];
                    k[05, 09] = buf[05, 09] + -0.5 * buf[11, 09];
                    k[05, 10] = buf[05, 10] + -0.5 * buf[11, 10];
                    k[05, 11] = buf[05, 11] + -0.5 * buf[11, 11];
                    k[06, 06] = buf[06, 06];
                    k[06, 07] = buf[06, 07];
                    k[06, 08] = buf[06, 08];
                    k[06, 09] = buf[06, 09];
                    k[06, 10] = buf[06, 10];
                    k[06, 11] = buf[06, 11];
                    k[07, 07] = buf[07, 07] + 1.5 / l * buf[11, 07];
                    k[07, 08] = buf[07, 08] + 1.5 / l * buf[11, 08];
                    k[07, 09] = buf[07, 09] + 1.5 / l * buf[11, 09];
                    k[07, 10] = buf[07, 10] + 1.5 / l * buf[11, 10];
                    k[07, 11] = buf[07, 11] + 1.5 / l * buf[11, 11];
                    k[08, 08] = buf[08, 08] + -1.5 / l * buf[10, 08];
                    k[08, 09] = buf[08, 09] + -1.5 / l * buf[10, 09];
                    k[08, 10] = buf[08, 10] + -1.5 / l * buf[10, 10];
                    k[08, 11] = buf[08, 11] + -1.5 / l * buf[10, 11];
                    k[09, 09] = buf[09, 09];
                    k[09, 10] = buf[09, 10];
                    k[09, 11] = buf[09, 11];


                    #endregion
                }

                if (this.hingedAtStart && this.hingedAtEnd)
                {
                    if (!considerShearDeformation)
                    {
                        //boosting performance
                        k[0, 0] = k[6, 6] = e*a/l;
                        k[0, 6] = k[6, 0] = -e*a/l;

                        k[3, 3] = k[9, 9] = -(k[3, 9] = -g*j/l);
                    }
                    else
                    {
                        #region begin released and end released

                        k[00, 00] = buf[00, 00];
                        k[00, 01] = buf[00, 01];
                        k[00, 02] = buf[00, 02];
                        k[00, 03] = buf[00, 03];
                        k[00, 04] = buf[00, 04];
                        k[00, 05] = buf[00, 05];
                        k[00, 06] = buf[00, 06];
                        k[00, 07] = buf[00, 07];
                        k[00, 08] = buf[00, 08];
                        k[00, 09] = buf[00, 09];
                        k[00, 10] = buf[00, 10];
                        k[00, 11] = buf[00, 11];
                        k[01, 01] = buf[01, 01] + -(1 / l) * buf[05, 01] + -(1 / l) * buf[11, 01];
                        k[01, 02] = buf[01, 02] + -(1 / l) * buf[05, 02] + -(1 / l) * buf[11, 02];
                        k[01, 03] = buf[01, 03] + -(1 / l) * buf[05, 03] + -(1 / l) * buf[11, 03];
                        k[01, 04] = buf[01, 04] + -(1 / l) * buf[05, 04] + -(1 / l) * buf[11, 04];
                        k[01, 05] = buf[01, 05] + -(1 / l) * buf[05, 05] + -(1 / l) * buf[11, 05];
                        k[01, 06] = buf[01, 06] + -(1 / l) * buf[05, 06] + -(1 / l) * buf[11, 06];
                        k[01, 07] = buf[01, 07] + -(1 / l) * buf[05, 07] + -(1 / l) * buf[11, 07];
                        k[01, 08] = buf[01, 08] + -(1 / l) * buf[05, 08] + -(1 / l) * buf[11, 08];
                        k[01, 09] = buf[01, 09] + -(1 / l) * buf[05, 09] + -(1 / l) * buf[11, 09];
                        k[01, 10] = buf[01, 10] + -(1 / l) * buf[05, 10] + -(1 / l) * buf[11, 10];
                        k[01, 11] = buf[01, 11] + -(1 / l) * buf[05, 11] + -(1 / l) * buf[11, 11];
                        k[02, 02] = buf[02, 02] + -(-1 / l) * buf[04, 02] + -(-1 / l) * buf[10, 02];
                        k[02, 03] = buf[02, 03] + -(-1 / l) * buf[04, 03] + -(-1 / l) * buf[10, 03];
                        k[02, 04] = buf[02, 04] + -(-1 / l) * buf[04, 04] + -(-1 / l) * buf[10, 04];
                        k[02, 05] = buf[02, 05] + -(-1 / l) * buf[04, 05] + -(-1 / l) * buf[10, 05];
                        k[02, 06] = buf[02, 06] + -(-1 / l) * buf[04, 06] + -(-1 / l) * buf[10, 06];
                        k[02, 07] = buf[02, 07] + -(-1 / l) * buf[04, 07] + -(-1 / l) * buf[10, 07];
                        k[02, 08] = buf[02, 08] + -(-1 / l) * buf[04, 08] + -(-1 / l) * buf[10, 08];
                        k[02, 09] = buf[02, 09] + -(-1 / l) * buf[04, 09] + -(-1 / l) * buf[10, 09];
                        k[02, 10] = buf[02, 10] + -(-1 / l) * buf[04, 10] + -(-1 / l) * buf[10, 10];
                        k[02, 11] = buf[02, 11] + -(-1 / l) * buf[04, 11] + -(-1 / l) * buf[10, 11];
                        k[03, 03] = buf[03, 03];
                        k[03, 04] = buf[03, 04];
                        k[03, 05] = buf[03, 05];
                        k[03, 06] = buf[03, 06];
                        k[03, 07] = buf[03, 07];
                        k[03, 08] = buf[03, 08];
                        k[03, 09] = buf[03, 09];
                        k[03, 10] = buf[03, 10];
                        k[03, 11] = buf[03, 11];
                        k[06, 06] = buf[06, 06];
                        k[06, 07] = buf[06, 07];
                        k[06, 08] = buf[06, 08];
                        k[06, 09] = buf[06, 09];
                        k[06, 10] = buf[06, 10];
                        k[06, 11] = buf[06, 11];
                        k[07, 07] = 1 / l * buf[05, 07] + buf[07, 07] + 1 / l * buf[11, 07];
                        k[07, 08] = 1 / l * buf[05, 08] + buf[07, 08] + 1 / l * buf[11, 08];
                        k[07, 09] = 1 / l * buf[05, 09] + buf[07, 09] + 1 / l * buf[11, 09];
                        k[07, 10] = 1 / l * buf[05, 10] + buf[07, 10] + 1 / l * buf[11, 10];
                        k[07, 11] = 1 / l * buf[05, 11] + buf[07, 11] + 1 / l * buf[11, 11];
                        k[08, 08] = -1 / l * buf[04, 08] + buf[08, 08] + -1 / l * buf[10, 08];
                        k[08, 09] = -1 / l * buf[04, 09] + buf[08, 09] + -1 / l * buf[10, 09];
                        k[08, 10] = -1 / l * buf[04, 10] + buf[08, 10] + -1 / l * buf[10, 10];
                        k[08, 11] = -1 / l * buf[04, 11] + buf[08, 11] + -1 / l * buf[10, 11];
                        k[09, 09] = buf[09, 09];
                        k[09, 10] = buf[09, 10];
                        k[09, 11] = buf[09, 11];





                        #endregion
                    }
                }

                MathUtil.FillLowerTriangleFromUpperTriangle(k);//again

                Array.Copy(k.CoreArray, buf.CoreArray, 144);
            }


            return buf;
        }


        /// <summary>
        /// Gets the internal force at <see cref="x" /> position.
        /// </summary>
        /// <param name="x">The position (from start point).</param>
        /// <param name="cmb">The <see cref="LoadCombination" />.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks>
        /// Will calculate the internal forces of member regarding the <see cref="cmb" /> <see cref="LoadCombination" />
        /// </remarks>
        public override Force GetInternalForceAt(double x, LoadCombination cmb)
        {
            var gStartDisp = StartNode.GetNodalDisplacement(cmb);
            var gEndDisp = EndNode.GetNodalDisplacement(cmb);

            var lStartDisp = new Displacement(
                TransformGlobalToLocal(gStartDisp.Displacements),
                TransformGlobalToLocal(gStartDisp.Rotations));

            var lEndDisp = new Displacement(
                TransformGlobalToLocal(gEndDisp.Displacements),
                TransformGlobalToLocal(gEndDisp.Rotations));

            var displVector = new double[]
            {
                lStartDisp.Dx,lStartDisp.Dy,lStartDisp.Dz,
                lStartDisp.Rx,lStartDisp.Ry,lStartDisp.Rz,

                lEndDisp.Dx,lEndDisp.Dy,lEndDisp.Dz,
                lEndDisp.Rx,lEndDisp.Ry,lEndDisp.Rz
            };

            var lStartForces = Matrix.Multiply(GetLocalStiffnessMatrix(), displVector);

            var startForce = Force.FromVector(lStartForces, 0);

            var forceAtX = -startForce.Move(new Vector(x,0,0));

            foreach (var ld in loads)
            {
                if (!cmb.ContainsKey(ld.Case))
                    continue;

                var frc = ((Load1D) ld).GetInternalForceAt(this, x);

                forceAtX += cmb[ld.Case]*frc;
            }

            return forceAtX;
        }

        /// <summary>
        /// Gets the internal force at.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks>
        /// Will calculate the internal forces of member regarding Default load case (default load case means a load case where <see cref="LoadCase.LoadType" /> is equal to <see cref="LoadType.Default" /> and <see cref="LoadCase.CaseName" /> is equal to null)
        /// </remarks>
        public override Force GetInternalForceAt(double x)
        {
            var cmb = new LoadCombination();
            cmb[new LoadCase()] = 1.0;

            return GetInternalForceAt(x, cmb);
        }



        /// <summary>
        /// Converts the vector from local to global coordinations system.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        internal Vector TransformGlobalToLocal(Vector v)
        {
            var pars = GetTransformationParameters();
            var buf = new Vector(
                pars[0] * v.X + pars[3] * v.Y + pars[6] * v.Z,
                pars[1] * v.X + pars[4] * v.Y + pars[7] * v.Z,
                pars[2] * v.X + pars[5] * v.Y + pars[8] * v.Z);

            return buf;
        }

        /// <summary>
        /// Converts the vectors from local to global coordinations system.
        /// </summary>
        /// <param name="vs">The v.</param>
        /// <returns></returns>
        internal Vector[] TransformGlobalToLocal(params Vector[] vs)
        {
            var pars = GetTransformationParameters();

            var bf = new Vector[vs.Length];


            for (var i = 0; i < vs.Length; i++)
            {
                var v = vs[i];
                var buf = new Vector(
                    pars[0]*v.X + pars[3]*v.Y + pars[6]*v.Z,
                    pars[1]*v.X + pars[4]*v.Y + pars[7]*v.Z,
                    pars[2]*v.X + pars[5]*v.Y + pars[8]*v.Z);

                bf[i] = buf;
            }

            return bf;
        }


        /// <summary>
        /// Converts the vector from global to localcoordinations system.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        internal Vector TransformLocalToGlobal(Vector v)
        {
            var pars = GetTransformationParameters();
            var buf = new Vector(
                pars[0] * v.X + pars[1] * v.Y + pars[2] * v.Z,
                pars[3] * v.X + pars[4] * v.Y + pars[5] * v.Z,
                pars[6] * v.X + pars[7] * v.Y + pars[8] * v.Z);

            return buf;
        }

        /// <summary>
        /// Converts the an array of vectors from global to localcoordinations system.
        /// </summary>
        /// <param name="vs">The vectors.</param>
        /// <returns></returns>
        internal Vector[] TransformLocalToGlobal(params Vector[] vs)
        {
            var pars = GetTransformationParameters();

            var bf = new Vector[vs.Length];


            for (var i = 0; i < vs.Length; i++)
            {
                var v = vs[i];

                var tv = new Vector(
                    pars[0]*v.X + pars[1]*v.Y + pars[2]*v.Z,
                    pars[3]*v.X + pars[4]*v.Y + pars[5]*v.Z,
                    pars[6]*v.X + pars[7]*v.Y + pars[8]*v.Z);
                bf[i] = tv;

            }
            
            return bf;
        }

        
        /// <summary>
        /// The last transformation parameters
        /// </summary>
        /// <remarks>Storing transformation parameters corresponding to <see cref="LastElementVector"/> for better performance.</remarks>
        private double[] LastTransformationParameters=new double[9];
        
        /// <summary>
        /// The last element vector
        /// </summary>
        /// <remarks>Last vector corresponding to current <see cref="LastTransformationParameters"/> </remarks>
        private Vector LastElementVector;


        

        private double[] GetTransformationParameters()
        {
            var v = this.EndNode.Location - this.StartNode.Location;

            if (!v.Equals(LastElementVector))
                ReCalculateTransformationParameters();

            return LastTransformationParameters;
        }

        /// <summary>
        /// Calculates the transformation parameters for this <see cref="FrameElement2Node"/>
        /// </summary>
        private void ReCalculateTransformationParameters()
        {
            var cxx = 0.0;
            var cxy = 0.0;
            var cxz = 0.0; 
            
            var cyx = 0.0;
            var cyy = 0.0;
            var cyz = 0.0;

            var czx = 0.0;
            var czy = 0.0;
            var czz = 0.0;
            
            var v = this.EndNode.Location - this.StartNode.Location;


            if (MathUtil.Equals(0, v.X) && MathUtil.Equals(0, v.Y))
            {
                if (v.Z > 0)
                {
                    czx = 1;
                    cyy = 1;
                    cxz = -1;
                }
                else
                {
                    czx = -1;
                    cyy = 1;
                    cxz = 1;
                }
            }
            else
            {
                var l = v.Length;
                cxx = v.X / l;
                cyx = v.Y / l;
                czx = v.Z / l;
                var d = Math.Sqrt(cxx * cxx + cyx * cyx);
                cxy = -cyx / d;
                cyy = cxx / d;
                cxz = -cxx * czx / d;
                cyz = -cyx * czx / d;
                czz = d;
            }


           
            this.LastElementVector = v;

            var pars = this.LastTransformationParameters;

            pars[0] = cxx;
            pars[1] = cxy;
            pars[2] = cxz;

            pars[3] = cyx;
            pars[4] = cyy;
            pars[5] = cyz;

            pars[6] = czx;
            pars[7] = czy;
            pars[8] = czz;
        }
    }
}