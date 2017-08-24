using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;
using CCS = CSparse.Double.CompressedColumnStorage;


namespace BriefFiniteElementNet.Elements
{
    [Serializable]
    [CLSCompliant(true)]
    [DebuggerDisplay("{this.GetType().Name}, Label: {Label}")]
    /// <summary>
    /// Represents an abstract class for a Multi-Point Constraint element.
    /// </summary>
    public abstract class MpcElement: StructurePart
    {
        public bool AppliesForLoadCase(LoadCase lc)
        {
            if (_useForAllLoads)
                return true;

            if (AppliedLoadCases.Contains(lc))
                return true;

            if (_appliedLoadTypes.Contains(lc.LoadType))
                return true;

            return false;
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.nodeNumbers = new int[this._nodes.Count];
            for (int i = 0; i < _nodes.Count; i++)
                nodeNumbers[i] = _nodes[i].Index;

            info.AddValue("nodeNumbers", nodeNumbers);
            info.AddValue("_useForAllLoads", _useForAllLoads);
            info.AddValue("_appliedLoadCases", _appliedLoadCases);

            base.GetObjectData(info, context);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected MpcElement(SerializationInfo info, StreamingContext context):base(info,context)
        {
            nodeNumbers = (int[])info.GetValue("nodeNumbers",typeof(int[]));

            this._nodes = new NodeList();

            _useForAllLoads = (bool)info.GetValue("_useForAllLoads", typeof(bool));
            _appliedLoadCases = (LoadCaseCollection)info.GetValue("_appliedLoadCases", typeof(LoadCaseCollection));

        }


        public MpcElement()
        {
            _nodes = new NodeList();
        }

        protected NodeList _nodes;

        public NodeList Nodes
        {
            get { return _nodes; }
            set { _nodes = value; }
        }


        /// <summary>
        /// Gets the nodes.
        /// </summary>
        /// <value>
        /// The nodes which this <see cref="MpcElement"/> connects them together.
        /// </value>
        /*public NodeList Nodes
        {
            get { return _nodes; }
        }
        */

        private bool _useForAllLoads;


        /// <summary>
        /// Gets or sets a value indicating whether this rigid element should be considered in all <see cref="LoadCase"/>s and <see cref="LoadType"/>s.
        /// </summary>
        /// <value>
        /// <c>true</c> if this MPC element should be used for all load cases; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Some times <see cref="MPC"/> should be considered only in one load case or load type, (like rigid diaphragm which only is used in earthquake load cases) where <see cref="Model"/> have many <see cref="LoadCase"/>s. and other times not!
        /// </remarks>
        public bool UseForAllLoads
        {
            get { return _useForAllLoads; }
            set { _useForAllLoads = value; }
        }


        private LoadCaseCollection _appliedLoadCases = new LoadCaseCollection();



        /// <summary>
        /// Gets the load cases which this <see cref="MpcElement"/> should be considered while analyzing regarding those <see cref="LoadCase"/>s.
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
        /// Gets the load types which this <see cref="MpcElement"/> should be considered while analyzing regarding those <see cref="LoadType"/>s.
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

        public override void ReAssignNodeReferences(Model parent)
        {
            var relm = this;

            for (int i = 0; i < relm.nodeNumbers.Length; i++)
            {
                var idx = relm.nodeNumbers[i];

                relm._nodes.Add(idx == -1 ? null : parent.Nodes[idx]);
            }

            base.ReAssignNodeReferences(parent);
        }

        public abstract CCS GetExtraEquations();

        /// <summary>
        /// Gets the count of equations returned by <see cref="MpcElement.GetExtraEquations()"/>. (number of rows of it)
        /// </summary>
        /// <returns></returns>
        public abstract int GetExtraEquationsCount();
    }
}
