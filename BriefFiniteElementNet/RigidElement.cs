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
        [NonSerialized()]
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
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
			
            this.nodeNumbers = new int[this._nodes.Count];

            for (var i = 0; i < _nodes.Count; i++)
                nodeNumbers[i] = _nodes[i] == null ? -1 : _nodes[i].Index;

            info.AddValue("_useForAllLoads", _useForAllLoads);
            info.AddValue("_appliedLoadCases", _appliedLoadCases);
            info.AddValue("_appliedLoadTypes", this._appliedLoadTypes);
            info.AddValue("nodeNumbers", nodeNumbers);

            //info.AddValue("_hingedConnections", _hingedConnections);

            //if (_centralNode != null)
            //    _centralNodeNumber = _centralNode.Index;
            //else
            //    _centralNodeNumber = -1;

            //info.AddValue("_centralNodeNumber", _centralNodeNumber);

            base.GetObjectData(info, context);
        }


        private RigidElement(SerializationInfo info, StreamingContext context) : base(info, context)
        {
			this._appliedLoadTypes = (BriefFiniteElementNet.LoadTypeCollection)info.GetValue("_appliedLoadTypes", typeof(BriefFiniteElementNet.LoadTypeCollection));
            nodeNumbers = (int[]) info.GetValue("nodeNumbers", typeof(int[]));

            _useForAllLoads = info.GetBoolean("_useForAllLoads");
            _appliedLoadCases = (LoadCaseCollection)info.GetValue("_appliedLoadCases",typeof(LoadCaseCollection));

            //_centralNodeNumber = info.GetValue<int>("_centralNodeNumber");
            //_hingedConnections = info.GetBoolean("_hingedConnections");

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

        /// <summary>
        /// The central node number. used for 
        /// </summary>
        [NonSerialized()]
        [Obsolete("Not used in calculation anymore")]
        internal int _centralNodeNumber;

        [Obsolete("Not used in calculation anymore")]
        [NonSerialized] private Node _centralNode;

        /// <summary>
        /// Gets or sets the central node.
        /// </summary>
        /// <value>
        /// The central node, if is set, then it will be used as master node.
        /// </value>
        [Obsolete("Not used in calculation anymore")]
        public Node CentralNode
        {
            get { return _centralNode; }
            set { _centralNode = value; }
        }


        public override void ReAssignNodeReferences(Model parent)
        {
            var relm = this;

            for (int i = 0; i < relm.nodeNumbers.Length; i++)
            {
                var idx = relm.nodeNumbers[i];

                relm.Nodes.Add(idx == -1 ? null : parent.Nodes[idx]);
            }

            {
                //var idx = relm._centralNodeNumber;

                //relm.CentralNode = relm._centralNodeNumber == -1 ? null : parent.Nodes[relm._centralNodeNumber];
            }

            base.ReAssignNodeReferences(parent);
        }
    }
}