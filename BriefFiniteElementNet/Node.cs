using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using BriefFiniteElementNet.Elements;

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
            settlements = new List<Settlement>();

            //memberLoads = new List<NodalLoad>();
        }

        public Node(Point location):this()
        {
            this.location = location;
        }

        /// <summary>
        /// Initiates a new node
        /// </summary>
        /// <param name="x">X component of node's location</param>
        /// <param name="y">Y component of node's location</param>
        /// <param name="z">Z component of node's location</param>
        public Node(double x, double y, double z) : this(new Point(x, y, z))
        {}

        /// <summary>
        /// Gets the index of node
        /// </summary>
        public int Index
        {
            get { return _index; }
            internal set { _index = value; }
        }

        [NonSerialized]
        private int _index;

        
        /// <summary>
        /// The connected elements, used for calculating 
        /// </summary>
        [NonSerialized][Obsolete]
        internal List<Element> ConnectedElements = new List<Element>();
        private List<NodalLoad> loads;
        private List<Settlement> settlements;

        private Point location;
        //private Displacement settlements_old;
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
        /// gets the settlements
        /// </summary>
        public List<Settlement> Settlements
        {
            get { return settlements; }
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

        /*
        /// <summary>
        /// Gets or sets the settlements.
        /// </summary>
        /// <value>
        /// The settlements (initial displacements) of the Node in 3D (including both movement and rotation).
        /// </value>
        [Obsolete("use Settlementss instead")]
        public Displacement Settlements_old
        {
            get { return settlements_old; }
            set { settlements_old = value; }
        }
        */

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

        /*
        /// <summary>
        /// Gets or sets the member loads.
        /// </summary>
        /// <value>
        /// The concentrated loads who come from distributed loads from members.
        /// </value>
        /// <remarks>
        /// For creating forces load, distributed loads should convert to nodal load, the <see cref="MembersLoads"/> is the nodal loads resulting from <see cref="Load"/>s applied to members.
        /// These loads are in global coordination system</remarks>
        [Obsolete("See comments in StaticLinearAnalysisResult.GetTotalForceVector()")]
        internal List<NodalLoad> MembersLoads
        {
            get { return memberLoads; }
            private set { memberLoads = value; }
        }
        */

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
        /// Gets the nodal displacement regarding defined load case
        /// </summary>
        /// <param name="cse">load case</param>
        /// <returns></returns>
        public Displacement GetNodalDisplacement(LoadCase cse)
        {
            parent.LastResult.AddAnalysisResultIfNotExists(cse);
            var disps = parent.LastResult.Displacements[cse];

            var buf = Displacement.FromVector(disps, this.Index * 6);

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
        public Force GetSupportReaction(LoadCombination cmb)
        {
            var buf = new Force();

            foreach (var kv in cmb)
            {
                buf += kv.Value*GetSupportReaction(kv.Key);
            }

            return buf;
        }



        /// <summary>
        /// Gets the supports reaction that are from load combination of <see cref="cmb"/>, which are applying to this <see cref="Node"/> from supports.
        /// </summary>
        /// <param name="cmb">The CMB.</param>
        /// <returns></returns>
        [Obsolete("Use GetSupportReaction")]
        internal Force GetSupportReaction2(LoadCombination cmb)
        {
            //TODO: this methods not works correctly!

            var f1 = new Force();
            var f = new Force();

            foreach (var cse in cmb.Keys)
                parent.LastResult.AddAnalysisResultIfNotExists(cse);

            

            #region From Connected Elements

            foreach (var elm in ConnectedElements)
            {
                var ind = elm.Nodes.IndexOfReference(this);

                foreach (var cse in cmb.Keys)
                {
                    foreach (var lde in elm.Loads)
                    {
                        if (lde.Case != cse)
                            continue;

                        //elm.GetGlobalEquivalentNodalLoads
                        var loads = elm.GetGlobalEquivalentNodalLoads(lde);

                        f1 += cmb[cse] * loads[ind];
                    }
                }
            }

            #endregion

            #region From Loads on this node

            foreach (var load in this.loads)
            {
                if (!cmb.ContainsKey(load.Case))
                    continue;

                f1 += cmb[load.Case] * load.Force;
            }

            #endregion


            foreach (var cse in cmb.Keys)//
            {
                f += cmb[cse] * Force.FromVector(parent.LastResult.Forces[cse], 6 * this.Index);
            }

            var buf = f + -f1;


            if (constraints.DX == DofConstraint.Released) buf.Fx = 0;
            if (constraints.DY == DofConstraint.Released) buf.Fy = 0;
            if (constraints.DZ == DofConstraint.Released) buf.Fz = 0;

            if (constraints.RX == DofConstraint.Released) buf.Mx = 0;
            if (constraints.RY == DofConstraint.Released) buf.My = 0;
            if (constraints.RZ == DofConstraint.Released) buf.Mz = 0;


            return buf;
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets the supports reaction that is from load cse which are applying to this <see cref="Node"/> from supports.
        /// </summary>
        /// <param name="cse">The loadCase.</param>
        /// <returns></returns>
        public Force GetSupportReaction(LoadCase cse)
        {
            if (constraints == Constraint.Released)
                return new Force();


            parent.LastResult.AddAnalysisResultIfNotExists(cse);

            var cs = Force.FromVector(parent.LastResult.ConcentratedForces[cse], 6 * Index);
            var es = Force.FromVector(parent.LastResult.ElementForces[cse], 6 * Index);
            var ss = Force.FromVector(parent.LastResult.SupportReactions[cse], 6 * Index);

            var buf = ss - cs - es;

            if (constraints.DX == DofConstraint.Released)
                buf.Fx = 0;
            if (constraints.DY == DofConstraint.Released)
                buf.Fy = 0;
            if (constraints.DZ == DofConstraint.Released)
                buf.Fz = 0;


            if (constraints.RX == DofConstraint.Released)
                buf.Mx = 0;
            if (constraints.RY == DofConstraint.Released)
                buf.My = 0;
            if (constraints.RZ == DofConstraint.Released)
                buf.Mz = 0;
            
            return buf;
        }

       

        /// <summary>
        /// Gets the supports reaction that are from loads with <see cref="LoadCase.DefaultLoadCase"/> (can be said default loads), which are applying to this <see cref="Node"/> from supports.
        /// </summary>
        /// <returns></returns>
        public Force GetSupportReaction()
        {
            return GetSupportReaction(LoadCombination.DefaultLoadCombination);
        }

        /// <summary>
        /// Gets the total external force.
        /// </summary>
        /// <param name="combination">The combination.</param>
        /// <remarks>
        /// This will return all loads which are applying to this node from anywhere other than connected elements!
        /// </remarks>
        /// <returns></returns>
        [Obsolete]
        public Force GetTotalExternalForce(LoadCombination combination)
        {
            var buf = new Force();

            foreach (var kv in combination)
            {
                var cse = kv.Key;

                if (!parent.LastResult.Forces.ContainsKey(cse))
                    parent.LastResult.AddAnalysisResultIfNotExists(cse);

                var totForceVector = parent.LastResult.Forces[cse];

                var fc = Force.FromVector(totForceVector, this.Index*6);

                buf += fc*combination[cse];
            }

            return buf;
        }

        /// <summary>
        /// Gets the sum of external forces (including support reaction + exnternal nodal loads + Equivalent nodal loads of elemental loads)
        /// </summary>
        /// <param name="loadCase">the load case</param>
        /// <returns></returns>
        public Force GetTotalExternalForces(LoadCase loadCase)
        {
            parent.LastResult.AddAnalysisResultIfNotExists(loadCase);

            var cs = Force.FromVector(parent.LastResult.ConcentratedForces[loadCase], 6 * Index);
            var es = Force.FromVector(parent.LastResult.ElementForces[loadCase], 6 * Index);
            var ss = Force.FromVector(parent.LastResult.SupportReactions[loadCase], 6 * Index);

            var buf = ss - cs - es;

            return buf;
        }

        /// Gets the sum of external applying forces (including exnternal nodal loads + Equivalent nodal loads of elemental loads)
        /// </summary>
        /// <param name="loadCase">the load case</param>
        /// <returns></returns>
        public Force GetTotalApplyingForces(LoadCase loadCase)
        {
            parent.LastResult.AddAnalysisResultIfNotExists(loadCase);

            var cs = Force.FromVector(parent.LastResult.ConcentratedForces[loadCase], 6 * Index);
            var es = Force.FromVector(parent.LastResult.ElementForces[loadCase], 6 * Index);

            var buf = -cs - es;

            return buf;
        }

        /// <summary>
        /// Gets the sum of external forces (including exnternal nodal loads)
        /// </summary>
        /// <param name="loadCombination">the load combintation</param>
        /// <returns></returns>
        [Obsolete]

        public Force GetTotalApplyingForces(LoadCombination loadCombination)
        {
            var buf = new Force();

            foreach (var tuple in loadCombination)
            {
                buf += GetTotalApplyingForces(tuple.Key) * tuple.Value;
            }

            return buf;
        }

        /// <summary>
        /// Gets the sum of external forces (including support reaction and exnternal nodal loads)
        /// </summary>
        /// <param name="loadCombination">the load combintation</param>
        /// <returns></returns>
        [Obsolete]
        public Force GetTotalExternalForces(LoadCombination loadCombination)
        {
            var buf = new Force();

            foreach(var tuple in loadCombination)
            {
                buf += GetTotalExternalForces(tuple.Key) * tuple.Value;
            }

            return buf;
        }

        /// <summary>
        /// Gets the total displacements with specified <see cref="loadCase"/>
        /// </summary>
        /// <param name="loadCase">the load case</param>
        /// <returns></returns>
        public Displacement GetTotalSettlementAmount(LoadCase loadCase)
        {
            var buf = new Displacement();

            foreach (var st in settlements)
                if (st.LoadCase == loadCase)
                    buf += st.Displacement;

            return buf;
        }

        /// <summary>
        /// Gets the total displacements with specified <see cref="loadCombination"/>
        /// </summary>
        /// <param name="loadCase">the load case</param>
        /// <returns></returns>
        public Displacement GetTotalSettlementAmount(LoadCombination loadCombination)
        {
            var buf = new Displacement();

            foreach (var st in settlements)
                if (loadCombination.ContainsKey(st.LoadCase))
                    buf += loadCombination[st.LoadCase] * st.Displacement;

            return buf;
        }

        #region Serialization stuff

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("location", location);
            info.AddValue("Index", Index);
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
            Index = (int) info.GetValue("Index", typeof (int));
            loads = (List<NodalLoad>) info.GetValue("loads", typeof (List<NodalLoad>));
            settlements = info.GetValue<List<Settlement>>("settlements");
            constraints = (Constraint) info.GetValue("constraints", typeof (Constraint));
        }

        #endregion

        
    }
}
