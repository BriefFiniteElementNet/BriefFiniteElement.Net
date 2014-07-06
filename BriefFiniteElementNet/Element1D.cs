using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BriefFiniteElementNet
{
    public abstract class Element1D : Element
    {
        protected Element1D(int nodes) : base(nodes)
        {
        }

        protected double e;
        protected double g;

        /// <summary>
        /// Gets or sets the e.
        /// </summary>
        /// <value>
        /// The elastic modulus of member
        /// </value>
        public double E
        {
            get { return e; }
            set { e = value; }
        }

        /// <summary>
        /// Gets or sets the g.
        /// </summary>
        /// <value>
        /// The shear modulus of member
        /// </value>
        public double G
        {
            get { return g; }
            set { g = value; }
        }

        /// <summary>
        /// Gets the internal force at <see cref="x"/> position.
        /// </summary>
        /// <param name="x">The position (from start point).</param>
        /// <param name="cmb">The <see cref="LoadCombination"/>.</param>
        /// <remarks>Will calculate the internal forces of member regarding the <see cref="cmb"/> <see cref="LoadCombination"/>
        /// </remarks>
        /// <returns></returns>
        public abstract Force GetInternalForceAt(double x, LoadCombination cmb);

        /// <summary>
        /// Gets the internal force at.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <remarks>Will calculate the internal forces of member regarding Default load case (default load case means a load case where <see cref="LoadCase.LoadType"/> is equal to <see cref="LoadType.Default"/> and <see cref="LoadCase.CaseName"/> is equal to null)</remarks>
        /// <returns></returns>
        public abstract Force GetInternalForceAt(double x);
    }
}
