using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Elements;
using CSparse.Storage;

namespace BriefFiniteElementNet.ElementHelpers
{
    public class TetrahedralHelper : IElementHelper
    {
        public Element TargetElement { get; set; }

        /// <inheritdoc/>
        public Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
        {
            //http://what-when-how.com/the-finite-element-method/fem-for-3d-solids-finite-element-method-part-1/

            var tetra = targetElement as Tetrahedral;

            var ls = targetElement.AllocateFromPool(4, 4);

            {
                var ps = tetra.Nodes.Select(i => i.Location).ToArray();

                {//eq. 9.11
                    ls.SetColumn(0, new double[] { 1, 1, 1, 1 });
                    ls.SetColumn(1, new double[] { ps[0].X, ps[1].X, ps[2].X, ps[3].X });
                    ls.SetColumn(2, new double[] { ps[0].Y, ps[1].Y, ps[2].Y, ps[3].Y });
                    ls.SetColumn(3, new double[] { ps[0].Z, ps[1].Z, ps[2].Z, ps[3].Z });
                }
            }

            var Q = ls.Inverse();// tetra.IsoCoordsToLocalCoords(isoCoords);


            double a1 = Q[0, 1]; double b1 = Q[0, 2]; double c1 = Q[0, 3];
            double a2 = Q[1, 1]; double b2 = Q[1, 2]; double c2 = Q[1, 3];
            double a3 = Q[2, 1]; double b3 = Q[2, 2]; double c3 = Q[2, 3];
            double a4 = Q[3, 1]; double b4 = Q[3, 2]; double c4 = Q[3, 3];

            var b = new Matrix(6, 12, new double[] {
                a1, 0, 0, a2, 0, 0, a3, 0, 0, a4, 0, 0,
                0, b1, 0, 0, b2, 0, 0, b3, 0, 0, b4, 0,
                0, 0, c1, 0, 0, c2, 0, 0, c3, 0, 0, c4,
                0, c1, b1, 0, c2, b2, 0, c3, b3, 0, c4, b4,
                c1, 0, a1, c2, 0, a2, c3, 0, a3, c4, 0, a4,
                b1, a1, 0, b2, a2, 0, b3, a3, 0, b4, a4, 0 });

            return b;
        }

        

        /// <inheritdoc/>
        public Matrix GetDMatrixAt(Element targetElement, params double[] isoCoords)
        {
            throw new NotImplementedException();
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
            //http://what-when-how.com/the-finite-element-method/fem-for-3d-solids-finite-element-method-part-1/

            var tetra = targetElement as Tetrahedral;

            var locs = targetElement.AllocateFromPool(4, 4);

            var ps = tetra.Nodes.Select(i => i.Location).ToArray();

            {//eq. 9.11
                locs.SetColumn(0, new double[] { 1, 1, 1, 1 });
                locs.SetColumn(1, new double[] { ps[0].X, ps[1].X, ps[2].X, ps[3].X });
                locs.SetColumn(2, new double[] { ps[0].Y, ps[1].Y, ps[2].Y, ps[3].Y });
                locs.SetColumn(3, new double[] { ps[0].Z, ps[1].Z, ps[2].Z, ps[3].Z });
            }


            var local = tetra.IsoCoordsToLocalCoords(isoCoords);

            var buf = locs.Solve(new double[] { 1, local[0], local[1], local[2] });//9.15

            return new Matrix(4, 1, buf);
        }

        /// <inheritdoc/>
        public Matrix GetJMatrixAt(Element targetElement, params double[] isoCoords)
        {
            var tet = targetElement as Tetrahedral;

            if (tet == null)
                throw new Exception();

            var n1 = tet.Nodes[0].Location;
            var n2 = tet.Nodes[1].Location;
            var n3 = tet.Nodes[2].Location;
            var n4 = tet.Nodes[3].Location;

            var buf = new Matrix(3, 3);

            // TODO: MAT - set values directly
            buf.SetRow(0, new double[] { n2.X - n1.X, n3.X - n1.X, n4.X - n1.X });
            buf.SetRow(1, new double[] { n2.Y - n1.Y, n3.Y - n1.Y, n4.Y - n1.Y });
            buf.SetRow(2, new double[] { n2.Z - n1.Z, n3.Z - n1.Z, n4.Z - n1.Z });

            return buf;
        }

        /// <inheritdoc/>
        public Matrix CalcLocalStiffnessMatrix(Element targetElement)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
        public int[] GetNMaxOrder(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public int[] GetBMaxOrder(Element targetElement)
        {
            throw new NotImplementedException();
        }

        public int[] GetDetJOrder(Element targetElement)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public GeneralStressTensor GetLoadStressAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public GeneralStressTensor GetLocalInternalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            throw new NotImplementedException();
        }
    }
}
