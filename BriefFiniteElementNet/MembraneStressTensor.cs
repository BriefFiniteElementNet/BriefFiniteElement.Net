using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    public class MembraneStressTensor
    {
        public double Sx, Sy, Txy; //σx, σy, τxy

        public static MembraneStressTensor operator +(MembraneStressTensor left, MembraneStressTensor right)
        {
            var buf = new MembraneStressTensor();

            buf.Sx = left.Sx + right.Sx;
            buf.Sy = left.Sy + right.Sy;
            buf.Txy = left.Txy + right.Txy;

            return buf;
        }
    }

    public class PlateBendingStressTensor
    {
        public double Mx, My, Mxy; //fig 3, AN EXPLICIT FORMULATION FOR AN EFFICIENT TRIANGULAR PLATE-BENDING ELEMENT

        public static PlateBendingStressTensor operator +(PlateBendingStressTensor left, PlateBendingStressTensor right)
        {
            var buf = new PlateBendingStressTensor();

            buf.Mx = left.Mx + right.Mx;
            buf.My = left.My + right.My;
            buf.Mxy = left.Mxy + right.Mxy;

            return buf;
        }
    }
}