using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.StructuralExample.Lib
{
    /// <summary>
    /// Represents a rigid diaphragm
    /// </summary>
    public class RigidDiaphragm
    {
        /// <summary>
        /// The nodes which this rigid diaphragm connect them together
        /// </summary>
        public List<Node> Nodes { get; set; }


        /// <summary>
        /// The coordination of mass center of diaphragm
        /// </summary>
        public Point MassCenter { get; set; }


        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label of the rigid diaphragm.
        /// </value>
        public string Label { get; set; }
    }
}
