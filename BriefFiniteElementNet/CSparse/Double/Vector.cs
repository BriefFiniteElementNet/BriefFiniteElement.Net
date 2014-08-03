
using System;

namespace BriefFiniteElementNet.CSparse.Double
{
    /// <summary>
    /// Vector helper methods.
    /// </summary>
    public static class Vector
    {
        /// <summary>
        /// Copy one vector to another.
        /// </summary>
        /// <param name="src">The source array.</param>
        /// <param name="dst">The destination array.</param>
        public static void Copy(double[] src, double[] dst)
        {
            Buffer.BlockCopy(src, 0, dst, 0, src.Length * Constants.SizeOfDouble);
        }

        /// <summary>
        /// Copy one vector to another.
        /// </summary>
        /// <param name="src">The source array.</param>
        /// <param name="dst">The destination array.</param>
        /// <param name="n">Number of values to copy.</param>
        public static void Copy(double[] src, double[] dst, int n)
        {
            Buffer.BlockCopy(src, 0, dst, 0, n * Constants.SizeOfDouble);
        }

        /// <summary>
        /// Create a new vector.
        /// </summary>
        public static double[] Create(int length, double value)
        {
            double[] result = new double[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = value;
            }

            return result;
        }

        /// <summary>
        /// Clone the given vector.
        /// </summary>
        public static double[] Clone(double[] src)
        {
            double[] result = new double[src.Length];

            Buffer.BlockCopy(src, 0, result, 0, src.Length * Constants.SizeOfDouble);

            return result;
        }

        /// <summary>
        /// Set vector values to zero.
        /// </summary>
        public static void Clear(double[] x)
        {
            Array.Clear(x, 0, x.Length);
        }

        /// <summary>
        /// Computes the dot product of two vectors.
        /// </summary>
        public static double DotProduct(double[] x, double[] y)
        {
            int length = x.Length;

            double result = 0.0;

            for (int i = 0; i < length; i++)
            {
                result += x[i] * y[i];
            }

            return result;
        }

        /// <summary>
        /// Computes the norm of a vector, sqrt( x' * x ).
        /// </summary>
        public static double Norm(double[] x)
        {
            int length = x.Length;

            double result = 0.0;

            for (int i = 0; i < length; ++i)
            {
                result += x[i] * x[i];
            }

            return Math.Sqrt(result);
        }

        /// <summary>
        /// Computes the norm of a vector avoiding overflow, sqrt( x' * x ).
        /// </summary>
        public static double NormRobust(double[] x)
        {
            int length = x.Length;

            double scale = 0.0, ssq = 1.0;

            for (int i = 0; i < length; ++i)
            {
                if (x[i] != 0.0)
                {
                    double absxi = Math.Abs(x[i]);
                    if (scale < absxi)
                    {
                        ssq = 1.0 + ssq * (scale / absxi) * (scale / absxi);
                        scale = absxi;
                    }
                    else
                    {
                        ssq += (absxi / scale) * (absxi / scale);
                    }
                }
            }

            return scale * Math.Sqrt(ssq);
        }

        /// <summary>
        /// Scales a vector by a given factor, x = alpha * x.
        /// </summary>
        public static void Scale(double alpha, double[] x)
        {
            int length = x.Length;

            for (int i = 0; i < length; i++)
            {
                x[i] *= alpha;
            }
        }

        /// <summary>
        /// Add a scaled vector to another vector, y = y + alpha * x.
        /// </summary>
        public static void Axpy(double alpha, double[] x, double[] y)
        {
            int length = x.Length;

            for (int i = 0; i < length; i++)
            {
                y[i] += alpha * x[i];
            }
        }

        /// <summary>
        /// Add two scaled vectors, z = alpha * x + beta * y.
        /// </summary>
        public static void Add(double alpha, double[] x, double beta, double[] y, double[] z)
        {
            int length = x.Length;

            for (int i = 0; i < length; i++)
            {
                z[i] = alpha * x[i] + beta * y[i];
            }
        }
    }
}
