using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    public struct FlatShellStressTensor
    {
        public MembraneStressTensor MembraneTensor;

        public PlateBendingStressTensor BendingTensor;
    }
}
