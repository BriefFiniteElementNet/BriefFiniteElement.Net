using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a Concentrated load (contains both force and moment in 3d space) which is applying to an <see cref="Element1D"/> body.
    /// </summary>
    [Serializable]
    public class ConcentratedLoad1D : Load1D
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcentratedLoad1D"/> class.
        /// </summary>
        public ConcentratedLoad1D()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcentratedLoad1D"/> class.
        /// </summary>
        /// <param name="force">The force.</param>
        /// <param name="distanseFromStartNode">The distanse from start node.</param>
        /// <param name="coordinationSystem">The coordination system.</param>
        public ConcentratedLoad1D(Force force, double distanseFromStartNode, CoordinationSystem coordinationSystem, LoadCase cse)
        {
            this.force = force;
            this.distanseFromStartNode = distanseFromStartNode;
            this.coordinationSystem = coordinationSystem;
            this.Case = cse;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ConcentratedLoad1D"/> class.
        /// </summary>
        /// <param name="force">The force.</param>
        /// <param name="distanseFromStartNode">The distanse from start node.</param>
        /// <param name="coordinationSystem">The coordination system.</param>
        public ConcentratedLoad1D(Force force, double distanseFromStartNode, CoordinationSystem coordinationSystem)
        {
            this.force = force;
            this.distanseFromStartNode = distanseFromStartNode;
            this.coordinationSystem = coordinationSystem;
        }

        #endregion

        #region Properties

        private Force force;

        /// <summary>
        /// Gets or sets the force.
        /// </summary>
        /// <remarks>Concentrated force consist of force in 3 directions and moments in three dimentions</remarks>
        /// <value>
        /// The concentrated force.
        /// </value>
        public Force Force
        {
            get { return force; }
            set { force = value; }
        }

        /// <summary>
        /// Gets or sets the distanse of concentrated load from start node.
        /// </summary>
        /// <value>
        /// The distanse of the <see cref="ConcentratedLoad1D"/> from start node.
        /// </value>
        public double DistanseFromStartNode
        {
            get { return distanseFromStartNode; }
            set { distanseFromStartNode = value; }
        }


        private double distanseFromStartNode;

        #endregion

        #region Methods

        public override Force GetInternalForceAt(Element1D elm, double x)
        {
            if (elm is FrameElement2Node)
            {
                var e = elm as FrameElement2Node;

                var localForce = this.force;

                if (this.coordinationSystem == CoordinationSystem.Global)
                {
                    var tmp = e.TransformGlobalToLocal(localForce.Forces, localForce.Moments);

                    localForce.Forces = tmp[0];
                    localForce.Moments = tmp[1];
                }

                var l = (e.EndNode.Location - e.StartNode.Location).Length;

                var l1 = distanseFromStartNode;
                var l2 = l - l1;

                var mymy1 = localForce.My*l2/(l*l)*(l2 - 2*l1);
                var myfz1 = 6*localForce.My*l1*l2/(l*l*l);

                var mzmz1 = localForce.Mz*l2/(l*l)*(l2 - 2*l1);
                var mzfy1 = -6*localForce.Mz*l1*l2/(l*l*l);

                var fzmy1 = -localForce.Fz*l1*l2*l2/(l*l);
                var fzfz1 = localForce.Fz*l2*l2/(l*l*l)*(3*l1 + l2);

                var fymz1 = localForce.Fy*l1*l2*l2/(l*l);
                var fyfy1 = localForce.Fy*l2*l2/(l*l*l)*(3*l1 + l2);


                var fxfx1 = localForce.Fx*l2/l;
                var mxmx1 = localForce.Mx*l2/l;

                var f1 = -new Force(
                    fxfx1,
                    mzfy1 + fyfy1,
                    fzfz1 + myfz1,
                    mxmx1,
                    mymy1 + fzmy1,
                    mzmz1 + fymz1
                    );

                if (x < this.distanseFromStartNode)
                {
                    var v1 = new Vector(x, 0, 0);
                    var buf = -f1.Move(v1);

                    return buf;
                }

                else
                {
                    var v1 = new Vector(x, 0, 0);
                    var v2 = new Vector(x - this.distanseFromStartNode, 0, 0);

                    var buf = -f1.Move(v1) - this.force.Move(v2);

                    return buf;
                }
                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }


        public override Force[] GetEquivalentNodalLoads(Element element)
        {
            if (element is FrameElement2Node)
            {
                var e = element as FrameElement2Node;

                var localForce = this.force;

                if (this.coordinationSystem == CoordinationSystem.Global)
                {
                    var tmp = e.TransformGlobalToLocal(localForce.Forces, localForce.Moments);

                    localForce.Forces = tmp[0];
                    localForce.Moments = tmp[1];
                }

                var buf = new Force[2];

                buf[0] = new Force();

                var l = (e.EndNode.Location - e.StartNode.Location).Length;

                var l1 = distanseFromStartNode;
                var l2 = l - l1;

                var mymy1 = localForce.My*l2/(l*l)*(l2 - 2*l1);
                var mymy2 = localForce.My*l1/(l*l)*(l1 - 2*l2);

                var myfz1 = 6*localForce.My*l1*l2/(l*l*l);
                var myfz2 = -myfz1;


                var mzmz1 = localForce.Mz*l2/(l*l)*(l2 - 2*l1);
                var mzmz2 = localForce.Mz*l1/(l*l)*(l1 - 2*l2);

                var mzfy1 = -6*localForce.Mz*l1*l2/(l*l*l);
                var mzfy2 = -mzfy1;


                var fzmy1 = -localForce.Fz*l1*l2*l2/(l*l);
                var fzmy2 = localForce.Fz*l1*l1*l2/(l*l);

                var fzfz1 = localForce.Fz*l2*l2/(l*l*l)*(3*l1 + l2);
                var fzfz2 = localForce.Fz*l1*l1/(l*l*l)*(3*l2 + l1);


                var fymz1 = localForce.Fy*l1*l2*l2/(l*l);
                var fymz2 = -localForce.Fy*l1*l1*l2/(l*l);

                var fyfy1 = localForce.Fy*l2*l2/(l*l*l)*(3*l1 + l2);
                var fyfy2 = localForce.Fy*l1*l1/(l*l*l)*(3*l2 + l1);


                var fxfx1 = localForce.Fx*l2/l;
                var fxfx2 = localForce.Fx*l1/l;

                var mxmx1 = localForce.Mx*l2/l;
                var mxmx2 = localForce.Mx*l1/l;

                var f1 = new Force(
                    fxfx1,
                    mzfy1 + fyfy1,
                    fzfz1 + myfz1,
                    mxmx1,
                    mymy1 + fzmy1,
                    mzmz1 + fymz1
                    );

                var f2 = new Force(
                    fxfx2,
                    mzfy2 + fyfy2,
                    fzfz2 + myfz2,
                    mxmx2,
                    mymy2 + fzmy2,
                    mzmz2 + fymz2
                    );


                var vecs = new Vector[] {f1.Forces, f1.Moments, f2.Forces, f2.Moments};
                vecs = e.TransformLocalToGlobal(vecs);

                buf[0] = new Force(vecs[0], vecs[1]);
                buf[1] = new Force(vecs[2], vecs[3]);

                return buf;
            }

            throw new NotImplementedException();
        }

        #endregion

        #region Serialization stuff and constructor

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("distanseFromStartNode", distanseFromStartNode);
            info.AddValue("force", force);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="UniformLoad1D"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        internal ConcentratedLoad1D(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.distanseFromStartNode = info.GetDouble("distanseFromStartNode");
            this.force = info.GetValue<Force>("force");
        }

        #endregion
    }
}