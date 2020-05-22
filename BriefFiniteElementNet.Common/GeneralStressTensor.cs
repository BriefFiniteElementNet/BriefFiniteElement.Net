using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Common
{
    /// <summary>
    /// represents a stress general stress tensor which consists of a bending part and cauchy part, the most general stress tensor.
    /// </summary>
    public struct GeneralStressTensor
    {
        public GeneralStressTensor(CauchyStressTensor membraneTensor, BendingStressTensor bendingTensor)
        {
            MembraneTensor = membraneTensor;
            BendingTensor = bendingTensor;
        }

        public GeneralStressTensor(BendingStressTensor bendingTensor)
        {
            MembraneTensor = new CauchyStressTensor();
            BendingTensor = bendingTensor;
        }

        public GeneralStressTensor(CauchyStressTensor membraneTensor)
        {
            MembraneTensor = membraneTensor;
            BendingTensor = new BendingStressTensor();
        }

        public CauchyStressTensor MembraneTensor;

        public BendingStressTensor BendingTensor;

        public static GeneralStressTensor operator +(GeneralStressTensor left, GeneralStressTensor right)
        {
            var buf = new GeneralStressTensor
            {
                BendingTensor = left.BendingTensor + right.BendingTensor,
                MembraneTensor = left.MembraneTensor + right.MembraneTensor
            };


            return buf;
        }


        public static GeneralStressTensor operator -(GeneralStressTensor left, GeneralStressTensor right)
        {
            var buf = new GeneralStressTensor
            {
                BendingTensor = left.BendingTensor - right.BendingTensor,
                MembraneTensor = left.MembraneTensor - right.MembraneTensor
            };


            return buf;
        }

        public static GeneralStressTensor Multiply(double coef, GeneralStressTensor tensor)
        {
            var buf = new GeneralStressTensor
            {
                BendingTensor = coef * tensor.BendingTensor,
                MembraneTensor = coef * tensor.MembraneTensor
            };

            return buf;
        }

        public static GeneralStressTensor Multiply(GeneralStressTensor tensor, double coef)
        {
            var buf = new GeneralStressTensor
            {
                BendingTensor = coef * tensor.BendingTensor,
                MembraneTensor = coef * tensor.MembraneTensor
            };

            return buf;
        }

        public static GeneralStressTensor Transform(GeneralStressTensor tensor, Matrix transformationMatrix)
        {
            var buf = new GeneralStressTensor
            {
                MembraneTensor = CauchyStressTensor.Transform(tensor.MembraneTensor, transformationMatrix),
                BendingTensor = BendingStressTensor.Transform(tensor.BendingTensor, transformationMatrix)
            };

            return buf;
        }

        public static GeneralStressTensor operator *(double coef, GeneralStressTensor tensor)
        {
            var buf = new GeneralStressTensor
            {
                BendingTensor = coef * tensor.BendingTensor,
                MembraneTensor = coef * tensor.MembraneTensor
            };

            return buf;
        }
    }
}
