using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CSparse.Double;
using System.Security.Permissions;

namespace BriefFiniteElementNet.MpcElements
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

        public override SparseMatrix GetExtraEquations()
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
        }

        #endregion
    }
}
