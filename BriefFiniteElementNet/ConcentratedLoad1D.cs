using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BriefFiniteElementNet
{
    public sealed class ConcentratedLoad1D : Load1D
    {
        public override Force[] GetEquivalentNodalLoads(Element element)
        {
            throw new NotImplementedException();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        protected ConcentratedLoad1D(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }


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


        public override Force GetInternalForceAt(Element1D elm, double x)
        {
            throw new NotImplementedException();
        }
    }
}
