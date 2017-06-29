using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Sections;

namespace BriefFiniteElementNet.Validation
{
    public class MatrixTransformValidator
    {
        public static void Validate()
        {
            var dim = 42;

            var B = Matrix.RandomMatrix(dim, dim);// elm.GetLocalStifnessMatrix();

            var A = Matrix.RandomMatrix(3, 3);

            var sp = System.Diagnostics.Stopwatch.StartNew();

            sp.Restart();

            var at_b_a_mgr = TransformManagerL2G.At_B_A(A, B);
            var a_b_at_mgr = TransformManagerL2G.A_B_At(A, B);

            sp.Stop();

            System.Console.WriteLine("Optimised took {0} ms", sp.ElapsedMilliseconds);

            var AA = Matrix.DiagonallyRepeat(A, dim / 3);

            sp.Restart();
            var at_b_a_dir = AA.Transpose() * B * AA;
            var a_b_at_dir = AA * B * AA.Transpose();

            System.Console.WriteLine("Non optimised took {0} ms", sp.ElapsedMilliseconds);

            var d1 = (at_b_a_mgr - at_b_a_dir).ToArray().Max(i => Math.Abs(i));
            var d2 = (a_b_at_mgr - a_b_at_dir).ToArray().Max(i => Math.Abs(i));

            System.Console.WriteLine("ERR = {0} for {1}x{1} matrix", d1, dim);
            System.Console.WriteLine("ERR = {0} for {1}x{1} matrix", d2, dim);
        }
    }
}
