using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    public struct FlatShellStressTensor
    {
        public FlatShellStressTensor(CauchyStressTensor membraneTensor, BendingStressTensor bendingTensor)
        {
            MembraneTensor = membraneTensor;
            BendingTensor = bendingTensor;
        }

        public CauchyStressTensor MembraneTensor;

        public BendingStressTensor BendingTensor;

        public static FlatShellStressTensor operator +(FlatShellStressTensor left, FlatShellStressTensor right)
        {
            var buf = new FlatShellStressTensor
            {
                BendingTensor = left.BendingTensor + right.BendingTensor,
                MembraneTensor = left.MembraneTensor + right.MembraneTensor
            };


            return buf;
        }


        public static FlatShellStressTensor operator -(FlatShellStressTensor left, FlatShellStressTensor right)
        {
            var buf = new FlatShellStressTensor
            {
                BendingTensor = left.BendingTensor - right.BendingTensor,
                MembraneTensor = left.MembraneTensor - right.MembraneTensor
            };


            return buf;
        }

        public static FlatShellStressTensor Multiply(double coef, FlatShellStressTensor tensor)
        {
            var buf = new FlatShellStressTensor
            {
                BendingTensor = coef*tensor.BendingTensor,
                MembraneTensor = coef*tensor.MembraneTensor
            };

            return buf;
        }

        public static FlatShellStressTensor Multiply(FlatShellStressTensor tensor,double coef )
        {
            var buf = new FlatShellStressTensor
            {
                BendingTensor = coef * tensor.BendingTensor,
                MembraneTensor = coef * tensor.MembraneTensor
            };

            return buf;
        }

        public static FlatShellStressTensor Transform(FlatShellStressTensor tensor, Matrix transformationMatrix)
        {
            var buf = new FlatShellStressTensor
            {
                MembraneTensor = CauchyStressTensor.Transform(tensor.MembraneTensor, transformationMatrix),
                BendingTensor = BendingStressTensor.Transform(tensor.BendingTensor, transformationMatrix)
            };

            return buf;
        }

        public static FlatShellStressTensor operator *(double coef, FlatShellStressTensor tensor)
        {
            var buf = new FlatShellStressTensor
            {
                BendingTensor = coef * tensor.BendingTensor,
                MembraneTensor = coef * tensor.MembraneTensor
            };

            return buf;
        }
    }
}
