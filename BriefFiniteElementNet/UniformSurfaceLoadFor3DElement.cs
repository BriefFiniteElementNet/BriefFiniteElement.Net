using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a uniform load which is applying to one side of a 3d element
    /// </summary>
    [Serializable]
    public class UniformSurfaceLoadFor3DElement : Load3D
    {
        /// <inheritdoc />
        public override Force[] GetGlobalEquivalentNodalLoads(Element element)
        {
            switch (element.ElementType)
            {
                case ElementType.Undefined:
                    break;
                case ElementType.FrameElement2Node:
                    break;
                case ElementType.TrussElement2Noded:
                    break;
                case ElementType.TetrahedralIso:
                    return GetGlobalEquivalentNodalLoads((Tetrahedral)element);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the global equivalent nodal loads for <see cref="Tetrahedral"/> element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public Force[] GetGlobalEquivalentNodalLoads(Tetrahedral element)
        {
            //p.264 of Structural Analysis with the Finite Element Method Linear Statics, ISBN: 978-1-4020-8733-2
            //formula (8.39) - (8.42) : Uniform surface tractions are distributed in equal parts between the three nodes of the linear tetrahedron face affected by the loading.

            //area of triangle in 3D: S = | AB x AC |/2
            //using cross product of two lines of three lines of triangle

            var p1 = _surfaceNodes[0].Location;
            var p2 = _surfaceNodes[1].Location;
            var p3 = _surfaceNodes[2].Location;

            var v1 = p2 - p1;
            var v2 = p3 - p1;

            var s = v1.Cross(v2).Length/2;

            var f = new Force {Fx = this.Sx*s/3, Fy = this.Sy*s/3, Fz = this.Sz*s/3};


            return new[] {f, f, f};
        }

        [NonSerialized]
        private Node[] _surfaceNodes;

        /// <summary>
        /// used for referencing nodes after de serialization
        /// </summary>
        
        internal int[] nodeNumbers;

        /// <summary>
        /// Gets or sets an array of nodes which are formed target surface.
        /// </summary>
        /// <value>
        /// The surface nodes.
        /// </value>
        public Node[] SurfaceNodes
        {
            get { return _surfaceNodes; }
            set { _surfaceNodes = value; }
        }

        private double _sx, _sy, _sz;

        /// <summary>
        /// Gets or sets the X component of this instance.
        /// </summary>
        /// <value>
        /// The X component of distributed surface load in N/m^2 dimension.
        /// This is in global coordination system not element's local coordination system.
        /// </value>
        public double Sx
        {
            get { return _sx; }
            set { _sx = value; }
        }

        /// <summary>
        /// Gets or sets the Y component of this instance.
        /// </summary>
        /// <value>
        /// The Y component of distributed surface load in N/m^2 dimension.
        /// This is in global coordination system not element's local coordination system.
        /// </value>
        public double Sy
        {
            get { return _sy; }
            set { _sy = value; }
        }

        /// <summary>
        /// Gets or sets the Z component of this load.
        /// </summary>
        /// <value>
        /// The Z component of distributed surface load in N/m^2 dimension.
        /// This is in global coordination system not element's local coordination system.
        /// </value>
        public double Sz
        {
            get { return _sz; }
            set { _sz = value; }
        }



      

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.nodeNumbers = new int[this._surfaceNodes.Length];

            for (var i = 0; i < _surfaceNodes.Length; i++)
                nodeNumbers[i] = _surfaceNodes[i].Index;

            info.AddValue("nodeNumbers", nodeNumbers);

            info.AddValue("_sx", _sx);
            info.AddValue("_sy", _sy);
            info.AddValue("_sz", _sz);


            base.GetObjectData(info, context);
        }        

         /// <inheritdoc/>
         protected UniformSurfaceLoadFor3DElement(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _sx = info.GetDouble("_sx");
            _sy = info.GetDouble("_sy");
            _sz = info.GetDouble("_sz");

             this.nodeNumbers = (int[]) info.GetValue("nodeNumbers", typeof(int[]));

             //NOTE: reassigning the references of _surfaceNodes elements is done in Model.ReassignNodeReferences.
             //this is exception ...
        }


    }
}
