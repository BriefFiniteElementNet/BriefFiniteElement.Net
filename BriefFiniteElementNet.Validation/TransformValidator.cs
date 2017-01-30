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
            var elm = new BarElement(new Node(0, 0, 0), new Node(3, 5, 7));

            elm.Behavior = BarElementBehaviours.FullFrame;
            elm.Material = new UniformBarMaterial(100, 100, 0.25);
            elm.Section = new UniformParametricBarElementCrossSection() { A = 1, Iy = 2, Iz = 3, J = 4 };

            var dim = 120;

            var sp = System.Diagnostics.Stopwatch.StartNew();


            var kl = Matrix.RandomMatrix(dim, dim);// elm.GetLocalStifnessMatrix();

            var t = elm.GetTransformationMatrix();

            var mgr = LocalGlobalTransformManager.MakeFromTransformationMatrix(t);

            sp.Restart();

            var g = mgr.TransformLocalToGlobal(kl);
            sp.Stop();

            System.Console.WriteLine("TransformManager took {0} ms", sp.ElapsedMilliseconds);

            var t_a = Matrix.DiagonallyRepeat(t, dim / 3);

            sp.Restart();
            var g2 = t_a.Transpose() * kl * t_a;
            System.Console.WriteLine("Usual took {0} ms", sp.ElapsedMilliseconds);


            var d = (g2 - g).ToArray().Max(i => Math.Abs(i));

            System.Console.WriteLine("ERR = {0} for {1}x{1} matrix", d, dim);
        }
    }
}
