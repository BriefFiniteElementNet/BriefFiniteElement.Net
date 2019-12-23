using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    [Obsolete("use CauchyStressTensor instead")]
    public class MembraneStressTensor
    {
        public MembraneStressTensor()
        {
        }

        public MembraneStressTensor(double sx, double sy, double txy)
        {
            Sx = sx;
            Sy = sy;
            Txy = txy;
        }

        public double Sx, Sy, Sz; //σx, σy, σz, τxy
        public double Txy, Tyz, Tzx; //τxy, τyz, τzx

        public static MembraneStressTensor operator +(MembraneStressTensor left, MembraneStressTensor right)
        {
            var buf = new MembraneStressTensor();

            buf.Sx = left.Sx + right.Sx;
            buf.Sy = left.Sy + right.Sy;
            buf.Sz = left.Sz + right.Sz;

            buf.Txy = left.Txy + right.Txy;
            buf.Tyz = left.Tyz + right.Tyz;
            buf.Tzx = left.Tzx + right.Tzx;

            return buf;
        }

        public static MembraneStressTensor operator -(MembraneStressTensor left, MembraneStressTensor right)
        {
            var buf = new MembraneStressTensor();

            buf.Sx = left.Sx - right.Sx;
            buf.Sy = left.Sy - right.Sy;
            buf.Sz = left.Sz - right.Sz;

            buf.Txy = left.Txy - right.Txy;
            buf.Tyz = left.Tyz - right.Tyz;
            buf.Tzx = left.Tzx - right.Tzx;

            return buf;
        }

        public static MembraneStressTensor Multiply(MembraneStressTensor t1, double coef)
        {
            var buf = new MembraneStressTensor();

            buf.Sx = t1.Sx * coef;
            buf.Sy = t1.Sy * coef;
            buf.Sz = t1.Sz * coef;

            buf.Txy = t1.Txy * coef;
            buf.Tyz = t1.Tyz * coef;
            buf.Tzx = t1.Tzx * coef;

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
        }

        public static MembraneStressTensor Transform(MembraneStressTensor tensor, Matrix transformationMatrix)
        {
            var tensorMatrix = ToMatrix(tensor);

            var rtd = transformationMatrix.Transpose()*tensorMatrix*transformationMatrix;

            var buf = FromMatrix(rtd);

            return buf;
        }

        public static Matrix ToMatrix(MembraneStressTensor tensor)
        {
            var tens = new Matrix(3, 3);

            tens[0, 0] = tensor.Sx;
            tens[1, 1] = tensor.Sy;
            tens[2, 2] = tensor.Sz;

            tens[0, 1] = tens[1, 0] = tensor.Txy;
            tens[0, 2] = tens[2, 0] = tensor.Tzx;
            tens[1, 2] = tens[2, 1] = tensor.Tyz;

            return tens;
        }

        public static Matrix ToMatrix(CauchyStressTensor tensor)
        {
            var tens = new Matrix(3, 3);

            tens[0, 0] = tensor.S11;
            tens[1, 1] = tensor.S22;
            tens[2, 2] = tensor.S33;

            tens[0, 1] = tens[1, 0] = tensor.S12;
            tens[0, 2] = tens[2, 0] = tensor.S31;
            tens[1, 2] = tens[2, 1] = tensor.S23;

            return tens;
        }

        public static MembraneStressTensor FromMatrix(Matrix mtx)
        {
            var buf = new MembraneStressTensor();

            buf.Sx = mtx[0, 0];
            buf.Sy = mtx[1, 1];
            buf.Sz = mtx[2, 2];

            buf.Txy = mtx[0, 1];
            buf.Tyz = mtx[1, 2];
            buf.Tzx = mtx[2, 0];

            return buf;
        }



        public static implicit operator CauchyStressTensor(MembraneStressTensor tens)
        {
            var mtx = ToMatrix(tens);

            return CauchyStressTensor.FromMatrix(mtx);
        }

        public static implicit operator MembraneStressTensor(CauchyStressTensor tens)
        {
            var buf = new MembraneStressTensor();

            buf.Sx = tens.S11;
            buf.Sy = tens.S22;
            buf.Sz = tens.S33;

            buf.Txy = tens.S12;
            buf.Tyz = tens.S23;
            buf.Tzx = tens.S31;

            return buf;
        }
    }

    [Obsolete("use BendingStressTensor instead")]
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



        public static implicit operator PlateBendingStressTensor(BendingStressTensor tens)
        {
            var buf = new PlateBendingStressTensor();

            buf.Mx = tens.M11;
            buf.My = tens.M22;
            buf.Mxy = tens.M12;

            return buf;
        }

        public static implicit operator BendingStressTensor(PlateBendingStressTensor tens)
        {
            var buf = new BendingStressTensor();

            buf.M11 = tens.Mx;
            buf.M22 = tens.My;
            buf.M12 = tens.Mxy;

            return buf;
        }
    }
}