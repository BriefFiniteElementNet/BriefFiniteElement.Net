using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;

namespace BriefFiniteElementNet.Elements
{
    [Obsolete("not fully implemented yet")]
    public class TriangleElement : Element
    {
        public TriangleElement(): base(3)
        {
        }

        public BaseTriangleMaterial _material;

        public BaseTriangleSection _section;

        public FlatShellBehaviour _behavior;

        public MembraneFormulation _formulation;


        public BaseTriangleMaterial Material
        {
            get { return _material; }
            set { _material = value; }
        }

        public BaseTriangleSection Section
        {
            get { return _section; }
            set { _section = value; }
        }

        public FlatShellBehaviour Behavior
        {
            get { return _behavior; }
            set { _behavior = value; }
        }

        public MembraneFormulation Formulation
        {
            get { return _formulation; }
            set { _formulation = value; }
        }


        public override Matrix ComputeBMatrix(params double[] location)
        {
            throw new NotImplementedException();
        }

        public override Matrix ComputeDMatrixAt(params double[] location)
        {
            throw new NotImplementedException();
        }

        public override Matrix ComputeJMatrixAt(params double[] location)
        {
            throw new NotImplementedException();
        }

        public override Matrix ComputeNMatrixAt(params double[] location)
        {
            throw new NotImplementedException();
        }

        public override Force[] GetEquivalentNodalLoads(Load load)
        {
            throw new NotImplementedException();
        }

        public override Matrix GetGlobalDampingMatrix()
        {
            throw new NotImplementedException();
        }

        public override Matrix GetGlobalMassMatrix()
        {
            throw new NotImplementedException();
        }

        public override Matrix GetGlobalStifnessMatrix()
        {
            throw new NotImplementedException();
        }

        public Matrix GetTransformationMatrix()
        {
            var p1 = nodes[0].Location;
            var p2 = nodes[1].Location;
            var p3 = nodes[2].Location;

            var v1 = p1 - Point.Origins;
            var v2 = p2 - Point.Origins;
            var v3 = p3 - Point.Origins;

            var ii = (v1 + v2) / 2;
            var jj = (v2 + v3) / 2;
            var kk = (v3 + v1) / 2;

            var vx = (jj - kk).GetUnit();//eq. 5.3
            var vr = (ii - v3).GetUnit();//eq. 5.5
            var vz = Vector.Cross(vx, vr);//eq. 5.6
            var vy = Vector.Cross(vz, vx);//eq. 5.7

            var lamX = vx.GetUnit();//Lambda_x
            var lamY = vy.GetUnit();//Lambda_x
            var lamZ = vz.GetUnit();//Lambda_x

            var lambda = new Matrix(new[]
            {
                new[] {lamX.X, lamY.X, lamZ.X},
                new[] {lamX.Y, lamY.Y, lamZ.Y},
                new[] {lamX.Z, lamY.Z, lamZ.Z}
            });//eq. 5.13

            return lambda;
        }

        public Matrix GetLocalStifnessMatrix()
        {
            var helpers = new List<IElementHelper>();

            if ((this._behavior & FlatShellBehaviour.ThinPlate) != 0)
            {
                helpers.Add(new DktHelper());
            }
            
            var buf = new Matrix(18, 18);

            var transMatrix = GetTransformationMatrix();

            for (var i = 0; i < helpers.Count; i++)
            {
                var helper = helpers[i];

                var ki = helper.CalcLocalKMatrix(this, transMatrix);// ComputeK(helper, transMatrix);

                var dofs = helper.GetDofOrder(this);

                for (var ii = 0; ii < dofs.Length; ii++)
                {
                    var bi = dofs[ii].NodeIndex * 6 + (int)dofs[ii].Dof;

                    for (var jj = 0; jj < dofs.Length; jj++)
                    {
                        var bj = dofs[jj].NodeIndex * 6 + (int)dofs[jj].Dof;

                        buf[bi, bj] += ki[ii, jj];
                    }
                }


            }

            return buf;

            throw new NotImplementedException();
        }
    }
}
