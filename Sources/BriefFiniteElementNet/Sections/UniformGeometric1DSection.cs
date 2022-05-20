using BriefFiniteElementNet.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace BriefFiniteElementNet.Sections
{
    /// <summary>
    /// Represents a uniform section for <see cref="BarElement"/> which defines section's geometry as a polygon
    /// </summary>
    /// <remarks>
    /// </remarks>
    [Serializable]
    public class UniformGeometric1DSection : Base1DSection
    {
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_geometry", _geometry);
            info.AddValue("_jOverride", _jOverride);
            info.AddValue("_resetCentroid", _resetCentroid);

            base.GetObjectData(info, context);
        }

        protected UniformGeometric1DSection(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _geometry = (PointYZ[])info.GetValue("_geometry", typeof(PointYZ[]));
            _jOverride = (double)info.GetValue("_jOverride", typeof(double));
            _resetCentroid = (bool)info.GetValue("_resetCentroid", typeof(bool));
        }

        public UniformGeometric1DSection()
        {
        }

        public UniformGeometric1DSection(PointYZ[] geometry)
        {
            _geometry = geometry;
        }

        public UniformGeometric1DSection(PointYZ[] geometry,double jOverride)
        {
            _geometry = geometry;
            _jOverride = jOverride;
        }

        PointYZ[] _geometry;

        /// <summary>
        /// Gets or sets the Geometry.
        /// </summary>
        /// <value>
        /// The _geometry of section.
        /// </value>
        public PointYZ[] Geometry
        {
            get { return _geometry; }
            set { _geometry = value; }
        }


        /// <summary>
        /// If sets to true, all geometric properties are calculated based on centroid, not coordination system origins.
        /// for more info see pull request #34
        /// </summary>
        /// <remarks>
        /// If set to true, then all properties are calculated based on centroid, not coordination system origins. In this case <see cref="Qy"/> and <see cref="Qz"/> are zeros,
        /// <see cref="Iy"/> and <see cref="Iz"/> and <see cref="Iyz"/> are calculated based on the section centroid. <see cref="A"/> do not change.
        /// </remarks>
        public bool ResetCentroid
        {
            get { return _resetCentroid; }
            set
            {
                _resetCentroid = value;
            }
        }

        /// <summary>
        /// Gets or sets the overrided value for torsion constant J.
        /// </summary>
        /// <remarks>
        /// As there is no (simple) analytic solution to find torsion constant for non circular sections, this should be manually set.
        /// If <see cref="JOverride"/> is set to positive value, then value is used in torsion stiffness matrix.
        /// if <see cref="JOverride"/> is set to zero (by default) or negative value, then polar moment of area is used in torsion stiffness matrix.
        /// </remarks>
        public double JOverride
        {
            get
            {
                return _jOverride;
            }

            set
            {
                _jOverride = value;
            }
        }

        private bool _resetCentroid = true;
        private double _jOverride;

        public override _1DCrossSectionGeometricProperties GetCrossSectionPropertiesAt(double xi)
        {
            var buf = _1DCrossSectionGeometricProperties.Calculate(this.Geometry, this._resetCentroid);

            if (_jOverride > 0)
                buf.J = _jOverride;
            else
                buf.J = buf.Jx;

            return buf;
        }


        public override int[] GetMaxFunctionOrder()
        {
            return new int[] { 0, 0, 0 };
        }

    }
}
