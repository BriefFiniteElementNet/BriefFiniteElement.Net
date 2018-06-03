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

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents a Bar element with two nodes (start and end)
    /// </summary>
    [Serializable]
    [Obsolete("not fully implemented yet")]
    public class BarElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarElement"/> class.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        public BarElement(Node n1, Node n2) : base(2)
        {
            StartNode = n1;
            EndNode = n2;
        }

        #region Field & Properties

        private double _webRotation;

        //private BarElementEndConnection _startConnection = BarElementEndConnection.Fixed;
        //private BarElementEndConnection _endtConnection = BarElementEndConnection.Fixed;
        private BarElementBehaviour _behavior = BarElementBehaviours.FullFrame;
        private Base1DSection _section;
        private BaseMaterial _material;
        private Constraint _startReleaseCondition = Constraints.Fixed;
        private Constraint _endReleaseCondition = Constraints.Fixed;

        /// <summary>
        /// Gets or sets the start node.
        /// </summary>
        /// <value>
        /// The start node of <see cref="BarElement"/>.
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
        /// The end node of <see cref="BarElement"/>.
        /// </value>
        public Node EndNode
        {
            get { return nodes[1]; }
            set { nodes[1] = value; }
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
            get { return _startReleaseCondition; }
            set { _startReleaseCondition = value; }
        }

        /// <summary>
        /// Gets or sets the connection constraints od element to the end node
        /// </summary>
        public Constraint EndReleaseCondition
        {
            get { return _endReleaseCondition; }
            set { _endReleaseCondition = value; }
        }

        #endregion

        public override Matrix ComputeBMatrix(params double[] location)
        {
            var L = (EndNode.Location - StartNode.Location).Length;

            var L2 = L * L;
            var L3 = L2 * L;


            //location is xi varies from -1 to 1
            var xi = location[0];

            if (xi < -1 || xi > 1)
                throw new ArgumentOutOfRangeException(nameof(location));

            var buf = new Matrix(4, 12);

            
            if ((this._behavior & BarElementBehaviour.BeamYEulerBernoulli) != 0)
            {
                //BeamY is in behaviors, should use beam with Iz

                var arr = new double[] { 0, (6 * xi) / L2, 0, 0,
                0, (3 * xi) / L - 1 / L, 0, -(6 * xi) / L2,
                0, 0, 0, (3 * xi) / L + 1 / L};

                buf.FillRow(0, arr);
            }

            if ((this._behavior & BarElementBehaviour.BeamYTimoshenko) != 0)
            {
                throw new NotImplementedException();

                double c;

                {
                    var e = 1.0;
                    var g = 1.0;
                    var i = 1.0;
                    var k = 1.0;

                    var a = e*i/L3;
                    var b = k*g*a*L;//6.20 ref[4]

                    c = 6*a*L/(12*a*L2 + b);
                }

                var arr = new double[]
                {
                    0, -12*c*L*xi + 6*xi, 0, 0,
                    0, - 6*c*L*xi + L*(3*xi - 1), 0, 12*c*L*xi - 6*xi,
                    0, 0, 0, 6*c*L*xi + L*(3*xi + 1)
                };

                buf.FillRow(0, arr);
            }

            if ((this._behavior & BarElementBehaviour.BeamZEulerBernoulli) != 0)
            {
                //BeamZ in behaviours, should use beam with Iy

                var arr = new double[] {  0, 0, (6 * xi) / L2, 0,
                (3 * xi) / L - 1 / L, 0, 0, 0,
                -(6 * xi) / L2, 0, (3 * xi) / L + 1 / L, 0};

                buf.FillRow(1, arr);
            }

            if ((this._behavior & BarElementBehaviour.BeamZTimoshenko) != 0)
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

                buf.FillRow(1, arr);
            }

            if ((this._behavior & BarElementBehaviour.Truss) != 0)
            {
                var arr = new double[] {  1 / L, 0, 0, 0,
                0, 0, -1 / L, 0,
                0, 0, 0, 0,};

                buf.FillRow(2, arr);
            }

            if ((this._behavior & BarElementBehaviour.Shaft) != 0)
            {
                var arr = new double[] {   0, 0, 0, 1 / L,
                0, 0, 0, 0,
                0, -1 / L, 0, 0};

                buf.FillRow(3, arr);
            }


            

            return buf;
        }

        public override Matrix ComputeDMatrixAt(params double[] location)
        {
            double e = 0.0, g = 0;//mechanical

            double iz = 0, iy = 0, j = 0, a = 0;//geometrical

            var buf = new Matrix(4, 4);

            buf[0, 0] = e * iz;
            buf[1, 1] = e * iy;
            buf[2, 2] = e * a;
            buf[3, 3] = e * j;

            return buf;
        }

        public override Matrix ComputeJMatrixAt(params double[] location)
        {
            // J =  ∂x / ∂ξ
            var L = (EndNode.Location - StartNode.Location).Length;

            var buf =new Matrix(1,1);
            buf[0, 0] = L/2;/// J =  ∂x / ∂ξ
            return buf;
        }

        public override Matrix GetLambdaMatrix()
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


            var buf = new Matrix(3, 3);

            //buf.FillColumn(0, pars[0], pars[1], pars[2]);
            //buf.FillColumn(1, pars[3], pars[4], pars[5]);
            //buf.FillColumn(2, pars[6], pars[7], pars[8]);

            buf.FillRow(0, pars[0], pars[1], pars[2]);
            buf.FillRow(1, pars[3], pars[4], pars[5]);
            buf.FillRow(2, pars[6], pars[7], pars[8]);

            return buf;
        }

        public override IElementHelper[] GetHelpers()
        {
            return GetElementHelpers().ToArray();
        }

        public override Matrix ComputeNMatrixAt(params double[] location)
        {
            throw new NotImplementedException();
        }

        public override Force[] GetGlobalEquivalentNodalLoads(Load load)
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

            CalcUtil.ApplyTransformMatrix(local, t);

            return local;
        }

        public override Matrix GetGlobalMassMatrix()
        {
            var local = GetLocalMassMatrix();

            var t = GetTransformationMatrix();

            CalcUtil.ApplyTransformMatrix(local, t);

            return local;
        }

        public override Matrix GetGlobalStifnessMatrix()
        {
            var local = GetLocalStifnessMatrix();

            var t = GetTransformationMatrix();

            var mgr = TransformManagerL2G.MakeFromTransformationMatrix(t);

            var buf = mgr.TransformLocalToGlobal(local);

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


            var buf = new Matrix(3, 3);

            buf.FillColumn(0, pars[0], pars[1], pars[2]);
            buf.FillColumn(1, pars[3], pars[4], pars[5]);
            buf.FillColumn(2, pars[6], pars[7], pars[8]);

            return buf;
        }

        /// <inheritdoc/>
        public override double[] IsoCoordsToLocalCoords(params double[] isoCoords)
        {
            var xi = isoCoords[0];

            var L = (this.EndNode.Location - this.StartNode.Location).Length;

            return new double[] { L / 2 * (xi + 1) } ;
        }

        /// <summary>
        /// Gets the stifness matrix in local coordination system.
        /// </summary>
        /// <returns>stiffness matrix</returns>
        public Matrix GetLocalStifnessMatrix()
        {
            var helpers = GetElementHelpers();



            var buf = new Matrix(12, 12);

            //var transMatrix = GetTransformationMatrix();

            for (var i = 0; i < helpers.Count; i++)
            {
                var helper = helpers[i];

                var ki = helper.CalcLocalKMatrix(this);// ComputeK(helper, transMatrix);

                var dofs = helper.GetDofOrder(this);


                for (var ii = 0; ii < dofs.Length; ii++)
                {
                    var bi = dofs[ii].NodeIndex*6 + (int)dofs[ii].Dof;

                    for (var jj = 0; jj < dofs.Length; jj++)
                    {
                        var bj = dofs[jj].NodeIndex*6 + (int)dofs[jj].Dof;

                        buf[bi, bj] += ki[ii, jj];
                    }
                }
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

                var ki = helper.CalcLocalCMatrix(this);// ComputeK(helper, transMatrix);

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

                var ki = helper.CalcLocalMMatrix(this);// ComputeK(helper, transMatrix);

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
        private List<IElementHelper> GetElementHelpers()
        {
            var helpers = new List<IElementHelper>();

            if ((this._behavior & BarElementBehaviour.BeamYEulerBernoulli) != 0)
            {
                helpers.Add(new EulerBernoulliBeamHelper(BeamDirection.Y));
            }

            if ((this._behavior & BarElementBehaviour.BeamYTimoshenko) != 0)
            {
                helpers.Add(new TimoshenkoBeamHelper(BeamDirection.Y));
            }

            if ((this._behavior & BarElementBehaviour.BeamZEulerBernoulli) != 0)
            {
                helpers.Add(new EulerBernoulliBeamHelper(BeamDirection.Z));
            }

            if ((this._behavior & BarElementBehaviour.BeamZTimoshenko) != 0)
            {
                helpers.Add(new TimoshenkoBeamHelper(BeamDirection.Z));
            }

            if ((this._behavior & BarElementBehaviour.Truss) != 0)
            {
                helpers.Add(new TrussHelper());
            }

            if ((this._behavior & BarElementBehaviour.Shaft) != 0)
            {
                helpers.Add(new ShaftHelper());
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
            info.AddValue("_startReleaseCondition", _startReleaseCondition);
            info.AddValue("_endReleaseCondition", _endReleaseCondition);
            
        }

       

        protected BarElement(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _webRotation = (double)info.GetValue("_webRotation", typeof(double));
            _material = (BaseMaterial)info.GetValue("_material", typeof(BaseMaterial));
            _behavior = (BarElementBehaviour)info.GetValue("_behavior", typeof(int));
            _section = (Base1DSection)info.GetValue("_section", typeof(Base1DSection));
            _startReleaseCondition = (Constraint)info.GetValue("_startReleaseCondition", typeof(Constraint));
            _endReleaseCondition = (Constraint)info.GetValue("_endReleaseCondition", typeof(Constraint));
        }

        #endregion

        #region Constructor

        public BarElement():base(2)
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
                buf += this.GetInternalForceAt(xi, lc);

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
                buf += this.GetExactInternalForceAt(xi, lc);

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
        public Force GetInternalForceAt(double xi, LoadCase loadCase)
        {
            var buf = new FlatShellStressTensor();

            var helpers = GetHelpers();

            var lds = new Displacement[this.Nodes.Length];
            var tr = this.GetTransformationManager();

            for (var i = 0; i < Nodes.Length; i++)
            {
                var globalD = Nodes[i].GetNodalDisplacement(loadCase);
                var local = tr.TransformGlobalToLocal(globalD);
                lds[i] = local;
            }

            var buff = new Force();

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
                //buf = buf + tns;
            }

            

            var forces = new Vector(buf.MembraneTensor.S11, buf.MembraneTensor.S12, buf.MembraneTensor.S13);
            //Fx, Vy, Vz
            var moments = new Vector(buf.BendingTensor.M11, buf.BendingTensor.M12, buf.BendingTensor.M13);
            //Mx, My, Mz

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
            var approx = GetInternalForceAt(xi, loadCase);

            var buf = new FlatShellStressTensor();

            var helpers = GetHelpers();

            foreach (var load in this.Loads)
                foreach (var helper in helpers)
                {
                    var tns = helper.GetLoadInternalForceAt(this, load, new[] { xi });
                    //buf = buf + tns;
                }

            var buff = new Force();

            var forces = new Vector(buf.MembraneTensor.S11, buf.MembraneTensor.S12, buf.MembraneTensor.S13);
            //Fx, Vy, Vz
            var moments = new Vector(buf.BendingTensor.M11, buf.BendingTensor.M12, buf.BendingTensor.M13);
            //Mx, My, Mz

            return new Force(forces, moments) + approx;
        }

        /// <summary>
        /// Gets the internal force at.
        /// </summary>
        /// <param name="xi">The iso coordinate of desired point (start = -1, mid = 0, end = 1).</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
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
        /// <exception cref="System.NotImplementedException"></exception>
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
            throw new NotImplementedException();
        }

        
        public Displacement GetExactInternalDisplacementAt(double xi, LoadCombination combination)
        {
            throw new NotImplementedException();
        }

        
        public Displacement GetInternalDisplacementAt(double xi, LoadCase loadCase)
        {
            var buf = Displacement.Zero;

            var helpers = GetHelpers();

            var lds = new Displacement[this.Nodes.Length];
            var tr = this.GetTransformationManager();

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
            throw new NotImplementedException();
        }

        
        public Displacement GetExactInternalDisplacementAt(double xi, LoadCase loadCase)
        {
            throw new NotImplementedException();
        }

        
        public Displacement GetInternalDisplacementAt(double xi)
        {
            throw new NotImplementedException();
        }

        
        public Displacement GetExactInternalDisplacementAt(double xi)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
