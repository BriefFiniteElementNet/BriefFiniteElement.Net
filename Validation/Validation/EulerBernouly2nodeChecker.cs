using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Mathh;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
                var n2 = EulerBernoulliBeamHelper2Node.GetNMatrixAt(xi, l, dir);

                var diff = n1 - n2;

                var d1 = diff.ExtractRow(0);
                var d2 = diff.ExtractRow(1);
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
                var n2 = EulerBernoulliBeamHelper2Node.GetNMatrixAt(xi, l, dir);

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
                var n2 = TrussHelper2Node.GetNMatrixAt(xi, l, DofConstraint.Fixed, DofConstraint.Fixed);

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
                var n2 = ShaftHelper2Node.GetNMatrixAt(xi, l, DofConstraint.Fixed, DofConstraint.Fixed);

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

        public static void GenerateShapefunctionCode()
        {
            //generate code for two node partial
            var l = 2;

            var node1 = new Node(0, 0, 0);
            var node2 = new Node(l, 0, 0);

            var elm = new BarElement(node1, node2);

            var dir = BeamDirection.Y;

            
            var h1 = new EulerBernoulliBeamHelper(dir, elm);


            SingleVariablePolynomial[] nss, mss;

            var perm = new List<bool[]>();
            var perm2 = new List<string>();
            
            {
                var vals = new DofConstraint[] { DofConstraint.Fixed, DofConstraint.Released };
                var nums = new int[] { 0, 1, 2, 3 };
                
                for (var i = 0; i < 16; i++)
                {
                    var t = ByteToBools((byte)i);
                    perm.Add(t);

                    var t2 = string.Join("", t.Select(ii => ii ? "1" : "0"));
                    perm2.Add(t2);
                }


                if (perm2.Distinct().Count() != 16)
                    throw new Exception();


            }

            var sb = new StringBuilder();

            foreach (var i in perm)
            {
                elm.NodalReleaseConditions[0] = elm.NodalReleaseConditions[1] = Constraints.Fixed;

                var D0 = elm.NodalReleaseConditions[0].DZ = i[0] ? DofConstraint.Released : DofConstraint.Fixed;
                var R0 = elm.NodalReleaseConditions[0].RY = i[1] ? DofConstraint.Released : DofConstraint.Fixed;
                var D1 = elm.NodalReleaseConditions[1].DZ = i[2] ? DofConstraint.Released : DofConstraint.Fixed;
                var R1 = elm.NodalReleaseConditions[1].RY = i[3] ? DofConstraint.Released : DofConstraint.Fixed;

                var conds = elm.NodalReleaseConditions;

                var d0 = D0 == DofConstraint.Fixed ? 1 : 0;
                var r0 = R0 == DofConstraint.Fixed ? 1 : 0;
                var d1 = D1 == DofConstraint.Fixed ? 1 : 0;
                var r1 = R1 == DofConstraint.Fixed ? 1 : 0;

                int num = d0 * 8 + r0 * 4 + d1 * 2 + r1;


                //var dofs = new [] { conds[0].DY, conds[0].RZ, conds[1].DY, conds[1].RZ };
                //var bools = dofs.Select(ii => ii == DofConstraint.Fixed).ToArray();

                //sb.AppendFormat("if (D0.Is{0}() &&  R0.Is{1}() && D1.Is{2}() &&  R1.Is{3}()))", conds[0].DY, conds[0].RZ, conds[1].DY, conds[1].RZ);
                sb.AppendFormat("case {0}:", num);
                sb.AppendLine("{");


                h1.GetShapeFunctions(elm, out nss, out mss);

                var n1 = nss[0];
                var n2 = nss[1];

                var m1 = mss[0];
                var m2 = mss[1];

                sb.AppendFormat("n1= {0};\r\n", n1.ToCodeString());
                sb.AppendFormat("n2= {0};\r\n", n2.ToCodeString());
                sb.AppendFormat("m1= {0};\r\n", m1.ToCodeString());
                sb.AppendFormat("m2= {0};\r\n", m2.ToCodeString());
                sb.AppendLine("break;");
                sb.AppendLine("}");
                sb.AppendLine();
            }
           
           
            var t3 = sb.ToString();

            Console.WriteLine("Done");
        }

        static bool[] ByteToBools(byte value)
        {
            var values = new BitArray(new byte[] { value });

            var buf = new bool[8];

            values.CopyTo(buf, 0);
            Array.Resize(ref buf, 4);

            return buf;
        }



        public static void Check2NodeShapeFunctionYDir()
        {
            var l = 2;

            var node1 = new Node(0, 0, 0);
            var node2 = new Node(l, 0, 0);

            var elm = new BarElement(node1, node2);

            var dir = BeamDirection.Y;


            var h1 = new EulerBernoulliBeamHelper(dir, elm);


            SingleVariablePolynomial[] nssExpected, mssExpected;
            SingleVariablePolynomial[] nssCurrent, mssCurrent;

            var perm = new List<bool[]>();

            {
                var vals = new DofConstraint[] { DofConstraint.Fixed, DofConstraint.Released };
                var nums = new int[] { 0, 1, 2, 3 };

                for (var i = 0; i < 16; i++)
                {
                    var t = ByteToBools((byte)i);
                    perm.Add(t);

                }
            }

            var sb = new StringBuilder();

            foreach (var i in perm)
            {
                elm.NodalReleaseConditions[0] = elm.NodalReleaseConditions[1] = Constraints.Fixed;

                var D0 = elm.NodalReleaseConditions[0].DZ = i[0] ? DofConstraint.Released : DofConstraint.Fixed;
                var R0 = elm.NodalReleaseConditions[0].RY = i[1] ? DofConstraint.Released : DofConstraint.Fixed;
                var D1 = elm.NodalReleaseConditions[1].DZ = i[2] ? DofConstraint.Released : DofConstraint.Fixed;
                var R1 = elm.NodalReleaseConditions[1].RY = i[3] ? DofConstraint.Released : DofConstraint.Fixed;

                var conds = elm.NodalReleaseConditions;

                var d0 = D0 == DofConstraint.Fixed ? 1 : 0;
                var r0 = R0 == DofConstraint.Fixed ? 1 : 0;
                var d1 = D1 == DofConstraint.Fixed ? 1 : 0;
                var r1 = R1 == DofConstraint.Fixed ? 1 : 0;

                int num = d0 * 8 + r0 * 4 + d1 * 2 + r1;

                sb.AppendFormat("case {0}:", num);
                sb.AppendLine("{");


                h1.GetShapeFunctions(elm, out nssExpected, out mssExpected);

                EulerBernouly2NodeShapeFunction.GetShapeFunctions(l, D0, R0, D1, R1, dir, out nssCurrent, out mssCurrent);

                for (int j = 0; j < nssCurrent.Length; j++)
                {
                    var nc = nssCurrent[j];
                    var ne = nssExpected[j];

                    if(!nc.EqualsPolynomial(ne,1e-6))
                    {
                        throw new Exception();
                    }
                }
            }

        }


        public static void Check2NodeShapeFunctionZDir()
        {
            var l = 2;

            var node1 = new Node(0, 0, 0);
            var node2 = new Node(l, 0, 0);

            var elm = new BarElement(node1, node2);

            var dir = BeamDirection.Z;


            var h1 = new EulerBernoulliBeamHelper(dir, elm);


            SingleVariablePolynomial[] nssExpected, mssExpected;
            SingleVariablePolynomial[] nssCurrent, mssCurrent;

            var perm = new List<bool[]>();

            {
                var vals = new DofConstraint[] { DofConstraint.Fixed, DofConstraint.Released };
                var nums = new int[] { 0, 1, 2, 3 };

                for (var i = 0; i < 16; i++)
                {
                    var t = ByteToBools((byte)i);
                    perm.Add(t);

                }
            }

            var sb = new StringBuilder();

            foreach (var i in perm)
            {
                elm.NodalReleaseConditions[0] = elm.NodalReleaseConditions[1] = Constraints.Fixed;

                var D0 = elm.NodalReleaseConditions[0].DY = i[0] ? DofConstraint.Released : DofConstraint.Fixed;
                var R0 = elm.NodalReleaseConditions[0].RZ = i[1] ? DofConstraint.Released : DofConstraint.Fixed;
                var D1 = elm.NodalReleaseConditions[1].DY = i[2] ? DofConstraint.Released : DofConstraint.Fixed;
                var R1 = elm.NodalReleaseConditions[1].RZ = i[3] ? DofConstraint.Released : DofConstraint.Fixed;

                var conds = elm.NodalReleaseConditions;

                var d0 = D0 == DofConstraint.Fixed ? 1 : 0;
                var r0 = R0 == DofConstraint.Fixed ? 1 : 0;
                var d1 = D1 == DofConstraint.Fixed ? 1 : 0;
                var r1 = R1 == DofConstraint.Fixed ? 1 : 0;

                int num = d0 * 8 + r0 * 4 + d1 * 2 + r1;

                sb.AppendFormat("case {0}:", num);
                sb.AppendLine("{");


                h1.GetShapeFunctions(elm, out nssExpected, out mssExpected);

                EulerBernouly2NodeShapeFunction.GetShapeFunctions(l, D0, R0, D1, R1, dir, out nssCurrent, out mssCurrent);

                for (int j = 0; j < nssCurrent.Length; j++)
                {
                    var nc = nssCurrent[j];
                    var ne = nssExpected[j];

                    if (!nc.EqualsPolynomial(ne, 1e-6))
                    {
                        throw new Exception();
                    }
                }
            }

        }

    }
}
