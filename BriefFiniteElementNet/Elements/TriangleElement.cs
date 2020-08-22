﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using System.Runtime.Serialization;
using System.Security.Permissions;
using BriefFiniteElementNet.Common;
using BriefFiniteElementNet.Loads;

namespace BriefFiniteElementNet.Elements
{
    [Serializable]
    [Obsolete("not fully implemented yet")]
    public class TriangleElement : Element
    {
        public TriangleElement() : base(3)
        {
        }

        #region properties
        /// <inheritdoc/>

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
        #endregion

        #region coordinate transformations
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
        public override double[] IsoCoordsToLocalCoords(params double[] isoCoords)
        {
            var tr = GetTransformationManager();
            List<Node> Nodes = new List<Node>();
            for (int i = 0; i < this.Nodes.Count(); i++)
            {
                Vector p = tr.TransformGlobalToLocal(new Vector() { X = this.Nodes[i].Location.X - this.Nodes[0].Location.X, Y = this.Nodes[i].Location.Y - this.Nodes[0].Location.Y, Z = this.Nodes[i].Location.Z - this.Nodes[0].Location.Z });
                Nodes.Add(new Node() { Location = new Point() { X = p.X, Y = p.Y, Z = p.Z } });
            }
            double N1 = isoCoords[0];
            double N2 = isoCoords[1];
            double N3 = 1.0 - isoCoords[0] - isoCoords[1];
            double x = Nodes[0].Location.X * N1 + Nodes[1].Location.X * N2 + Nodes[2].Location.X * N3;
            double y = Nodes[0].Location.Y * N1 + Nodes[1].Location.Y * N2 + Nodes[2].Location.Y * N3;

            return new double[] { x, y };
        }
        public Point IsoCoordsToGlobalCoords(params double[] isoCoords)
        {
            var tr = GetTransformationManager();

            var localA = IsoCoordsToLocalCoords(isoCoords);
            var local = new Vector(localA[0], localA[1], 0);

            var global = tr.TransformLocalToGlobal(local);

            var globalP = this.Nodes[0].Location + global;

            return globalP;
        }
        #endregion

        #region loading
        public override Force[] GetGlobalEquivalentNodalLoads(ElementalLoad load)
        {
            if (load is UniformLoadForPlanarElements)
            {
                //lumped approach is used as used in several references
                var ul = load as UniformLoadForPlanarElements;

                var u = new Vector();

                u.X = ul.Ux;
                u.Y = ul.Uy;
                u.Z = ul.Uz;

                if (ul.CoordinationSystem == CoordinationSystem.Local)
                {
                    var trans = GetTransformationManager();
                    u = trans.TransformLocalToGlobal(u); //local to global
                }

                if (_behavior == PlateElementBehaviour.Membrane)
                    u.Z = 0;//remove one for plate bending

                if (_behavior == PlateElementBehaviour.Bending)
                    u.Y = u.X = 0;//remove those for membrane


                var area = CalcUtil.GetTriangleArea(nodes[0].Location, nodes[1].Location, nodes[2].Location);

                var f = u * (area / 3.0);
                var frc = new Force(f, Vector.Zero);
                return new[] { frc, frc, frc };
            }
            if (load is PressureLoadForPlanarElements)
            {
                //lumped approach is used as used in several references
                var pl = load as PressureLoadForPlanarElements;

                var u = new Vector();

                u.X = 0.0;
                u.Y = 0.0;
                u.Z = pl.Magnitude;

                var trans = GetTransformationManager();
                u = trans.TransformLocalToGlobal(u); //local to global

                if (_behavior == PlateElementBehaviour.Membrane)
                    u.Z = 0;//remove one for plate bending

                if (_behavior == PlateElementBehaviour.Bending)
                    u.Y = u.X = 0;//remove those for membrane

                var area = CalcUtil.GetTriangleArea(nodes[0].Location, nodes[1].Location, nodes[2].Location);

                var f = u * (area / 3.0);
                var frc = new Force(f, Vector.Zero);
                return new[] { frc, frc, frc };
            }

            throw new NotImplementedException();
        }
        #endregion

        #region element methods
        public override IElementHelper[] GetHelpers()
        {
            var helpers = new List<IElementHelper>();

            {
                if ((this._behavior & PlateElementBehaviour.Bending) != 0)
                {
                    helpers.Add(new DktHelper());
                }

                if ((this._behavior & PlateElementBehaviour.Membrane) != 0)
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
        public override Matrix GetGlobalDampingMatrix()
        {
            var local = GetLocalDampMatrix();
            var tr = GetTransformationManager();

            var buf = tr.TransformLocalToGlobal(local);

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
        public override Matrix GetGlobalMassMatrix()
        {
            var local = GetLocalMassMatrix();
            var tr = GetTransformationManager();

            var buf = tr.TransformLocalToGlobal(local);

            return buf;
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
        public override Matrix GetGlobalStifnessMatrix()
        {
            var local = GetLocalStifnessMatrix();
            var tr = GetTransformationManager();

            var buf = tr.TransformLocalToGlobal(local);

            return buf;
        }
        #endregion

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

        #region stress
        /// <summary>
        /// Gets the internal stress at defined location.
        /// tensor is in local coordinate system. 
        /// </summary>
        /// <param name="isoLocation">The location for the stress probe in iso coordinates. Order: x,y,z. Maximum bending stress at the shell thickness (z=1.0). Must be withing 0 and 1.</param>
        /// <param name="combination">The load combination.</param>
        /// <param name="probeLocation">The probe location for the stress.</param>
        /// <returns>Stress tensor of flat shell, in local coordination system</returns>
        /// <remarks>
        /// For more info about local coordinate of flat shell see page [72 of 166] (page 81 of pdf) of "Development of Membrane, Plate and Flat Shell Elements in Java" thesis by Kaushalkumar Kansara freely available on the web
        /// </remarks>
        public FlatShellStressTensor GetInternalStress(double[] isoLocation, LoadCombination combination, SectionPoints probeLocation)
        {
            if (isoLocation[2] < 0 || isoLocation[2] > 1.0)
            {
                throw new Exception("z must be between 0 and 1. 0 is the centre of the plate and 1 is on the plate surface. Use the section points to get the top/bottom.") { };
            }
            var helpers = GetHelpers();

            var buf = new FlatShellStressTensor();
            for (var i = 0; i < helpers.Count(); i++)
            {
                if (helpers[i] is CstHelper)
                {
                    buf.MembraneTensor = ((CstHelper)helpers[i]).GetMembraneInternalStress(this, combination);
                }
                else if (helpers[i] is DktHelper)
                {
                    buf.BendingTensor = ((DktHelper)helpers[i]).GetBendingInternalStress(this, combination, new double[] { isoLocation[0], isoLocation[1] }).BendingTensor;
                }
            }
            buf.UpdateTotalStress(_section.GetThicknessAt(new double[] { isoLocation[0], isoLocation[1] }) * isoLocation[2], probeLocation);
            return buf;
        }

        /// <summary>
        /// Gets the internal stress at defined <see cref="isoLocation"/>.
        /// tensor is in local coordinate system. 
        /// </summary>
        /// <param name="loadCase">the load case </param>
        /// <param name="isoLocation"></param>
        /// <returns>Stress tensor of flat shell, in local coordination system</returns>
        /// <remarks>
        /// for more info about local coordinate of flat shell see page [72 of 166] (page 81 of pdf) of "Development of Membrane, Plate and Flat Shell Elements in Java" thesis by Kaushalkumar Kansara freely available on the web
        /// </remarks>
        public CauchyStressTensor GetLocalInternalStress(LoadCase loadCase, double[] isoLocation)
        {
            var helpers = GetHelpers();

            var gst = new GeneralStressTensor();
            var tr = this.GetTransformationManager();

            var ld = this.Nodes.Select(i => tr.TransformGlobalToLocal(i.GetNodalDisplacement(loadCase))).ToArray();

            for (var i = 0; i < helpers.Count(); i++)
            {
                var st = helpers[i].GetLocalInternalStressAt(this, ld, isoLocation);
                gst += st;
            }

            var buf = new CauchyStressTensor();

            buf += gst.MembraneTensor;

            {
                //step2: update Cauchy based on bending,
                //bending tensor also affects the Cauchy tensor regarding how much distance between desired location and center of plate.

                //old code: buf.UpdateTotalStress(_section.GetThicknessAt(new double[] { localX, localY }) * localZ, probeLocation);

                var lambda = 0.0;

                if (isoLocation.Length == 3)
                    lambda = isoLocation[2];
                if (lambda > 1.0 || lambda < -1.0)
                {
                    throw new Exception("lambda must be between -1 and +1") { };
                }
                var thickness = Section.GetThicknessAt(isoLocation);

                //var z = thickness * lambda;//distance from plate center, measure in [m]
                if (lambda > 0)
                {
                    //top -> add bending stress
                    buf += gst.BendingTensor.ConvertBendingStressToCauchyTensor(thickness, lambda);
                }
                else
                {
                    //bottom -> subtract bending stress
                    buf -= gst.BendingTensor.ConvertBendingStressToCauchyTensor(thickness, lambda);
                }
            }
            return buf;
        }
        #endregion
    }
}
