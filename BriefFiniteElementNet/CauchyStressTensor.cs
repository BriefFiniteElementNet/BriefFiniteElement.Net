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
    /// for more info:
    /// https://en.wikipedia.org/wiki/Cauchy_stress_tensor
    /// </remarks>
    public struct CauchyStressTensor
    {
        public double S11, S12, S13, S21, S22, S23, S31, S32, S33;

        /// <summary>
        /// Transforms the specified tensor using transformation matrix.
        /// </summary>
        /// <param name="tensor">The tensor.</param>
        /// <param name="transformationMatrix">The transformation matrix.</param>
        /// <returns>transformed tensor</returns>
        public static CauchyStressTensor Transform(CauchyStressTensor tensor, Matrix transformationMatrix)
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
        public static CauchyStressTensor TransformBack(CauchyStressTensor tensor, Matrix transformationMatrix)
        {
            var tensorMatrix = ToMatrix(tensor);

            var rtd = transformationMatrix * tensorMatrix * transformationMatrix.Transpose();

            var buf = FromMatrix(rtd);

            return buf;
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

        public static CauchyStressTensor FromMatrix(Matrix mtx)
        {
            var buf = new CauchyStressTensor();

            buf.S11 = mtx[0, 0];
            buf.S22 = mtx[1, 1];
            buf.S33 = mtx[2, 2];

            buf.S12 = mtx[0, 1];
            buf.S23 = mtx[1, 2];
            buf.S31 = mtx[2, 0];

            return buf;
        }


        #region math operators

        public static CauchyStressTensor operator +(CauchyStressTensor left, CauchyStressTensor right)
        {
            var buf = new CauchyStressTensor();

            buf.S11 = left.S11 + right.S11;
            buf.S22 = left.S22 + right.S22;
            buf.S33 = left.S33 + right.S33;

            buf.S12 = left.S12 + right.S12;
            buf.S23 = left.S23 + right.S23;
            buf.S31 = left.S31 + right.S31;

            return buf;
        }

        public static CauchyStressTensor operator -(CauchyStressTensor left, CauchyStressTensor right)
        {
            var buf = new CauchyStressTensor();

            buf.S11 = left.S11 - right.S11;
            buf.S22 = left.S22 - right.S22;
            buf.S33 = left.S33 - right.S33;

            buf.S12 = left.S12 - right.S12;
            buf.S23 = left.S23 - right.S23;
            buf.S31 = left.S31 - right.S31;

            return buf;
        }

        public static CauchyStressTensor operator -(CauchyStressTensor tensor)
        {
            var buf = new CauchyStressTensor();

            buf.S11 = -tensor.S11;
            buf.S22 = -tensor.S22;
            buf.S33 = -tensor.S33;

            buf.S12 = -tensor.S12;
            buf.S23 = -tensor.S23;
            buf.S31 = -tensor.S31;

            return buf;
        }

        public static CauchyStressTensor operator *(double coef, CauchyStressTensor tensor)
        {
            var buf = new CauchyStressTensor();

            buf.S11 = coef*tensor.S11;
            buf.S22 = coef*tensor.S22;
            buf.S33 = coef*tensor.S33;

            buf.S12 = coef*tensor.S12;
            buf.S23 = coef*tensor.S23;
            buf.S31 = coef*tensor.S31;

            return buf;
        }

        #endregion

    }

    
}