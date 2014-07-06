using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BriefFiniteElementNet.CodeGeneration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetMForBothRelease();
        }


        private static void Test1()
        {
            var rArr = Enumerable.Range(0, 9).Select(i => string.Format("r[{0:00}]", i)).ToArray();
            var r = StringMatrix.MatrixFromStringArray(rArr);

            var kArr =
                //new int[] { 0, 1, 2, 12, 13, 14, 24, 25, 26 }.Select(i => string.Format("tmp[st+{0:00}]", i)).ToArray();
                Enumerable.Range(0,9).Select(i => string.Format("tmp[{0:00}]", i)).ToArray();
            
            var k = StringMatrix.MatrixFromStringArray(kArr);

            var sb = new StringBuilder();

            var kr = k*r;

            var hhh = kr.GetFillCode("tmp2");


            var tt = sb.ToString().Replace("]+", "] + ").Replace("=", " = ").Replace("*", " * ");

        }


        private static void GetLocalStNoShear()
        {
            var sb = new StringBuilder();

            var co = StringMatrix.VectorFromStringArray("12", "6*l", "4*l2", "2*l2");
            var x = "a*l2";
            var y = "iy" * co;
            var z = "iz" * co;
            var g = "g*j*l2/e";

            var k1 = StringMatrix.Diag(x, z[0], y[0]);
            var k2 = new StringMatrix(3, 3);
            k2[1, 2] = z[1];
            k2[2, 1] = "-" + y[1];

            var k3 = StringMatrix.Diag(g, y[2], z[2]);
            var k4 = StringMatrix.Diag("-" + g, y[3], z[3]);

            var buf1 = StringMatrix.HorzCat(k1, k2);
            buf1 = StringMatrix.HorzCat(buf1, -k1);
            buf1 = StringMatrix.HorzCat(buf1, k2);


            var buf2 = StringMatrix.HorzCat(k2.Transpose(), k3);
            buf2 = StringMatrix.HorzCat(buf2, -k2.Transpose());
            buf2 = StringMatrix.HorzCat(buf2, k4);


            var buf3 = StringMatrix.HorzCat(-k1, -k2);
            buf3 = StringMatrix.HorzCat(buf3, k1);
            buf3 = StringMatrix.HorzCat(buf3, -k2);


            var buf4 = StringMatrix.HorzCat(k2.Transpose(), k4);
            buf4 = StringMatrix.HorzCat(buf4, -k2.Transpose());
            buf4 = StringMatrix.HorzCat(buf4, k3);

            var buf = StringMatrix.VertCat(buf1, buf2);
            buf = StringMatrix.VertCat(buf, buf3);
            buf = StringMatrix.VertCat(buf, buf4);

            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    buf[i, j] = null;
                }
            }

            var tt = buf.GetFillCode("buf");
            throw new NotImplementedException();
        }


        private static void GetLocalStWithShear()
        {
            var sb = new StringBuilder();

            var co = StringMatrix.VectorFromStringArray("12", "6*l", "4*l2", "2*l2");
            var x = "a/l";
            
            var ez = "e*iz/(ay*g)" ;
            var ey = "e*iy/(az*g)" ;
            var z = "iz/(l*(l2/12+ez))"*StringMatrix.VectorFromStringArray("1", "l/2", "(l2/3+ez)", "(l2/6-ez)");
            var y = "iy/(l*(l2/12+ey))"*StringMatrix.VectorFromStringArray("1", "l/2", "(l2/3+ey)", "(l2/6-ey)");
            
            var g = "g*j/e*l";

            var k1 = StringMatrix.Diag(x, z[0], y[0]);
            var k2 = new StringMatrix(3, 3);
            k2[1, 2] = z[1];
            k2[2, 1] = "-" + y[1];

            var k3 = StringMatrix.Diag(g, y[2], z[2]);
            var k4 = StringMatrix.Diag("-" + g, y[3], z[3]);

            var buf1 = StringMatrix.HorzCat(k1, k2);
            buf1 = StringMatrix.HorzCat(buf1, -k1);
            buf1 = StringMatrix.HorzCat(buf1, k2);


            var buf2 = StringMatrix.HorzCat(k2.Transpose(), k3);
            buf2 = StringMatrix.HorzCat(buf2, -k2.Transpose());
            buf2 = StringMatrix.HorzCat(buf2, k4);


            var buf3 = StringMatrix.HorzCat(-k1, -k2);
            buf3 = StringMatrix.HorzCat(buf3, k1);
            buf3 = StringMatrix.HorzCat(buf3, -k2);


            var buf4 = StringMatrix.HorzCat(k2.Transpose(), k4);
            buf4 = StringMatrix.HorzCat(buf4, -k2.Transpose());
            buf4 = StringMatrix.HorzCat(buf4, k3);

            var buf = StringMatrix.VertCat(buf1, buf2);
            buf = StringMatrix.VertCat(buf, buf3);
            buf = StringMatrix.VertCat(buf, buf4);

            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    buf[i, j] = null;
                }
            }

            var tt = buf.GetFillCode("buf");
            throw new NotImplementedException();
        }


        private static void GetMForStartRelease()
        {
            var sb = new StringBuilder();

            var w = StringMatrix.Diag("1", "", "");
            var x = StringMatrix.Diag("", "-0.5", "-0.5");
            var y = new StringMatrix(3, 3);
            y[1, 2] = "1.5/l";
            y[2, 1] = "-1.5/l";
            var I = StringMatrix.Diag("1", "1", "1");

            var z = new StringMatrix(3, 3);
            var p = StringMatrix.VectorFromStringArray("4", "5", "6");
            var q = StringMatrix.VectorFromStringArray("10", "11", "12");

            //y = "2/3"*y;

            var m = StringMatrix.Join(new StringMatrix[,] { { I, -y, z, z }, { z, w, z, z }, { z, y, I, z }, { z, x, z, I } });

            var k = new StringMatrix(12, 12);

            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    k[i, j] = string.Format("buf[{0:00},{1:00}]", i, j);
                }
            }


            var res = (m*k).GetFillCode("k").Replace("1*", "");

            var sb2 = new StringBuilder();

            foreach (var ln in res.Split('\r'))
            {
                var mtch = Regex.Match(ln.Trim(), @"^k\[(\d+),(\d+)\]");

                if (!mtch.Success)
                    continue;

                var i = int.Parse(mtch.Groups[1].Value);
                var j = int.Parse(mtch.Groups[2].Value);

                if (j >= i)
                    sb2.AppendLine(ln.Trim());
            }

            var res2 = sb2.ToString();
            throw new NotImplementedException();
        }

        private static void GetMForEndRelease()
        {
            var sb = new StringBuilder();

            var w = StringMatrix.Diag("1", "", "");
            var x = StringMatrix.Diag("", "-0.5", "-0.5");
            var y = new StringMatrix(3, 3);
            y[1, 2] = "1.5/l";
            y[2, 1] = "-1.5/l";
            var I = StringMatrix.Diag("1", "1", "1");

            var z = new StringMatrix(3, 3);
            var p = StringMatrix.VectorFromStringArray("4", "5", "6");
            var q = StringMatrix.VectorFromStringArray("10", "11", "12");

            //y = "2/3"*y;

            var m = StringMatrix.Join(new StringMatrix[,] { { I, z, z, -y }, { z, I, z, x }, { z, z, I, y }, { z, z, z, w } });

            var k = new StringMatrix(12, 12);

            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    k[i, j] = string.Format("buf[{0:00},{1:00}]", i, j);
                }
            }


            var res = (m * k).GetFillCode("k");


            var sb2 = new StringBuilder();

            foreach (var ln in res.Split('\r'))
            {
                var mtch = Regex.Match(ln.Trim(), @"^k\[(\d+),(\d+)\]");

                if (!mtch.Success)
                    continue;

                var i = int.Parse(mtch.Groups[1].Value);
                var j = int.Parse(mtch.Groups[2].Value);

                if (j >= i)
                    sb2.AppendLine(ln.Trim().Replace("1*",""));
            }

            var res2 = sb2.ToString();

            throw new NotImplementedException();
        }

        private static void GetMForBothRelease()
        {
            var sb = new StringBuilder();

            var w = StringMatrix.Diag("1", "", "");
            var x = StringMatrix.Diag("", "-0.5", "-0.5");
            var y = new StringMatrix(3, 3);
            y[1, 2] = "1/l";
            y[2, 1] = "-1/l";
            var I = StringMatrix.Diag("1", "1", "1");

            var z = new StringMatrix(3, 3);
            var p = StringMatrix.VectorFromStringArray("4", "5", "6");
            var q = StringMatrix.VectorFromStringArray("10", "11", "12");

            //y = "2/3"*y; we've chaged the 
            //y[1, 2] = "1.5/l"
            //to 
            //y[1, 2] = "1/l";;

            var m = StringMatrix.Join(new StringMatrix[,] { { I, -y, z, -y }, { z, w, z, z }, { z, y, I, y }, { z, z, z, w } });

            var k = new StringMatrix(12, 12);

            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    k[i, j] = string.Format("buf[{0:00},{1:00}]", i, j);
                }
            }


            var res = (m * k).GetFillCode("k");

            var sb2 = new StringBuilder();

            foreach (var ln in res.Split('\r'))
            {
                var mtch = Regex.Match(ln.Trim(), @"^k\[(\d+),(\d+)\]");

                if (!mtch.Success)
                    continue;

                var i = int.Parse(mtch.Groups[1].Value);
                var j = int.Parse(mtch.Groups[2].Value);

                if (j >= i)
                    sb2.AppendLine(ln.Trim().Replace("1*", ""));
            }

            var res2 = sb2.ToString();

            throw new NotImplementedException();
        }
    }
}
