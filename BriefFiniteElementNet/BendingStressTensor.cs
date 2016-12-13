using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a 3x3 bending tensor for bending stress in 3D. Usually uses for plate bending.
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

        /// <summary>
        /// Transforms the specified tensor using transformation matrix.
        /// </summary>
        /// <param name="tensor">The tensor.</param>
        /// <param name="transformationMatrix">The transformation matrix.</param>
        /// <returns>transformed tensor</returns>
        public static BendingStressTensor Transform(BendingStressTensor tensor, Matrix transformationMatrix)
        {
            var tensorMatrix = ToMatrix(tensor);

            var rtd = transformationMatrix.Transpose() * tensorMatrix * transformationMatrix;

            var buf = FromMatrix(rtd);

            return buf;
        }

        /// <summary>
        /// Reverse Transforms the specified tensor using transformation matrix.
        /// </summary>
        /// <param name="tensor">The tensor.</param>
        /// <param name="transformationMatrix">The transformation matrix.</param>
        /// <returns>reversely transformed tensor</returns>
        public static BendingStressTensor TransformBack(BendingStressTensor tensor, Matrix transformationMatrix)
        {
            var tensorMatrix = ToMatrix(tensor);

            var rtd = transformationMatrix * tensorMatrix * transformationMatrix.Transpose();

            var buf = FromMatrix(rtd);

            return buf;
        }


        public static Matrix ToMatrix(BendingStressTensor tensor)
        {
            var tens = new Matrix(3, 3);

            tens[0, 0] = tensor.M11;
            tens[1, 1] = tensor.M22;
            tens[2, 2] = tensor.M33;

            tens[0, 1] = tens[1, 0] = tensor.M12;
            tens[0, 2] = tens[2, 0] = tensor.M31;
            tens[1, 2] = tens[2, 1] = tensor.M23;

            return tens;
        }

        public static BendingStressTensor FromMatrix(Matrix mtx)
        {
            var buf = new BendingStressTensor();

            buf.M11 = mtx[0, 0];
            buf.M22 = mtx[1, 1];
            buf.M33 = mtx[2, 2];

            buf.M12 = mtx[0, 1];
            buf.M23 = mtx[1, 2];
            buf.M31 = mtx[2, 0];

            return buf;
        }



        #region math operators

        public static BendingStressTensor operator +(BendingStressTensor left, BendingStressTensor right)
        {
            var buf = new BendingStressTensor();

            buf.M11 = left.M11 + right.M11;
            buf.M22 = left.M22 + right.M22;
            buf.M33 = left.M33 + right.M33;

            buf.M12 = left.M12 + right.M12;
            buf.M23 = left.M23 + right.M23;
            buf.M31 = left.M31 + right.M31;

            return buf;
        }

        public static BendingStressTensor operator -(BendingStressTensor left, BendingStressTensor right)
        {
            var buf = new BendingStressTensor();

            buf.M11 = left.M11 - right.M11;
            buf.M22 = left.M22 - right.M22;
            buf.M33 = left.M33 - right.M33;

            buf.M12 = left.M12 - right.M12;
            buf.M23 = left.M23 - right.M23;
            buf.M31 = left.M31 - right.M31;

            return buf;
        }

        public static BendingStressTensor operator -(BendingStressTensor tensor)
        {
            var buf = new BendingStressTensor();

            buf.M11 = -tensor.M11;
            buf.M22 = -tensor.M22;
            buf.M33 = -tensor.M33;

            buf.M12 = -tensor.M12;
            buf.M23 = -tensor.M23;
            buf.M31 = -tensor.M31;

            return buf;
        }

        public static BendingStressTensor operator *(double coef, BendingStressTensor tensor)
        {
            var buf = new BendingStressTensor();

            buf.M11 = coef * tensor.M11;
            buf.M22 = coef * tensor.M22;
            buf.M33 = coef * tensor.M33;

            buf.M12 = coef * tensor.M12;
            buf.M23 = coef * tensor.M23;
            buf.M31 = coef * tensor.M31;

            return buf;
        }

        #endregion
    }
}
