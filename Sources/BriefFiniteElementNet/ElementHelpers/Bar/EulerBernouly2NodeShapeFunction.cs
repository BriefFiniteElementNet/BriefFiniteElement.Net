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

            double[] _n1, _n2, _m1, _m2;

            switch (num)//prior generated code! have alook at Validation.EulerBernouly2nodeChecker.GenerateShapefunctionCode()
            {
                case 15:
                    {
                        _n1 = new double[] { 0.25, 0, -0.75, 0.5 }; ;
                        _n2 = new double[] { -0.25, 0, 0.75, 0.5 }; ;
                        _m1 = new double[] { 0.25, -0.25, -0.25, 0.25 }; ;
                        _m2 = new double[] { 0.25, 0.25, -0.25, -0.25 }; ;
                        break;
                    }

                case 7:
                    {
                        _n1 = new double[] { };
                        _n2 = new double[] { 0, 0, 0, 1 };
                        _m1 = new double[] { 0, -0.25, 0.5, -0.25 };
                        _m2 = new double[] { 0, 0.25, 0.5, -0.75 };
                        break;
                    }

                case 11:
                    {
                        _n1 = new double[] { 0.0625, 0.1875, -0.5625, 0.3125 }; ;
                        _n2 = new double[] { -0.0625, -0.1875, 0.5625, 0.6875 }; ;
                        _m1 = new double[] { }; ;
                        _m2 = new double[] { 0.125, 0.375, -0.125, -0.375 }; ;
                        break;
                    }

                case 3:
                    {
                        _n1 = new double[] { };
                        _n2 = new double[] { 0, 0, 0, 1 };
                        _m1 = new double[] { };
                        _m2 = new double[] { 0, 0, 1, -1 };
                        break;
                    }

                case 13:
                    {
                        _n1 = new double[] { 0, 0, 0, 1 }; ;
                        _n2 = new double[] { }; ;
                        _m1 = new double[] { 0, -0.25, 0.5, 0.75 }; ;
                        _m2 = new double[] { 0, 0.25, 0.5, 0.25 }; ;
                        break;
                    }

                case 5:
                    {
                        _n1 = new double[] { 0, 0, 0, 1 }; ;
                        _n2 = new double[] { }; ;
                        _m1 = new double[] { 0, -0.25, 0.5, 0.75 }; ;
                        _m2 = new double[] { 0, 0.25, 0.5, 0.25 }; ;
                        break;
                    }

                case 9:
                    {
                        _n1 = new double[] { 0, 0, 0, 1 }; ;
                        _n2 = new double[] { }; ;
                        _m1 = new double[] { }; ;
                        _m2 = new double[] { 0, 0, 1, 1 }; ;
                        break;
                    }

                case 1:
                    {
                        _n1 = new double[] { 0, 0, 0, 1 }; ;
                        _n2 = new double[] { }; ;
                        _m1 = new double[] { }; ;
                        _m2 = new double[] { 0, 0, 1, 1 }; ;
                        break;
                    }

                case 14:
                    {
                        _n1 = new double[] { 0.0625, -0.1875, -0.5625, 0.6875 }; ;
                        _n2 = new double[] { -0.0625, 0.1875, 0.5625, 0.3125 }; ;
                        _m1 = new double[] { 0.125, -0.375, -0.125, 0.375 }; ;
                        _m2 = new double[] { }; ;
                        break;
                    }

                case 6:
                    {
                        _n1 = new double[] { }; ;
                        _n2 = new double[] { 0, 0, 0, 1 }; ;
                        _m1 = new double[] { 0, 0, 1, -1 }; ;
                        _m2 = new double[] { }; ;
                        break;
                    }

                case 10:
                    {
                        _n1 = new double[] { 0, 0, -0.5, 0.5 }; ;
                        _n2 = new double[] { 0, 0, 0.5, 0.5 }; ;
                        _m1 = new double[] { }; ;
                        _m2 = new double[] { }; ;
                        break;
                    }

                case 2:
                    {
                        _n1 = new double[] { 0, 0, -0.5, 0.5 }; ;
                        _n2 = new double[] { 0, 0, 0.5, 0.5 }; ;
                        _m1 = new double[] { }; ;
                        _m2 = new double[] { }; ;
                        break;
                    }

                case 12:
                    {
                        _n1 = new double[] { 0, 0, 0, 1 }; ;
                        _n2 = new double[] { }; ;
                        _m1 = new double[] { 0, 0, 1, 1 }; ;
                        _m2 = new double[] { }; ;
                        break;
                    }

                case 4:
                    {
                        _n1 = new double[] { 0, 0, 0, 1 }; ;
                        _n2 = new double[] { }; ;
                        _m1 = new double[] { 0, 0, 1, 1 }; ;
                        _m2 = new double[] { }; ;
                        break;
                    }

                case 8:
                    {
                        _n1 = new double[] { 0, 0, 0, 1 };
                        _n2 = new double[] { };
                        _m1 = new double[] { 0, 0, 1, 1 };
                        _m2 = new double[] { };
                        break;
                    }

                case 0:
                    {
                        _n1 = new double[] { 0, 0, 0, 1 };
                        _n2 = new double[] { };
                        _m1 = new double[] { 0, 0, 1, 1 };
                        _m2 = new double[] { };
                        break;
                    }
                default:
                    throw new Exception();
            }


            n1 = new SingleVariablePolynomial(_n1);
            n2 = new SingleVariablePolynomial(_n2);
            m1 = new SingleVariablePolynomial(_m1);
            m2 = new SingleVariablePolynomial(_m2);

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
