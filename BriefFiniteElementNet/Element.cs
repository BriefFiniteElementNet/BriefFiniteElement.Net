using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents an abstract class for a 'Finite Element' with physical properties.
    /// </summary>
    [DebuggerDisplay("{Label}")]
    public abstract class Element : StructurePart
    {
        protected ElementType elementType;

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <value>
        /// The type of the element.
        /// </value>
        public ElementType ElementType
        {
            get { return elementType; }
            private set { elementType = value; }
        }



        protected List<Load> loads;

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

        protected Node[] nodes;

        /// <summary>
        /// Gets the stifness matrix of member in global coordination system.
        /// </summary>
        /// <returns>The stiffnes matrix</returns>
        /// <remarks>
        /// The number of DoFs is in element local regrading order in <see cref="Nodes"/>!</remarks>
        public abstract Matrix GetGlobalStifnessMatrix();

        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        /// <param name="nodes">The number of nodes that the <see cref="Element"/> connect together.</param>
        protected Element(int nodes)
        {
            this.nodes = new Node[nodes];
            this.loads = new List<Load>();
        }
    }
}
