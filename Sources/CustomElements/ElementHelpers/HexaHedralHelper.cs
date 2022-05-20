using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Integration;
using BriefFiniteElementNet.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements.ElementHelpers
{
    [Obsolete("not fully implemented yet")]
    public class HexaHedralHelper : IElementHelper
    {
        public Element TargetElement { get; set; }

        public Matrix CalcLocalDampMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix CalcLocalMassMatrix(Element targetElement)
        {
            var hex = targetElement as HexahedralElement;
            //http://what-when-how.com/the-finite-element-method/fem-for-3d-solids-finite-element-method-part-2/
            var j = GetJMatrixAt(targetElement, new double[] { 0, 0, 0 });
            var v = Math.Abs(j.Determinant());

            var buf = targetElement.MatrixPool.Allocate(8, 8);

            for (var i = 0; i < 8; i++)
            {
                buf[i, i] = 8.0;
            }
            buf[1, 0] = buf[0, 1] = 4.0;
            buf[2, 0] = buf[0, 2] = 2.0;
            buf[3, 0] = buf[0, 3] = 4.0;
            buf[4, 0] = buf[0, 4] = 4.0;
            buf[5, 0] = buf[0, 5] = 2.0;
            buf[6, 0] = buf[0, 6] = 1.0;
            buf[7, 0] = buf[0, 7] = 2.0;

            buf[2, 1] = buf[1, 2] = 4.0;
            buf[3, 1] = buf[1, 3] = 2.0;
            buf[4, 1] = buf[1, 4] = 2.0;
            buf[5, 1] = buf[1, 5] = 4.0;
            buf[6, 1] = buf[1, 6] = 2.0;
            buf[7, 1] = buf[1, 7] = 1.0;

            buf[3, 2] = buf[2, 3] = 4.0;
            buf[4, 2] = buf[2, 4] = 1.0;
            buf[5, 2] = buf[2, 5] = 2.0;
            buf[6, 2] = buf[2, 6] = 4.0;
            buf[7, 2] = buf[2, 7] = 2.0;

            buf[4, 3] = buf[3, 4] = 2.0;
            buf[5, 3] = buf[3, 5] = 1.0;
            buf[6, 3] = buf[3, 6] = 2.0;
            buf[7, 3] = buf[3, 7] = 4.0;

            buf[5, 4] = buf[4, 5] = 4.0;
            buf[6, 4] = buf[4, 6] = 2.0;
            buf[7, 4] = buf[4, 7] = 4.0;

            buf[6, 5] = buf[5, 6] = 4.0;
            buf[7, 5] = buf[5, 7] = 2.0;

            buf[7, 6] = buf[6, 7] = 4.0;

            var density = hex.Material.GetMaterialPropertiesAt(0).Rho;//density considered as constant

            buf.Scale(density);

            return buf;
        }

        public Matrix CalcLocalStiffnessMatrix(Element targetElement)
        {
            var intg = new GaussianIntegrator();

            intg.GammaPointCount = 2;
            intg.XiPointCount = 2;
            intg.EtaPointCount = 2;

            intg.A2 = 1;
            intg.A1 = -1;

            intg.F2 = (gama => +1);
            intg.F1 = (gama => -1);

            intg.G2 = (eta, gama) => +1;
            intg.G1 = (eta, gama) => -1;

            intg.H = new FunctionMatrixFunction((xi, eta, gamma) =>
            {
                var b = this.GetBMatrixAt(targetElement, new double[] { xi, eta, gamma });
                var d = this.GetDMatrixAt(targetElement, new double[] { xi, eta, gamma });
                var j = this.GetJMatrixAt(targetElement, new double[] { xi, eta, gamma });

                var buf = targetElement.MatrixPool.Allocate(b.ColumnCount, b.ColumnCount);

                CalcUtil.Bt_D_B(b, d, buf);

                //var detj = Math.Abs(j.Determinant());

                //buf.MultiplyByConstant(detj);

                return buf;
            });

            var res = intg.Integrate();

            return res;
        }

        public virtual Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var J = GetJMatrixAt(targetElement, isoCoords);
            var detJ = J.Determinant();

            //  var V = detJ;
            // if (V < 0)
            // throw new Exception();

            var xi = isoCoords[0];
            var eta = isoCoords[1];
            var zeta = isoCoords[2];

            var n1 = targetElement.Nodes[0].Location;
            var n2 = targetElement.Nodes[1].Location;
            var n3 = targetElement.Nodes[2].Location;
            var n4 = targetElement.Nodes[3].Location;
            var n5 = targetElement.Nodes[4].Location;
            var n6 = targetElement.Nodes[5].Location;
            var n7 = targetElement.Nodes[6].Location;
            var n8 = targetElement.Nodes[7].Location;

            var x1 = n1.X;
            var x2 = n2.X;
            var x3 = n3.X;
            var x4 = n4.X;
            var x5 = n5.X;
            var x6 = n6.X;
            var x7 = n7.X;
            var x8 = n8.X;

            var y1 = n1.Y;
            var y2 = n2.Y;
            var y3 = n3.Y;
            var y4 = n4.Y;
            var y5 = n5.Y;
            var y6 = n6.Y;
            var y7 = n7.Y;
            var y8 = n8.Y;

            var z1 = n1.Z;
            var z2 = n2.Z;
            var z3 = n3.Z;
            var z4 = n4.Z;
            var z5 = n5.Z;
            var z6 = n6.Z;
            var z7 = n7.Z;
            var z8 = n8.Z;

            //d(xi)
            var J11 = 1.0 / 8.0 * (-1.0 - eta + zeta + zeta * eta);
            var J12 = 1.0 / 8.0 * (-1.0 - eta - zeta - zeta * eta);
            var J13 = 1.0 / 8.0 * (1.0 - eta - zeta + zeta * eta);
            var J14 = 1.0 / 8.0 * (1.0 + eta + zeta + zeta * eta);
            var J15 = 1.0 / 8.0 * (-1.0 - eta + zeta + eta * zeta);
            var J16 = 1.0 / 8.0 * (-1.0 + eta - zeta + eta * zeta);
            var J17 = 1.0 / 8.0 * (1.0 + eta - zeta - eta * zeta);
            var J18 = 1.0 / 8.0 * (1.0 - eta - zeta + eta * zeta);


            //d(eta)
            var J21 = 1.0 / 8.0 * (-1.0 - xi + zeta + zeta * xi);
            var J23 = 1.0 / 8.0 * (-1.0 - xi + zeta + zeta * xi);
            var J24 = 1.0 / 8.0 * (1.0 + xi + zeta + zeta * xi);
            var J25 = 1.0 / 8.0 * (1.0 - xi - zeta + zeta * xi);
            var J22 = 1.0 / 8.0 * (1.0 - xi + zeta - zeta * xi);
            var J26 = 1.0 / 8.0 * (-1.0 + xi - zeta + zeta * xi);
            var J27 = 1.0 / 8.0 * (-1.0 + xi + zeta - zeta * xi);
            var J28 = 1.0 / 8.0 * (-1.0 - xi + zeta + zeta * xi);


            //d(zeta)
            var J31 = 1.0 / 8.0 * (-1.0 + xi + eta + eta * xi);
            var J33 = 1.0 / 8.0 * (1.0 - xi + eta - eta * xi);
            var J34 = 1.0 / 8.0 * (-1.0 - xi + eta + eta * xi);
            var J35 = 1.0 / 8.0 * (1.0 + xi + eta + eta * xi);
            var J32 = 1.0 / 8.0 * (-1.0 + xi - eta + eta * xi);
            var J36 = 1.0 / 8.0 * (1.0 - xi - eta + eta * xi);
            var J37 = 1.0 / 8.0 * (-1.0 - xi + eta - eta * xi);
            var J38 = 1.0 / 8.0 * (-1.0 - xi + eta + eta * xi);

            var B10 = targetElement.MatrixPool.Allocate(3, 1);
            //B10.FillMatrixRowise(J11, J21, J31);
            // TODO: MAT - set values directly
            B10.SetColumn(0, new double[] { J11, J21, J31 });
            var B1 = J.Inverse() * B10;

            /*
            var B20 = targetElement.MatrixPool.Allocate(3, 1);
            B20.FillMatrixRowise(J12, J22, J32);
            var B2 = J.Inverse() * B20;

            var B30 = targetElement.MatrixPool.Allocate(3, 1);
            B30.FillMatrixRowise(J13, J23, J33);
            var B3 = J.Inverse() * B30;

            var B40 = targetElement.MatrixPool.Allocate(3, 1);
            B40.FillMatrixRowise(J14, J24, J34);
            var B4 = J.Inverse() * B40;

            var B50 = targetElement.MatrixPool.Allocate(3, 1);
            B50.FillMatrixRowise(J15, J25, J35);
            var B5 = J.Inverse() * B50;

            var B60 = targetElement.MatrixPool.Allocate(3, 1);
            B60.FillMatrixRowise(J16, J26, J36);
            var B6 = J.Inverse() * B60;

            var B70 = targetElement.MatrixPool.Allocate(3, 1);
            B70.FillMatrixRowise(J17, J27, J37);
            var B7 = J.Inverse() * B70;

            var B80 = targetElement.MatrixPool.Allocate(3, 1);
            B80.FillMatrixRowise(J18, J28, J38);
            var B8 = J.Inverse() * B80;
            //*/

            double a1 = B1[0, 0]; double b1 = B1[1, 0]; double c1 = B1[2, 0];
            double a2 = B1[0, 0]; double b2 = B1[1, 0]; double c2 = B1[2, 0];
            double a3 = B1[0, 0]; double b3 = B1[1, 0]; double c3 = B1[2, 0];
            double a4 = B1[0, 0]; double b4 = B1[1, 0]; double c4 = B1[2, 0];
            double a5 = B1[0, 0]; double b5 = B1[1, 0]; double c5 = B1[2, 0];
            double a6 = B1[0, 0]; double b6 = B1[1, 0]; double c6 = B1[2, 0];
            double a7 = B1[0, 0]; double b7 = B1[1, 0]; double c7 = B1[2, 0];
            double a8 = B1[0, 0]; double b8 = B1[1, 0]; double c8 = B1[2, 0];

            // TODO: MAT - use matrix pool
            // var b = targetElement.MatrixPool.Allocate(6, 24);

            // transpose of b
            var b = Matrix.OfRowMajor(6, 24, new double[] {
                a1, 0, 0, a2, 0, 0, a3, 0, 0, a4, 0, 0, a5, 0, 0, a6, 0, 0, a7, 0, 0, a8, 0, 0,
                0, b1, 0, 0, b2, 0, 0, b3, 0, 0, b4, 0, 0, b5, 0, 0, b6, 0, 0, b7, 0, 0, b8, 0,
                0, 0, c1, 0, 0, c2, 0, 0, c3, 0, 0, c4, 0, 0, c5, 0, 0, c6, 0, 0, c7, 0, 0, c8,
                0, c1, b1, 0, c2, b2, 0, c3, b3, 0, c4, b4, 0, c5, b5, 0, c6, b6, 0, c7, b7, 0, c8, b8,
                c1, 0, a1, c2, 0, a2, c3, 0, a3, c4, 0, a4, c5, 0, a5, c6, 0, a6, c7, 0, a7, c8, 0, a8,
                b1, a1, 0, b2, a2, 0, b3, a3, 0, b4, a4, 0, b5, a5, 0, b6, a6, 0, b7, a7, 0, b8, a8, 0 });

            return b.AsMatrix();
        }

        public int[] GetBMaxOrder(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public int[] GetDetJOrder(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix GetDMatrixAt(Element targetElement, params double[] isoCoords)
        {
            //Gets the consitutive matrix. Only for isotropic materials!!!If orthotropic is needed: check http://web.mit.edu/16.20/homepage/3_Constitutive/Constitutive_files/module_3_with_solutions.pdf
            var props = (targetElement as HexahedralElement).Material.GetMaterialPropertiesAt(isoCoords);
            
            //material considered to be isotropic
            var miu = props.NuXy;
            var e = props.Ex;

            // var D = targetElement.MatrixPool.Allocate(6, 6);

            var s = (1 - miu);
            // TODO: MAT - use matrix pool
            var D = Matrix.OfRowMajor(6, 6, new double[] { 1, miu / s, miu / s, 0, 0, 0, miu / s, 1, miu / s, 0, 0, 0, miu / s, miu / s, 1, 0, 0, 0, 0, 0, 0,
                (1 - 2 * miu) / (2 * s), 0, 0, 0, 0, 0, 0, (1 - 2 * miu) / (2 * s), 0, 0, 0, 0, 0, 0, (1 - 2 * miu) / (2 * s) });

            D.Scale(e * (1 - miu) / ((1 + miu) * (1 - 2 * miu)));

            return D.AsMatrix();
        }

        public ElementPermuteHelper.ElementLocalDof[] GetDofOrder(Element targetElement)
        {
            var buf = new ElementPermuteHelper.ElementLocalDof[]
           {
                new ElementPermuteHelper.ElementLocalDof(0, DoF.Dx),
                new ElementPermuteHelper.ElementLocalDof(0, DoF.Dy),
                new ElementPermuteHelper.ElementLocalDof(0, DoF.Dz),

                new ElementPermuteHelper.ElementLocalDof(1, DoF.Dx),
                new ElementPermuteHelper.ElementLocalDof(1, DoF.Dy),
                new ElementPermuteHelper.ElementLocalDof(1, DoF.Dz),

                new ElementPermuteHelper.ElementLocalDof(2, DoF.Dx),
                new ElementPermuteHelper.ElementLocalDof(2, DoF.Dy),
                new ElementPermuteHelper.ElementLocalDof(2, DoF.Dz),

                new ElementPermuteHelper.ElementLocalDof(3, DoF.Dx),
                new ElementPermuteHelper.ElementLocalDof(3, DoF.Dy),
                new ElementPermuteHelper.ElementLocalDof(3, DoF.Dz),
           };

            return buf;
        }

        public virtual Matrix GetJMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var xi = isoCoords[0];
            var eta = isoCoords[1];
            var zeta = isoCoords[2];

            var n1 = targetElement.Nodes[0].Location;
            var n2 = targetElement.Nodes[1].Location;
            var n3 = targetElement.Nodes[2].Location;
            var n4 = targetElement.Nodes[3].Location;
            var n5 = targetElement.Nodes[4].Location;
            var n6 = targetElement.Nodes[5].Location;
            var n7 = targetElement.Nodes[6].Location;
            var n8 = targetElement.Nodes[7].Location;

            var x1 = n1.X;
            var x2 = n2.X;
            var x3 = n3.X;
            var x4 = n4.X;
            var x5 = n5.X;
            var x6 = n6.X;
            var x7 = n7.X;
            var x8 = n8.X;

            var y1 = n1.Y;
            var y2 = n2.Y;
            var y3 = n3.Y;
            var y4 = n4.Y;
            var y5 = n5.Y;
            var y6 = n6.Y;
            var y7 = n7.Y;
            var y8 = n8.Y;

            var z1 = n1.Z;
            var z2 = n2.Z;
            var z3 = n3.Z;
            var z4 = n4.Z;
            var z5 = n5.Z;
            var z6 = n6.Z;
            var z7 = n7.Z;
            var z8 = n8.Z;

            //d(xi)
            var J11 = 1.0 / 8.0 * (-1.0 - eta + zeta + zeta * eta);
            var J12 = 1.0 / 8.0 * (-1.0 - eta - zeta - zeta * eta);
            var J13 = 1.0 / 8.0 * (1.0 - eta - zeta + zeta * eta);
            var J14 = 1.0 / 8.0 * (1.0 + eta + zeta + zeta * eta);
            var J15 = 1.0 / 8.0 * (-1.0 - eta + zeta + eta * zeta);
            var J16 = 1.0 / 8.0 * (-1.0 + eta - zeta + eta * zeta);
            var J17 = 1.0 / 8.0 * (1.0 + eta - zeta - eta * zeta);
            var J18 = 1.0 / 8.0 * (1.0 - eta - zeta + eta * zeta);


            //d(eta)
            var J21 = 1.0 / 8.0 * (-1.0 - xi + zeta + zeta * xi);
            var J23 = 1.0 / 8.0 * (-1.0 - xi + zeta + zeta * xi);
            var J24 = 1.0 / 8.0 * (1.0 + xi + zeta + zeta * xi);
            var J25 = 1.0 / 8.0 * (1.0 - xi - zeta + zeta * xi);
            var J22 = 1.0 / 8.0 * (1.0 - xi + zeta - zeta * xi);
            var J26 = 1.0 / 8.0 * (-1.0 + xi - zeta + zeta * xi);
            var J27 = 1.0 / 8.0 * (-1.0 + xi + zeta - zeta * xi);
            var J28 = 1.0 / 8.0 * (-1.0 - xi + zeta + zeta * xi);


            //d(zeta)
            var J31 = 1.0 / 8.0 * (-1.0 + xi + eta + eta * xi);
            var J33 = 1.0 / 8.0 * (1.0 - xi + eta - eta * xi);
            var J34 = 1.0 / 8.0 * (-1.0 - xi + eta + eta * xi);
            var J35 = 1.0 / 8.0 * (1.0 + xi + eta + eta * xi);
            var J32 = 1.0 / 8.0 * (-1.0 + xi - eta + eta * xi);
            var J36 = 1.0 / 8.0 * (1.0 - xi - eta + eta * xi);
            var J37 = 1.0 / 8.0 * (-1.0 - xi + eta - eta * xi);
            var J38 = 1.0 / 8.0 * (-1.0 - xi + eta + eta * xi);

            var buf = targetElement.MatrixPool.Allocate(3, 3);

            // TODO: MAT - set values directly
            buf.SetRow(0, new double[] { (J11 * x1 + J12 * x2 + J13 * x3 + J14 * x4 + J15 * x5 + J16 * x6 + J17 * x7 + J18 * x8),
                           (J11 * y1 + J12 * y2 + J13 * y3 + J14 * y4 + J15 * y5 + J16 * y6 + J17 * y7 + J18 * y8),
                           (J11 * z1 + J12 * z2 + J13 * z3 + J14 * z4 + J15 * z5 + J16 * z6 + J17 * z7 + J18 * z8) });
            buf.SetRow(1, new double[] { (J21 * x1 + J22 * x2 + J23 * x3 + J24 * x4 + J25 * x5 + J26 * x6 + J27 * x7 + J28 * x8),
                           (J21 * y1 + J22 * y2 + J23 * y3 + J24 * y4 + J25 * y5 + J26 * y6 + J27 * y7 + J28 * y8),
                           (J21 * z1 + J22 * z2 + J23 * z3 + J24 * z4 + J25 * z5 + J26 * z6 + J27 * z7 + J28 * z8) });
            buf.SetRow(2, new double[] { (J31 * x1 + J32 * x2 + J33 * x3 + J34 * x4 + J35 * x5 + J36 * x6 + J37 * x7 + J38 * x8),
                           (J31 * y1 + J32 * y2 + J33 * y3 + J34 * y4 + J35 * y5 + J36 * y6 + J37 * y7 + J38 * y8),
                           (J31 * z1 + J32 * z2 + J33 * z3 + J34 * z4 + J35 * z5 + J36 * z6 + J37 * z7 + J38 * z8) });
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
            var hex = targetElement as HexahedralElement;
            var n = hex.Nodes.Length;


            var tr = targetElement.GetTransformationManager();

            #region uniform 

            if (load is UniformLoad || load is PartialNonUniformLoad)
            {

                Func<double, double> magnitude;
                Vector localDir;

                double xi0, xi1;
                double eta0, eta1;
                double lambda0, lambda1;

                int degree;//polynomial degree of magnitude function

                #region inits
                if (load is UniformLoad)
                {
                    var uld = (load as UniformLoad);

                    magnitude = (xi => uld.Magnitude);
                    localDir = uld.Direction;

                    if (uld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    localDir = localDir.GetUnit();

                    xi0 = -1;
                    xi1 = 1;

                    eta0 = -1;
                    eta1 = 1;

                    lambda0 = -1;
                    lambda1 = 1;

                    degree = 0;
                }
                else if (load is PartialNonUniformLoad)
                {
                    var uld = (load as PartialNonUniformLoad);

                    magnitude = (xi => uld.GetMagnitudeAt(targetElement, new IsoPoint(xi)));
                    localDir = uld.Direction;

                    if (uld.CoordinationSystem == CoordinationSystem.Global)
                        localDir = tr.TransformGlobalToLocal(localDir);

                    localDir = localDir.GetUnit();

                    xi0 = uld.StartLocation.Xi;
                    xi1 = uld.EndLocation.Xi;

                    eta0 = uld.StartLocation.Eta;
                    eta1 = uld.EndLocation.Eta;

                    lambda0 = uld.StartLocation.Lambda;
                    lambda1 = uld.EndLocation.Lambda;

                    degree = uld.SeverityFunction.Degree[0];// Coefficients.Length;
                }
                else
                    throw new NotImplementedException();

                localDir = localDir.GetUnit();
                #endregion

                {

                    var nOrd = GetNMaxOrder(targetElement);

                    //var gpt = (nOrd + degree) / 2 + 1;//gauss point count

                    var intg = new GaussianIntegrator();

                    intg.A1 = lambda0;
                    intg.A2 = lambda1;

                    intg.F1 = gama => eta0;
                    intg.F2 = gama => eta1;

                    intg.G1 = (eta, gama) => xi0;
                    intg.G2 = (eta, gama) => xi1;

                    intg.XiPointCount = (nOrd[0] + degree) / 2 + 1;
                    intg.EtaPointCount = (nOrd[1] + degree) / 2 + 1;
                    intg.GammaPointCount = (nOrd[2] + degree) / 2 + 1;

                    intg.MatrixPool = hex.MatrixPool;

                    intg.H = new FunctionMatrixFunction((xi, eta, gama) =>
                    {
                        var shp = GetNMatrixAt(targetElement, xi, eta, gama);

                        var shp2 = hex.MatrixPool.Allocate(3, shp.ColumnCount);

                        shp2.SetSubMatrix(0, 0, shp);
                        shp2.SetSubMatrix(1, 0, shp);
                        shp2.SetSubMatrix(2, 0, shp);


                        var j = GetJMatrixAt(targetElement, xi, 0, 0);
                        shp.Scale(j.Determinant());

                        var q__ = magnitude(xi);

                        var q_ = localDir * q__;

                        shp2.ScaleRow(0, q_.X);
                        shp2.ScaleRow(1, q_.Y);
                        shp2.ScaleRow(2, q_.Z);

                        return shp2;
                    });


                    var res = intg.Integrate();

                    var localForces = new Force[2];

                    //res is 3x8 matrix, each columns has x,y and z components for force on each node
                    for (var i = 0; i < 8; i++)
                    {
                        var fx = res[0, i];
                        var fy = res[1, i];
                        var fz = res[2, i];

                        localForces[i] = new Force(fx, fy, fz, 0, 0, 0);
                    }

                    return localForces;
                }
            }

            #endregion

            #region ConcentratedLoad

            if (load is ConcentratedLoad)
            {
                var cl = load as ConcentratedLoad;

                var localforce = cl.Force;

                if (cl.CoordinationSystem == CoordinationSystem.Global)
                    localforce = tr.TransformGlobalToLocal(localforce);

                var buf = new Force[n];

                var ns = GetNMatrixAt(targetElement, cl.ForceIsoLocation.Xi, cl.ForceIsoLocation.Eta, cl.ForceIsoLocation.Lambda);

                for (var i = 0; i < n; i++)
                    buf[i] = localforce * ns[0, i];

                return buf;
            }

            #endregion


            throw new NotImplementedException();
        }


        public IEnumerable<Tuple<DoF, double>> GetLocalInternalForceAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public GeneralStressTensor GetLocalInternalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            var hex = targetElement as HexahedralElement;
            var n = localDisplacements.Length;

            var d = this.GetDMatrixAt(hex, isoCoords);
            var b = this.GetBMatrixAt(hex, isoCoords);
            var u = targetElement.MatrixPool.Allocate(24, 1);

            for (var i = 0; i < n; i++)
            {
                u[3 * i + 0, 0] = localDisplacements[i].DX;
                u[3 * i + 1, 0] = localDisplacements[i].DY;
                u[3 * i + 2, 0] = localDisplacements[i].DZ;
            }

            var res = d * b * u;//result is ?x?

            var c = CauchyStressTensor.FromMatrix(res);

            return new GeneralStressTensor(c);
        }

        public GeneralStressTensor GetLocalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetMuMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords)
        {
            //return type is 8x1
            var buf = targetElement.MatrixPool.Allocate(1, 8);//n1 n2 n3 n4 n5 n6 n7 n8
            
            throw new NotImplementedException();//fill the buf

            return buf;
        }

        public int[] GetNMaxOrder(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix GetRhoMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }
    }
}
