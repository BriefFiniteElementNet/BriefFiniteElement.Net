using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    public abstract class Element3D : Element
    {
        protected Element3D(int nodes) : base(nodes)
        {
        }
    }
}
