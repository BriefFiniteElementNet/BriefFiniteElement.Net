using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;
using BriefFiniteElementNet.ElementHelpers;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents an abstract class for a 'Finite Element' with physical properties.
    /// </summary>
    [Serializable]
    [CLSCompliant(true)]
    [DebuggerDisplay("{this.GetType().Name}, Label: {Label}")]
    public abstract class Element : StructurePart
    {
        [NonSerialized]
        internal int Index;

        [Obsolete]
        protected ElementType elementType;

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <value>
        /// The type of the element.
        /// </value>
        /// <remarks>Obsolete because logically prevent us to make an element outside this library</remarks>
        [Obsolete]
        public ElementType ElementType
        {
            get { return elementType; }
            private set { elementType = value; }
        }



        protected List<Load> loads = new List<Load>();

        /// <summary>
        /// Gets or sets the loads.
        /// </summary>
        /// <value>
        /// The loads.
        /// </value>
        public List<Load> Loads
        {
            get { return loads; }
        }

        /// <summary>
        /// Gets the nodes.
        /// </summary>
        /// <value>
        /// The nodes.
        /// </value>
        public Node[] Nodes
        {
            get { return nodes; }
            private set { nodes = value; }
        }

        [NonSerialized]
        protected Node[] nodes;

        internal int[] nodeNumbers;


        /// <summary>
        /// Gets the stiffness matrix of member in global coordination system.
        /// </summary>
        /// <returns>The stiffness matrix</returns>
        /// <remarks>
        /// The number of DoFs is in element local regrading order in <see cref="Element.Nodes"/>!</remarks>
        public abstract Matrix GetGlobalStifnessMatrix();


        /// <summary>
        /// Gets the mass matrix of element in global coordination system.
        /// </summary>
        /// <returns>The mass matrix</returns>
        /// <remarks>
        /// The number of DoFs is in element local regrading order in <see cref="Element.Nodes"/>!</remarks>
        public abstract Matrix GetGlobalMassMatrix();


        /// <summary>
        /// Gets the damping matrix in global coordination system.
        /// </summary>
        /// <returns>the damping matrix</returns>
        public abstract Matrix GetGlobalDampingMatrix();        


        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        /// <param name="nodes">The number of nodes that the <see cref="Element"/> connect together.</param>
        protected Element(int nodes)
        {
            this.nodes = new Node[nodes];
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        protected Element()
        {
        }



        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.nodeNumbers = new int[this.nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
                nodeNumbers[i] = nodes[i].Index;

            info.AddValue("elementType", (int)elementType);
            info.AddValue("loads", loads);
            info.AddValue("nodeNumbers", nodeNumbers);
            info.AddValue("_massFormulationType", (int) _massFormulationType);

            base.GetObjectData(info, context);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected Element(SerializationInfo info, StreamingContext context):base(info,context)
        {
            nodeNumbers = (int[]) info.GetValue("nodeNumbers", typeof(int[]));

            elementType = (ElementType)info.GetInt32("elementType");
            loads = (List<Load>) info.GetValue("loads", typeof(List<Load>));


            foreach (var pair in info)
            {
                if (pair.Name == "_massFormulationType")
                    _massFormulationType = (MassFormulation) (int) info.GetValue("_massFormulationType", typeof(int));
            }
            

            this.nodes = new Node[nodeNumbers.Length];
        }


        #region MassFormulationType property and field

        private MassFormulation _massFormulationType;

        /// <summary>
        /// Gets or sets the type of the mass formulation.
        /// </summary>
        /// <value>
        /// The type of the mass formulation.
        /// </value>
        public MassFormulation MassFormulationType
        {
            get { return _massFormulationType; }
            set { _massFormulationType = value; }
        }

        #endregion

        /// <summary>
        /// Gets the equivalent nodal loads due to specified <see cref="load"/>.
        /// </summary>
        /// <param name="load">The load.</param>
        /// <returns>Equivalent nodal loads in global coordinate system</returns>
        public abstract Force[] GetEquivalentNodalLoads(Load load);


        /// <summary>
        /// Computes the B matrix (strain-displacement matrix).
        /// </summary>
        /// <param name="location">The location, in local coordination system (local means xi-eta things..., between -1 to 1).</param>
        /// <remarks>B matrix can be in iso parametric coordination system, local coordinate system (linear transform from global) or global coordination system.
        /// This will not be used expect by element itself!
        /// For example B is ∂N / ∂x and is NOT ∂N / ∂ξ
        /// </remarks>
        /// <returns>The B matrix at specified <see cref="location"/></returns>
        [Obsolete]
        public abstract Matrix ComputeBMatrix(params double[] location);

        /// <summary>
        /// Gets the constitutive matrix in local coordination system at specified <see cref="location" />.
        /// </summary>
        /// <param name="location">The location, in local coordination system (local means xi-eta things..., between -1 to 1).</param>
        /// <returns>
        /// The constitutive matrix at specified <see cref="location" />
        /// </returns>
        [Obsolete]
        public abstract Matrix ComputeDMatrixAt(params double[] location);

        /// <summary>
        /// Gets the N matrix (shape function) in local coordination system at specified <see cref="location" />.
        /// </summary>
        /// <param name="location">The location, in local coordination system (local means xi-eta things..., between -1 to 1).</param>
        /// <returns>
        /// The N matrix at specified <see cref="location" />
        /// </returns>
        [Obsolete]
        public abstract Matrix ComputeNMatrixAt(params double[] location);

        /// <summary>
        /// Computes the J matrix at specified <see cref="location"/>.
        /// </summary>
        /// <remarks>
        /// for 1D J is 1x1 matrix:
        /// J =  ∂x / ∂ξ
        /// 
        /// for 2D:
        /// ...
        /// </remarks>
        /// <param name="location">The location.</param>
        /// <returns>the Jacobian matrix</returns>
        [Obsolete]
        public abstract Matrix ComputeJMatrixAt(params double[] location);

        /// <summary>
        /// Gets the lambda matrix of element (for transforming between local and global axis).
        /// For more info see TransformManagerL2G.md file
        /// </summary>
        /// <remarks>
        /// lambda * Local = Global
        /// </remarks>
        /// <returns></returns>
        public abstract Matrix GetLambdaMatrix();

        /// <summary>
        /// Gets the local to global (and vice versa) transformation manager for this element.
        /// </summary>
        /// <returns>the trasformation manager related to this element</returns>
        public TransformManagerL2G GetTransformationManager()
        {
            if (this.lastNodalLocations == null)
                lastNodalLocations = new Point[this.nodes.Length];

            var flag = true;

            for (var i = 0; i < this.nodes.Length; i++)
            {
                if (lastNodalLocations[i] != nodes[i].Location)
                {
                    flag = false;
                    break;
                }
            }

            if (flag)
                return TransformManagerL2G.MakeFromLambdaMatrix(lastLambdaMatrix);
            else
            {
                for (var i = 0; i < this.nodes.Length; i++)
                    lastNodalLocations[i] = nodes[i].Location;

                return TransformManagerL2G.MakeFromLambdaMatrix(lastLambdaMatrix = GetLambdaMatrix());
            }
        }

        /// <summary>
        /// Converts the Isometric coordinates to global location.
        /// </summary>
        /// <param name="isoCoords">The isometric coords in element.</param>
        /// <returns>The location in global system</returns>
        public abstract Point IsoCoordsToGlobalLocation(params double[] isoCoords);


        [NonSerialized]
        private Point[] lastNodalLocations;
        [NonSerialized]
        private Matrix lastLambdaMatrix = Matrix.Eye(3);

        public abstract IElementHelper[] GetHelpers();


        public override void ReAssignNodeReferences(Model parent)
        {
            var elm = this;

            for (int i = 0; i < elm.nodeNumbers.Length; i++)
            {
                var idx = elm.nodeNumbers[i];
                elm.nodes[i] = idx == -1 ? null : parent.Nodes[idx];
            }

            base.ReAssignNodeReferences(parent);
        }



    }
}
