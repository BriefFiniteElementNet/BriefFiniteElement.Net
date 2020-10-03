using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents a 2 node truss element (start and end)
    /// </summary>
    [Serializable]
    [Obsolete("Use BarElement with Truss behaviour instead")]
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
            //this.elementType = ElementType.TrussElement2Noded;
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
                lStartDisp.DX, lStartDisp.DY, lStartDisp.DZ,
                lStartDisp.RX, lStartDisp.RY, lStartDisp.RZ,
                lEndDisp.DX, lEndDisp.DY, lEndDisp.DZ,
                lEndDisp.RX, lEndDisp.RY, lEndDisp.RZ
            };

            var lStartForces = GetLocalStiffnessMatrix().Multiply(displVector);

            var startForce = Force.FromVector(lStartForces, 0);

            var forceAtX = -startForce.Move(new Vector(x, 0, 0));

            foreach (var ld in loads)
            {
                if (!cmb.ContainsKey(ld.Case))
                    continue;

                var frc = ((Load1D)ld).GetInternalForceAt(this, x);

                forceAtX += cmb[ld.Case] * frc;
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
            return GetInternalForceAt(x, LoadCombination.DefaultLoadCombination);
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
            var k = GetLocalStiffnessMatrix();
            var kArr = k.Values;

            var r = GetTransformationParameters();

            for (int i = 0; i < 4; i++)
            {
                for (int j = i; j < 4; j++)
                {
                    MultSubMatrix(kArr, r, i, j);
                }
            }

            Common.MathUtil.FillLowerTriangleFromUpperTriangle(k);

            return k;
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

        public Matrix GetLocalStiffnessMatrix()
        {
            //codes borrowed from FrameElement2Node
            double area = 0;

            if (this.useOverridedProperties)
                area = A;
            else if (geometry != null)
                area = Geometry.GetSectionGeometricalProperties()[0];

            double a = area;

            var l = (this.EndNode.Location - this.StartNode.Location).Length;

            //var baseArr = new double[144];
            var buf = new Matrix(12,12);//.FromRowColCoreArray(12, 12, baseArr);

            buf[0, 0] = buf[6, 6] = E*a/l;
            buf[6, 0] = buf[0, 6] = -E*a/l;

            return buf;
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
            geometry = (PolygonYz)info.GetValue("geometry",typeof(PolygonYz));
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
            var vm = Matrix.OfVector(new[] { v.X, v.Y, v.Z });

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
            var vm = Matrix.OfVector(new[] { v.X, v.Y, v.Z });

            var buf = t.Transpose() * vm;

            return new Vector(buf[0, 0], buf[1, 0], buf[2, 0]);
        }

        /// <inheritdoc/>
        public override double[] IsoCoordsToLocalCoords(params double[] isoCoords)
        {
            throw new NotImplementedException();
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


        ///<inheritdoc/>
        public override Force[] GetGlobalEquivalentNodalLoads(ElementalLoad load)
        {
            throw new NotImplementedException();
        }


        
    }
}
