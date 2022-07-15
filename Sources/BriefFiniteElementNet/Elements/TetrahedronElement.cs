using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;

namespace BriefFiniteElementNet.Elements
{

    [Serializable]
    [Obsolete("not fully implemented yet")]
    public class TetrahedronElement:Element
    {
        public TetrahedronElement() : base(4)
        {
        }

        #region properties

        private BaseMaterial _material;

        public BaseMaterial Material
        {
            get { return _material; }
            set { _material = value; }
        }

        #endregion


        #region Deserialization Constructor

        protected TetrahedronElement(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _material = (BaseMaterial)info.GetValue("_material", typeof(BaseMaterial));
        }

        #endregion


        /// <summary>
        /// Checks the node order, throws exception if order is invalid
        /// </summary>
        private void CheckNodeOrder()
        {
            //https://academic.csuohio.edu/duffy_s/CVE_512_12.pdf
            //corner nodes numbered 1 through 4.  The nodes are numbered in such a way that the first three nodes are numbered in a counterclockwise fashion when viewed from the last node
            //we check for counterclockwise order of nodes 1-2-3 when viewed from node 4, if not counterclockwise then will change 2-3 to 3-2 and make it counterclockwise

            //base on this solution https://stackoverflow.com/a/1988164/1106889

            bool ccw; //wether 1-2-3 are ccw when viewed from 4

            {
                var a = nodes[0].Location;
                var b = nodes[1].Location;
                var c = nodes[2].Location;

                var v = nodes[3].Location;

                var ba = b - a;
                var ca = c - a;

                var av = a - v;

                var n = Vector.Cross(ba, ca);

                var w = Vector.Dot(n, av);

                ccw = w < 0;
            }

            //return ccw;
            if (!ccw)
                throw new Exception("invalid node order for tetrahedron element");
        }

        public void FixNodeOrder()
        {
            //https://academic.csuohio.edu/duffy_s/CVE_512_12.pdf
            //corner nodes numbered 1 through 4.  The nodes are numbered in such a way that the first three nodes are numbered in a counterclockwise fashion when viewed from the last node
            //we check for counterclockwise order of nodes 1-2-3 when viewed from node 4, if not counterclockwise then will change 2-3 to 3-2 and make it counterclockwise

            //base on this solution https://stackoverflow.com/a/1988164/1106889

            bool ccw; //wether 1-2-3 are ccw when viewed from 4

            {
                var a = nodes[0].Location;
                var b = nodes[1].Location;
                var c = nodes[2].Location;

                var v = nodes[3].Location;

                var ba = b - a;
                var ca = c - a;

                var av = a - v;

                var n = Vector.Cross(ba, ca);

                var w = Vector.Dot(n, av);

                ccw = w < 0;
            }

            //return ccw;
            if (!ccw)
            {
                var old = Nodes[2];
                Nodes[2] = Nodes[3];
                nodes[3] = old;
            }
        }

        #region stress
        /// <summary>
        /// Gets the internal stress at defined <see cref="isoLocation"/>.
        /// tensor is in global coordinate system. 
        /// </summary>
        /// <param name="loadCase">the load case </param>
        /// <param name="isoLocation"></param>
        /// <returns>Stress tensor of flat shell, in local coordination system</returns>
        /// <remarks>
        /// for more info about local coordinate of flat shell see page [72 of 166] (page 81 of pdf) of "Development of Membrane, Plate and Flat Shell Elements in Java" thesis by Kaushalkumar Kansara freely available on the web
        /// </remarks>
        public CauchyStressTensor GetLocalInternalStress(LoadCase loadCase, params double[] isoLocation)
        {
            var helpers = GetHelpers();

            var gds = new Displacement[this.Nodes.Length];//global displacements
            var tr = this.GetTransformationManager();

            for (var i = 0; i < Nodes.Length; i++)
            {
                var globalD = Nodes[i].GetNodalDisplacement(loadCase);
                var local = tr.TransformGlobalToLocal(globalD);
                gds[i] = local;
            }

            var trs = this.GetTransformationManager();

            var lds = gds.Select(i => tr.TransformGlobalToLocal(gds)).ToArray();

            var buf = new CauchyStressTensor();

            foreach (var helper in helpers)
            {
                var tns = helper.GetLocalInternalStressAt(this, gds, isoLocation);

                buf += tns.MembraneTensor;
            }

            return buf;
        }
        #endregion

        #region ISerialization Implementation

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_material", _material);
        }

        public override Matrix GetGlobalStifnessMatrix()
        {
            CheckNodeOrder();
            var local = GetLocalStifnessMatrix();

            var buf = local;

            return buf;
        }


        public Matrix GetLocalStifnessMatrix()
        {
            var helpers = GetHelpers();

            var buf = MatrixPool.Allocate(24, 24);

            for (var i = 0; i < helpers.Length; i++)
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
            }
            return buf;
        }

        public override Matrix GetGlobalMassMatrix()
        {
            CheckNodeOrder();
            throw new NotImplementedException();
        }

        public override Matrix GetGlobalDampingMatrix()
        {
            CheckNodeOrder();
            throw new NotImplementedException();
        }

        public override Force[] GetGlobalEquivalentNodalLoads(ElementalLoad load)
        {
            CheckNodeOrder();
            throw new NotImplementedException();
        }

        public override Matrix GetLambdaMatrix()
        {
            CheckNodeOrder();
            
            return Matrix.Eye(3);
        }

        public override double[] IsoCoordsToLocalCoords(params double[] isoCoords)
        {
            CheckNodeOrder();
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IElementHelper[] GetHelpers()
        {
            return new IElementHelper[] {new TetrahedronHelper()};
        }

        #endregion
    }
}
