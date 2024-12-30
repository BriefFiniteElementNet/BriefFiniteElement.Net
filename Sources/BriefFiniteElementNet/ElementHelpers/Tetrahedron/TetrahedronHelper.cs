using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Elements;
using CSparse.Storage;

namespace BriefFiniteElementNet.ElementHelpers
{
    public class TetrahedronHelper : IElementHelper
    {
        public Element TargetElement { get; set; }

        public Matrix GetBMatrixAt(Element targetElement, params double[] isoCoords)
        {
            //based on this: 
            //http://what-when-how.com/the-finite-element-method/fem-for-3d-solids-finite-element-method-part-1/


            var tetra = targetElement as TetrahedronElement;

            var X = (Matrix)targetElement.AllocateFromPool(4, 4);

            {
                var ps = tetra.Nodes.Select(i => i.Location).ToArray();

                {//9.11
                    X.SetRow(0, new double[] { 1, 1, 1, 1 });
                    X.SetRow(1, new double[] { ps[0].X, ps[1].X, ps[2].X, ps[3].X });
                    X.SetRow(2, new double[] { ps[0].Y, ps[1].Y, ps[2].Y, ps[3].Y });
                    X.SetRow(3, new double[] { ps[0].Z, ps[1].Z, ps[2].Z, ps[3].Z });

                }
            }

            var v = X.Determinant() / 6.0  ;//9.14

            var buf = targetElement.AllocateFromPool(6, 12);

            {
                var Q = X.Inverse();//* J.Determinant();
                Q.Scale(6 * v);//X^-1=1/6v*A >> A = X^-1*6v

                double a1 = Q[0, 0]; double b1 = Q[0, 1]; double c1 = Q[0, 2]; double d1 = Q[0, 3];
                double a2 = Q[1, 0]; double b2 = Q[1, 1]; double c2 = Q[1, 2]; double d2 = Q[1, 3];
                double a3 = Q[2, 0]; double b3 = Q[2, 1]; double c3 = Q[2, 2]; double d3 = Q[2, 3];
                double a4 = Q[3, 0]; double b4 = Q[3, 1]; double c4 = Q[3, 2]; double d4 = Q[3, 3];

                buf.SetRow(0, new[] { b1, 00, 00, b2, 00, 00, b3, 00, 00, b4, 00, 00 });
                buf.SetRow(1, new[] { 00, c1, 00, 00, c2, 00, 00, c3, 00, 00, c4, 00 });
                buf.SetRow(2, new[] { 00, 00, d1, 00, 00, d2, 00, 00, d3, 00, 00, d4 });

                buf.SetRow(3, new[] { c1, b1, 00, c2, b2, 00, c3, b3, 00, c4, b4, 00 });
                buf.SetRow(4, new[] { 00, d1, c1, 00, d2, c2, 00, d3, c3, 00, d4, c4 });
                buf.SetRow(5, new[] { d1, 00, b1, d2, 00, b2, d3, 00, b3, d4, 00, b4 });

                buf.Scale(1 / (6.0 * v));
            }

            return buf.AsMatrix();
        }

        /// <inheritdoc/>
        public Matrix GetBMatrixAt_1(Element targetElement, params double[] isoCoords)
        {
            //port from D3_TETRAH.m from fem_toolbox

            var tetra = targetElement as TetrahedronElement;

            var J = (Matrix)targetElement.AllocateFromPool(4, 4);

            {
                var ps = tetra.Nodes.Select(i => i.Location-tetra.Nodes[0].Location).ToArray();

                {//p6
                    J.SetRow(0, new double[] { 1, 1, 1, 1 });
                    J.SetRow(1, new double[] { ps[0].X, ps[1].X, ps[2].X, ps[3].X });
                    J.SetRow(2, new double[] { ps[0].Y, ps[1].Y, ps[2].Y, ps[3].Y });
                    J.SetRow(3, new double[] { ps[0].Z, ps[1].Z, ps[2].Z, ps[3].Z });

                    //liv.TransposeInPlace();
                }
            }

            //J.TransposeInPlace();

            //J = GetJMatrixAt(targetElement, isoCoords).AsMatrix();

            var v = 1 / 6.0 * J.Determinant();

            var buf = targetElement.AllocateFromPool(6, 12);

            {
                var Q = J.Inverse();//* J.Determinant();
                //Q.Scale(J.Determinant());

                double a1 = Q[0, 1]; double b1 = Q[0, 2]; double c1 = Q[0, 3]; //double d1 = Q[0, 3];
                double a2 = Q[1, 1]; double b2 = Q[1, 2]; double c2 = Q[1, 3]; //double d2 = Q[1, 3];
                double a3 = Q[2, 1]; double b3 = Q[2, 2]; double c3 = Q[2, 3]; //double d3 = Q[2, 3];
                double a4 = Q[3, 1]; double b4 = Q[3, 2]; double c4 = Q[3, 3]; //double d4 = Q[3, 3];

                buf.SetRow(0, new[] { a1, 00, 00, a2, 00, 00, a3, 00, 00, a4, 00, 00 });
                buf.SetRow(1, new[] { 00, b1, 00, 00, b2, 00, 00, b3, 00, 00, b4, 00 });
                buf.SetRow(2, new[] { 00, 00, c1, 00, 00, c2, 00, 00, c3, 00, 00, c4 });
                buf.SetRow(3, new[] { 00, c1, b1, 00, c2, b2, 00, c3, b3, 00, c4, b4 });
                buf.SetRow(4, new[] { c1, 00, a1, c2, 00, a2, c3, 00, a3, c4, 00, a4 });
                buf.SetRow(5, new[] { b1, a1, 00, b2, a2, 00, b3, a3, 00, b4, a4, 00 });

                buf.Scale(1 / (6.0 * v));
            }

            return buf.AsMatrix();
        }

        public Matrix GetBMatrixAt_old(Element targetElement, params double[] isoCoords)
        {
            //http://what-when-how.com/the-finite-element-method/fem-for-3d-solids-finite-element-method-part-1/
            //https://academic.csuohio.edu/duffy_s/CVE_512_12.pdf


            var tetra = targetElement as TetrahedronElement;

            var liv = targetElement.AllocateFromPool(4, 4);

            {
                var ps = tetra.Nodes.Select(i => i.Location).ToArray();

                {//p6
                    liv.SetColumn(0, new double[] { 1, 1, 1, 1 });
                    liv.SetColumn(1, new double[] { ps[0].X, ps[1].X, ps[2].X, ps[3].X });
                    liv.SetColumn(2, new double[] { ps[0].Y, ps[1].Y, ps[2].Y, ps[3].Y });
                    liv.SetColumn(3, new double[] { ps[0].Z, ps[1].Z, ps[2].Z, ps[3].Z });

                    //liv.TransposeInPlace();
                }
            }
            
            var ls = liv.Inverse();// tetra.IsoCoordsToLocalCoords(isoCoords);

            var v = ls.Determinant() * 1 / 6.0;

            //ls.Scale(1/(6.0 * v));

            var buf = targetElement.AllocateFromPool(6, 12);

            {//eq. 9.18
                double a1 = ls[0, 0]; double b1 = ls[1, 0]; double c1 = ls[2, 0]; double d1 = ls[3, 0];
                double a2 = ls[0, 1]; double b2 = ls[1, 1]; double c2 = ls[2, 1]; double d2 = ls[3, 1];
                double a3 = ls[0, 2]; double b3 = ls[1, 2]; double c3 = ls[2, 2]; double d3 = ls[3, 2];
                double a4 = ls[0, 3]; double b4 = ls[1, 3]; double c4 = ls[2, 3]; double d4 = ls[3, 3];

                buf.SetRow(0, new[] { b1, 0, 0, b2, 0, 0, b3, 0, 0, b4, 0, 0 });
                buf.SetRow(1, new[] { 0, c1, 0, 0, c2, 0, 0, c3, 0, 0, c4, 0, });
                buf.SetRow(2, new[] { 0, 0, d1, 0, 0, d2, 0, 0, d3, 0, 0, d4, });
                buf.SetRow(3, new[] { c1, b1, 0, c2, b2, 0, c3, b3, 0, c4, b4, 0 });
                buf.SetRow(4, new[] { 0, d1, c1, 0, d2, c2, 0, d3, c3, 0, d4, c4 });
                buf.SetRow(5, new[] { d1, 0, b1, d2, 0, b2, d3, 0, b3, d4, 0, b4 });

                buf.Scale(1 / (6.0 * v));
            }

            return buf.AsMatrix();
        }

        /// <inheritdoc/>
        public Matrix GetDMatrixAt(Element targetElement, params double[] isoCoords)
        {
            //new: based on this document https://www.sciencedirect.com/topics/engineering/tetrahedron-element
            //old: based on this document https://www.efunda.com/formulae/solid_mechanics/mat_mechanics/hooke_plane_strain.cfm
            var tetra = targetElement as TetrahedronElement;
            //Gets the constitutive matrix. Only for isotropic materials!!!

            if (tetra.Material is Materials.UniformAnisotropicMaterial)
                throw new NotImplementedException("UniformAnisotropicMaterial for Tetrahedron not implemented yet");

            var buf = targetElement.AllocateFromPool(6, 6);

            var props = tetra.Material.GetMaterialPropertiesAt(isoCoords);

            //material considered to be isotropic
            var miu = props.NuXy;
            var e = props.Ex;

            var coeff = e / ((1 + miu) * (1 - 2 * miu));

            buf.SetRow(0, 1-miu, miu, miu, 0, 0, 0);
            buf.SetRow(1, miu, 1-miu, miu, 0, 0, 0);
            buf.SetRow(2, miu, miu, 1-miu, 0, 0, 0);

            buf.SetRow(3, 0, 0, 0, (1 - 2 * miu)/2, 0, 0);
            buf.SetRow(4, 0, 0, 0, 0, (1 - 2 * miu)/2, 0);
            buf.SetRow(5, 0, 0, 0, 0, 0, (1 - 2 * miu)/2);

            buf.Scale(coeff);
            return buf.AsMatrix();
        }

        /// <inheritdoc/>
        public Matrix GetDMatrixAt_old2(Element targetElement, params double[] isoCoords)
        {
            //not works right
            //based on this document http://web.mit.edu/16.20/homepage/3_Constitutive/Constitutive_files/module_3_with_solutions.pdf
            var tetra = targetElement as TetrahedronElement;
            //Gets the constitutive matrix. Only for isotropic materials!!!

            if (tetra.Material is Materials.UniformAnisotropicMaterial)
                throw new NotImplementedException("UniformAnisotropicMaterial for Tetrahedron not implemented yet");

            var buf = targetElement.AllocateFromPool(6, 6);

            var props = tetra.Material.GetMaterialPropertiesAt(isoCoords);

            //material considered to be isotropic
            var miu = props.NuXy;
            var e = props.Ex;

            var lambda = e / ((1.0 + miu) * (1.0 - 2.0 * miu));//eq. 3.52
            var u = e / (2 + 2 * miu);//eq. 3.52

            //eq. 3.51
            buf.SetRow(0, lambda + 2 * u, lambda, lambda, 0, 0, 0);
            buf.SetRow(1, lambda, lambda + 2 * u, lambda, 0, 0, 0);
            buf.SetRow(2, lambda, lambda, lambda + 2 * u, 0, 0, 0);

            buf.SetRow(3, 0, 0, 0, u, 0, 0);
            buf.SetRow(4, 0, 0, 0, 0, u, 0);
            buf.SetRow(5, 0, 0, 0, 0, 0, u);

            return buf.AsMatrix();
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

            var tetra = targetElement as TetrahedronElement;

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
            var tet = targetElement as TetrahedronElement;

            if (tet == null)
                throw new Exception();

            var n0 = tet.Nodes[0].Location;
            var n1 = tet.Nodes[1].Location;
            var n2 = tet.Nodes[2].Location;
            var n3 = tet.Nodes[3].Location;

            var buf = targetElement.AllocateFromPool(3, 3);

            // TODO: MAT - set values directly
            //https://www.iue.tuwien.ac.at/phd/hollauer/node29.html 5.23
            //"J is the so - called Jacobian matrix which only depends on the global coordinates(x, y, z)"

            
            buf.SetRow(0, new double[] { n1.X - n0.X, n2.X - n0.X, n3.X - n0.X });
            buf.SetRow(1, new double[] { n1.Y - n0.Y, n2.Y - n0.Y, n3.Y - n0.Y });
            buf.SetRow(2, new double[] { n1.Z - n0.Z, n2.Z - n0.Z, n3.Z - n0.Z });

            return buf.AsMatrix();
        }

        /// <inheritdoc/>
        public Matrix CalcLocalStiffnessMatrix(Element targetElement)
        {
            /**/
            var tetra = targetElement as TetrahedronElement;

            var X = (Matrix)targetElement.AllocateFromPool(4, 4);

            {
                var ps = tetra.Nodes.Select(i => i.Location).ToArray();

                {//9.11
                    X.SetRow(0, new double[] { 1, 1, 1, 1 });
                    X.SetRow(1, new double[] { ps[0].X, ps[1].X, ps[2].X, ps[3].X });
                    X.SetRow(2, new double[] { ps[0].Y, ps[1].Y, ps[2].Y, ps[3].Y });
                    X.SetRow(3, new double[] { ps[0].Z, ps[1].Z, ps[2].Z, ps[3].Z });

                }
            }
            /**/
            //edit
            var b = GetBMatrixAt(targetElement,0, 0, 0);
            var d = GetDMatrixAt(targetElement, 0, 0, 0);

            var k = (b.Transpose() * d) * b;

            var v = X.Determinant() / 6.0;


            if (v < 0)
                throw new Exception();


            k.Scale(v);

            return k;
            return ElementHelperExtensions.CalcLocalKMatrix_Tetrahedron(this, targetElement);
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
            return new[] {0, 0, 0};
        }

        public int[] GetDetJOrder(Element targetElement)
        {
            return new[] { 0, 0, 0 };
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

        public GeneralStressTensor GetLoadStressAt(Element targetElement, ElementalLoad load, double[] isoLocation)
        {
            throw new NotImplementedException();
        }

        public GeneralStressTensor GetLocalInternalStressAt(Element targetElement, Displacement[] localDisplacements, params double[] isoCoords)
        {
            var disps =
                localDisplacements.Select(i => i.Displacements).//extract only displacement part (not include rotation)
                SelectMany(i => new[] { i.X, i.Y, i.Z }).//convert to single double array
                ToArray();

            var b = GetBMatrixAt(targetElement);//b is constant over tetrahedron (4node 3d)
            var u = Matrix.OfVector(disps);
            var d = GetDMatrixAt(targetElement, isoCoords);

            var res = d * b * u;

            //res = (Matrix) res.Transpose();

            var cauchy = new CauchyStressTensor();

            cauchy.S11 = res[0, 0];
            cauchy.S22 = res[1, 0];
            cauchy.S33 = res[2, 0];

            cauchy.S23 = cauchy.S32 = res[3, 0];
            cauchy.S13 = cauchy.S31 = res[4, 0];
            cauchy.S12 = cauchy.S21 = res[5, 0];

            return new GeneralStressTensor(cauchy);
        }

        public ILoadHandler[] GetLoadHandlers()
        {
            throw new NotImplementedException();
        }

        public Matrix CalcLocalStiffnessMatrix(Element targetElement, Matrix matrix)
        {
            throw new NotImplementedException();
        }

        public int CalcLocalStiffnessMatrixSize(Element targetElement)
        {
            throw new NotImplementedException();
        }
    }
}
