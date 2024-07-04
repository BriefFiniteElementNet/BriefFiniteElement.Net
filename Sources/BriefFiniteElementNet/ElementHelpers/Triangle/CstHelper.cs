using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Elements;
using BriefFiniteElementNet.Integration;
using CSparse.Storage;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Loads;

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

            var p1l = tmgr.TransformGlobalToLocal(p1g);
            var p2l = tmgr.TransformGlobalToLocal(p2g);
            var p3l = tmgr.TransformGlobalToLocal(p3g);

            var x1 = p1l.X;
            var x2 = p2l.X;
            var x3 = p3l.X;

            var y1 = p1l.Y;
            var y2 = p2l.Y;
            var y3 = p3l.Y;

            var buf = targetElement.MatrixPool.Allocate(3, 6);

            // TODO: MAT - set values directly
            buf.SetRow(0, new double[] { y2 - y3, 0, y3 - y1, 0, y1 - y2, 0 });
            buf.SetRow(1, new double[] { 0, x3 - x2, 0, x1 - x3, 0, x2 - x1 });
            buf.SetRow(2, new double[] { x3 - x2, y2 - y3, x1 - x3, y3 - y1, x2 - x1, y1 - y2 });

            var a = 0.5*Math.Abs((x3 - x1)*(y1 - y2) - (x1 - x2)*(y3 - y1));

            buf.Scale(1/(2*a));

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

            var d = targetElement.MatrixPool.Allocate(3, 3);

            


            if (tri.MembraneFormulation == MembraneFormulation.PlaneStress)
            {
                //http://help.solidworks.com/2013/english/SolidWorks/cworks/c_linear_elastic_orthotropic.htm
                //orthotropic material

                var mat = tri.Material.GetMaterialPropertiesAt(isoCoords);


                d[0, 0] = mat.Ex / (1 - mat.NuXy * mat.NuYx);
                d[1, 1] = mat.Ey / (1 - mat.NuXy * mat.NuYx);
                d[1, 0] = d[0, 1] = mat.NuXy * mat.Ey / (1 - mat.NuXy * mat.NuYx);

                d[2, 2] = mat.Ex*mat.Ey/(mat.Ex + mat.Ey + 2*mat.Ey*mat.NuXy);

                return d;
            }

            if (tri.MembraneFormulation == MembraneFormulation.PlaneStrain)
            {
                if (tri.Material is Materials.UniformIsotropicMaterial iso)
                {
                    //https://www.efunda.com/formulae/solid_mechanics/mat_mechanics/hooke_plane_strain.cfm

                    var mat = tri.Material.GetMaterialPropertiesAt(isoCoords);

                    var E = mat.Ex;
                    var nu = mat.NuXy;

                    d[0, 0] = d[1, 1] = 1 - nu;
                    d[1, 0] = d[0, 1] = nu;
                    d[2, 2] = 1 - 2 * nu;

                    d.Scale(E / ((1 + nu) * (1 - 2 * nu)));

                    return d;
                }

                if (tri.Material is Materials.UniformAnisotropicMaterial aniso)
                {
                    var mat = tri.Material.GetMaterialPropertiesAt(isoCoords);

                    var delta = 1 - mat.NuXy * mat.NuYx - mat.NuZy * mat.NuYz - mat.NuZx * mat.NuXz - 2 * mat.NuXy * mat.NuYz * mat.NuZx;

                    delta /= mat.Ex * mat.Ey * mat.Ez;

                    //http://www.efunda.com/formulae/solid_mechanics/mat_mechanics/hooke_orthotropic.cfm

                    d[0, 0] = (1 - mat.NuYz * mat.NuZy) / (mat.Ey * mat.Ez * delta);
                    d[0, 1] = (mat.NuYx + mat.NuZx * mat.NuYz) / (mat.Ey * mat.Ez * delta);
                    d[1, 0] = (mat.NuXy + mat.NuXz * mat.NuZy) / (mat.Ez * mat.Ex * delta);
                    d[1, 1] = (1 - mat.NuZx * mat.NuXz) / (mat.Ez * mat.Ex * delta);

                    return d;
                }
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

            var buf = targetElement.MatrixPool.Allocate(2, 2);

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
            var tri = targetElement as TriangleElement;

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

                ki.Scale(Math.Abs(j.Determinant()));
                //eq 3.27: thickness* bT * d * b;
                ki.Scale(tri.Section.GetThicknessAt(new double[] { xi, eta }));

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
        #region internal forces/stress
        /// <inheritdoc/>
        public IEnumerable<Tuple<DoF, double>> GetLocalInternalForceAt(Element targetElement,
            Displacement[] globalDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Gets the stresses for a single element
        /// </summary>
        /// <param name="targetElement"></param>
        /// <param name="loadCase"></param>
        /// <param name="isoCoords"></param>
        /// <returns></returns>
        public GeneralStressTensor GetLocalInternalForce(Element targetElement, LoadCase loadCase, params double[] isoCoords)
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

            // var locals = tr.TransformGlobalToLocal(globalDisplacements);

            var b = GetBMatrixAt(targetElement, isoCoords);
            var d = GetDMatrixAt(targetElement, isoCoords);

            var u1l = lds[0];
            var u2l = lds[1];
            var u3l = lds[2];

            var uDkt = targetElement.MatrixPool.Allocate(6, 1);

            // TODO: MAT - set values directly
            uDkt.SetColumn(1, new double[] { u1l.DX, u1l.DY, /**/ u2l.DX, u2l.DY, /**/ u3l.DX, u3l.DY });

            var mDkt = d * b * uDkt; //eq. 32, batoz article


            var bTensor = new CauchyStressTensor();

            bTensor.S11 = mDkt[0, 0];
            bTensor.S22 = mDkt[1, 0];
            bTensor.S12 = bTensor.S21 = mDkt[2, 0];

            var buf = new GeneralStressTensor(bTensor);
            return buf;
            throw new NotImplementedException();
        }
        #endregion
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

            if (load is UniformLoad)
            {
                var ul = load as UniformLoad;

                var u = ul.Direction;// new Vector();


                if (ul.CoordinationSystem == CoordinationSystem.Local)
                {
                    var trans = targetElement.GetTransformationManager();
                    u = trans.TransformLocalToGlobal(u); //local to global
                }

                var nodes = targetElement.Nodes;

                var area = CalcUtil.GetTriangleArea(nodes[0].Location, nodes[1].Location, nodes[2].Location);

                var f = u * (area / 3.0);

                f.Z = 0;//force component in z directions
                var frc = new Force(f, Vector.Zero);
                return new[] { frc, frc, frc };
            }
            throw new NotImplementedException();
        }

        public void AddStiffnessComponents(CoordinateStorage<double> global)
        {
            throw new NotImplementedException();
        }

        #region stress
      

        /// <summary>
        /// Gets the stress due to the membrane action. Needs to be combined with bending stress!!!
        /// </summary>
        /// <param name="targetElement">Element under consideration</param>
        /// <param name="combination">A load combination</param>
        /// <returns>The stress in the element due to the membrane action. Needs to be combined with the stress due to bending!</returns>
        public CauchyStressTensor GetMembraneInternalStress(Element targetElement, LoadCombination combination)
        {
            //code moved to GetLocalInternalStressAt()

            //Note: membrane internal force is constant

            //step 1 : get transformation matrix
            //step 2 : convert globals points to locals
            //step 3 : convert global displacements to locals
            //step 4 : calculate B matrix and D matrix
            //step 5 : M=D*B*U

            var lds = new Displacement[targetElement.Nodes.Length];
            var tr = targetElement.GetTransformationManager();

            for (var i = 0; i < targetElement.Nodes.Length; i++)
            {
                var globalD = targetElement.Nodes[i].GetNodalDisplacement(combination);
                var local = tr.TransformGlobalToLocal(globalD);
                lds[i] = local;
            }
            //step 3
            var d1l = lds[0];
            var d2l = lds[1];
            var d3l = lds[2];

            var uCst = targetElement.MatrixPool.Allocate(6, 1);

            // TODO: MAT - set values directly
            uCst.SetColumn(1, new double[] { d1l.DX, d1l.DY, d2l.DX, d2l.DY, /**/d3l.DX, d3l.DY });

            //cst -> constant over entire element -> provide random values for local
            var dbCst = this.GetDMatrixAt(targetElement, new double[] { 0, 0 });

            var bCst = this.GetBMatrixAt(targetElement, new double[] { 0, 0 });

            var sCst = dbCst * bCst * uCst;

            var buf = new CauchyStressTensor();

            buf.S11 = sCst[0, 0];
            buf.S22 = sCst[1, 0];
            buf.S12 = buf.S21 = sCst[2, 0];

            return buf;
        }
        #endregion
        public GeneralStressTensor GetLoadStressAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }
        #region strain
        /// <summary>
        /// Determine the strain in the element for a given load case
        /// </summary>
        /// <param name="targetElement">Element under consideration</param>
        /// <param name="loadCase">Loadcase for strain determination</param>
        /// <param name="isoCoords">The location - doesn't matter since it is constant in the CST element</param>
        /// <returns></returns>
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

            var uCst =
                   targetElement.MatrixPool.Allocate(new[]
                   {u1l.DX, u1l.DY, /**/ u2l.DX, u2l.DY, /**/ u3l.DX, u3l.DY});

            var ECst = b * uCst;

            var buf = new StrainTensor();

            buf.S11 = ECst[0, 0];
            buf.S22 = ECst[1, 0];
            buf.S12 = ECst[2, 0];

            return buf;
        }

        public GeneralStressTensor GetLocalInternalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            //Note: membrane internal force is constant

            //step 1 : get transformation matrix
            //step 2 : convert globals points to locals
            //step 3 : convert global displacements to locals
            //step 4 : calculate B matrix and D matrix
            //step 5 : M=D*B*U


            var lds = localDisplacements;
            //step 3
            var d1l = lds[0];
            var d2l = lds[1];
            var d3l = lds[2];

            var uCst =
                   targetElement.MatrixPool.Allocate(new[]
                   {d1l.DX, d1l.DY, d2l.DX, d2l.DY, /**/d3l.DX, d3l.DY});

            //cst -> constant over entire element -> provide random values for local
            var dbCst = this.GetDMatrixAt(targetElement, new double[] { 0, 0 });

            var bCst = this.GetBMatrixAt(targetElement, new double[] { 0, 0 });

            var sCst = dbCst * bCst * uCst;

            var buf = new CauchyStressTensor();

            buf.S11 = sCst[0, 0];
            buf.S22 = sCst[1, 0];
            buf.S12 = buf.S21 = sCst[2, 0];

            return new GeneralStressTensor(buf);
        }

        public ILoadHandler[] GetLoadHandlers()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
