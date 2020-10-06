// -----------------------------------------------------------------------
// <copyright file="FrameElement2Node.cs">
// Copyright (c) 2010, Hossein Rahami (https://www.mathworks.com/matlabcentral/fileexchange/27012-matrix-structural-analysis)
// Copyright (c) 2018, Ehsan Mohammad Ali, C# version
// </copyright>
// -----------------------------------------------------------------------



using BriefFiniteElementNet.Common;
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace BriefFiniteElementNet.Legacy
//namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a Frame Element with 2 nodes (start and end)
    /// </summary>
    [Serializable]
    [Obsolete("use BarElement instead")]
    public class FrameElement2Node : Element1D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameElement2Node"/> class.
        /// </summary>
        public FrameElement2Node() : base(2)
        {
            //elementType = ElementType.FrameElement2Node;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameElement2Node"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public FrameElement2Node(Node start, Node end)
            : base(2)
        {
            this.nodes[0] = start;
            this.nodes[1] = end;

            //elementType = ElementType.FrameElement2Node;
        }

        #region Field & Properties

        private double _webRotation;
        private double _a;
        private double _ay;
        private double _az;
        private double _iy;
        private double _iz;
        private double _j;
        private bool _useOverridedProperties = true;
        private PolygonYz _geometry;
        private bool _considerShearDeformation;
        private bool _hingedAtStart;
        private bool _hingedAtEnd;
        private double _massDensity;

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
        /// shear area of element, in local y direction, only used when shear deformation should be considered
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
        /// shear area of element, in local z direction, only used when shear deformation should be considered
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
        /// The polar moment of inertial.
        /// </value>
        /// <remarks>
        ///     /          /
        /// J= | ρ². dA = | (y²+z²).dA = <see cref="Iy"/> + <see cref="Iz"/> 
        ///    /A         /A
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
            get { return _useOverridedProperties; }
            set { _useOverridedProperties = value; }
        }

        /// <summary>
        /// Gets or sets the _geometry.
        /// </summary>
        /// <value>
        /// The _geometry of section.
        /// </value>
        /// <remarks>
        /// When <see cref="UseOverridedProperties"/> is setted to <c>false</c>, geometric properties of section of <see cref="FrameElement2Node"/> such as Area and Second moments of area etc. will be calculated with regard to this _geometry.
        /// </remarks>
        public PolygonYz Geometry
        {
            get { return _geometry; }
            set { _geometry = value; }
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
            get { return _considerShearDeformation; }
            set { _considerShearDeformation = value; }
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
            get { return _hingedAtStart; }
            set { _hingedAtStart = value; }
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
            get { return _hingedAtEnd; }
            set { _hingedAtEnd = value; }
        }

        /// <summary>
        /// Gets or sets the web rotation of this member in Degree
        /// </summary>
        /// <value>
        /// The web rotation in degree.
        /// </value>
        public double WebRotation
        {
            get { return _webRotation; }
            set { _webRotation = value; }
        }


        /// <summary>
        /// Gets or sets the mass density.
        /// </summary>
        /// <value>
        /// The mass density of member in kg/m^3.
        /// </value>
        [System.Obsolete]
        public double MassDensity
        {
            get { return _massDensity; }
            set { _massDensity = value; }
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

            Common.MathUtil.FillLowerTriangleFromUpperTriangle(k);

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


        /// <inheritdoc/>
        public override double[] IsoCoordsToLocalCoords(params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the stiffness matrix in Local Coordination system of element.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        /// <exception cref="System.InvalidOperationException">When considering shear defoemation none of the parameters Ay, Az and G should be zero</exception>
        public virtual Matrix GetLocalStiffnessMatrix()
        {

            //if (this.e.Equals(0.0))
            //Is it really need?    throw new BriefFiniteElementNetException("E should be set on a frame element");

            var props = GetGeometricProperties();

            double iz = props[0],
                iy = props[1],
                j = props[2],
                a = props[3],
                ay = props[4],
                az = props[5];

            var l = (this.EndNode.Location - this.StartNode.Location).Length;
            var l2 = l*l;


            var baseArr = new double[144];
            var buf = Matrix.FromRowColCoreArray(12, 12, baseArr);

            if (!this._considerShearDeformation)
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
                    throw new InvalidOperationException(
                        "When considering shear defoemation none of the parameters Ay, Az and G should be zero");

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
                buf[3, 3] = g*j/(e*l);
                buf[3, 9] = -g*j/(e*l);
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
                buf[9, 9] = g*j/(e*l);
                buf[10, 10] = (iy/(l*(l2/12 + ey)))*((l2/3 + ey));
                buf[11, 11] = (iz/(l*(l2/12 + ez)))*((l2/3 + ez));

                #endregion

                var alfa = E;
                for (var i = 0; i < 144; i++)
                    baseArr[i] *= alfa;
            }

            Common.MathUtil.FillLowerTriangleFromUpperTriangle(buf);

            if (_hingedAtStart || _hingedAtEnd)
                buf = GetReleaseMatrix() * buf; 

            return buf;
        }


        /// <summary>
        /// Gets the internal force at <see cref="x" /> position.
        /// </summary>
        /// <param name="x">The position (from start point).</param>
        /// <param name="cmb">The <see cref="LoadCombination" />.</param>
        /// <returns></returns>
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
                lStartDisp.DX, lStartDisp.DY, lStartDisp.DZ,
                lStartDisp.RX, lStartDisp.RY, lStartDisp.RZ,
                lEndDisp.DX, lEndDisp.DY, lEndDisp.DZ,
                lEndDisp.RX, lEndDisp.RY, lEndDisp.RZ
            };

            var lStartForces = Matrix.Multiply(GetLocalStiffnessMatrix(), displVector);

            var startForce = Force.FromVector(lStartForces, 0);

            var forceAtX = -startForce.Move(new Vector(x, 0, 0));

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

        #region transformations


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
                    pars[0] * v.X + pars[3] * v.Y + pars[6] * v.Z,
                    pars[1] * v.X + pars[4] * v.Y + pars[7] * v.Z,
                    pars[2] * v.X + pars[5] * v.Y + pars[8] * v.Z);

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
                    pars[0] * v.X + pars[1] * v.Y + pars[2] * v.Z,
                    pars[3] * v.X + pars[4] * v.Y + pars[5] * v.Z,
                    pars[6] * v.X + pars[7] * v.Y + pars[8] * v.Z);
                bf[i] = tv;
            }

            return bf;
        }

        /// <summary>
        /// The last transformation parameters
        /// </summary>
        /// <remarks>Storing transformation parameters corresponding to <see cref="LastElementVector"/> for better performance.</remarks>
        [NonSerialized]
        private double[] LastTransformationParameters = new double[9];

        /// <summary>
        /// The last start point which LastTransformationParameters is related to
        /// </summary>
        [NonSerialized]
        protected Point? LastStartPoint;

        /// <summary>
        /// The last end point which LastTransformationParameters is related to
        /// </summary>
        [NonSerialized]
        protected Point? LastEndPoint;



        /// <summary>
        /// The last element vector
        /// </summary>
        /// <remarks>Last vector corresponding to current <see cref="LastTransformationParameters"/> </remarks>
        //[NonSerialized]
        //private Vector LastElementVector;

        protected virtual double[] GetTransformationParameters()
        {
            var recalc = true;

            if(LastStartPoint.HasValue && LastEndPoint.HasValue)
                if (LastStartPoint.Value.Equals(StartNode.Location) && LastEndPoint.Value.Equals(EndNode.Location))
                    recalc = false;

            if (recalc)
                ReCalculateTransformationStuff();

            return LastTransformationParameters;
        }

        /// <summary>
        /// Calculates the transformation parameters for this <see cref="FrameElement2Node"/>
        /// </summary>
        protected virtual void ReCalculateTransformationStuff()
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


            var teta = _webRotation;

            var s = Math.Sin(teta * Math.PI / 180.0);
            var c = Math.Cos(teta * Math.PI / 180.0);

            var v = this.EndNode.Location - this.StartNode.Location;


            if (Common.MathUtil.Equals(0, v.X) && Common.MathUtil.Equals(0, v.Y))
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

            LastEndPoint = EndNode.Location;
            LastStartPoint = StartNode.Location;

            var pars = this.LastTransformationParameters;

            pars[0] = cxx;
            pars[1] = cxy * c + cxz * s;
            pars[2] = -cxy * s + cxz * c;

            pars[3] = cyx;
            pars[4] = cyy * c + cyz * s;
            pars[5] = -cyy * s + cyz * c;

            pars[6] = czx;
            pars[7] = czy * c + czz * s;
            pars[8] = -czy * s + czz * c;
            
        }

        /// <summary>
        /// Gets the 12x12 transformation matrix.
        /// </summary>
        /// <returns></returns>
        private Matrix GetTransformationMatrix()
        {

            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Gets the geometric properties that should be used in calculations.
        /// </summary>
        /// <returns>Geometric properties</returns>
        /// <remarks>
        /// if UseOverridedProperties set to false then geometric properties from Geometry polygon should be used.
        /// This method will return whatever properties that should be used regarding to UseOverridedProperties;
        /// </remarks>
        protected double[] GetGeometricProperties()
        {
            if (this._useOverridedProperties)
                return new[] { Iz, Iy, J, A, Ay, Az };
            else
                return Geometry.GetSectionGeometricalProperties();
        }

        #region Serialiation
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_a", _a);
            info.AddValue("_ay", _ay);
            info.AddValue("_az", _az);
            info.AddValue("_iy", _iy);
            info.AddValue("_iz", _iz);
            info.AddValue("_j", _j);
            info.AddValue("_geometry", _geometry);
            info.AddValue("_useOverridedProperties", _useOverridedProperties);
            info.AddValue("_considerShearDeformation", _considerShearDeformation);
            info.AddValue("_hingedAtStart", _hingedAtStart);
            info.AddValue("_hingedAtEnd", _hingedAtEnd);
            info.AddValue("_webRotation", _webRotation);
            info.AddValue("_massDensity", _massDensity);

            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameElement2Node"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected FrameElement2Node(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _a = info.GetDouble("_a");
            _ay = info.GetDouble("_ay");
            _az = info.GetDouble("_az");
            _iy = info.GetDouble("_iy");
            _iz = info.GetDouble("_iz");
            _j = info.GetDouble("_j");
            _geometry = (PolygonYz)info.GetValue("_geometry",typeof(PolygonYz));
            _useOverridedProperties = info.GetBoolean("_useOverridedProperties");
            _considerShearDeformation = info.GetBoolean("_considerShearDeformation");
            _hingedAtStart = info.GetBoolean("_hingedAtStart");
            _hingedAtEnd = info.GetBoolean("_hingedAtEnd");
            _webRotation = info.GetDouble("_webRotation");
            _massDensity = info.GetDouble("_massDensity");
        }

        #endregion


        /// <summary>
        /// Calculates the M matrix.
        /// </summary>
        /// <remarks>M matrix is related to member partial release conditions.</remarks>
        internal Matrix GetReleaseMatrix()
        {
            var buf = Matrix.Eye(12);

            var l = (this.EndNode.Location - this.StartNode.Location).Length;


            var y = new Matrix(3, 3);
            y[1, 2] = -(y[2, 1] = -1.5/l);
            var z = new Matrix(3, 3);
            var x = new Matrix(3, 3);
            x[1, 1] = x[2, 2] = -0.5;
            var w = new Matrix(3, 3);
            w[0, 0] = 1;


            var c = 2*(_hingedAtStart ? 0 : 1) + (_hingedAtEnd ? 0 : 1); //2*H(3)+H(4)=1

            switch (c)
            {
                case 0:
                    //2*H(3)+H(4)=0
                    //y = 2/3*y
                    // M =
                    //  I -y  z -y  
                    //  z  w  z  z
                    //  z  y  I  y
                    //  z  z  z  w
                    y.MultiplyByConstant(2.0 / 3.0);
                    buf.AssembleInside(-y, 0, 3);
                    buf.AssembleInside(w, 3, 3);
                    buf.AssembleInside(y, 6, 3);
                    //buf.AssembleInside(z, 9, 3); no need!
                    buf.AssembleInside(-y, 0, 9);
                    //buf.AssembleInside(z, 3, 9); no need!
                    buf.AssembleInside(y, 6, 9);
                    buf.AssembleInside(w, 9, 9);

                    break;
                case 1:
                    //2*H(3)+H(4)=1
                    // M =
                    //  I -y  z  z  
                    //  z  w  z  z
                    //  z  y  I  z
                    //  z  x  z  I
                    buf.AssembleInside(-y, 0, 3);
                    buf.AssembleInside(w, 3, 3);
                    buf.AssembleInside(y, 6, 3);
                    buf.AssembleInside(x, 9, 3);

                    break;
                case 2:
                    //2*H(3)+H(4)=2
                    // M =
                    //  I  z  z -y  
                    //  z  I  z  x
                    //  z  z  I  y
                    //  z  z  z  w
                    buf.AssembleInside(-y, 0, 9);
                    buf.AssembleInside(x, 3, 9);
                    buf.AssembleInside(y, 6, 9);
                    buf.AssembleInside(w, 9, 9);

                    break;
                case 3:
                    //2*H(3)+H(4)=3
                    //both ends are fixed
                    // M = eye(12)

                    break;
                default:
                    throw new Exception();
            }

            return buf;
        }

        ///<inheritdoc/>
        public override Matrix GetGlobalMassMatrix()
        {
            var m = GetLocalMassMatrix();
            var mArr = m.CoreArray;

            var r = GetTransformationParameters();

            for (int i = 0; i < 4; i++)
            {
                for (int j = i; j < 4; j++)
                {
                    MultSubMatrix(mArr, r, i, j);
                }
            }

            Common.MathUtil.FillLowerTriangleFromUpperTriangle(m);


            return m;
        }

        /// <summary>
        /// Gets the mass matrix in Local Coordination system of element.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Matrix GetLocalMassMatrix()
        {
            var m = new Matrix(12, 12);

            var props = GetGeometricProperties();

            double iz = props[0],
                iy = props[1],
                j = props[2],
                a = props[3],
                ay = props[4],
                az = props[5];


            var l = (this.EndNode.Location - this.StartNode.Location).Length;
            var ro = this.MassDensity;
            var i0 = iy + iz;


            if (MassFormulationType == MassFormulation.Consistent)
            {
                #region filling m

                m[0, 0] = 140;
                m[1, 1] = 156;
                m[2, 2] = 156;
                m[3, 3] = 140*i0/a;
                m[4, 4] = 4*l*l;
                m[5, 5] = 4*l*l;

                m[6, 6] = 140;
                m[7, 7] = 156;
                m[8, 8] = 156;
                m[9, 9] = 140*i0/a;
                m[10, 10] = 4*l*l;
                m[11, 11] = 4*l*l;


                m[0, 6] = 70;

                m[1, 5] = 22*l;
                m[1, 7] = 54;
                m[1, 11] = -13*l;

                m[2, 4] = -22*l;
                m[2, 8] = 54;
                m[2, 10] = 13*l;

                m[3, 9] = 70*i0/a;

                m[4, 8] = -13*l;
                m[4, 10] = -3*l*l;

                m[5, 7] = 13*l;
                m[5, 11] = -3*l*l;

                m[7, 11] = -22*l;

                m[8, 10] = 22*l;

                #endregion

                var c = ro * a * l / 420.0;

                for (var i = 0; i < m.CoreArray.Length; i++) //m=c*m
                    m.CoreArray[i] *= c;
            }
            else
            {
                #region filling m

                for (int i = 0; i < 12; i++)
                {
                    m[i, i] = 1;
                }

                m[3, 3] = m[9, 9] = 0*i0/a;
                m[4, 4] = m[5, 5] = m[10, 10] = m[11, 11] = 0;

                #endregion

                var c = ro * a * l / 2.0;

                for (var i = 0; i < m.CoreArray.Length; i++) //m=c*m
                    m.CoreArray[i] *= c;
            }


           

            if (_hingedAtStart || _hingedAtEnd)
                if (MassFormulationType != MassFormulation.Lumped)
                m = GetReleaseMatrix()*m;

            return m;
        }

        ///<inheritdoc/>
        public override Matrix GetGlobalDampingMatrix()
        {
            throw new NotImplementedException();
        }



        private static void MultSubMatrix(double[] k, double[] r, int i, int j)
        {
            var st = j * 36 + 3 * i;

            var tmp = new double[9];
            var tmp2 = new double[9];

            tmp[0] = r[00] * k[st + 00] + r[01] * k[st + 01] + r[02] * k[st + 02];
            tmp[3] = r[00] * k[st + 12] + r[01] * k[st + 13] + r[02] * k[st + 14];
            tmp[6] = r[00] * k[st + 24] + r[01] * k[st + 25] + r[02] * k[st + 26];
            tmp[1] = r[03] * k[st + 00] + r[04] * k[st + 01] + r[05] * k[st + 02];
            tmp[4] = r[03] * k[st + 12] + r[04] * k[st + 13] + r[05] * k[st + 14];
            tmp[7] = r[03] * k[st + 24] + r[04] * k[st + 25] + r[05] * k[st + 26];
            tmp[2] = r[06] * k[st + 00] + r[07] * k[st + 01] + r[08] * k[st + 02];
            tmp[5] = r[06] * k[st + 12] + r[07] * k[st + 13] + r[08] * k[st + 14];
            tmp[8] = r[06] * k[st + 24] + r[07] * k[st + 25] + r[08] * k[st + 26];

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





        ///<inheritdoc/>
        public override Force[] GetGlobalEquivalentNodalLoads(ElementalLoad load)
        {
            if (load is UniformLoad1D)
                return (load as UniformLoad1D).GetGlobalEquivalentNodalLoads(this);

            throw new NotImplementedException();
        }

        #region obsoletes
        ///<inheritdoc/>
        public Matrix ComputeBMatrix(params double[] location)
        {
            var L = (EndNode.Location - StartNode.Location).Length;

            var L2 = L*L;

            //location is xi varies from -1 to 1
            var xi = location[0];

            if (xi < -1 || xi > 1)
                throw new ArgumentOutOfRangeException("location");


            var buf = new Matrix(4, 12);

            buf.FillMatrixRowise(
                0, 0, (6 * xi) / L2, 0,
                (3 * xi) / L - 1 / L, 0, 0, 0,
                -(6 * xi) / L2, 0, (3 * xi) / L + 1 / L, 0,


                0, (6 * xi) / L2, 0, 0,
                0, (3 * xi) / L - 1 / L, 0, -(6 * xi) / L2,
                0, 0, 0, (3 * xi) / L + 1 / L,


                1 / L, 0, 0, 0,
                0, 0, -1 / L, 0,
                0, 0, 0, 0,


                0, 0, 0, 1 / L,
                0, 0, 0, 0,
                0, -1 / L, 0, 0);

            return buf;
        }

        ///<inheritdoc/>
        public Matrix ComputeDMatrixAt(params double[] location)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public Matrix ComputeNMatrixAt(params double[] location)
        {
            var L = (EndNode.Location - StartNode.Location).Length;

            var xi = location[0];

            if (xi < -1 || xi > 1)
                throw new ArgumentOutOfRangeException("location");
            var buf = new Matrix(1, 12);

            buf.FillMatrixRowise(
                xi / 2 + 1.0 / 2,
                ((xi - 1) * (xi - 1) * (xi + 2)) / 4,
                ((xi - 1) * (xi - 1) * (xi + 2)) / 4,
                xi / 2 + 1.0 / 2,
                (L * (xi - 1) * (xi - 1) * (xi + 1)) / 8,
                (L * (xi - 1) * (xi - 1) * (xi + 1)) / 8,
                1.0 / 2 - xi / 2,
                -((xi + 1) * (xi + 1) * (xi - 2)) / 4,
                -((xi + 1) * (xi + 1) * (xi - 2)) / 4,
                1.0 / 2 - xi / 2,
                (L * (xi - 1) * (xi + 1) * (xi + 1)) / 8,
                (L * (xi - 1) * (xi + 1) * (xi + 1)) / 8);

            return buf;
        }

        ///<inheritdoc/>
        public Matrix ComputeJMatrixAt(params double[] location)
        {
            var v = (EndNode.Location - StartNode.Location).Length;

            return new Matrix(new double[] { v / 2 });
        }
        #endregion

    }
}
