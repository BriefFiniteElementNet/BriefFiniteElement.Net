using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a 3x3 strain tensor
    /// </summary>
    /// <remarks>
    /// this is the order: 
    /// | σ₁₁ τ₁₂ τ₁₃| = |S11 S12 S13|
    /// | τ₂₁ σ₂₂ τ₂₃| = |S21 S22 S23|
    /// | τ₃₁ τ₃₂ σ₃₃| = |S31 S32 S33|
    /// for more info:
    /// https://en.wikipedia.org/wiki/Cauchy_stress_tensor
    /// </remarks>
    [Obsolete]
    public struct StrainTensor
    {
        public double E11, E12, E13, E21, E22, E23, E31, E32, E33;

        /// <summary>
        /// Transforms the specified tensor using transformation matrix.
        /// </summary>
        /// <param name="tensor">The tensor.</param>
        /// <param name="transformationMatrix">The transformation matrix.</param>
        /// <returns>transformed tensor</returns>
        public static StrainTensor Transform(StrainTensor tensor, Matrix transformationMatrix)
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
        public static StrainTensor TransformBack(StrainTensor tensor, Matrix transformationMatrix)
        {
            var tensorMatrix = ToMatrix(tensor);

            var rtd = transformationMatrix * tensorMatrix * transformationMatrix.Transpose();

            var buf = FromMatrix(rtd);

            return buf;
        }

        /// <summary>
        /// Converts the defined tensor into a 3x3 matrix.
        /// </summary>
        /// <param name="tensor">The tensor.</param>
        /// <returns>generated matrix</returns>
        public static Matrix ToMatrix(StrainTensor tensor)
        {
            var tens = new Matrix(3, 3);

            tens[0, 0] = tensor.E11;
            tens[1, 1] = tensor.E22;
            tens[2, 2] = tensor.E33;

            tens[0, 1] = tensor.E12;
            tens[1, 0] = tensor.E21;

            tens[0, 2] = tensor.E13;
            tens[2, 0] = tensor.E31;

            tens[1, 2] = tensor.E23;
            tens[2, 1] = tensor.E32;

            return tens;
        }

        /// <summary>
        /// Generates a tensor from defined matrix
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>generated tensor</returns>
        public static StrainTensor FromMatrix(Matrix matrix)
        {
            var buf = new StrainTensor();

            buf.E11 = matrix[0, 0];
            buf.E22 = matrix[1, 1];
            buf.E33 = matrix[2, 2];

            buf.E12 = matrix[0, 1];
            buf.E21 = matrix[1, 0];

            buf.E13 = matrix[0, 2];
            buf.E31 = matrix[2, 0];

            buf.E23 = matrix[1, 2];
            buf.E32 = matrix[2, 1];

            return buf;
        }

        /// <summary>
        /// Mutiplies the specified tensor with specified coefficient.
        /// </summary>
        /// <param name="tensor">The left.</param>
        /// <param name="coefficient">The coefficient.</param>
        /// <returns><see cref="tensor"/> * <see cref="coefficient"/></returns>
        public static StrainTensor Multiply(StrainTensor tensor, double coef)
        {
            var buf = new StrainTensor();

            buf.E11 = coef * tensor.E11;
            buf.E22 = coef * tensor.E22;
            buf.E33 = coef * tensor.E33;

            buf.E12 = coef * tensor.E12;
            buf.E23 = coef * tensor.E23;
            buf.E31 = coef * tensor.E31;

            buf.E21 = coef * tensor.E21;
            buf.E32 = coef * tensor.E32;
            buf.E13 = coef * tensor.E13;

            return buf;
        }

        /// <summary>
        /// Adds the specified tensors and return the result.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns><see cref="left"/> + <see cref="right"/></returns>
        public static StrainTensor Add(StrainTensor left, StrainTensor right)
        {
            var buf = new StrainTensor();

            buf.E11 = left.E11 + right.E11;
            buf.E22 = left.E22 + right.E22;
            buf.E33 = left.E33 + right.E33;

            buf.E12 = left.E12 + right.E12;
            buf.E23 = left.E23 + right.E23;
            buf.E31 = left.E31 + right.E31;

            buf.E21 = left.E21 + right.E21;
            buf.E32 = left.E32 + right.E32;
            buf.E13 = left.E13 + right.E13;

            return buf;
        }

        /// <summary>
        /// Subtract the specified tensors and return the result.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns><see cref="left"/> - <see cref="right"/></returns>
        public static StrainTensor Subtract(StrainTensor left, StrainTensor right)
        {
            var buf = new StrainTensor();

            buf.E11 = left.E11 - right.E11;
            buf.E22 = left.E22 - right.E22;
            buf.E33 = left.E33 - right.E33;

            buf.E12 = left.E12 - right.E12;
            buf.E23 = left.E23 - right.E23;
            buf.E31 = left.E31 - right.E31;

            buf.E21 = left.E21 - right.E21;
            buf.E32 = left.E32 - right.E32;
            buf.E13 = left.E13 - right.E13;

            return buf;
        }


        #region math operators

        public static StrainTensor operator +(StrainTensor left, StrainTensor right)
        {
            return Add(left, right);
        }

        public static StrainTensor operator -(StrainTensor left, StrainTensor right)
        {
            return Subtract(left, right);
        }

        public static StrainTensor operator -(StrainTensor tensor)
        {
            return Multiply(tensor, -1);
        }

        public static StrainTensor operator *(double coef, StrainTensor tensor)
        {
            return Multiply(tensor, coef);
        }

        #endregion

    }

    
}