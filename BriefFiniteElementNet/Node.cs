using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a Node which is interconnection between Finite Elements.
    /// </summary>
    [Serializable]
    public class Node : StructurePart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        
        public Node()
        {
            loads = new List<NodalLoad>();
            memberLoads = new List<NodalLoad>();
        }

        public Node(Point location):this()
        {
            this.location = location;
        }

        public Node(double x, double y, double z) : this(new Point(x, y, z))
        {}

        internal int Index;

        /// <summary>
        /// The connected elements, used for calculating 
        /// </summary>
        internal List<Element> ConnectedElements = new List<Element>(); 
        private List<NodalLoad> loads;
        private List<NodalLoad> memberLoads=new List<NodalLoad>();
        private Point location;
        private Displacement settlements;
        private Constraint constraints;



        /// <summary>
        /// Gets the loads.
        /// </summary>
        /// <value>
        /// The loads.
        /// </value>
        public List<NodalLoad> Loads
        {
            get { return loads; }
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location of <see cref="Node"/> in 3D space.
        /// </value>
        public Point Location
        {
            get { return location; }
            set { location = value; }
        }

        /// <summary>
        /// Gets or sets the settlements.
        /// </summary>
        /// <value>
        /// The settlements (initial displacements) of the Node in 3D (including both movement and rotation).
        /// </value>
        public Displacement Settlements
        {
            get { return settlements; }
            set { settlements = value; }
        }

        /// <summary>
        /// Gets or sets the constraints.
        /// </summary>
        /// <value>
        /// The constraints of node in 3D (including both movement and rotation).
        /// </value>
        public Constraint Constraints
        {
            get { return constraints; }
            set { constraints = value; }
        }

        /// <summary>
        /// Gets or sets the member loads.
        /// </summary>
        /// <value>
        /// The concentrated loads who come from distributed loads from members.
        /// </value>
        /// <remarks>
        /// For creating forces load, distributed loads should convert to nodal load, the <see cref="MembersLoads"/> is the nodal loads resulting from <see cref="Load"/>s applied to members.
        /// These loads are in global coordination system</remarks>
        internal List<NodalLoad> MembersLoads
        {
            get { return memberLoads; }
            private set { memberLoads = value; }
        }


        /// <summary>
        /// Gets the nodal displacement regarding specified <see cref="LoadCombination"/> <see cref="cmb"/>.
        /// </summary>
        /// <param name="cmb">The Load Combination.</param>
        /// <returns></returns>
        public Displacement GetNodalDisplacement(LoadCombination cmb)
        {
            var buf = new Displacement();

            foreach (var pair in cmb)
            {
                var cf = pair.Value;

                parent.LastResult.AddAnalysisResultIfNotExists(pair.Key);

                var disps = parent.LastResult.Displacements[pair.Key];

                buf += cf*Displacement.FromVector(disps, this.Index*6);
            }

            return buf;
        }

        /// <summary>
        /// Gets the nodal displacement regarding Default load case (default load case means a load case where <see cref="LoadCase.LoadType" /> is equal to <see cref="LoadType.Default" /> and <see cref="LoadCase.CaseName" /> is equal to null)
        /// </summary>
        /// <returns></returns>
        public Displacement GetNodalDisplacement()
        {
            var cmb = new LoadCombination();
            cmb[new LoadCase()] = 1;
            return GetNodalDisplacement(cmb);
        }

        /// <summary>
        /// Gets the supports reaction that are from load combination of <see cref="cmb"/>, which are applying to this <see cref="Node"/> from supports.
        /// </summary>
        /// <param name="cmb">The CMB.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Force GetSupportReaction(LoadCombination cmb)
        {
            var f1 = new Force();
            var f = new Force();

            foreach (var cse in cmb.Keys)
                parent.LastResult.AddAnalysisResultIfNotExists(cse);


            foreach (var elm in ConnectedElements)
            {
                var ind = elm.Nodes.IndexOfReference(this);

                foreach (var cse in cmb.Keys)
                {
                    foreach (var lde in elm.Loads)
                    {
                        if (lde.Case != cse)
                            continue;

                        var loads = lde.GetGlobalEquivalentNodalLoads(elm);

                        f1 += cmb[cse]*loads[ind];
                    }


                    f += cmb[cse] * Force.FromVector(parent.LastResult.Forces[cse], 6 * this.Index);
                }
                
            }

            var buf = f + -f1;


            if (constraints.Dx == DofConstraint.Released) buf.Fx = 0;
            if (constraints.Dy == DofConstraint.Released) buf.Fy = 0;
            if (constraints.Dz == DofConstraint.Released) buf.Fz = 0;

            if (constraints.Rx == DofConstraint.Released) buf.Mx = 0;
            if (constraints.Ry == DofConstraint.Released) buf.My = 0;
            if (constraints.Rz == DofConstraint.Released) buf.Mz = 0;


            return buf;
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the supports reaction that are from loads with <see cref="LoadCase.DefaultLoadCase"/> (can be said default loads), which are applying to this <see cref="Node"/> from supports.
        /// </summary>
        /// <returns></returns>
        public Force GetSupportReaction()
        {
            return GetSupportReaction(LoadCombination.DefaultLoadCombination);
        }

        #region Serialization stuff

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("location", location);
            info.AddValue("index", Index);
            info.AddValue("loads", loads);
            info.AddValue("settlements", settlements);
            info.AddValue("constraints", constraints);
        }

        /// <summary>
        /// Constructor of <see cref="Node"/> for satisfying the <class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected Node(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            location = (Point) info.GetValue("location", typeof (Point));
            Index = (int) info.GetValue("index", typeof (int));
            loads = (List<NodalLoad>) info.GetValue("loads", typeof (List<NodalLoad>));
            settlements = (Displacement) info.GetValue("settlements", typeof (Displacement));
            constraints = (Constraint) info.GetValue("constraints", typeof (Constraint));
        }

        #endregion

    }
}
