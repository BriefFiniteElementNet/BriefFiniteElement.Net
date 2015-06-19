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
    /// | σ₁₁ τ₁₂ τ₁₃| = |StressTensor3D.S11 StressTensor3D.S12 StressTensor3D.S13|
    /// | τ₂₁ σ₂₂ τ₂₃| = |StressTensor3D.S21 StressTensor3D.S22 StressTensor3D.S23|
    /// | τ₃₁ τ₃₂ σ₃₃| = |StressTensor3D.S31 StressTensor3D.S32 StressTensor3D.S33|
    /// </remarks>
    public struct StressTensor3D
    {
        public double S11, S12, S13, S21, S22, S23, S31, S32, S33;


    }
}