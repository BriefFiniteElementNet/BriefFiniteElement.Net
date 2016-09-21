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

        public static MembraneStressTensor Multiply(MembraneStressTensor t1, double coef)
        {
            var buf = new MembraneStressTensor();

            buf.Sx = t1.Sx * coef;
            buf.Sy = t1.Sy * coef;
            buf.Txy = t1.Txy * coef;

            return buf;
        }

        /// <summary>
        /// Rotates the specified tensor.
        /// </summary>
        /// <param name="tensor">The tensor.</param>
        /// <param name="angle">The angle, in radian.</param>
        /// <returns></returns>
        public static MembraneStressTensor Rotate(MembraneStressTensor tensor, double angle)
        {
            var rads = angle;
            
            //formulation based on this: http://www.creatis.insa-lyon.fr/~dsarrut/bib/Archive/others/phys/www.jwave.vt.edu/crcd/batra/lectures/esmmse5984/node38.html

            var sigma = new Matrix(2, 2);
            sigma.SetRow(0, tensor.Sx, tensor.Txy);
            sigma.SetRow(1, tensor.Txy, tensor.Sy);

            var t = new Matrix(2, 2);
            t.SetRow(0, Math.Cos(rads), Math.Sin(rads));
            t.SetRow(1, -Math.Sin(rads), Math.Cos(rads));

            var rotated = t*sigma*t.Transpose();

            var buf = new MembraneStressTensor()
            {
                Sx = rotated[0, 0],
                Sy = rotated[1, 1],
                Txy = rotated[0, 1]
            };

            return buf;
        }

        public static MembraneStressTensor GetPrincipalStresses(MembraneStressTensor tens)
        {
            var c1 = (tens.Sx + tens.Sy) / 2;
            var c2 = Math.Sqrt(Math.Pow((tens.Sx - tens.Sy)/2, 2) + Math.Pow(tens.Txy, 2));

            var buf = new MembraneStressTensor();

            buf.Sx = c1 + c2;
            buf.Sy = c1 - c2;

            return buf;
            throw new NotImplementedException();
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

        public static PlateBendingStressTensor Multiply(PlateBendingStressTensor t1, double coef)
        {
            var buf = new PlateBendingStressTensor();

            buf.Mx = t1.Mx * coef;
            buf.My = t1.My * coef;
            buf.Mxy = t1.Mxy * coef;

            return buf;
        }
    }
}