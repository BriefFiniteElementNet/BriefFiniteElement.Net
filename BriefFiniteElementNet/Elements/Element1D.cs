using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using BriefFiniteElementNet.ElementHelpers;
using BriefFiniteElementNet.Elements;

namespace BriefFiniteElementNet
{
    [Serializable]
    public abstract class Element1D : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Element1D"/> class.
        /// </summary>
        /// <param name="nodes">The number of nodes that the <see cref="Element" /> connect together.</param>
        protected Element1D(int nodes) : base(nodes)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Element1D"/> class.
        /// </summary>
        [Obsolete("use Element1D(int)")]
        protected Element1D():base()
        {
        }
        

        protected double e;
        protected double g;

        /// <summary>
        /// Gets or sets the elastic module of section.
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
        /// Gets or sets the shear module of section.
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


        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("g", g);
            info.AddValue("e", e);
            base.GetObjectData(info, context);
        }

        public override Matrix GetLambdaMatrix()
        {
            throw new NotImplementedException();
        }

        public override IElementHelper[] GetHelpers()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Element1D"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected Element1D (SerializationInfo info, StreamingContext context):base(info,context)
        {
            g = info.GetDouble("g");
            e = info.GetDouble("e");
        }
    }
}
