using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CSparse.Double;
using System.Security.Permissions;

namespace BriefFiniteElementNet.Elements
{
    /// <summary>
    /// Represents a hinge link between two nodes.
    /// For more info see HingeLink.md
    /// </summary>
    [Serializable]
    [Obsolete("use spring1d at the moment")]
    public class HingeLink: MpcElement
    {
        protected HingeLink(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public HingeLink()
        {
        }

        [NonSerialized]
        public Node Node1;
        [NonSerialized]
        public Node Node2;

        public override CompressedColumnStorage GetExtraEquations()
        {
            throw new NotImplementedException();
        }

        public override int GetExtraEquationsCount()
        {
            throw new NotImplementedException();
        }

        #region ISerialization Implementation



        #endregion

        #region ISerialization Implementation

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Node1", Node1);
            info.AddValue("Node2", Node2);
        }

        #endregion
    }
}
