using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Diagnostics;

namespace BriefFiniteElementNet
{

    /// <summary>
    /// Represents a collection of <see cref="Element"/>s.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class ElementCollection : IList<Element>,ISerializable
    {
        public ElementCollection(Model parent)
        {
            this.parent = parent;
        }

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
        /// Ensures the node label validity. If not satiffies, throws exception.
        /// </summary>
        /// <param name="elm">The node.</param>
        /// <exception cref="System.InvalidOperationException">
        /// This member is child of another Model
        /// or
        /// This member is already included in this Model
        /// </exception>
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


        private readonly IList<Element> _list = new List<Element>();

        #region Implementation of IEnumerable

        public IEnumerator<Element> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<Element>

        public void Add(Element item)
        {
            EnsureNewElementLabelValidity(item);
            _list.Add(item);
            item.Parent = this.parent;
        }

        public void AddRange(params Element[] elements)
        {
            foreach (var elm in elements)
            {
                Add(elm);
            }
        }

        public void Clear()
        {
            foreach (var node in _list)
            {
                node.Parent = null;
            }

            _list.Clear();
        }

        public bool Contains(Element item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(Element[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(Element item)
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

        #region Implementation of IList<Element>

        public int IndexOf(Element item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, Element item)
        {
            EnsureNewElementLabelValidity(item);
            _list.Insert(index, item);
            item.Parent = this.parent;
        }

        public void RemoveAt(int index)
        {
            var itm = _list[index];
            itm.Parent = null;
            _list.RemoveAt(index);
        }

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
        /// The first <see cref="Element" /> with specifid <see cref="label"/>.
        /// </value>
        /// <param name="label">The label.</param>
        /// <returns>The first <see cref="Element" /> with specifid <see cref="label" /> </returns>
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
        /// Gets the <see cref="Element"/> with the specified identifier.
        /// </summary>
        /// <value>
        /// The <see cref="Element"/>.
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
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_list", _list);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementCollection"/> class.
        /// Satisfies rule: ImplementSerializationConstructors. 
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected ElementCollection(SerializationInfo info, StreamingContext context)
        {
            _list = (List<Element>) info.GetValue("_list", typeof (List<Element>));
        }

        #endregion
    }
}