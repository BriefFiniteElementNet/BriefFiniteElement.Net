using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;



namespace BriefFiniteElementNet.Sections
{
    /// <summary>
    /// Represents a Uniform Parametric 2D section
    /// </summary>
    /// <remarks>
    /// Uniform Parametric 2D Section means a uniform or constant thickness secition that thickness parameter is defined in the <see cref="T"/> member of class.
    /// </remarks>
    /// <seealso cref="BriefFiniteElementNet.Sections.Base2DSection" />
    [Serializable]
    public class UniformParametric2DSection : Base2DSection
    {
        private double _t;

        /// <summary>
        /// Gets or sets the Thickness.
        /// </summary>
        /// <value>
        /// The thickness.
        /// </value>
        public double T
        {
            get { return _t; }
            set { _t = value; }
        }


        /// <summary>
        /// Gets the thickness of section at specified coordinates.
        /// </summary>
        /// <param name="isoCoords">The iso coords.</param>
        /// <returns>thicness of section</returns>
        public override double GetThicknessAt(params double[] isoCoords)
        {
            return T;
        }

        public override int[] GetMaxFunctionOrder()
        {
            return new[] {0, 0, 0};
        }


        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_t", _t);
            base.GetObjectData(info, context);
        }

        protected UniformParametric2DSection(SerializationInfo info, StreamingContext context):base(info,context)
        {
            _t = info.GetDouble("_t");
        }

        public UniformParametric2DSection() : base()
        {

        }


        public UniformParametric2DSection(double t)
        {
            _t = t;
        }
    }
}
