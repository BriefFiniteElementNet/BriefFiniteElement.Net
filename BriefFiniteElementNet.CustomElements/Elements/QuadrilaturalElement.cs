using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Materials;
using BriefFiniteElementNet.Sections;
using System.Runtime.Serialization;
using System.Security.Permissions;

using BriefFiniteElementNet.ElementHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet.Elements
{
    [Serializable]
    [Obsolete("not fully implemented yet")]
    public class QuadrilaturalElement : Element
    {

        #region properties
        private BaseMaterial _material;

        private Base2DSection _section;

        private TriangleElementBehaviour _behavior;

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

        public TriangleElementBehaviour Behavior
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

        public override Matrix GetGlobalStifnessMatrix()
        {
            throw new NotImplementedException();
        }

        public override IElementHelper[] GetHelpers()
        {
            var helpers = new List<IElementHelper>();

            {
                if ((this._behavior & TriangleElementBehaviour.Bending) != 0)
                {
                    helpers.Add(new DkqHelper());
                }

                if ((this._behavior & TriangleElementBehaviour.Membrane) != 0)
                {
                    //helpers.Add(new CstHelper());
                    throw new NotImplementedException();
                }

                if ((this._behavior & TriangleElementBehaviour.DrillingDof) != 0)
                {
                    helpers.Add(new TriangleBasicDrillingDofHelper());
                }
            }


            return helpers.ToArray();
        }

        public override Matrix GetLambdaMatrix()
        {
            throw new NotImplementedException();
        }

        public override double[] IsoCoordsToLocalCoords(params double[] isoCoords)
        {
            throw new NotImplementedException();
        }
    }
}
