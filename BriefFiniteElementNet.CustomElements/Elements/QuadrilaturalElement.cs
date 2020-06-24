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
            throw new NotImplementedException();
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

            var ii = (v1 + v2) / 2; // midpoints
            var jj = (v2 + v3) / 2;
            var kk = (v3 + v4) / 2;
            var ll = (v4 + v1) / 2;

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
            throw new NotImplementedException();
        }
    }
}
