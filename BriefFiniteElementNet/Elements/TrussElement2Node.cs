using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents a 2 node truss element (start and end)
    /// </summary>
    [Serializable]
    public sealed class TrussElement2Node : Element1D
    {
        #region Members

        private double _a;

        /// <summary>
        /// Gets or sets the mass density.
        /// </summary>
        /// <value>
        /// The mass density of member in kg/m^3.
        /// </value>
        public double MassDensity
        {
            get { return _massDensity; }
            private set { _massDensity = value; }
        }


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

        private bool useOverridedProperties = true;

        /// <summary>
        /// Gets or sets a value indicating whether [use overrided properties].
        /// </summary>
        /// <value>
        /// <c>true</c> if overrided properties (<see cref="A"/>,<see cref="A"/>) should be taken into account for calculations such as stiffness matrix calculation ];
        /// <c>false</c> if geometric properties of section should be calculated from <see cref="Geometry"/> Property of <see cref="TrussElement2Node"/>.
        /// </value>
        public bool UseOverridedProperties
        {
            get { return useOverridedProperties; }
            set { useOverridedProperties = value; }
        }

        private PolygonYz geometry;

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

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TrussElement2Node"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public TrussElement2Node(Node start, Node end) : base(2)
        {
            this.nodes[0] = start;
            this.nodes[1] = end;
            this.elementType = ElementType.TrussElement2Noded;
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
            
            throw new NotImplementedException();
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
            cmb[LoadCase.DefaultLoadCase] = 1.0;
            return GetInternalForceAt(x, cmb);
        }

        /// <summary>
        /// Gets the stiffness matrix of member in global coordination system.
        /// </summary>
        /// <returns>
        /// The stiffness matrix
        /// </returns>
        /// <exception cref="System.NotImplementedException">
        /// </exception>
        /// <remarks>
        /// The number of DoFs is in element local regrading order in <see cref="Nodes" />!
        /// </remarks>
        public override Matrix GetGlobalStifnessMatrix()
        {
            throw new NotImplementedException();

            //TODO: these are not correct, to be fixed...
            var v = this.EndNode.Location - this.StartNode.Location;

            var l = v.Length;
            var cx = v.X/l;
            var cy = v.Y/l;
            var cz = v.Z/l;

            var localS = new Matrix(2, 2);

            var a = this.A;//area

            localS[0, 0] = localS[1, 1] = this.e * a / l;
            localS[1, 0] = localS[0, 1] = -this.e * a / l;

            var t = new Matrix(2, 6);
            t[0, 0] = cx;
            t[0, 1] = cy;
            t[0, 2] = cz;

            t[1, 3] = cx;
            t[1, 4] = cy;
            t[1, 5] = cz;


            var buf = new Matrix(12, 12);

            buf[0 + 0, 0] = cx*cx;
            buf[0 + 0, 1] = cx*cy;
            buf[0 + 0, 2] = cx*cz;
            buf[1 + 0, 0] = cx*cy;
            buf[1 + 0, 1] = cy*cy;
            buf[1 + 0, 2] = cy*cz;
            buf[2 + 0, 0] = cx*cz;
            buf[2 + 0, 1] = cy*cz;
            buf[2 + 0, 2] = cz*cz;


            buf[0 + 6, 0] = -cx*cx;
            buf[0 + 6, 1] = -cx*cy;
            buf[0 + 6, 2] = -cx*cz;
            buf[1 + 6, 0] = -cx*cy;
            buf[1 + 6, 1] = -cy*cy;
            buf[1 + 6, 2] = -cy*cz;
            buf[2 + 6, 0] = -cx*cz;
            buf[2 + 6, 1] = -cy*cz;
            buf[2 + 6, 2] = -cz*cz;


            buf[0, 0 + 6] = -cx*cx;
            buf[0, 1 + 6] = -cx*cy;
            buf[0, 2 + 6] = -cx*cz;
            buf[1, 0 + 6] = -cx*cy;
            buf[1, 1 + 6] = -cy*cy;
            buf[1, 2 + 6] = -cy*cz;
            buf[2, 0 + 6] = -cx*cz;
            buf[2, 1 + 6] = -cy*cz;
            buf[2, 2 + 6] = -cz*cz;


            buf[0 + 6, 0 + 6] = cx*cx;
            buf[0 + 6, 1 + 6] = cx*cy;
            buf[0 + 6, 2 + 6] = cx*cz;
            buf[1 + 6, 0 + 6] = cx*cy;
            buf[1 + 6, 1 + 6] = cy*cy;
            buf[1 + 6, 2 + 6] = cy*cz;
            buf[2 + 6, 0 + 6] = cx*cz;
            buf[2 + 6, 1 + 6] = cy*cz;
            buf[2 + 6, 2 + 6] = cz*cz;

            if (!useOverridedProperties)
                throw new NotImplementedException();

            var cf = this.e*this._a/l;

            for (int i = 0; i < 144; i++)
            {
                buf.CoreArray[i] *= cf;
            }

            return buf;
            throw new NotImplementedException();
        }

        /// <summary>
        /// The last transformation matrix associated with <see cref="_lastElementVector"/>
        /// </summary>
        [NonSerialized]
        private Matrix _lastTransformationMatrix;

        /// <summary>
        /// The last transformation matrix associated with <see cref="_lastTransformationMatrix"/>
        /// </summary>
        [NonSerialized]
        private Vector _lastElementVector;


        /// <summary>
        /// Gets or sets the start node.
        /// </summary>
        /// <value>
        /// The start node of <see cref="TrussElement2Node"/>.
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
        /// The end node of <see cref="TrussElement2Node"/>.
        /// </value>
        public Node EndNode
        {
            get { return nodes[1]; }
            set { nodes[1] = value; }
        }


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
            info.AddValue("geometry", geometry);
            info.AddValue("useOverridedProperties", useOverridedProperties);
            info.AddValue("_massDensity", _massDensity);
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrussElement2Node"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private TrussElement2Node(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _a = info.GetDouble("_a");
            geometry = info.GetValue<PolygonYz>("geometry");
            useOverridedProperties = info.GetBoolean("useOverridedProperties");
            _massDensity = info.GetDouble("_massDensity");
        }



        /// <summary>
        /// Converts the vector from local to global coordination system.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        internal Vector TransformLocalToGlobal(Vector v)
        {
            var t = GetTransformationMatrix();
            var vm = new Matrix(new[] {v.X, v.Y, v.Z});

            var buf = t*vm;

            return new Vector(buf[0, 0], buf[1, 0], buf[2, 0]);
        }

        /// <summary>
        /// Converts the vector from global to local coordination system.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        internal Vector TransformGlobalToLocal(Vector v)
        {
            var t = GetTransformationMatrix();
            var vm = new Matrix(new[] { v.X, v.Y, v.Z });

            var buf = t.Transpose() * vm;

            return new Vector(buf[0, 0], buf[1, 0], buf[2, 0]);
        }



        /// <summary>
        /// Gets the transformation matrix.
        /// </summary>
        /// <returns></returns>
        private Matrix GetTransformationMatrix()
        {
            var v = this.EndNode.Location - this.StartNode.Location;

            if (!v.Equals(_lastElementVector))
            {
                _lastTransformationMatrix = CalcUtil.Get2NodeElementTransformationMatrix(v);
                _lastElementVector = v;
            }

            return _lastTransformationMatrix;
        }


        private double _massDensity;


        ///<inheritdoc/>
        public override Matrix GetGlobalMassMatrix()
        {
            throw new NotImplementedException();
        }


        ///<inheritdoc/>
        public override Matrix GetGlobalDampingMatrix()
        {
            throw new NotImplementedException();
        }
    }
}