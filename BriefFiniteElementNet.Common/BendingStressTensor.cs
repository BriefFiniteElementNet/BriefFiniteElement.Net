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
        private double m33;
        private double m11;
        private double m12;
        private double m13;
        private double m21;
        private double m22;
        private double m23;
        private double m31;
        private double m32;

        public double M11
        {
            get { return m11; }
            set
            {
                m11 = value;
            }
        }
        public double M12
        {
            get { return m12; }
            set
            {
                m12 = value;
            }
        }
        public double M13
        {
            get { return m13; }
            set
            {
                m13 = value;
            }
        }
        public double M21
        {
            get { return m21; }
            set
            {
                m21 = value;
            }
        }
        public double M22
        {
            get { return m22; }
            set
            {
                m22 = value;
            }
        }
        public double M23
        {
            get { return m23; }
            set
            {
                m23 = value;
            }
        }
        public double M31
        {
            get { return m31; }
            set
            {
                m31 = value;
            }
        }
        public double M32
        {
            get { return m32; }
            set
            {
                m32 = value;
            }
        }
        public double M33
        {
            get { return m33; }
            set
            {
                m33 = value;
            }
        }

        /// <summary>
        /// The bending stress tensor needs to be converted into a cauchy stress tensor. Best to get the bending stresses at the integration points
        /// </summary>
        /// <param name="shellThickness">The thickness of the shell (for maximum bending stress)</param>
        /// <param name="lambda">Thickness coordinate. Must be between -1 (bottom) and 1 (top). A value of 0 results in 0 stress (bendings stress is equal to zero for all components on the mid-fibre) </param>
        /// <returns>The stress due to the bending of the plate. Add/subtract this stress with the membrame stress to get the total stress</returns>
        public static CauchyStressTensor ConvertBendingStressToCauchyTensor(BendingStressTensor tensor, double shellThickness, double lambda)
        {
            //follows same logic as bending of the beams
            //sigma = M . z / I

            CauchyStressTensor cauchyStressTensor = new CauchyStressTensor();

            var z = shellThickness / 2.0 * lambda;

            var I = Math.Pow(shellThickness, 3) / 12;

            //cauchyStressTensor.S11 = tensor.M11 * z / I;
            //cauchyStressTensor.S22 = tensor.M22 * z / I;

            cauchyStressTensor.S11 = tensor.M13 * z / I;//due to naming on bending stress components defined here: https://www.scielo.br/scielo.php?script=sci_arttext&pid=S1679-78252014000900010#fig1
            cauchyStressTensor.S22 = tensor.M31 * z / I;//due to naming on bending stress components defined here: https://www.scielo.br/scielo.php?script=sci_arttext&pid=S1679-78252014000900010#fig1

            //cauchyStressTensor.S12 = tensor.M12 * z / I;//are you sure?
            //cauchyStressTensor.S21 = tensor.M21 * z / I;//are you sure?

            return cauchyStressTensor;
        }

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

        /// <summary>
        /// Converts the defined tensor into a 3x3 matrix.
        /// </summary>
        /// <param name="tensor">The tensor.</param>
        /// <returns>generated matrix</returns>
        public static Matrix ToMatrix(BendingStressTensor tensor)
        {
            var tens = new Matrix(3, 3);

            tens[0, 0] = tensor.M11;
            tens[1, 1] = tensor.M22;
            tens[2, 2] = tensor.M33;

            tens[0, 1] =  tensor.M12;
            tens[1, 0] = tensor.M21;

            tens[0, 2] = tensor.M13;
            tens[2, 0] = tensor.M31;

            tens[1, 2] = tensor.M23;
            tens[2, 1] = tensor.M32;


            return tens;
        }

        /// <summary>
        /// Generates a tensor from defined matrix
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>generated tensor</returns>
        public static BendingStressTensor FromMatrix(Matrix mtx)
        {
            var buf = new BendingStressTensor();

            buf.M11 = mtx[0, 0];
            buf.M22 = mtx[1, 1];
            buf.M33 = mtx[2, 2];

            buf.M12 = mtx[0, 1];
            buf.M21 = mtx[1, 0];

            buf.M23 = mtx[1, 2];
            buf.M32 = mtx[2, 1];

            buf.M31 = mtx[2, 0];
            buf.M13 = mtx[0, 2];

            return buf;
        }

        /// <summary>
        /// Adds the specified tensors and return the result.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns><see cref="left"/> + <see cref="right"/></returns>
        public static BendingStressTensor Add(BendingStressTensor left, BendingStressTensor right)
        {
            var buf = new BendingStressTensor();

            buf.M11 = left.M11 + right.M11;
            buf.M22 = left.M22 + right.M22;
            buf.M33 = left.M33 + right.M33;

            buf.M12 = left.M12 + right.M12;
            buf.M23 = left.M23 + right.M23;
            buf.M31 = left.M31 + right.M31;

            buf.M21 = left.M21 + right.M21;
            buf.M32 = left.M32 + right.M32;
            buf.M13 = left.M13 + right.M13;

            return buf;
        }

        /// <summary>
        /// Subtract the specified tensors and return the result.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns><see cref="left"/> - <see cref="right"/></returns>
        public static BendingStressTensor Subtract(BendingStressTensor left, BendingStressTensor right)
        {
            var buf = new BendingStressTensor();

            buf.M11 = left.M11 - right.M11;
            buf.M22 = left.M22 - right.M22;
            buf.M33 = left.M33 - right.M33;

            buf.M12 = left.M12 - right.M12;
            buf.M23 = left.M23 - right.M23;
            buf.M31 = left.M31 - right.M31;

            buf.M21 = left.M21 - right.M21;
            buf.M32 = left.M32 - right.M32;
            buf.M13 = left.M13 - right.M13;

            return buf;
        }

        /// <summary>
        /// Mutiplies the specified tensor with specified coefficient.
        /// </summary>
        /// <param name="tensor">The left.</param>
        /// <param name="coefficient">The coefficient.</param>
        /// <returns><see cref="tensor"/> * <see cref="coefficient"/></returns>
        public static BendingStressTensor Multiply(BendingStressTensor tensor, double coefficient)
        {
            var buf = new BendingStressTensor();

            buf.M11 = coefficient * tensor.M11;
            buf.M22 = coefficient * tensor.M22;
            buf.M33 = coefficient * tensor.M33;

            buf.M12 = coefficient * tensor.M12;
            buf.M23 = coefficient * tensor.M23;
            buf.M31 = coefficient * tensor.M31;

            buf.M21 = coefficient * tensor.M21;
            buf.M32 = coefficient * tensor.M32;
            buf.M13 = coefficient * tensor.M13;

            return buf;
        }

        #region math operators

        public static BendingStressTensor operator +(BendingStressTensor left, BendingStressTensor right)
        {
            return Add(left, right);
        }

        public static BendingStressTensor operator -(BendingStressTensor left, BendingStressTensor right)
        {
            return Subtract(left, right);
        }

        public static BendingStressTensor operator -(BendingStressTensor tensor)
        {
            return Multiply(tensor, -1);
        }

        public static BendingStressTensor operator *(double coef, BendingStressTensor tensor)
        {
            return Multiply(tensor, coef);
        }

        #endregion
    }
}
