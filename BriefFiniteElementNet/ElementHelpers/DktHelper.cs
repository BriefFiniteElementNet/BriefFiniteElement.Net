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
    /// Represents a calculation helper for DKT element (Discrete Kirchhoff Triangle)
    /// </summary>
    public class DktHelper:IElementHelper
    {
        public Element TargetElement { get; set; }

        /// <inheritdoc/>
        public Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var tri = targetElement as TriangleElement;

            if (tri == null)
                throw new Exception();

            var xi = isoCoords[0];
            var eta = isoCoords[1];

            #region inits

            var mgr = targetElement.GetTransformationManager();//TransformManagerL2G.MakeFromTransformationMatrix(transformMatrix);

            var p1l = mgr.TransformGlobalToLocal(tri.Nodes[0].Location);
            var p2l = mgr.TransformGlobalToLocal(tri.Nodes[1].Location);
            var p3l = mgr.TransformGlobalToLocal(tri.Nodes[2].Location);

            var p23 = p2l - p3l;
            var p31 = p3l - p1l;
            var p12 = p1l - p2l;

            var x23 = p23.X;
            var x31 = p31.X;
            var x12 = p12.X;

            var y23 = p23.Y;
            var y31 = p31.Y;
            var y12 = p12.Y;

            var a = 0.5*Math.Abs(x31*y12 - x12*y31);

            var l23_2 = y23*y23 + x23*x23;
            var l31_2 = y31*y31 + x31*x31;
            var l12_2 = y12*y12 + x12*x12;

            var P4 = -6*x23/l23_2;
            var P5 = -6*x31/l31_2;
            var P6 = -6*x12/l12_2;

            var q4 = 3*x23*y23/l23_2;
            var q5 = 3*x31*y31/l31_2;
            var q6 = 3*x12*y12/l12_2;

            var r4 = 3*y23*y23/l23_2;
            var r5 = 3*y31*y31/l31_2;
            var r6 = 3*y12*y12/l12_2;

            var t4 = -6*y23/l23_2;
            var t5 = -6*y31/l31_2;
            var t6 = -6*y12/l12_2;

            #endregion

            #region h{x,y}{kesi,no}

            var hx_xi = new double[]//eq. 4.27 ref [1], also noted in several other references
            {
                P6*(1 - 2*xi) + (P5 - P6)*eta,
                q6*(1 - 2*xi) - (q5 + q6)*eta,
                -4 + 6*(xi + eta) + r6*(1 - 2*xi) - eta*(r5 + r6),
                -P6*(1 - 2*xi) + eta*(P4 + P6),
                q6*(1 - 2*xi) - eta*(q6 - q4),
                -2 + 6*xi + r6*(1 - 2*xi) + eta*(r4 - r6),
                -eta*(P5 + P4),
                eta*(q4 - q5),
                -eta*(r5 - r4)
            };

            var hy_xi = new double[] //eq. 4.28 ref [1], also noted in several other references
            {
                t6*(1 - 2*xi) + eta*(t5 - t6),
                1 + r6*(1 - 2*xi) - eta*(r5 + r6),
                -q6*(1 - 2*xi) + eta*(q5 + q6),
                -t6*(1 - 2*xi) + eta*(t4 + t6),
                -1 + r6*(1 - 2*xi) + eta*(r4 - r6),
                -q6*(1 - 2*xi) - eta*(q4 - q6),
                -eta*(t4 + t5),
                eta*(r4 - r5),
                -eta*(q4 - q5)
            };


            var hx_eta = new double[] //eq. 4.29 ref [1], also noted in several other references
            {
                -P5*(1 - 2*eta) - xi*(P6 - P5),
                q5*(1 - 2*eta) - xi*(q5 + q6),
                -4 + 6*(xi + eta) + r5*(1 - 2*eta) - xi*(r5 + r6),
                xi*(P4 + P6),
                xi*(q4 - q6),
                -xi*(r6 - r4),
                P5*(1 - 2*eta) - xi*(P4 + P5),
                q5*(1 - 2*eta) + xi*(q4 - q5),
                -2 + 6*eta + r5*(1 - 2*eta) + xi*(r4 - r5)
            };

            var hy_eta = new double[] //eq. 4.30 ref [1], also noted in several other references
            {
                -t5*(1 - 2*eta) - xi*(t6 - t5),
                1 + r5*(1 - 2*eta) - xi*(r5 + r6),
                -q5*(1 - 2*eta) + xi*(q5 + q6),
                xi*(t4 + t6),
                xi*(r4 - r6),
                -xi*(q4 - q6),
                t5*(1 - 2*eta) - xi*(t4 + t5),
                -1 + r5*(1 - 2*eta) + xi*(r4 - r5),
                -q5*(1 - 2*eta) - xi*(q4 - q5)
            };

            #endregion

            var buf = new Matrix(3, 9);

            for (var i = 0; i < 9; i++)
            {
                buf[0, i] = y31 * hx_xi[i] + y12 * hx_eta[i];
                buf[1, i] = -x31 * hy_xi[i] - x12 * hy_eta[i];
                buf[2, i] = -x31*hx_xi[i] - x12*hx_eta[i] + y31*hy_xi[i] + y12*hy_eta[i];
            }//eq. 4.26 page 46 ref [1]

            buf.MultiplyByConstant(1 / (2 * a));

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

            var mat = tri.Material.GetMaterialPropertiesAt(isoCoords);
            var t = tri.Section.GetThicknessAt(isoCoords);

            var d = new Matrix(3, 3);

            {
                var cf = t*t*t/12;

                d[0, 0] = mat.Ex / (1 - mat.NuXy * mat.NuYx);
                d[1, 1] = mat.Ey / (1 - mat.NuXy * mat.NuYx);
                d[0, 1] = d[1, 0] =
                    mat.Ex*mat.NuYx/(1 - mat.NuXy*mat.NuYx);

                d[2, 2] = mat.Ex/(2*(1 + mat.NuXy));

                //p55 http://www.code-aster.org/doc/v11/en/man_r/r3/r3.07.03.pdf

                d.MultiplyByConstant(cf);
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
            var tmgr = targetElement.GetTransformationManager();

            var tri = targetElement as TriangleElement;

            if (tri == null)
                throw new Exception();

            var xi = isoCoords[0];
            var eta = isoCoords[1];

            var mgr = tmgr;

            var p1l = mgr.TransformGlobalToLocal(tri.Nodes[0].Location);
            var p2l = mgr.TransformGlobalToLocal(tri.Nodes[1].Location);
            var p3l = mgr.TransformGlobalToLocal(tri.Nodes[2].Location);

            var p23 = p2l - p3l;
            var p31 = p3l - p1l;
            var p12 = p1l - p2l;


            var x23 = p23.X;
            var x31 = p31.X;
            var x12 = p12.X;

            var y23 = p23.Y;
            var y31 = p31.Y;
            var y12 = p12.Y;

            var buf = new Matrix(2, 2);

            buf[0, 0] = x31;
            buf[1, 1] = y12;

            buf[0, 1] = x12;
            buf[1, 0] = y31;

            return buf;
        }

        /// <inheritdoc/>
        public Matrix CalcLocalStiffnessMatrix(Element targetElement)
        {
            return ElementHelperExtensions.CalcLocalKMatrix_Triangle(this, targetElement);
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
                new ElementPermuteHelper.ElementLocalDof(0, DoF.Dz),
                new ElementPermuteHelper.ElementLocalDof(0, DoF.Rx),
                new ElementPermuteHelper.ElementLocalDof(0, DoF.Ry),

                new ElementPermuteHelper.ElementLocalDof(1, DoF.Dz),
                new ElementPermuteHelper.ElementLocalDof(1, DoF.Rx),
                new ElementPermuteHelper.ElementLocalDof(1, DoF.Ry),

                new ElementPermuteHelper.ElementLocalDof(2, DoF.Dz),
                new ElementPermuteHelper.ElementLocalDof(2, DoF.Rx),
                new ElementPermuteHelper.ElementLocalDof(2, DoF.Ry),
            };

            return buf;
        }

        /// <inheritdoc/>
        public IEnumerable<Tuple<DoF, double>> GetLocalInternalForceAt(Element targetElement,
            Displacement[] globalDisplacements, params double[] isoCoords)
        {
            //step 1 : get transformation matrix
            //step 2 : convert globals points to locals
            //step 3 : convert global displacements to locals
            //step 4 : calculate B matrix and D matrix
            //step 5 : M=D*B*U
            //Note : Steps changed...

            var tr = targetElement.GetTransformationManager();

            var locals = tr.TransformGlobalToLocal(globalDisplacements);

            var b = GetBMatrixAt(targetElement, isoCoords);

            var d = GetDMatrixAt(targetElement, isoCoords);

            var u1l = locals[0];
            var u2l = locals[1];
            var u3l = locals[2];

            var uDkt =
                   new Matrix(new[]
                   {u1l.DZ, u1l.RX, u1l.RY, /**/ u2l.DZ, u2l.RX, u2l.RY, /**/ u3l.DZ, u3l.RX, u3l.RY});


            var mDkt = d * b * uDkt; //eq. 32, batoz article

            //var buf = new PlateBendingStressTensor();

            //buf.Mx = mDkt[0, 0];
            //buf.My = mDkt[1, 0];
            //buf.Mxy = mDkt[2, 0];

            //return buf;

            var bTensor = new BendingStressTensor();

            //var buf = new List<Tuple<DoF, double>>();

            bTensor.M11 = mDkt[0, 0];
            bTensor.M22 = mDkt[1, 0];
            bTensor.M21 = bTensor.M12 = mDkt[2, 0];

            var buf = new FlatShellStressTensor(bTensor);

            throw new NotImplementedException();
        }

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
            //step 1 : get transformation matrix
            //step 2 : convert globals points to locals
            //step 3 : convert global displacements to locals
            //step 4 : calculate B matrix and D matrix
            //step 5 : M=D*B*U
            //Note : Steps changed...

            var tr = targetElement.GetTransformationManager();

            var locals = localDisplacements;// tr.TransformGlobalToLocal(globalDisplacements);

            var b = GetBMatrixAt(targetElement, isoCoords);

            var d = GetDMatrixAt(targetElement, isoCoords);

            var u1l = locals[0];
            var u2l = locals[1];
            var u3l = locals[2];

            var uDkt =
                   new Matrix(new[]
                   {u1l.DZ, u1l.RX, u1l.RY, /**/ u2l.DZ, u2l.RX, u2l.RY, /**/ u3l.DZ, u3l.RX, u3l.RY});


            var mDkt = d * b * uDkt; //eq. 32, batoz article

            var bTensor = new BendingStressTensor();

            bTensor.M11 = mDkt[0, 0];
            bTensor.M22 = mDkt[1, 0];
            bTensor.M21 = bTensor.M12 = mDkt[2, 0];

            var buf = new GeneralStressTensor(bTensor);
            return buf;
        }

        public GeneralStressTensor GetLoadStressAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }
    }
}
