using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet
{
    [Serializable]
    public abstract class Element2D : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Element2D"/> class.
        /// </summary>
        /// <param name="nodes">The number of nodes that the <see cref="Element" /> connect together.</param>
        protected Element2D(int nodes) : base(nodes)
        {
        }

        #region Serialization stuff

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public override Matrix GetLambdaMatrix()
        {
            throw new NotImplementedException();
        }

        public override IElementHelper[] GetHelpers()
        {
            throw new NotImplementedException();
        }

        protected Element2D(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

    }
}
