//---------------------------------------------------------------------------------------
//
// Project: VIT-V
//
// Program: BriefFiniteElement.Net - QuadrilaturalElement.cs
//
// Revision History
//
// Date          Author          	            Description
// 11.06.2020    T.Thaler, M.Mischke     	    v1.0  
// 
//---------------------------------------------------------------------------------------
// Copyleft 2017-2020 by Brandenburg University of Technology. Intellectual proprerty 
// rights reserved in terms of LGPL license. This work is a research work of Brandenburg
// University of Technology. Use or copying requires an indication of the authors reference.
//---------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using System.Runtime.Serialization;
using System.Security.Permissions;
using BriefFiniteElementNet.Elements.ElementHelpers;

namespace BriefFiniteElementNet.Elements
{
    [Serializable]
    [Obsolete("not fully implemented yet")]
    public class QuadrilaturalElement : Element     // based on the BFE.Net TriangleElement-Class
    {

        #region properties
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

        public QuadrilaturalElement() : base(4)
        {
        }

        public override Matrix GetGlobalDampingMatrix()
        {
            var local = GetLocalDampMatrix();
            var tr = GetTransformationManager();

            var buf = tr.TransformLocalToGlobal(local);

            return buf;
        }
        public Matrix GetLocalDampMatrix()
        {
            var helpers = new List<IElementHelper>();

            if ((this._behavior & PlateElementBehaviour.Bending) != 0)
            {
                helpers.Add(new DkqHelper());
            }

            var buf = new Matrix(24, 24);

            for (var i = 0; i < helpers.Count; i++)
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
        public override Force[] GetGlobalEquivalentNodalLoads(ElementalLoad load)
        {
            throw new NotImplementedException();
        }

        public override Matrix GetGlobalMassMatrix()
        {
            throw new NotImplementedException();
        }

        public override Matrix GetGlobalStifnessMatrix()    // made by epsi1on
        {
            var local = GetLocalStifnessMatrix();
            var tr = GetTransformationManager();

            var buf = tr.TransformLocalToGlobal(local);

            return buf;
        }
        public Matrix GetLocalMassMatrix()
        {
            var helpers = GetHelpers();

            var buf = new Matrix(24, 24);

            for (var i = 0; i < helpers.Count(); i++)
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
        public Matrix GetLocalStifnessMatrix()  
        {
            var helpers = GetHelpers();

            var buf = new Matrix(24, 24); // 6*nodecount x 6*nodecount

            for (var i = 0; i < helpers.Length; i++)    
            {
                var helper = helpers[i];

                var ki = helper.CalcLocalStiffnessMatrix(this);

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

        public override IElementHelper[] GetHelpers()
        {
            var helpers = new List<IElementHelper>();

            {
                if ((this._behavior & PlateElementBehaviour.Bending) != 0)
                {
                    helpers.Add(new DkqHelper());
                }

                if ((this._behavior & PlateElementBehaviour.Membrane) != 0)
                {
                    helpers.Add(new Q4MembraneHelper());
                }

                
                if ((this._behavior & PlateElementBehaviour.DrillingDof) != 0)
                {
                    helpers.Add(new QuadBasicDrillingDofHelper());
                }
                
            }

            return helpers.ToArray();
        }

        public override Matrix GetLambdaMatrix()
        {
            var p1 = nodes[0].Location;
            var p2 = nodes[1].Location;
            var p3 = nodes[2].Location;
            var p4 = nodes[3].Location;

            var v1 = p1 - Point.Origins;
            var v2 = p2 - Point.Origins;
            var v3 = p3 - Point.Origins;
            var v4 = p4 - Point.Origins;

            var ii = (v1 + v2) / 2.0; // midpoints
            var jj = (v2 + v3) / 2.0;
            var kk = (v3 + v4) / 2.0;
            var ll = (v4 + v1) / 2.0;

            var vx = (jj - ll).GetUnit();//ref [1] eq. 5.8
            var vr = (kk - ii).GetUnit();//eq. 5.10
            var vz = Vector.Cross(vx, vr);//eq. 5.11
            var vy = Vector.Cross(vz, vx);//eq. 5.12

            var lamX = vx.GetUnit();//Lambda_X -> eq. 5.9
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
            double N1 = 0.25 * (1 - isoCoords[0]) * (1 - isoCoords[1]);
            double N2 = 0.25 * (1 + isoCoords[0]) * (1 - isoCoords[1]);
            double N3 = 0.25 * (1 + isoCoords[0]) * (1 + isoCoords[1]);
            double N4 = 0.25 * (1 - isoCoords[0]) * (1 + isoCoords[1]);
            double x = Nodes[0].Location.X * N1 + Nodes[1].Location.X * N2 + Nodes[2].Location.X * N3 + Nodes[3].Location.X * N4;
            double y = Nodes[0].Location.Y * N1 + Nodes[1].Location.Y * N2 + Nodes[2].Location.Y * N3 + Nodes[3].Location.Y * N4;

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
        #region Deserialization Constructor

        protected QuadrilaturalElement(SerializationInfo info, StreamingContext context) : base(info, context)
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

        #region stresses
        /// <summary>
        /// Gets the internal stress at defined location.
        /// tensor is in local coordinate system. 
        /// </summary>
        /// <param name="localX">The X in local coordinate system (see remarks).</param>
        /// <param name="localY">The Y in local coordinate system (see remarks).</param>
        /// <param name="combination">The load combination.</param>
        /// <param name="probeLocation">The probe location for the stress.</param>
        /// <param name="localZ">The location for the bending stress. Maximum at the shell thickness (1). Must be withing 0 and 1</param>
        /// <returns>Stress tensor of flat shell, in local coordination system</returns>
        /// <remarks>
        /// for more info about local coordinate of flat shell see page [72 of 166] (page 81 of pdf) of "Development of Membrane, Plate and Flat Shell Elements in Java" thesis by Kaushalkumar Kansara freely available on the web
        /// </remarks>
        public FlatShellStressTensor GetInternalStress(double localX, double localY, double localZ, LoadCombination combination, SectionPoints probeLocation)
        {
            if (localZ < 0 || localZ > 1.0)
            {
                throw new Exception("z must be between 0 and 1. 0 is the centre of the plate and 1 is on the plate surface. Use the section points to get the top/bottom.") { };
            }
            var helpers = GetHelpers();

            var buf = new FlatShellStressTensor();
            for (var i = 0; i < helpers.Count(); i++)
            {
                if (helpers[i] is Q4MembraneHelper)
                {
                    buf.MembraneTensor = ((Q4MembraneHelper)helpers[i]).GetLocalInternalStress(this, combination, new double[] { localX, localY }).MembraneTensor;
                }
                else if (helpers[i] is DkqHelper)
                {
                    buf.BendingTensor = ((DkqHelper)helpers[i]).GetBendingInternalStress(this, combination, new double[] { localX, localY }).BendingTensor;
                }
            }
            buf.UpdateTotalStress(_section.GetThicknessAt(new double[] { localX, localY }) * localZ, probeLocation);
            return buf;
        }
        #endregion
    }
}
