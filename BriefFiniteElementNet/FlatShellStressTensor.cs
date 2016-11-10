using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    public struct FlatShellStressTensor
    {
        public FlatShellStressTensor(MembraneStressTensor membraneTensor, PlateBendingStressTensor bendingTensor)
        {
            MembraneTensor = membraneTensor;
            BendingTensor = bendingTensor;
        }

        public MembraneStressTensor MembraneTensor;

        public PlateBendingStressTensor BendingTensor;

        public static FlatShellStressTensor operator +(FlatShellStressTensor left, FlatShellStressTensor right)
        {
            var buf = new FlatShellStressTensor
            {
                BendingTensor = left.BendingTensor + right.BendingTensor,
                MembraneTensor = left.MembraneTensor + right.MembraneTensor
            };


            return buf;
        }

        public static FlatShellStressTensor Multiply(FlatShellStressTensor left, double coef)
        {
            var buf = new FlatShellStressTensor
            {
                BendingTensor = PlateBendingStressTensor.Multiply(left.BendingTensor, coef),
                MembraneTensor = MembraneStressTensor.Multiply(left.MembraneTensor, coef)
            };

            return buf;
        }

    }
}
