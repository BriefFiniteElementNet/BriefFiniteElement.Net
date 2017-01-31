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
            var buf = new LocalGlobalTransformManager();
            buf.LambdaMatrix = lambdaMatrix;

            buf.VeryMagicNumber = 1;

            return buf;
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
            double[] buf;

            if (VeryMagicNumber == 1)
                buf = A_B(LambdaMatrix, vector.X, vector.Y, vector.Z);
            else
                buf = At_B(TransformMatrix, vector.X, vector.Y, vector.Z);

            return new Vector(buf[0], buf[1], buf[2]);
        }

        /// <summary>
        /// Transforms the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Transformed point (point in new coordination system : global)</returns>
        public Point TransformLocalToGlobal(Point point)
        {
            double[] buf;

            if (VeryMagicNumber == 1)
                buf = A_B(LambdaMatrix, point.X, point.Y, point.Z);
            else
                buf = At_B(TransformMatrix, point.X, point.Y, point.Z);

            return new Point(buf[0], buf[1], buf[2]);
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
                return A_B_At(LambdaMatrix, matrix);

            if (VeryMagicNumber == 2)
                return At_B_A(TransformMatrix, matrix);

            throw new NotImplementedException();
        }




        /// <summary>
        /// Transforms back the specified vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Back transformed vector (vector in original coordination system : local)</returns>
        public Vector TransformGlobalToLocal(Vector vector)
        {
            double[] buf;

            if (VeryMagicNumber == 1)
                buf = At_B(LambdaMatrix, vector.X, vector.Y, vector.Z);
            else
                buf = A_B(TransformMatrix, vector.X, vector.Y, vector.Z);

            return new Vector(buf[0], buf[1], buf[2]);
        }

        /// <summary>
        /// Transforms back the specified point.
        /// </summary>
        /// <param name="point">The vector.</param>
        /// <returns>Back transformed point (point in original coordination system : local)</returns>
        public Point TransformGlobalToLocal(Point point)
        {
            double[] buf;

            if (VeryMagicNumber == 1)
                buf = At_B(LambdaMatrix, point.X, point.Y, point.Z);
            else
                buf = A_B(TransformMatrix, point.X, point.Y, point.Z);

            return new Point(buf[0], buf[1], buf[2]);
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
                return At_B_A(LambdaMatrix, matrix);

            if (VeryMagicNumber == 2)
                return A_B_At(TransformMatrix, matrix);

            throw new NotImplementedException();
        }







        /// <summary>
        /// calculated the C = A' * B  * A where B is 3nX3n matrix and A is 3x3 matrix (read the docs for more info)
        /// </summary>
        /// <param name="A">the A</param>
        /// <param name="B">The B</param>
        /// <returns>C</returns>
        internal static Matrix At_B_A(Matrix A, Matrix B)
        {
            var matrix = B;

            if (!A.IsSquare() || !matrix.IsSquare())
                throw new Exception();

            if (A.RowCount != 3 || matrix.RowCount % 3 != 0)
                throw new Exception();

            var count = matrix.RowCount / 3;

            var a11 = A[0, 0];
            var a12 = A[0, 1];
            var a13 = A[0, 2];

            var a21 = A[1, 0];
            var a22 = A[1, 1];
            var a23 = A[1, 2];

            var a31 = A[2, 0];
            var a32 = A[2, 1];
            var a33 = A[2, 2];

            var buf = new Matrix(matrix.RowCount, matrix.ColumnCount);

            double b_i_j, b_i_j1, b_i_j2, b_i1_j, b_i1_j1, b_i1_j2, b_i2_j, b_i2_j1, b_i2_j2;

            for (var ic = 0; ic < count; ic++)
            {
                for (var jc = 0; jc < count; jc++)
                {
                    var iStart = ic * 3;
                    var jStart = jc * 3;

                    b_i_j = matrix[iStart, jStart];
                    b_i_j1 = matrix[iStart, jStart + 1];
                    b_i_j2 = matrix[iStart, jStart + 2];

                    b_i1_j = matrix[iStart + 1, jStart];
                    b_i1_j1 = matrix[iStart + 1, jStart + 1];
                    b_i1_j2 = matrix[iStart + 1, jStart + 2];

                    b_i2_j = matrix[iStart + 2, jStart];
                    b_i2_j1 = matrix[iStart + 2, jStart + 1];
                    b_i2_j2 = matrix[iStart + 2, jStart + 2];


                    buf[iStart, jStart] = a11 * (a11 * b_i_j + a21 * b_i1_j + a31 * b_i2_j) + a21 * (a11 * b_i_j1 + a21 * b_i1_j1 + a31 * b_i2_j1) + a31 * (a11 * b_i_j2 + a21 * b_i1_j2 + a31 * b_i2_j2);
                    buf[iStart, jStart + 1] = a12 * (a11 * b_i_j + a21 * b_i1_j + a31 * b_i2_j) + a22 * (a11 * b_i_j1 + a21 * b_i1_j1 + a31 * b_i2_j1) + a32 * (a11 * b_i_j2 + a21 * b_i1_j2 + a31 * b_i2_j2);
                    buf[iStart, jStart + 2] = a13 * (a11 * b_i_j + a21 * b_i1_j + a31 * b_i2_j) + a23 * (a11 * b_i_j1 + a21 * b_i1_j1 + a31 * b_i2_j1) + a33 * (a11 * b_i_j2 + a21 * b_i1_j2 + a31 * b_i2_j2);

                    buf[iStart + 1, jStart] = a11 * (a12 * b_i_j + a22 * b_i1_j + a32 * b_i2_j) + a21 * (a12 * b_i_j1 + a22 * b_i1_j1 + a32 * b_i2_j1) + a31 * (a12 * b_i_j2 + a22 * b_i1_j2 + a32 * b_i2_j2);
                    buf[iStart + 1, jStart + 1] = a12 * (a12 * b_i_j + a22 * b_i1_j + a32 * b_i2_j) + a22 * (a12 * b_i_j1 + a22 * b_i1_j1 + a32 * b_i2_j1) + a32 * (a12 * b_i_j2 + a22 * b_i1_j2 + a32 * b_i2_j2);
                    buf[iStart + 1, jStart + 2] = a13 * (a12 * b_i_j + a22 * b_i1_j + a32 * b_i2_j) + a23 * (a12 * b_i_j1 + a22 * b_i1_j1 + a32 * b_i2_j1) + a33 * (a12 * b_i_j2 + a22 * b_i1_j2 + a32 * b_i2_j2);

                    buf[iStart + 2, jStart] = a11 * (a13 * b_i_j + a23 * b_i1_j + a33 * b_i2_j) + a21 * (a13 * b_i_j1 + a23 * b_i1_j1 + a33 * b_i2_j1) + a31 * (a13 * b_i_j2 + a23 * b_i1_j2 + a33 * b_i2_j2);
                    buf[iStart + 2, jStart + 1] = a12 * (a13 * b_i_j + a23 * b_i1_j + a33 * b_i2_j) + a22 * (a13 * b_i_j1 + a23 * b_i1_j1 + a33 * b_i2_j1) + a32 * (a13 * b_i_j2 + a23 * b_i1_j2 + a33 * b_i2_j2);
                    buf[iStart + 2, jStart + 2] = a13 * (a13 * b_i_j + a23 * b_i1_j + a33 * b_i2_j) + a23 * (a13 * b_i_j1 + a23 * b_i1_j1 + a33 * b_i2_j1) + a33 * (a13 * b_i_j2 + a23 * b_i1_j2 + a33 * b_i2_j2);
                }
            }

            return buf;
        }

        /// <summary>
        /// calculated the C = A * B  * A' where B is 3nX3n matrix and A is 3x3 matrix (read the docs for more info)
        /// </summary>
        /// <param name="A">the A</param>
        /// <param name="B">The B</param>
        /// <returns>C</returns>
        internal static Matrix A_B_At(Matrix A, Matrix B)
        {
            var matrix = B;

            if (!A.IsSquare() || !matrix.IsSquare())
                throw new Exception();

            if (A.RowCount != 3 || matrix.RowCount % 3 != 0)
                throw new Exception();

            var count = matrix.RowCount / 3;

            var a11 = A[0, 0];
            var a21 = A[1, 0];
            var a31 = A[2, 0];

            var a12 = A[0, 1];
            var a22 = A[1, 1];
            var a32 = A[2, 1];

            var a13 = A[0, 2];
            var a23 = A[1, 2];
            var a33 = A[2, 2];

            var buf = new Matrix(matrix.RowCount, matrix.ColumnCount);

            double b_i_j, b_i_j1, b_i_j2, b_i1_j, b_i1_j1, b_i1_j2, b_i2_j, b_i2_j1, b_i2_j2;

            for (var ic = 0; ic < count; ic++)
            {
                for (var jc = 0; jc < count; jc++)
                {
                    var iStart = ic * 3;
                    var jStart = jc * 3;

                    b_i_j = matrix[iStart, jStart];
                    b_i_j1 = matrix[iStart, jStart + 1];
                    b_i_j2 = matrix[iStart, jStart + 2];

                    b_i1_j = matrix[iStart + 1, jStart];
                    b_i1_j1 = matrix[iStart + 1, jStart + 1];
                    b_i1_j2 = matrix[iStart + 1, jStart + 2];

                    b_i2_j = matrix[iStart + 2, jStart];
                    b_i2_j1 = matrix[iStart + 2, jStart + 1];
                    b_i2_j2 = matrix[iStart + 2, jStart + 2];


                    buf[iStart, jStart] = a11 * (a11 * b_i_j + a12 * b_i1_j + a13 * b_i2_j) + a12 * (a11 * b_i_j1 + a12 * b_i1_j1 + a13 * b_i2_j1) + a13 * (a11 * b_i_j2 + a12 * b_i1_j2 + a13 * b_i2_j2);
                    buf[iStart, jStart + 1] = a21 * (a11 * b_i_j + a12 * b_i1_j + a13 * b_i2_j) + a22 * (a11 * b_i_j1 + a12 * b_i1_j1 + a13 * b_i2_j1) + a23 * (a11 * b_i_j2 + a12 * b_i1_j2 + a13 * b_i2_j2);
                    buf[iStart, jStart + 2] = a31 * (a11 * b_i_j + a12 * b_i1_j + a13 * b_i2_j) + a32 * (a11 * b_i_j1 + a12 * b_i1_j1 + a13 * b_i2_j1) + a33 * (a11 * b_i_j2 + a12 * b_i1_j2 + a13 * b_i2_j2);

                    buf[iStart + 1, jStart] = a11 * (a21 * b_i_j + a22 * b_i1_j + a23 * b_i2_j) + a12 * (a21 * b_i_j1 + a22 * b_i1_j1 + a23 * b_i2_j1) + a13 * (a21 * b_i_j2 + a22 * b_i1_j2 + a23 * b_i2_j2);
                    buf[iStart + 1, jStart + 1] = a21 * (a21 * b_i_j + a22 * b_i1_j + a23 * b_i2_j) + a22 * (a21 * b_i_j1 + a22 * b_i1_j1 + a23 * b_i2_j1) + a23 * (a21 * b_i_j2 + a22 * b_i1_j2 + a23 * b_i2_j2);
                    buf[iStart + 1, jStart + 2] = a31 * (a21 * b_i_j + a22 * b_i1_j + a23 * b_i2_j) + a32 * (a21 * b_i_j1 + a22 * b_i1_j1 + a23 * b_i2_j1) + a33 * (a21 * b_i_j2 + a22 * b_i1_j2 + a23 * b_i2_j2);

                    buf[iStart + 2, jStart] = a11 * (a31 * b_i_j + a32 * b_i1_j + a33 * b_i2_j) + a12 * (a31 * b_i_j1 + a32 * b_i1_j1 + a33 * b_i2_j1) + a13 * (a31 * b_i_j2 + a32 * b_i1_j2 + a33 * b_i2_j2);
                    buf[iStart + 2, jStart + 1] = a21 * (a31 * b_i_j + a32 * b_i1_j + a33 * b_i2_j) + a22 * (a31 * b_i_j1 + a32 * b_i1_j1 + a33 * b_i2_j1) + a23 * (a31 * b_i_j2 + a32 * b_i1_j2 + a33 * b_i2_j2);
                    buf[iStart + 2, jStart + 2] = a31 * (a31 * b_i_j + a32 * b_i1_j + a33 * b_i2_j) + a32 * (a31 * b_i_j1 + a32 * b_i1_j1 + a33 * b_i2_j1) + a33 * (a31 * b_i_j2 + a32 * b_i1_j2 + a33 * b_i2_j2);
                }
            }

            return buf;
        }

        /// <summary>
        /// calculated the C = A * B, where A is 3x3 matrix and B is 3x1 vector.
        /// </summary>
        /// <param name="A">the A.</param>
        /// <param name="B">The B.</param>
        /// <returns>C</returns>
        internal static double[] A_B(Matrix A, params double[] B)
        {
            var buf = new double[3];

            buf[0] = A[0, 0] * B[0] + A[0, 1] * B[1] + A[0, 2] * B[2];
            buf[1] = A[1, 0] * B[0] + A[1, 1] * B[1] + A[1, 2] * B[2];
            buf[2] = A[2, 0] * B[0] + A[2, 1] * B[1] + A[2, 2] * B[2];

            return buf;
        }

        /// <summary>
        /// calculated the C = A' * B, where A is 3x3 matrix and B is 3x1 vector.
        /// </summary>
        /// <param name="A">the A.</param>
        /// <param name="B">The B.</param>
        /// <returns>C</returns>
        internal static double[] At_B(Matrix A, params double[] B)
        {
            var buf = new double[3];

            buf[0] = A[0, 0] * B[0] + A[1, 0] * B[1] + A[2, 0] * B[2];
            buf[1] = A[0, 1] * B[0] + A[1, 1] * B[1] + A[2, 1] * B[2];
            buf[2] = A[0, 2] * B[0] + A[1, 2] * B[1] + A[2, 2] * B[2];

            return buf;
        }
    }
}
