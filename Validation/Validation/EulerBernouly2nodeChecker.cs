using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Validation
{
    public class EulerBernouly2nodeChecker
    {
        public static void Test()
        {
            var dir = BeamDirection.Z;

            var l = 2;

            var node1 = new Node(0, 0, 0);
            var node2 = new Node(l, 0, 0);

            var elm = new BarElement(node1, node2);

            var h1 = new EulerBernoulliBeamHelper(dir, elm);

            var epsilon = 1e-8;

            for (var xi = -1.0; xi <= 1; xi += 0.1)
            {
                var n1 = h1.GetNMatrixAt(elm, xi);
                var n2 = EulerBernoulliBeamHelper_new.GetNMatrixAt(xi, l, dir);

                var diff = n1 - n2;

                var d1 = diff.ExtractRow(0);
                var d2 = n2.ExtractRow(1);
                var d3 = diff.ExtractRow(2);
                var d4 = diff.ExtractRow(3);

                var norm = diff.L1Norm();

                if (Math.Abs(norm) > epsilon)
                {
                    throw new Exception();
                }
            }

            Console.WriteLine("Done");
        }


        public static void Test2()
        {
            var dir = BeamDirection.Y;

            var l = 2;

            var node1 = new Node(0, 0, 0);
            var node2 = new Node(l, 0, 0);

            var elm = new BarElement(node1, node2);

            var h1 = new EulerBernoulliBeamHelper(dir, elm);

            var epsilon = 1e-8;

            for (var xi = -1.0; xi <= 1; xi += 0.1)
            {
                var n1 = h1.GetNMatrixAt(elm, xi);
                var n2 = EulerBernoulliBeamHelper_new.GetNMatrixAt(xi, l, dir);

                var diff = n1 - n2;

                var d1 = diff.ExtractRow(0);
                var d2 = n2.ExtractRow(1);
                var d3 = diff.ExtractRow(2);
                var d4 = diff.ExtractRow(3);

                var norm = diff.L1Norm();

                if (Math.Abs(norm) > epsilon)
                {
                    throw new Exception();
                }
            }

            Console.WriteLine("Done");
        }


        public static void Test3()
        {
            var l = 2;

            var node1 = new Node(0, 0, 0);
            var node2 = new Node(l, 0, 0);

            var elm = new BarElement(node1, node2);

            var h1 = new TrussHelper(elm);

            var epsilon = 1e-8;

            for (var xi = -1.0; xi <= 1; xi += 0.1)
            {
                var n1 = h1.GetNMatrixAt(elm, xi);
                var n2 = TrussHelper2Node.GetNMatrixAt(xi, l);

                var diff = n1 - n2;

                var norm = diff.L1Norm();

                if (Math.Abs(norm) > epsilon)
                {
                    throw new Exception();
                }
            }

            Console.WriteLine("Done");
        }

        public static void Test4()
        {
            var l = 2;

            var node1 = new Node(0, 0, 0);
            var node2 = new Node(l, 0, 0);

            var elm = new BarElement(node1, node2);

            var h1 = new ShaftHelper(elm);

            var epsilon = 1e-8;

            for (var xi = -1.0; xi <= 1; xi += 0.1)
            {
                var n1 = h1.GetNMatrixAt(elm, xi);
                var n2 = ShaftHelper2Node.GetNMatrixAt(xi, l);

                var diff = n1 - n2;

                var norm = diff.L1Norm();

                if (Math.Abs(norm) > epsilon)
                {
                    throw new Exception();
                }
            }

            Console.WriteLine("Done");
        }

        public static void Test5()
        {
            var l = 2;

            var node1 = new Node(0, 0, 0);
            var node2 = new Node(l, 0, 0);

            var elm = new BarElement(node1, node2);

            var dir = BeamDirection.Y;

            var h1 = new EulerBernoulliBeamHelper(dir, elm);
            var h2 = new EulerBernoulliBeamHelper(dir, elm);

            var epsilon = 1e-8;

            for (var xi = -1.0; xi <= 1; xi += 0.1)
            {
                var v1 = h1.GetNMatrixAt(elm, xi);
                var v2 = h2.GetNMatrixAt(elm, xi);

                var diff = v1 - v2;

                var norm = diff.L1Norm();

                if (Math.Abs(norm) > epsilon)
                {
                    throw new Exception();
                }
            }

            Console.WriteLine("Done");
        }
    }
}
