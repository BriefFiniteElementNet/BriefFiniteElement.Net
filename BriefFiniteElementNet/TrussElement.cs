using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    public class TrussElement : Element1D
    {
        #region Members

        private double _a;

        /// <summary>
        /// Gets or sets a.
        /// </summary>
        /// <value>
        /// The area of section in m^2
        /// </value>
        public double A
        {
            get { return _a; }
            set { _a = value; }
        }

        private bool useOverridedProperties=true;

        /// <summary>
        /// Gets or sets a value indicating whether [use overrided properties].
        /// </summary>
        /// <value>
        /// <c>true</c> if overrided properties (<see cref="A"/>,<see cref="A"/>) should be taken into account for calculations such as stiffness matrix calculation ];
        /// <c>false</c> if geomtric properties of section should be calculated from <see cref="Geometry"/> Property of <see cref="TrussElement"/>.
        /// </value>
        public bool UseOverridedProperties
        {
            get { return useOverridedProperties; }
            set { useOverridedProperties = value; }
        }

        private PolygonYz geometry;

        /// <summary>
        /// Gets or sets the geometry.
        /// </summary>
        /// <value>
        /// The geometry of section.
        /// </value>
        /// <remarks>
        /// When <see cref="UseOverridedProperties"/> is setted to <c>false</c>, geometric properties of section of <see cref="FrameElement2Node"/> such as Area and Second moments of area etc. will be calculated with regard to this geometry.
        /// </remarks>
        public PolygonYz Geometry
        {
            get { return geometry; }
            set { geometry = value; }
        }

        #endregion


        public TrussElement(Node start,Node end) : base(2)
        {
            this.nodes[0] = start;
            this.nodes[1] = end;
        }

        public override Force GetInternalForceAt(double x, LoadCombination cmb)
        {
            throw new NotImplementedException();
        }

        public override Force GetInternalForceAt(double x)
        {
            throw new NotImplementedException();
        }

        public override Matrix GetGlobalStifnessMatrix()
        {
            var v = this.EndNode.Location - this.StartNode.Location;

            var l = v.Length;
            var cx = v.X/l;
            var cy = v.Y/l;
            var cz = v.Z/l;

            var buf = new Matrix(12, 12);

            buf[0 + 0, 0] = cx * cx; buf[0 + 0, 1] = cx * cy; buf[0 + 0, 2] = cx * cz;
            buf[1 + 0, 0] = cx * cy; buf[1 + 0, 1] = cy * cy; buf[1 + 0, 2] = cy * cz;
            buf[2 + 0, 0] = cx * cz; buf[2 + 0, 1] = cy * cz; buf[2 + 0, 2] = cz * cz;



            buf[0 + 6, 0] = -cx * cx; buf[0 + 6, 1] = -cx * cy; buf[0 + 6, 2] = -cx * cz;
            buf[1 + 6, 0] = -cx * cy; buf[1 + 6, 1] = -cy * cy; buf[1 + 6, 2] = -cy * cz;
            buf[2 + 6, 0] = -cx * cz; buf[2 + 6, 1] = -cy * cz; buf[2 + 6, 2] = -cz * cz;



            buf[0, 0 + 6] = -cx * cx; buf[0, 1 + 6] = -cx * cy; buf[0, 2 + 6] = -cx * cz;
            buf[1, 0 + 6] = -cx * cy; buf[1, 1 + 6] = -cy * cy; buf[1, 2 + 6] = -cy * cz;
            buf[2, 0 + 6] = -cx * cz; buf[2, 1 + 6] = -cy * cz; buf[2, 2 + 6] = -cz * cz;


            buf[0 + 6, 0 + 6] = cx * cx; buf[0 + 6, 1 + 6] = cx * cy; buf[0 + 6, 2 + 6] = cx * cz;
            buf[1 + 6, 0 + 6] = cx * cy; buf[1 + 6, 1 + 6] = cy * cy; buf[1 + 6, 2 + 6] = cy * cz;
            buf[2 + 6, 0 + 6] = cx * cz; buf[2 + 6, 1 + 6] = cy * cz; buf[2 + 6, 2 + 6] = cz * cz;

            if (!useOverridedProperties)
                throw new NotImplementedException();

            var cf = this.e*this._a/l;

            for (int i = 0; i < 144; i++)
            {
                buf.CoreArray[i] *= cf;
            }

            return buf;
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets or sets the start node.
        /// </summary>
        /// <value>
        /// The start node of <see cref="TrussElement"/>.
        /// </value>
        public Node StartNode
        {
            get { return nodes[0]; }
            set { nodes[0] = value; }
        }

        /// <summary>
        /// Gets or sets the end node.
        /// </summary>
        /// <value>
        /// The end node of <see cref="TrussElement"/>.
        /// </value>
        public Node EndNode
        {
            get { return nodes[1]; }
            set { nodes[1] = value; }
        }

    }
}
