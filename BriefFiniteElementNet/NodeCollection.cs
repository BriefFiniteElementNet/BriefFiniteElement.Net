using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace BriefFiniteElementNet
{

    /// <summary>
    /// Represents a collection of <see cref="Node"/>s.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class NodeCollection : IList<Node>, ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCollection"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public NodeCollection(Model parent)
        {
            this.parent = parent;
        }

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
        /// Ensures the node label validity. If not satisfies, throws exception.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <exception cref="System.InvalidOperationException">
        /// This member is child of another Model
        /// or
        /// This member is already included in this Model
        /// </exception>
        private void EnsureNewNodeLabelValidity(Node node)
        {
            if (node.Parent != null)
            {
                if (!ReferenceEquals(this.parent, node.Parent))
                    throw new InvalidOperationException("This member is child of another Model");
                else
                    throw new InvalidOperationException("This member is already included in this Model");
            }

            if (node.Label != null)
            {
                if (!parent.IsValidLabel(node.Label))
                    ExceptionHelper.ThrowMemberWithSameLabelExistsException(node.Label);
            }
        }


        private readonly IList<Node> _list = new List<Node>();

        #region Implementation of IEnumerable

        public IEnumerator<Node> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<node>

        /// <summary>
        /// Adds the specified node to this collection.
        /// </summary>
        /// <param name="nde">The nde.</param>
        public void Add(Node nde)
        {
            EnsureNewNodeLabelValidity(nde);
            _list.Add(nde);
            nde.Parent = this.parent;
        }

        /// <summary>
        /// Adds the specified nodes to this collection.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public void Add(params Node[] nodes)
        {
            foreach (var nde in nodes)
                Add(nde);
        }

        public void Clear()
        {
            foreach (var node in _list)
            {
                node.Parent = null;
            }

            _list.Clear();
        }

        public bool Contains(Node item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(Node[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(Node item)
        {
            if (_list.Remove(item))
            {
                item.Parent = null;
                return true;
            }

            return false;
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return _list.IsReadOnly; }
        }

       

        #endregion

        #region Implementation of IList<node>

        public int IndexOf(Node item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, Node item)
        {
            EnsureNewNodeLabelValidity(item);
            _list.Insert(index, item);
            item.Parent = this.parent;
        }

        public void RemoveAt(int index)
        {
            var itm = _list[index];
            itm.Parent = null;
            _list.RemoveAt(index);
        }

        public Node this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        #endregion

        #region Your Added Stuff

        /// <summary>
        /// Gets the <see cref="Node" /> with the specified label.
        /// </summary>
        /// <value>
        /// The first <see cref="Node" /> with specifid <see cref="label"/>.
        /// </value>
        /// <param name="label">The label.</param>
        /// <returns>The first <see cref="Node" /> with specifid <see cref="label" /> </returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If <see cref="Node" /> with <see cref="label" /> not exists</exception>
        public Node this[string label]
        {
            get
            {
                var cmp = new FemNetStringCompairer();

                var cleanLabel = FemNetStringCompairer.Clean(label);

                foreach (var elm in _list)
                {
                    if (cmp.Equals(cleanLabel, elm.Label))
                        return elm;
                }

                throw new KeyNotFoundException("label");
            }
        }

        /// <summary>
        /// Gets the <see cref="Node"/> with the specified identifier.
        /// </summary>
        /// <value>
        /// The <see cref="Node"/>.
        /// </value>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">id</exception>
        public Node this[Guid id]
        {
            get
            {
                foreach (var elm in _list)
                {
                    if (elm.Id.Equals(id))
                        return elm;
                }

                throw new KeyNotFoundException("id");
            }
        }

        #endregion

        #region ISerializable Method and Constructor

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_list", _list);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCollection"/> class.
        /// Satisfies rule: ImplementSerializationConstructors. 
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected NodeCollection(SerializationInfo info, StreamingContext context)
        {
            _list = (List<Node>) info.GetValue("_list", typeof (List<Node>));
        }

        #endregion

        public void AddRange(IEnumerable<Node> nodes)
        {
            foreach (var nde in nodes)
            {
                this.Add(nde);
            }
        }
    }
}