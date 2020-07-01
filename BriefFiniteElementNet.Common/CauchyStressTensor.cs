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
        private double s33;
        private double s11;
        private double s12;
        private double s13;
        private double s21;
        private double s22;
        private double s23;
        private double s31;
        private double s32;

        public double S11
        {
            get { return s11; }
            set
            {
                s11 = value;
            }
        }
        public double S12
        {
            get { return s12; }
            set
            {
                s12 = value;
            }
        }
        public double S13
        {
            get { return s13; }
            set
            {
                s13 = value;
            }
        }
        public double S21
        {
            get { return s21; }
            set
            {
                s21 = value;
            }
        }
        public double S22
        {
            get { return s22; }
            set
            {
                s22 = value;
            }
        }
        public double S23
        {
            get { return s23; }
            set
            {
                s23 = value;
            }
        }
        public double S31
        {
            get { return s31; }
            set
            {
                s31 = value;
            }
        }
        public double S32
        {
            get { return s32; }
            set
            {
                s32 = value;
            }
        }
        public double S33
        {
            get { return s33; }
            set
            {
                s33 = value;
            }
        }

        /// <summary>
        /// Updates the principal stresses based on the tensor 
        /// </summary>
        public static CauchyStressTensor GetPrincipalStresses(CauchyStressTensor t)
        {
            Matrix tmpTrans;
            return GetPrincipalStresses(t, out tmpTrans);
        }

        /// <summary>
        /// Updates the principal stresses based on the tensor 
        /// </summary>
        /// <param name="transformationMatrix">the transformation matrix that converts principal stresses into given stress</param>
        /// <param name="t">the tensor</param>
        public static CauchyStressTensor GetPrincipalStresses(CauchyStressTensor t, out Matrix transformationMatrix)
        {
            //better to do with eigenvalue and eigen vector
            //more info here
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the von Mises stress 
        /// </summary>
        /// <returns>The von Mises stress</returns>
        public static double GetVonMisesStress(CauchyStressTensor t)
        {
            double temp = 1.0 / Math.Sqrt(2.0);
            double tempXXYY = Math.Pow(t.s11 - t.s22, 2);
            double tempYYZZ = Math.Pow(t.s22 - t.s33, 2);
            double tempZZXX = Math.Pow(t.s33 - t.s11, 2);

            double tempXY2 = Math.Pow(t.s12, 2);
            double tempYZ2 = Math.Pow(t.s23, 2);
            double tempZX2 = Math.Pow(t.s31, 2);

            double squared = tempXXYY + tempYYZZ + tempZZXX + 6.0 * (tempXY2 + tempYZ2 + tempZX2);
            return temp * Math.Sqrt(squared);
        }

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

        /// <summary>
        /// Converts the defined tensor into a 3x3 matrix.
        /// </summary>
        /// <param name="tensor">The tensor.</param>
        /// <returns>generated matrix</returns>
        public static Matrix ToMatrix(CauchyStressTensor tensor)
        {
            var tens = new Matrix(3, 3);

            tens[0, 0] = tensor.S11;
            tens[1, 1] = tensor.S22;
            tens[2, 2] = tensor.S33;

            tens[0, 1] = tensor.S12;
            tens[1, 0] = tensor.S21;

            tens[0, 2] = tensor.S13;
            tens[2, 0] = tensor.S31;

            tens[1, 2] = tensor.S23;
            tens[2, 1] = tensor.S32;

            return tens;
        }

        /// <summary>
        /// Generates a tensor from defined matrix
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>generated tensor</returns>
        public static CauchyStressTensor FromMatrix(Matrix matrix)
        {
            var buf = new CauchyStressTensor();

            buf.S11 = matrix[0, 0];
            buf.S22 = matrix[1, 1];
            buf.S33 = matrix[2, 2];

            buf.S12 = matrix[0, 1];
            buf.S21 = matrix[1, 0];

            buf.S13 = matrix[0, 2];
            buf.S31 = matrix[2, 0];

            buf.S23 = matrix[1, 2];
            buf.S32 = matrix[2, 1];

            return buf;
        }

        /// <summary>
        /// Mutiplies the specified tensor with specified coefficient.
        /// </summary>
        /// <param name="tensor">The left.</param>
        /// <param name="coefficient">The coefficient.</param>
        /// <returns><see cref="tensor"/> * <see cref="coefficient"/></returns>
        public static CauchyStressTensor Multiply(CauchyStressTensor tensor, double coef)
        {
            var buf = new CauchyStressTensor();

            buf.S11 = coef * tensor.S11;
            buf.S22 = coef * tensor.S22;
            buf.S33 = coef * tensor.S33;

            buf.S12 = coef * tensor.S12;
            buf.S23 = coef * tensor.S23;
            buf.S31 = coef * tensor.S31;

            buf.S21 = coef * tensor.S21;
            buf.S32 = coef * tensor.S32;
            buf.S13 = coef * tensor.S13;

            return buf;
        }

        /// <summary>
        /// Adds the specified tensors and return the result.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns><see cref="left"/> + <see cref="right"/></returns>
        public static CauchyStressTensor Add(CauchyStressTensor left, CauchyStressTensor right)
        {
            var buf = new CauchyStressTensor();

            buf.S11 = left.S11 + right.S11;
            buf.S22 = left.S22 + right.S22;
            buf.S33 = left.S33 + right.S33;

            buf.S12 = left.S12 + right.S12;
            buf.S23 = left.S23 + right.S23;
            buf.S31 = left.S31 + right.S31;

            buf.S21 = left.S21 + right.S21;
            buf.S32 = left.S32 + right.S32;
            buf.S13 = left.S13 + right.S13;

            return buf;
        }

        /// <summary>
        /// Subtract the specified tensors and return the result.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns><see cref="left"/> - <see cref="right"/></returns>
        public static CauchyStressTensor Subtract(CauchyStressTensor left, CauchyStressTensor right)
        {
            var buf = new CauchyStressTensor();

            buf.S11 = left.S11 - right.S11;
            buf.S22 = left.S22 - right.S22;
            buf.S33 = left.S33 - right.S33;

            buf.S12 = left.S12 - right.S12;
            buf.S23 = left.S23 - right.S23;
            buf.S31 = left.S31 - right.S31;

            buf.S21 = left.S21 - right.S21;
            buf.S32 = left.S32 - right.S32;
            buf.S13 = left.S13 - right.S13;

            return buf;
        }


        #region math operators

        public static CauchyStressTensor operator +(CauchyStressTensor left, CauchyStressTensor right)
        {
            return Add(left, right);
        }

        public static CauchyStressTensor operator -(CauchyStressTensor left, CauchyStressTensor right)
        {
            return Subtract(left, right);
        }

        public static CauchyStressTensor operator -(CauchyStressTensor tensor)
        {
            return Multiply(tensor, -1);
        }

        public static CauchyStressTensor operator *(double coef, CauchyStressTensor tensor)
        {
            return Multiply(tensor, coef);
        }

        #endregion

    }


}