using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents an abstract class for a 'Finite Element' with physical properties.
    /// </summary>
    [Serializable]
    [CLSCompliant(true)]
    [DebuggerDisplay("{ElementType}, Label: {Label}")]
    public abstract class Element : StructurePart
    {
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
        /// Gets the consistent mass matrix of member in global coordination system.
        /// </summary>
        /// <returns>The consistent mass matrix</returns>
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
            nodeNumbers = info.GetValue<int[]>("nodeNumbers");
            elementType = (ElementType)info.GetInt32("elementType");
            loads = info.GetValue<List<Load>>("loads");


            foreach (var pair in info)
            {
                if (pair.Name == "_massFormulationType")
                    _massFormulationType = (MassFormulation) info.GetValue<int>("_massFormulationType");
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
        /// Computes the B matrix.
        /// </summary>
        /// <param name="location">The location, in local coordination system (local means xi-eta things..., between -1 to 1).</param>
        /// <returns>The B matrix at specified <see cref="location"/></returns>
        public abstract Matrix ComputeB(params double[] location);



        /// <summary>
        /// Gets the constitutive matrix in local coordination system at specified <see cref="location" />.
        /// </summary>
        /// <param name="location">The location, in local coordination system (local means xi-eta things..., between -1 to 1).</param>
        /// <returns>
        /// The constitutive matrix at specified <see cref="location" />
        /// </returns>
        public abstract Matrix GetD(params double[] location);
    }
}
