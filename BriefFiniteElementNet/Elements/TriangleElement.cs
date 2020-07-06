using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace BriefFiniteElementNet.Elements
{
    [Serializable]
    [Obsolete("not fully implemented yet")]
    public class TriangleElement : Element
    {
        public TriangleElement(): base(3)
        {
        }

        /// <inheritdoc/>
        public override double[] IsoCoordsToLocalCoords(params double[] isoCoords)
        {
            throw new NotImplementedException();
        }

        private BaseMaterial _material;

        private Base2DSection _section;

        private PlateElementBehaviour _behavior;

        private MembraneFormulation _formulation;



        public BaseMaterial Material
        {
            get { return _material; }
            set { _material = value; }
        }

        public Base2DSection Section
        {
            get { return _section; }
            set { _section = value; }
        }

        public PlateElementBehaviour Behavior
        {
            get { return _behavior; }
            set { _behavior = value; }
        }

        public MembraneFormulation MembraneFormulation
        {
            get { return _formulation; }
            set { _formulation = value; }
        }



        

        public override Force[] GetGlobalEquivalentNodalLoads(ElementalLoad load)
        {
            throw new NotImplementedException();
        }

        public override Matrix GetGlobalDampingMatrix()
        {
            var local = GetLocalDampMatrix();
            var tr = GetTransformationManager();

            var buf = tr.TransformLocalToGlobal(local);

            return buf;
        }

        public override Matrix GetGlobalMassMatrix()
        {
            var local = GetLocalMassMatrix();
            var tr = GetTransformationManager();

            var buf = tr.TransformLocalToGlobal(local);

            return buf;
        }

        public override Matrix GetGlobalStifnessMatrix()
        {
            var local = GetLocalStifnessMatrix();
            var tr = GetTransformationManager();

            var buf = tr.TransformLocalToGlobal(local);

            return buf;
        }

        /// <inheritdoc /> 
        public override Matrix GetLambdaMatrix()
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

            var lamX = vx.GetUnit();//Lambda_X
            var lamY = vy.GetUnit();//Lambda_Y
            var lamZ = vz.GetUnit();//Lambda_Z

            var lambda = Matrix.FromJaggedArray(new[]
            {
                new[] {lamX.X, lamY.X, lamZ.X},
                new[] {lamX.Y, lamY.Y, lamZ.Y},
                new[] {lamX.Z, lamY.Z, lamZ.Z}
            });

            return lambda;
        }

        public override IElementHelper[] GetHelpers()
        {
            var helpers = new List<IElementHelper>();

            {
                if ((this._behavior & PlateElementBehaviour.Bending) != 0)
                {
                    helpers.Add(new DktHelper());
                }

                if ((this._behavior & PlateElementBehaviour.Membrane) != 0 )
                {
                    helpers.Add(new CstHelper());
                }

                if ((this._behavior & PlateElementBehaviour.DrillingDof) != 0)
                {
                    helpers.Add(new TriangleBasicDrillingDofHelper());
                }
            }


            return helpers.ToArray();
        }


        public Matrix GetLocalStifnessMatrix()
        {
            var helpers = GetHelpers();
            
            var buf = new Matrix(18, 18);

            for (var i = 0; i < helpers.Length; i++)
            {
                var helper = helpers[i];

                var ki = helper.CalcLocalStiffnessMatrix(this);// ComputeK(helper, transMatrix);

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
        }

        public Matrix GetLocalMassMatrix()
        {
            var helpers = new List<IElementHelper>();

            if ((this._behavior & PlateElementBehaviour.Bending) != 0)
            {
                helpers.Add(new DktHelper());
            }

            var buf = new Matrix(18, 18);

            for (var i = 0; i < helpers.Count; i++)
            {
                var helper = helpers[i];

                var ki = helper.CalcLocalMassMatrix(this);// ComputeK(helper, transMatrix);

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
        }

        public Matrix GetLocalDampMatrix()
        {
            var helpers = GetHelpers();

            var buf = new Matrix(18, 18);

            for (var i = 0; i < helpers.Length; i++)
            {
                var helper = helpers[i];

                var ki = helper.CalcLocalDampMatrix(this);// ComputeK(helper, transMatrix);

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
        }

        #region Deserialization Constructor

        protected TriangleElement(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _material = (BaseMaterial)info.GetValue("_material", typeof(BaseMaterial));
            _section = (Base2DSection)info.GetValue("_section", typeof(Base2DSection));
            _behavior = (PlateElementBehaviour)info.GetInt32("_behavior");
            _formulation = (MembraneFormulation)info.GetInt32("_behavior");
        }

        #endregion

        #region ISerialization Implementation

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_material", _material);
            info.AddValue("_section", _section);
            info.AddValue("_behavior", (int)_behavior);
            info.AddValue("_formulation", (int)_formulation);

        }

        #endregion


        public CauchyStressTensor GetLocalInternalForce(IsoPoint location,LoadCase loadCase)
        {

            //membrane creates constant cauchy stress throught the thickness
            //bending causes variable cauchy stress throught the thickness
            
            var helpers = GetHelpers();

            var buf = new CauchyStressTensor();

            var disps = this.nodes.Select(i => i.GetNodalDisplacement(loadCase)).ToArray();

            var tr = this.GetTransformationManager();

            var lDisp = tr.TransformGlobalToLocal(disps);

            for (var i = 0; i < helpers.Length; i++)
            {
                var helper = helpers[i];

                var frc = helper.GetLocalInternalForceAt(this, lDisp, location.Xi, location.Eta, location.Lambda);

                foreach (var component in frc)
                {
                    switch (component.Item1)
                    {
                        case DoF.Dx:
                            buf.S11 += component.Item2;
                            break;
                        case DoF.Dy:
                            buf.S22 += component.Item2;
                            break;
                        case DoF.Dz:
                            buf.S11 += component.Item2;
                            break;

                        case DoF.Rx:
                            break;
                        case DoF.Ry:
                            break;
                        case DoF.Rz:
                            break;

                        default:
                            break;
                    }
                }
            }

            return buf;
        }
    }
}
