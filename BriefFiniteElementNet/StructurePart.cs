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
using System.Globalization;

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
        protected internal int idHashCode;
        protected internal Guid id;
        protected internal string label;
        protected internal string tag;
        [NonSerialized]
        protected Model parent;


        private bool isChanged;

        /// <summary>
        /// Determines wether this structural part is changed since last time.
        /// </summary>
        /// <remarks>Every time anything changes on this structural part that possibly can take effect into total stiffness matrix, then this field should set to true</remarks>
        public bool IsChanged
        {
            get { return isChanged; }
            set
            {
                isChanged = value;
            }
        }



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
                                throw new InvalidLabelException(string.Format(CultureInfo.CurrentCulture, "member with same label ({0}) exist in the structure", value));

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

        [Obsolete]
        public XmlSchema GetSchema()
        {
            return null;
        }

        [Obsolete]
        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        [Obsolete]
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
        /// Re assign s the node references. Used after deserialization
        /// </summary>
        /// <param name="parent">The parent.</param>
        public virtual void ReAssignNodeReferences(Model parent)
        {

        }
    }
}
