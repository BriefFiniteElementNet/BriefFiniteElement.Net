using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a 3x3 Cauchy tensor for stress in 3D
    /// </summary>
    /// <remarks>
    /// this is the order: 
    /// | σ₁₁ τ₁₂ τ₁₃| = |S11 S12 S13|
    /// | τ₂₁ σ₂₂ τ₂₃| = |S21 S22 S23|
    /// | τ₃₁ τ₃₂ σ₃₃| = |S31 S32 S33|
    /// </remarks>
    public struct CauchyStressTensor3D
    {
        public double S11, S12, S13, S21, S22, S23, S31, S32, S33;
    }
}