using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Diagnostics;
using BriefFiniteElementNet.Elements;
using __targetType__ = BriefFiniteElementNet.Elements.MpcElement;

namespace BriefFiniteElementNet
{
    // <summary>
    /// Represents collection of <see cref="MpcElement" />s.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class MpcElementCollection : StructuralPartCollection<MpcElement>
    {
        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
base.GetObjectData(info,context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BriefFiniteElementNet.MpcElementCollection" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        internal MpcElementCollection(Model parent):base(parent)
        {
        }

        protected MpcElementCollection(SerializationInfo info, StreamingContext context):base(info,context)
        {
        }


    }
}
