using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    public enum LoadType
    {
        Default=0,
        Dead,
        Live,
        Snow,
        Wind,
        Quake,
        Crane,
        Other
    }
}
