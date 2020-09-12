using System;
using System.Linq;
using NUnit.Framework;

namespace BriefFiniteElementNet.Tests
{
    public class DenseMatrixTest
    {
        [Test]
        public void TestScale()
        {
            var A = Matrix.OfRowMajor(2, 2, new double[]
            {
                1.0, 2.0,
                3.0, 4.0
            });

            A.Scale(2.0);

            Assert.AreEqual(2.0, A.At(0, 0));
            Assert.AreEqual(4.0, A.At(0, 1));
            Assert.AreEqual(6.0, A.At(1, 0));
            Assert.AreEqual(8.0, A.At(1, 1));
        }

        [Test]
        public void TestScaleRow()
        {
            var A = Matrix.OfRowMajor(2, 2, new double[]
            {
                1.0, 2.0,
                3.0, 4.0,
                5.0, 6.0
            });

            A.ScaleRow(0, 2.0);

            Assert.AreEqual(2.0, A.At(0, 0));
            Assert.AreEqual(4.0, A.At(0, 1));
            Assert.AreEqual(3.0, A.At(1, 0));

            A.ScaleRow(1, 0.5);

            Assert.AreEqual(2.0, A.At(0, 0));
            Assert.AreEqual(1.5, A.At(1, 0));
            Assert.AreEqual(2.0, A.At(1, 1));
        }

        [Test]
        public void TestTransposeMultiply()
        {
            var A = Matrix.OfRowMajor(2, 2, new double[]
            {
                1.0, 2.0,
                3.0, 4.0
            });

            var B = Matrix.OfRowMajor(2, 2, new double[]
            {
                5.0, 6.0,
                7.0, 8.0
            }).AsMatrix();

            var expected = A.Transpose().Multiply(B);
            var actual = new Matrix(2, 2);

            A.TransposeMultiply(B, actual);

            Assert.IsTrue(expected.Equals(actual));
        }

        [Test]
        public void TestTransposeMultiplyNonSquare()
        {
            var A = Matrix.OfRowMajor(2, 3, new double[]
            {
                1.0, 2.0, 3.0,
                4.0, 5.0, 6.0,
            });

            var B = Matrix.OfRowMajor(2, 1, new double[]
            {
                7.0,
                8.0
            }).AsMatrix();

            var expected = A.Transpose().Multiply(B);
            var actual = new Matrix(3, 1);

            A.TransposeMultiply(B, actual);

            Assert.IsTrue(expected.Equals(actual));
        }

        [Test]
        public void TestSolve()
        {
            var n = 4;

            // initiate matrix
            var A = new Matrix(n, n);
            var b = new double[n];

            // fill matrix with sample value
            for (var i = 0; i < n * n; i++)
            {
                A.Values[i] = i + Math.Sqrt(i);
            }

            for (var i = 0; i < n; i++)
            {
                b[i] = i;
            }

            var x = A.Solve(b);

            // compute residual
            A.Multiply(1.0, x, -1.0, b);

            Assert.IsTrue(b.Max(i => Math.Abs(i)) < 1e-10);
        }

        [Test]
        public void TestInverse()
        {
            var n = 4;

            //initiate matrix
            var matrix = new Matrix(n, n);

            //fill matrix with sample value
            for (var i = 0; i < n * n; i++)
                matrix.Values[i] = i + Math.Sqrt(i);

            var inverse = matrix.Inverse();//find inverse by target method

            var eye1 = inverse * matrix;//should be I
            var eye2 = matrix * inverse;//should be I too

            var maxErr1 = (eye1 - Matrix.Eye(n)).Values.Max(i => Math.Abs(i));
            var maxErr2 = (eye2 - Matrix.Eye(n)).Values.Max(i => Math.Abs(i));

            Assert.IsTrue(maxErr1 < 1e-10);
            Assert.IsTrue(maxErr2 < 1e-10);
        }
    }
}
