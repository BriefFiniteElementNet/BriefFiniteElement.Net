using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// represents a stress tensor for a flat shell which consists of a bending part and cauchy part
    /// </summary>
    [Obsolete("Use general stress tensor insted")]
    public struct FlatShellStressTensor
    {
        public FlatShellStressTensor(CauchyStressTensor membraneTensor, BendingStressTensor bendingTensor)
        {
            MembraneTensor = membraneTensor;
            BendingTensor = bendingTensor;
            TotalStressTensor = new CauchyStressTensor();
        }

        public FlatShellStressTensor(BendingStressTensor bendingTensor)
        {
            MembraneTensor = new CauchyStressTensor();
            BendingTensor = bendingTensor;
            TotalStressTensor = new CauchyStressTensor();
        }

        public FlatShellStressTensor(CauchyStressTensor membraneTensor)
        {
            MembraneTensor = membraneTensor;
            BendingTensor = new BendingStressTensor();
            TotalStressTensor = new CauchyStressTensor();
        }

        //Stress due to membrane action -> inplane
        public CauchyStressTensor MembraneTensor;

        //Bending stress due to bending action -> dependent on location (best at integration points)
        public BendingStressTensor BendingTensor;

        //Total stress = membrane + bending stress. Determine the bending tensor + specify a thickness where you want the stress
        public CauchyStressTensor TotalStressTensor;

        /// <summary>
        /// Gets the stress of a shell element (combination of a membrane + bending element) as the combination of both stresses
        /// </summary>
        /// <param name="shellThickness">The location where you want to calculate the bending stress (max at shellthickness)</param>
        /// <param name="probeLocation">The location of the stress probe. Top of thickness/shell, bottom or envelope (max abs of both). </param>
        public void UpdateTotalStress(double shellThickness, SectionPoints probeLocation)
        {
            switch (probeLocation)
            {
                case SectionPoints.Envelope:
                    {
                        CauchyStressTensor top = MembraneTensor + BendingStressTensor.ConvertBendingStressToCauchyTensor(BendingTensor, shellThickness, 1);
                        CauchyStressTensor bottom = MembraneTensor + BendingStressTensor.ConvertBendingStressToCauchyTensor(BendingTensor, shellThickness,-1);
                        if (Math.Abs(CauchyStressTensor.GetVonMisesStress(top)) > Math.Abs(CauchyStressTensor.GetVonMisesStress(bottom)))
                        {
                            this.TotalStressTensor = top;
                        }
                        else
                        {
                            this.TotalStressTensor = bottom;
                        }
                        break;
                    }
                case SectionPoints.Top:
                    {
                        this.TotalStressTensor = MembraneTensor + BendingStressTensor.ConvertBendingStressToCauchyTensor(BendingTensor,shellThickness, 1);
                        break;
                    }
                case SectionPoints.Bottom:
                    {
                        this.TotalStressTensor = MembraneTensor + BendingStressTensor.ConvertBendingStressToCauchyTensor(BendingTensor,shellThickness, -1);
                        break;
                    }
                default:
                    break;
            }
        }

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

    /// <summary>
    /// Location where you want to probe the stress
    /// </summary>
    public enum SectionPoints
    {
        //max abs of both top/bottom
        Envelope,
        //top of the shell
        Top,
        //bottom of the shell
        Bottom
    }
}
