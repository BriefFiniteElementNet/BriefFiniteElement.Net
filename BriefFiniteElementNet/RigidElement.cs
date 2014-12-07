using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents an element with rigid body with no relative deformation of <see cref="Nodes"/>
    /// </summary>
    [Serializable]
    public sealed class RigidElement : StructurePart
    {
        private NodeList _nodes;

        /// <summary>
        /// Gets the nodes.
        /// </summary>
        /// <value>
        /// The nodes which this <see cref="RigidElement"/> connects them together.
        /// </value>
        public NodeList Nodes
        {
            get { return _nodes; }
        }

        /*
        /// <summary>
        /// Gets or sets the center of <see cref="RigidElement"/>.
        /// </summary>
        /// <value>
        /// The center of rigid element.
        /// </value>
        /// <remarks>
        /// It is optional, if be set, the value will be used for center else first node in <see cref="Nodes"/> will be used as center.</remarks>
        public Node Center
        {
            get { return _center; }
            set { _center = value; }
        }


        private Node _center;

        */


        private bool _useForAllLoads;


        /// <summary>
        /// Gets or sets a value indicating whether this rigid element should be considered in all <see cref="LoadCase"/>s and <see cref="LoadType"/>s.
        /// </summary>
        /// <value>
        /// <c>true</c> if this rigid element should be used for all load cases; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Some times <see cref="RigidElement"/> should be considered only in one load case or load type, (like rigid diaphragm which only is used in earthquake load cases) where <see cref="Model"/> have many <see cref="LoadCase"/>s. and other times not!
        /// </remarks>
        public bool UseForAllLoads
        {
            get { return _useForAllLoads; }
            set { _useForAllLoads = value; }
        }


        private LoadCaseCollection _appliedLoadCases = new LoadCaseCollection();


        /// <summary>
        /// Gets the load cases which this <see cref="RigidElement"/> should be considered while analyzing regarding those <see cref="LoadCase"/>s.
        /// </summary>
        /// <value>
        /// The applied load cases.
        /// </value>
        public LoadCaseCollection AppliedLoadCases
        {
            get { return _appliedLoadCases; }
        }

        private LoadTypeCollection _appliedLoadTypes = new LoadTypeCollection();

        /// <summary>
        /// Gets the load types which this <see cref="RigidElement"/> should be considered while analyzing regarding those <see cref="LoadType"/>s.
        /// </summary>
        /// <value>
        /// The applied load types.
        /// </value>
        public LoadTypeCollection AppliedLoadTypes
        {
            get { return _appliedLoadTypes; }
            set { _appliedLoadTypes = value; }
        }

        


        internal int[] nodeNumbers;

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.nodeNumbers = new int[this._nodes.Count];

            for (var i = 0; i < _nodes.Count; i++)
                nodeNumbers[i] = _nodes[i].Index;

            info.AddValue("_useForAllLoads", _useForAllLoads);
            info.AddValue("_appliedLoadCases", _appliedLoadCases);
            info.AddValue("nodeNumbers", nodeNumbers);

            base.GetObjectData(info, context);
        }


        private RigidElement(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            nodeNumbers = info.GetValue<int[]>("nodeNumbers");
            _useForAllLoads = info.GetBoolean("_useForAllLoads");
            _appliedLoadCases = info.GetValue<LoadCaseCollection>("_appliedLoadCases");
            this._nodes = new NodeList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RigidElement"/> class.
        /// </summary>
        public RigidElement()
        {
            this._nodes = new NodeList();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RigidElement"/> class.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public RigidElement(params Node[] nodes) : this()
        {
            foreach (var nde in nodes)
            {
                this._nodes.Add(nde);
            }
        }

        private Node _centralNode;

        /// <summary>
        /// Gets or sets the central node.
        /// </summary>
        /// <value>
        /// The central node, if is set, then it will be used as master node.
        /// </value>
        public Node CentralNode
        {
            get { return _centralNode; }
            set { _centralNode = value; }
        }
    }
}