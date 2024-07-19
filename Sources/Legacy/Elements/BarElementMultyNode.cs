using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Integration;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using System.Security.Permissions;
using System.Globalization;
using Mathh = BriefFiniteElementNet.Mathh;


using BriefFiniteElementNet;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Common;
using Legacy.ElementHelpers;
using BriefFiniteElementNet.ElementHelpers.BarHelpers;
using BriefFiniteElementNet.ElementHelpers.Bar;

namespace Legacy.Elements
{
    /// <summary>
    /// Represents a Bar element with two nodes (start and end)
    /// </summary>
    [Serializable]

    public class BarElementMultyNode : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarElementMultyNode"/> class.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        public BarElementMultyNode(Node n1, Node n2) : this(2)
        {
            StartNode = n1;
            EndNode = n2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BarElementMultyNode"/> class.
        /// </summary>
        /// <param name="nodeCount">The number of nodes.</param>
        public BarElementMultyNode(int nodeCount) : base(nodeCount)
        {
            _nodalReleaseConditions = Enumerable.Repeat(Constraints.Fixed, nodeCount).ToArray();
        }

        #region Field & Properties

        private double _webRotation;

        //private BarElementEndConnection _startConnection = BarElementEndConnection.Fixed;
        //private BarElementEndConnection _endtConnection = BarElementEndConnection.Fixed;
        private BarElementBehaviour _behavior = BarElementBehaviours.FullFrame;
        private Base1DSection _section;
        private BaseMaterial _material;
        //private Constraint _startReleaseCondition = Constraints.Fixed;
        //private Constraint _endReleaseCondition = Constraints.Fixed;
        internal Constraint[] _nodalReleaseConditions;


        /*
        public Constraint[] NodalReleaseConditions
        {
            get { return _nodalReleaseConditions; }
        }*/

        /// <summary>
        /// Gets or sets the node count of bar element
        /// </summary>
        public int NodeCount
        {
            get
            {
                return nodes.Length;
            }

        }


        /// <summary>
        /// Gets or sets the start node.
        /// </summary>
        /// <value>
        /// The start node of <see cref="BarElementMultyNode"/>.
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
        /// The end node of <see cref="BarElementMultyNode"/>.
        /// </value>
        public Node EndNode
        {
            get { return nodes[nodes.Length - 1]; }
            set { nodes[nodes.Length - 1] = value; }
        }


        /// <summary>
        /// Gets or sets the web rotation of this member in Degree
        /// </summary>
        /// <value>
        /// The web rotation in degree. It does rotate the local coordination system of element. (TODO: in CW or CCW direction?)
        /// </value>
        /// <example>
        /// <code>
        /// var bar = new BarElement();
        /// bar.WebRotation = 10;//sets web rotation to 10 degrees
        /// </code>
        /// </example>
        public double WebRotation
        {
            get { return _webRotation; }
            set { _webRotation = value; }
        }

        /// <summary>
        /// Gets or sets the cross section of bar element.
        /// </summary>
        /// <value>
        /// The cross section.
        /// </value>
        /// <example>
        /// <code>
        /// var bar = new BarElement();
        /// bar.Section = new UniformParametricBarElementCrossSection(1,1,1,1,1);//sets section
        /// </code>
        /// </example>
        public Base1DSection Section
        {
            get { return _section; }
            set { _section = value; }
        }

        /// <summary>
        /// Gets or sets the material of bar element.
        /// </summary>
        /// <value>
        /// The material.
        /// </value>
        /// <example>
        /// <code>
        /// var bar = new BarElement();
        /// bar.Section = new UniformBarMaterial(1,1);//sets material
        /// </code>
        /// </example>
        public BaseMaterial Material
        {
            get { return _material; }
            set { _material = value; }
        }

        /// <summary>
        /// Gets or sets the behavior of bar element.
        /// </summary>
        /// <value>
        /// The behaviors of bar element.
        /// </value>
        /// <example>
        /// <code>
        /// var bar = new BarElement();
        /// bar.Behavior = BarElementBehaviours.FullFrame
        /// </code>
        /// </example>
        public BarElementBehaviour Behavior
        {
            get { return _behavior; }
            set { _behavior = value; }
        }


        /// <summary>
        /// Gets or sets the connection constraints od element to the start node
        /// </summary>
        public Constraint StartReleaseCondition
        {
            get { return _nodalReleaseConditions[0]; }
            set { _nodalReleaseConditions[0] = value; }
        }

        /// <summary>
        /// Gets or sets the connection constraints od element to the end node
        /// </summary>
        public Constraint EndReleaseCondition
        {
            get { return _nodalReleaseConditions[_nodalReleaseConditions.Length - 1]; }
            set { _nodalReleaseConditions[_nodalReleaseConditions.Length - 1] = value; }
        }

        /// <summary>
        /// Gets or sets the connection constraints od element to the all node
        /// </summary>
        public Constraint[] NodalReleaseConditions
        {
            get { return _nodalReleaseConditions; }
            set { _nodalReleaseConditions = value; }
        }
        #endregion


        #region obsolete methods

        public Matrix ComputeBMatrix(params double[] location)
        {
            var L = (EndNode.Location - StartNode.Location).Length;

            var L2 = L * L;
            var L3 = L2 * L;


            //location is xi varies from -1 to 1
            var xi = location[0];

            if (xi < -1 || xi > 1)
                throw new ArgumentOutOfRangeException("location");

            var buf = new Matrix(4, 12);


            if ((_behavior & BarElementBehaviour.BeamYEulerBernoulli) != 0)
            {
                //BeamY is in behaviors, should use beam with Iz

                var arr = new double[] { 0, 6 * xi / L2, 0, 0,
                0, 3 * xi / L - 1 / L, 0, -(6 * xi) / L2,
                0, 0, 0, 3 * xi / L + 1 / L};

                buf.SetRow(0, arr);
            }

            if ((_behavior & BarElementBehaviour.BeamYTimoshenko) != 0)
            {
                throw new NotImplementedException();

                double c;

                {
                    var e = 1.0;
                    var g = 1.0;
                    var i = 1.0;
                    var k = 1.0;

                    var a = e * i / L3;
                    var b = k * g * a * L;//6.20 ref[4]

                    c = 6 * a * L / (12 * a * L2 + b);
                }

                var arr = new double[]
                {
                    0, -12*c*L*xi + 6*xi, 0, 0,
                    0, - 6*c*L*xi + L*(3*xi - 1), 0, 12*c*L*xi - 6*xi,
                    0, 0, 0, 6*c*L*xi + L*(3*xi + 1)
                };

                buf.SetRow(0, arr);
            }

            if ((_behavior & BarElementBehaviour.BeamZEulerBernoulli) != 0)
            {
                //BeamZ in behaviours, should use beam with Iy

                var arr = new double[] {  0, 0, 6 * xi / L2, 0,
                3 * xi / L - 1 / L, 0, 0, 0,
                -(6 * xi) / L2, 0, 3 * xi / L + 1 / L, 0};

                buf.SetRow(1, arr);
            }

            if ((_behavior & BarElementBehaviour.BeamZTimoshenko) != 0)
            {
                throw new NotImplementedException();

                double c;

                {
                    var e = 1.0;
                    var g = 1.0;
                    var i = 1.0;
                    var k = 1.0;

                    var a = e * i / L3;
                    var b = k * g * a * L;//6.20 ref[4]

                    c = 6 * a * L / (12 * a * L2 + b);
                }

                var arr = new double[]
                {
                    0, 0, -12*c*L*xi + 6*xi, 0,
                    -6*c*L*xi + L*(3*xi - 1), 0, 12*c*L*xi - 6*xi, 0, 0, 0,
                    6*c*L*xi + L*(3*xi + 1), 0
                };

                buf.SetRow(1, arr);
            }

            if ((_behavior & BarElementBehaviour.Truss) != 0)
            {
                var arr = new double[] {  1 / L, 0, 0, 0,
                0, 0, -1 / L, 0,
                0, 0, 0, 0,};

                buf.SetRow(2, arr);
            }

            if ((_behavior & BarElementBehaviour.Shaft) != 0)
            {
                var arr = new double[] {   0, 0, 0, 1 / L,
                0, 0, 0, 0,
                0, -1 / L, 0, 0};

                buf.SetRow(3, arr);
            }




            return buf;
        }

        public Matrix ComputeDMatrixAt(params double[] location)
        {
            double e = 0.0;//, g = 0;//mechanical

            double iz = 0, iy = 0, j = 0, a = 0;//geometrical

            var buf = new Matrix(4, 4);

            buf[0, 0] = e * iz;
            buf[1, 1] = e * iy;
            buf[2, 2] = e * a;
            buf[3, 3] = e * j;

            return buf;
        }

        public Matrix ComputeJMatrixAt(params double[] location)
        {
            // J =  ∂x / ∂ξ
            var L = (EndNode.Location - StartNode.Location).Length;

            var buf = new Matrix(1, 1);
            buf[0, 0] = L / 2;/// J =  ∂x / ∂ξ
            return buf;
        }

        public Matrix ComputeNMatrixAt(params double[] location)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override Matrix GetLambdaMatrix()
        {
            var p0 = nodes.First().Location;// + TrialDisplacements[0].Displacements;
            var p1 = nodes.Last().Location;// + TrialDisplacements[1].Displacements;

            var v = p1 - p0;

            var tr = BriefFiniteElementNet.Utils.BarElementUtils.GetBarTransformationMatrix(v,_webRotation);//, 0, this.MatrixPool);

            //tr = tr.Transpose();// 
            tr.TransposeInPlace();

            return tr;
            /*

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

            if (MathUtil.FEquals(0, v.X) && MathUtil.FEquals(0, v.Y))
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

            //transformation for webrotation
            
            var pars = new double[9];

            pars[0] = cxx;
            pars[1] = cxy * c + cxz * s;
            pars[2] = -cxy * s + cxz * c;

            pars[3] = cyx;
            pars[4] = cyy * c + cyz * s;
            pars[5] = -cyy * s + cyz * c;

            pars[6] = czx;
            pars[7] = czy * c + czz * s;
            pars[8] = -czy * s + czz * c;
            

            var buf = 
                //new Matrix(3, 3);
                MatrixPool.Allocate(3, 3);

            //buf.FillRow(0, pars[0], pars[1], pars[2]);
            //buf.FillRow(1, pars[3], pars[4], pars[5]);
            //buf.FillRow(2, pars[6], pars[7], pars[8]);

            buf.FillRow(0, cxx, cxy * c + cxz * s, -cxy * s + cxz * c);
            buf.FillRow(1, cyx, cyy * c + cyz * s, -cyy * s + cyz * c);
            buf.FillRow(2, czx, czy * c + czz * s, -czy * s + czz * c);

            return buf;
            */
        }

        public override IElementHelper[] GetHelpers()
        {
            return GetElementHelpers().ToArray();
        }



        public override Force[] GetGlobalEquivalentNodalLoads(ElementalLoad load)
        {
            var helpers = GetElementHelpers();

            var buf = new Force[nodes.Length];

            var t = GetTransformationManager();

            foreach (var helper in helpers)
            {
                var forces = helper.GetLocalEquivalentNodalLoads(this, load);

                for (var i = 0; i < buf.Length; i++)
                {
                    buf[i] = buf[i] + forces[i];
                }
            }


            for (var i = 0; i < buf.Length; i++)
                buf[i] = t.TransformLocalToGlobal(buf[i]);


            return buf;
        }

        public override Matrix GetGlobalDampingMatrix()
        {
            var local = GetLocalDampMatrix();
            var t = GetTransformationMatrix();

            var tr = GetTransformationManager();

            var globalDamp = tr.TransformLocalToGlobal(local);

            //CalcUtil.ApplyTransformMatrix(local, t);

            return globalDamp;
        }

        public override Matrix GetGlobalMassMatrix()
        {
            var local = GetLocalMassMatrix();

            //var t = GetTransformationMatrix();

            var tr = GetTransformationManager();

            //CalcUtil.ApplyTransformMatrix(local, t);

            var global = tr.TransformLocalToGlobal(local);

            tr.ReturnMatrixesToPool();
            local.ReturnToPool();

            return global;
        }

        public override Matrix GetGlobalStifnessMatrix()
        {
            var local = GetLocalStifnessMatrix();

            //var t = GetTransformationMatrix();

            //var mgr = TransformManagerL2G.MakeFromTransformationMatrix(t);

            var mgr = GetTransformationManager();

            var buf = mgr.TransformLocalToGlobal(local);

            local.ReturnToPool();

            mgr.ReturnMatrixesToPool();

            return buf;
        }

        public Matrix GetTransformationMatrix()
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

            var v = EndNode.Location - StartNode.Location;

            if (MathUtil.FEquals(0, v.X) && MathUtil.FEquals(0, v.Y))
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

            var pars = new double[9];

            pars[0] = cxx;
            pars[1] = cxy * c + cxz * s;
            pars[2] = -cxy * s + cxz * c;

            pars[3] = cyx;
            pars[4] = cyy * c + cyz * s;
            pars[5] = -cyy * s + cyz * c;

            pars[6] = czx;
            pars[7] = czy * c + czz * s;
            pars[8] = -czy * s + czz * c;

            /*
            var buf =
            //new Matrix(3, 3);
            MatrixPool.Allocate(3, 3);

            buf.SetColumn(0, new double[] { pars[0], pars[1], pars[2] });
            buf.SetColumn(1, new double[] { pars[3], pars[4], pars[5] });
            buf.SetColumn(2, new double[] { pars[6], pars[7], pars[8] });
            //*/

            // pars array is column major
            return new Matrix(3, 3, pars);
        }

        /// <inheritdoc/>
        public override double[] IsoCoordsToLocalCoords(params double[] isoCoords)
        {
            var pl = GetIsoToLocalConverter().Evaluate(isoCoords[0]);

            return new double[] { pl };
        }



        /// <summary>
        /// returns the coordinates of defined location on iso coord system in the global coordination system
        /// Note that this is not vector conversion, but is point conversion so StartNode.Location is taken into account
        /// </summary>
        /// <param name="isoCoords"></param>
        /// <returns></returns>
        public Point IsoCoordsToGlobalCoords(params double[] isoCoords)
        {
            var localA = IsoCoordsToLocalCoords(isoCoords);
            var local = new Vector(localA[0], 0, 0);

            var tr = GetTransformationManager();

            var global = tr.TransformLocalToGlobal(local);

            var globalP = StartNode.Location + global;

            return globalP;
        }

        public double[] LocalCoordsToIsoCoords(params double[] localCoords)
        {
            var pl = GetIsoToLocalConverter();
            var x = localCoords[0];

            double rt;

            if (!pl.TryFindRoot(localCoords[0], out rt))
            {
                throw new Exception();
            }

            return new double[] { rt };
        }

        /// <summary>
        /// Gets the stifness matrix in local coordination system.
        /// </summary>
        /// <returns>stiffness matrix</returns>
        public virtual Matrix GetLocalStifnessMatrix()
        {
            var helpers = GetElementHelpers();

            var buf =
                MatrixPool.Allocate(6 * nodes.Length, 6 * nodes.Length);

            //var transMatrix = GetTransformationMatrix();

            for (var i = 0; i < helpers.Count; i++)
            {
                var helper = helpers[i];

                var ki = helper.CalcLocalStiffnessMatrix(this);// ComputeK(helper, transMatrix);

                var dofs = helper.GetDofOrder(this);


                for (var ii = 0; ii < dofs.Length; ii++)
                {
                    var bi = dofs[ii].NodeIndex * 6 + (int)dofs[ii].Dof;

                    for (var jj = 0; jj < dofs.Length; jj++)
                    {
                        var bj = dofs[jj].NodeIndex * 6 + (int)dofs[jj].Dof;

                        buf[bi, bj] += ki[ii, jj];
                    }
                }

                ki.ReturnToPool();
            }


            for (var i = 0; i < 6 * NodeCount; i++)
            {
                if (buf[i, i] < 0)
                    Guid.NewGuid();

            }
            return buf;
        }

        /// <summary>
        /// Gets the  damp matrix in local coordination system.
        /// </summary>
        /// <returns>damp matrix</returns>
        public Matrix GetLocalDampMatrix()
        {
            var helpers = GetElementHelpers();

            var buf = new Matrix(12, 12);

            var transMatrix = GetTransformationMatrix();

            for (var i = 0; i < helpers.Count; i++)
            {
                var helper = helpers[i];

                var ki = helper.CalcLocalDampMatrix(this);// ComputeK(helper, transMatrix);

                var dofs = helper.GetDofOrder(this);

                for (var ii = 0; ii < dofs.Length; ii++)
                {
                    var bi = dofs[ii].NodeIndex * 6 + (int)dofs[ii].Dof;

                    for (var jj = 0; jj < dofs.Length; jj++)
                    {
                        var bj = dofs[jj].NodeIndex * 6 + (int)dofs[jj].Dof;

                        buf[bi, bj] += ki[ii, jj];
                    }
                }
            }

            return buf;
        }

        /// <summary>
        /// Gets the mass matrix in local coordination system.
        /// </summary>
        /// <returns>mass matrix</returns>
        public Matrix GetLocalMassMatrix()
        {
            var helpers = GetElementHelpers();

            var buf = new Matrix(12, 12);

            var transMatrix = GetTransformationMatrix();

            for (var i = 0; i < helpers.Count; i++)
            {
                var helper = helpers[i];

                var ki = helper.CalcLocalMassMatrix(this);// ComputeK(helper, transMatrix);

                var dofs = helper.GetDofOrder(this);

                for (var ii = 0; ii < dofs.Length; ii++)
                {
                    var bi = dofs[ii].NodeIndex * 6 + (int)dofs[ii].Dof;

                    for (var jj = 0; jj < dofs.Length; jj++)
                    {
                        var bj = dofs[jj].NodeIndex * 6 + (int)dofs[jj].Dof;

                        buf[bi, bj] += ki[ii, jj];
                    }
                }
            }

            return buf;
        }

        /// <summary>
        /// Gets the list of element helpers reagarding <see cref="Behavior"/>.
        /// </summary>
        /// <returns></returns>
        protected List<IElementHelper> GetElementHelpers()
        {
            var helpers = new List<IElementHelper>();

            if ((_behavior & BarElementBehaviour.BeamYEulerBernoulli) != 0)
            {
                helpers.Add(new EulerBernoulliBeamHelper(BeamDirection.Y, this));
            }

            if ((_behavior & BarElementBehaviour.BeamYTimoshenko) != 0)
            {
                helpers.Add(new TimoshenkoBeamHelper(BeamDirection.Y, this));
            }

            if ((_behavior & BarElementBehaviour.BeamZEulerBernoulli) != 0)
            {
                helpers.Add(new EulerBernoulliBeamHelper(BeamDirection.Z, this));
            }

            if ((_behavior & BarElementBehaviour.BeamZTimoshenko) != 0)
            {
                helpers.Add(new TimoshenkoBeamHelper(BeamDirection.Z, this));
            }

            if ((_behavior & BarElementBehaviour.Truss) != 0)
            {
                helpers.Add(new TrussHelper(this));
            }

            if ((_behavior & BarElementBehaviour.Shaft) != 0)
            {
                helpers.Add(new ShaftHelper(this));
            }

            return helpers;
        }

        #region ISerialization Implementation

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_webRotation", _webRotation);
            info.AddValue("_material", _material);
            info.AddValue("_section", _section);
            info.AddValue("_behavior", (int)_behavior);
            info.AddValue("_nodalReleaseConditions", _nodalReleaseConditions);

        }



        protected BarElementMultyNode(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _webRotation = (double)info.GetValue("_webRotation", typeof(double));
            _material = (BaseMaterial)info.GetValue("_material", typeof(BaseMaterial));
            _behavior = (BarElementBehaviour)info.GetValue("_behavior", typeof(int));
            _section = (Base1DSection)info.GetValue("_section", typeof(Base1DSection));
            _nodalReleaseConditions = (Constraint[])info.GetValue("_nodalReleaseConditions", typeof(Constraint[]));
        }

        #endregion

        #region Constructor

        public BarElementMultyNode() : this(2)
        {
        }



        #endregion

        #region GetInternalForceAt, GetInternalForceAt_Exact

        /// <summary>
        /// Gets the internal force at <see cref="xi" /> position.
        /// </summary>
        /// <param name="xi">The iso coordinate of desired point (start = -1, mid = 0, end = 1).</param>
        /// <param name="combination">The Load Combination.</param>
        /// <returns></returns>
        /// <remarks>
        /// Will calculate the internal forces of member regarding the <see cref="combination" />
        /// </remarks>
        public Force GetInternalForceAt(double xi, LoadCombination combination)
        {
            var buf = Force.Zero;

            foreach (var lc in combination.Keys)
                buf += combination[lc] * GetInternalForceAt(xi, lc);

            return buf;
        }

        /// <summary>
        /// Gets the exact internal force at <see cref="xi" /> position.
        /// </summary>
        /// <param name="xi">The iso coordinate of desired point (start = -1, mid = 0, end = 1).</param>
        /// <param name="combination">The Load Combination.</param>
        /// <returns></returns>
        /// <remarks>
        /// Will calculate the internal forces of member regarding the <see cref="combination" />
        /// </remarks>
        public Force GetExactInternalForceAt(double xi, LoadCombination combination)
        {
            var buf = Force.Zero;

            foreach (var lc in combination.Keys)
                buf += combination[lc] * GetExactInternalForceAt(xi, lc);

            return buf;
        }

        /// <summary>
        /// Gets the internal force at <see cref="xi" /> position.
        /// </summary>
        /// <param name="xi">The iso coordinate of desired point (start = -1, mid = 0, end = 1).</param>
        /// <param name="loadCase">The Load case.</param>
        /// <returns></returns>
        /// <remarks>
        /// Will calculate the internal forces of member regarding the <see cref="loadCase" />
        /// </remarks>
        public virtual Force GetInternalForceAt(double xi, LoadCase loadCase)
        {
            var helpers = GetHelpers();

            var lds = new Displacement[Nodes.Length];
            var tr = GetTransformationManager();

            for (var i = 0; i < Nodes.Length; i++)
            {
                var globalD = Nodes[i].GetNodalDisplacement(loadCase);
                var local = tr.TransformGlobalToLocal(globalD);
                lds[i] = local;
            }

            //var buff = new Force();

            var frc = new Vector();//forcec
            var mnt = new Vector();//moment


            foreach (var helper in helpers)
            {
                var tns = helper.GetLocalInternalForceAt(this, lds, new[] { xi });

                foreach (var tuple in tns)
                {
                    switch (tuple.Item1)
                    {
                        case DoF.Dx:
                            frc.X += tuple.Item2;
                            break;
                        case DoF.Dy:
                            frc.Y += tuple.Item2;
                            break;
                        case DoF.Dz:
                            frc.Z += tuple.Item2;
                            break;
                        case DoF.Rx:
                            mnt.X += tuple.Item2;
                            break;
                        case DoF.Ry:
                            mnt.Y += tuple.Item2;
                            break;
                        case DoF.Rz:
                            mnt.Z += tuple.Item2;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return new Force(frc, mnt);
        }


        /// <summary>
        /// Gets the internal force at <see cref="xi" /> position.
        /// </summary>
        /// <param name="xi">The iso coordinate of desired point (start = -1, mid = 0, end = 1).</param>
        /// <param name="loadCase">The Load case.</param>
        /// <returns></returns>
        /// <remarks>
        /// Will calculate the internal forces of member regarding the <see cref="loadCase" />
        /// </remarks>
        public Force GetExactInternalForceAt(double xi, LoadCase loadCase)
        {
            {
                //check to see if xi is exactly on any discrete points, for example shear exactly under a a concentrated force point is not a single value so exception must thrown

                var discretePoints = new List<IsoPoint>();

                discretePoints.AddRange(GetInternalForceDiscretationPoints());


                foreach (var load in Loads)
                {
                    if (load.Case == loadCase)
                        discretePoints.AddRange(load.GetInternalForceDiscretationPoints());
                }

                foreach (var point in discretePoints)
                {
                    if (xi == point.Xi)
                        throw new InvalidInternalForceLocationException(string.Format(CultureInfo.CurrentCulture, "Internal force diagram and value is descrete at xi = {0}, thus have no value in this location. try to find internal force a little bit after or before this point", xi));
                }
            }

            var approx =
                GetInternalForceAt(xi, loadCase);
            //Force.Zero;

            var fcs = new Dictionary<DoF, double>();

            //var buf = new FlatShellStressTensor();

            var helpers = GetHelpers();

            foreach (var load in Loads)
                if (load.Case == loadCase)
                    foreach (var helper in helpers)
                    {
                        var tns = helper.GetLoadInternalForceAt(this, load, new[] { xi });

                        foreach (var fc in tns)
                        {
                            double existing;

                            fcs.TryGetValue(fc.Item1, out existing);

                            fcs[fc.Item1] = existing + fc.Item2;
                        }
                    }

            var buff = new Force();
            buff += approx;//TODO: maybe += approx !

            if (fcs.ContainsKey(DoF.Dx))
                buff.Fx += fcs[DoF.Dx];

            if (fcs.ContainsKey(DoF.Dy))
                buff.Fy += fcs[DoF.Dy];

            if (fcs.ContainsKey(DoF.Dz))
                buff.Fz += fcs[DoF.Dz];

            if (fcs.ContainsKey(DoF.Rx))
                buff.Mx += fcs[DoF.Rx];

            if (fcs.ContainsKey(DoF.Ry))
                buff.My += fcs[DoF.Ry];

            if (fcs.ContainsKey(DoF.Rz))
                buff.Mz += fcs[DoF.Rz];

            return buff;
        }

        /// <summary>
        /// Gets the internal force at.
        /// </summary>
        /// <param name="xi">The iso coordinate of desired point (start = -1, mid = 0, end = 1).</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <remarks>
        /// Will calculate the internal forces of member regarding Default load case.
        /// </remarks>
        public Force GetInternalForceAt(double xi)
        {
            return GetInternalForceAt(xi, LoadCase.DefaultLoadCase);
        }

        /// <summary>
        /// Gets the exact internal force at specified <see cref="xi"/> for <see cref="LoadCase.DefaultLoadCase"/>.
        /// </summary>
        /// <param name="xi">The iso coordinate of desired point (start = -1, mid = 0, end = 1).</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <remarks>
        /// Will calculate the internal forces of member regarding Default load case.
        /// </remarks>
        public Force GetExactInternalForceAt(double xi)
        {
            return GetExactInternalForceAt(xi, LoadCase.DefaultLoadCase);
        }




        #endregion

        #region GetInternalDisplacementAt, GetExactInternalDisplacementAt


        public Displacement GetInternalDisplacementAt(double xi, LoadCombination combination)
        {
            var buf = new Displacement();

            foreach (var pair in combination)
                buf += pair.Value * GetInternalDisplacementAt(xi, pair.Key);

            return buf;
        }


        public Displacement GetExactInternalDisplacementAt(double xi, LoadCombination combination)
        {
            throw new NotImplementedException();
        }


        public Displacement GetInternalDisplacementAt(double xi, LoadCase loadCase)
        {
            var buf = Displacement.Zero;

            var helpers = GetHelpers();

            var lds = new Displacement[Nodes.Length];
            var tr = GetTransformationManager();

            for (var i = 0; i < Nodes.Length; i++)
            {
                var globalD = Nodes[i].GetNodalDisplacement(loadCase);
                var local = tr.TransformGlobalToLocal(globalD);
                lds[i] = local;
            }

            foreach (var hlpr in helpers)
            {
                buf += hlpr.GetLocalDisplacementAt(this, lds, xi);
            }

            return buf;
        }


        public Displacement GetExactInternalDisplacementAt(double xi, LoadCase loadCase)
        {
            throw new NotImplementedException();
        }


        public Displacement GetInternalDisplacementAt(double xi)
        {
            return GetInternalDisplacementAt(xi, LoadCase.DefaultLoadCase);
        }


        public Displacement GetExactInternalDisplacementAt(double xi)
        {
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// get the polynomial that takes iso coord as input and return local coord as output
        /// </summary>
        /// <returns>X(ξ) (ξ input, X output)</returns>
        public virtual Mathh.SingleVariablePolynomial GetIsoToLocalConverter()
        {
            var cachekey = "{54CEC6B2-F882-4505-9FC0-E7844C99F249}";

            Mathh.SingleVariablePolynomial chd;

            if (this.TryGetCache(cachekey, out chd))//prevent double calculation
                if (IsValidIsoToLocalConverter(chd))//Validation of chached polynomial to see if is outdated due to change in node locations
                    return chd;

            chd = null;

            var targetElement = this;
            var bar = this;

            Mathh.SingleVariablePolynomial x_xi = null;//x(ξ)

            var n = targetElement.Nodes.Length;

            var xs = new double[n];
            var xi_s = new double[n];

            {
                //var conds = new List<Tuple<double, double>>();//x[i] , ξ[i]

                for (var i = 0; i < n; i++)
                {
                    var deltaXi = 2.0 / (n - 1);
                    var xi = (bar.Nodes[i].Location - bar.Nodes[0].Location).Length;
                    var xi_i = -1 + deltaXi * i;

                    xs[i] = xi;
                    xi_s[i] = xi_i;

                    //conds.Add(Tuple.Create(xi, xi_i));
                }

                //polinomial degree of shape function is n-1


                var mtx =
                    //new Matrix(n, n);
                    MatrixPool.Allocate(n, n);

                var right =
                    //new Matrix(n, 1);
                    MatrixPool.Allocate(n, 1);

                //var o = n - 1;

                for (var i = 0; i < n; i++)
                {
                    //fill row i'th of mtx

                    //x[i] = { ξ[i]^o, ξ[i]^o-1 ... ξ[i]^1 ξ[i]^0} * {a[o] a[o-1] ... a[1] a[0]}'

                    var kesi_i = xi_s[i];

                    for (var j = 0; j < n; j++)
                    {
                        mtx[i, j] = Math.Pow(kesi_i, n - j - 1);
                        right[i, 0] = xs[i];
                    }
                }

                //var as_ = mtx.Inverse() * right;
                var as_ = mtx.Solve(right.Values);

                x_xi = new Mathh.SingleVariablePolynomial(as_);


                right.ReturnToPool();
                mtx.ReturnToPool();
            }

            SetCache(cachekey, x_xi);

            return x_xi;
        }

        /// <summary>
        /// determines is the defined converter a valid one for converting iso coord to local coords
        /// </summary>
        /// <param name="converter"></param>
        /// <returns></returns>
        private bool IsValidIsoToLocalConverter(BriefFiniteElementNet.Mathh.SingleVariablePolynomial converter)
        {
            return true;
            var bar = this;
            var n = NodeCount;

            var xs = new double[n];
            var xi_s = new double[n];

            {
                //var conds = new List<Tuple<double, double>>();//x[i] , ξ[i]

                for (var i = 0; i < n; i++)
                {
                    var deltaXi = 2.0 / (n - 1);
                    var xi = (bar.Nodes[i].Location - bar.Nodes[0].Location).Length;
                    var xi_i = -1 + deltaXi * i;

                    xs[i] = xi;
                    xi_s[i] = xi_i;
                }

                var poly = converter;

                {//test
                    for (var i = 0; i < n; i++)
                    {
                        var epsilon = poly.Evaluate(xi_s[i]) - xs[i];

                        if (Math.Abs(epsilon) > 1e-10)
                            return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the iso location of points that internal force in those points are discrete only due to element.
        /// </summary>
        /// <returns>list of iso locations</returns>
        public IsoPoint[] GetInternalForceDiscretationPoints()
        {
            var buf = new List<IsoPoint>();

            foreach (var node in nodes)
            {
                var x = (node.Location - nodes[0].Location).Length;

                var xi = LocalCoordsToIsoCoords(x)[0];

                buf.Add(new IsoPoint(xi));
            }

            //Note: internal loads are not accounted
            /*
            foreach (var load in this.loads)
            {
                if (load.Case != loadCase)
                    continue;

                var pts = load.GetInternalForceDiscretationPoints();

                buf.AddRange(pts);
            }
            */
            var b2 = new List<IsoPoint>();


            foreach (var point in buf.OrderBy(i => i.Xi))
            {
                var pt = new IsoPoint(point.Xi);

                if (!b2.Contains(pt))
                    b2.Add(pt);
            }

            return b2.ToArray();
        }

        /// <summary>
        /// Gets the iso location of points that internal force in those points are discrete only due to element.
        /// </summary>
        /// <returns>list of iso locations</returns>
        public IsoPoint[] GetInternalForceDiscretationPoints(LoadCase loadCase)
        {
            var buf = new List<IsoPoint>();

            foreach (var node in nodes)
            {
                var x = (node.Location - nodes[0].Location).Length;

                var xi = LocalCoordsToIsoCoords(x)[0];

                buf.Add(new IsoPoint(xi));
            }

            //Note: internal loads are not accounted
            foreach (var load in loads)
            {
                if (load.Case != loadCase)
                    continue;

                var pts = load.GetInternalForceDiscretationPoints();

                buf.AddRange(pts);
            }
            var b2 = new List<IsoPoint>();


            foreach (var point in buf.OrderBy(i => i.Xi))
            {
                var pt = new IsoPoint(point.Xi);

                if (!b2.Contains(pt))
                    b2.Add(pt);
            }

            return b2.ToArray();
        }
    }
}
