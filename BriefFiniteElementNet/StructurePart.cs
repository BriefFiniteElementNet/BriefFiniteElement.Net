using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents an abstract class for being used by Node and Element classes
    /// </summary>
    [DebuggerDisplay("Label: {Label}")]
    [Serializable]
    public abstract class StructurePart:ISerializable,IXmlSerializable
    {
        /// <summary>
        /// Represents the hash code of ID, used for better performance (probably!)
        /// Anytime id is changed, it should change accordingly to hash code of new value
        /// </summary>
        protected int idHashCode;
        protected Guid id;
        protected string label;
        protected string tag;
        [NonSerialized]
        protected Model parent;

        /// <summary>
        /// The temporal data, to store temporal or calculation sensitive data,
        /// This field is readonly and is not serialized with the containing object.
        /// </summary>
        [NonSerialized]
        protected readonly Dictionary<Array, object> TemporalData = new Dictionary<Array, object>(new ArrayEqualityComparer());

        

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id
        {
            get { return id; }
        }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        /// <exception cref="BriefFiniteElementNet.InvalidLabelException">member with same label exist in the structure</exception>
        /// <exception cref="System.NotImplementedException"></exception>
        public string Label
        {
            get { return label; }
            set
            {
                if (label != value)
                    if (value != null)
                        if (parent != null)
                            if (!parent.IsValidLabel(value))
                                throw new InvalidLabelException(string.Format("member with same label ({0}) exist in the structure", value));

                this.label = value;
            }
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public string Tag
        {
            get { return tag; }
            set { tag = value; }
        }

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

        #region ISerializable

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("idHashCode", idHashCode);
            info.AddValue("id", id);
            info.AddValue("label", label);
            info.AddValue("tag", tag);
        }

        /// <summary>
        /// This is constructor for de serialization. Satisfies the rule CA2229.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The source (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        protected StructurePart(SerializationInfo info, StreamingContext context)
        {
			this.tag = (string)info.GetValue("tag", typeof(string));
			this.label = (string)info.GetValue("label", typeof(string));
			this.id = (System.Guid)info.GetValue("id", typeof(System.Guid));
			this.idHashCode = (int)info.GetValue("idHashCode", typeof(int));
            
        }

        #endregion

        #region IXmlSerializable

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteValue("idHashCode", idHashCode);
            writer.WriteValue("id", id);
            writer.WriteValue("label", label);
            writer.WriteValue("tag", tag);
        }

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="StructurePart"/> class.
        /// </summary>
        protected StructurePart()
        {
            
        }

        /// <summary>
        /// Gets the temporal data with specified key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key, sesitive to order.</param>
        /// <returns>true if value exists, false otherwise</returns>
        public bool GetTemporalData<T>(out T value,params object[] key)
        {
            if (!TemporalData.ContainsKey(key))
            {
                value = default(T);
                return false;
            }

            var obj = TemporalData[key];

            value = (T)obj;
            return true;
        }

        /// <summary>
        /// Sets the temporal data with specified key.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="key">The key, sensitive to order.</param>
        public void SetTemporalData(object value, params object[] key)
        {
            TemporalData[key] = value;
        }

        /// <summary>
        /// Re assign s the node references. Used after deserialization
        /// </summary>
        /// <param name="parent">The parent.</param>
        public virtual void ReAssignNodeReferences(Model parent)
        {

        }
    }
}
