using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a 3x3 bending tensor for stress in 3D. Usually uses for plate bending.
    /// </summary>
    /// <remarks>
    /// this is the order: 
    /// | μ₁₁ μ₁₂ μ₁₃| = |M11 M12 M13|
    /// | μ₂₁ μ₂₂ μ₂₃| = |M21 M22 M23|
    /// | μ₃₁ μ₃₂ μ₃₃| = |M31 M32 M33|
    /// 
    /// for more info see this: 
    /// http://www.scielo.br/scielo.php?script=sci_arttext&pid=S1679-78252014000900010#fig1
    /// </remarks>
    public struct BendingStressTensor
    {
        public double M11, M12, M13, M21, M22, M23, M31, M32, M33;
    }
}
