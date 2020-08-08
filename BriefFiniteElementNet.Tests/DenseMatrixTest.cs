using System;
using System.Linq;
using NUnit.Framework;

namespace BriefFiniteElementNet.Tests
{

    public class DenseMatrixTest
    {
        [Test]
        public void TestGaussElimination()
        {
            var n = 4;

            //initiate matrix
            var matrix = new Matrix(n, n);
            var rightSide = new double[n];

            //fill matrix with sample value
            for (var i = 0; i < n * n; i++)
                matrix.CoreArray[i] = i + Math.Sqrt(i);

            for (var i = 0; i <  n; i++)
                rightSide[i] = i;

            var res = matrix.Solve(rightSide);

            var zero = matrix * new Matrix(res)- new Matrix(rightSide);

            var err = zero.CoreArray.Max(i => Math.Abs(i));

            Assert.IsTrue(err < 1e-10);

            }

        [Test]
        public void TestInverse2()
        {
            var n = 4;

            //initiate matrix
            var matrix = new Matrix(n, n);

            //fill matrix with sample value
            for (var i = 0; i < n * n; i++)
                matrix.CoreArray[i] = i + Math.Sqrt(i);

            var inverse = matrix.Inverse2();//find inverse by target method

            var eye1 = inverse * matrix;//should be I
            var eye2 = matrix * inverse;//should be I too

            var maxErr1 = (eye1 - Matrix.Eye(n)).CoreArray.Max(i => Math.Abs(i));
            var maxErr2 = (eye2 - Matrix.Eye(n)).CoreArray.Max(i => Math.Abs(i));

            Assert.IsTrue(maxErr1 < 1e-10);
            Assert.IsTrue(maxErr2 < 1e-10);
        }
    }
}
