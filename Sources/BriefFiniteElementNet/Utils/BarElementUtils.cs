using BriefFiniteElementNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriefFiniteElementNet.Utils
{
    public static class BarElementUtils
    {

        /// <summary>
        /// Gets the transformation matrix for converting local coordinate to global coordinate for a two node straight element.
        /// </summary>
        /// <param name="v">The [ end - start ] vector.</param>
        /// <param name="webR">The web rotation in radian.</param>
        /// <returns>
        /// transformation matrix
        /// </returns>
        public static Matrix Get2NodeElementTransformationMatrix(Vector v, double webR)
        {
            var cxx = 0.0;
            var cxy = 0.0;
            var cxz = 0.0;

            var cyx = 0.0;
            var cyy = 0.0;
            var cyz = 0.0;

            var czx = 0.0;
            var czy = 0.0;
            var czz = 0.0;


            var teta = webR;

            var s = webR.Equals(0.0) ? 0.0 : Math.Sin(teta);
            var c = webR.Equals(0.0) ? 1.0 : Math.Cos(teta);

            if (MathUtil.FEquals(0, v.X) && MathUtil.FEquals(0, v.Y))
            {
                if (v.Z > 0)
                {
                    czx = 1;
                    cyy = 1;
                    cxz = -1;
                }
                else
                {
                    czx = -1;
                    cyy = 1;
                    cxz = 1;
                }
            }
            else
            {
                var l = v.Length;
                cxx = v.X / l;
                cyx = v.Y / l;
                czx = v.Z / l;
                var d = Math.Sqrt(cxx * cxx + cyx * cyx);
                cxy = -cyx / d;
                cyy = cxx / d;
                cxz = -cxx * czx / d;
                cyz = -cyx * czx / d;
                czz = d;
            }

            var t = new Matrix(3, 3);

            t[0, 0] = cxx;
            t[0, 1] = cxy * c + cxz * s;
            t[0, 2] = -cxy * s + cxz * c;

            t[1, 0] = cyx;
            t[1, 1] = cyy * c + cyz * s;
            t[1, 2] = -cyy * s + cyz * c;

            t[2, 0] = czx;
            t[2, 1] = czy * c + czz * s;
            t[2, 2] = -czy * s + czz * c;

            return t;
        }


        /// <summary>
        /// Gets the transformation matrix for converting local coordinate to global coordinate for a two node straight element.
        /// </summary>
        /// <param name="v">The [ end - start ] vector.</param>
        /// <returns>
        /// transformation matrix
        /// </returns>
        public static Matrix Get2NodeElementTransformationMatrix(Vector v)
        {
            return Get2NodeElementTransformationMatrix(v, 0);
        }


        /// <summary>
        /// calculates the bar transformation matrix (local to/from global)
        /// </summary>
        /// <param name="v">vector that connects start to end</param>
        /// <param name="_webRotation">rotation around local x axis</param>
        /// <returns></returns>
        public static Matrix GetBarTransformationMatrix(Vector v, double _webRotation)
        {
            var buf = new Matrix(3, 3);

            GetBarTransformationMatrix(v, _webRotation, buf);

            return buf;
        }

        /// <summary>
        /// calculates the bar transformation matrix (local to/from global) and fill to <see cref="output"/> parameter
        /// </summary>
        /// <param name="v">vector that connects start to end</param>
        /// <param name="_webRotation">rotation around local x axis</param>
        /// <returns></returns>
        public static void GetBarTransformationMatrix(Vector v, double _webRotation, Matrix output)
        {
            var cxx = 0.0;
            var cxy = 0.0;
            var cxz = 0.0;

            var cyx = 0.0;
            var cyy = 0.0;
            var cyz = 0.0;

            var czx = 0.0;
            var czy = 0.0;
            var czz = 0.0;

            var teta = _webRotation;

            var s = Math.Sin(teta * Math.PI / 180.0);
            var c = Math.Cos(teta * Math.PI / 180.0);

            //var v = this.EndNode.Location - this.StartNode.Location;

            if (MathUtil.FEquals(0, v.X) && MathUtil.FEquals(0, v.Y))
            {
                if (v.Z > 0)
                {
                    czx = 1;
                    cyy = 1;
                    cxz = -1;
                }
                else
                {
                    czx = -1;
                    cyy = 1;
                    cxz = 1;
                }
            }
            else
            {
                var l = v.Length;
                cxx = v.X / l;
                cyx = v.Y / l;
                czx = v.Z / l;
                var d = Math.Sqrt(cxx * cxx + cyx * cyx);
                cxy = -cyx / d;
                cyy = cxx / d;
                cxz = -cxx * czx / d;
                cyz = -cyx * czx / d;
                czz = d;
            }

            var pars = new double[9];

            pars[0] = cxx;
            pars[1] = cxy * c + cxz * s;
            pars[2] = -cxy * s + cxz * c;

            pars[3] = cyx;
            pars[4] = cyy * c + cyz * s;
            pars[5] = -cyy * s + cyz * c;

            pars[6] = czx;
            pars[7] = czy * c + czz * s;
            pars[8] = -czy * s + czz * c;


            var buf = output;// new Matrix(3, 3);

            if (buf.RowCount != 3 || buf.ColumnCount != 3)
                throw new Exception();

            // TODO: MAT - set values directly (using pars array)
            buf.SetColumn(0, new double[] { pars[0], pars[1], pars[2] });
            buf.SetColumn(1, new double[] { pars[3], pars[4], pars[5] });
            buf.SetColumn(2, new double[] { pars[6], pars[7], pars[8] });

            //return buf;
        }
    }
}
