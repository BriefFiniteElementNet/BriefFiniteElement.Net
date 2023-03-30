using BriefFiniteElementNet.Mathh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.ElementHelpers.BarHelpers
{
    public static class EulerBernouly2NodeShapeFunction
    {
        public static void GetShapeFunctions(double l, DofConstraint D0, DofConstraint R0, DofConstraint D1, DofConstraint R1, BeamDirection dir,
            out SingleVariablePolynomial[] nss, out SingleVariablePolynomial[] mss)
        {
            //unstable conditions:
            // Todo
            SingleVariablePolynomial n1, n2, m1, m2;

            n1 = n2 = m1 = m2 = null;


            var d0 = D0 == DofConstraint.Fixed ? 1 : 0;
            var r0 = R0 == DofConstraint.Fixed ? 1 : 0;
            var d1 = D1 == DofConstraint.Fixed ? 1 : 0;
            var r1 = R1 == DofConstraint.Fixed ? 1 : 0;

            int num = d0 * 8 + r0 * 4 + d1 * 2 + r1;

            switch (num)//prior generated code! have alook at Validation.EulerBernouly2nodeChecker.GenerateShapefunctionCode()
            {
                case 15:
                    {
                        n1 = new SingleVariablePolynomial(0.25, 0, -0.75, 0.5); ;
                        n2 = new SingleVariablePolynomial(-0.25, 0, 0.75, 0.5); ;
                        m1 = new SingleVariablePolynomial(0.25, -0.25, -0.25, 0.25); ;
                        m2 = new SingleVariablePolynomial(0.25, 0.25, -0.25, -0.25); ;
                        break;
                    }

                case 7:
                    {
                        n1 = new SingleVariablePolynomial();
                        n2 = new SingleVariablePolynomial(0, 0, 0, 1);
                        m1 = new SingleVariablePolynomial(0, -0.25, 0.5, -0.25);
                        m2 = new SingleVariablePolynomial(0, 0.25, 0.5, -0.75);
                        break;
                    }

                case 11:
                    {
                        n1 = new SingleVariablePolynomial(0.0625, 0.1875, -0.5625, 0.3125); ;
                        n2 = new SingleVariablePolynomial(-0.0625, -0.1875, 0.5625, 0.6875); ;
                        m1 = new SingleVariablePolynomial(); ;
                        m2 = new SingleVariablePolynomial(0.125, 0.375, -0.125, -0.375); ;
                        break;
                    }

                case 3:
                    {
                        n1 = new SingleVariablePolynomial();
                        n2 = new SingleVariablePolynomial(0, 0, 0, 1);
                        m1 = new SingleVariablePolynomial();
                        m2 = new SingleVariablePolynomial(0, 0, 1, -1);
                        break;
                    }

                case 13:
                    {
                        n1 = new SingleVariablePolynomial(0, 0, 0, 1); ;
                        n2 = new SingleVariablePolynomial(); ;
                        m1 = new SingleVariablePolynomial(0, -0.25, 0.5, 0.75); ;
                        m2 = new SingleVariablePolynomial(0, 0.25, 0.5, 0.25); ;
                        break;
                    }

                case 5:
                    {
                        n1 = new SingleVariablePolynomial(0, 0, 0, 1); ;
                        n2 = new SingleVariablePolynomial(); ;
                        m1 = new SingleVariablePolynomial(0, -0.25, 0.5, 0.75); ;
                        m2 = new SingleVariablePolynomial(0, 0.25, 0.5, 0.25); ;
                        break;
                    }

                case 9:
                    {
                        n1 = new SingleVariablePolynomial(0, 0, 0, 1); ;
                        n2 = new SingleVariablePolynomial(); ;
                        m1 = new SingleVariablePolynomial(); ;
                        m2 = new SingleVariablePolynomial(0, 0, 1, 1); ;
                        break;
                    }

                case 1:
                    {
                        n1 = new SingleVariablePolynomial(0, 0, 0, 1); ;
                        n2 = new SingleVariablePolynomial(); ;
                        m1 = new SingleVariablePolynomial(); ;
                        m2 = new SingleVariablePolynomial(0, 0, 1, 1); ;
                        break;
                    }

                case 14:
                    {
                        n1 = new SingleVariablePolynomial(0.0625, -0.1875, -0.5625, 0.6875); ;
                        n2 = new SingleVariablePolynomial(-0.0625, 0.1875, 0.5625, 0.3125); ;
                        m1 = new SingleVariablePolynomial(0.125, -0.375, -0.125, 0.375); ;
                        m2 = new SingleVariablePolynomial(); ;
                        break;
                    }

                case 6:
                    {
                        n1 = new SingleVariablePolynomial(); ;
                        n2 = new SingleVariablePolynomial(0, 0, 0, 1); ;
                        m1 = new SingleVariablePolynomial(0, 0, 1, -1); ;
                        m2 = new SingleVariablePolynomial(); ;
                        break;
                    }

                case 10:
                    {
                        n1 = new SingleVariablePolynomial(0, 0, -0.5, 0.5); ;
                        n2 = new SingleVariablePolynomial(0, 0, 0.5, 0.5); ;
                        m1 = new SingleVariablePolynomial(); ;
                        m2 = new SingleVariablePolynomial(); ;
                        break;
                    }

                case 2:
                    {
                        n1 = new SingleVariablePolynomial(0, 0, -0.5, 0.5); ;
                        n2 = new SingleVariablePolynomial(0, 0, 0.5, 0.5); ;
                        m1 = new SingleVariablePolynomial(); ;
                        m2 = new SingleVariablePolynomial(); ;
                        break;
                    }

                case 12:
                    {
                        n1 = new SingleVariablePolynomial(0, 0, 0, 1); ;
                        n2 = new SingleVariablePolynomial(); ;
                        m1 = new SingleVariablePolynomial(0, 0, 1, 1); ;
                        m2 = new SingleVariablePolynomial(); ;
                        break;
                    }

                case 4:
                    {
                        n1 = new SingleVariablePolynomial(0, 0, 0, 1); ;
                        n2 = new SingleVariablePolynomial(); ;
                        m1 = new SingleVariablePolynomial(0, 0, 1, 1); ;
                        m2 = new SingleVariablePolynomial(); ;
                        break;
                    }

                case 8:
                    {
                        n1 = new SingleVariablePolynomial(0, 0, 0, 1);
                        n2 = new SingleVariablePolynomial();
                        m1 = new SingleVariablePolynomial(0, 0, 1, 1);
                        m2 = new SingleVariablePolynomial();
                        break;
                    }

                case 0:
                    {
                        n1 = new SingleVariablePolynomial(0, 0, 0, 1);
                        n2 = new SingleVariablePolynomial();
                        m1 = new SingleVariablePolynomial(0, 0, 1, 1);
                        m2 = new SingleVariablePolynomial();
                        break;
                    }
                default:
                    throw new Exception();

            }

            m1.MultiplyByConstant(l / 2);
            m2.MultiplyByConstant(l / 2);



            if (dir == BeamDirection.Y)
            {
                m1.MultiplyByConstant(-1);
                m2.MultiplyByConstant(-1);
            }


            nss = new SingleVariablePolynomial[] { n1, n2 };
            mss = new SingleVariablePolynomial[] { m1, m2 };

        }

    }
}
