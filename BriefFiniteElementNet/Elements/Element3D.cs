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
    public abstract class Element3D : Element
    {
        protected Element3D(int nodes) : base(nodes)
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

        protected Element3D(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
