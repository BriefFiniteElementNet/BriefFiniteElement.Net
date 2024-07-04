//---------------------------------------------------------------------------------------
//
// Project: VIT-V
//
// Program: BriefFiniteElement.Net - Q4MembraneHelper.cs
//
// Revision History
//
// Date          Author          	            Description
// 30.06.2020    T.Thaler, M.Mischke     	    v1.0  
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
    public class Q4MembraneHelper : IElementHelper
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
            var quad = targetElement as QuadrilaturalElement;

            intg.A2 = 1.0;
            intg.A1 = 0.0;

            intg.F2 = (gama => +1.0);
            intg.F1 = (gama => -1.0);

            intg.G2 = (eta, gama) => +1.0;
            intg.G1 = (eta, gama) => -1.0;

            intg.XiPointCount = intg.EtaPointCount = 3; // ref [2]
            intg.GammaPointCount = 1;

            intg.H = new FunctionMatrixFunction((xi, eta, gamma) =>
            {
                var b = GetBMatrixAt(targetElement, xi, eta);

                var d = this.GetDMatrixAt(targetElement, xi, eta);

                var j = GetJMatrixAt(targetElement, xi, eta);

                var detJ = j.Determinant();

                var ki = b.Transpose() * d * b;

                ki.Scale(Math.Abs(j.Determinant()));
                ki.Scale(quad.Section.GetThicknessAt(new double[] { xi, eta }));

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

            // TODO: MAT - set values directly
            var buf = targetElement.MatrixPool.Allocate(3, 4);
            buf.SetRow(0, new double[] { j[1, 1], -j[0, 1], 0, 0 });
            buf.SetRow(1, new double[] { 0, 0, -j[1, 0], j[0, 0] });
            buf.SetRow(2, new double[] { -j[1, 0], j[0, 0], j[1, 1], -j[0, 1] });

            //buf.MultiplyByConstant(detJ);
            //should be 1.0/detJ? See 3.41
            buf.Scale(1.0 / detJ);

            return buf;
        }

        public Matrix GetGMatrix(Element targetElement, params double[] isoCoords) // ref [1]: following eq. 3.45 and using N1, N1, N3, N4 from eq. 3.32
        {
            var xi = isoCoords[0];
            var eta = isoCoords[1];
            var eta_m = eta - 1.0;
            var eta_p = eta + 1.0;
            var xi_m = xi - 1.0;
            var xi_p = xi + 1.0;

            var q = 0.25;

            var N1_xi = q * eta_m; // δN1/δxi
            var N1_eta = q * xi_m; // δN1/δeta
            var N2_xi = -q * eta_m;
            var N2_eta = -q * xi_p;
            var N3_xi = q * eta_p;
            var N3_eta = q * xi_p;
            var N4_xi = -q * eta_p;
            var N4_eta = -q * xi_m;

            // TODO: MAT - set values directly
            var buf = targetElement.MatrixPool.Allocate(4, 8); //ref [1] eq. 3.45
            buf.SetRow(0, new double[] { N1_xi, 0, N2_xi, 0, N3_xi, 0, N4_xi, 0 });
            buf.SetRow(1, new double[] { N1_eta, 0, N2_eta, 0, N3_eta, 0, N4_eta, 0 });
            buf.SetRow(2, new double[] { 0, N1_xi, 0, N2_xi, 0, N3_xi, 0, N4_xi });
            buf.SetRow(3, new double[] { 0, N1_eta, 0, N2_eta, 0, N3_eta, 0, N4_eta });
            
            return buf;
        }


        public Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords) // ref [1]: eq. 3.47: B = A * G
        {
            Matrix A = GetAMatrix(targetElement, isoCoords);
            Matrix G = GetGMatrix(targetElement, isoCoords);

            return A.Multiply(G).AsMatrix(); 
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

            var d = targetElement.MatrixPool.Allocate(3, 3);

            var mat = quad.Material.GetMaterialPropertiesAt(isoCoords);


            if (quad.MembraneFormulation == MembraneFormulation.PlaneStress)
            {
                //http://help.solidworks.com/2013/english/SolidWorks/cworks/c_linear_elastic_orthotropic.htm
                //orthotropic material
               d[0, 0] = mat.Ex / (1.0 - mat.NuXy * mat.NuYx);
                d[1, 1] = mat.Ey / (1.0 - mat.NuXy * mat.NuYx);
                d[1, 0] = d[0, 1] = mat.NuXy * mat.Ey / (1.0 - mat.NuXy * mat.NuYx);

                d[2, 2] = mat.Ex * mat.Ey / (mat.Ex + mat.Ey + 2.0 * mat.Ey * mat.NuXy);
            }
            else
            {
                var delta = 1.0 - mat.NuXy * mat.NuYx - mat.NuZy * mat.NuYz - mat.NuZx * mat.NuXz - 2.0* mat.NuXy * mat.NuYz * mat.NuZx;

                delta /= mat.Ex * mat.Ey * mat.Ez;

                //http://www.efunda.com/formulae/solid_mechanics/mat_mechanics/hooke_orthotropic.cfm

                d[0, 0] = (1.0 - mat.NuYz * mat.NuZy) / (mat.Ey * mat.Ez * delta);
                d[0, 1] = (mat.NuYx + mat.NuZx * mat.NuYz) / (mat.Ey * mat.Ez * delta);
                d[1, 0] = (mat.NuXy + mat.NuXz * mat.NuZy) / (mat.Ez * mat.Ex * delta);
                d[1, 1] = (1.0 - mat.NuZx * mat.NuXz) / (mat.Ez * mat.Ex * delta);
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

            var buf = targetElement.MatrixPool.Allocate(2, 2); //ref [1] eq. 3.36 and 3.28/3.29

            var eta_m = eta - 1.0;
            var eta_p = eta + 1.0;
            var xi_m = xi - 1.0;
            var xi_p = xi + 1.0;

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
            var tr = targetElement.GetTransformationManager();

            #region uniform

            if (load is BriefFiniteElementNet.Loads.UniformLoad)
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
                    shp.Scale(j.Determinant());
                    shp.Scale(uz);

                    return shp;
                }
                );


                var res = intg.Integrate();

                var localForces = new Force[4];

                for (var i = 0; i < 4; i++)
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
                    shp.Scale(j.Determinant());

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

            var u = targetElement.MatrixPool.Allocate(new[] {d1l.DX, d1l.DY, d2l.DX, d2l.DY, d3l.DX, d3l.DY, d4l.DX, d4l.DY});
            var d = this.GetDMatrixAt(targetElement, isoCoords);
            var b = this.GetBMatrixAt(targetElement, isoCoords);

            var sQ4 = d * b * u;

            var buf = new CauchyStressTensor()
            {
                S11 = sQ4[0, 0],
                S22 = sQ4[1, 0],
                S12 = sQ4[2, 0],
                S21 = sQ4[2, 0]
            };

            return new GeneralStressTensor(buf);
        }
        /// <summary>
        /// Gets the stresses for a single element
        /// </summary>
        /// <param name="targetElement"></param>
        /// <param name="loadCase"></param>
        /// <param name="isoCoords"></param>
        /// <returns></returns>
        public GeneralStressTensor GetLocalInternalStress(Element targetElement, LoadCase loadCase, params double[] isoCoords)
        {
            //step 1 : get transformation matrix
            //step 2 : convert globals points to locals
            //step 3 : convert global displacements to locals
            //step 4 : calculate B matrix and D matrix
            //step 5 : M=D*B*U
            //Note : Steps changed...
            var lds = new Displacement[targetElement.Nodes.Length];
            var tr = targetElement.GetTransformationManager();

            for (var i = 0; i < targetElement.Nodes.Length; i++)
            {
                var globalD = targetElement.Nodes[i].GetNodalDisplacement(loadCase);
                var local = tr.TransformGlobalToLocal(globalD);
                lds[i] = local;
            }

            var d1l = lds[0];
            var d2l = lds[1];
            var d3l = lds[2];
            var d4l = lds[3];

            var u = targetElement.MatrixPool.Allocate(new[] { d1l.DX, d1l.DY, d2l.DX, d2l.DY, d3l.DX, d3l.DY, d4l.DX, d4l.DY });
            var d = this.GetDMatrixAt(targetElement, isoCoords);
            var b = this.GetBMatrixAt(targetElement, isoCoords);

            var sQ4 = d * b * u;

            //new type of stress tensor
            var buf = new CauchyStressTensor()
            {
                S11 = sQ4[0, 0],
                S22 = sQ4[1, 0],
                S12 = sQ4[2, 0],
                S21 = sQ4[2,0]
            };

            return new GeneralStressTensor(buf);
        }
        /// <summary>
        /// Gets the stresses for a single element
        /// </summary>
        /// <param name="targetElement"></param>
        /// <param name="loadCase"></param>
        /// <param name="isoCoords"></param>
        /// <returns></returns>
        public GeneralStressTensor GetLocalInternalStress(Element targetElement, LoadCombination cmb, params double[] isoCoords)
        {
            //step 1 : get transformation matrix
            //step 2 : convert globals points to locals
            //step 3 : convert global displacements to locals
            //step 4 : calculate B matrix and D matrix
            //step 5 : M=D*B*U
            //Note : Steps changed...
            var lds = new Displacement[targetElement.Nodes.Length];
            var tr = targetElement.GetTransformationManager();

            for (var i = 0; i < targetElement.Nodes.Length; i++)
            {
                var globalD = targetElement.Nodes[i].GetNodalDisplacement(cmb);
                var local = tr.TransformGlobalToLocal(globalD);
                lds[i] = local;
            }

            var d1l = lds[0];
            var d2l = lds[1];
            var d3l = lds[2];
            var d4l = lds[3];

            var u = targetElement.MatrixPool.Allocate(new[] { d1l.DX, d1l.DY, d2l.DX, d2l.DY, d3l.DX, d3l.DY, d4l.DX, d4l.DY });
            var d = this.GetDMatrixAt(targetElement, isoCoords);
            var b = this.GetBMatrixAt(targetElement, isoCoords);

            var sQ4 = d * b * u;

            //new type of stress tensor
            var buf = new CauchyStressTensor()
            {
                S11 = sQ4[0, 0],
                S22 = sQ4[1, 0],
                S12 = sQ4[2, 0],
                S21 = sQ4[2, 0]
            };

            return new GeneralStressTensor(buf);
        }


        public Matrix GetMuMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords)
        {
            //used for distributed load, 8.26 of http://what-when-how.com/the-finite-element-method/fem-for-plates-and-shells-finite-element-method-part-1/

            //N from 7.54 of http://what-when-how.com/the-finite-element-method/fem-for-two-dimensional-solids-finite-element-method-part-2/
            var xi = isoCoords[0];
            var eta = isoCoords[1];

            var buf = targetElement.MatrixPool.Allocate(4, 1);

            var xis = new double[] { -1, 1, 1, -1 };//for each node
            var etas = new double[] { -1, -1, 1, 1 };//for each node

            for (var i = 0; i < 4; i++)
            {
                buf[i, 0] = 0.25 * (1 + xis[i] * xi) * (1 + etas[i] * eta);
            }

            return buf;
        }

        public int[] GetNMaxOrder(Element targetElement)
        {
            return new int[] {1, 1, 0 };
        }

        public Matrix GetRhoMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        #region strain
        public StrainTensor GetMembraneInternalStrain(Element targetElement, LoadCase loadCase, params double[] isoCoords)
        {
            //Note: membrane internal force is constant

            //step 1 : get transformation matrix
            //step 2 : convert globals points to locals
            //step 3 : convert global displacements to locals
            //step 4 : calculate B matrix
            //step 5 : e=B*U
            //Note : Steps changed...
            var lds = new Displacement[targetElement.Nodes.Length];
            var tr = targetElement.GetTransformationManager();

            for (var i = 0; i < targetElement.Nodes.Length; i++)
            {
                var globalD = targetElement.Nodes[i].GetNodalDisplacement(loadCase);
                var local = tr.TransformGlobalToLocal(globalD);
                lds[i] = local;
            }

            // var locals = tr.TransformGlobalToLocal(globalDisplacements);

            var b = GetBMatrixAt(targetElement, isoCoords);

            var u1l = lds[0];
            var u2l = lds[1];
            var u3l = lds[2];
            var u4l = lds[3];

            var uQ4 =
                   targetElement.MatrixPool.Allocate(new[]
                   {u1l.DX, u1l.DY, /**/ u2l.DX, u2l.DY, /**/ u3l.DX, u3l.DY, u4l.DX, u4l.DY});

            var EQ4 = b * uQ4;

            var buf = new StrainTensor();

            buf.S11 = EQ4[0, 0];
            buf.S22 = EQ4[1, 0];
            buf.S12 = EQ4[2, 0];

            return buf;
        }

        public GeneralStressTensor GetLocalInternalStressAt(Element targetElement, Displacement[] lds, params double[] isoCoords)
        {
            //step 1 : get transformation matrix
            //step 2 : convert globals points to locals
            //step 3 : convert global displacements to locals
            //step 4 : calculate B matrix and D matrix
            //step 5 : M=D*B*U
            //Note : Steps changed...

            var d1l = lds[0];
            var d2l = lds[1];
            var d3l = lds[2];
            var d4l = lds[3];

            var u = Matrix.OfVector(new[] { d1l.DX, d1l.DY, d2l.DX, d2l.DY, d3l.DX, d3l.DY, d4l.DX, d4l.DY });
            var d = this.GetDMatrixAt(targetElement, isoCoords);
            var b = this.GetBMatrixAt(targetElement, isoCoords);

            var sQ4 = d * b * u;

            //new type of stress tensor
            var buf = new CauchyStressTensor()
            {
                S11 = sQ4[0, 0],
                S22 = sQ4[1, 0],
                S12 = sQ4[2, 0],
                S21 = sQ4[2, 0]
            };

            return new GeneralStressTensor(buf);
        }

        public ILoadHandler[] GetLoadHandlers()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
