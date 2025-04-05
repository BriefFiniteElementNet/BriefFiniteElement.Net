using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Elements.ElementHelpers;

namespace BriefFiniteElementNet.Elements.Elements
{
    /// <summary>
    /// represents a two noded parametric spring, parameters are copied to global stiffness matrix exactly without any modification or transform
    /// </summary>
    public class ParametricSpring : Element,ISerializable
    {

        #region proerties

        /// <summary>
        /// stiffness in global dof DX
        /// </summary>
        public double KDx { get; set; }

        /// <summary>
        /// stiffness in global dof DY
        /// </summary>
        public double KDy { get; set; }

        /// <summary>
        /// stiffness in global dof DZ
        /// </summary>
        public double KDz { get; set; }

        /// <summary>
        /// stiffness in global dof RX
        /// </summary>
        public double KRx { get; set; }

        /// <summary>
        /// stiffness in global dof RY
        /// </summary>
        public double KRy { get; set; }

        /// <summary>
        /// stiffness in global dof RZ
        /// </summary>
        public double KRz { get; set; }


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

    #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="BarElement"/> class.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        public ParametricSpring(Node n1, Node n2) : base(2)
        {
            StartNode = n1;
            EndNode = n2;
        }


        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(KDx), KDx);
            info.AddValue(nameof(KDy), KDy);
            info.AddValue(nameof(KDz), KDz);

            info.AddValue(nameof(KRx), KRx);
            info.AddValue(nameof(KRy), KRy);
            info.AddValue(nameof(KRz), KRz);

            base.GetObjectData(info, context);
        }

        protected ParametricSpring(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            KDx = info.GetDouble(nameof(KDx));
            KDy = info.GetDouble(nameof(KDy));
            KDz = info.GetDouble(nameof(KDz));

            KRx = info.GetDouble(nameof(KRx));
            KRy = info.GetDouble(nameof(KRy));
            KRz = info.GetDouble(nameof(KRz));
        }

        #region Element methods
        public Matrix GetGlobalStifnessMatrix()
        {
            var helpers = GetHelpers();

            var buf = MatrixPool.Allocate(12, 12); // 2 node

            for (var i = 0; i < helpers.Length; i++)
            {
                var helper = helpers[i];

                var ki = helper.CalcLocalStiffnessMatrix(this);

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

        public override Matrix GetGlobalMassMatrix()
        {
            var buf = new Matrix(6, 6);


            return buf;
        }

        public override Matrix GetGlobalDampingMatrix()
        {
            var buf = new Matrix(6, 6);


            return buf;

        }

        public override Force[] GetGlobalEquivalentNodalLoads(ElementalLoad load)
        {
            throw new BriefFiniteElementNetException(string.Format("{0} element cannot have internal loads",
                nameof(ParametricSpring)));
        }

        public override Matrix GetLambdaMatrix()
        {
            return Matrix.Eye(3);
        }

        public override double[] IsoCoordsToLocalCoords(params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public override IElementHelper[] GetHelpers()
        {
            var helpers = new List<IElementHelper>();
            {
                helpers.Add(new ParametricSpringHelper() { SpringType = DoF.Dx });
                helpers.Add(new ParametricSpringHelper() { SpringType = DoF.Dy });
                helpers.Add(new ParametricSpringHelper() { SpringType = DoF.Dz });
                helpers.Add(new ParametricSpringHelper() { SpringType = DoF.Rx });
                helpers.Add(new ParametricSpringHelper() { SpringType = DoF.Ry });
                helpers.Add(new ParametricSpringHelper() { SpringType = DoF.Rz });
            }

            return helpers.ToArray();
        }

        public override void GetGlobalStiffnessMatrix(Matrix stiffness)
        {
            var buf = this.GetGlobalMassMatrix();

            buf.CopyTo(stiffness);

            buf.ReturnToPool();
        }

        #endregion
    }
}
