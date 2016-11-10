using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.ElementHelpers;

namespace BriefFiniteElementNet.Elements
{
    [Obsolete("not fully implemented yet")]
    public class BarElement : Element
    {
        #region Field & Properties

        private double _webRotation;
        private bool _considerShearDeformation;
        private BarElementEndConnection _startConnection;
        private BarElementEndConnection _endtConnection;
        private BarElementBehaviour _behavior;
        private BaseBarElementCrossSection _section;
        private BaseBarElementMaterial _matterial;


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
        public BarElementEndConnection StartConnection
        {
            get { return _startConnection; }
            set { _startConnection = value; }
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
        public BarElementEndConnection EndConnection
        {
            get { return _endtConnection; }
            set { _endtConnection = value; }
        }

        /// <summary>
        /// Gets or sets the web rotation of this member in Degree
        /// </summary>
        /// <value>
        /// The web rotation in degree. It does rotate the local coordination system of element.
        /// </value>
        public double WebRotation
        {
            get { return _webRotation; }
            set { _webRotation = value; }
        }

        /// <summary>
        /// Gets or sets the cross section of bar element.
        /// </summary>
        /// <value>
        /// The section.
        /// </value>
        public BaseBarElementCrossSection Section
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
        public BaseBarElementMaterial Material
        {
            get { return _matterial; }
            set { _matterial = value; }
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


            {// take the end connections into account
                //b is 4 X 12, each row for one DoF

                var d1 = _startConnection;
                var d2 = _endtConnection;

                var arr = new DofConstraint[] {
                    d1.Dx, d1.Dy, d1.Dz, d1.Rx, d1.Ry, d1.Rz ,
                    d2.Dx, d2.Dy, d2.Dz, d2.Rx, d2.Ry, d2.Rz };

                for (var i = 0; i < 12; i++)
                    if (arr[i] == DofConstraint.Released)
                        buf.SetColumn(i, 0.0, 0.0, 0.0, 0.0);
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

        public override Matrix ComputeNMatrixAt(params double[] location)
        {
            throw new NotImplementedException();
        }

        public override Force[] GetEquivalentNodalLoads(Load load)
        {
            throw new NotImplementedException();
        }

        public override Matrix GetGlobalDampingMatrix()
        {
            throw new NotImplementedException();
        }

        public override Matrix GetGlobalMassMatrix()
        {
            throw new NotImplementedException();
        }

        public override Matrix GetGlobalStifnessMatrix()
        {
            throw new NotImplementedException();
        }

        private Matrix GetTransformationMatrix()
        {
            throw new NotImplementedException();
        }

        public Matrix GetLocalStifnessMatrix()
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

            var buf = new Matrix(12, 12);

            var transMatrix = GetTransformationMatrix();

            for (var i = 0; i < helpers.Count; i++)
            {
                var helper = helpers[i];

                var ki = helper.GetKMatrix(this, transMatrix);
                var dofs = helper.GetDofOrder(this);

                //TODO: substitude ki into buf
                
                throw new NotImplementedException();
            }

            return buf;
        }
    }
}
