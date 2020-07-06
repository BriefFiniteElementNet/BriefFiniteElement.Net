using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;
using static BriefFiniteElementNet.ElementPermuteHelper;
using System.Xml.XPath;


namespace BriefFiniteElementNet.Elements.ElementHelpers
{
    public class DkqHelper:IElementHelper
    {
        public Element TargetElement { get; set; }

        public virtual Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
        {
            // new version:
            var quad = targetElement as QuadrilaturalElement;
            var tmgr = targetElement.GetTransformationManager();

            if (quad == null)
                throw new Exception();

            var xi = isoCoords[0];
            var eta = isoCoords[1];

            var p1g = quad.Nodes[0].Location;
            var p2g = quad.Nodes[1].Location;
            var p3g = quad.Nodes[2].Location;
            var p4g = quad.Nodes[3].Location;

            var n1 = tmgr.TransformGlobalToLocal(p1g);
            var n2 = tmgr.TransformGlobalToLocal(p2g);
            var n3 = tmgr.TransformGlobalToLocal(p3g);
            var n4 = tmgr.TransformGlobalToLocal(p4g);

            /* old version
            var lpts = targetElement.GetTransformationManager().TransformGlobalToLocal(targetElement.Nodes.Select(i => i.Location));

            var xi = isoCoords[0];
            var eta = isoCoords[1];

            var n1 = lpts[0]; // local points
            var n2 = lpts[1];
            var n3 = lpts[2];
            var n4 = lpts[3];
            */

            var x1 = n1.X;
            var x2 = n2.X;
            var x3 = n3.X;
            var x4 = n4.X;

            var y1 = n1.Y;
            var y2 = n2.Y;
            var y3 = n3.Y;
            var y4 = n4.Y;

            var x12 = x2 - x1;
            var x23 = x3 - x2;
            var x34 = x4 - x3;
            var x41 = x1 - x4;

            var y12 = y2 - y1;
            var y23 = y3 - y2;
            var y34 = y4 - y3;
            var y41 = y1 - y4;

            var l12_2 = x12 * x12 + y12 * y12;
            var l23_2 = x23 * x23 + y23 * y23;
            var l34_2 = x34 * x34 + y34 * y34;
            var l41_2 = x41 * x41 + y41 * y41;

            var a5 = -0.75 * x12 * y12 / l12_2;
            var a6 = -0.75 * x23 * y23 / l23_2;
            var a7 = -0.75 * x34 * y34 / l34_2;
            var a8 = -0.75 * x41 * y41 / l41_2;

            var b5 = (0.5 * y12 * y12 - 0.25 * x12 * x12) / l12_2;
            var b6 = (0.5 * y23 * y23 - 0.25 * x23 * x23) / l23_2;
            var b7 = (0.5 * y34 * y34 - 0.25 * x34 * x34) / l34_2;
            var b8 = (0.5 * y41 * y41 - 0.25 * x41 * x41) / l41_2;

            var c5 = -x12 / l12_2;
            var c6 = -x23 / l23_2;
            var c7 = -x34 / l34_2;
            var c8 = -x41 / l41_2;

            var d5 = (0.5 * x12 * x12 - 0.25 * y12 * y12) / l12_2;
            var d6 = (0.5 * x23 * x23 - 0.25 * y23 * y23) / l23_2;
            var d7 = (0.5 * x34 * x34 - 0.25 * y34 * y34) / l34_2;
            var d8 = (0.5 * x41 * x41 - 0.25 * y41 * y41) / l41_2;

            var e5 = -y12 / l12_2;
            var e6 = -y23 / l23_2;
            var e7 = -y34 / l34_2;
            var e8 = -y41 / l41_2;

            var n_xi = new double[] //4.51 p69
            {
                0, //I've added this 0 at first element for indexing issue
                0.25*(2*xi + eta)*(1 - eta),
                0.25*(2*xi - eta)*(1 - eta),
                0.25*(2*xi + eta)*(1 + eta),
                0.25*(2*xi - eta)*(1 + eta),
                -xi*(1 - eta),
                0.5*(1 - eta*eta),
                -xi*(1 + eta),
                -0.5*(1 - eta*eta),
            };

            var n_eta = new double[] //4.51 p69
            {
                0, //I've added this 0 at first element for indexing issue
                0.25*(2*eta + xi)*(1 - xi),
                0.25*(2*eta - xi)*(1 + xi),
                0.25*(2*eta + xi)*(1 + xi),
                0.25*(2*eta - xi)*(1 - xi),
                -0.5*(1 - xi*xi),
                -eta*(1 + xi),
                0.5*(1 - xi*xi),
                -eta*(1 - xi),
            };

            var hx_xi = new double[] //4.45 p67 (using 4.51 p69)
            {
                1.5*(c5*n_xi[5] - c8*n_xi[8]),
                a5*n_xi[5] + a8*n_xi[8],
                -n_xi[1] - b5*n_xi[5] - b8*n_xi[8],

                1.5*(c6*n_xi[6] - c5*n_xi[5]),
                a6*n_xi[6] + a5*n_xi[5],
                -n_xi[2] - b6*n_xi[6] - b5*n_xi[5],

                1.5*(c7*n_xi[7] - c6*n_xi[6]),
                a7*n_xi[7] + a6*n_xi[6],
                -n_xi[3] - b7*n_xi[7] - b6*n_xi[6],

                1.5*(c8*n_xi[8] - c7*n_xi[7]),
                a8*n_xi[8] + a7*n_xi[7],
                -n_xi[4] - b8*n_xi[8] - b7*n_xi[7],
            };

            var hx_eta = new double[] //4.46 p67
            {
                1.5*(c5*n_eta[5] - c8*n_eta[8]),
                a5*n_eta[5] + a8*n_eta[8],
                -n_eta[1] - b5*n_eta[5] - b8*n_eta[8],

                1.5*(c6*n_eta[6] - c5*n_eta[5]),
                a6*n_eta[6] + a5*n_eta[5],
                -n_eta[2] - b6*n_eta[6] - b5*n_eta[5],

                1.5*(c7*n_eta[7] - c6*n_eta[6]),
                a7*n_eta[7] + a6*n_eta[6],
                -n_eta[3] - b7*n_eta[7] - b6*n_eta[6],

                1.5*(c8*n_eta[8] - c7*n_eta[7]),
                a8*n_eta[8] + a7*n_eta[7],
                -n_eta[4] - b8*n_eta[8] - b7*n_eta[7],
            };

            var hy_xi = new double[] //4.51 p69
            {
                1.5*(e5*n_xi[5] - e8*n_xi[8]),
                n_xi[1] + d5*n_xi[5] + d8*n_xi[8],
                -a5*n_xi[5] - a8*n_xi[8],

                1.5*(e6*n_xi[6] - e5*n_xi[5]),
                n_xi[2] + d6*n_xi[6] + d5*n_xi[5],
                -a6*n_xi[6] - a5*n_xi[5],

                1.5*(e7*n_xi[7] - e6*n_xi[6]),
                n_xi[3] + d7*n_xi[7] + d6*n_xi[6],
                -a7*n_xi[7] - a6*n_xi[6],

                1.5*(e8*n_xi[8] - e7*n_xi[7]),
                n_xi[4] + d8*n_xi[8] + d7*n_xi[7],
                -a8*n_xi[8] - a7*n_xi[7],
            };

            var hy_eta = new double[] //4.51 p69
            {
                1.5*(e5*n_eta[5] - e8*n_eta[8]),
                n_eta[1] + d5*n_eta[5] + d8*n_eta[8],
                -a5*n_eta[5] - a8*n_eta[8],

                1.5*(e6*n_eta[6] - e5*n_eta[5]),
                n_eta[2] + d6*n_eta[6] + d5*n_eta[5],
                -a6*n_eta[6] - a5*n_eta[5],

                1.5*(e7*n_eta[7] - e6*n_eta[6]),
                n_eta[3] + d7*n_eta[7] + d6*n_eta[6],
                -a7*n_eta[7] - a6*n_eta[6],

                1.5*(e8*n_eta[8] - e7*n_eta[7]),
                n_eta[4] + d8*n_eta[8] + d7*n_eta[7],
                -a8*n_eta[8] - a7*n_eta[7],
            };

            var jacob = GetJMatrixAt(targetElement, isoCoords);
            var invJ = jacob.Inverse();

            double j11, j12, j21, j22;

            j11 = invJ[0, 0];   // terms of the inverse JMatrix
            j12 = invJ[0, 1];
            j21 = invJ[1, 0];
            j22 = invJ[1, 1];

            var buf = new Matrix(3, 12);

            for (int i = 0; i < 12; i++)
            {
                buf[0, i] = j11 * hx_xi[i] + j12 * hx_eta[i];
                buf[1, i] = j21 * hy_xi[i] + j22 * hy_eta[i];
                buf[2, i] = j11 * hy_xi[i] + j12 * hy_eta[i] + j21 * hx_xi[i] + j22 * hx_eta[i];
            }

            return buf;
        }

        public Matrix GetB_iMatrixAt(Element targetElement, int i, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetDMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var f = targetElement as QuadrilaturalElement;

            if (f == null)
                throw new Exception();

            var mat = f.Material.GetMaterialPropertiesAt(isoCoords);

            if (!CalcUtil.IsIsotropicMaterial(mat))
                throw new Exception();

            var t = f.Section.GetThicknessAt(isoCoords);

            var d = new Matrix(3, 3);
            {
                var cf = t * t * t / 12.0;

                d[0, 0] = mat.Ex / (1.0 - mat.NuXy * mat.NuYx);
                d[1, 1] = mat.Ey / (1.0 - mat.NuXy * mat.NuYx);
                d[0, 1] = d[1, 0] =
                    mat.Ex * mat.NuYx / (1.0 - mat.NuXy * mat.NuYx);

                d[2, 2] = mat.Ex / (2.0 * (1.0 + mat.NuXy));

                //p55 http://www.code-aster.org/doc/v11/en/man_r/r3/r3.07.03.pdf

                d.MultiplyByConstant(cf);
            }

            return d;
        }

        public Matrix GetDMatrixAt2(Element targetElement, params double[] isoCoords)
        {
            //based on https://github.com/lge88/OpenSees/blob/1048cad190b8192cdd55448242ce4dea58246742/SRC/material/section/ElasticPlateSection.cpp
            var f = targetElement as QuadrilaturalElement;

            var mat = f.Material.GetMaterialPropertiesAt(isoCoords);

            if (!CalcUtil.IsIsotropicMaterial(mat))
                throw new Exception();

            var e = mat.Ex;
            var t = f.Section.GetThicknessAt(isoCoords);
            var nu = mat.NuXy;

            var d = new Matrix(3, 3);

            var tangent = d;

            {
                //page 64 of pdf

                var E = e;
                var h = t;

                double D = E * (h * h * h) / 12.0 / (1.0 - nu * nu);

                double G = 0.5 * E / (1.0 + nu);

                tangent[0, 0] = -D;
                tangent[1, 1] = -D;

                tangent[0, 1] = -nu * D;
                tangent[1, 0] = tangent[0, 1];

                tangent[2, 2] = -0.5 * D * (1.0 - nu);

                //tangent[3, 3] = 5.0 / 6.0 * G * h;

                //tangent[4, 4] = tangent[3, 3];

            }

            tangent.MultiplyByConstant(-1);

            var diff = (tangent - GetDMatrixAt2(targetElement, isoCoords)).Max(ii=>Math.Abs(ii));//= 1e-9
            
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
            //TODO: this is shape function maybe for Q4 element, not for quad element.
            //shape function is used in calculating equivalent nodal loads and internal displacements
            //both of these should takes the nodal rotations into account.
            //as dqk have 12 dof, shape function matrix should be 12 column matrix.

            //used for distributed load, 8.26 of http://what-when-how.com/the-finite-element-method/fem-for-plates-and-shells-finite-element-method-part-1/

            //N from 7.54 of http://what-when-how.com/the-finite-element-method/fem-for-two-dimensional-solids-finite-element-method-part-2/
            var xi = isoCoords[0];
            var eta = isoCoords[1];

            var buf = new Matrix(4, 1);

            var xis = new double[] {-1,1,1,-1 };//for each node
            var etas = new double[] {-1,-1,1,1 };//for each node

            for (var i = 0; i < 4; i++)
            {
                buf[i, 0] = 0.25 * (1 + xis[i] * xi) * (1 + etas[i] * eta);
            }

            return buf;
        }

        public virtual Matrix GetJMatrixAt(Element targetElement, params double[] isoCoords)
        {
            //4.49 p 68

            var xi = isoCoords[0];
            var eta = isoCoords[1];

            var lpts =
                targetElement.GetTransformationManager()
                    .TransformGlobalToLocal(targetElement.Nodes.Select(i => i.Location));

            var n1 = lpts[0];
            var n2 = lpts[1];
            var n3 = lpts[2];
            var n4 = lpts[3];

            var x1 = n1.X;
            var x2 = n2.X;
            var x3 = n3.X;
            var x4 = n4.X;

            var y1 = n1.Y;
            var y2 = n2.Y;
            var y3 = n3.Y;
            var y4 = n4.Y;

            var x12 = x1 - x2;
            var x23 = x2 - x3;
            var x34 = x3 - x4;
            var x41 = x4 - x1;

            var y12 = y1 - y2;
            var y23 = y2 - y3;
            var y34 = y3 - y4;
            var y41 = y4 - y1;

            var x21 = -x12;
            var x32 = -x23;

            var y21 = -y12;
            var y32 = -y23;

            var J11 = 0.25 * (x21 + x34 + eta * (x12 + x34));
            var J12 = 0.25 * (y21 + y34 + eta * (y12 + y34));

            var J21 = 0.25 * (x32 + x41 + xi * (x12 + x34));
            var J22 = 0.25 * (y32 + y41 + xi * (y12 + y34));

            var buf = new Matrix(2, 2);

            buf.FillRow(0, J11, J12);
            buf.FillRow(1, J21, J22);

            return buf;
        }

        public virtual Matrix CalcLocalKMatrix(Element targetElement)
        {

            return ElementHelperExtensions.CalcLocalKMatrix_Quad(this, targetElement); 

        }

        public Matrix CalcLocalMMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix CalcLocalCMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public ElementLocalDof[] GetDofOrder(Element targetElement)
        {
            return new ElementLocalDof[]
            {

                new ElementLocalDof(0, DoF.Dz),
                new ElementLocalDof(0, DoF.Rx),
                new ElementLocalDof(0, DoF.Ry),

                new ElementLocalDof(1, DoF.Dz),
                new ElementLocalDof(1, DoF.Rx),
                new ElementLocalDof(1, DoF.Ry),

                new ElementLocalDof(2, DoF.Dz),
                new ElementLocalDof(2, DoF.Rx),
                new ElementLocalDof(2, DoF.Ry),

                new ElementLocalDof(3, DoF.Dz),
                new ElementLocalDof(3, DoF.Rx),
                new ElementLocalDof(3, DoF.Ry),
            };
        }

        public virtual Matrix GetLocalInternalForceAt(Element targetElement, Displacement[] globalDisplacements, params double[] isoCoords)
        {
            var b = GetBMatrixAt(targetElement, isoCoords);

            var d = GetDMatrixAt(targetElement, isoCoords);

            var tr = targetElement.GetTransformationManager();

            var lc = globalDisplacements.Select(i => tr.TransformGlobalToLocal(i)).ToArray();

            var u = new Matrix(12, 1);

            for (var i = 0; i < lc.Length; i++)
            {
                u[3 * i + 0, 0] = lc[i].DZ;
                u[3 * i + 1, 0] = lc[i].RX;
                u[3 * i + 2, 0] = lc[i].RY;
            }

            var mDkq = d * b * u;

            return mDkq;
            /*
            var buf = new BendingStressTensor();

            buf.M11 = mDkq[0, 0];
            buf.M22 = mDkq[1, 0];
            buf.M12 = -(buf.M21 = mDkq[2, 0]);

            throw new NotImplementedException();
            */
        }

        public Displacement GetLocalDisplacementAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public int[] GetNMaxOrder(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public int Ng = 1;

        public int[] GetBMaxOrder(Element targetElement)
        {
            return new int[] { Ng, Ng, 0};
        }

        public int[] GetDetJOrder(Element targetElement)
        {
            return new int[] { 1, 1, 0 };
        }

        public Force[] GetLocalEquivalentNodalLoads(Element targetElement, ElementalLoad load)
        {
            var tr = targetElement.GetTransformationManager();

            #region uniform

            if (load is BriefFiniteElementNet.Loads.UniformLoad )
            {
                var ul = load as BriefFiniteElementNet.Loads.UniformLoad;

                var localDir = ul.Direction.GetUnit();

                if (ul.CoordinationSystem == CoordinationSystem.Global)
                {

                    localDir = tr.TransformGlobalToLocal(localDir);
                }

                var ux = localDir.X * ul.Magnitude;
                var uy = localDir.Y * ul.Magnitude;
                var uz = localDir.Z * ul.Magnitude;

                var intg = new GaussianIntegrator();

                intg.A1 = -1;
                intg.A2 = 1;

                intg.F1 = gama => -1;
                intg.F2 = gama => 1;

                intg.G1 = (eta, gama) => 0;
                intg.G2 = (eta, gama) => 1;

                intg.GammaPointCount = 1;
                intg.XiPointCount = intg.EtaPointCount = 2;

                intg.H = new FunctionMatrixFunction((xi, eta, gama) =>
                    {
                        var shp = GetNMatrixAt(targetElement, xi, eta, 0);
                        var j = GetJMatrixAt(targetElement, xi, eta, 0);
                        shp.MultiplyByConstant(j.Determinant());
                        shp.MultiplyByConstant(uz);

                        return shp;
                    }
                );

                
                var res = intg.Integrate();

                var localForces = new Force[4];

                for(var i=0;i<4;i++)
                {
                    localForces[i] = new Force(0, 0, res[i, 0], 0, 0, 0);
                }

                throw new NotImplementedException();
                var globalForces = localForces.Select(i => tr.TransformLocalToGlobal(i)).ToArray();

                return globalForces;
            }

            #endregion

            #region non uniform
            if (load is BriefFiniteElementNet.Loads.PartialNonUniformLoad)  // TODO
            {
                //TODO
                throw new NotImplementedException();

                var ul = load as BriefFiniteElementNet.Loads.PartialNonUniformLoad;

                var localDir = ul.Direction.GetUnit();

                if (ul.CoordinationSystem == CoordinationSystem.Global)
                    localDir = tr.TransformGlobalToLocal(localDir);
                /*
                var interpolator = new Func<double, double, double>((xi,eta)=>
                {
                    var shp = GetNMatrixAt(targetElement, xi, eta, 0).Transpose();
                    var frc = new Matrix(4, 1);
                    //frc.FillColumn(0, ul.NodalMagnitudes);
                    var mult = shp * frc;

                    return mult[0, 0];
                });

                var ux = new Func<double, double, double>((xi, eta) => localDir.X * interpolator(xi, eta));
                var uy = new Func<double, double, double>((xi, eta) => localDir.Y * interpolator(xi, eta));
                var uz = new Func<double, double, double>((xi, eta) => localDir.Z * interpolator(xi, eta));
                */

                var st = ul.StartLocation;
                var en = ul.EndLocation;

                var intg = new GaussianIntegrator();

                intg.A1 = 0;
                intg.A2 = 1;

                intg.F1 = gama => st.Eta;
                intg.F2 = gama => en.Eta;
                
                intg.G1 = (eta, gama) => st.Xi;
                intg.G2 = (eta, gama) => en.Xi;

                var order = ul.SeverityFunction.Degree;

                intg.GammaPointCount = 1;
                intg.XiPointCount = intg.EtaPointCount = 2;
                
                throw new Exception();

                intg.H = new FunctionMatrixFunction((xi, eta, gama) =>
                {
                    var shp = GetNMatrixAt(targetElement, xi, eta, 0);
                    var j = GetJMatrixAt(targetElement, xi, eta, 0);
                    shp.MultiplyByConstant(j.Determinant());

                    //var uzm = ul.SeverityFunction.Evaluate(xi, eta);

                    //shp.MultiplyByConstant(uzm);

                    return shp;
                }
                );

                var res = intg.Integrate();

                var localForces = new Force[4];

                for (var i = 0; i < 4; i++)
                {
                    localForces[i] = new Force(0, 0, res[i, 0], 0, 0, 0);
                }

                return localForces;
            }

            #endregion

            throw new NotImplementedException();
        }

        public virtual FlatShellStressTensor GetLoadInternalForceAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public FlatShellStressTensor GetLoadDisplacementAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Tuple<DoF, double>> IElementHelper.GetLocalInternalForceAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Tuple<DoF, double>> IElementHelper.GetLoadInternalForceAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        Displacement IElementHelper.GetLoadDisplacementAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public Matrix CalcLocalStiffnessMatrix(Element targetElement)
        {
            return CalcLocalKMatrix(targetElement);
        }

        public Matrix CalcLocalMassMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix CalcLocalDampMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public GeneralStressTensor GetLocalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public GeneralStressTensor GetLoadStressAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }
        
        //test
    }
}
