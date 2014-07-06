using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    public abstract class Element2D : Element
    {
        protected Element2D(int nodes) : base(nodes)
        {
        }
    }
}
