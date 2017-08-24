using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CSparse.Double;

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

        public Node Node1;
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
    }
}
