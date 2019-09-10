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



        
    }
}
