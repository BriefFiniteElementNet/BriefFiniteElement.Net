using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a 3x3 tensor for stress in 3D
    /// </summary>
    /// <remarks>
    /// this is the order: 
    /// | ε₁₁ γ₁₂ γ₁₃| = |StrainTensor3D.S11 StrainTensor3D.S12 StrainTensor3D.S13|
    /// | γ₂₁ ε₂₂ γ₂₃| = |StrainTensor3D.S21 StrainTensor3D.S22 StrainTensor3D.S23|
    /// | γ₃₁ γ₃₂ ε₃₃| = |StrainTensor3D.S31 StrainTensor3D.S32 StrainTensor3D.S33|
    /// </remarks>
    public struct StrainTensor3D
    {
        public double S11, S12, S13, S21, S22, S23, S31, S32, S33;


    }
}
