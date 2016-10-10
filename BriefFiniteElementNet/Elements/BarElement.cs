using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private BaseBarElemenetCrossSection _section;


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

        #endregion

        public override Matrix ComputeBMatrix(params double[] location)
        {
            var L = (EndNode.Location - StartNode.Location).Length;

            var L2 = L * L;

            //location is xi varies from -1 to 1
            var xi = location[0];

            if (xi < -1 || xi > 1)
                throw new ArgumentOutOfRangeException(nameof(location));

            var buf = new Matrix(4, 12);

            
            if ((this._behavior & BarElementBehaviour.BeamY) != 0)
            {
                //BeamY is in behaviors, should use beam with Iz

                var arr = new double[] { 0, (6 * xi) / L2, 0, 0,
                0, (3 * xi) / L - 1 / L, 0, -(6 * xi) / L2,
                0, 0, 0, (3 * xi) / L + 1 / L};

                buf.FillRow(0, arr);
            }

            if ((this._behavior & BarElementBehaviour.BeamYWithShearDefomation) != 0)
            {
                throw new NotImplementedException();
            }

            if ((this._behavior & BarElementBehaviour.BeamZ) != 0)
            {
                //BeamZ in behaviours, should use beam with Iy

                var arr = new double[] {  0, 0, (6 * xi) / L2, 0,
                (3 * xi) / L - 1 / L, 0, 0, 0,
                -(6 * xi) / L2, 0, (3 * xi) / L + 1 / L, 0};

                buf.FillRow(1, arr);
            }

            if ((this._behavior & BarElementBehaviour.BeamZWithShearDefomation) != 0)
            {
                throw new NotImplementedException();
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
            
            throw new NotImplementedException();
        }

        public override Matrix ComputeJMatrixAt(params double[] location)
        {
            throw new NotImplementedException();
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
    }
}
