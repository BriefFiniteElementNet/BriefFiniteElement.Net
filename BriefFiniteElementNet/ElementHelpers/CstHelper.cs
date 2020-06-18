using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;
using CSparse.Storage;
using BriefFiniteElementNet.Common;

namespace BriefFiniteElementNet.ElementHelpers
{
    /// <summary>
    /// Represents a calculation helper for CST element (Constant Stress/Strain Triangle)
    /// </summary>
    public class CstHelper : IElementHelper
    {
        public Element TargetElement { get; set; }

        /// <inheritdoc/>
        public Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var tri = targetElement as TriangleElement;

            var tmgr = targetElement.GetTransformationManager();


            if (tri == null)
                throw new Exception();

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

            var buf = new Matrix(3, 6);

            buf.FillRow(0, y2 - y3, 0, y3 - y1, 0, y1 - y2, 0);
            buf.FillRow(1, 0, x3 - x2, 0, x1 - x3, 0, x2 - x1);
            buf.FillRow(2, x3 - x2, y2 - y3, x1 - x3, y3 - y1, x2 - x1, y1 - y2);

            var a = 0.5*Math.Abs((x3 - x1)*(y1 - y2) - (x1 - x2)*(y3 - y1));

            buf.MultiplyByConstant(1/(2*a));

            return buf;
        }

        public Matrix GetB_iMatrixAt(Element targetElement, int i, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Matrix GetDMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var tri = targetElement as TriangleElement;

            if (tri == null)
                throw new Exception();

            var d = new Matrix(3, 3);

            var mat = tri.Material.GetMaterialPropertiesAt(isoCoords);


            if (tri.MembraneFormulation == MembraneFormulation.PlaneStress)
            {
                //http://help.solidworks.com/2013/english/SolidWorks/cworks/c_linear_elastic_orthotropic.htm
                //orthotropic material
                d[0, 0] = mat.Ex / (1 - mat.NuXy * mat.NuYx);
                d[1, 1] = mat.Ey / (1 - mat.NuXy * mat.NuYx);
                d[1, 0] = d[0, 1] = mat.NuXy * mat.Ey / (1 - mat.NuXy * mat.NuYx);

                d[2, 2] = mat.Ex*mat.Ey/(mat.Ex + mat.Ey + 2*mat.Ey*mat.NuXy);
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

        public Matrix GetRhoMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        public Matrix GetMuMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Matrix GetNMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public Matrix CalcLocalStiffnessMatrix(Element targetElement)
        {
            var intg = new BriefFiniteElementNet.Integration.GaussianIntegrator();

            intg.A2 = 1;
            intg.A1 = 0;

            intg.F2 = (gama => 1);
            intg.F1 = (gama => 0);

            intg.G2 = ((eta, gama) => 1 - eta);
            intg.G1 = ((eta, gama) => 0);

            intg.XiPointCount = intg.EtaPointCount = 3;
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

        public Matrix CalcLocalMassMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public Matrix CalcLocalDampMatrix(Element targetElement)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ElementPermuteHelper.ElementLocalDof[] GetDofOrder(Element targetElement)
        {
            var buf = new ElementPermuteHelper.ElementLocalDof[]
            {
                new ElementPermuteHelper.ElementLocalDof(0, DoF.Dx),
                new ElementPermuteHelper.ElementLocalDof(0, DoF.Dy),

                new ElementPermuteHelper.ElementLocalDof(1, DoF.Dx),
                new ElementPermuteHelper.ElementLocalDof(1, DoF.Dy),

                new ElementPermuteHelper.ElementLocalDof(2, DoF.Dx),
                new ElementPermuteHelper.ElementLocalDof(2, DoF.Dy),
            };

            return buf;
        }

        /// <inheritdoc/>
        public IEnumerable<Tuple<DoF, double>> GetLocalInternalForceAt(Element targetElement,
            Displacement[] globalDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool DoesOverrideKMatrixCalculation(Element targetElement, Matrix transformMatrix)
        {
            return false;
        }

        /// <inheritdoc/>
        public int Ng = 2;

        /// <inheritdoc/>
        public int[] GetNMaxOrder(Element targetElement)
        {
            return new int[] { Ng, Ng, 0 };
        }

        public int[] GetBMaxOrder(Element targetElement)
        {
            return new int[] { Ng, Ng, 0 };
        }

        public int[] GetDetJOrder(Element targetElement)
        {
            return new int[] { 0, 0, 0 };
        }

        public IEnumerable<Tuple<DoF, double>> GetLoadInternalForceAt(Element targetElement, ElementalLoad load,
            double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public Displacement GetLoadDisplacementAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Displacement GetLocalDisplacementAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }


        public Force[] GetLocalEquivalentNodalLoads(Element targetElement, ElementalLoad load)
        {
            throw new NotImplementedException();
        }

        public void AddStiffnessComponents(CoordinateStorage<double> global)
        {
            throw new NotImplementedException();
        }

        public GeneralStressTensor GetLocalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            //Note: membrane internal force is constant

            //step 1 : get transformation matrix
            //step 2 : convert globals points to locals
            //step 3 : convert global displacements to locals
            //step 4 : calculate B matrix and D matrix
            //step 5 : M=D*B*U
            //Note : Steps changed...

            //var trans = this.GetTransformationMatrix();

            //var lp = GetLocalPoints();

            //var g2l = new Func<Vector, Vector>(glob => (trans.Transpose() * glob.ToMatrix()).ToVector());
            //var l2g = new Func<Vector, Vector>(local => (trans*local.ToMatrix()).ToPoint());


            //var d1g = this.nodes[0].GetNodalDisplacement(combination);
            //var d2g = this.nodes[1].GetNodalDisplacement(combination);
            //var d3g = this.nodes[2].GetNodalDisplacement(combination);

            //step 3
            var d1l = localDisplacements[0];// new Displacement(g2l(d1g.Displacements), g2l(d1g.Rotations));
            var d2l = localDisplacements[1];//new Displacement(g2l(d2g.Displacements), g2l(d2g.Rotations));
            var d3l = localDisplacements[2];//new Displacement(g2l(d3g.Displacements), g2l(d3g.Rotations));

            var u =
                   new Matrix(new[]
                   {d1l.DX, d1l.DY, d2l.DX, d2l.DY, /**/d3l.DX, d3l.DY});

            var d = this.GetDMatrixAt(targetElement, isoCoords);
            
            var b = this.GetBMatrixAt(targetElement, isoCoords);


            var sCst = d * b * u;

            var buf = new MembraneStressTensor();

            buf.Sx = sCst[0, 0];
            buf.Sy = sCst[1, 0];
            buf.Txy = sCst[2, 0];

            return new GeneralStressTensor(buf);
        }

        public GeneralStressTensor GetLoadStressAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }
    }
}
