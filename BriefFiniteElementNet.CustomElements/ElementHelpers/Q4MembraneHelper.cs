//---------------------------------------------------------------------------------------
//
// Project: VIT-V
//
// Program: BriefFiniteElement.Net - Q4MembraneHelper.cs
//
// Revision History
//
// Date          Author          	            Description
// 22.06.2020    T.Thaler, M.Mischke     	    v1.0  
// 
//---------------------------------------------------------------------------------------
// Copyleft 2017-2020 by Brandenburg University of Technology. Intellectual proprerty 
// rights reserved in terms of LGPL license. This work is a research work of Brandenburg
// University of Technology. Use or copying requires an indication of the authors reference.
//---------------------------------------------------------------------------------------

using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.ElementHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;
using CSparse.Storage;

namespace BriefFiniteElementNet.Elements.ElementHelpers
{
    class Q4MembraneHelper : IElementHelper
    {
        public Element TargetElement { get; set; }

        public Matrix CalcLocalDampMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix CalcLocalMassMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix CalcLocalStiffnessMatrix(Element targetElement)
        {
            var intg = new BriefFiniteElementNet.Integration.GaussianIntegrator(); // using eq. 3.50 [1] / eq 8 [3]

            intg.A2 = 1;
            intg.A1 = 0;

            intg.F2 = (gama => +1);
            intg.F1 = (gama => -1);

            intg.G2 = (eta, gama) => +1;
            intg.G1 = (eta, gama) => -1;

            intg.XiPointCount = intg.EtaPointCount = 3; // ref [2]
            intg.GammaPointCount = 1;

            intg.H = new FunctionMatrixFunction((xi, eta, gamma) =>
            {
                var b = GetBMatrixAt(targetElement, xi, eta);

                var d = this.GetDMatrixAt(targetElement, xi, eta);

                var j = GetJMatrixAt(targetElement, xi, eta);

                var detJ = j.Determinant();

                var ki = b.Transpose() * d * b;

                ki.MultiplyByConstant(Math.Abs(j.Determinant()));

                return ki;
            });

            var res = intg.Integrate();
            return res;
        }

        public Matrix GetAMatrix(Element targetElement, params double[] isoCoords) // ref [1]: following eq. 3.41
        {
            var xi = isoCoords[0];
            var eta = isoCoords[1];
            Matrix j = GetJMatrixAt(targetElement, xi, eta);    
            var detJ = j.Determinant(); 

            var buf = new Matrix(3, 4);
            buf.FillRow(0, j[1, 1], -j[0, 1], 0, 0);
            buf.FillRow(1, 0, 0, -j[1, 0], j[0, 0]);
            buf.FillRow(2, -j[1, 0], j[0, 0], j[1, 1], -j[0, 1]);

            buf.MultiplyByConstant(detJ);

            return buf;
        }

        public Matrix GetGMatrix(Element targetElement, params double[] isoCoords) // ref [1]: following eq. 3.45 and using N1, N1, N3, N4 from eq. 3.32
        {
            var xi = isoCoords[0];
            var eta = isoCoords[1];
            var eta_m = eta - 1;
            var eta_p = eta + 1;
            var xi_m = xi - 1;
            var xi_p = xi + 1;

            var q = (1 / 4);

            var N1_xi = q * eta_m; // δN1/δxi
            var N1_eta = q * xi_m; // δN1/δeta
            var N2_xi = -q * eta_m;
            var N2_eta = -q * xi_p;
            var N3_xi = q * eta_p;
            var N3_eta = q * xi_p;
            var N4_xi = -q * eta_p;
            var N4_eta = -q * xi_m;

            var buf = new Matrix(4, 8); //ref [1] eq. 3.45
            buf.FillRow(0, N1_xi, 0, N2_xi, 0, N3_xi, 0, N4_xi, 0);
            buf.FillRow(1, N1_eta, 0, N2_eta, 0, N3_eta, 0, N4_eta, 0);
            buf.FillRow(2, 0, N1_xi, 0, N2_xi, 0, N3_xi, 0, N4_xi);
            buf.FillRow(3, 0, N1_eta, 0, N2_eta, 0, N3_eta, 0, N4_eta);
            
            return buf;
        }


        public Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords) // ref [1]: eq. 3.47: B = A * G
        {
            Matrix A = GetAMatrix(targetElement, isoCoords);
            Matrix G = GetGMatrix(targetElement, isoCoords);

            return Matrix.Multiply(A, G); 
        }

        public int[] GetBMaxOrder(Element targetElement)
        {
            return new int[] { 2, 2, 0 }; // xi, eta, gamma -> max. order
        }

        public int[] GetDetJOrder(Element targetElement)
        {
            return new int[] { 1, 1, 0 };
        }

        public Matrix GetDMatrixAt(Element targetElement, params double[] isoCoords) // source is CST-Element --> stays the same (made by epsi1on)
        {
            var quad = targetElement as QuadrilaturalElement;

            if (quad == null)
                throw new Exception();

            var d = new Matrix(3, 3);

            var mat = quad.Material.GetMaterialPropertiesAt(isoCoords);


            if (quad.MembraneFormulation == MembraneFormulation.PlaneStress)
            {
                //http://help.solidworks.com/2013/english/SolidWorks/cworks/c_linear_elastic_orthotropic.htm
                //orthotropic material
                d[0, 0] = mat.Ex / (1 - mat.NuXy * mat.NuYx);
                d[1, 1] = mat.Ey / (1 - mat.NuXy * mat.NuYx);
                d[1, 0] = d[0, 1] = mat.NuXy * mat.Ey / (1 - mat.NuXy * mat.NuYx);

                d[2, 2] = mat.Ex * mat.Ey / (mat.Ex + mat.Ey + 2 * mat.Ey * mat.NuXy);
            }
            else
            {
                var delta = 1 - mat.NuXy * mat.NuYx - mat.NuZy * mat.NuYz - mat.NuZx * mat.NuXz - 2 * mat.NuXy * mat.NuYz * mat.NuZx;

                delta /= mat.Ex * mat.Ey * mat.Ez;

                //http://www.efunda.com/formulae/solid_mechanics/mat_mechanics/hooke_orthotropic.cfm

                d[0, 0] = (1 - mat.NuYz * mat.NuZy) / (mat.Ey * mat.Ez * delta);
                d[0, 1] = (mat.NuYx + mat.NuZx * mat.NuYz) / (mat.Ey * mat.Ez * delta);
                d[1, 0] = (mat.NuXy + mat.NuXz * mat.NuZy) / (mat.Ez * mat.Ex * delta);
                d[1, 1] = (1 - mat.NuZx * mat.NuXz) / (mat.Ez * mat.Ex * delta);
            }

            return d;
        }

        public ElementPermuteHelper.ElementLocalDof[] GetDofOrder(Element targetElement) // made by epsi1on
        {
            var buf = new ElementPermuteHelper.ElementLocalDof[]
            {
                new ElementPermuteHelper.ElementLocalDof(0, DoF.Dx),
                new ElementPermuteHelper.ElementLocalDof(0, DoF.Dy),

                new ElementPermuteHelper.ElementLocalDof(1, DoF.Dx),
                new ElementPermuteHelper.ElementLocalDof(1, DoF.Dy),

                new ElementPermuteHelper.ElementLocalDof(2, DoF.Dx),
                new ElementPermuteHelper.ElementLocalDof(2, DoF.Dy),

                new ElementPermuteHelper.ElementLocalDof(3, DoF.Dx),
                new ElementPermuteHelper.ElementLocalDof(3, DoF.Dy),
            };

            return buf;
        }

        public Matrix GetJMatrixAt(Element targetElement, params double[] isoCoords)
        {
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

            var p1l = tmgr.TransformGlobalToLocal(p1g);
            var p2l = tmgr.TransformGlobalToLocal(p2g);
            var p3l = tmgr.TransformGlobalToLocal(p3g);
            var p4l = tmgr.TransformGlobalToLocal(p4g);

            var x1 = p1l.X;
            var x2 = p2l.X;
            var x3 = p3l.X;
            var x4 = p4l.X;

            var y1 = p1l.Y;
            var y2 = p2l.Y;
            var y3 = p3l.Y;
            var y4 = p4l.Y;

            var buf = new Matrix(2, 2); //ref [1] eq. 3.36 and 3.28/3.29

            var eta_m = eta - 1;
            var eta_p = eta + 1;
            var xi_m = xi - 1;
            var xi_p = xi + 1;

            var q = 0.25;

            #region Test
            /*
            var j00 = q * ((x1 * eta_m) - (x2 * eta_m) + (x3 * eta_p) - (x4 * eta_p));
            var j01 = q * ((y1 * eta_m) - (y2 * eta_m) + (y3 * eta_p) - (y4 * eta_p));
            var j10 = q * ((x1 * xi_m) - (x2 * xi_p) + (x3 * xi_p) - (x4 * xi_m));
            var j11 = q * ((y1 * xi_m) - (y2 * xi_p) + (y3 * xi_p) - (y4 * xi_m));
            */
            #endregion

            buf[0, 0] = q * ((x1 * eta_m) - (x2 * eta_m) + (x3 * eta_p) - (x4 * eta_p));  // δx/δxi ; x(xi, eta) = N1*x1 + N2*x2 + N3*x3 + N4*x4 ; y(xi, eta) = N1*y1 + N2*y2 + N3*y3 + N4*y4 (N1, N2, N3, N4 from 3.32)
            buf[0, 1] = q * ((y1 * eta_m) - (y2 * eta_m) + (y3 * eta_p) - (y4 * eta_p)); // δy/δxi
            buf[1, 0] = q * ((x1 * xi_m) - (x2 * xi_p) + (x3 * xi_p) - (x4 * xi_m));      // δx/δeta
            buf[1, 1] = q * ((y1 * xi_m) - (y2 * xi_p) + (y3 * xi_p) - (y4 * xi_m));      // δy/δeta

            #region Test
            /*
            buf.FillRow(0, (1 / 4) * (x1 * eta_m - x2 * eta_m + x3 * eta_p - x4 * eta_p), (1 / 4) * (y1 * eta_m - y2 * eta_m + y3 * eta_p - y4 * eta_p));
            buf.FillRow(1, (1 / 4) * (x1 * xi_m - x2 * xi_p + x3 * xi_p - x4 * xi_m), (1 / 4) * (y1 * xi_m - y2 * xi_p + y3 * xi_p - y4 * xi_m));
            */
            #endregion
            return buf;
        }

        public Displacement GetLoadDisplacementAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<DoF, double>> GetLoadInternalForceAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public GeneralStressTensor GetLoadStressAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public Displacement GetLocalDisplacementAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Force[] GetLocalEquivalentNodalLoads(Element targetElement, ElementalLoad load)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<DoF, double>> GetLocalInternalForceAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public GeneralStressTensor GetLocalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            // Displacements:
            var d1l = localDisplacements[0];
            var d2l = localDisplacements[1];
            var d3l = localDisplacements[2];
            var d4l = localDisplacements[3];

            var u = new Matrix(new[] {d1l.DX, d1l.DY, d2l.DX, d2l.DY, d3l.DX, d3l.DY, d4l.DX, d4l.DY});
            var d = this.GetDMatrixAt(targetElement, isoCoords);
            var b = this.GetBMatrixAt(targetElement, isoCoords);

            var sQ4 = d * b * u;

            var buf = new MembraneStressTensor()
            { 
                Sx = sQ4[0, 0],
                Sy = sQ4[1, 0],
                Txy = sQ4[2, 0]
            };

            return new GeneralStressTensor(buf);
        }

        public Matrix GetMuMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public int[] GetNMaxOrder(Element targetElement)
        {
            return new int[] {1, 1, 0 };
        }

        public Matrix GetRhoMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }
    }
}
