using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Diagnostics;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet
{

    /// <summary>
    /// Represents collection of <see cref="Element" />s.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class ElementCollection : IList<Element>,ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementCollection" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        internal ElementCollection(Model parent)
        {
            this.parent = parent;
        }

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
        /// Ensures the node label validity. If not satisfies, throws exception.
        /// </summary>
        /// <param name="elm">The node.</param>
        /// <exception cref="System.InvalidOperationException">This member is child of another Model
        /// or
        /// This member is already included in this Model</exception>
        private void EnsureNewElementLabelValidity(Element elm)
        {
            if (elm.Parent != null)
            {
                if (!ReferenceEquals(this.parent, elm.Parent))
                    throw new InvalidOperationException("This member is child of another Model");
                else
                    throw new InvalidOperationException("This member is already included in this Model");
            }

            if (elm.Label != null)
            {
                if (!parent.IsValidLabel(elm.Label))
                    ExceptionHelper.ThrowMemberWithSameLabelExistsException(elm.Label);
            }
        }


        /// <summary>
        /// The _list
        /// </summary>
        private readonly IList<Element> _list = new List<Element>();

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Element> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<Element>

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public void Add(Element item)
        {
            EnsureNewElementLabelValidity(item);
            _list.Add(item);
            item.Parent = this.parent;
        }

        /// <summary>
        /// Adds the specified elements to this collection.
        /// </summary>
        /// <param name="elements">The elements.</param>
        public void Add(params Element[] elements)
        {
            foreach (var elm in elements)
            {
                Add(elm);
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            foreach (var node in _list)
            {
                node.Parent = null;
            }

            _list.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        /// </returns>
        public bool Contains(Element item)
        {
            return _list.Contains(item);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(Element[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        public bool Remove(Element item)
        {
            if (_list.Remove(item))
            {
                item.Parent = null;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return _list.IsReadOnly; }
        }

        #endregion

        #region Implementation of IList<Element>

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <returns>
        /// The index of <paramref name="item" /> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(Element item)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        public void Insert(int index, Element item)
        {
            EnsureNewElementLabelValidity(item);
            _list.Insert(index, item);
            item.Parent = this.parent;
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            var itm = _list[index];
            itm.Parent = null;
            _list.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public Element this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        #endregion

        #region Your Added Stuff

        /// <summary>
        /// Gets the <see cref="Element" /> with the specified label.
        /// </summary>
        /// <value>
        /// The first <see cref="Element" /> with specifid <see cref="label" />.
        /// </value>
        /// <param name="label">The label.</param>
        /// <returns>
        /// The first <see cref="Element" /> with specifid <see cref="label" />
        /// </returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If <see cref="Element" /> with <see cref="label" /> not exists</exception>
        public Element this[string label]
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
        /// Gets the <see cref="Element" /> with the specified identifier.
        /// </summary>
        /// <value>
        /// The <see cref="Element" />.
        /// </value>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">id</exception>
        public Element this[Guid id]
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
        /// This is constructor for deserialization. Satisfies the rule CA2229.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The source (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        protected ElementCollection(SerializationInfo info, StreamingContext context)
        {
            _list = (List<Element>) info.GetValue("_list", typeof (List<Element>));
        }


        #endregion
    }
}