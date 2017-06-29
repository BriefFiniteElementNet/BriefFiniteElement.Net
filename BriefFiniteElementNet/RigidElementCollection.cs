using System;
using System.Collections.Generic;
using BriefFiniteElementNet.Elements;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a collection for rigid elements
    /// </summary>
    [Serializable]
    public class RigidElementCollection : List<RigidElement>
    {
        /// <summary>
        /// The parent
        /// </summary>
        [NonSerialized]
        private Model parent;

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        internal Model Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementCollection" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        internal RigidElementCollection(Model parent)
        {
            this.parent = parent;
        }
    }

    /// <summary>
    /// Represents a collection for rigid elements
    /// </summary>
    [Serializable]
    public class TelepathyLinkCollection : List<TelepathyLink>
    {
        /// <summary>
        /// The parent
        /// </summary>
        [NonSerialized]
        private Model parent;

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        internal Model Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementCollection" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        internal TelepathyLinkCollection(Model parent)
        {
            this.parent = parent;
        }
    }
}
