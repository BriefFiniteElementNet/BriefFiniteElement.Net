using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    /// <summary>
    /// Represents a class for managing the linear coordination transformation from local to global and vice versa.
    /// </summary>
    /// <remarks>
    /// Planned to be used for transforming local to global
    /// means <see cref="TransformLocalToGlobal(BriefFiniteElementNet.Vector)"/> will transform from local to global, and <see cref="TransformGlobalToLocal"/> converts from global to local.
    /// </remarks>
    public class LocalGlobalTransformManager
    {

        /// <summary>
        /// Makes a new <see cref="LocalGlobalTransformManager"/> from a transformation matrix.
        /// </summary>
        /// <param name="transformMatrix">The T Matrix.</param>
        /// <returns>new LocalGlobalTransformManager</returns>
        public static LocalGlobalTransformManager MakeFromTransformationMatrix(Matrix transformMatrix)
        {
            var buf = new LocalGlobalTransformManager();
            buf.TransformMatrix = transformMatrix;

            buf.VeryMagicNumber = 2;

            return buf;
        }

        /// <summary>
        /// Makes a new <see cref="LocalGlobalTransformManager"/> from a lambda matrix.
        /// </summary>
        /// <param name="lambdaMatrix">The λ Matrix.</param>
        /// <returns>new LocalGlobalTransformManager</returns>
        public static LocalGlobalTransformManager MakeFromLambdaMatrix(Matrix lambdaMatrix)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The transform
        /// </summary>
        /// <remarks>
        /// transform matrix, which when applied to local vector, </remarks>
        private Matrix TransformMatrix;

        /// <summary>
        /// The lambda matrix
        /// </summary>
        private Matrix LambdaMatrix;

        /// <summary>
        /// The magic number, if == 1 use lambda matrix, if == 2 use tranfrom matrix.
        /// </summary>
        private int VeryMagicNumber;

        /// <summary>
        /// Transforms the specified vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Transformed vector (vector in new coordination system : global)</returns>
        public Vector TransformLocalToGlobal(Vector vector)
        {
            var lambda = TransformMatrix;

            var buf = new Vector();

            buf.X =
                lambda[0, 0] * vector.X +
                lambda[0, 1] * vector.Y +
                lambda[0, 2] * vector.Z;

            buf.Y =
                lambda[1, 0] * vector.X +
                lambda[1, 1] * vector.Y +
                lambda[1, 2] * vector.Z;

            buf.Z =
                lambda[2, 0] * vector.X +
                lambda[2, 1] * vector.Y +
                lambda[2, 2] * vector.Z;

            return buf;
        }

        /// <summary>
        /// Transforms back the specified vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Back transformed vector (vector in original coordination system : local)</returns>
        public Vector TransformGlobalToLocal(Vector vector)
        {
            var lambda = TransformMatrix;

            var buf = new Vector();

            buf.X =
                lambda[0, 0] * vector.X +
                lambda[1, 0] * vector.Y +
                lambda[2, 0] * vector.Z;

            buf.Y =
                lambda[0, 1] * vector.X +
                lambda[1, 1] * vector.Y +
                lambda[2, 1] * vector.Z;

            buf.Z =
                lambda[0, 2] * vector.X +
                lambda[1, 2] * vector.Y +
                lambda[2, 2] * vector.Z;

            return buf;
        }

        /// <summary>
        /// Transforms the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Transformed point (point in new coordination system : global)</returns>
        public Vector TransformLocalToGlobal(Point point)
        {
            var lambda = TransformMatrix;

            var buf = new Vector();

            buf.X =
                lambda[0, 0] * point.X +
                lambda[0, 1] * point.Y +
                lambda[0, 2] * point.Z;

            buf.Y =
                lambda[1, 0] * point.X +
                lambda[1, 1] * point.Y +
                lambda[1, 2] * point.Z;

            buf.Z =
                lambda[2, 0] * point.X +
                lambda[2, 1] * point.Y +
                lambda[2, 2] * point.Z;

            return buf;
        }

        /// <summary>
        /// Transforms back the specified point.
        /// </summary>
        /// <param name="point">The vector.</param>
        /// <returns>Back transformed point (point in original coordination system : local)</returns>
        public Vector TransformGlobalToLocal(Point point)
        {
            var lambda = TransformMatrix;

            var buf = new Vector();

            buf.X =
                lambda[0, 0] * point.X +
                lambda[1, 0] * point.Y +
                lambda[2, 0] * point.Z;

            buf.Y =
                lambda[0, 1] * point.X +
                lambda[1, 1] * point.Y +
                lambda[2, 1] * point.Z;

            buf.Z =
                lambda[0, 2] * point.X +
                lambda[1, 2] * point.Y +
                lambda[2, 2] * point.Z;

            return buf;
        }


        /// <summary>
        /// Transforms the defined square matrix from global system to local system.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <remarks><see cref="matrix"/> should be square, and its dimension should be a multiply of 3</remarks>
        /// <returns>matrix in local coordination system</returns>
        public Matrix TransformGlobalToLocal(Matrix matrix)
        {
            if (VeryMagicNumber == 1)
                return G2L_L(matrix);

            if (VeryMagicNumber == 2)
                return G2L_T(matrix);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Transforms the defined square matrix from local system to global system.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <remarks><see cref="matrix"/> should be square, and its dimension should be a multiply of 3</remarks>
        /// <returns>Transformed point (point in new coordination system : global)</returns>
        public Matrix TransformLocalToGlobal(Matrix matrix)
        {
            if (VeryMagicNumber == 1)
                return L2G_L(matrix);

            if (VeryMagicNumber == 2)
                return L2G_T(matrix);

            throw new NotImplementedException();
        }


        /// <summary>
        /// Transforms the global to local using lambda.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// </exception>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks>
        /// Main advantage of this method is higher performance for multiplying transformation</remarks>
        public Matrix G2L_L(Matrix matrix)
        {
            throw new NotImplementedException();
        }

        public Matrix L2G_L(Matrix matrix)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Transforms the defined square matrix from local system to global system.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <remarks><see cref="matrix"/> should be square, and its dimension should be a multiply of 3</remarks>
        /// <returns>matrix in global coordination system</returns>
        public Matrix L2G_T(Matrix matrix)
        {
            var tr = TransformMatrix;

            if (!tr.IsSquare() || !matrix.IsSquare())
                throw new Exception();

            if (tr.RowCount != 3 || matrix.RowCount % 3 != 0)
                throw new Exception();

            var count = matrix.RowCount / 3;

            var t11 = tr[0, 0];
            var t12 = tr[0, 1];
            var t13 = tr[0, 2];

            var t21 = tr[1, 0];
            var t22 = tr[1, 1];
            var t23 = tr[1, 2];

            var t31 = tr[2, 0];
            var t32 = tr[2, 1];
            var t33 = tr[2, 2];

            var buf = new Matrix(matrix.RowCount, matrix.ColumnCount);

            for (var ic = 0; ic < count; ic++)
            {
                for (var jc = 0; jc < count; jc++)
                {
                    var iStart = ic * 3;
                    var jStart = jc * 3;

                    var k_i_j = matrix[iStart, jStart];
                    var k_i_j1 = matrix[iStart, jStart + 1];
                    var k_i_j2 = matrix[iStart, jStart + 2];

                    var k_i1_j = matrix[iStart + 1, jStart];
                    var k_i1_j1 = matrix[iStart + 1, jStart + 1];
                    var k_i1_j2 = matrix[iStart + 1, jStart + 2];

                    var k_i2_j = matrix[iStart + 2, jStart];
                    var k_i2_j1 = matrix[iStart + 2, jStart + 1];
                    var k_i2_j2 = matrix[iStart + 2, jStart + 2];


                    buf[iStart, jStart] = t11 * (k_i1_j * t21 + k_i2_j * t31 + k_i_j * t11) + t21 * (k_i1_j1 * t21 + k_i2_j1 * t31 + k_i_j1 * t11) + t31 * (k_i1_j2 * t21 + k_i2_j2 * t31 + k_i_j2 * t11);
                    buf[iStart, jStart + 1] = t12 * (k_i1_j * t21 + k_i2_j * t31 + k_i_j * t11) + t22 * (k_i1_j1 * t21 + k_i2_j1 * t31 + k_i_j1 * t11) + t32 * (k_i1_j2 * t21 + k_i2_j2 * t31 + k_i_j2 * t11);
                    buf[iStart, jStart + 2] = t13 * (k_i1_j * t21 + k_i2_j * t31 + k_i_j * t11) + t23 * (k_i1_j1 * t21 + k_i2_j1 * t31 + k_i_j1 * t11) + t33 * (k_i1_j2 * t21 + k_i2_j2 * t31 + k_i_j2 * t11);

                    buf[iStart + 1, jStart] = t11 * (k_i1_j * t22 + k_i2_j * t32 + k_i_j * t12) + t21 * (k_i1_j1 * t22 + k_i2_j1 * t32 + k_i_j1 * t12) + t31 * (k_i1_j2 * t22 + k_i2_j2 * t32 + k_i_j2 * t12);
                    buf[iStart + 1, jStart + 1] = t12 * (k_i1_j * t22 + k_i2_j * t32 + k_i_j * t12) + t22 * (k_i1_j1 * t22 + k_i2_j1 * t32 + k_i_j1 * t12) + t32 * (k_i1_j2 * t22 + k_i2_j2 * t32 + k_i_j2 * t12);
                    buf[iStart + 1, jStart + 2] = t13 * (k_i1_j * t22 + k_i2_j * t32 + k_i_j * t12) + t23 * (k_i1_j1 * t22 + k_i2_j1 * t32 + k_i_j1 * t12) + t33 * (k_i1_j2 * t22 + k_i2_j2 * t32 + k_i_j2 * t12);

                    buf[iStart + 2, jStart] = t11 * (k_i1_j * t23 + k_i2_j * t33 + k_i_j * t13) + t21 * (k_i1_j1 * t23 + k_i2_j1 * t33 + k_i_j1 * t13) + t31 * (k_i1_j2 * t23 + k_i2_j2 * t33 + k_i_j2 * t13);
                    buf[iStart + 2, jStart + 1] = t12 * (k_i1_j * t23 + k_i2_j * t33 + k_i_j * t13) + t22 * (k_i1_j1 * t23 + k_i2_j1 * t33 + k_i_j1 * t13) + t32 * (k_i1_j2 * t23 + k_i2_j2 * t33 + k_i_j2 * t13);
                    buf[iStart + 2, jStart + 2] = t13 * (k_i1_j * t23 + k_i2_j * t33 + k_i_j * t13) + t23 * (k_i1_j1 * t23 + k_i2_j1 * t33 + k_i_j1 * t13) + t33 * (k_i1_j2 * t23 + k_i2_j2 * t33 + k_i_j2 * t13);
                }
            }

            return buf; 
        }

        private Matrix G2L_T(Matrix matrix)
        {
            throw new NotImplementedException();
        }
    }
}
