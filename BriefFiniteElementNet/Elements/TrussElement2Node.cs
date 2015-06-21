using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a 2 noded truss element (start and end)
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

        private bool useOverridedProperties=true;

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
        public TrussElement2Node(Node start,Node end) : base(2)
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
            throw new NotImplementedException();
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
            var v = this.EndNode.Location - this.StartNode.Location;

            var l = v.Length;
            var cx = v.X/l;
            var cy = v.Y/l;
            var cz = v.Z/l;

            var buf = new Matrix(12, 12);

            buf[0 + 0, 0] = cx * cx; buf[0 + 0, 1] = cx * cy; buf[0 + 0, 2] = cx * cz;
            buf[1 + 0, 0] = cx * cy; buf[1 + 0, 1] = cy * cy; buf[1 + 0, 2] = cy * cz;
            buf[2 + 0, 0] = cx * cz; buf[2 + 0, 1] = cy * cz; buf[2 + 0, 2] = cz * cz;



            buf[0 + 6, 0] = -cx * cx; buf[0 + 6, 1] = -cx * cy; buf[0 + 6, 2] = -cx * cz;
            buf[1 + 6, 0] = -cx * cy; buf[1 + 6, 1] = -cy * cy; buf[1 + 6, 2] = -cy * cz;
            buf[2 + 6, 0] = -cx * cz; buf[2 + 6, 1] = -cy * cz; buf[2 + 6, 2] = -cz * cz;



            buf[0, 0 + 6] = -cx * cx; buf[0, 1 + 6] = -cx * cy; buf[0, 2 + 6] = -cx * cz;
            buf[1, 0 + 6] = -cx * cy; buf[1, 1 + 6] = -cy * cy; buf[1, 2 + 6] = -cy * cz;
            buf[2, 0 + 6] = -cx * cz; buf[2, 1 + 6] = -cy * cz; buf[2, 2 + 6] = -cz * cz;


            buf[0 + 6, 0 + 6] = cx * cx; buf[0 + 6, 1 + 6] = cx * cy; buf[0 + 6, 2 + 6] = cx * cz;
            buf[1 + 6, 0 + 6] = cx * cy; buf[1 + 6, 1 + 6] = cy * cy; buf[1 + 6, 2 + 6] = cy * cz;
            buf[2 + 6, 0 + 6] = cx * cz; buf[2 + 6, 1 + 6] = cy * cz; buf[2 + 6, 2 + 6] = cz * cz;

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
            info.AddValue("_a",_a);
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
        private TrussElement2Node(SerializationInfo info, StreamingContext context):base(info,context)
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
            var pars = GetTransformationParameters();
            var buf = new Vector(
                pars[0] * v.X + pars[1] * v.Y + pars[2] * v.Z,
                pars[3] * v.X + pars[4] * v.Y + pars[5] * v.Z,
                pars[6] * v.X + pars[7] * v.Y + pars[8] * v.Z);

            return buf;
        }


        private double[] GetTransformationParameters()
        {
            var v = this.EndNode.Location - this.StartNode.Location;

            if (!v.Equals(LastElementVector))
                ReCalculateTransformationParameters();

            return LastTransformationParameters;
        }


        /// <summary>
        /// The last transformation parameters
        /// </summary>
        /// <remarks>Storing transformation parameters corresponding to <see cref="LastElementVector"/> for better performance.</remarks>
        [NonSerialized]
        private double[] LastTransformationParameters = new double[9];

        /// <summary>
        /// The last element vector
        /// </summary>
        /// <remarks>Last vector corresponding to current <see cref="LastTransformationParameters"/> </remarks>
        [NonSerialized]
        private Vector LastElementVector;

        private double _massDensity;


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


            var teta = 0.0;

            var s = Math.Sin(teta * Math.PI / 180.0);
            var c = Math.Cos(teta * Math.PI / 180.0);

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
            pars[1] = cxy * c + cxz * s;
            pars[2] = -cxy * s + cxz * c;

            pars[3] = cyx;
            pars[4] = cyy * c + cyz * s;
            pars[5] = -cyy * s + cyz * c;

            pars[6] = czx;
            pars[7] = czy * c + czz * s;
            pars[8] = -czy * s + czz * c;
         
        }


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
