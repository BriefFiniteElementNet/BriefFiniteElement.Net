using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;

namespace BriefFiniteElementNet.ElementHelpers
{
    /// <summary>
    /// Represents a calculation helper for DKT element (Discrete Kirchhoff Triangle)
    /// </summary>
    public class Gt9Helper:IElementHelper
    {
        public Element TargetElement { get; set; }

        public Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var shp = shape2d(targetElement, isoCoords);
            var shpDrill = shpDrilll(targetElement, isoCoords);

            var buf = new Matrix(3, 12);

            for (int node = 0; node < 4; node++)
            {
                Matrix Bmembrane = new Matrix(3, 3);

                // ------Bmembrane Matrix in standard {1,2,3} mechanics notation ---------------
                // 
                //               | N,1    0      Nu,1  |
                //   Bmembrane = | 0     N,2     Nv,2  |
                //               | N,2   N,1  Nv,1+Nu,2|
                // -----------------------------------------------------------------------------------

                buf[0, 3 * node + 0] = Bmembrane[0, 0] = shp[0, node];
                buf[0, 3 * node + 2] = Bmembrane[0, 2] = shpDrill[0, node];
                buf[1, 3 * node + 1] = Bmembrane[1, 1] = shp[1, node];
                buf[1, 3 * node + 2] = Bmembrane[1, 2] = shpDrill[3, node];
                buf[2, 3 * node + 0] = Bmembrane[2, 0] = shp[1, node];
                buf[2, 3 * node + 1] = Bmembrane[2, 1] = shp[0, node];
                buf[2, 3 * node + 2] = Bmembrane[2, 2] = shpDrill[1, node] + shpDrill[2, node];
            }

            return buf;
        }

        public Matrix shape2d(Element targetElement, params double[] isoCoords)
        {

            var qElm = targetElement as TriangleElement;

            var locals = qElm.GetTransformationManager().TransformGlobalToLocal(qElm.Nodes.Select(i => i.Location));

            var ss = isoCoords[0];
            var tt = isoCoords[1];
            var qq = isoCoords[2];

            var x = new Matrix(2, 3);

            const double one_over_three = 1.0 / 3.0;
            const double one_over_five = 1.0 / 5.0;
            const double three_over_five = 3.0 / 5.0;

            double[] s = { one_over_three, one_over_five, three_over_five, one_over_five };
            double[] t = { one_over_three, three_over_five, one_over_five, one_over_five };
            double[] q = { one_over_three, one_over_five, one_over_five, three_over_five };



            for (var i = 0; i < locals.Length; i++)
            {
                x[i, 0] = locals[i].X;
                x[i, 1] = locals[i].Y;
            }
            //copy from ShellDKGQ::shape2d( double ss, double tt, 

            var a = new double[3];
            var b = new double[3];
            var c = new double[3];

            //double[] s = { -0.5, 0.5, 0.5, -0.5 };
            //double[] t = { -0.5, -0.5, 0.5, 0.5 };
            double area;

            a[0] = x[0,1] * x[1,2] - x[0,2] * x[1,1];
            a[1] = x[0,2] * x[1,0] - x[0,0] * x[1,2];
            a[2] = x[0,0] * x[1,1] - x[0,1] * x[1,0];

            b[0] = x[1,1] - x[1,2];
            b[1] = x[1,2] - x[1,0];
            b[2] = x[1,0] - x[1,1];

            c[0] = -x[0,1] + x[0,2];
            c[1] = -x[0,2] + x[0,0];
            c[2] = -x[0,0] + x[0,1];


            area = 0.5 * (x[0,0] * x[1,1] + x[0,1] * x[1,2] + x[0,2] * x[1,0] - x[0,0] * x[1,2] - x[0,1] * x[1,0] - x[0,2] * x[1,1]);

            var shp = new Matrix(3, 3);

            shp[2,0] = ss;
            shp[2,1] = tt;
            shp[2,2] = qq;
            /*
            xs[0,0] = x[0,1] - x[0,0];
            xs[0,1] = x[1,1] - x[1,0];
            xs[1,0] = x[0,2] - x[0,0];
            xs[1,1] = x[1,2] - x[1,0];
            */
            var xs = GetJMatrixAt(targetElement, isoCoords).Transpose();
            var sx = xs.Inverse();

            for (var i = 0; i < 3; i++)
            {
                shp[0,i] = b[i] / 2.0 / area;
                shp[1,i] = c[i] / 2.0 / area;
            }

            return shp;
        }

        public Matrix shpDrilll(Element targetElement, params double[] isoCoords)
        {
            var qElm = targetElement as TriangleElement;

            var locals = qElm.GetTransformationManager().TransformGlobalToLocal(qElm.Nodes.Select(ii => ii.Location));

            var x = new Matrix(2, 4);

            var ss = isoCoords[0];
            var tt = isoCoords[1];
            var qq = isoCoords[2];

            for (int i = 0; i < 4; i++)
            {
                x[0, i] = locals[i].X;
                x[1, i] = locals[i].Y;
            }

            var a = new double[3];
            var b = new double[3];
            var c = new double[3];
            var l = new double[3];

            a[0] = x[0,1] * x[1,2] - x[0,2] * x[1,1];
            a[1] = x[0,2] * x[1,0] - x[0,0] * x[1,2];
            a[2] = x[0,0] * x[1,1] - x[0,1] * x[1,0];

            b[0] = x[1,1] - x[1,2];
            b[1] = x[1,2] - x[1,0];
            b[2] = x[1,0] - x[1,1];

            c[0] = -x[0,1] + x[0,2];
            c[1] = -x[0,2] + x[0,0];
            c[2] = -x[0,0] + x[0,1];

            var area = 0.5 * (x[0,0] * x[1,1] + x[0,1] * x[1,2] + x[0,2] * x[1,0] - x[0,0] * x[1,2] - x[0,1] * x[1,0] - x[0,2] * x[1,1]);

            l[0] = ss;
            l[1] = tt;
            l[2] = qq;

            var shpDrill = new Matrix(4,3);

            shpDrill[0,0] = b[0] * (b[2] * l[1] - b[1] * l[2]) / 4.0 / area;
            shpDrill[1,0] = c[0] * (b[2] * l[1] - b[1] * l[2]) / 4.0 / area;
            shpDrill[2,0] = b[0] * (c[2] * l[1] - c[1] * l[2]) / 4.0 / area;
            shpDrill[3,0] = c[0] * (c[2] * l[1] - c[1] * l[2]) / 4.0 / area;

            shpDrill[0,1] = b[1] * (b[0] * l[2] - b[2] * l[0]) / 4.0 / area;
            shpDrill[1,1] = c[1] * (b[0] * l[2] - b[2] * l[0]) / 4.0 / area;
            shpDrill[2,1] = b[1] * (c[0] * l[2] - c[2] * l[0]) / 4.0 / area;
            shpDrill[3,1] = c[1] * (c[0] * l[2] - c[2] * l[0]) / 4.0 / area;

            shpDrill[0,2] = b[2] * (b[1] * l[0] - b[0] * l[1]) / 4.0 / area;
            shpDrill[1,2] = c[2] * (b[1] * l[0] - b[0] * l[1]) / 4.0 / area;
            shpDrill[2,2] = b[2] * (c[1] * l[0] - c[0] * l[1]) / 4.0 / area;
            shpDrill[3,2] = c[2] * (c[1] * l[0] - c[0] * l[1]) / 4.0 / area;

            return shpDrill;
        }


        public Matrix GetB_iMatrixAt(Element targetElement, int i, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetDMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var f = targetElement as TriangleElement;

            var mat = f.Material.GetMaterialPropertiesAt(isoCoords); ;

            if (!CalcUtil.IsIsotropicMaterial(mat))
                throw new Exception();


            var d = new Matrix(3, 3);

            var e = mat.Ex;
            var nu = mat.NuXy;
            var t = f.Section.GetThicknessAt(isoCoords);

            if (f.MembraneFormulation == MembraneFormulation.PlaneStress)
            {
                //page 23 of JAVA Thesis pdf

                var cf = e / (1 - nu * nu);

                d[0, 0] = d[1, 1] = 1;
                d[1, 0] = d[0, 1] = nu;
                d[2, 2] = (1 - nu) / 2;

                d.MultiplyByConstant(cf);
            }
            else
            {
                //page 24 of JAVA Thesis pdf

                var cf = e / ((1 + nu) * (1 - 2 * nu));

                d[0, 0] = d[1, 1] = 1 - nu;
                d[1, 0] = d[0, 1] = nu;
                d[2, 2] = (1 - 2 * nu) / 2;

                d.MultiplyByConstant(cf);
            }

            //

            d.MultiplyByConstant(t);

            return d;
        }

        public Matrix GetRhoMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetMuMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetJMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var tri = targetElement as TriangleElement;

            var tmgr = targetElement.GetTransformationManager();

            if (tri == null)
                throw new Exception();

            var xi = isoCoords[0];
            var eta = isoCoords[1];

            var p1g = tri.Nodes[0].Location;
            var p2g = tri.Nodes[1].Location;
            var p3g = tri.Nodes[2].Location;

            var p1l = tmgr.TransformGlobalToLocal(p1g);// p1g.TransformBack(transformMatrix);
            var p2l = tmgr.TransformGlobalToLocal(p2g);// p2g.TransformBack(transformMatrix);
            var p3l = tmgr.TransformGlobalToLocal(p3g);// p3g.TransformBack(transformMatrix);

            var x1 = p1l.X;
            var x2 = p2l.X;
            var x3 = p3l.X;

            var y1 = p1l.Y;
            var y2 = p2l.Y;
            var y3 = p3l.Y;

            var buf = new Matrix(2, 2);

            var x12 = x1 - x2;
            var x31 = x3 - x1;
            var y31 = y3 - y1;

            var y23 = y2 - y3;

            var y13 = y1 - y3;
            var y12 = y1 - y2;
            var X12 = x1 - x2;

            var x23 = x2 - x3;

            buf[0, 0] = x31;
            buf[1, 1] = y12;

            buf[0, 1] = x12;
            buf[1, 0] = y31;

            return buf;
        }

        public Matrix CalcLocalKMatrix(Element targetElement)
        {
            var buf = ElementHelperExtensions.CalcLocalKMatrix_Triangle(this, targetElement);

            return buf;
        }

        public Matrix CalcLocalMMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix CalcLocalCMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public FluentElementPermuteManager.ElementLocalDof[] GetDofOrder(Element targetElement)
        {
            return new FluentElementPermuteManager.ElementLocalDof[]
            {
                new FluentElementPermuteManager.ElementLocalDof(0, DoF.Dx),
                new FluentElementPermuteManager.ElementLocalDof(0, DoF.Dy),
                new FluentElementPermuteManager.ElementLocalDof(0, DoF.Rz),

                new FluentElementPermuteManager.ElementLocalDof(1, DoF.Dx),
                new FluentElementPermuteManager.ElementLocalDof(1, DoF.Dy),
                new FluentElementPermuteManager.ElementLocalDof(1, DoF.Rz),

                new FluentElementPermuteManager.ElementLocalDof(2, DoF.Dx),
                new FluentElementPermuteManager.ElementLocalDof(2, DoF.Dy),
                new FluentElementPermuteManager.ElementLocalDof(2, DoF.Rz),
            };
        }

        public Matrix GetLocalInternalForceAt(Element targetElement, Displacement[] globalDisplacements,
            params double[] isoCoords)
        {
            //todo: fix internal forces based on https://en.wikipedia.org/wiki/Kirchhoff%E2%80%93Love_plate_theory#Equilibrium_equations

            //return new Matrix(3, 1);
            var strain = GetLocalStrainAt(targetElement, globalDisplacements, isoCoords);

            var d = GetDMatrixAt(targetElement, isoCoords);


            var mQ4 = d * strain;

            return mQ4;

            throw new NotImplementedException();
        }

        public Displacement GetLocalDisplacementAt(Element targetElement, Displacement[] localDisplacements,
            params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public int[] GetNMaxOrder(Element targetElement)
        {
            return new int[] { Ng, Ng, 0 };
        }

        public int Ng = 1;

        public int[] GetBMaxOrder(Element targetElement)
        {
            return new int[] { Ng, Ng, 0 };
        }

        public int[] GetDetJOrder(Element targetElement)
        {
            return new int[] { 1, 1, 0 };
        }

        public Force[] GetLocalEquivalentNodalLoads(Element targetElement, Load load)
        {
            throw new NotImplementedException();
        }

        public FlatShellStressTensor GetLoadInternalForceAt(Element targetElement, Load load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public FlatShellStressTensor GetLoadDisplacementAt(Element targetElement, Load load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public Matrix GetLocalStrainAt(Element targetElement, Displacement[] globalDisplacements, double[] isoCoords)
        {
            var b = GetBMatrixAt(targetElement, isoCoords);

            var d = GetDMatrixAt(targetElement, isoCoords);

            var tr = targetElement.GetTransformationManager();

            var lc = globalDisplacements.Select(i => tr.TransformGlobalToLocal(i)).ToArray();

            var u = new Matrix(12, 1);

            for (var i = 0; i < lc.Length; i++)
            {
                u[3 * i + 0, 0] = lc[i].DX;
                u[3 * i + 1, 0] = lc[i].DY;
                u[3 * i + 2, 0] = lc[i].RZ;
            }

            var buf = b * u;

            return buf;
        }


    }
}
